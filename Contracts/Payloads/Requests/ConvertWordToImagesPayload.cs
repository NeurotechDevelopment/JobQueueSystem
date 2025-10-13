using System.Text.Json.Serialization;

namespace Contracts.Payloads.Requests
{
    public record ConvertWordToImagesPayload : JobPayloadBase
    {
        public int? StartPage { get; set; }

        public int? NumberOfPages { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ImageType TargetType { get; set; } = ImageType.Jpeg;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public WordFormat? WordFormat { get; set; }
    }

    public enum WordFormat
    {
        Doc,
        Docx,
    }
}
