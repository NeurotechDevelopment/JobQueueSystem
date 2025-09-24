namespace Contracts
{
    /// <summary>
    /// Represents a request to create a new job for processing.
    /// </summary>
    public record JobRequest
    {
        /// <summary>
        /// Client set job id. Must be unique.
        /// </summary>
        public Guid JobId { get; set; }

        /// <summary>
        /// Job type.
        /// </summary>
        public JobType Type { get; set; }

        /// <summary>
        /// Represents job request payload, such as instructions for file processing
        /// or direct payload for processing when job type is not of a file type.
        /// </summary>
        public JobPayload? Payload { get; set; }
    }
}