using Contracts;
using Contracts.Payloads;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobRepositoryService
{
    /// <summary>
    /// MongoDB representation of job metadata
    /// </summary>
    public record JobDocument : JobBase
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid JobId { get; set; }

        public JobPayload? Payload { get; set; }

        public JobResult? Result { get; set; }
    }
}
