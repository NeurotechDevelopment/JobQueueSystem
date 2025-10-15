using Contracts;
using MassTransit;
using Shared;

namespace JobDispatcherService
{
    public class JobDispatcher: IJobDispatcher
    {
        private readonly ILogger<JobDispatcher> logger;
        private readonly IServiceScopeFactory scopeFactory;

        public JobDispatcher(ILogger<JobDispatcher> logger, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.scopeFactory = scopeFactory;
        }

        public async Task DispatchJobAsync(Job job, CancellationToken ct)
        {
            await DispatchJobRequestAsync(
                new JobRequest
                {
                    JobId = job.JobId, 
                    Description = job.Description, 
                    Type = job.Type, 
                    Payload = job.RequestPayload
                },
                ct);
        }

        public async Task DispatchJobRequestAsync(JobRequest jobRequest, CancellationToken ct)
        {
            using var scope = this.scopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var workerQueue = new Uri($"queue:{jobRequest.Type}{QueueNames.DispatchedQueueSuffix}");
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(workerQueue);
            logger.LogTrace($"Sending job {jobRequest.JobId} to {workerQueue}");
            await sendEndpoint.Send(jobRequest, ct);
            logger.LogTrace($"Sent job {jobRequest.JobId} to {workerQueue}");
        }
    }
}
