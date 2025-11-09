namespace Core.Application.Dtos.Generic
{
    public class GenericApiResponse<DTO>
    {
        public required DTO Payload { get; set; }
        public required int Statuscode { get; set; }
        public required string Message { get; set; }
    }
}
