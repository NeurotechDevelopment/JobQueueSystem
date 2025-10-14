using System.Text.Json.Serialization;

namespace Contracts.Payloads.Requests
{
    public record ConvertScanToSearchablePdfPayload : JobPayloadBase
    {
        /// <summary>
        /// Language for OCR processing, e.g. "eng" for English, "fra" for French.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PdfScanLanguage? Language { get; set; }
    }

    public enum PdfScanLanguage
    {
        Bulgarian,
        English,
        French, 
        Polish, 
        Russian,
        Ukrainian
    }
}
