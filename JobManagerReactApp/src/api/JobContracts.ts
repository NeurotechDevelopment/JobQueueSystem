export enum JobType {
    Dummy,
    ConvertWordToPdf,
    ConvertHtmlToPdf,
    ConvertExcelToPdf,
    ConvertScanToSearchablePdf
}

export enum JobStatus {
    NotStarted,
    Enqueued,
    InProgress,
    Failed,
    Finished
}

export interface IJobInfo {
    /** Format: uuid */
    jobId?: string;
    type?: JobType;
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
    jobId?: string;
    type?: JobType;
    status?: JobStatus;
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
    payloadJsonSchema: string;
    resultJsonSchema: string;
}