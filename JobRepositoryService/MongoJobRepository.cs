using AutoMapper;
using Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JobRepositoryService
{
    public class MongoJobRepository : IJobRepository, IDisposable
    {
        private const string Job = "JobCollection";
        private readonly IMapper mapper;
        private readonly IOptions<ApplicationSettings> optionSettings;
        private readonly MongoClient mongoClient = null;

        public MongoJobRepository(IMapper mapper, IOptions<ApplicationSettings> optionSettings)
        {
            this.mapper = mapper;
            this.optionSettings = optionSettings;
            this.mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString);
        }

        public async Task<string> GetJobPayloadAsync(Guid jobId)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var item = await db.GetCollection<JobDocument>(Job)
                .FindAsync(x => x.JobId == jobId);
            
            return item.SingleOrDefault()?.Payload.ToJson();
        }

        public async Task<IEnumerable<Job>> GetJobsAsync()
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = await db.GetCollection<JobDocument>(Job)
                .FindAsync(_ => true);
            return items.ToList().Select(mapper.Map<Job>);
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
                    JobId = x.JobId,
                    LastStatusChanged = x.LastStatusChanged,
                    ReceivedAt = x.ReceivedAt,
                    Status = x.Status,
                    Type = x.Type
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

        public async Task<long> SetResultAsync(Guid jobId, string result)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);

            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);

            var update = Builders<JobDocument>.Update
                .Set(j => j.Result, BsonDocument.Parse(result))
                .Set(j => j.Status, JobStatus.Finished)
                .Set(j => j.LastStatusChanged, DateTime.UtcNow)
                .Set(j => j.FinishedAt, DateTime.UtcNow);

            var updateResult = await items.UpdateOneAsync(filter, update);
            return updateResult.ModifiedCount;
        }

        public void Dispose()
        {
            mongoClient.Dispose();
        }
    }
}
