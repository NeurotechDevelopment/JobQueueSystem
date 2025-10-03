// @ts-nocheck
import { QGuidPath, QEntityPath, QBooleanPath, QStringPath, QEnumPath, QDateTimeOffsetPath, QueryObject, QId, QGuidParam, QNumberPath } from "@odata2ts/odata-query-objects";
import type { JobId } from "./ContractsModel";
import { JobType, JobStatus } from "./ContractsModel";

export class QJob extends QueryObject {
    public readonly JobId = new QGuidPath(this.withPrefix("JobId"));
    public readonly RequestPayload = new QEntityPath(this.withPrefix("RequestPayload"), () => QJobPayload);
    public readonly ResultPayload = new QEntityPath(this.withPrefix("ResultPayload"), () => QJobResult);
    public readonly IsSuccess = new QBooleanPath(this.withPrefix("IsSuccess"));
    public readonly ErrorMessage = new QStringPath(this.withPrefix("ErrorMessage"));
    public readonly Type = new QEnumPath(this.withPrefix("Type"), JobType);
    public readonly Status = new QEnumPath(this.withPrefix("Status"), JobStatus);
    public readonly LastStatusChanged = new QDateTimeOffsetPath(this.withPrefix("LastStatusChanged"));
    public readonly ReceivedAt = new QDateTimeOffsetPath(this.withPrefix("ReceivedAt"));
    public readonly FinishedAt = new QDateTimeOffsetPath(this.withPrefix("FinishedAt"));
}

export const qJob = new QJob();

export class QJobId extends QId<JobId> {
    private readonly params = [new QGuidParam("JobId")];

    getParams() {
        return this.params
    }
}

export class QJobResult extends QueryObject {
    public readonly Payload = new QEntityPath(this.withPrefix("Payload"), () => QJobPayload);
    public readonly IsSuccess = new QBooleanPath(this.withPrefix("IsSuccess"));
    public readonly ErrorMessage = new QStringPath(this.withPrefix("ErrorMessage"));
}

export const qJobResult = new QJobResult();

export class QJobPayload extends QueryObject {
    public readonly Data = new QStringPath(this.withPrefix("Data"));
    public readonly Attachment = new QEntityPath(this.withPrefix("Attachment"), () => QAttachment);
}

export const qJobPayload = new QJobPayload();

export class QAttachment extends QueryObject {
    public readonly Id = new QStringPath(this.withPrefix("Id"));
    public readonly FileName = new QStringPath(this.withPrefix("FileName"));
    public readonly ContentType = new QStringPath(this.withPrefix("ContentType"));
    public readonly Size = new QNumberPath(this.withPrefix("Size"));
}

export const qAttachment = new QAttachment();
