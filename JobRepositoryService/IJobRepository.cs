using Contracts;

namespace JobRepositoryService
{
    public interface IJobRepository
    {
        [Obsolete("Payloads will be part of a Job class.")]
        public Task<JobPayload> GetJobPayloadAsync(Guid jobId);

        public IQueryable<Job> GetQueryableJobDocuments();

        public Task<IEnumerable<JobInfo>> GetJobsAsync();

        public Job? GetJob(Guid jobId);

        public Task<Job?> GetJobAsync(Guid jobId);

        public Task AddJobRequestAsync(JobRequest request);

        public Task<long> DeleteJobAsync(Guid  jobId);

        public Task<long> SetStatusAsync(Guid jobId, JobStatus status);

        public Task<long> SetResultAsync(Guid jobId, JobPayload result);
    }
}
