using Contracts;

namespace JobRepositoryService
{
    public interface IJobRepository
    {
        public Task<JobPayload> GetJobPayloadAsync(Guid jobId);

        public IQueryable<Job> GetQueryableJobDocuments();

        public Task<IEnumerable<Job>> GetJobsAsync();

        public Task AddJobRequestAsync(JobRequest request);

        public Task<long> DeleteJobAsync(Guid  jobId);

        public Task<long> SetStatusAsync(Guid jobId, JobStatus status);

        public Task<long> SetResultAsync(Guid jobId, JobPayload result);
    }
}
