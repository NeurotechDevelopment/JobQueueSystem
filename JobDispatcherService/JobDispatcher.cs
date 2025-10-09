using Contracts;
using MassTransit;

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

        public async Task DispatchAsync(Job job, CancellationToken ct)
        {
            using var scope = this.scopeFactory.CreateScope();
            var sendEndpointProvider = scope.ServiceProvider.GetService<ISendEndpointProvider>();
            var workerQueue = new Uri($"queue:{job.Type}-queue");
            var sendEndpoint = await sendEndpointProvider.GetSendEndpoint(workerQueue);
            logger.LogTrace($"Sending job {job.JobId} to {workerQueue}");
            await sendEndpoint.Send(new JobRequest { JobId = job.JobId, Description = job.Description, Type = job.Type, Payload = job.RequestPayload }, ct);
            logger.LogTrace($"Sent job {job.JobId} to {workerQueue}");
        }
    }
}
