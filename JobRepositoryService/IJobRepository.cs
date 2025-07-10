using Contracts;

namespace JobRepositoryService
{
    public interface IJobRepository
    {
        public IEnumerable<Job> GetList();

        public void AddJobRequest(JobRequest request);

        public long DeleteJob(Guid  jobId);

        public long SetStatus(Guid jobId, string status);
    }
}
