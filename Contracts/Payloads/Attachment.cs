namespace Contracts.Payloads
{
    /// <summary>
    /// File for job requests and results.
    /// </summary>
    public class Attachment
    {
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
