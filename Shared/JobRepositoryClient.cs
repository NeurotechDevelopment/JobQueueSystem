using System.Linq.Expressions;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using Shared.Queries;

namespace Shared
{
    public class JobRepositoryClient
    {
        private readonly ILogger<JobRepositoryClient> logger;
        private readonly string jobServiceUrl;

        public JobRepositoryClient(ILogger<JobRepositoryClient> logger, IOptions<JobRepositoryClientConfig> config)
        {
            this.logger = logger;
            this.jobServiceUrl = config.Value.BaseUrl;
        }

        public IEnumerable<Job> QueryJobs(Expression<Func<Job, bool>> expression)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var odataFilter = JobOdataQuery.Where(expression);
                var request = new RestRequest($"{ServicesConstants.OdataRoutePrefix}/{ServicesConstants.JobsEntity}");
                request.AddParameter("$filter", odataFilter, true);
                return client.Get<IEnumerable<Job>>(request);
            }
        }

        public IEnumerable<Job> GetJobs()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<IEnumerable<Job>>(ServicesConstants.JobsRepository);
            }
        }

        public void AddJobRequest(JobRequest jobRequest)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest(ServicesConstants.JobsRepository, Method.Post)
                    .AddJsonBody(jobRequest);
                client.Post<JobRequest>(request);
            }
        }

        public long RemoveJob(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}", Method.Delete);
                return client.Delete<long>(request);
            }
        }

        public long SetStatus(Guid jobId, JobStatus status)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}/SetStatus/{status}", Method.Put);
                
                return client.Put<long>(request);
            }
        }

        public long SetResult(Guid jobId, string result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(result, ContentType.Json);
                return client.Put<long>(request);
            }
        }
    }
}