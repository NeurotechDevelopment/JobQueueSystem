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

        [HttpPost]
        [Route("create-job")]
        public async Task<ActionResult> Post(JobRequest jobRequest)
        {
            var uri = $"queue:{this.appSettings.Value.RabbitConfig.QueueName}";
            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));
            await endpoint.Send(jobRequest);

            return Accepted(jobRequest.JobId);
        }

        [HttpPost]
        [Route("create-job-file")]
        public async Task<ActionResult> Post(Guid jobId, JobType type, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var request = new JobRequest { JobId = jobId, Type = type };
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var base64FileContent = Convert.ToBase64String(stream.GetBuffer());

                return await Post(new JobRequest
                {
                    JobId = request.JobId, 
                    Payload = new JobPayload(base64FileContent), 
                    Type = request.Type
                });
            }
        }
    }
}