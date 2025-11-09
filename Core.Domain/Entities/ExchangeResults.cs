namespace Core.Domain.Entities
{
    public class ExchangeResults
    {
        public string ProviderName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
