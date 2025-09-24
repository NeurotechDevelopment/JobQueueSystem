using System.Text.Json;
using Contracts;
using RestSharp;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        #region Async versions

        public async Task<IEnumerable<JobTypeDescriptor>> GetJobTypesAsync()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<IEnumerable<JobTypeDescriptor>>($"{JobApiResource}/{ServicesConstants.JobTypesUrlSegment}");
            }
        }

        public async Task<Job> GetJobAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<Job>($"{JobApiResource}/{jobId}");
            }
        }

        public async Task<JobPayload> GetJobPayloadAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<JobPayload>($"{JobApiResource}/payload/{jobId}");
            }
        }

        public async Task<IEnumerable<JobInfo>> GetJobsAsync()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<IEnumerable<Job>>(JobApiResource);
            }
        }

        public async Task AddJobRequestAsync(JobRequest jobRequest)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest(JobApiResource, Method.Post)
                    .AddJsonBody(jobRequest);
                await client.PostAsync<JobRequest>(request);
            }
        }

        public async Task<long> RemoveJobAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}", Method.Delete);
                return await client.DeleteAsync<long>(request);
            }
        }

        public async Task<long> SetStatusAsync(Guid jobId, JobStatus status)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}/SetStatus/{status}", Method.Put);

                return await client.PutAsync<long>(request);
            }
        }

        public async Task<long> SetResultAsync(Guid jobId, JobPayload result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{JobApiResource}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(JsonSerializer.Serialize(result), ContentType.Json);
                return await client.PutAsync<long>(request);
            }
        }

        #endregion
    }
}
