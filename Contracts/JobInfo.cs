namespace Contracts
{
    /// <summary>
    /// Lightweight job information used in job listings.
    /// (No payload or result data.)
    /// </summary>
    public record JobInfo : JobBase
    {
        public Guid JobId { get; set; }

        public bool? IsSuccess { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
