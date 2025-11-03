using Contracts;
using MassTransit;
using Shared;

namespace JobRequestReceiverService
{
    internal class JobRequestConsumer : IConsumer<JobRequest>
    {
        private readonly ILogger<JobRequestConsumer> logger;
        private readonly IJobRepositoryClient client;
        private readonly ISendEndpointProvider sendEndpointProvider;

        public JobRequestConsumer(ILogger<JobRequestConsumer> logger, 
            IJobRepositoryClient client,
            ISendEndpointProvider sendEndpointProvider)
        {
            this.logger = logger;
            this.client = client;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<JobRequest> context)
        {
            var jobRequest = context.Message;
            
            this.logger.LogTrace($"Received queue message. JobId {jobRequest.JobId}, Type: {jobRequest.Type}, Payload: {jobRequest.Payload}");

            this.client.AddJobRequest(jobRequest);

            this.logger.LogTrace($"Published job {jobRequest.JobId} request to repository.");

            const string uri = $"queue:{QueueNames.DispatcherReady}";
            this.logger.LogTrace($"Publishing job request {jobRequest.JobId} to dispatcher queue {uri}");

            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));

            await endpoint.Send(jobRequest);

            this.logger.LogTrace($"Sent job request to {jobRequest.JobId} to dispatcher queue {uri} successfully.");
        }
    }
}
