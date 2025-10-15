using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Contracts.Payloads.Requests
{
    public enum PdfConformanceLevel
    {
        None,
        Pdf_A1B,
        Pdf_X1A2001,
        Pdf_A2B,
        Pdf_A3B,
        Pdf_A1A,
        Pdf_A2A,
        Pdf_A2U,
        Pdf_A3A,
        Pdf_A3U,
        Pdf_A4,
        Pdf_A4E,
        Pdf_A4F
    }

    public enum GridLinesDisplayStyle
    {
        Auto,
        Visible,
        Invisible
    }

    public enum LayoutOptions
    {
        FitSheetOnOnePage = 1,
        NoScaling = 2,
        FitAllColumnsOnOnePage = 4,
        FitAllRowsOnOnePage = 8,
        CustomScaling = 16, // 0x00000010
        Automatic = 32, // 0x00000020
    }

    public enum PageOrientation
    {
        /// <summary>Portrait orientation.</summary>
        Portrait,
        /// <summary>Landscape orientation.</summary>
        Landscape
    }

    public struct SizeF
    {
        public float? Width
        {
            get;
            set;
        }
        public float? Height
        {
            get;
            set;
        }
    }

    public record ConvertExcelToPdfPayload : JobPayloadBase
    {
        public SizeF? CustomPaperSize
        {
            get;
            set;
        }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PageOrientation? PageOrientation
        {
            get;
            set;
        }

        public SizeF? PageSize
        {
            get;
            set;
        }

        [DefaultValue(Requests.PdfConformanceLevel.None)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PdfConformanceLevel? PdfConformanceLevel
        {
            get;
            set;
        } = Requests.PdfConformanceLevel.None;

        [DefaultValue(Requests.LayoutOptions.Automatic)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public LayoutOptions? LayoutOptions
        {
            get;
            set;
        } = Requests.LayoutOptions.Automatic;

        [DefaultValue(Requests.GridLinesDisplayStyle.Auto)]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public GridLinesDisplayStyle? DisplayGridLines
        {
            get;
            set;
        } = GridLinesDisplayStyle.Auto;

        [DefaultValue(true)]
        public bool? ShowHeader
        {
            get;
            set;
        } = true;

        [DefaultValue(true)]
        public bool? ShowFooter
        {
            get;
            set;
        } = true;

        public bool? RenderBySheet
        {
            get;
            set;
        }

        public bool? ShowFileNameWithExtension
        {
            get;
            set;
        }

        public bool? AutoDetectComplexScript
        {
            get;
            set;
        }

        public bool? EmbedFonts
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public bool? ExportBookmarks
        {
            get;
            set;
        } = true;

        [DefaultValue(true)]
        public bool? ExportDocumentProperties
        {
            get;
            set;
        } = true;

        public bool? ThrowWhenExcelFileIsEmpty
        {
            get;
            set;
        }

        public bool? ExportQualityImage
        {
            get;
            set;
        }

        [DefaultValue(true)]
        public bool? IsConvertBlankSheet
        {
            get;
            set;
        } = true;

        [DefaultValue(true)]
        public bool? IsConvertBlankPage
        {
            get;
            set;
        } = true;

        public bool? EnableFormFields
        {
            get;
            set;
        }

        public bool? AutoTag
        {
            get;
            set;
        }
    }
}
