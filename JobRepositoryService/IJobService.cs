using Contracts;

namespace JobRepositoryService
{
    /// <summary>
    /// Incorporates business logic around jobs.
    /// When no logic needed, controllers can call repositories directly.
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// Lists all available job types in a JobQueueSystem.
        /// </summary>
        /// <returns>For now, dumps all JobType enum values with their descriptions at the attribute level.</returns>
        public IEnumerable<KeyValuePair<JobType, string>> GetJobTypes();

        /// <summary>
        /// Contains business logic to delete the job and all associated data (attachments).
        /// </summary>
        /// <param name="jobId">Job id.</param>
        /// <returns>Number of affected records.</returns>
        public Task<long> DeleteJobAsync(Guid jobId);
    }
}
