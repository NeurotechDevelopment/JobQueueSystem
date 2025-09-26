namespace Contracts.Payloads.Responses
{
    public record SimpleMessagePayload
    {
        public SimpleMessagePayload(string message)
        {
            Message = message;
        }

        public string? Message { get; set; }
    }
}
