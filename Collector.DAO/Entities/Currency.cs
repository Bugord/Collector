using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.DAO.Entities
{
    public class Currency : BaseEntity
    {
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
