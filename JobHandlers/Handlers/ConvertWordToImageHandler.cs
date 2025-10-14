using Contracts;
using Contracts.FileTypes;
using Contracts.Payloads;
using Contracts.Payloads.Requests;
using Shared;
using Syncfusion.Compression.Zip;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using FileAttributes = Syncfusion.Compression.FileAttributes;
using ImageType = Contracts.ImageType;

namespace JobHandlers.Handlers
{
    [JobTypeHandler(JobType.ConvertWordToImages)]
    internal sealed class ConvertWordToImageHandler : JobHandler<ConvertWordToImagesPayload, EmptyPayload>
    {
        private readonly IFileUtilitiesService fileService;

        public ConvertWordToImageHandler(ILogger<ConvertWordToImageHandler> logger, IJobRepositoryClient client, IFileUtilitiesService fileService) : base(logger, client)
        {
            this.fileService = fileService;
        }

        public override JobType Handles => JobType.ConvertWordToImages;

        protected override async Task<(EmptyPayload Result, Attachment? ResultFile)> PerformWorkAsync(Guid jobId, ConvertWordToImagesPayload? payload, Attachment? requestAttachment)
        {
            this.fileService.AssertValidAttachment(requestAttachment);
            ArgumentNullException.ThrowIfNull(payload);
            await using (var docStream = await this.fileService.FetchStreamAsync(this.client, requestAttachment))
            {
                var formatType = InferWordType(payload, requestAttachment);
                //Load file stream into Word document.
                using (WordDocument wordDocument = new WordDocument(docStream, formatType))
                {
                    //Create a new instance of DocIORenderer class.
                    using (DocIORenderer render = new DocIORenderer())
                    {
                        //Convert the first page of the Word document into an image.
                        var startPage = payload.StartPage.GetValueOrDefault(0);
                        Attachment resultAttachment = null;
                        // Multiple pages will save to zip archive as a collection of images
                        if (payload.NumberOfPages.HasValue)
                        {
                            resultAttachment = await CreateRangeImages(jobId, payload.TargetType, wordDocument,
                                requestAttachment, startPage, payload.NumberOfPages.Value);
                        }
                        else
                        {
                            // Single page is saved as an image
                            var exportImageFormat = Enum.Parse<ExportImageFormat>(payload.TargetType.ToString(), ignoreCase: true);
                            resultAttachment = await CreateSinglePageImage(jobId, payload.TargetType, wordDocument,
                                requestAttachment, startPage, exportImageFormat);

                        }
                        
                        return (EmptyPayload.Instance, resultAttachment);
                    }
                }
            }
        }

        private async Task<Attachment> CreateSinglePageImage(Guid jobId, ImageType targetType, WordDocument wordDocument, Attachment requestAttachment, int startPage, ExportImageFormat exportImageFormat)
        {
            var imageStream = wordDocument.RenderAsImages(startPage, exportImageFormat);
            //Reset the stream position.
            imageStream.Position = 0;
            //Save the stream as file.
            var resultAttachment = this.fileService.CreateAttachment(requestAttachment, (FileType)targetType,
                imageStream.Length);
            resultAttachment.Id = await this.client.UploadAttachmentAsync(
                jobId.ToString(),
                resultAttachment.FileName,
                imageStream,
                resultAttachment.ContentType);
            return resultAttachment;
        }

        private async Task<Attachment> CreateRangeImages(Guid jobId, ImageType targetType, WordDocument wordDocument,
            Attachment requestAttachment, int startPage, int pageCount)
        {
            Stream[] pagesStreams = wordDocument.RenderAsImages(startPage, pageCount);
            var baseFileName = Path.GetFileNameWithoutExtension(requestAttachment.FileName);
            long archiveSize = 0;
            using (var archive = new ZipArchive())
            {
                for (var i = 0; i < pagesStreams.Length; i++)
                {
                    var stream = pagesStreams[i];
                    var item = archive.AddItem($"{baseFileName}_{i + 1}.{targetType.ToString().ToLower()}", stream, bControlStream: true,
                        FileAttributes.Normal);
                    archiveSize += item.DataStream.Length;

                }

                using (var ms = new MemoryStream())
                {
                    archive.Save(ms, closeStream: false);
                    ms.Position = 0;
                    var resultAttachment =
                        this.fileService.CreateAttachment(requestAttachment, FileType.Zip, archiveSize);
                    resultAttachment.Id = await this.client.UploadAttachmentAsync(jobId.ToString(),
                        resultAttachment.FileName, ms,
                        resultAttachment.ContentType);
                    return resultAttachment;
                }
            }
        }

        private FormatType InferWordType(ConvertWordToImagesPayload payload, Attachment attachment)
        {
            FormatType retVal;
            if (payload.WordFormat.HasValue &&
                Enum.TryParse<FormatType>(payload.WordFormat.Value.ToString(), ignoreCase: true, out retVal))
            {
                return retVal;
            }

            var fileType = this.fileService.TryParseFileType(attachment);
            return Enum.TryParse<FormatType>(fileType.ToString(), ignoreCase: true, out retVal) ? retVal : FormatType.Automatic;
        }
    }
}
