import { useState, useEffect } from 'react'
import type { JSX } from 'react'
import { useParams } from 'react-router-dom';
import Alert from 'react-bootstrap/Alert';
import Card from 'react-bootstrap/Card';
import Stack  from 'react-bootstrap/Stack';
import Accordion from 'react-bootstrap/Accordion';
import Button from 'react-bootstrap/Button'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import RemoveJobModal from './RemoveJobModal';
import RjfsForm from "@rjsf/core";
import validator from "@rjsf/validator-ajv8"
import { payloadHasProperties } from '../api/JobContracts';
import type { IJobTypeDescriptor as JobTypeDescriptor, IJob as Job, JobType } from '../api/JobContracts';
import useApiClient from '../api/api-client';
import * as jobUtils from '../utils/jobdetails.ts'
import JobDeletedConfirmation from './JobDeletedConfirmation';

// Takes jobId as a route parameter and displays job details.
function JobDetailsView() {
    const { jobId } = useParams<{ jobId: string }>();
    const { getJob, getJobTypeDescriptor, genTempFileLink } = useApiClient();
    const [job, setJob] = useState<Job | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [jobTypeDescriptor, setJobTypeDescriptor] = useState<JobTypeDescriptor>();
    const [requestAttachmentDownloadUrl, setRequestAttachmentDownloadUrl] = useState<string | null>(null);
    const [resultAttachmentDownloadUrl, setResultAttachmentDownloadUrl] = useState<string | null>(null);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [deletedShow, setDeletedShow] = useState<boolean>(false)

    useEffect(() => {
            (async () => fetchData(jobId))();
        }, [jobId]);

    async function fetchData(jobId: string | undefined) {
        if (!jobId) return;

        const job = await fetchJob(jobId);
        if (!job) return;

        const jobTypeDescriptor = await fetchJobTypeDescriptor(job.type);
        if (!jobTypeDescriptor) return;

        setJob(job);
        setJobTypeDescriptor(jobTypeDescriptor);

        if (job && jobUtils.hasRequestAttachment(job)) {
            const url = await genTempFileLink(job.requestPayload!.attachment!.id!);
            setRequestAttachmentDownloadUrl(url.data);
        }

        if (job && jobUtils.hasResultAttachment(job)) {
            const url = await genTempFileLink(job.resultPayload!.payload!.attachment!.id!);
            setResultAttachmentDownloadUrl(url.data);
        }
    }

    // Fetches job type descriptors from job repository service
    async function fetchJobTypeDescriptor(jobType: JobType) {
        try {
            const response = await getJobTypeDescriptor(jobType);
            setError(null);
            return response.data;
        }
        catch(err) {
            console.error('Error fetching job type details', err);
            setError('Error fetching job type details:' + err.message);
            return null;
        }
    }

    // Reads job details from JobRepository service.
    async function fetchJob(jobId: string) : Promise<Job | null> {
        try {
            const response = await getJob(jobId);
            setError(null);
            return response.data;
        } catch(err) {
            console.error('Error fetching job details', err);
            setError('Error fetching job details: ' + err.message);
            return null;
        }
    }

    function onCloseRemoveConfirm() {
        setShowDeleteModal(false);
        setDeletedShow(true);
    }

    function renderGenericErrorCard() : JSX.Element {
        return (
            <Card border='danger'>                
                <Card.Title><i className="bi bi-bug text-danger">&nbsp;</i>General Error</Card.Title>
                <Card.Body>We could not retrieve task details due to internal error. Please try again later.</Card.Body>
            </Card>
        );
    }

    function renderJobDetails(job: Job) : JSX.Element {
        return (
            <div>
                <p><strong>ID:</strong> {job.jobId}</p>
                <p><strong>Type:</strong> {job.type}</p>
                <p><strong>Description:</strong> {job.description}</p>
                <p><strong>Status:</strong> {job.status}</p>
                {jobUtils.isErrorResult(job) && 
                 <Card border='danger'>
                    <Card.Title>Error</Card.Title>  
                    <Card.Body>{job.resultPayload!.errorMessage}</Card.Body>
                 </Card>
                }
            </div>
        );
    }

    function renderJobRequestPayload(job: Job, jobTypeDescriptor: JobTypeDescriptor) : JSX.Element {
        return (
            <Accordion>
            {(jobUtils.hasRequestPayload(job) || jobUtils.hasRequestAttachment(job)) && 
            <Accordion.Item eventKey="0">
                <Accordion.Header>Request parameters</Accordion.Header>
                <Accordion.Body>
                {jobUtils.hasRequestPayload(job) && payloadHasProperties(jobUtils.getRequestSchema(jobTypeDescriptor)) &&
                 <RjfsForm
                   schema={jobUtils.getRequestSchema(jobTypeDescriptor)}
                   formData={jobUtils.parseJson(job.requestPayload!.data)}
                   disabled={true}
                   showErrorList={false}
                   liveValidate={true}
                   validator={validator}
                 >
                   <></>
                 </RjfsForm>}
                 {jobUtils.hasRequestAttachment(job) && requestAttachmentDownloadUrl &&
                    <div>Uploaded attachment:  
                      <a target='_blank' href={requestAttachmentDownloadUrl}>{job.requestPayload!.attachment!.fileName}</a>
                    </div>
                }
                </Accordion.Body>
            </Accordion.Item>}
            </Accordion>
        );
    }

    function renderJobResultPayload(job: Job, jobTypeDescriptor: JobTypeDescriptor) : JSX.Element {
        return (
            <div>
                <h3>Result</h3>
                <RjfsForm
                   schema={jobUtils.getResultSchema(jobTypeDescriptor)}
                   formData={jobUtils.parseJson(job.resultPayload!.payload!.data)}
                   disabled={true}
                   showErrorList={false}
                   liveValidate={true}
                   validator={validator}
                >
                   <></>
                </RjfsForm>
            </div>
        );
    }

    function renderJobResultAttachment(job: Job): JSX.Element {
        if (!resultAttachmentDownloadUrl) return <div></div>;
        
        return (
                <div>File result:  
                 <a target='_blank' href={resultAttachmentDownloadUrl}>
                     {job.resultPayload!.payload!.attachment!.fileName}
                 </a>
                </div>
        );
    }

    function renderNoJob(): JSX.Element {
        return (<Card>
            <p>Could not find a task with id <strong>{jobId}</strong>. It could have not yet been registered with the system or has been removed.</p>
            <p>Please try again later.</p>
        </Card>);
    }
    function renderJobDetailsView(): JSX.Element {
        return (
            <div>
                {error && <Alert variant="danger">Error loading task with id {jobId}. {error}</Alert>}
                <h2>Task Details</h2>
                <ButtonGroup aria-label="Basic example">
                    <Button variant="light"> <i className="bi bi-arrow-clockwise"
                        title="Refresh"
                        style={{ fontSize: '1.5rem', cursor: 'pointer' }}
                        onClick={() => fetchData(jobId)}
                    ></i></Button>
                    {job && <Button variant="light" onClick={() => setShowDeleteModal(true)}><a><i className="bi bi-trash text-danger"></i></a></Button>}
                </ButtonGroup>
                <RemoveJobModal show={showDeleteModal} jobId={jobId!} onCancel={() => setShowDeleteModal(false)} onDelete={onCloseRemoveConfirm} />
                {error && renderGenericErrorCard()}
                {!job && renderNoJob() }
                <Stack gap={3}>
                    {job &&
                        <div className="p-2">{renderJobDetails(job)}</div>}
                    {job && jobTypeDescriptor &&
                        <div className="p-2">{renderJobRequestPayload(job, jobTypeDescriptor)}</div>}
                    {job && jobTypeDescriptor && jobUtils.hasResultPayload(job) && payloadHasProperties(jobUtils.getResultSchema(jobTypeDescriptor)) &&
                        <div className="p-2">{renderJobResultPayload(job, jobTypeDescriptor)}</div>}
                    {job && jobUtils.hasResultAttachment(job) &&
                        <div className="p-2">{renderJobResultAttachment(job)}</div>}
                </Stack>
            </div>
        );
    }

    if (deletedShow) {
        return <JobDeletedConfirmation jobId={jobId!} />
    } else {
        return (
            renderJobDetailsView()
        );
    }
}

export default JobDetailsView;