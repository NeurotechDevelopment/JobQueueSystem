namespace Contracts
{
    public class JobRequest
    {
        public Guid JobId { get; set; }

        public JobType Type { get; set; }

        public string Payload { get; set; }
    }
}