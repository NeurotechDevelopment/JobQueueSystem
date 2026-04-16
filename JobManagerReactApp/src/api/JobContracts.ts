import type { JSONSchema7 } from "json-schema";

export const JobType = {
    Dummy:"Dummy",
    ConvertWordToPdf:"ConvertWordToPdf",
    ConvertHtmlToPdf:"ConvertHtmlToPdf",
    ConvertExcelToPdf:"ConvertExcelToPdf",
    ConvertScanToSearchablePdf:"ConvertScanToSearchablePdf",
    ConvertWordToImages:"ConvertWordToImages"

} as const;

export type JobType = typeof JobType[keyof typeof JobType];

export const JobStatus = {
    NotStarted: "NotStarted",
    Enqueued: "Enqueued",
    InProgress: "InProgress",
    Failed: "Failed",
    Finished: "Finished"
} as const;

export type JobStatus = typeof JobStatus[keyof typeof JobStatus];

export interface IJobInfo {
    /** Format: uuid */
    jobId?: string;
    type?: JobType;
    description?: string | null;
    status?: JobStatus;
    /** Format: date-time */
    lastStatusChanged?: Date | null;
    /** Format: date-time */
    receivedAt?: Date | null;
    /** Format: date-time */
    finishedAt?: Date | null;
    isSuccess?: boolean | null;
    errorMessage?: string | null;
}

export interface IJob {
    /** Format: uuid */
    jobId: string;
    type: JobType;
    status: JobStatus;
    description?: string;
    /** Format: date-time */
    lastStatusChanged?: Date | null;
    /** Format: date-time */
    receivedAt?: Date | null;
    /** Format: date-time */
    finishedAt?: Date | null;
    isSuccess?: boolean | null;
    errorMessage?: string | null;
    requestPayload?: IJobPayload;
    resultPayload?: IJobResult;
}

export interface IAttachment {
    id?: string | null;
    fileName?: string | null;
    contentType?: string | null;
    /** Format: int64 */
    size?: number;
}

export interface IJobPayload {
    data?: string | null;
    attachment?: IAttachment;
}

export interface IJobRequest {
    /** Format: uuid */
    jobId?: string;
    description?: string | null;
    type?:    JobType;
    payload?: IJobPayload;
}

export interface IJobResult {
    payload?: IJobPayload;
    isSuccess?: boolean | null;
    errorMessage?: string | null;
}

export interface IJobTypeDescriptor {
    jobType: JobType;
    description: string;
    allowedAttachments: string[];
    payloadJsonSchema: JSONSchema7;
    resultJsonSchema: JSONSchema7;
}

// Check if the payload schema has any properties. If not, don't render rjfs form.
export function payloadHasProperties(schema: JSONSchema7): boolean {
    if (schema.type === 'object' && schema.properties) {
        return Object.keys(schema.properties).length > 0;
    }
    return false;
}