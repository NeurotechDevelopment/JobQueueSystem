using System.Text.Json.Serialization;

namespace Contracts
{
    public class JobTypeDescriptor
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobType JobType { get; set; }

        public string Description { get; set; }
    }
}
