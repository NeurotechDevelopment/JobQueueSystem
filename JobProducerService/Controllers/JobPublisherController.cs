using Contracts;
using Contracts.Payloads;
using JobProducerService.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;

namespace JobProducerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobPublisherController : ControllerBase
    {
        private readonly ILogger<JobPublisherController> logger;
        private readonly IOptions<ApplicationSettings> appSettings;
        private readonly ISendEndpointProvider sendEndpointProvider;
        private readonly IJobRepositoryClient client;

        public JobPublisherController(ILogger<JobPublisherController> logger, IOptions<ApplicationSettings> appSettings, ISendEndpointProvider sendEndpointProvider, IJobRepositoryClient client)
        {
            this.logger = logger;
            this.appSettings = appSettings;
            this.sendEndpointProvider = sendEndpointProvider;
            this.client = client;
            
            logger.LogTrace($"Created an instance of {nameof(JobPublisherController)}. Queue name: {appSettings.Value.RabbitConfig.QueueName}");
        }

        [HttpPost]
        [Route("create-job")]
        public async Task<ActionResult> Post(JobRequest jobRequest)
        {
            this.logger.LogTrace($"Entered Post with jobId: {jobRequest.JobId}, jobType: {jobRequest.Type}");

            await SendJobRequest(jobRequest);

            return Accepted(jobRequest.JobId);
        }

        [HttpPost]
        [Route("create-job-file")]
        public async Task<ActionResult> Post([FromForm] JobRequest jobRequest, IFormFile? file)
        {
            this.logger.LogTrace($"Entered Post with jobId: {jobRequest.JobId}, jobType: {jobRequest.Type}");

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            this.logger.LogTrace($"Uploading attachment for jobId: {jobRequest.JobId}, fileName: {file.FileName}, contentType: {file.ContentType}, size: {file.Length}");

            await using (var stream = file.OpenReadStream())
            {
                var fileId = await this.client.UploadAttachmentAsync(jobRequest.JobId.ToString(), file.FileName, stream, file.ContentType);
                var attachment = new Attachment(fileId, file.FileName, file.ContentType, file.Length);
                if (jobRequest.Payload == null)
                {
                    jobRequest = jobRequest with { Payload = new JobPayload(null, attachment) };
                }
                else
                {
                    jobRequest.Payload.Attachment = attachment;
                }
            }

            await SendJobRequest(jobRequest);
            return Accepted(jobRequest.JobId);
        }

        private async Task SendJobRequest(JobRequest jobRequest)
        {
            var uri = $"queue:{this.appSettings.Value.RabbitConfig.QueueName}";

            this.logger.LogTrace($"Getting endpoint for uri: {uri}");

            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));

            await endpoint.Send(jobRequest);
        }
    }
}