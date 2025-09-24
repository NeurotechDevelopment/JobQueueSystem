using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using Contracts;
using Contracts.Payloads.Requests;
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
                Options.Create(new JobRepositoryClientConfig { BaseUrl = Settings.Default.JobRepositoryServiceUrl }));

            // Find all private static methods with Description attribute - these are our test actions
            // Construct invokable actions from them
            var testableActions = typeof(Program).GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                .Where(x => x.GetCustomAttribute<DescriptionAttribute>() != null)
                .Select(x => new
                {
                    Description = x.GetCustomAttribute<DescriptionAttribute>().Description,
                    TestAction = (Action)(() => x.Invoke(null, new[] { client })) // No need to cast, we know the signature
                })
                .OrderBy(x => x.Description)
                .ToArray();

            void DumpHelp()
            {
                Console.WriteLine("Select test:");
                for (int i = 0; i < testableActions.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {testableActions[i].Description}");
                }
                Console.WriteLine("q - Exit");
            }

            DumpHelp();
            var keyInfo = Console.ReadKey(true);
            while (keyInfo.KeyChar != 'q')
            {
                var convertedChar = keyInfo.KeyChar - '1';
                if (convertedChar >= 0 && convertedChar < testableActions.Length)
                {
                    testableActions[convertedChar].TestAction();
                }
                else
                {
                    DumpHelp();
                }

                keyInfo = Console.ReadKey(true);
            }
        }

        [Description("Upload attachment as stream")]
        private static void TestUploadAttachment(JobRepositoryClient client)
        {
            using (var stream = File.OpenRead("resources/GettingStartedWithOneDrive.pdf"))
            {
                var fileId = client.UploadAttachment("TestTag", "GettingStartedWithOneDrive.pdf", stream, "application/pdf");
                Console.WriteLine($"Uploaded pdf and got fileId: {fileId}");
            }
        }

        [Description("Dumps job types")]
        private static void TestJobTypes(JobRepositoryClient client)
        {
            var jobTypes = client.GetJobTypes();
            Console.WriteLine("Allowed job types are:");
            foreach (var jobType in jobTypes)
            {
                Console.WriteLine($"{jobType.JobType}: {jobType.Description}");
            }
        }

        [Description("Demonstrates usage of Odata queries")]
        private static void TestOdata(JobRepositoryClient client)
        {
            IEnumerable<Job> jobs;
            Console.WriteLine("Testing Odata");

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .OrderBy(x => x.Type)
                .OrderBy(x => x.FinishedAt));

            Console.WriteLine("Queried finished jobs ordered by type and than by finished.");
            DumpJobs(jobs);

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .OrderBy(x => x.FinishedAt)
                .Top(3));

            Console.WriteLine("First 3 finished jobs order by FinishedAt.");
            DumpJobs(jobs);

            var count = client.CountJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .Top(3));
            
            Console.WriteLine($"Count of finished jobs is {count}");

            jobs = client.QueryJobs(JobOdataQueryBuilder.Create()
                .Where(x => x.Status == JobStatus.Finished)
                .OrderByDescending(x => x.FinishedAt)
                .Skip(7));

            Console.WriteLine("All finished jobs skipping first 7 order descending by FinishedAt.");
            DumpJobs(jobs);
        }

        [Description("Dump all jobs")]
        private static void DumpAllJobs(JobRepositoryClient client)
        {
            Console.WriteLine("Dumping all the jobs");
            DumpJobs(client);
        }
        
        [Description("Test JobRepository CRUD")]
        private static void JobCruds(JobRepositoryClient client)
        {
            var jobId = Guid.NewGuid();
            client.AddJobRequest(new JobRequest
            {
                JobId = jobId,
                Type = JobType.Dummy,
                Payload = new JobPayload { Data = "Console app test payload" }
            });

            Console.WriteLine($"Added new job with id {jobId}");

            var job = client.GetJob(jobId);
            DumpJobs(job);

            Console.WriteLine("Set result job payload");
            var typedPayload = new ConvertScanToSearchablePdfPayload();
            typedPayload.Language = "en";
            var payload = JsonSerializer.Serialize(typedPayload);
            client.SetResult(jobId, new JobPayload { Data = payload } );

            Console.WriteLine("Set it to status finished");
            client.SetStatus(jobId, JobStatus.Finished);
            
            job = client.GetJob(jobId);
            DumpJobs(job);

            client.SetResult(jobId, new JobPayload { Data = "Some result 2" });

            var removed = client.RemoveJob(jobId);
            Console.WriteLine($"Removed job with id {jobId}. Affected records: {removed}.");
        }

        static void DumpJobs(IEnumerable<Job> jobs)
        {
            foreach (var job in jobs)
            {
                Console.WriteLine($"{job.JobId} {job.Status} {job.Type} {job.ReceivedAt} {job.LastStatusChanged} {job.FinishedAt}");
            }
        }

        static void DumpJobs(params Job[] jobs)
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