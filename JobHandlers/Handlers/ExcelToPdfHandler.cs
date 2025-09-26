using Contracts;
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
        public ExcelToPdfHandler(ILogger<ExcelToPdfHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertExcelToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, EmptyPayload? payload, Attachment? requestAttachment)
        {
            FileUtilities.AssertValidAttachment(requestAttachment);

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = null;

                await using (var stream = await FileUtilities.FetchStreamAsync(this.client, requestAttachment))
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

                    var length = outputStream.Length;
                    var newFileName = FileUtilities.ReplaceFileExtension(requestAttachment.FileName, "pdf");
                    var contentType = "application/pdf";
                    var fileId = await this.client.UploadAttachmentAsync(jobId.ToString(),
                        newFileName, outputStream, contentType);
                    
                    return (EmptyPayload.Instance, new Attachment(fileId, newFileName, contentType, length) );
                }
            }
        }
    }
}
