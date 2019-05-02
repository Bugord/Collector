using System;
using System.Collections.Generic;
using System.Text;

namespace Collector.DAO.Entities
{
    public class CurrencyRate : BaseEntity
    {
        public Currency Currency { get; set; }
        public float Rate { get; set; }
    }
}
