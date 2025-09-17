using Contracts;
using Shared;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertExcelToPdf)]
    internal class ExcelToPdfHandler : JobHandler
    {
        public ExcelToPdfHandler(ILogger<JobHandler> logger, IJobRepositoryClient client) : base(logger, client)
        {
        }

        public override JobType Handles => JobType.ConvertExcelToPdf;

        protected override Task<string> PerformWorkAsync(Guid jobId, JobPayload payload)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                var fileContent = Convert.FromBase64String(payload.Data);
                IWorkbook workbook = null;
                using (MemoryStream inputStream = new MemoryStream(fileContent))
                {
                    workbook = application.Workbooks.Open(inputStream);
                }

                //Initialize XlsIO renderer.
                XlsIORenderer renderer = new XlsIORenderer();

                //Convert Excel document into PDF document 
                PdfDocument pdfDocument = renderer.ConvertToPDF(workbook);

                using(var outputStream = new MemoryStream())
                {
                    pdfDocument.Save(outputStream);
                    outputStream.Position = 0;
                    return Task.FromResult(Convert.ToBase64String(outputStream.ToArray()));
                }
            }
        }
    }
}
