namespace Contracts.Payloads
{
    public record SimpleMessagePayload : JobPayloadBase
    {
        public SimpleMessagePayload(string message)
        {
            Message = message;
        }

        public string? Message { get; set; }
    }
}
