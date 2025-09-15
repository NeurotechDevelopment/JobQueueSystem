using Contracts;
using MassTransit;
using Shared;

namespace JobHandlers.Handlers
{
    public abstract class JobHandler : IConsumer<JobRequest>
    {
        protected readonly ILogger<JobHandler> logger;
        protected readonly JobRepositoryClient client;

        protected JobHandler(ILogger<JobHandler> logger, JobRepositoryClient client)
        {
            this.logger = logger;
            this.client = client;

            this.logger.LogTrace($"JobRequest handler {GetType().Name} instantiated.");
        }

        public abstract JobType Handles { get; }

        protected abstract Task<string> PerformWorkAsync(Guid jobId, JobPayload payload);

        public async Task Consume(ConsumeContext<JobRequest> context)
        {
            var jobId = context.Message.JobId;
            this.logger.LogTrace($"Received job request with jobId: {jobId}");

            try
            {
                var payload = await this.client.GetJobPayloadAsync(jobId);

                await this.client.SetStatusAsync(jobId, JobStatus.InProgress);

                this.logger.LogTrace($"Executing {nameof(PerformWorkAsync)}.");
                string result = await this.PerformWorkAsync(jobId, payload);
                this.logger.LogTrace($"Executed {nameof(PerformWorkAsync)} OK.");

                await this.client.SetResultAsync(jobId, new JobPayload(result));
                await this.client.SetStatusAsync(jobId, JobStatus.Finished);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error with jobId={jobId}");
                await this.client.SetResultAsync(jobId, new JobPayload(ex.ToString()));
                await this.client.SetStatusAsync(jobId, JobStatus.Failed);
            }

            this.logger.LogTrace($"Leaving job request handler for jobId: {jobId}");
        }
    }
}
