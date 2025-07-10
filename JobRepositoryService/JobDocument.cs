using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace JobRepositoryService
{
    /// <summary>
    /// MongoDB representation of job metadata
    /// </summary>
    public class JobDocument
    {
        #region Job contract. Can't inherit because of JobId being Guid.

        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid JobId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime? LastStatusChanged { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? FinishedAt { get; set; }

        #endregion

        public BsonDocument Payload { get; set; }

        public BsonDocument Result { get; set; }
    }
}
