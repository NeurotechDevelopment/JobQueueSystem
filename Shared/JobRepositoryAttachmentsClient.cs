namespace Shared
{
    public partial class JobRepositoryClient
    {
        public string UploadAttachment(string tag, string fileName, Stream fileStream, string contentType)
        {
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType ?? "application/octet-stream");
            var streamUploadUri = $"{this.jobServiceUrl.TrimEnd('/')}/{AttachmentsApiResource}/stream/{tag}/{fileName}";
            var response = HttpClient.PostAsync(streamUploadUri, content).Result;
            response.EnsureSuccessStatusCode();
            var fileId = response.Content.ReadAsStringAsync().Result;
            return fileId;
        }
    }
}
