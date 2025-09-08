using Contracts;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace JobRepositoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        public IEnumerable<Job> GetJobs()
        {
            return this.repository.GetJobs();
        }

        [HttpGet("{jobId}")]
        public Job GetJob(Guid jobId)
        {
            return this.repository
                .GetQueryableJobDocuments()
                .SingleOrDefault(x => x.JobId == jobId);
        }

        [HttpGet("payload/{jobId}")]
        public string GetJobPayload(Guid jobId)
        {
            return this.repository.GetJobPayload(jobId);
        }

        [HttpPost]
        public IActionResult CreateJobRequest(JobRequest jobRequest)
        {
            this.repository.AddJobRequest(jobRequest);
            return Ok();
        }

        [HttpDelete("{jobId}")]
        public ActionResult<long> DeleteJob(Guid jobId)
        {
            return this.repository.DeleteJob(jobId);
        }

        [HttpPut("{jobId}/SetStatus/{status}")]
        public ActionResult<long> SetStatus(Guid jobId, JobStatus status)
        {
            return this.repository.SetStatus(jobId, status);
        }

        [HttpPut("{jobId}/SetResult")]
        public ActionResult<long> SetResult(Guid jobId, [FromBody] string result)
        {
            return this.repository.SetResult(jobId, result);
        }
    }
}