using Microsoft.AspNetCore.Mvc;
namespace JobRepositoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobsAttachmentsController : ControllerBase
    {
        private readonly ILogger<JobsAttachmentsController> logger;
        private readonly IBlobStorage storage;

        public JobsAttachmentsController(ILogger<JobsAttachmentsController> logger, IBlobStorage storage)
        {
            this.logger = logger;
            this.storage = storage;
        }

        // We want to allow large file uploads, so we disable the request size limit.
        // Kestrel cuts it off at 30MB by default. May need to configure that during Startup too.
        [DisableRequestSizeLimit]
        [HttpPost("stream/{tag}/{blobName}")]
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
            
            return Ok(new { Id = id });
        }

        [HttpGet("stream/{id}")]
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
            //return File(stream, "application/octet-stream");
            return File(stream, fileInfo.contentType, fileInfo.fileName);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> DownloadAttachmentAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Id must be provided.");
            }
            var fileInfo = await this.storage.GetFileInfoAsync(id);
            var buffer =  await this.storage.DownloadAsync(id);

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
