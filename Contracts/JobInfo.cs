namespace Contracts
{
    public record JobInfo : JobBase
    {
        public Guid JobId { get; set; }

        public bool? IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
