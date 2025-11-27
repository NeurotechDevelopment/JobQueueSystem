using Contracts;
using MassTransit;
using Shared;

namespace JobDispatcherService
{
    internal class JobRequestConsumer : IConsumer<JobRequest>
    {
        private readonly ILogger<JobRequestConsumer> logger;
        private readonly IJobRepositoryClient client;
        private readonly IJobDispatcher jobDispatcher;

        public JobRequestConsumer(ILogger<JobRequestConsumer> logger,
            IJobRepositoryClient client,
            IJobDispatcher jobDispatcher)
        {
            this.logger = logger;
            this.client = client;
            this.jobDispatcher = jobDispatcher;

            this.logger.LogTrace($"{nameof(JobRequestConsumer)} initialized.");
        }

        public async Task Consume(ConsumeContext<JobRequest> context)
        {
            var jobRequest = context.Message;
            this.logger.LogDebug(
                $"Received queue message. JobId {jobRequest.JobId}, Type: {jobRequest.Type}, Description: {jobRequest.Description}, Payload: {jobRequest.Payload}");

            await this.jobDispatcher.DispatchJobRequestAsync(jobRequest);

            this.logger.LogDebug(
                $"Dispatched queue message. JobId {jobRequest.JobId}, Type: {jobRequest.Type}, Description: {jobRequest.Description}, Payload: {jobRequest.Payload}");
        }
    }
}
