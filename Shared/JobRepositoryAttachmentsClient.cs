using System.Net.Http.Headers;
using Contracts;
using RestSharp;

namespace Shared
{
    public partial class JobRepositoryClient
    {
        public string UploadAttachment(string tag, string fileName, Stream fileStream, string contentType, string? authToken = null)
        {
            if (fileStream.CanSeek)
            {
                // Ensure the stream position is at the beginning
                fileStream.Position = 0;
            }
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/octet-stream");
            var streamUploadUri = $"{this.JobServiceUrl}/{AttachmentsApiResource}/{ServicesConstants.ActionRoutes.Stream}/{tag}/{fileName}";
            var request = new HttpRequestMessage(HttpMethod.Post, streamUploadUri)
            {
                Content = content
            };

            if (!string.IsNullOrWhiteSpace(authToken) || this.IsAuthEnabled)
            {
                // In case authToken wasn't passed, but Auth is enabled, get a token.
                authToken = !string.IsNullOrWhiteSpace(authToken) ? authToken : this.GetAuthToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("Bearer ", string.Empty));
            }

            var response = HttpClient.Send(request);
            response.EnsureSuccessStatusCode();
            var fileId = response.Content.ReadAsStringAsync().Result;
            return fileId;
        }

        public async Task<string> UploadAttachmentAsync(string tag, string fileName, Stream fileStream, string contentType, string? authToken = null)
        {
            if (fileStream.CanSeek)
            {
                // Ensure the stream position is at the beginning
                fileStream.Position = 0;
            }

            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/octet-stream");
            var streamUploadUri = $"{this.JobServiceUrl}/{AttachmentsApiResource}/{ServicesConstants.ActionRoutes.Stream}/{tag}/{fileName}";

            var request = new HttpRequestMessage(HttpMethod.Post, streamUploadUri)
            {
                Content = content
            };

            if (!string.IsNullOrWhiteSpace(authToken) || this.IsAuthEnabled)
            {
                // In case authToken wasn't passed, but Auth is enabled, get a token.
                authToken = !string.IsNullOrWhiteSpace(authToken) ? authToken : this.GetAuthToken();
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken.Replace("Bearer ", string.Empty));
            }

            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var fileId = await response.Content.ReadAsStringAsync();
            return fileId;
        }

        public Stream DownloadAttachmentStream(string attachmentId)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{ServicesConstants.ActionRoutes.Stream}/{attachmentId}");
                return client.DownloadStream(request);
            }
        }

        public async Task<Stream> DownloadAttachmentStreamAsync(string attachmentId, CancellationToken token = default)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{ServicesConstants.ActionRoutes.Stream}/{attachmentId}");
                return await client.DownloadStreamAsync(request, token);
            }
        }

        public byte[] DownloadAttachment(string attachmentId)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{attachmentId}");
                return client.DownloadData(request);
            }
        }

        public async Task<byte[]> DownloadAttachmentAsync(string attachmentId)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                var request = new RestRequest($"{AttachmentsApiResource}/{attachmentId}");
                return await client.DownloadDataAsync(request);
            }
        }

        public void DeleteAttachment(string attachmentId)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                client.Delete($"{AttachmentsApiResource}/{attachmentId}");
            }
        }

        public async Task DeleteAttachmentAsync(string attachmentId)
        {
            using (var client = new RestClient(Options(JobServiceUrl)))
            {
                await client.DeleteAsync($"{AttachmentsApiResource}/{attachmentId}");
            }
        }
    }
}
