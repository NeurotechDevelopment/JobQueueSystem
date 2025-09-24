namespace Contracts
{
    public class ServicesConstants
    {
        private const string ApiPrefix = "api";

        private const string JobsRepository = "JobsRepository";

        private const string JobsAttachments = "JobsAttachments";

        public const string JobsEntity = "Jobs";

        public const string OdataRoutePrefix = "Odata";

        public const string JobTypesUrlSegment = "job-types";

        public class ServiceResources
        {
            public const string JobsApiResource = $"{ApiPrefix}/{JobsRepository}";

            public const string OdataJobsApiResource = $"{OdataRoutePrefix}/{JobsEntity}";

            public const string AttachmentsApiResource = $"{ApiPrefix}/{JobsAttachments}";
        }
    }
}
