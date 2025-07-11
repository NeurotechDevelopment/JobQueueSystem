using System.Diagnostics;
using Contracts;
using Microsoft.Extensions.Options;
using Shared;

namespace JobRepoClientTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            JobRepositoryClient client = new JobRepositoryClient(null,
                Options.Create(new JobRepositoryClientConfig { BaseUrl = "https://localhost:7089" }));
            DumpJobs(client);

            var jobId = Guid.NewGuid();
            client.AddJobRequest(new JobRequest
            {
                JobId = jobId,
                Type = "Console application",
                Payload = "{ 'Console': 'app' }"
            });

            DumpJobs(client);

            client.SetStatus(jobId, JobStatus.Finished);
            DumpJobs(client);
            client.SetResult(jobId, "\"{'Some': 'Result'}\"");

            client.RemoveJob(jobId);

            DumpJobs(client);
            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        static void DumpJobs(JobRepositoryClient client)
        {
            var jobs = client.GetJobs();
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.JobId} {job.Status} {job.Type}");
            }
        }
    }
}