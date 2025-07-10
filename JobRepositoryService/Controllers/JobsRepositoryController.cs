using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JobRepositoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsRepositoryController : ControllerBase
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;

        public JobsRepositoryController(ILogger<JobsRepositoryController> logger, IJobRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        [HttpGet(Name = nameof(GetJobs))]
        public IEnumerable<Job> GetJobs()
        {
            return this.repository.GetList();
        }

        [HttpPost(Name = nameof(CreateJobRequest))]
        public IActionResult CreateJobRequest(JobRequest jobRequest)
        {
            this.repository.AddJobRequest(jobRequest);
            return Ok();
        }

        [HttpDelete(Name = nameof(DeleteJob))]
        public ActionResult<long> DeleteJob(Guid jobId)
        {
            return this.repository.DeleteJob(jobId);
        }

        [HttpPut(Name = nameof(SetStatus))]
        public ActionResult<long> SetStatus(Guid jobId, string status)
        {
            return this.repository.SetStatus(jobId, status);
        }
    }
}