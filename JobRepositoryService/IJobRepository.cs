using Contracts;
using Contracts.Payloads;

namespace JobRepositoryService
{
    public interface IJobRepository
    {
        public IQueryable<Job> GetQueryableJobDocuments();

        public Task<IEnumerable<JobInfo>> GetJobsAsync();

        public Task<Job?> GetJobAsync(Guid jobId);

        public Task AddJobRequestAsync(JobRequest request);

        public Task<long> DeleteJobAsync(Guid  jobId);

        public Task<long> SetStatusAsync(Guid jobId, JobStatus status);

        /// <summary>
        /// Sets the result payload for a job that executed successfully.
        /// </summary>
        /// <param name="jobId">Job id.</param>
        /// <param name="resultPayload">Result payload.</param>
        /// <returns>Number of affected records.</returns>
        public Task<long> SetResultAsync(Guid jobId, JobPayload resultPayload);

        /// <summary>
        /// Sets the error message for a failed job.
        /// Sets IsSuccess to false and clears any existing result payload.
        /// Sets FinishedAt timestamp.
        /// Sets status to Failed.
        /// </summary>
        /// <param name="jobId">Job id.</param>
        /// <param name="errorMessage">Error message describing job failure. Typically an exception.</param>
        /// <returns>Number of affected records.</returns>
        public Task<long> SetErrorResultAsync(Guid jobId, string errorMessage);
    }
}
