using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobRepositoryService
{
    /// <summary>
    /// MongoDB representation of job metadata
    /// </summary>
    public class JobDocument
    {
        [BsonId]
        public Guid JobId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        public BsonDocument Payload { get; set; }

        public BsonDocument Result { get; set; }
    }
}
