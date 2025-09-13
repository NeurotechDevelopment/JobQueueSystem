namespace Contracts
{
    public class JobRequest
    {
        public Guid JobId { get; set; }

        public JobType Type { get; set; }

        public JobPayload Payload { get; set; }
    }
}