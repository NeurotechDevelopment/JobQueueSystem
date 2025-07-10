using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JobRepositoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsRepositoryController : ControllerBase
    {
        private readonly ILogger<JobsRepositoryController> _logger;

        public JobsRepositoryController(ILogger<JobsRepositoryController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = nameof(GetJobs))]
        public IEnumerable<Job> GetJobs()
        {
            return null;
        }
    }
}