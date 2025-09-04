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

        public string GetJobPayload(Guid jobId)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var item = db.GetCollection<JobDocument>(Job)
                .Find(x => x.JobId == jobId);
            
            return item.SingleOrDefault()?.Payload.ToJson();
        }

        public IEnumerable<Job> GetJobs()
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job)
                .Find(_ => true);
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

        public void AddJobRequest(JobRequest request)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);
            var jobDocument = this.mapper.Map<JobDocument>(request);

            jobDocument.Status = JobStatus.NotStarted;
            jobDocument.ReceivedAt = DateTime.UtcNow;

            items.InsertOne(jobDocument);
        }

        public long DeleteJob(Guid jobId)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);
            var filter = Builders<JobDocument>.Filter
                .Eq(doc => doc.JobId, jobId);

            // Deletes the first document that matches the filter
            return items.DeleteOne(filter).DeletedCount;
        }

        public long SetStatus(Guid jobId, JobStatus status)
        {
            var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
            var items = db.GetCollection<JobDocument>(Job);

            var filter = Builders<JobDocument>.Filter
                .Eq(j => j.JobId, jobId);

            var update = Builders<JobDocument>.Update
                .Set(j => j.Status, status)
                .Set(j => j.LastStatusChanged, DateTime.UtcNow);

            return items.UpdateOne(filter, update).ModifiedCount;
        }

        public long SetResult(Guid jobId, string result)
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

            return items.UpdateOne(filter, update).ModifiedCount;
        }

        public void Dispose()
        {
            mongoClient.Dispose();
        }
    }
}
