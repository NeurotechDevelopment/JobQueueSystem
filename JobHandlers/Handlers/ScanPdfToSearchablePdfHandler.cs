using Contracts;
using Contracts.Payloads;
using Shared;
using Syncfusion.Pdf.Parsing;
using Syncfusion.OCRProcessor;
using Contracts.Payloads.Requests;
using Shared.FileTypes;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertScanToSearchablePdf)]
    internal class ScanPdfToSearchablePdfHandler : JobHandler<ConvertScanToSearchablePdfPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public ScanPdfToSearchablePdfHandler(ILogger<ScanPdfToSearchablePdfHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertScanToSearchablePdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, ConvertScanToSearchablePdfPayload? payload, Attachment? requestAttachment)
        {
            this.fileService.AssertValidAttachment(requestAttachment);

            // Initialize the OCR processor
            using (OCRProcessor processor = new OCRProcessor())
            {
                await using var stream = await this.fileService.FetchStreamAsync(this.client, requestAttachment);
                using (PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream))
                {
                    // Set OCR language to process
                    processor.Settings.Language = payload.Language;

                    // Process OCR by providing the PDF document
                    processor.PerformOCR(pdfLoadedDocument);

                    //Create file stream.
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        //Save the PDF document to file stream.
                        pdfLoadedDocument.Save(outputStream);

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
}
