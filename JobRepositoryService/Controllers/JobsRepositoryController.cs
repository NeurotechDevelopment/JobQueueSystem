using Contracts;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace JobRepositoryService.Controllers
{
    [ApiController]
    [Route(ServicesConstants.ServiceResources.JobsApiResource)]
    public class JobsRepositoryController : ControllerBase
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;
        private readonly JobTypesService jobTypeService;
        private readonly IMapper mapper;

        public JobsRepositoryController(ILogger<JobsRepositoryController> logger, IJobRepository repository, JobTypesService jobTypeService, IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.jobTypeService = jobTypeService;
            this.mapper = mapper;
        }

        [HttpGet(ServicesConstants.JobTypesUrlSegment)]
        public IEnumerable<JobTypeDescriptor> GetJobTypes()
        {
            return this.mapper.Map<IEnumerable<JobTypeDescriptor>>(jobTypeService.GetJobTypes());
        }

        [HttpGet]
        public async Task<IEnumerable<Job>> GetJobs()
        {
            return await this.repository.GetJobsAsync();
        }

        [HttpGet("{jobId}")]
        public Job GetJob(Guid jobId)
        {
            return this.repository
                .GetQueryableJobDocuments()
                .SingleOrDefault(x => x.JobId == jobId);
        }

        [HttpGet("payload/{jobId}")]
        public async Task<JobPayload> GetJobPayload(Guid jobId)
        {
            return await this.repository.GetJobPayloadAsync(jobId);
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
            return await this.repository.DeleteJobAsync(jobId);
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
    }
}