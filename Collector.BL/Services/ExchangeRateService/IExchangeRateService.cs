using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Collector.DAO.Entities;

namespace Collector.BL.Services.ExchangeRateService
{
    public interface IExchangeRateService
    {
        Task UpdateExchangeRatesAsync();
        Task<CurrencyExchange> GetExchangeRatesAsync();
        Task<decimal> Convert(decimal value, Currency currencyFrom, Currency currencyTo);
    }
}
