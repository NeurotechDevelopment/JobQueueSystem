using Contracts;
using Contracts.FileTypes;
using Contracts.Payloads;
using Shared;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertExcelToPdf)]
    internal class ExcelToPdfHandler : JobHandler<EmptyPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public ExcelToPdfHandler(ILogger<ExcelToPdfHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertExcelToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            this.fileService.AssertValidAttachment(requestAttachment);

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = null;

                await using (var stream = await this.fileService.FetchStreamAsync(this.client, requestAttachment))
                {
                    workbook = application.Workbooks.Open(stream);
                }

                //Initialize XlsIO renderer.
                XlsIORenderer renderer = new XlsIORenderer();

                //Convert Excel document into PDF document 
                PdfDocument pdfDocument = renderer.ConvertToPDF(workbook);

                await using (var outputStream = new MemoryStream())
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
