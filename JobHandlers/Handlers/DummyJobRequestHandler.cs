using Contracts;
using Contracts.Payloads;
using Contracts.Payloads.Responses;
using Shared;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.Dummy)]
    public sealed class DummyJobRequestHandler : JobHandler<SimpleMessagePayload, SimpleMessagePayload>
    {
        public DummyJobRequestHandler(ILogger<DummyJobRequestHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.Dummy;
        
        protected override Task<(SimpleMessagePayload Result, Attachment ResultFile)> PerformWorkAsync(Guid jobId, SimpleMessagePayload? payload, Attachment? requestAttachment)
        {
            var payloadText = payload?.Message ?? null;
            if (payloadText == "GenerateError")
            {
                throw new InvalidOperationException($"This is a dummy error generated on purpose for job {jobId}.");
            }
            return Task.FromResult(( new SimpleMessagePayload($"Dummy service response for job {jobId}. You sent payload {payload}"), (Attachment)null));
        }
    }
}
