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
            return this.mapper.Map<IEnumerable<JobTypeDescriptor>>(this.jobService.GetJobTypes());
        }

        [HttpGet($"{ServicesConstants.ActionRoutes.JobTypes}/{{jobTypeId}}")]
        public JobTypeDescriptor GetJobTypeDescriptor(JobType jobTypeId)
        {
            var jobTypeDescriptor = this.jobService.GetJobTypes().Single(x => x.Key == jobTypeId);
            return this.mapper.Map<JobTypeDescriptor>(jobTypeDescriptor);
        }

        #endregion

        #region Jobs

        [HttpGet(ServicesConstants.ActionRoutes.Jobs)]
        public async Task<IEnumerable<JobInfo>> GetJobInfos()
        {
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
            await this.repository.AddJobRequestAsync(jobRequest);
            return Ok();
        }

        [HttpDelete($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}")]
        public async Task<ActionResult<long>> DeleteJob(Guid jobId)
        {
            // Removes a job from datastore with associated files.
            return await this.jobService.DeleteJobAsync(jobId);
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetStatus/{{status}}")]
        public async Task<ActionResult<long>> SetStatus(Guid jobId, JobStatus status)
        {
            return await this.repository.SetStatusAsync(jobId, status);
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetResult")]
        public async Task<ActionResult<long>> SetResult(Guid jobId, [FromBody] JobPayload result)
        {
            return await this.repository.SetResultAsync(jobId, result);
        }

        [HttpPut($"{ServicesConstants.ActionRoutes.Jobs}/{{jobId}}/SetErrorResult")]
        public async Task<ActionResult<long>> SetErrorResult(Guid jobId, [FromBody] ErrorPayload errorPayload)
        {
            return await this.repository.SetErrorResultAsync(jobId, errorPayload.ErrorMessage);
        }

        #endregion
    }
}