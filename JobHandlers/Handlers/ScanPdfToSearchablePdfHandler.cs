using System.Text.Json;
using Contracts;
using Contracts.Payloads;
using Shared;
using Syncfusion.Pdf.Parsing;
using Syncfusion.OCRProcessor;
using Contracts.Payloads.Requests;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertScanToSearchablePdf)]
    internal class ScanPdfToSearchablePdfHandler : JobHandler<ConvertScanToSearchablePdfPayload, EmptyPayload>
    {
        public ScanPdfToSearchablePdfHandler(ILogger<ScanPdfToSearchablePdfHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertScanToSearchablePdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, ConvertScanToSearchablePdfPayload? payload, Attachment? requestAttachment)
        {
            FileUtilities.AssertValidAttachment(requestAttachment);

            // Initialize the OCR processor
            using (OCRProcessor processor = new OCRProcessor())
            {
                await using var stream = await FileUtilities.FetchStreamAsync(this.client, requestAttachment);
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

                        var length = outputStream.Length;
                        var newFileName = FileUtilities.ReplaceFileExtension(requestAttachment.FileName, "pdf");
                        var contentType = "application/pdf";
                        var fileId = await this.client.UploadAttachmentAsync(jobId.ToString(),
                            newFileName, outputStream, contentType);

                        return (EmptyPayload.Instance, new Attachment(fileId, newFileName, contentType, length));
                    }
                }
            }
        }
    }
}
