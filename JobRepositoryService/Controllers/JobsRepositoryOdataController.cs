using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace JobRepositoryService.Controllers
{
    [Route(ServicesConstants.OdataRoutePrefix + "/" + ServicesConstants.JobsEntity)]
    [ApiController]
    public class JobsRepositoryOdataController : ODataController
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;

        public JobsRepositoryOdataController(ILogger<JobsRepositoryController> logger, IJobRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
        }

        [EnableQuery]
        [HttpGet("$count")] // For /$count support
        [HttpGet] // For regular queries support
        public IQueryable<Job> GetQueryableJobs()
        {
            return this.repository.GetQueryableJobDocuments();
        }
    }
}
