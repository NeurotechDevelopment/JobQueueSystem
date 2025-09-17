using System.Text;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using Contracts;
using Shared;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertHtmlToPdf)]
    internal class HtmlToPdfHandler : JobHandler
    {
        public HtmlToPdfHandler(ILogger<JobHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertHtmlToPdf;

        protected override Task<string> PerformWorkAsync(Guid jobId, JobPayload payload)
        {
            //Initialize HTML to PDF converter.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            //Create blink converter settings
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;
            //Convert URL to PDF document.
            var fileContentBytes = Convert.FromBase64String(payload.Data);
            var fileContent = Encoding.UTF8.GetString(fileContentBytes);
            using (PdfDocument document = htmlConverter.Convert(fileContent, string.Empty ))
            {
                //Saves the PDF file to memory stream.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    document.Save(outputStream);
                    outputStream.Position = 0;
                    return Task.FromResult(Convert.ToBase64String(outputStream.ToArray()));
                }
            }
        }
    }
}
