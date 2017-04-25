using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Mrxrm.D365.AzureWebJob.CurrencyUpdate.Models.D365;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;

namespace Mrxrm.D365.AzureWebJob.CurrencyUpdate
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        // todo: !important! Those two connection strings should be manually added to Azure portal.
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            Console.WriteLine("Start Currency Update");

            //IoC
            var kernel = new Ninject.StandardKernel();
            kernel.Bind<ICurrencyExchanger>().To<CurrencyExchanger>();
            kernel.Bind<IOrganizationService>().ToConstructor(x => new OrganizationService(x.Inject<string>())).WithConstructorArgument("connectionStringName", "Xrm");
            kernel.Bind<ICrmServiceContext>().To<CrmServiceContext>().WithConstructorArgument("service", kernel.Get<IOrganizationService>());

            
            Execute(kernel.Get<ICurrencyExchanger>(), kernel.Get<ICrmServiceContext>());
        }

        public static void Execute(ICurrencyExchanger exchanger, ICrmServiceContext ctx)
        {
            // Get Open exchange rates
            var exchangeRates = exchanger.GetExchangeRates();

            // Nice to have logging code. Indicates if OpenExchange.org is working or not.
            foreach (var rate in exchangeRates)
            {
                Console.WriteLine($"{rate.Key}: {rate.Value}");
            }

            // Get base currency
            var baseIso = ctx.OrganizationSet
                .Join(ctx.TransactionCurrencySet, o => o.BaseCurrencyId.Id, c => c.TransactionCurrencyId,
                    (o, c) => new {o, c})
                .Select(j => j.c.ISOCurrencyCode)
                .First();

            Console.WriteLine($"Base ISO: {baseIso}");

            decimal baseCurrencyRate = exchangeRates[baseIso];

            var currencies = ctx.TransactionCurrencySet
                .Select(c => new TransactionCurrency()
                {
                    TransactionCurrencyId = c.TransactionCurrencyId,
                    ISOCurrencyCode = c.ISOCurrencyCode
                }).ToArray();

            foreach (var c in currencies)
            {
                if (c.ISOCurrencyCode == baseIso) continue;

                var rate = exchangeRates[c.ISOCurrencyCode] / baseCurrencyRate;

                ctx.Execute(new UpdateRequest()
                {
                    Target = new TransactionCurrency()
                    {
                        TransactionCurrencyId = c.TransactionCurrencyId,
                        ExchangeRate = rate
                    }
                });

                Console.WriteLine($"Update D365 currency {c.ISOCurrencyCode} exchange rate to {rate}");
            }
        }
    }
}
