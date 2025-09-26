namespace Contracts.Payloads
{
    public record EmptyPayload
    {
        public static readonly EmptyPayload Instance = new EmptyPayload();
    }
}
