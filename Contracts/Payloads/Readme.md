This folder contains payload subcontracts of a JobRequest Data.
Two-fold usage: 
1. Json schema generation for GUI clients where they can fill out the forms.
2. Strongly typed data contracts for internal JobHandlers that need retrieveing Job parameters.
There should be no more than one payload per JobType.
However, base Payload class may be used for multiple job types if they share the same parameters.

We will keep request/response payloads as a string. Although json polymorhpism in System.Text.Json.Serialization looks promising, 
it is somewhat raw, needs discriminator property; since payload may travel over different services, we want to keep it simple and explicit.
We may revisit this decision in the future.
Quick example how polymorphism may be implemented:
```csharp
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$jobParameterType")]
    [JsonDerivedType(typeof(ConvertScanToSearchablePdfPayload), nameof(JobType.ConvertScanToSearchablePdf))]
    [JsonDerivedType(typeof(JobFileParameters), nameof(JobType.ConvertWordToPdf))]
    [JsonDerivedType(typeof(JobFileParameters), nameof(JobType.ConvertExcelToPdf))]
    [JsonDerivedType(typeof(JobFileParameters), nameof(JobType.ConvertHtmlToPdf))]
    public abstract record JobParameters
    {
    }

    public record JobFileParameters : JobParameters
    {
        public string FileContent { get; set; }
    }
```


