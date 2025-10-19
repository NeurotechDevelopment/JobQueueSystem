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
    [Route(ServicesConstants.ServiceResources.JobsApiResource)]
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

        [HttpGet(ServicesConstants.JobTypesUrlSegment)]
        public IEnumerable<JobTypeDescriptor> GetJobTypeDescriptors()
        {
            return this.mapper.Map<IEnumerable<JobTypeDescriptor>>(this.jobService.GetJobTypes());
        }

        [HttpGet($"{ServicesConstants.JobTypesUrlSegment}/{{jobTypeId}}")]
        public JobTypeDescriptor GetJobTypeDescriptor(JobType jobTypeId)
        {
            var jobTypeDescriptor = this.jobService.GetJobTypes().Single(x => x.Key == jobTypeId);
            return this.mapper.Map<JobTypeDescriptor>(jobTypeDescriptor);
        }

        [HttpGet]
        public async Task<IEnumerable<JobInfo>> GetJobInfos()
        {
            return await this.repository.GetJobsAsync();
        }

        [HttpGet("{jobId}")]
        public async Task<Job> GetJob(Guid jobId)
        {
            return await this.repository.GetJobAsync(jobId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJobRequest(JobRequest jobRequest)
        {
            await this.repository.AddJobRequestAsync(jobRequest);
            return Ok();
        }

        [HttpDelete("{jobId}")]
        public async Task<ActionResult<long>> DeleteJob(Guid jobId)
        {
            // Removes a job from datastore with associated files.
            return await this.jobService.DeleteJobAsync(jobId);
        }

        [HttpPut("{jobId}/SetStatus/{status}")]
        public async Task<ActionResult<long>> SetStatus(Guid jobId, JobStatus status)
        {
            return await this.repository.SetStatusAsync(jobId, status);
        }

        [HttpPut("{jobId}/SetResult")]
        public async Task<ActionResult<long>> SetResult(Guid jobId, [FromBody] JobPayload result)
        {
            return await this.repository.SetResultAsync(jobId, result);
        }

        [HttpPut("{jobId}/SetErrorResult")]
        public async Task<ActionResult<long>> SetErrorResult(Guid jobId, [FromBody] ErrorPayload errorPayload)
        {
            return await this.repository.SetErrorResultAsync(jobId, errorPayload.ErrorMessage);
        }
    }
}