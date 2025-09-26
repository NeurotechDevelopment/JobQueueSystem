using Contracts.Payloads;

namespace Contracts
{
    public record Job : JobInfo
    {
        public JobPayload? RequestPayload { get; set; }

        public JobResult? ResultPayload { get; set; }
    }
}
