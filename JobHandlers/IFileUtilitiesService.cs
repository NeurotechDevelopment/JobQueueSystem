using Contracts;
using Shared;
using Shared.FileTypes;

namespace JobHandlers;

internal interface IFileUtilitiesService
{
    /// <summary>
    /// Copies attachment file name, changes file extension to match target file type.
    /// Sets content type to match target file type.
    /// </summary>
    /// <param name="attachment">Source attachment.</param>
    /// <param name="targetFileType">Target file type.</param>
    /// <param name="targetFileSize">Target file size in bytes.</param>
    public Attachment CreateAttachment(Attachment attachment, FileType targetFileType, long targetFileSize);

    /// <summary>
    /// Asserts that the attachment is not null and has a non-empty id.
    /// </summary>
    /// <param name="attachment">Attachment to validate.</param>
    public void AssertValidAttachment(Attachment? attachment);

    /// <summary>
    /// Fetches a stream from the repository client.
    /// </summary>
    /// <param name="client">IJobRepositoryClient client.</param>
    /// <param name="attachment">Attachment information for fetching a stream.</param>
    /// <returns>A stream from repository service. Must be disposed after done.</returns>
    public Task<Stream> FetchStreamAsync(IJobRepositoryClient client, Attachment attachment);
}