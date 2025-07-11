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

        [HttpGet]
        public IEnumerable<Job> GetJobs()
        {
            return this.repository.GetList();
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