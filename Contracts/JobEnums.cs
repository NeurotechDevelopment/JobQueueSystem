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

        [Description("Converts MS Word document to PDF document.")]
        ConvertWordToPdf,

        [Description("Converts Html document to PDF document.")]
        ConvertHtmlToPdf,

        [Description("Converts Excel document to PDF document.")]
        ConvertExcelToPdf
    }
}