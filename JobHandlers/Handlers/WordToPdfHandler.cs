using Contracts;
using Contracts.FileTypes;
using Contracts.Payloads;
using Shared;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertWordToPdf)]
    internal sealed class WordToPdfHandler : JobHandler<EmptyPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public WordToPdfHandler(ILogger<WordToPdfHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertWordToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            this.fileService.AssertValidAttachment(requestAttachment);

            this.logger.LogTrace($"Fetching attachment with id {requestAttachment.Id}");
            using (var inputStream = await this.fileService.FetchStreamAsync(this.client, requestAttachment))
            {
                this.logger.LogTrace("Opening stream with WordDocument");

                //Loads an existing Word document.
                using (WordDocument wordDocument = new WordDocument(inputStream, FormatType.Automatic))
                {
                    this.logger.LogTrace("Creating an instance of DocIORenderer");

                    //Creates an instance of DocIORenderer.
                    using (DocIORenderer renderer = new DocIORenderer())
                    {

                        this.logger.LogTrace("Begin converting with DocIORenderer.");

                        //Converts Word document into PDF document.
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {

                            this.logger.LogTrace("Converted with DocIORenderer. Saving to stream.");

                            //Saves the PDF file to file system.    
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                pdfDocument.Save(outputStream);
                                outputStream.Position = 0;
                                
                                var resultAttachment = this.fileService.CreateAttachment(requestAttachment, FileType.Pdf, outputStream.Length);

                                this.logger.LogTrace($"Created attachment ContentType: {resultAttachment.ContentType}, Filename: {resultAttachment.FileName}. Uploading.");

                                resultAttachment.Id = await this.client.UploadAttachmentAsync(
                                    jobId.ToString(),
                                    resultAttachment.FileName,
                                    outputStream,
                                    resultAttachment.ContentType);

                                this.logger.LogTrace($"Uploaded attachment with id {resultAttachment.Id}.");

                                return (EmptyPayload.Instance, resultAttachment);
                            }
                        }
                    }
                }
            }
        }
    }
}
