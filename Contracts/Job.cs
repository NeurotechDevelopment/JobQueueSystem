using Contracts.Payloads;

namespace Contracts
{
    public record Job : JobBase
    {
        public Guid JobId { get; set; }

        public JobPayload? RequestPayload { get; set; }

        public JobResult? ResultPayload { get; set; }
    }
}
