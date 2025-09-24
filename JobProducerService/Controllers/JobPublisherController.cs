using Contracts;
using Contracts.Payloads;
using JobProducerService.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;
using System.Net;

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

            var uri = $"queue:{this.appSettings.Value.RabbitConfig.QueueName}";

            this.logger.LogTrace($"Getting endpoint for uri: {uri}");

            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));

            await endpoint.Send(jobRequest);

            this.logger.LogTrace($"Sent job request with id {jobRequest.JobId} to queue {uri}");

            return Accepted(jobRequest.JobId);
        }

        [HttpPost]
        [Route("create-job-file")]
        public async Task<ActionResult> Post(Guid jobId, JobType type, string? payload, IFormFile file)
        {
            this.logger.LogTrace($"Entered Post with jobId: {jobId}, jobType: {type}");

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            this.logger.LogTrace($"Uploading attachment for jobId: {jobId}, fileName: {file.FileName}, contentType: {file.ContentType}, size: {file.Length}");

            await using (var stream = file.OpenReadStream())
            {
                var fileId = await this.client.UploadAttachmentAsync(jobId.ToString(), file.FileName, stream, file.ContentType);
                return await Post(new JobRequest
                {
                    JobId = jobId, 
                    Payload = new JobPayload
                    {
                        Attachment = new Attachment
                        {
                            Id = fileId,
                            FileName = file.FileName,
                            ContentType = file.ContentType,
                            Size = file.Length
                        },
                        Data = payload
                    }, 
                    Type = type
                });
            }
        }
    }
}