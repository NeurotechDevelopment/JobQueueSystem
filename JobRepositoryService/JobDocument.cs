using Contracts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobRepositoryService
{
    /// <summary>
    /// MongoDB representation of job metadata
    /// </summary>
    public class JobDocument : JobBase
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid JobId { get; set; }

        public BsonDocument Payload { get; set; }

        public BsonDocument Result { get; set; }
    }
}
