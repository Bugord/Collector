namespace Collector.BL.Models.Debt
{
    public class CurrencyReturnDTO
    {
        public long Id { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public float Rate { get; set; }
    }
}
