using Contracts;

namespace JobDispatcherService
{
    public interface IJobDispatcher
    {
        public Task DispatchAsync(Job job, CancellationToken ct);
    }
}
