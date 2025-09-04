using Contracts;
using Shared;
using Shared.Queries;

namespace JobDispatcherService
{
    public class JobDispatcherWorker : BackgroundService
    {
        private const int PollInterval = 1000; // ms
        private readonly ILogger<JobDispatcherWorker> logger;
        private readonly JobRepositoryClient client;
        private readonly IJobDispatcher jobDispatcher;

        public JobDispatcherWorker(ILogger<JobDispatcherWorker> logger, JobRepositoryClient client, IJobDispatcher jobDispatcher)
        {
            this.logger = logger;
            this.client = client;
            this.jobDispatcher = jobDispatcher;
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

                await Parallel.ForEachAsync(newJobs, stoppingToken, async (job, ct) =>
                {
                    try
                    {
                        await this.jobDispatcher.DispatchAsync(job, ct);

                        // Mark job as enqueued. We don't want to pick it up again.
                        logger.LogTrace($"Changing job {job.JobId} to status {JobStatus.Enqueued}");
                        this.client.SetStatus(job.JobId, JobStatus.Enqueued);
                        logger.LogTrace($"Changed job {job.JobId} to status {JobStatus.Enqueued}");
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Error while dispatching job {job.JobId}");
                    }
                });

                await Task.Delay(PollInterval, stoppingToken);
            }
        }
    }
}
