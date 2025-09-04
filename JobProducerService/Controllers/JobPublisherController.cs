using Contracts;
using JobProducerService.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JobProducerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobPublisherController : ControllerBase
    {
        private readonly ILogger<JobPublisherController> logger;
        private readonly IOptions<ApplicationSettings> appSettings;
        private readonly ISendEndpointProvider sendEndpointProvider;

        public JobPublisherController(ILogger<JobPublisherController> logger, IOptions<ApplicationSettings> appSettings, ISendEndpointProvider sendEndpointProvider)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost(Name = "PostJob")]
        public async Task<ActionResult> Post(JobRequest jobRequest)
        {
            
            var uri = $"queue:{this.appSettings.Value.RabbitConfig.QueueName}";
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));
            await endpoint.Send(jobRequest);

            return Accepted(jobRequest.JobId);
        }
    }
}