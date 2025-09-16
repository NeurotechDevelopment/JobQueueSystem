using Contracts;
using Shared;
using Syncfusion.Pdf;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertWordToPdf)]
    internal sealed class WordToPdfHandler : JobHandler
    {
        public WordToPdfHandler(ILogger<JobHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertWordToPdf;

        protected override Task<string> PerformWorkAsync(Guid jobId, JobPayload payload)
        {
            var fileContent = Convert.FromBase64String(payload.Data);
            using (MemoryStream inputStream = new MemoryStream(fileContent))
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
                                return Task.FromResult(Convert.ToBase64String(outputStream.ToArray()));
                            }
                        }
                    }
                }
            }
        }
    }
}
