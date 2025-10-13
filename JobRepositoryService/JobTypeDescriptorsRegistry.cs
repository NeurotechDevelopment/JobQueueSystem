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
                if (pi.PropertyType.IsPrimitiveType())
                {
                    props[pi.Name] = new PropertyJsonSchema
                    {
                        Type = pi.PropertyType.ToJsonType(),
                        Title = pi.Name,
                        Default = null
                    };
                }
                else
                {
                    props[pi.Name] = GenerateSchema(pi.PropertyType);
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
                JobType.ConvertWordToImages => new [] { "doc", "docx" },
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
                case JobType.ConvertWordToImages:
                    return typeof(ConvertWordToImagesPayload);
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
                case JobType.ConvertWordToImages:
                    return typeof(EmptyPayload);
                default:
                    throw new NotImplementedException($"No payload binding found for a job type {jobType}");
            }
        }


        #region Extension methods for types

        private static bool IsPrimitiveType(this Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
            {
                return true;
            }

            if (type.IsNullableType() && type.IsGenericType)
            {
                return type.GenericTypeArguments[0].IsPrimitiveType();
            }

            return false;
        }

        private static string ToJsonType(this Type type)
        {
            // Unwrap nullable types
            if (Nullable.GetUnderlyingType(type) is Type underlying)
            {
                type = underlying;
            }

            if (type.IsEnum)
            {
                return "string";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type == typeof(bool))
            {
                return "boolean";
            }

            if (type == typeof(byte) || type == typeof(short) ||
                type == typeof(int) || type == typeof(long))
            {
                return "integer";
            }

            if (type == typeof(float) || type == typeof(double) ||
                type == typeof(decimal))
            {
                return "number";
            }

            if (type.IsArray || (type.IsGenericType &&
                                 typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition())))
            {
                return "array";
            }

            if (type.IsClass || type.IsValueType)
            {
                return "object";
            }

            return "string"; // fallback
        }

        #endregion
    }
}
