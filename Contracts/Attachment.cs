namespace Contracts
{
    /// <summary>
    /// File for job requests and results.
    /// </summary>
    public record Attachment
    {
        public Attachment()
        {
        }

        public Attachment(string id, string fileName, string contentType, long size)
        {
            Id = id;
            FileName = fileName;
            ContentType = contentType;
            Size = size;
        }

        /// <summary>
        /// Id as assigned by the storage service.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Filename.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// File type (MIME).
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// File size in bytes.
        /// </summary>
        public long Size { get; set; }
        
    }
}
