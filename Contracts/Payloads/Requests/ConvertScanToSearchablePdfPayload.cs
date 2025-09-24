namespace Contracts.Payloads.Requests
{
    public record ConvertScanToSearchablePdfPayload : JobPayloadBase
    {
        /// <summary>
        /// Language for OCR processing, e.g. "eng" for English, "fra" for French.
        /// </summary>
        public string? Language { get; set; }
    }
}
