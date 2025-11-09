namespace Core.Domain.Entities
{
    public class ExchangeResults
    {
        public string ProviderName { get; set; } = string.Empty;
        public required string ExchangePair { get; set; }
        public decimal Rate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
