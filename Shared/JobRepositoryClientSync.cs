using System.Text.Json;
using Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RestSharp;
using Shared.Configuration;
using Shared.Queries;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        private readonly ILogger<JobRepositoryClient> logger;
        private readonly string jobServiceUrl;

        public JobRepositoryClient(ILogger<JobRepositoryClient> logger, IOptions<JobRepositoryClientConfig> config)
        {
            this.logger = logger;
            this.jobServiceUrl = config.Value.BaseUrl;
        }

        #region ODATA

        public IEnumerable<Job> QueryJobs(JobOdataQueryBuilder queryBuilder = null)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var odataFilterParams = queryBuilder?.ToRequestParams();
                var request = new RestRequest($"{ServicesConstants.OdataRoutePrefix}/{ServicesConstants.JobsEntity}");

                if (odataFilterParams != null)
                {
                    foreach (var odataRequestParam in odataFilterParams)
                    {
                        request.AddParameter(odataRequestParam.Key, odataRequestParam.Value);
                    }
                }

                var response = client.Get<ODataJobResponse>(request);
                return response.Value;
            }
        }

        public int CountJobs(JobOdataQueryBuilder queryBuilder)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var odataFilterParams = queryBuilder.ToRequestParams();
                var request = new RestRequest($"{ServicesConstants.OdataRoutePrefix}/{ServicesConstants.JobsEntity}/$count");

                foreach (var odataRequestParam in odataFilterParams)
                {
                    request.AddParameter(odataRequestParam.Key, odataRequestParam.Value);
                }

                var response = client.Get(request);
                return int.Parse(response.Content);
            }
        }

        #endregion

        #region REST API

        #region Sync versions

        public IEnumerable<JobTypeDescriptor> GetJobTypes()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<IEnumerable<JobTypeDescriptor>>($"{ServicesConstants.JobsRepository}/{ServicesConstants.JobTypesUrlSegment}");
            }
        }

        public Job GetJob(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<Job>($"{ServicesConstants.JobsRepository}/{jobId}");
            }
        }

        public string GetJobPayload(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<string>($"{ServicesConstants.JobsRepository}/payload/{jobId}");
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

        public long SetResult(Guid jobId, JobPayload result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(JsonSerializer.Serialize(request), ContentType.Json);
                return client.Put<long>(request);
            }
        }

        #endregion

        #endregion
    }
}