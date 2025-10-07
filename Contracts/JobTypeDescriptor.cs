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
        public string? PayloadJsonSchema { get; set; }

        /// <summary>
        /// Json schema of a job result payload.
        /// Consumers may include React GUI that needs to dynamically construct forms.
        /// </summary>
        public string? ResultJsonSchema { get; set; }
    }
}
