using Contracts;
using Contracts.Payloads;
using RestSharp;
using System.Text.Json;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        #region REST API

        #region Sync versions

        public IEnumerable<JobTypeDescriptor> GetJobTypes()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<IEnumerable<JobTypeDescriptor>>($"{JobApiResource}/{ServicesConstants.JobTypesUrlSegment}");
            }
        }

        public Job GetJob(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<Job>($"{JobApiResource}/{jobId}");
            }
        }

        public IEnumerable<JobInfo> GetJobs()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return client.Get<IEnumerable<Job>>(JobApiResource);
            }
        }

        public void AddJobRequest(JobRequest jobRequest)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest(JobApiResource, Method.Post)
                    .AddJsonBody(jobRequest);
                client.Post<JobRequest>(request);
            }
        }

        public long RemoveJob(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}", Method.Delete);
                return client.Delete<long>(request);
            }
        }

        public long SetStatus(Guid jobId, JobStatus status)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}/SetStatus/{status}", Method.Put);
                
                return client.Put<long>(request);
            }
        }

        public long SetResult(Guid jobId, JobPayload result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(JsonSerializer.Serialize(result), ContentType.Json);
                return client.Put<long>(request);
            }
        }

        public long SetErrorResult(Guid jobId, string errorMessage)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}/SetErrorResult", Method.Put);
                request.AddJsonBody(new ErrorPayload { ErrorMessage = errorMessage });
                return client.Put<long>(request);
            }
        }

        #endregion

        #endregion
    }
}