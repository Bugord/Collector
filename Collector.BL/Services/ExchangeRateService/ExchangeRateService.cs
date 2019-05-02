using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Collector.BL.Exceptions;
using Collector.DAO.Entities;
using Collector.DAO.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Collector.BL.Services.ExchangeRateService
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRepository<CurrencyExchange> _currencyExchangeRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IConfiguration _configuration;

        public ExchangeRateService(IRepository<CurrencyExchange> currencyExchangeRepository,
            IRepository<User> userRepository, IRepository<Currency> currencyRepository,
            IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _currencyExchangeRepository = currencyExchangeRepository;
            _userRepository = userRepository;
            _currencyRepository = currencyRepository;
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        public async Task UpdateExchangeRatesAsync()
        {
            if (await _currencyExchangeRepository.ExistsAsync(exchange => exchange.Created.Date == DateTime.Now.Date))
                return;

            var client = _clientFactory.CreateClient();

            var currencies = await _currencyRepository.GetAllAsync();

            var urlBuilder = new UriBuilder(_configuration.GetSection("openExchangeRates")["Url"]);
            var query = HttpUtility.ParseQueryString(urlBuilder.Query);
            query["app_id"] = _configuration.GetSection("openExchangeRates")["AppId"];
            query["base"] = currencies.First().CurrencySymbol;
            query["symbols"] = string.Join(",", currencies.Select(currency => currency.CurrencySymbol));
            urlBuilder.Query = query.ToString();
            var url = urlBuilder.ToString();

            var response = await client.GetAsync(url);
            var responseObject = JsonConvert.DeserializeObject<Response>(await response.Content.ReadAsStringAsync());

            var systemUser = await _userRepository.GetFirstAsync(user => user.Username == "system");
            if (systemUser == null)
                throw new ServerFailException("System user not exist");


            var exchangeRates = new List<CurrencyRate>();

            foreach (var key in responseObject.Rates.Keys)
            {
                var currency = currencies.FirstOrDefault(c =>
                    c.CurrencySymbol.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (currency == null)
                    continue;

                exchangeRates.Add(new CurrencyRate
                {
                    CreatedBy = systemUser.Id,
                    Currency = currency,
                    Rate = responseObject.Rates[key]
                });
            }

            var exchangeRate = new CurrencyExchange
            {
                CreatedBy = systemUser.Id,
                CurrencyRates = exchangeRates
            };

            await _currencyExchangeRepository.InsertAsync(exchangeRate);
        }

        public async Task<CurrencyExchange> GetExchangeRatesAsync()
        {
            await UpdateExchangeRatesAsync();

            var exchangeRate =
                await (await _currencyExchangeRepository.GetAllAsync(
                        exchange => exchange.Created.Date == DateTime.Today))
                    .Include(exchange => exchange.CurrencyRates)
                    //.ThenInclude(rate => rate.Currency)
                    .FirstOrDefaultAsync();

            return exchangeRate;
        }

        public async Task<decimal> Convert(decimal value, Currency currencyFrom, Currency currencyTo)
        {
            if (currencyFrom.CurrencySymbol == currencyTo.CurrencySymbol)
                return value;

            await UpdateExchangeRatesAsync();

            var rates = (await (await _currencyExchangeRepository.GetAllAsync(exchange =>
                        exchange.Created.Date == DateTime.Today)).Include(exchange => exchange.CurrencyRates)
                    .ThenInclude(collection => collection.Currency).FirstOrDefaultAsync())
                .CurrencyRates;

            var from = rates.FirstOrDefault(rate => rate.Currency == currencyFrom);
            var to = rates.FirstOrDefault(rate => rate.Currency == currencyTo);

            if (from == null || to == null)
                throw new SqlNullValueException("Currency not founded");

            return value * (decimal)to.Rate / (decimal)from.Rate;
        }
    }


    //public class Rates
    //{
    //    public float Rub { get; set; }
    //    public float Usd { get; set; }
    //    public float Eur { get; set; }
    //}

    public class Response
    {
        public Dictionary<string, float> Rates { get; set; }
    }
}