using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Contracts.Payloads.Requests
{
    public record ConvertWordToImagesPayload : JobPayloadBase
    {
        [Display(Name = "Start page")]
        public int? StartPage { get; set; }

        [Display(Name = "Number of pages")]
        public int? NumberOfPages { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Display(Name = "Image type")]
        public ImageType TargetType { get; set; } = ImageType.Jpeg;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [Display(Name = "File format")]
        public WordFormat? WordFormat { get; set; }
    }

    /// <summary>
    /// Copied from Syncfusion.DocIO.FormatType because that is what this will convert to in a handler.
    /// We don't want any 3rd party nuget refs in this project.
    /// </summary>
    public enum WordFormat
    {
        /// <summary>Microsoft Word 97-2003 document format.</summary>
        Doc,
        /// <summary>Microsoft Word 97-2003 template format.</summary>
        Dot,
        /// <summary>Microsoft Word document format.</summary>
        Docx,
        /// <summary>Microsoft Strict Word document format.</summary>
        StrictDocx,
        /// <summary>Microsoft Word 2007 document format.</summary>
        Word2007,
        /// <summary>Microsoft Word 2010 document format.</summary>
        Word2010,
        /// <summary>Microsoft Word 2013 document format.</summary>
        Word2013,
        /// <summary>Microsoft Word 2007 template format.</summary>
        Word2007Dotx,
        /// <summary>Microsoft Word 2010 template format.</summary>
        Word2010Dotx,
        /// <summary>Microsoft Word 2013 template format.</summary>
        Word2013Dotx,
        /// <summary>Microsoft Word template format.</summary>
        Dotx,
        /// <summary>Microsoft Word 2007 macro enabled file format.</summary>
        Word2007Docm,
        /// <summary>Microsoft Word 2010 macro enabled file format.</summary>
        Word2010Docm,
        /// <summary>Microsoft Word 2013 macro enabled file format.</summary>
        Word2013Docm,
        /// <summary>Microsoft Word macro enabled file format.</summary>
        Docm,
        /// <summary>Microsoft Word 2007 macro enabled template format.</summary>
        Word2007Dotm,
        /// <summary>Microsoft Word 2010 macro enabled template format.</summary>
        Word2010Dotm,
        /// <summary>Microsoft Word 2013 macro enabled template format.</summary>
        Word2013Dotm,
        /// <summary>Microsoft Word macro enabled template format.</summary>
        Dotm,
        /// <summary>Specifies the WordProcessingML (.XML) documents</summary>
        /// <remarks> Provides Read/Write support for Microsoft Office WordXML format (FlatOPC) documents
        /// Provides read only support for Microsoft Office Word 2003 XML format documents
        /// </remarks>
        WordML,
        /// <summary>Rich text format (RTF).</summary>
        Rtf,
        /// <summary>Microsoft Windows text format.</summary>
        Txt,
        /// <summary>Markdown.</summary>
        Markdown,
        /// <summary>Support all Format Types.</summary>
        Automatic,
        /// <summary>Html format</summary>
        /// <remarks>This enum is not supported in Silverlight, Windows Phone, Universal and Universal Windows Platform applications.</remarks>
        Html,
        /// <summary>
        /// Saves theWord document as an Open Document Textformat.
        /// </summary>
        Odt
    }
}
