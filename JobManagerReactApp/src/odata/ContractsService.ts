// @ts-nocheck
import type { ODataHttpClient } from "@odata2ts/http-client-api";
import { ODataService, EntityTypeServiceV4, ODataServiceOptionsInternal, EntitySetServiceV4 } from "@odata2ts/odata-service";
import type { JobId, Job, EditableJob, JobResult, EditableJobResult, JobPayload, EditableJobPayload, Attachment, EditableAttachment } from "./ContractsModel";
import type { QJob, QJobResult, QJobPayload, QAttachment } from "./QContracts";
import { QJobId, qJob, qJobResult, qJobPayload, qAttachment } from "./QContracts";

export class ContractsService<in out ClientType extends ODataHttpClient> extends ODataService<ClientType> {
    public Jobs(): JobCollectionService<ClientType>;
    public Jobs(id: JobId): JobService<ClientType>;
    public Jobs(id?: JobId | undefined) {
        const fieldName = "Jobs";
        const { client, path, options, isUrlNotEncoded } = this.__base;
        return typeof id === "undefined" || id === null
        ? new JobCollectionService(client, path, fieldName, options)
        : new JobService(client, path, new QJobId(fieldName).buildUrl(id, isUrlNotEncoded()), options);
    }
}

export class JobService<in out ClientType extends ODataHttpClient> extends EntityTypeServiceV4<ClientType, Job, EditableJob, QJob> {
    private _RequestPayload?: JobPayloadService<ClientType>;
    private _ResultPayload?: JobResultService<ClientType>;

    constructor(client: ClientType, basePath: string, name: string, options?: ODataServiceOptionsInternal) {
        super(client, basePath, name, qJob, options);
    }

    public RequestPayload(): JobPayloadService<ClientType> {
        if(!this._RequestPayload) {
          const { client, path, options } = this.__base;
          this._RequestPayload = new JobPayloadService(client, path, "RequestPayload", options)
        }

        return this._RequestPayload
    }

    public ResultPayload(): JobResultService<ClientType> {
        if(!this._ResultPayload) {
          const { client, path, options } = this.__base;
          this._ResultPayload = new JobResultService(client, path, "ResultPayload", options)
        }

        return this._ResultPayload
    }
}

export class JobCollectionService<in out ClientType extends ODataHttpClient> extends EntitySetServiceV4<ClientType, Job, EditableJob, QJob, JobId> {
    constructor(client: ClientType, basePath: string, name: string, options?: ODataServiceOptionsInternal) {
        super(client, basePath, name, qJob, new QJobId(name), options);
    }
}

export class JobResultService<in out ClientType extends ODataHttpClient> extends EntityTypeServiceV4<ClientType, JobResult, EditableJobResult, QJobResult> {
    private _Payload?: JobPayloadService<ClientType>;

    constructor(client: ClientType, basePath: string, name: string, options?: ODataServiceOptionsInternal) {
        super(client, basePath, name, qJobResult, options);
    }

    public Payload(): JobPayloadService<ClientType> {
        if(!this._Payload) {
          const { client, path, options } = this.__base;
          this._Payload = new JobPayloadService(client, path, "Payload", options)
        }

        return this._Payload
    }
}

export class JobPayloadService<in out ClientType extends ODataHttpClient> extends EntityTypeServiceV4<ClientType, JobPayload, EditableJobPayload, QJobPayload> {
    private _Attachment?: AttachmentService<ClientType>;

    constructor(client: ClientType, basePath: string, name: string, options?: ODataServiceOptionsInternal) {
        super(client, basePath, name, qJobPayload, options);
    }

    public Attachment(): AttachmentService<ClientType> {
        if(!this._Attachment) {
          const { client, path, options } = this.__base;
          this._Attachment = new AttachmentService(client, path, "Attachment", options)
        }

        return this._Attachment
    }
}

export class AttachmentService<in out ClientType extends ODataHttpClient> extends EntityTypeServiceV4<ClientType, Attachment, EditableAttachment, QAttachment> {
    constructor(client: ClientType, basePath: string, name: string, options?: ODataServiceOptionsInternal) {
        super(client, basePath, name, qAttachment, options);
    }
}
