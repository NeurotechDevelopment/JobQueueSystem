using AutoMapper;
using Contracts;
using Contracts.Payloads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobRepositoryService.Controllers
{
    [Authorize]
    [ApiExplorerSettings(GroupName = ServicesConstants.JobsRepository)]
    [ApiController]
    [Route(ServicesConstants.ControllerRoutes.JobsApiResource)]
    public class JobsRepositoryController : ControllerBase
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;
        private readonly IMapper mapper;
        private readonly IJobService jobService;

        public JobsRepositoryController(ILogger<JobsRepositoryController> logger, IJobRepository repository, IMapper mapper, IJobService jobService)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
            this.jobService = jobService;

            this.logger.LogTrace($"Created {nameof(JobsRepositoryController)} instance.");
        }

        #region JobTypes

        [HttpGet(ServicesConstants.ActionRoutes.JobTypes)]
        public IEnumerable<JobTypeDescriptor> GetJobTypeDescriptors()
        {
            this.logger.LogDebug("Requested all job type descriptors.");

            return this.mapper.Map<IEnumerable<JobTypeDescriptor>>(this.jobService.GetJobTypes());
        }

        [HttpGet($"{ServicesConstants.ActionRoutes.JobTypes}/{{jobTypeId}}")]
        public JobTypeDescriptor GetJobTypeDescriptor(JobType jobTypeId)
        {
            this.logger.LogDebug($"Requested job type descriptor for JobTypeId: {jobTypeId}.");

            var jobTypeDescriptor = this.jobService.GetJobTypes().Single(x => x.Key == jobTypeId);
            return this.mapper.Map<JobTypeDescriptor>(jobTypeDescriptor);
        }

        #endregion

        #region Jobs

        [HttpGet(ServicesConstants.ActionRoutes.Jobs)]
        public async Task<IEnumerable<JobInfo>> GetJobInfos()
        {
            this.logger.LogDebug("Requested all job infos.");

            return await this.repository.GetJobsAsync();
        }

        [HttpGet($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}")]
        public async Task<Job> GetJob(Guid jobId)
        {
            return await this.repository.GetJobAsync(jobId);
        }

        [HttpPost(ServicesConstants.ActionRoutes.Jobs)]
        public async Task<IActionResult> CreateJobRequest(JobRequest jobRequest)
        {
            this.logger.LogDebug($"Received job request. JobId: {jobRequest.JobId}, Type: {jobRequest.Type}");
            
            await this.repository.AddJobRequestAsync(jobRequest);

            this.logger.LogDebug($"Created job request. JobId: {jobRequest.JobId}, Type: {jobRequest.Type}");

            return Ok();
        }

        [HttpDelete($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}")]
        public async Task<ActionResult<long>> DeleteJob(Guid jobId)
        {
            this.logger.LogDebug($"Deleting job and associated data. JobId: {jobId}.");

            // Removes a job from datastore with associated files.
            var affectedRecords = await this.jobService.DeleteJobAsync(jobId);

            this.logger.LogDebug($"Deleted job and associated data. JobId: {jobId}. AffectedRecords: {affectedRecords}.");

            return affectedRecords;
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetStatus/{{status}}")]
        public async Task<ActionResult<long>> SetStatus(Guid jobId, JobStatus status)
        {
            this.logger.LogDebug($"Setting job status. JobId: {jobId}, Status: {status}.");

            var affectedRecords = await this.repository.SetStatusAsync(jobId, status);

            this.logger.LogDebug($"Set job status. JobId: {jobId}, Status: {status}, AffectedRecords: {affectedRecords}.");

            return affectedRecords;
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetResult")]
        public async Task<ActionResult<long>> SetResult(Guid jobId, [FromBody] JobPayload result)
        {
            this.logger.LogDebug($"Setting job result. JobId: {jobId}.");

            var affectedRecords = await this.repository.SetResultAsync(jobId, result);

            this.logger.LogDebug($"Set job result. JobId: {jobId}, AffectedRecords: {affectedRecords}.");

            return affectedRecords;
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetErrorResult")]
        public async Task<ActionResult<long>> SetErrorResult(Guid jobId, [FromBody] ErrorPayload errorPayload)
        {
            this.logger.LogDebug($"Setting job error result. JobId: {jobId}, ErrorMessage: {errorPayload.ErrorMessage}.");

            var affectedRecords = await this.repository.SetErrorResultAsync(jobId, errorPayload.ErrorMessage);

            this.logger.LogDebug($"Set job error result. JobId: {jobId}, AffectedRecords: {affectedRecords}.");

            return affectedRecords;
        }

        #endregion
    }
}