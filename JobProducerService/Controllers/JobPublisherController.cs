using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace JobProducerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobPublisherController : ControllerBase
    {
        private readonly ILogger<JobPublisherController> _logger;

        public JobPublisherController(ILogger<JobPublisherController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostJob")]
        public IActionResult Post(JobRequest jobRequest)
        {
            return null;
        }
    }
}