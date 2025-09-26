using Contracts.Payloads;
using Shared;
using Shared.FileTypes;

namespace JobHandlers
{
    /// <summary>
    /// Handy utilities for dealing with files.
    /// </summary>
    internal class FileUtilitiesService : IFileUtilitiesService
    {
        private const int SmallAttachmentThreshold = 100 * 1024 * 1024; // 100 MB

        public Attachment CreateAttachment(Attachment sourceAttachment, FileType targetFileType, long targetFileSize)
        {
            var targetAttachment = new Attachment();

            var index = sourceAttachment.FileName.LastIndexOf('.');
            targetAttachment.FileName = index >= 0
                ? sourceAttachment.FileName.Substring(0, index) + "." + targetFileType.ToString().ToLower()
                : sourceAttachment.FileName;

            targetAttachment.Size = targetFileSize;
            targetAttachment.ContentType = targetFileType.MimeType();

            return targetAttachment;
        }

        public void AssertValidAttachment(Attachment? attachment)
        {
            ArgumentNullException.ThrowIfNull(attachment);

            if (string.IsNullOrWhiteSpace(attachment.Id))
            {
                throw new ArgumentException("Attachment id cannot be empty for a file handler type.");
            }
        }

        /// <summary>
        /// Creates a stream from a stream fetched from the repository client.
        /// Remember to dispose the stream when done.
        /// For small files, a memory stream is returned.
        /// For big files, a file stream is returned (file is downloaded to a temp file). Temp file is NOT deleted automatically.
        /// </summary>
        /// <param name="client">Job repository client.</param>
        /// <param name="attachment">Attachment whose fileId is used to download a stream, Size to determine stream strategy.</param>
        /// <returns></returns>
        public async Task<Stream> FetchStreamAsync(IJobRepositoryClient client, Attachment attachment)
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
