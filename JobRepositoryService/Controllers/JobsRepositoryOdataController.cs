using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace JobRepositoryService.Controllers
{
    [Route(ServicesConstants.OdataRoutePrefix + "/" + ServicesConstants.JobsEntity)]
    [ApiController]
    public class JobsRepositoryOdataController : ControllerBase
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;

        public JobsRepositoryOdataController(ILogger<JobsRepositoryController> logger, IJobRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<Job> GetQueryableJobs()
        {
            return this.repository.GetQueryableJobDocuments();
        }
    }
}
