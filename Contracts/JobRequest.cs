namespace Contracts
{
    public class JobRequest<T>
    {
        public Guid JobId { get; set; }

        public string Type { get; set; }

        public T Payload { get; set; }
    }
}