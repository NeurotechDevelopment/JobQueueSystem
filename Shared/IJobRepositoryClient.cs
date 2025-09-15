using Contracts;
using Shared.Queries;

namespace Shared
{
    public interface IJobRepositoryClient
    {
        #region Odata

        public IEnumerable<Job> QueryJobs(JobOdataQueryBuilder? queryBuilder = null);

        public int CountJobs(JobOdataQueryBuilder? queryBuilder = null);

        #endregion

        #region Sync versions

        public IEnumerable<JobTypeDescriptor> GetJobTypes();

        public Job GetJob(Guid jobId);

        public JobPayload GetJobPayload(Guid jobId);

        public IEnumerable<Job> GetJobs();

        public void AddJobRequest(JobRequest jobRequest);

        public long RemoveJob(Guid jobId);

        public long SetStatus(Guid jobId, JobStatus status);

        public long SetResult(Guid jobId, JobPayload result);

        #endregion

        #region Async versions

        public Task<IEnumerable<JobTypeDescriptor>> GetJobTypesAsync();

        public Task<Job> GetJobAsync(Guid jobId);

        public Task<JobPayload> GetJobPayloadAsync(Guid jobId);

        public Task<IEnumerable<Job>> GetJobsAsync();

        public Task AddJobRequestAsync(JobRequest jobRequest);

        public Task<long> RemoveJobAsync(Guid jobId);

        Task<long> SetStatusAsync(Guid jobId, JobStatus inProgress);

        Task<long> SetResultAsync(Guid jobId, JobPayload jobPayload);

        #endregion
    }
}
