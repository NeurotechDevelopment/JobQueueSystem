import type { JSONSchema7 } from "json-schema";
import type { IJobTypeDescriptor as JobTypeDescriptor, IJob as Job } from '../api/JobContracts.ts';

// Forms file download link to JobRepository service with the given fileId.
export function getFileLink(fileId: string | null | undefined): string {
    return !!fileId ? `${import.meta.env.VITE_API_BASE_URL}/JobsAttachments/${fileId}` : '';
}

// Accesses Job Request payload schema. Stips away title and description for RJFS not to render those.
export function getRequestSchema(jobTypeDescriptor: JobTypeDescriptor): JSONSchema7 {
    const { title, description, ...rest } = jobTypeDescriptor.payloadJsonSchema;
    return rest;
}

// Accesses JobResult payload schema. Stips away title and description for RJFS not to render those.
export function getResultSchema(jobTypeDescriptor: JobTypeDescriptor) : JSONSchema7 {
    const { title, description, ...rest } = jobTypeDescriptor.resultJsonSchema;
    return rest;
}

// Checks whether job has resultPayload set.
export function hasResult(job: Job): boolean {
    if (!job) {
        return false;
    }

    return job.resultPayload != null;
}

// Checks whether job has requestPayload set and its data is set.
export function hasRequestPayload(job: Job) : boolean {
    if (!job) {
        return false;
    }

    return job.requestPayload != null && job.requestPayload.data != null;
}

// Checks whether job has requestPayload attachment set (i.e. if file was uploaded with JobRequest).
export function hasRequestAttachment(job: Job) : boolean {
    if (!job) {
        return false;
    }
    
    return job.requestPayload != null && job.requestPayload.attachment != null;
}

// Checks whether job has resultPayload set and its data is set.
export function hasResultPayload(job: Job) : boolean {
    return hasResult(job) && job.resultPayload!.payload != null && job.resultPayload!.payload.data != null;
}

// Checks whether job result errored out and contains error message.
export function isErrorResult(job: Job) : boolean {
    return hasResult(job) && !job.resultPayload!.isSuccess && !!job.resultPayload!.errorMessage;
}

// Checks whether job has resultPayload attachment set (i.e. if file was formed as a job result).
export function hasResultAttachment(job: Job) : boolean {
    return hasResult(job) && job.resultPayload!.payload != null && !!job.resultPayload!.payload.attachment;
}

// Tries to parse value as json or returns empty {} object.
export function parseJson(value: string | null | undefined)  {
    try {
        return JSON.parse(value!);
    } catch {
        return {};
    }
}

// TypeScript Date to locale datetime
export function toDate(value: Date | null | undefined) {
    if (!value) {
        return '';
    }

    return new Date(value).toLocaleString();
}