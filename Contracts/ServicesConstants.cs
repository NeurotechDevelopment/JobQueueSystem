namespace Contracts
{
    public class ServicesConstants
    {
        private const string ApiPrefix = "api";

        public const string JobsRepository = "JobsRepository";

        public const string JobsAttachments = "JobsAttachments";

        public const string JobsEntity = "Jobs";

        public const string OdataRoutePrefix = "Odata";

        public class ControllerRoutes
        {
            public const string JobsApiResource = $"{ApiPrefix}/{JobsRepository}";

            public const string OdataJobsApiResource = $"{OdataRoutePrefix}/{JobsEntity}";

            public const string AttachmentsApiResource = $"{ApiPrefix}/{JobsAttachments}";
        }

        public class ActionRoutes
        {
            public const string Jobs = "jobs";
            public const string JobTypes = "job-types";
            public const string Stream = "stream";
            public const string DownloadTempFileRoute = "download-temp-file";
        }
    }
}
