using System.Text.Json.Serialization;

namespace Contracts
{
    public class JobTypeDescriptor
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobType Key { get; set; }

        public string Description { get; set; }
    }
}
