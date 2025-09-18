using System.ComponentModel.DataAnnotations;

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
        [Display(Name = nameof(Dummy), Description = "Used for testing purposes.")]
        Dummy,

        [Display(Name = nameof(ConvertWordToPdf), Description = "Convert MS Word document to PDF document.")]
        ConvertWordToPdf,

        [Display(Name = nameof(ConvertHtmlToPdf), Description = "Convert Html document to PDF document.")]
        ConvertHtmlToPdf,

        [Display(Name = nameof(ConvertExcelToPdf), Description = "Convert Excel document to PDF document.")]
        ConvertExcelToPdf,

        [Display(Name = nameof(ConvertScanToSearchablePdf), Description = "Convert scanned PDF to a searchable PDF document.")]
        ConvertScanToSearchablePdf
    }
}