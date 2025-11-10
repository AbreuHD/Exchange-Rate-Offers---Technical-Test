namespace ThirdApi.Presentation.Model
{
    public class Response
    {
        public int StatusCode { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public decimal ExchangeRate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
