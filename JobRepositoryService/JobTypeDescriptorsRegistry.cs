using AutoMapper.Internal;
using Contracts;
using Contracts.Payloads;
using Contracts.Payloads.Requests;

namespace JobRepositoryService
{
    internal static class JobTypePayloadRegistry
    {
        public static PayloadJsonSchema GetPayloadSchema(JobType jobType)
        {
            var payloadType = GetPayloadSchemaType(jobType);
            return GenerateSchema(payloadType);
        }

        public static PayloadJsonSchema GetResultPayloadSchema(JobType jobType)
        {
            var payloadType = GetResultPayloadSchemaType(jobType);
            return GenerateSchema(payloadType);
        }

        private static PayloadJsonSchema GenerateSchema(Type payloadType)
        {
            var requiredProperties = new List<string>();
            var schema = new PayloadJsonSchema
            {
                Title = payloadType.Name,
                Description = payloadType.FullName,
                Type = "object"
            };

            var props = new Dictionary<string, PropertyJsonSchema>();
            foreach (var pi in payloadType.GetProperties())
            {
                // Add to schema Required
                if (!pi.PropertyType.IsNullableType())
                {
                    requiredProperties.Add(pi.Name);
                }

                // Complex type recurses, simple go as a property.
                if (!pi.PropertyType.IsPrimitive && pi.PropertyType != typeof(string))
                {
                    props[pi.Name] = GenerateSchema(pi.PropertyType);
                }
                else
                {
                    props[pi.Name] = new PropertyJsonSchema
                    {
                        Type = pi.PropertyType.Name.ToLower(),
                        Title = pi.Name,
                        Default = null
                    };
                }
            }

            schema.Required = requiredProperties;
            schema.Properties = props;

            return schema;
        }

        /// <summary>
        /// * means any allowed. Empty means attachment not allowed.
        /// </summary>
        /// <param name="jobType">Job type.</param>
        /// <returns>Collection of file extensions allowed for upload for a JobRequest.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<string> GetAllowedFileExtensions(JobType jobType)
        {
            return jobType switch
            {
                JobType.Dummy => new string[] {},
                JobType.ConvertExcelToPdf => new[] { "xls", "xlsx" },
                JobType.ConvertHtmlToPdf => new[] { "htm", "html" },
                JobType.ConvertWordToPdf => new[] { "doc", "docx" },
                JobType.ConvertScanToSearchablePdf => new[] { "pdf" },
                _ => throw new NotImplementedException($"No payload binding found for a job type {jobType}")
            };
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
