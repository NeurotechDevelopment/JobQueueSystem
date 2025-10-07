using Contracts;
using Contracts.Payloads;
using Contracts.Payloads.Requests;
using NJsonSchema;

namespace JobRepositoryService
{
    internal static class JobTypePayloadRegistry
    {
        public static string GetPayloadSchema(JobType jobType)
        {
            var payloadType = GetPayloadSchemaType(jobType);
            var schema = JsonSchema.FromType(payloadType);
            schema.Title = payloadType.Name;
            return schema.ToJson();
        }

        public static string GetResultPayloadSchema(JobType jobType)
        {
            var payloadType = GetResultPayloadSchemaType(jobType);
            var schema = JsonSchema.FromType(payloadType);
            schema.Title = payloadType.Name;
            return schema.ToJson();
        }

        private static Type GetPayloadSchemaType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.Dummy:
                    return typeof(SimpleMessagePayload);
                case JobType.ConvertExcelToPdf:
                case JobType.ConvertHtmlToPdf:
                case JobType.ConvertWordToPdf:
                    return typeof(EmptyPayload);
                case JobType.ConvertScanToSearchablePdf:
                    return typeof(ConvertScanToSearchablePdfPayload);
                default:
                    throw new NotImplementedException($"No payload binding found for a job type {jobType}");
            }
        }

        private static Type GetResultPayloadSchemaType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.Dummy:
                    return typeof(SimpleMessagePayload);
                case JobType.ConvertExcelToPdf:
                case JobType.ConvertHtmlToPdf:
                case JobType.ConvertWordToPdf:
                case JobType.ConvertScanToSearchablePdf:
                    return typeof(EmptyPayload);
                default:
                    throw new NotImplementedException($"No payload binding found for a job type {jobType}");
            }
        }
    }
}
