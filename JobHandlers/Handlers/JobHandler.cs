using System.Text.Json;
using Contracts;
using Contracts.Payloads;
using MassTransit;
using Shared;

namespace JobHandlers.Handlers
{
    public abstract class JobHandler<TRequestPayload, TResultPayloadType> : IConsumer<JobRequest>
    {
        protected readonly ILogger<JobHandler<TRequestPayload, TResultPayloadType>> logger;
        protected readonly IJobRepositoryClient client;

        protected JobHandler(ILogger<JobHandler<TRequestPayload,TResultPayloadType>> logger, IJobRepositoryClient client)
        {
            this.logger = logger;
            this.client = client;

            this.logger.LogTrace($"JobRequest handler {GetType().Name} instantiated that handles {Handles} job type.");
        }

        public abstract JobType Handles { get; }

        protected abstract Task<(TResultPayloadType Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, TRequestPayload? payload, Attachment? requestAttachment);

        public async Task Consume(ConsumeContext<JobRequest> context)
        {
            var jobRequest = context.Message;
            var jobId = jobRequest.JobId;
            
            this.logger.LogTrace($"Received job request with jobId: {jobId}");

            try
            {
                await this.client.SetStatusAsync(jobId, JobStatus.InProgress);

                this.logger.LogTrace($"Executing {nameof(PerformWorkAsync)}.");

                TRequestPayload? item = jobRequest.Payload != null && jobRequest.Payload.Data != null ? JsonSerializer.Deserialize<TRequestPayload>(jobRequest.Payload.Data) : default(TRequestPayload);
                var result = await this.PerformWorkAsync(jobId, item, jobRequest.Payload?.Attachment);
                
                this.logger.LogTrace($"Executed {nameof(PerformWorkAsync)} OK.");

                // SetResult also sets status to Finished
                await this.client.SetResultAsync(jobId, new JobPayload(JsonSerializer.Serialize(result.Result), result.ResultFile));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, $"Error with jobId={jobId}");

                // SetErrorResult also sets status to Failed
                await this.client.SetErrorResultAsync(jobId, ex.ToString());
            }

            this.logger.LogTrace($"Leaving job request handler for jobId: {jobId}");
        }
    }
}
