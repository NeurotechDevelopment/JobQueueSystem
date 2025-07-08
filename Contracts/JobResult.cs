namespace Contracts
{
    public class JobResult<T>
    {
        public Guid JobId { get; set; }

        public T Result { get; set; }
    }
}
