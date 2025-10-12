import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom';
import Alert from 'react-bootstrap/Alert';
import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import Stack  from 'react-bootstrap/Stack';
import Accordion from 'react-bootstrap/Accordion';
import RjfsForm from "@rjsf/core";
import validator from "@rjsf/validator-ajv8"
import type { JSONSchema7 } from "json-schema";
import { payloadHasProperties } from '../api/JobContracts';
import type { IJobTypeDescriptor as JobTypeDescriptor, IJob as Job, IAttachment as Attachment } from '../api/JobContracts';
import axios from 'axios';

function JobDetailsView() {
    const { jobId } = useParams<{ jobId: uuid }>(); 
    const [job, setJob] = useState<Job | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [jobTypeDescriptors, setJobTypeDescriptors] = useState<JobTypeDescriptor>();
    useEffect(() => {
                axios.get<JobTypeDescriptor>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types`)
                .then(r => {
                    setJobTypeDescriptors(r.data);
                })
                .catch(err => {
                    console.error('Error fetching job type details', err);
                });
            fetchJob(jobId);
        }, [jobId]);

    // Reads job details from JobRepository service.
    function fetchJob(jobId: uuid) {
        axios.get<Job>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/${jobId}`)
            .then(r => {
                setJob(r.data);
                setError(null);
                
             })
            .catch(err => {
                    console.error('Error fetching job details', err);
                    setError('Error fetching job details: ' + err.message);
                });
    }

    // Forms file download link to JobRepository service with the given fileId.
    function getFileLink(fileId: string): string {
        return `${import.meta.env.VITE_API_BASE_URL}/JobsAttachments/${fileId}`
    }

    // Accesses Job Request payload schema. Stips away title and description for RJFS not to render those.
    function getRequestSchema(): JSONSchema7 {
        if (!job) {
            return;
        }

        const jobTypeDescriptor = jobTypeDescriptors.find(x => x.jobType == job.type);
        const { title, description, ...rest } = jobTypeDescriptor.payloadJsonSchema;
        return rest;
    }

    // Accesses JobResult payload schema. Stips away title and description for RJFS not to render those.
    function getResultSchema() : JSONSchema7 {
        if (!job) {
            return;
        }

        const jobTypeDescriptor = jobTypeDescriptors.find(x => x.jobType == job.type);
        const { title, description, ...rest } = jobTypeDescriptor.resultJsonSchema;
        return rest;
    }

    // Checks whether job has resultPayload set.
    function hasResult() : boolean {
        if (!job) {
            return false;
        }

        return job.resultPayload;
    }

    // Checks whether job has requestPayload set and its data is set.
    function hasRequestPayload() {
        if (!job) {
            return false;
        }

        return job.requestPayload && job.requestPayload.data;
    }

    // Checks whether job has requestPayload attachment set (i.e. if file was uploaded with JobRequest).
    function hasRequestAttachment() : boolean {
        if (!job) {
            return false;
        }
        
        return job.requestPayload && job.requestPayload.attachment;
    }

    // Checks whether job has resultPayload set and its data is set.
    function hasResultPayload() : boolean {
        return hasResult() && job.resultPayload.payload && job.resultPayload.payload.data;
    }

    // Checks whether job result errored out and contains error message.
    function isErrorResult() : boolean {
        return hasResult() && !job.resultPayload.isSuccess && !!job.resultPayload.errorMessage;
    }

    // Checks whether job has resultPayload attachment set (i.e. if file was formed as a job result).
    function hasResultAttachment() : boolean {
        return hasResult() && job.resultPayload && job.resultPayload.payload && job.resultPayload.payload.attachment;
    }

    // Tries to parse value as json or returns empty {} object.
    function parseJson(value: string)  {
        try {
            return JSON.parse(value);
        } catch {
            return {};
        }
    }

    function renderJobDetails() : JSX.Element {
        return (
            <div>
                <p><strong>ID:</strong> {job.jobId}</p>
                <p><strong>Type:</strong> {job.type}</p>
                <p><strong>Description:</strong> {job.description}</p>
                <p><strong>Status:</strong> {job.status}</p>
                {isErrorResult() && 
                 <Card border='danger'>
                    <Card.Title>Error</Card.Title>  
                    <Card.Body>{job.resultPayload.errorMessage}</Card.Body>
                 </Card>
                }
            </div>
        );
    }

    function renderJobRequestPayload() : JSX.Element {
        return (
            <Accordion>
            {(hasRequestPayload() || hasRequestAttachment()) && 
            <Accordion.Item eventKey="0">
                <Accordion.Header>Request parameters</Accordion.Header>
                <Accordion.Body>
                {hasRequestPayload() && payloadHasProperties(getRequestSchema()) &&
                 <RjfsForm
                   schema={getRequestSchema()}
                   formData={parseJson(job.requestPayload.data)}
                   disabled={true}
                   showErrorList={false}
                   liveValidate={true}
                   validator={validator}
                 >
                   <></>
                 </RjfsForm>}
                {hasRequestAttachment() && 
                    <div>Uploaded attachment:  
                        <a href={getFileLink(job.requestPayload.attachment.id)}>{job.requestPayload.attachment.fileName}</a>
                    </div>
                }
                </Accordion.Body>
            </Accordion.Item>}
            </Accordion>
        );
    }

    function renderJobResultPayload() : JSX.Element {
        return (
            <div>
                <h3>Result</h3>
                <RjfsForm
                   schema={getResultSchema()}
                   formData={parseJson(job.resultPayload.payload.data)}
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

    function renderJobResultAttachment() : JSX.Element {
        return (
                <div>File result:  
                 <a href={getFileLink(job.resultPayload.payload.attachment.id)}>
                     {job.resultPayload.payload.attachment.fileName}
                 </a>
                </div>
        );
    }

    return (
        <div>
          {error && <Alert variant="danger">Error loading task with id {jobId}. {error}</Alert>}
          <h2>Task Details</h2>
          <i            className="bi bi-arrow-clockwise"
                        title="Refresh"
                        style={{ fontSize: '1.5rem', cursor: 'pointer' }}
                        onClick={() => fetchJob(jobId)}
           ></i>
           <Stack gap={3}>
            {job && 
                <div className="p-2">{renderJobDetails()}</div>}
            {job && 
                <div className="p-2">{renderJobRequestPayload()}</div>}
            {hasResultPayload() && payloadHasProperties(getResultSchema()) && 
                <div className="p-2">{renderJobResultPayload()}</div>}
            {hasResultAttachment() && 
                <div className="p-2">{renderJobResultAttachment()}</div>}
           </Stack>
        </div>
    );
}

export default JobDetailsView;