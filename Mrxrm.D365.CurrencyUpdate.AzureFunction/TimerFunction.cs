using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Mrxrm.D365.CurrencyUpdate.AzureFunction
{
    public static class TimerFunction
    {
        [FunctionName("TimerTriggerUpdateForex")]
        public static void Run([TimerTrigger("20 0 2,8,14,20 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            //log.Info(System.Configuration.ConfigurationManager.AppSettings["ExchangeApiUrl"]);

            ICurrencyExchanger currencyExchanger = new CurrencyExchanger(log);
            Main(currencyExchanger);
        }

        public static void Main(ICurrencyExchanger currencyEx)
        {
            currencyEx.GetExchangeRates();
        }
    }
}