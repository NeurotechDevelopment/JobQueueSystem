namespace Contracts.FileTypes;

/// <summary>
/// Lists supported file types together with their MIME types in a <see cref="MimeTypeAttribute"/>.
/// </summary>
public enum FileType
{
    [MimeType(FileTypeEnumHelper.DefaultMimeType)]
    Any,
    [MimeType("application/pdf")]
    Pdf,
    [MimeType("application/msword")]
    Doc,
    [MimeType("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    Docx,
    [MimeType("application/vnd.ms-excel")]
    Xls,
    [MimeType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    Xlsx,
    [MimeType("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    Pptx,
    [MimeType("text/plain")]
    Txt,
    [MimeType("text/html")]
    Html,
    [MimeType(FileTypeEnumHelper.DefaultMimeType)]
    Zip,
    [MimeType("image/jpeg")]
    Jpeg,
    [MimeType("image/png")]
    Png
}