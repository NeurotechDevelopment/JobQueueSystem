using Contracts;
using Shared;
using Syncfusion.Pdf.Parsing;
using Syncfusion.OCRProcessor;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertScanToSearchablePdf)]
    internal class ScanPdfToSearchablePdfHandler : JobHandler
    {
        public ScanPdfToSearchablePdfHandler(ILogger<JobHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertScanToSearchablePdf;

        protected override Task<string> PerformWorkAsync(Guid jobId, JobPayload payload)
        {
            // Initialize the OCR processor
            using (OCRProcessor processor = new OCRProcessor())
            {
                var fileContent = Convert.FromBase64String(payload.Data);
                using (MemoryStream stream = new MemoryStream(fileContent))
                {
                    using (PdfLoadedDocument pdfLoadedDocument = new PdfLoadedDocument(stream))
                    {
                        // Set OCR language to process
                        processor.Settings.Language = Languages.English;

                        // Process OCR by providing the PDF document
                        processor.PerformOCR(pdfLoadedDocument);

                        //Create file stream.
                        using (MemoryStream outputFileStream = new MemoryStream())
                        {
                            //Save the PDF document to file stream.
                            pdfLoadedDocument.Save(outputFileStream);

                            return Task.FromResult(Convert.ToBase64String(outputFileStream.ToArray()));
                        }
                    }
                }
            }
        }
    }
}
