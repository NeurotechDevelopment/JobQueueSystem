namespace JobRepositoryService
{
    public interface IBlobStorage
    {
        Task<string> GetFileIdByTagNameAsync(string tag);

        Task<(string tag, string fileName, string contentType)> GetFileInfoAsync(string id);

        Task<string> UploadAsync(string tag, string blobName, string contentType, byte[] data);

        Task<string> UploadStreamAsync(string tag, string blobName, string contentType, Stream data);

        Task<Stream> DownloadStreamAsync(string id);

        Task<byte[]> DownloadAsync(string id);

        Task DeleteAsync(string id);
    }
}
