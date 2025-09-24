using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace Shared
{
    public partial class JobRepositoryClient : IJobRepositoryClient
    {
        private const string JobApiResource = ServicesConstants.ServiceResources.JobsApiResource;
        private const string OdataJobApiResource = ServicesConstants.ServiceResources.OdataJobsApiResource;
        private const string AttachmentsApiResource = ServicesConstants.ServiceResources.AttachmentsApiResource;

        private readonly ILogger<JobRepositoryClient> logger;
        private readonly string jobServiceUrl;
        private static readonly HttpClient HttpClient = new HttpClient(); // Workaround for file upload issues with RestSharp. RestSharp as of now can't do a stream upload.

        public JobRepositoryClient(ILogger<JobRepositoryClient> logger, IOptions<JobRepositoryClientConfig> config)
        {
            this.logger = logger;
            this.jobServiceUrl = config.Value.BaseUrl;
        }
    }
}
