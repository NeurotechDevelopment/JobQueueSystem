using RestSharp;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        public string UploadAttachment(string tag, string fileName, Stream fileStream, string contentType)
        {
            if (fileStream.CanSeek)
            {
                // Ensure the stream position is at the beginning
                fileStream.Position = 0;
            }
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType ?? "application/octet-stream");
            var streamUploadUri = $"{this.jobServiceUrl.TrimEnd('/')}/{AttachmentsApiResource}/stream/{tag}/{fileName}";
            var response = HttpClient.PostAsync(streamUploadUri, content).Result;
            response.EnsureSuccessStatusCode();
            var fileId = response.Content.ReadAsStringAsync().Result;
            return fileId;
        }

        public async Task<string> UploadAttachmentAsync(string tag, string fileName, Stream fileStream, string contentType)
        {
            if (fileStream.CanSeek)
            {
                // Ensure the stream position is at the beginning
                fileStream.Position = 0;
            }

            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType ?? "application/octet-stream");
            var streamUploadUri = $"{this.jobServiceUrl.TrimEnd('/')}/{AttachmentsApiResource}/stream/{tag}/{fileName}";
            var response = await HttpClient.PostAsync(streamUploadUri, content);
            response.EnsureSuccessStatusCode();
            var fileId = await response.Content.ReadAsStringAsync();
            return fileId;
        }

        public Stream DownloadAttachmentStream(string attachmentId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/stream/{attachmentId}");
                return client.DownloadStream(request);
            }
        }

        public async Task<Stream> DownloadAttachmentStreamAsync(string attachmentId, CancellationToken token = default)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/stream/{attachmentId}");
                return await client.DownloadStreamAsync(request, token);
            }
        }

        public byte[] DownloadAttachment(string attachmentId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{attachmentId}");
                return client.DownloadData(request);
            }
        }

        public async Task<byte[]> DownloadAttachmentAsync(string attachmentId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{attachmentId}");
                return await client.DownloadDataAsync(request);
            }
        }

        public void DeleteAttachment(string attachmentId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                client.Delete($"{AttachmentsApiResource}/{attachmentId}");
            }
        }

        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            using (var client = new RestClient(jobServiceUrl))
            {
                await client.DeleteAsync($"{AttachmentsApiResource}/{attachmentId}");
            }
        }
    }
}
