using Contracts;
using Shared;
using Shared.Queries;

namespace JobDispatcherService
{
    public class JobDispatcherWorker : BackgroundService
    {
        private readonly ILogger<JobDispatcherWorker> logger;
        private readonly JobRepositoryClient client;

        public JobDispatcherWorker(ILogger<JobDispatcherWorker> logger, JobRepositoryClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogTrace("JobDispatcherWorker running at: {time}", DateTimeOffset.Now);

                var newJobs = this.client.QueryJobs(JobOdataQueryBuilder.Create()
                    .Where(x => x.Status == JobStatus.NotStarted)
                    .OrderBy(x => x.ReceivedAt))
                    .ToArray();

                logger.LogTrace($"Found {newJobs.Length} not started jobs.");

                

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
