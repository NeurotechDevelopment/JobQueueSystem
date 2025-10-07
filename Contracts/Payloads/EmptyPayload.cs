namespace Contracts.Payloads
{
    public record EmptyPayload : JobPayloadBase
    {
        public static readonly EmptyPayload Instance = new EmptyPayload();
    }
}
