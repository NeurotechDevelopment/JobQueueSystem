using System.Text.Json.Serialization;

namespace Contracts
{
    public class JobTypeDescriptor
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobType JobType { get; set; }

        /// <summary>
        /// User-friendly job type description.
        /// </summary>
        public required string Description { get; set; }

        /// <summary>
        /// Json schema of a job request payload.
        /// Consumers may include React GUI that needs to dynamically construct forms.
        /// </summary>
        public required PayloadJsonSchema PayloadJsonSchema { get; set; }

        /// <summary>
        /// Json schema of a job result payload.
        /// Consumers may include React GUI that needs to dynamically construct forms.
        /// </summary>
        public required PayloadJsonSchema ResultJsonSchema { get; set; }

        /// <summary>
        /// A collection of file extensions allowed for JobRequest payload.
        /// </summary>
        public IEnumerable<string> AllowedAttachments { get; set; }
    }
}
