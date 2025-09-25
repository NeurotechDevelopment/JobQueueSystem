namespace Contracts
{
    public record Job : JobInfo
    {
        public JobPayload? RequestPayload { get; set; }

        public JobPayload? ResultPayload { get; set; }
    }
}
