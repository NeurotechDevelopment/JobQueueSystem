using Contracts.Payloads;

namespace Contracts
{
    /// <summary>
    /// Represents the result of a job processing operation.
    /// </summary>
    public record JobResult
    {
        public JobResult()
        {
        }

        public JobResult(JobPayload payload, bool? isSuccess, string? errorMessage = null)
        {
            Payload = payload;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Represents job result payload.
        /// Expected strongly typed json.
        /// </summary>
        public JobPayload? Payload { get; set; }

        /// <summary>
        /// Indicates whether the job completed successfully.
        /// </summary>
        public bool? IsSuccess { get; set; }

        /// <summary>
        /// Error message if the job failed.
        /// Should be null if IsSuccess is true.
        /// Should be set if IsSuccess is false.
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
