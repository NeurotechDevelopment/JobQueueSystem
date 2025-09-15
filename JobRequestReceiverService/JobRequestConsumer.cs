using Contracts;
using MassTransit;
using Shared;

namespace JobRequestReceiverService
{
    public class JobRequestConsumer : IConsumer<JobRequest>
    {
        private readonly ILogger<JobRequestConsumer> logger;
        private readonly IJobRepositoryClient client;

        public JobRequestConsumer(ILogger<JobRequestConsumer> logger, 
            IJobRepositoryClient client)
        {
            this.logger = logger;
            this.client = client;
        }

        public Task Consume(ConsumeContext<JobRequest> context)
        {
            var msg = context.Message;
            this.logger.LogDebug($"Received queue message. JobId {msg.JobId}, Type: {msg.Type}, Payload: {msg.Payload}");

            this.client.AddJobRequest(msg);

            return Task.CompletedTask;
        }
    }
}
