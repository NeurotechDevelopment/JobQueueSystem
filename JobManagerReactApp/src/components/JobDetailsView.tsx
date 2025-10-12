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
import * as jobUtils from '../utils/jobdetails.ts'
import bugImage from "../assets/bug.svg"

function JobDetailsView() {
    const { jobId } = useParams<{ jobId: uuid }>(); 
    const [job, setJob] = useState<Job | null>(null);
    const [error, setError] = useState<string | null>(null);
    const [jobTypeDescriptor, setJobTypeDescriptor] = useState<JobTypeDescriptor>();
    useEffect(() => {
            (async () => fetchData())();
        }, [jobId]);

    async function fetchData() {
        const jobTypeDescriptors = await fetchJobTypeDescriptors();
        if (!jobTypeDescriptors) {
            return;
        }
        const job = await fetchJob(jobId);
        if (!job) {
            return;
        }
        setJob(job);
        const jobTypeDescriptor = jobTypeDescriptors.find(x => x.jobType == job.type);
        setJobTypeDescriptor(jobTypeDescriptor);
    }

    // Fetches job type descriptors from job repository service
    async function fetchJobTypeDescriptors() {
        try {
            const response = await axios.get<JobTypeDescriptor[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types`);
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
    async function fetchJob(jobId: uuid) : Promise<Job | null> {
        try {
            const response = await axios.get<Job>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/${jobId}`);
            setError(null);
            return response.data;
        } catch(err) {
            console.error('Error fetching job details', err);
            setError('Error fetching job details: ' + err.message);
            return null;
        }
    }

    function renderGenericErrorCard() : JSX.Element {
        return (
            <Card border='danger'>                
                <Card.Title><i class="bi bi-bug text-danger">&nbsp;</i>General Error</Card.Title>
                <Card.Body>We could not retrieve task details due to internal error. Please try again later.</Card.Body>
            </Card>
        );
    }

    function renderJobDetails() : JSX.Element {
        return (
            <div>
                <p><strong>ID:</strong> {job.jobId}</p>
                <p><strong>Type:</strong> {job.type}</p>
                <p><strong>Description:</strong> {job.description}</p>
                <p><strong>Status:</strong> {job.status}</p>
                {jobUtils.isErrorResult(job) && 
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
            {(jobUtils.hasRequestPayload(job) || jobUtils.hasRequestAttachment(job)) && 
            <Accordion.Item eventKey="0">
                <Accordion.Header>Request parameters</Accordion.Header>
                <Accordion.Body>
                {jobUtils.hasRequestPayload(job) && payloadHasProperties(jobUtils.getRequestSchema(jobTypeDescriptor)) &&
                 <RjfsForm
                   schema={jobUtils.getRequestSchema(jobTypeDescriptor)}
                   formData={jobUtils.parseJson(job.requestPayload.data)}
                   disabled={true}
                   showErrorList={false}
                   liveValidate={true}
                   validator={validator}
                 >
                   <></>
                 </RjfsForm>}
                {jobUtils.hasRequestAttachment(job) && 
                    <div>Uploaded attachment:  
                        <a target='_blank' href={jobUtils.getFileLink(job.requestPayload.attachment.id)}>{job.requestPayload.attachment.fileName}</a>
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
                   schema={jobUtils.getResultSchema(jobTypeDescriptor)}
                   formData={jobUtils.parseJson(job.resultPayload.payload.data)}
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
                 <a target='_blank' href={jobUtils.getFileLink(job.resultPayload.payload.attachment.id)}>
                     {job.resultPayload.payload.attachment.fileName}
                 </a>
                </div>
        );
    }

    return (
        <div>
          {error && <Alert variant="danger">Error loading task with id {jobId}. {error}</Alert>}
          <h2>Task Details</h2>
          <i className="bi bi-arrow-clockwise"
             title="Refresh"
             style={{ fontSize: '1.5rem', cursor: 'pointer' }}
             onClick={() => fetchData(jobId)}
           ></i>
           {error && renderGenericErrorCard()}
           <Stack gap={3}>
            {job && 
                <div className="p-2">{renderJobDetails()}</div>}
            {job && 
                <div className="p-2">{renderJobRequestPayload()}</div>}
            {jobUtils.hasResultPayload(job) && payloadHasProperties(jobUtils.getResultSchema(jobTypeDescriptor)) && 
                <div className="p-2">{renderJobResultPayload()}</div>}
            {jobUtils.hasResultAttachment(job) && 
                <div className="p-2">{renderJobResultAttachment()}</div>}
           </Stack>
        </div>
    );
}

export default JobDetailsView;