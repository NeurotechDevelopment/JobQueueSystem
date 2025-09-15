using Contracts;
using Shared;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.Dummy)]
    public sealed class DummyJobRequestHandler : JobHandler
    {
        public DummyJobRequestHandler(ILogger<DummyJobRequestHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.Dummy;

        protected override Task<string> PerformWorkAsync(Guid jobId, JobPayload payload)
        {
            if (payload.Data == "GenerateError")
            {
                throw new InvalidOperationException($"This is a dummy error generated on purpose for job {jobId}.");
            }
            return Task.FromResult($"Dummy service response for job {jobId}. You sent payload {payload.Data}");
        }
    }
}
