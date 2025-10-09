namespace Contracts
{
    public record PayloadJsonSchema : PropertyJsonSchema
    {
        public string Description { get; set; }

        public IEnumerable<string> Required { get; set; }

        public IDictionary<string, PropertyJsonSchema> Properties { get; set; }
    }

    public record PropertyJsonSchema
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public object? Default { get; set; }
    }
}
