using System.Text.Json.Serialization;

namespace Contracts
{
    /// <summary>
    /// Schema definition.
    /// </summary>
    public record PayloadJsonSchema : PropertyJsonSchema
    {
        public string Description { get; set; }

        public IEnumerable<string> Required { get; set; }

        public IDictionary<string, PropertyJsonSchema> Properties { get; set; }

        /// <summary>
        /// Enum definitions.
        /// </summary>
        public IDictionary<string, IDictionary<string, IEnumerable<string>>>? Definitions { get; set; }
    }

    /// <summary>
    /// Holds schema property value.
    /// </summary>
    public record PropertyJsonSchema
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Type { get; set; }

        public string Title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Default { get; set; }

        /// <summary>
        /// Holds reference to Definitions.
        /// #/definitions/locations
        /// </summary>
        [JsonPropertyName("$ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Ref { get; set; }
    }
}
