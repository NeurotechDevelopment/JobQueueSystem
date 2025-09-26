using Contracts;
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
        public WordToPdfHandler(ILogger<WordToPdfHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertWordToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            FileUtilities.AssertValidAttachment(requestAttachment);

            using (var inputStream = await FileUtilities.FetchStreamAsync(this.client, requestAttachment))
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
    }
}
