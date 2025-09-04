using System.ComponentModel;

namespace Contracts
{
    public enum JobType
    {
        [Description("Used for testing purposes.")]
        Dummy,

        [Description("Converts MS Word document to PDF document.")]
        ConvertWordToPdf
    }
}
