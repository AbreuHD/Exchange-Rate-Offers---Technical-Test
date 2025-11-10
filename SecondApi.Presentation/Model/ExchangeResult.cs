using System.Runtime.Serialization;

namespace SecondApi.Presentation.Model
{
    [DataContract]
    public class ExchangeResult
    {
        [DataMember(Order = 1)]
        public string From { get; set; } = string.Empty;

        [DataMember(Order = 2)]
        public string To { get; set; } = string.Empty;

        [DataMember(Order = 3)]
        public decimal Rate { get; set; }

        [DataMember(Order = 4)]
        public decimal OriginalAmount { get; set; }

        [DataMember(Order = 5)]
        public decimal ConvertedAmount { get; set; }

        [DataMember(Order = 6)]
        public string Message { get; set; } = string.Empty;
    }
}
