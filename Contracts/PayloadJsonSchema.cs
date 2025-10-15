using System.Text.Json.Serialization;

namespace Contracts
{
    /// <summary>
    /// Schema definition or schema property. Has 2-fold purpose: hold the schema for a payload.
    /// Each property can also be a schema itself. When not a schema, only Title and Type are set.
    /// This is done because of serialization issues if we were to use separate record for property item.
    /// </summary>
    public record PayloadJsonSchema
    {
        public string Title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Description { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Default { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<string>? Required { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, PayloadJsonSchema> Properties { get; set; }

        /// <summary>
        /// Enum definitions.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, IDictionary<string, IEnumerable<string>>>? Definitions { get; set; }

        /// <summary>
        /// Holds reference to Definitions.
        /// #/definitions/locations
        /// </summary>
        [JsonPropertyName("$ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Ref { get; set; }
    }
}
