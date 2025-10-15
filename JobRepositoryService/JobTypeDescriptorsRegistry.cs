using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using AutoMapper.Internal;
using Contracts;
using Contracts.Payloads;
using Contracts.Payloads.Requests;

namespace JobRepositoryService
{
    /// <summary>
    /// Contains definitions per <see cref="JobType"/> for Request/Response payloads, allowed file names.
    /// Generates json schema per each payload.
    /// </summary>
    internal static class JobTypePayloadRegistry
    {
        /// <summary>
        /// Generates <see cref="PayloadJsonSchema"/> for request Payload for the given <see cref="JobType"/>
        /// </summary>
        /// <param name="jobType">Job type for which request schema to be generated.</param>
        public static PayloadJsonSchema GetPayloadSchema(JobType jobType)
        {
            var payloadType = GetPayloadSchemaType(jobType);
            return GenerateSchema(payloadType);
        }

        /// <summary>
        /// Generates <see cref="PayloadJsonSchema"/> for response Payload for the given <see cref="JobType"/>
        /// </summary>
        /// <param name="jobType">Job type for which result schema to be generated.</param>
        public static PayloadJsonSchema GetResultPayloadSchema(JobType jobType)
        {
            var payloadType = GetResultPayloadSchemaType(jobType);
            return GenerateSchema(payloadType);
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

        /// <summary>
        /// Generates json schema for a given payload type. Expected consumer: React rjfs component.
        /// </summary>
        /// <param name="payloadType">Payload type.</param>
        /// <returns>Json schema.</returns>
        private static PayloadJsonSchema GenerateSchema(Type payloadType)
        {
            var enumDefs = new Dictionary<string, IDictionary<string, IEnumerable<string>>>();
            var requiredProperties = new List<string>();
            var schema = new PayloadJsonSchema
            {
                Title = payloadType.Name,
                Description = payloadType.FullName,
                Type = "object"
            };

            var props = new Dictionary<string, PayloadJsonSchema>();
            foreach (var pi in payloadType.GetProperties())
            {
                // Add to schema Required
                if (!pi.PropertyType.IsNullableType() && pi.PropertyType.IsPrimitiveType())
                {
                    requiredProperties.Add(pi.Name);
                }

                // Complex type recurses, simple go as a property.
                if (pi.PropertyType.IsPrimitiveType())
                {
                    var propGenerator = GetSimplePropGenerator(pi, enumDefs);
                    props[pi.Name] = propGenerator();
                }
                else
                {
                    props[pi.Name] = GenerateSchema(pi.PropertyType.ToUnderlying());
                    props[pi.Name].Title = pi.GetFriendlyName();
                    props[pi.Name].Description = null;
                }
            }

            schema.Required = requiredProperties;
            schema.Properties = props;
            if (enumDefs.Any())
            {
                schema.Definitions = enumDefs;
            }

            return schema;
        }

        private static Func<PayloadJsonSchema> GetSimplePropGenerator(PropertyInfo pi, Dictionary<string, IDictionary<string, IEnumerable<string>>> enumDefs)
        {
            return pi.PropertyType.IsEnum() ? () => GenerateEnumEntry(pi, enumDefs) : () => GenerateSimpleProp(pi);
        }

        private static PayloadJsonSchema GenerateEnumEntry(PropertyInfo pi, Dictionary<string, IDictionary<string, IEnumerable<string>>> enumDefs)
        {
            var enumNames = Enum.GetNames(pi.PropertyType.ToUnderlying());
            enumDefs.Add($"{pi.Name}s", new Dictionary<string, IEnumerable<string>> { { "enum", enumNames } });
            return new PayloadJsonSchema
            {
                Title = pi.GetFriendlyName(),
                Ref = $"#/definitions/{pi.Name}s",
                Default = pi.GetCustomAttribute<DefaultValueAttribute>()?.Value
            };
        }

        private static PayloadJsonSchema GenerateSimpleProp(PropertyInfo pi)
        {
            return new PayloadJsonSchema
            {
                Title = pi.GetFriendlyName(),
                Type = pi.PropertyType.ToJsonType(),
                Default = pi.GetCustomAttribute<DefaultValueAttribute>()?.Value
            };
        }

        /// <summary>
        /// Lists bindings between JobType and Request Payload.
        /// </summary>
        /// <param name="jobType">JobType for which request payload type is requested.</param>
        /// <exception cref="NotImplementedException"></exception>
        private static Type GetPayloadSchemaType(JobType jobType)
        {
            switch (jobType)
            {
                case JobType.Dummy:
                    return typeof(SimpleMessagePayload);
                case JobType.ConvertExcelToPdf:
                    return typeof(ConvertExcelToPdfPayload);
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

        /// <summary>
        /// Lists bindings between JobType and Result Payload.
        /// </summary>
        /// <param name="jobType">JobType for which result payload type is requested.</param>
        /// <exception cref="NotImplementedException"></exception>
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

        /// <summary>
        /// Reads Name prop of a Display attribute when found, or just a property name.
        /// </summary>
        /// <param name="pi">Property info.</param>
        private static string GetFriendlyName(this PropertyInfo pi)
        {
            var displayAttribute = pi.GetCustomAttribute<DisplayAttribute>();
            return displayAttribute != null ? displayAttribute.Name : pi.Name;
        }

        /// <summary>
        /// For our case, primitive is a string, .NET primitive, enum or any of these for nullable underlying type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
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

        private static bool IsEnum(this Type type)
        {
            if (type.IsEnum)
            {
                return true;
            }

            if (type.IsNullableType() && type.IsGenericType)
            {
                return type.GenericTypeArguments[0].IsEnum;
            }

            return false;
        }

        private static Type ToUnderlying(this Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlying)
            {
                return underlying;
            }

            return type;
        }

        /// <summary>
        /// Maps .NET type to one of the json types: string, number, integer, boolean, array, object.
        /// </summary>
        /// <param name="type">.NET type.</param>
        /// <returns>Json equivalent type.</returns>
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
