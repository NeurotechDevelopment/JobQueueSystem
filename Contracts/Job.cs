namespace Contracts
{
    public class Job
    {
        public Guid JobId { get; set; }

        public string Type { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }
}
