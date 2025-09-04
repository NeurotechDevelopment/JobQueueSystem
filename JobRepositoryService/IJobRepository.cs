using Contracts;

namespace JobRepositoryService
{
    public interface IJobRepository
    {
        public string GetJobPayload(Guid jobId);

        public IQueryable<Job> GetQueryableJobDocuments();

        public IEnumerable<Job> GetJobs();

        public void AddJobRequest(JobRequest request);

        public long DeleteJob(Guid  jobId);

        public long SetStatus(Guid jobId, JobStatus status);

        public long SetResult(Guid jobId, string result);
    }
}
