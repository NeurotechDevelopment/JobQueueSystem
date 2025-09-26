using Contracts;
using Contracts.Payloads;
using Shared;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Text;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertHtmlToPdf)]
    internal class HtmlToPdfHandler : JobHandler<EmptyPayload, EmptyPayload>
    {
        public HtmlToPdfHandler(ILogger<HtmlToPdfHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertHtmlToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            FileUtilities.AssertValidAttachment(requestAttachment);

            //Initialize HTML to PDF converter.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            //Create blink converter settings
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;
            
            //Convert URL to PDF document.
            var fileContentBytes = await this.client.DownloadAttachmentAsync(requestAttachment.Id);
            var fileContent = Encoding.UTF8.GetString(fileContentBytes);

            using (PdfDocument document = htmlConverter.Convert(fileContent, string.Empty))
            {
                //Saves the PDF file to memory stream.
                using (MemoryStream outputStream = new MemoryStream())
                {
                    document.Save(outputStream);
                    outputStream.Position = 0;

                    var length = outputStream.Length;
                    var newFileName = FileUtilities.ReplaceFileExtension(requestAttachment.FileName, "xlsx");
                    var contentType = "application/vnd.ms-excel";
                    var fileId = await this.client.UploadAttachmentAsync(jobId.ToString(),
                        newFileName, outputStream, contentType);

                    return (EmptyPayload.Instance, new Attachment(fileId, newFileName, contentType, length));
                }
            }
        }
    }
}
