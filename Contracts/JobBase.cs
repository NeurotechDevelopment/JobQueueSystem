using System.Text.Json.Serialization;

namespace Contracts
{
    public abstract record JobBase
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobType Type { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public JobStatus Status { get; set; }

        public DateTime? LastStatusChanged { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public DateTime? FinishedAt { get; set; }
    }
}
