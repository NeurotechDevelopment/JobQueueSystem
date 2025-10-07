using Contracts;
using Contracts.Payloads;
using Contracts.Payloads.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using Shared.Configuration;
using Shared.Queries;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace JobRepoClientTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Logger<JobRepositoryClient> logger = new Logger<JobRepositoryClient>(new LoggerFactory());
            JobRepositoryClient client = new JobRepositoryClient(logger,
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

        [Description("Set error result for a job")]
        private static void SetErrorResultTest(JobRepositoryClient client)
        {
            Console.WriteLine("Enter job id:");
            var jobId = Guid.Parse(Console.ReadLine());
            Console.WriteLine("Enter error message:");
            var errorMessage = Console.ReadLine();
            var affected = client.SetErrorResult(jobId, errorMessage);
            Console.WriteLine($"Set error result for job {jobId}. Affected records: {affected}.");
        }

        [Description("Download attachment as stream")]
        private static void DownloadAttachmentStreamTest(JobRepositoryClient client)
        {
            Console.WriteLine("Enter attachment id:");
            var id = Console.ReadLine();
            
            using (var stream = client.DownloadAttachmentStream(id))
            {
                using (var fileStream = File.Create("downloadedFile"))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        [Description("Download attachment as bytes")]
        private static void DownloadAttachmentAsBytesTest(JobRepositoryClient client)
        {
            Console.WriteLine("Enter attachment id:");
            var id = Console.ReadLine();

            var bytes = client.DownloadAttachment(id);

            File.WriteAllBytes("downloadedFile", bytes);
        }

        [Description("Upload attachment as stream")]
        private static void UploadAttachmentTest(JobRepositoryClient client)
        {
            using (var stream = File.OpenRead("resources/GettingStartedWithOneDrive.pdf"))
            {
                var fileId = client.UploadAttachment("TestTag", "GettingStartedWithOneDrive.pdf", stream, "application/pdf");
                Console.WriteLine($"Uploaded pdf and got fileId: {fileId}");
            }
        }

        [Description("Dumps job types")]
        private static void JobTypesTest(JobRepositoryClient client)
        {
            var jobTypes = client.GetJobTypes();
            Console.WriteLine("Allowed job types are:");
            foreach (var jobType in jobTypes)
            {
                Console.WriteLine($"{jobType.JobType}: {jobType.Description}");
            }
        }

        [Description("Demonstrates usage of Odata queries")]
        private static void OdataTest(JobRepositoryClient client)
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
        private static void DumpAllJobsTest(JobRepositoryClient client)
        {
            Console.WriteLine("Dumping all the jobs");
            DumpJobs(client);
        }

        [Description("Create Dummy JobRequest")]
        private static void DummyJobRequestTest(JobRepositoryClient client)
        {
            var jobId = Guid.NewGuid();
            client.AddJobRequest(new JobRequest
            {
                JobId = jobId,
                Type = JobType.Dummy,
                Payload = new JobPayload
                {
                    Data = JsonSerializer.Serialize(new SimpleMessagePayload("Hello from Dummy job at console world."))
                }
            });
           
            var job = client.GetJob(jobId);
            DumpJobs(job);

            var resultPayload = JsonSerializer.Serialize(new SimpleMessagePayload("Console result"));
            client.SetResult(jobId, new JobPayload(resultPayload));
            Console.WriteLine("Set it to status finished");
            
            job = client.GetJob(jobId);
            DumpJobs(job);

            client.SetErrorResult(jobId, "Now it is console error");
            job = client.GetJob(jobId);
            DumpJobs(job);

            var removed = client.RemoveJob(jobId);
            Console.WriteLine($"Removed job with id {jobId}. Affected records: {removed}.");
        }
        [Description("Test strongly-typed payload request with attachment")]
        private static void JobCrudsTest(JobRepositoryClient client)
        {
            var jobId = Guid.NewGuid();

            Attachment attachment = new Attachment();
            attachment.FileName = "GettingStartedWithOneDrive.pdf";
            attachment.ContentType = "application/pdf";
            
            using (var stream = File.OpenRead("resources/GettingStartedWithOneDrive.pdf"))
            {
                attachment.Size = stream.Length;
                stream.Position = 0;
                var fileId = client.UploadAttachment(jobId.ToString(), "GettingStartedWithOneDrive.pdf", stream, "application/pdf");
                attachment.Id = fileId;
                
                Console.WriteLine($"Uploaded pdf and got fileId: {fileId}");
            }

            var typedPayload = new ConvertScanToSearchablePdfPayload
            {
                Language = "en"
            };
            var payloadData = JsonSerializer.Serialize(typedPayload);
            var payloadWithAttachment = new JobPayload
            {
                Data = payloadData,
                Attachment = attachment
            };
            client.AddJobRequest(new JobRequest
            {
                JobId = jobId,
                Type = JobType.ConvertScanToSearchablePdf,
                Payload = payloadWithAttachment
            });

            Console.WriteLine($"Added new job with id {jobId}");

            var job = client.GetJob(jobId);
            DumpJobs(job);

            Console.WriteLine("Set result job payload");
           
            client.SetResult(jobId, payloadWithAttachment);

            Console.WriteLine("Set it to status finished");
            client.SetStatus(jobId, JobStatus.Finished);
            
            job = client.GetJob(jobId);
            DumpJobs(job);

            var removed = client.RemoveJob(jobId);
            Console.WriteLine($"Removed job with id {jobId}. Affected records: {removed}.");
        }

        static void DumpJobs(IEnumerable<JobInfo> jobs)
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