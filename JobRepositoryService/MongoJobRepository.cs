using AutoMapper;
using Contracts;
using Contracts.Payloads;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace JobRepositoryService
{
    public class MongoJobRepository : IJobRepository
    {
        private const string Job = "JobCollection";
        private readonly IMapper mapper;
        private readonly IOptions<ApplicationSettings> optionSettings;
        private readonly IMongoClient mongoClient = null;

        public MongoJobRepository(IMongoClient client, IMapper mapper, IOptions<ApplicationSettings> optionSettings)
        {
            this.mapper = mapper;
            this.optionSettings = optionSettings;
            this.mongoClient = client;
        }

        public async Task<IEnumerable<JobInfo>> GetJobsAsync()
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = await db.GetCollection<JobDocument>(Job)
                .FindAsync(_ => true);
            
            return items.ToList().OrderByDescending(x => x.ReceivedAt).Select(mapper.Map<JobInfo>);
        }

        public IQueryable<Job> GetQueryableJobDocuments()
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            return db.GetCollection<JobDocument>(Job)
                .AsQueryable()
                // Avoid automapper to allow Mongo driver translating query correctly to the database
                .Select(x => new Job
                {
                    FinishedAt = x.FinishedAt,
                    Description = x.Description,
                    JobId = x.JobId,
                    LastStatusChanged = x.LastStatusChanged,
                    ReceivedAt = x.ReceivedAt,
                    Status = x.Status,
                    Type = x.Type,
                    // An ugly workaround for Mongo Linq provider not working with AutoMapper or any functions.
                    RequestPayload = x.Payload != null ? new JobPayload(x.Payload.Data, x.Payload.Attachment) : null,
                    // An ugly workaround for Mongo Linq provider not working with AutoMapper or any functions.
                    ResultPayload = x.Result != null ? new JobResult(new JobPayload(x.Result.Payload != null ? x.Result.Payload.Data : null, x.Result.Payload != null ? x.Result.Payload.Attachment : null), x.Result.IsSuccess, x.Result.ErrorMessage) : null
                });
        }

        public async Task AddJobRequestAsync(JobRequest request)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);
            var jobDocument = this.mapper.Map<JobDocument>(request);

            jobDocument.Status = JobStatus.NotStarted;
            jobDocument.ReceivedAt = DateTime.UtcNow;

            await items.InsertOneAsync(jobDocument);
        }

        public async Task<long> DeleteJobAsync(Guid jobId)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);
            var filter = Builders<JobDocument>.Filter
                .Eq(doc => doc.JobId, jobId);

            // Deletes the first document that matches the filter
            var deleteResult = await items.DeleteOneAsync(filter);
            return deleteResult.DeletedCount;
        }

        public async Task<long> SetStatusAsync(Guid jobId, JobStatus status)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);

            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);

            var update = Builders<JobDocument>.Update
                .Set(j => j.Status, status)
                .Set(j => j.LastStatusChanged, DateTime.UtcNow);
            var updateResult = await items.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount;
        }

        public async Task<long> SetResultAsync(Guid jobId, JobPayload resultPayload)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);

            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);
            
            var jobResult = new JobResult
            {
                Payload = resultPayload,
                IsSuccess = true,
                ErrorMessage = null
            };

            var update = Builders<JobDocument>.Update
                .Set(j => j.Result, jobResult)
                .Set(j => j.Status, JobStatus.Finished)
                .Set(j => j.LastStatusChanged, DateTime.UtcNow)
                .Set(j => j.FinishedAt, DateTime.UtcNow);

            var updateResult = await items.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount;
        }

        public async Task<long> SetErrorResultAsync(Guid jobId, string errorMessage)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);

            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);

            var jobResult = new JobResult
            {
                Payload = null,
                IsSuccess = false,
                ErrorMessage = errorMessage
            };

            var update = Builders<JobDocument>.Update
                .Set(j => j.Result, jobResult)
                .Set(j => j.Status, JobStatus.Failed)
                .Set(j => j.LastStatusChanged, DateTime.UtcNow)
                .Set(j => j.FinishedAt, DateTime.UtcNow);

            var updateResult = await items.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount;
        }

        public async Task<Job?> GetJobAsync(Guid jobId)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);
            var asyncCursorJobDocument = await db.GetCollection<JobDocument>(Job).FindAsync(filter);
            
            return this.mapper.Map<Job>(asyncCursorJobDocument.SingleOrDefault());

        }
    }
}

