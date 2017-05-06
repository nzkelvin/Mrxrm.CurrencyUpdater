namespace Mrxrm.D365.AzureWebJob.CurrencyUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

#pragma warning disable SA1649 // File name must match first type name
    public interface ICurrencyExchanger
#pragma warning restore SA1649 // File name must match first type name
    {
        Dictionary<string, decimal> GetExchangeRates();
    }

    public class CurrencyExchanger : ICurrencyExchanger
    {
        private readonly string _exchangeApiUrl;

        public CurrencyExchanger()
        {
            this._exchangeApiUrl = System.Configuration.ConfigurationManager.AppSettings["ExchangeApiUrl"];
        }

        public Dictionary<string, decimal> GetExchangeRates()
        {
            Dictionary<string, decimal> exchangeRates = Task.Run(new Func<Task<Dictionary<string, decimal>>>(async () =>
                {
                    HttpClient client = new HttpClient();
                    var response = await client.GetAsync(this._exchangeApiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        JObject jResult = JObject.Parse(result);
                        var rates = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(jResult["rates"].ToString());

                        return rates;
                    }

                    return null;
                }))
                .GetAwaiter()
                .GetResult();

            return exchangeRates;
        }
    }
}