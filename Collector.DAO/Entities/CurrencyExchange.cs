using System.Collections.Generic;

namespace Collector.DAO.Entities
{
    public class CurrencyExchange : BaseEntity
    {
        public ICollection<CurrencyRate> CurrencyRates { get; set; }
    }
}