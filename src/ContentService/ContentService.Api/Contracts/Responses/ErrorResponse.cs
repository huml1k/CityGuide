namespace ContentService.Api.Contracts.Responses
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }

        public string Message { get; set; } = string.Empty;

        public IEnumerable<string>? Details { get; set; }

        public string? TraceId { get; set; }
    }
}
