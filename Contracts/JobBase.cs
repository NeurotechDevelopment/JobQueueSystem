using System.Text.Json.Serialization;

namespace Contracts
{
    public abstract class JobBase
    {
        public string Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStatus Status { get; set; }

        public DateTime? LastStatusChanged { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }
}
