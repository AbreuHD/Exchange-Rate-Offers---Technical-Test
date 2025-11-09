namespace Core.Application.Dtos
{
    public class ExchangeResponse
    {
        public string ProviderName { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
