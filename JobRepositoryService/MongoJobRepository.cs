using AutoMapper;
using Contracts;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace JobRepositoryService
{
    public class MongoJobRepository : IJobRepository
    {
        private const string Job = "JobCollection";
        private readonly IMapper mapper;
        private readonly IOptions<ApplicationSettings> optionSettings;

        public MongoJobRepository(IMapper mapper, IOptions<ApplicationSettings> optionSettings)
        {
            this.mapper = mapper;
            this.optionSettings = optionSettings;
        }

        public IEnumerable<Job> GetList()
        {
            using (var mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString))
            {
                var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
                var items = db.GetCollection<JobDocument>(Job)
                    .Find(_ => true);

                return items.ToList<JobDocument>().Select(x => this.mapper.Map<Job>(x));
            }
        }

        public void AddJobRequest(JobRequest request)
        {
            using (var mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString))
            {
                var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
                var items = db.GetCollection<JobDocument>(Job);
                var jobDocument = this.mapper.Map<JobDocument>(request);
                
                jobDocument.Status = JobStatus.NotStarted;
                jobDocument.ReceivedAt = DateTime.UtcNow;
                
                items.InsertOne(jobDocument);
            }
        }

        public long DeleteJob(Guid jobId)
        {
            using (var mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString))
            {
                var db = mongoClient.GetDatabase(this.optionSettings.Value.Database);
                var items = db.GetCollection<JobDocument>(Job);
                var filter = Builders<JobDocument>.Filter
                    .Eq(doc => doc.JobId, jobId);
                
                // Deletes the first document that matches the filter
                return items.DeleteOne(filter).DeletedCount;
            }
        }

        public long SetStatus(Guid jobId, JobStatus status)
        {
            using (var mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString))
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
        }

        public long SetResult(Guid jobId, string result)
        {
            using (var mongoClient = new MongoClient(this.optionSettings.Value.ConnectionString))
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
        }
    }
}
