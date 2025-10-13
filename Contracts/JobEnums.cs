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

        [Display(Name = nameof(ConvertWordToPdf), Description = "Converts MS Word document to PDF document.")]
        ConvertWordToPdf,

        [Display(Name = nameof(ConvertHtmlToPdf), Description = "Converts Html document to PDF document.")]
        ConvertHtmlToPdf,

        [Display(Name = nameof(ConvertExcelToPdf), Description = "Converts Excel document to PDF document.")]
        ConvertExcelToPdf,

        [Display(Name = nameof(ConvertScanToSearchablePdf), Description = "Converts scanned PDF to a searchable PDF document.")]
        ConvertScanToSearchablePdf,

        [Display(Name = nameof(ConvertWordToImages), Description = "Converts MS Word document to images.")]
        ConvertWordToImages
    }
}