using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Collections.Generic;

namespace Mrxrm.D365.CurrencyUpdate.AzureFunction
{
    public static class TimerFunction
    {
        [FunctionName("TimerTriggerUpdateForex")]
        public static void Run([TimerTrigger("0 50 2,8,14,20 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            //log.Info(System.Configuration.ConfigurationManager.AppSettings["ExchangeApiUrl"]);

            ICurrencyExchanger currencyExchanger = new CurrencyExchanger(log);
            ICrmProxy crmProxy = new CrmProxy(log);
            Main(currencyExchanger, crmProxy, log);
        }

        public static void Main(ICurrencyExchanger currencyEx, ICrmProxy crmProxy, TraceWriter log)
        {
            var rates = currencyEx.GetExchangeRates();
            if (rates == null || rates.Count < 1)
            {
                log.Warning("Did not receive any exchange rates from Open Exchange Rate web API.");
            }
            
            crmProxy.UpdateExchangeRates(rates);
        }
    }
}