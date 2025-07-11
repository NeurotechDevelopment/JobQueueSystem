using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Shared
{
    public class JobRepositoryClient
    {
        private const string JobRepository = "JobsRepository";

        private readonly ILogger<JobRepositoryClient> logger;
        private readonly string jobServiceUrl;

        public JobRepositoryClient(ILogger<JobRepositoryClient> logger, IOptions<JobRepositoryClientConfig> config)
        {
            this.logger = logger;
            this.jobServiceUrl = config.Value.BaseUrl;
        }

        public IEnumerable<Job> GetJobs()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<IEnumerable<Job>>(JobRepository);
            }
        }

        public void AddJobRequest(JobRequest jobRequest)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest(JobRepository, Method.Post)
                    .AddJsonBody(jobRequest);
                client.Post<JobRequest>(request);
            }
        }

        public long RemoveJob(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobRepository}/{jobId}", Method.Delete);
                return client.Delete<long>(request);
            }
        }

        public long SetStatus(Guid jobId, JobStatus status)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobRepository}/{jobId}/SetStatus/{status}", Method.Put);
                
                return client.Put<long>(request);
            }
        }

        public long SetResult(Guid jobId, string result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobRepository}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(result, ContentType.Json);
                return client.Put<long>(request);
            }
        }
    }
}