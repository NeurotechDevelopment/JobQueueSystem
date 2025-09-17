using System.ComponentModel;

namespace Contracts
{
    public enum JobStatus
    {
        NotStarted,
        Enqueued,
        InProgress,
        Failed,
        Finished
    }

    public enum JobType
    {
        [Description("Used for testing purposes.")]
        Dummy,

        [Description("Convert MS Word document to PDF document.")]
        ConvertWordToPdf,

        [Description("Convert Html document to PDF document.")]
        ConvertHtmlToPdf,

        [Description("Convert Excel document to PDF document.")]
        ConvertExcelToPdf,

        [Description("Convert scanned PDF to a searchable PDF document.")]
        ConvertScanToSearchablePdf
    }
}