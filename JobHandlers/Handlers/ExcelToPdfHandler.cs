using Contracts;
using Contracts.FileTypes;
using Contracts.Payloads;
using Contracts.Payloads.Requests;
using Shared;
using Syncfusion.Pdf;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertExcelToPdf)]
    internal class ExcelToPdfHandler : JobHandler<ConvertExcelToPdfPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public ExcelToPdfHandler(ILogger<ExcelToPdfHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertExcelToPdf;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, ConvertExcelToPdfPayload? payload, Attachment? requestAttachment)
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
                XlsIORendererSettings settings = ParseRendererSettings(payload);

                //Convert Excel document into PDF document 
                PdfDocument pdfDocument = renderer.ConvertToPDF(workbook, settings);
                
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

        private static XlsIORendererSettings ParseRendererSettings(ConvertExcelToPdfPayload? payload)
        {
            var settings = new XlsIORendererSettings();
            if (payload == null)
            {
                return settings;
            }

            if (payload.PageSize != null)
            {
                var pageSize = settings.TemplateDocument.PageSettings.Size;
                pageSize.Height = payload.PageSize.Value.Height ?? pageSize.Height;
                pageSize.Width = payload.PageSize.Value.Width ?? pageSize.Width;
                settings.TemplateDocument.PageSettings.Size = pageSize;
            }

            if (payload.CustomPaperSize != null)
            {
                var customPaperSize = settings.CustomPaperSize;
                customPaperSize.Height = payload.CustomPaperSize.Value.Height ?? customPaperSize.Height;
                customPaperSize.Width = payload.CustomPaperSize.Value.Width ?? customPaperSize.Width;
                settings.CustomPaperSize = customPaperSize;
            }

            if (payload.ShowHeader.HasValue || payload.ShowFooter.HasValue)
            {
                settings.HeaderFooterOption.ShowHeader = payload.ShowHeader ?? settings.HeaderFooterOption.ShowHeader;
                settings.HeaderFooterOption.ShowFooter = payload.ShowFooter ?? settings.HeaderFooterOption.ShowFooter;
            }

            // Remaining are enums or bools. Exclude ShowHeader and ShowFooter as handled above separately.
            var simpleSettings = typeof(XlsIORendererSettings).GetProperties()
                .Where(x => (x.PropertyType == typeof(bool) || x.PropertyType.IsEnum) && x.Name != nameof(payload.ShowHeader) && x.Name != nameof(payload.ShowFooter));
            foreach (var simpleSetting in simpleSettings)
            {
                var payloadProp = payload.GetType().GetProperty(simpleSetting.Name);

                // No such property on payload, skip.
                if (payloadProp == null)
                {
                    continue;
                }

                var value = payloadProp.GetValue(payload);
                if (value != null)
                {
                    simpleSetting.SetValue(settings, value);
                }
            }

            return settings;
        }
    }
}
