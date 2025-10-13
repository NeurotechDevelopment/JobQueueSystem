namespace Contracts.FileTypes
{
    /// <summary>
    /// To decorate FileType enum values with MIME types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    internal sealed class MimeTypeAttribute : Attribute
    {
        internal string MimeType { get; }
        internal MimeTypeAttribute(string mimeType)
        {
            MimeType = mimeType;
        }
    }
}
