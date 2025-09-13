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
                return await client.GetAsync<IEnumerable<JobTypeDescriptor>>($"{ServicesConstants.JobsRepository}/{ServicesConstants.JobTypesUrlSegment}");
            }
        }

        public async Task<Job> GetJobAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<Job>($"{ServicesConstants.JobsRepository}/{jobId}");
            }
        }

        public async Task<string> GetJobPayloadAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<string>($"{ServicesConstants.JobsRepository}/payload/{jobId}");
            }
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                return await client.GetAsync<IEnumerable<Job>>(ServicesConstants.JobsRepository);
            }
        }

        public async Task AddJobRequestAsync(JobRequest jobRequest)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest(ServicesConstants.JobsRepository, Method.Post)
                    .AddJsonBody(jobRequest);
                await client.PostAsync<JobRequest>(request);
            }
        }

        public async Task<long> RemoveJobAsync(Guid jobId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}", Method.Delete);
                return await client.DeleteAsync<long>(request);
            }
        }

        public async Task<long> SetStatusAsync(Guid jobId, JobStatus status)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}/SetStatus/{status}", Method.Put);

                return await client.PutAsync<long>(request);
            }
        }

        public async Task<long> SetResultAsync(Guid jobId, JobPayload result)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{ServicesConstants.JobsRepository}/{jobId}/SetResult", Method.Put);
                request.AddStringBody(JsonSerializer.Serialize(result), ContentType.Json);
                return await client.PutAsync<long>(request);
            }
        }

        #endregion
    }
}
