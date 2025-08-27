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
            
            Console.WriteLine("Dumping all the jobs");
            DumpJobs(client);
            
            var jobId = Guid.NewGuid();
            client.AddJobRequest(new JobRequest
            {
                JobId = jobId,
                Type = "Console application",
                Payload = "{ 'Console': 'app' }"
            });

            Console.WriteLine($"Added new job with id {jobId}");
            DumpJobs(client);

            client.SetStatus(jobId, JobStatus.Finished);
            Console.WriteLine($"Set it to status finished");
            DumpJobs(client);

            client.SetResult(jobId, "\"{'Some': 'Result'}\"");
            var jobs = client.QueryJobs(x => x.JobId == jobId && x.Status == JobStatus.Finished);
            
            Console.WriteLine("Fetching current job with odata");
            DumpJobs(jobs);

            client.RemoveJob(jobId);
            Console.WriteLine($"Removed job with id {jobId}");
            DumpJobs(client);

            Console.WriteLine("Testing Odata");
            jobs = client.QueryJobs(x =>
                (x.JobId == jobId || x.LastStatusChanged <= DateTime.UtcNow) &&
                (x.Type == "Petya" || x.Type != "Petya"));
            DumpJobs(jobs);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        static void DumpJobs(IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.JobId} {job.Status} {job.Type}");
            }
        }

        static void DumpJobs(JobRepositoryClient client)
        {
            var jobs = client.GetJobs();
            DumpJobs(jobs);
        }
    }
}