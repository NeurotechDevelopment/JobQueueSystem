using Contracts;
using Contracts.Payloads;
using Shared;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Text;
using Shared.FileTypes;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertHtmlToPdf)]
    internal class HtmlToPdfHandler : JobHandler<EmptyPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public HtmlToPdfHandler(ILogger<HtmlToPdfHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertHtmlToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            this.fileService.AssertValidAttachment(requestAttachment);

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

                    var resultAttachment = this.fileService.CreateAttachment(requestAttachment, FileType.Pdf, outputStream.Length);
                    resultAttachment.Id = await this.client.UploadAttachmentAsync(
                        jobId.ToString(),
                        resultAttachment.FileName,
                        outputStream,
                        resultAttachment.ContentType);

                    return (EmptyPayload.Instance, resultAttachment);
                }
            }
        }
    }
}
