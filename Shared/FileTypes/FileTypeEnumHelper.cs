using System.Reflection;

namespace Shared.FileTypes
{
    public static class FileTypeEnumHelper
    {
        public const string DefaultMimeType = "application/octet-stream";

        public static string MimeType(this FileType fileType)
        {
            var type = typeof(FileType);
            var memInfo = type.GetMember(fileType.ToString());
            var mimeFileAttribute = memInfo.First().GetCustomAttribute<MimeTypeAttribute>(false);
            return mimeFileAttribute != null ? mimeFileAttribute.MimeType : DefaultMimeType;
        }
    }
}
