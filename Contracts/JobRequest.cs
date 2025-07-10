namespace Contracts
{
    public class JobRequest
    {
        public Guid JobId { get; set; }

        public string Type { get; set; }

        public string Payload { get; set; }
    }
}