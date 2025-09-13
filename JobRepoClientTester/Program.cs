using Contracts;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;
using Shared.Queries;

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
                Type = JobType.Dummy,
                Payload = new JobPayload { Data = "Console app test payload" }
            });

            Console.WriteLine($"Added new job with id {jobId}");
            DumpJobs(client);

            var job = client.GetJob(jobId);
            var payload = client.GetJobPayload(jobId);
            Console.WriteLine($"Payload of jobId {jobId} is {payload}");

            var jobTypes = client.GetJobTypes();
            Console.WriteLine("Allowed job types are:");
            foreach (var jobType in jobTypes)
            {
                Console.WriteLine($"{jobType.JobType}: {jobType.Description}");
            }

            client.SetResult(jobId, new JobPayload { Data = "Some result"});
            client.SetStatus(jobId, JobStatus.Finished);
            Console.WriteLine("Set it to status finished");
            DumpJobs(client);

            client.SetResult(jobId, new JobPayload { Data = "Some result 2" });
            var j = client.QueryJobs();
            var jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                                                      .Where(x => x.JobId == jobId && x.Status == JobStatus.Finished));
            
            Console.WriteLine("Fetching current job with odata");
            DumpJobs(jobs);

            client.RemoveJob(jobId);
            Console.WriteLine($"Removed job with id {jobId}");
            DumpJobs(client);

            Console.WriteLine("Testing Odata");

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .OrderBy(x => x.Type)
                .OrderBy(x => x.FinishedAt));

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                                    .Where(x => x.Status == JobStatus.Finished)
                                    .Top(3));
            
;

            var count = client.CountJobs(JobOdataQueryBuilder.Create()
                                            .Where(x => x.Status == JobStatus.Finished)
                                            .Top(3));

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .Skip(7));


            DumpJobs(jobs);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();
        }

        static void DumpJobs(IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.JobId} {job.Status} {job.Type} {job.ReceivedAt} {job.LastStatusChanged} {job.FinishedAt}");
            }
        }

        static void DumpJobs(JobRepositoryClient client)
        {
            var jobs = client.GetJobs();
            DumpJobs(jobs);
        }
    }
}