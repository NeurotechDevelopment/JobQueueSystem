using Contracts;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JobRepositoryService
{
    /// <summary>
    /// Incorporates business logic around jobs.
    /// When no logic needed, controllers can call repositories directly.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly ILogger<JobService> logger;
        private readonly IJobRepository jobs;
        private readonly IBlobStorage blobs;

        public JobService(ILogger<JobService> logger, IJobRepository jobs, IBlobStorage blobs)
        {
            this.logger = logger;
            this.jobs = jobs;
            this.blobs = blobs;

            this.logger.LogTrace("JobService created");
        }

        private static Lazy<IEnumerable<KeyValuePair<JobType, string>>> JobTypes =
            new(() =>
            {
                var jobTypes = Enum.GetValues<JobType>();
                return jobTypes.Select(x => new KeyValuePair<JobType, string>(x, GetDescription(x)));
            });

        #region IJobService implementation

        public IEnumerable<KeyValuePair<JobType, string>> GetJobTypes()
        {
            return JobTypes.Value;
        }

        public async Task<long> DeleteJobAsync(Guid jobId)
        {
            this.logger.LogTrace("Deleting job {jobId}", jobId);

            var job = await this.jobs.GetJobAsync(jobId);
            if (job == null)
            {
                this.logger.LogWarning("Job {jobId} not found. Nothing to delete.", jobId);
                return 0;
            }

            async void DeleteAttachment(Attachment? attachment)
            {
                if (!string.IsNullOrWhiteSpace(attachment?.Id))
                {
                    this.logger.LogTrace($"Deleting file with id {attachment.Id}");
                    await this.blobs.DeleteAsync(attachment.Id);
                }
            }

            DeleteAttachment(job.RequestPayload?.Attachment);
            DeleteAttachment(job.ResultPayload?.Payload?.Attachment);
            var affected = await this.jobs.DeleteJobAsync(jobId);

            this.logger.LogTrace("Job {jobId} deleted", jobId);

            return affected;
        }

        #endregion

        private static string GetDescription(JobType jobType)
        {
            return jobType.GetType().GetMember(jobType.ToString()).First().GetCustomAttribute<DisplayAttribute>()
                .Description;
        }
    }
}
