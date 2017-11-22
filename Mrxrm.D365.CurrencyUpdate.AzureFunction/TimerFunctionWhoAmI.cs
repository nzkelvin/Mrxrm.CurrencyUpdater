using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Mrxrm.D365.CurrencyUpdate.AzureFunction
{
    public static class TimerFunctionWhoAmI
    {
        [FunctionName("TimerFunction.WhoAmI")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
