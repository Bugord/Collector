using Collector.BL.Models.Debt;
using Collector.DAO.Entities;

namespace Collector.BL.Extentions
{
    public static class CurrencyExtentions
    {
        public static CurrencyReturnDTO ToCurrencyReturnDTO(this Currency currency, float rate)
        {
            return new CurrencyReturnDTO
            {
                Id = currency.Id,
                CurrencyName = currency.CurrencyName,
                CurrencySymbol = currency.CurrencySymbol,
                Rate = rate
            };
        }
    }
}
