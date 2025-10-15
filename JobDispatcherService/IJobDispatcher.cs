using Contracts;

namespace JobDispatcherService
{
    public interface IJobDispatcher
    {
        public Task DispatchJobAsync(Job job, CancellationToken ct = default);

        public Task DispatchJobRequestAsync(JobRequest jobRequest, CancellationToken ct = default);
    }
}
