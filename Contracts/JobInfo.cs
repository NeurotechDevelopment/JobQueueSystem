namespace Contracts
{
    public record JobInfo : JobBase
    {
        public Guid JobId { get; set; }
    }
}
