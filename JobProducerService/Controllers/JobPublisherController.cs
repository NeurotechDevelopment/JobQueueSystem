using System.Text.Json;
using Contracts;
using Contracts.Payloads;
using JobProducerService.Configuration;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Shared;

namespace JobProducerService.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Posting a job request with an attachment file.
        /// jobRequest is a JSON string representing the JobRequest object.
        /// It doesn't work if you try specifying the JobRequest object directly as a parameter.
        /// </summary>
        /// <param name="jobRequest">Serialized job request.</param>
        /// <param name="file">Mandatory file attachment.</param>
        [HttpPost]
        [Route("create-job-file")]
        public async Task<ActionResult> Post([FromForm] string jobRequest, IFormFile? file)
        {
            this.logger.LogTrace($"Entered Post with {jobRequest}");

            var request = JsonSerializer.Deserialize<JobRequest>(jobRequest, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            this.logger.LogTrace($"Uploading attachment for jobId: {request.JobId}, fileName: {file.FileName}, contentType: {file.ContentType}, size: {file.Length}");

            await using (var stream = file.OpenReadStream())
            {
                var fileId = await this.client.UploadAttachmentAsync(request.JobId.ToString(), file.FileName, stream, file.ContentType);
                var attachment = new Attachment(fileId, file.FileName, file.ContentType, file.Length);
                if (request.Payload == null)
                {
                    request = request with { Payload = new JobPayload(null, attachment) };
                }
                else
                {
                    request.Payload.Attachment = attachment;
                }
            }

            await SendJobRequest(request);
            return Accepted(request.JobId);
        }

        private async Task SendJobRequest(JobRequest jobRequest)
        {
            var uri = string.IsNullOrWhiteSpace(this.appSettings.Value.RabbitConfig.QueueName)
                ? $"queue:{QueueNames.InitialJobRequests}"
                : $"queue:{this.appSettings.Value.RabbitConfig.QueueName}";

            this.logger.LogTrace($"Getting endpoint for uri: {uri}");

            var endpoint = await sendEndpointProvider.GetSendEndpoint(new Uri(uri));

            await endpoint.Send(jobRequest);
        }
    }
}