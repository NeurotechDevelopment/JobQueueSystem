using Contracts;
using Contracts.Payloads;
using Shared.Queries;

namespace Shared
{
    public interface IJobRepositoryClient
    {
        #region Odata

        public IEnumerable<Job> QueryJobs(JobOdataQueryBuilder? queryBuilder = null);

        public int CountJobs(JobOdataQueryBuilder? queryBuilder = null);

        #endregion

        #region Jobs

        #region Sync versions

        public IEnumerable<JobTypeDescriptor> GetJobTypes();

        public JobTypeDescriptor GetJobTypeDescriptor(JobType jobType);

        public Job GetJob(Guid jobId);

        public IEnumerable<JobInfo> GetJobs();

        public void AddJobRequest(JobRequest jobRequest);

        public long RemoveJob(Guid jobId);

        public long SetStatus(Guid jobId, JobStatus status);

        public long SetResult(Guid jobId, JobPayload result);

        public long SetErrorResult(Guid jobId, string errorMessage);

        #endregion

        #region Async versions

        public Task<IEnumerable<JobTypeDescriptor>> GetJobTypesAsync();

        public Task<JobTypeDescriptor> GetJobTypeDescriptorAsync(JobType jobType);

        public Task<Job> GetJobAsync(Guid jobId);

        public Task<IEnumerable<JobInfo>> GetJobsAsync();

        public Task AddJobRequestAsync(JobRequest jobRequest);

        public Task<long> RemoveJobAsync(Guid jobId);

        Task<long> SetStatusAsync(Guid jobId, JobStatus inProgress);

        /// <summary>
        /// Sets the result of a job. Implies success.
        /// </summary>
        Task<long> SetResultAsync(Guid jobId, JobPayload jobPayload);

        /// <summary>
        /// Sets the result of a job as failed with the provided error message.
        /// </summary>
        Task<long> SetErrorResultAsync(Guid jobId, string errorMessage);

        #endregion

        #endregion

        #region Attachments

        #region Sync versions

        public string UploadAttachment(string tag, string fileName, Stream fileStream, string contentType);

        public Stream DownloadAttachmentStream(string attachmentId);

        public byte[] DownloadAttachment(string requestAttachmentId);

        public void DeleteAttachment(string attachmentId);

        #endregion

        #region Async versions

        public Task<string> UploadAttachmentAsync(string tag, string fileName, Stream fileStream, string contentType);

        public Task<Stream> DownloadAttachmentStreamAsync(string attachmentId, CancellationToken token = default);

        public Task<byte[]> DownloadAttachmentAsync(string requestAttachmentId);

        public Task DeleteAttachmentAsync(string attachmentId);

        #endregion 


        #endregion
    }
}
