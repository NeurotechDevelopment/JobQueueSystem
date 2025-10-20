using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace JobRepositoryService.Controllers
{
    [Authorize]
    [ApiExplorerSettings(GroupName = ServicesConstants.OdataRoutePrefix)]
    [Route(ServicesConstants.ControllerRoutes.OdataJobsApiResource)]
    [ApiController]
    public class JobsRepositoryOdataController : ODataController
    {
        private readonly ILogger<JobsRepositoryController> logger;
        private readonly IJobRepository repository;

        public JobsRepositoryOdataController(ILogger<JobsRepositoryController> logger, IJobRepository repository)
        {
            this.logger = logger;
            this.repository = repository;
            this.logger.LogTrace($"Created {nameof(JobsRepositoryOdataController)} instance.");
        }

        [EnableQuery]
        [HttpGet("$count")] // For /$count support
        [HttpGet] // For regular queries support
        public IQueryable<Job> GetQueryableJobs()
        {
            this.logger.LogTrace($"{nameof(GetQueryableJobs)}. Query: {Request.QueryString}");
            return this.repository.GetQueryableJobDocuments();
        }
    }
}
