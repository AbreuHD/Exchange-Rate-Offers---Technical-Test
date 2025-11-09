namespace Core.Domain.Common
{
    public class GenericResponse<DTO>
    {
        public DTO? Payload { get; set; }
        public int Statuscode { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
