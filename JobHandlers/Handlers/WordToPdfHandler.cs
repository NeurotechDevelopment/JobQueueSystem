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

            using (var inputStream = await this.fileService.FetchStreamAsync(this.client, requestAttachment))
            {
                //Loads an existing Word document.
                using (WordDocument wordDocument = new WordDocument(inputStream, FormatType.Automatic))
                {
                    //Creates an instance of DocIORenderer.
                    using (DocIORenderer renderer = new DocIORenderer())
                    {
                        //Converts Word document into PDF document.
                        using (PdfDocument pdfDocument = renderer.ConvertToPDF(wordDocument))
                        {
                            //Saves the PDF file to file system.    
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                pdfDocument.Save(outputStream);
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
        }
    }
}
