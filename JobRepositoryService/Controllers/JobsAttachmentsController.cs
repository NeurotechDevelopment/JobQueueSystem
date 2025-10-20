using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace JobRepositoryService.Controllers
{
    [Authorize]
    [ApiExplorerSettings(GroupName = ServicesConstants.JobsAttachments)]
    [ApiController]
    [Route(ServicesConstants.ControllerRoutes.AttachmentsApiResource)]
    public class JobsAttachmentsController : ControllerBase
    {
        private readonly ILogger<JobsAttachmentsController> logger;
        private readonly IBlobStorage storage;
        private readonly IOptions<ApplicationSettings> appSettings;
        private readonly IMemoryCache cache; // To hold mapping of temp id to actual blob id.

        public JobsAttachmentsController(ILogger<JobsAttachmentsController> logger, 
                                         IBlobStorage storage, 
                                         IOptions<ApplicationSettings> appSettings, 
                                         IMemoryCache cache)
        {
            this.logger = logger;
            this.storage = storage;
            this.appSettings = appSettings;
            this.cache = cache;
        }

        // We want to allow large file uploads, so we disable the request size limit.
        // Kestrel cuts it off at 30MB by default. May need to configure that during Startup too.
        [DisableRequestSizeLimit]
        [HttpPost($"{ServicesConstants.ActionRoutes.Stream}/{{tag}}/{{blobName}}")]
        public async Task<IActionResult> UploadAttachmentStreamAsync(string tag, string blobName)
        {
            this.logger.LogTrace("Uploading attachment with tag: {Tag}, blobName: {BlobName}", tag, blobName);
            if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(blobName))
            {
                return BadRequest("Tag and blobName must be provided.");
            }
            if (Request.ContentLength == null || Request.ContentLength == 0)
            {
                return BadRequest("Request body is empty.");
            }
            
            await using var stream = Request.Body;
            var id = await this.storage.UploadStreamAsync(tag, blobName, Request.ContentType ?? "application/octet-stream", stream);
            
            return Ok(id);
        }

        [HttpGet($"{ServicesConstants.ActionRoutes.Stream}/{{id}}")]
        public async Task<IActionResult> DownloadAttachmentStreamAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id must be provided.");
            }
            var fileInfo = await this.storage.GetFileInfoAsync(id);
            var stream = await this.storage.DownloadStreamAsync(id);
            if (stream == null)
            {
                return NotFound();
            }

            return File(stream, fileInfo.contentType, fileInfo.fileName);
        }

        // This endpoint allows anonymous access via the temp link.
        [AllowAnonymous]
        [HttpGet($"{ServicesConstants.ActionRoutes.Stream}/temp/{{tempId}}", Name = "DownloadTempFileEndpoint")]
        public async Task<IActionResult> DownloadAttachmentViaTempLinkAsync(string tempId)
        {
            if (string.IsNullOrWhiteSpace(tempId))
            {
                return BadRequest("TempId must be provided.");
            }
            if (!this.cache.TryGetValue<string>(tempId, out var fileId))
            {
                return NotFound("The temporary link has expired or is invalid.");
            }

            return await DownloadAttachmentStreamAsync(fileId!);
        }

        [HttpGet($"{ServicesConstants.ActionRoutes.Stream}/generate-temp-link/{{id}}")]
        public async Task<IActionResult> GenerateTempDownloadLinkAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id must be provided.");
            }

            // To verify the file exists.
            if (!await this.storage.FileExistsAsync(id))
            {
                return NotFound($"File with id {id} not found.");
            }

            var tempId = Guid.NewGuid().ToString();
            this.cache.Set(tempId, id, TimeSpan.FromSeconds(this.appSettings.Value.TempFileLinkExpirationInSeconds));
            var downloadLink = Url.Link("DownloadTempFileEndpoint", new { tempId = tempId });
            return Ok(downloadLink);
        }

        /// <summary>
        /// Buffered download of attachment.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadAttachmentAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id must be provided.");
            }
            var fileInfo = await this.storage.GetFileInfoAsync(id);
            var buffer = await this.storage.DownloadAsync(id);

            return File(buffer, fileInfo.contentType, fileInfo.fileName);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttachmentAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id must be provided.");
            }
            await this.storage.DeleteAsync(id);
            return NoContent();
        }
    }
}
