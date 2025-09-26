using Contracts.Payloads.Requests;

namespace Contracts.Payloads
{
    public record JobPayload : JobPayloadBase
    {
        public JobPayload()
        {
        }
        
        public JobPayload(string? data)
        {
            Data = data;
        }

        public JobPayload(string? data, Attachment? attachment)
        {
            Data = data;
            Attachment = attachment;
        }

        /// <summary>
        /// Some strongly-typed data as JSON string.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Represents an optional file associated with the payload.
        /// </summary>
        public Attachment? Attachment { get; set; }
    }
}
