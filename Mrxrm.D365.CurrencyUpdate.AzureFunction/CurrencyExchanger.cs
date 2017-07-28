using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mrxrm.D365.CurrencyUpdate.AzureFunction
{
    public interface ICurrencyExchanger
    {
        Dictionary<string, decimal> GetExchangeRates();
    }

    public class CurrencyExchanger : ICurrencyExchanger
    {
        private readonly string _exchangeApiUrl;
        private TraceWriter _log;

        public CurrencyExchanger(TraceWriter log)
        {
            this._log = log;
            this._exchangeApiUrl = System.Configuration.ConfigurationManager.AppSettings["ExchangeApiUrl"];
            log.Info($"ExchangeApiUrl: {_exchangeApiUrl}");
        }

        public Dictionary<string, decimal> GetExchangeRates()
        {
            Dictionary<string, decimal> exchangeRates = Task.Run(new Func<Task<Dictionary<string, decimal>>>(async () =>
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(this._exchangeApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    _log.Error($"Request to the Exchange API was successful.");
                    var result = await response.Content.ReadAsStringAsync();
                    JObject jResult = JObject.Parse(result);
                    var rates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jResult["rates"].ToString());

                    return new Dictionary<string, decimal>(rates, StringComparer.InvariantCultureIgnoreCase);
                }
                else
                {
                    _log.Error($"Request to the Exchange API was not successful. Status Code: {response.StatusCode.ToString()}.");
                }

                return null;
            }))
            .GetAwaiter()
            .GetResult();

            return exchangeRates;
        }
    }
}
