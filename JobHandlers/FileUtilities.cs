using Contracts.Payloads;
using Shared;

namespace JobHandlers
{
    /// <summary>
    /// TODO: to be moved to a service.
    /// </summary>
    internal static class FileUtilities
    {
        internal static string ReplaceFileExtension(string fileName, string newExtension)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return fileName;
            }
            var index = fileName.LastIndexOf('.');
            if (index >= 0)
            {
                return fileName.Substring(0, index) + "." + newExtension;
            }
            return fileName + newExtension;
        }

        internal static void AssertValidAttachment(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException("Attachment cannot be null for a file handler type.");
            }

            if (string.IsNullOrWhiteSpace(attachment.Id))
            {
                throw new ArgumentException("Attachment id cannot be empty for a file handler type.");
            }
        }

        const int SmallAttachmentThreshold = 100 * 1024 * 1024; // 100 MB
        internal static async Task<Stream> FetchStreamAsync(IJobRepositoryClient client, Attachment attachment)
        {
            AssertValidAttachment(attachment);

            // Big attachments will be downloaded to temp file and file stream returned.
            if (attachment.Size > SmallAttachmentThreshold)
            {
                var tempFilePath = Path.GetTempFileName();
                await using var downloadStream = await client.DownloadAttachmentStreamAsync(attachment.Id);
                await using (var fileStream = File.OpenWrite(tempFilePath))
                {
                    await downloadStream.CopyToAsync(fileStream);
                }
                return File.OpenRead(tempFilePath);
            }
            // Small attachments will be downloaded to memory stream and memory stream returned.
            else
            {
                await using var downloadStream = await client.DownloadAttachmentStreamAsync(attachment.Id);
                var ms = new MemoryStream();
                await downloadStream.CopyToAsync(ms);
                return ms;
            }
        }
    }
}
