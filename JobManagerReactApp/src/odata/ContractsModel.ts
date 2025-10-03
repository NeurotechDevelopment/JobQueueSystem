// @ts-nocheck

export enum JobType {
    Dummy = "Dummy",
    ConvertWordToPdf = "ConvertWordToPdf",
    ConvertHtmlToPdf = "ConvertHtmlToPdf",
    ConvertExcelToPdf = "ConvertExcelToPdf",
    ConvertScanToSearchablePdf = "ConvertScanToSearchablePdf"
}

export enum JobStatus {
    NotStarted = "NotStarted",
    Enqueued = "Enqueued",
    InProgress = "InProgress",
    Failed = "Failed",
    Finished = "Finished"
}

export interface Job {
    /**
     * **Key Property**: This is a key property used to identify the entity.<br/>**Managed**: This property is managed on the server side and cannot be edited.
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `JobId` |
     * | Type | `Edm.Guid` |
     * | Nullable | `false` |
     */
    JobId: string;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `RequestPayload` |
     * | Type | `Contracts.Payloads.JobPayload` |
     */
    RequestPayload: JobPayload | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `ResultPayload` |
     * | Type | `Contracts.JobResult` |
     */
    ResultPayload: JobResult | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `IsSuccess` |
     * | Type | `Edm.Boolean` |
     */
    IsSuccess: boolean | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `ErrorMessage` |
     * | Type | `Edm.String` |
     */
    ErrorMessage: string | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Type` |
     * | Type | `Contracts.JobType` |
     * | Nullable | `false` |
     */
    Type: JobType;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Status` |
     * | Type | `Contracts.JobStatus` |
     * | Nullable | `false` |
     */
    Status: JobStatus;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `LastStatusChanged` |
     * | Type | `Edm.DateTimeOffset` |
     */
    LastStatusChanged: string | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `ReceivedAt` |
     * | Type | `Edm.DateTimeOffset` |
     */
    ReceivedAt: string | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `FinishedAt` |
     * | Type | `Edm.DateTimeOffset` |
     */
    FinishedAt: string | null;
}

export type JobId = string | {JobId: string};

export interface EditableJob extends Pick<Job, "Type" | "Status">, Partial<Pick<Job, "IsSuccess" | "ErrorMessage" | "LastStatusChanged" | "ReceivedAt" | "FinishedAt">> {
    RequestPayload?: EditableJobPayload | null;
    ResultPayload?: EditableJobResult | null;
}

export interface JobResult {
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Payload` |
     * | Type | `Contracts.Payloads.JobPayload` |
     */
    Payload: JobPayload | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `IsSuccess` |
     * | Type | `Edm.Boolean` |
     */
    IsSuccess: boolean | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `ErrorMessage` |
     * | Type | `Edm.String` |
     */
    ErrorMessage: string | null;
}

export interface EditableJobResult extends Partial<Pick<JobResult, "IsSuccess" | "ErrorMessage">> {
    Payload?: EditableJobPayload | null;
}

export interface JobPayload {
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Data` |
     * | Type | `Edm.String` |
     */
    Data: string | null;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Attachment` |
     * | Type | `Contracts.Payloads.Attachment` |
     */
    Attachment: Attachment | null;
}

export interface EditableJobPayload extends Partial<Pick<JobPayload, "Data">> {
    Attachment?: EditableAttachment | null;
}

export interface Attachment {
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Id` |
     * | Type | `Edm.String` |
     * | Nullable | `false` |
     */
    Id: string;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `FileName` |
     * | Type | `Edm.String` |
     * | Nullable | `false` |
     */
    FileName: string;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `ContentType` |
     * | Type | `Edm.String` |
     * | Nullable | `false` |
     */
    ContentType: string;
    /**
     *
     * OData Attributes:
     * |Attribute Name | Attribute Value |
     * | --- | ---|
     * | Name | `Size` |
     * | Type | `Edm.Int64` |
     * | Nullable | `false` |
     */
    Size: number;
}

export interface EditableAttachment extends Pick<Attachment, "Id" | "FileName" | "ContentType" | "Size"> {
}
