using System;
using System.Linq;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;
using Mrxrm.D365.CurrencyUpdate.AzureFunction.Models.D365;
using Microsoft.Xrm.Sdk.Messages;

namespace Mrxrm.D365.CurrencyUpdate.AzureFunction
{
    public interface ICrmProxy
    {
        void UpdateExchangeRates(Dictionary<string, decimal> exchangeRates);
    }

    public class CrmProxy : ICrmProxy
    {
        private TraceWriter _log;

        public CrmProxy(TraceWriter log)
        {
            this._log = log;
        }
        public void UpdateExchangeRates(Dictionary<string, decimal> exchangeRates)
        {
            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["Xrm"].ToString();
            var client = new Microsoft.Xrm.Tooling.Connector.CrmServiceClient(connStr);
            using (var ctx = new CrmServiceContext(client))
            {
                // Get the base currency
                var baseCurrencyCode =
                    ctx.TransactionCurrencySet
                    .Join(ctx.OrganizationSet
                            , t => t.TransactionCurrencyId
                            , o => o.BaseCurrencyId.Id
                            , (t, o) => t.ISOCurrencyCode).FirstOrDefault();

                if (baseCurrencyCode == null)
                {
                    _log.Error("Cannot find base currency ISO code.");
                }
                _log.Info($"The base currency ISO code is {baseCurrencyCode}.");

                var baseRate = exchangeRates[baseCurrencyCode];

                // Get all currencies
                var currencies = ctx.TransactionCurrencySet
                                    .Select(t => new TransactionCurrency()
                                    {
                                        TransactionCurrencyId = t.TransactionCurrencyId,
                                        ISOCurrencyCode = t.ISOCurrencyCode
                                    })
                                    .ToArray();

                // Loop through and update the exchange rates
                foreach (var c in currencies)
                {
                    if (String.Equals(c.ISOCurrencyCode, baseCurrencyCode, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    var rate = exchangeRates[c.ISOCurrencyCode] / baseRate;
                    _log.Info($"Updating {exchangeRates[c.ISOCurrencyCode]}: {rate} to D365.");

                    ctx.Execute(new UpdateRequest()
                    {
                        Target = new TransactionCurrency()
                        {
                            TransactionCurrencyId = c.TransactionCurrencyId,
                            ExchangeRate = rate
                        }
                    });
                }
            }
        }
    }
}