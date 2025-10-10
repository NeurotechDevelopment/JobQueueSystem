import { useState, useEffect } from 'react'
import { useParams } from 'react-router-dom';
import Alert from 'react-bootstrap/Alert';
import Card from 'react-bootstrap/Card';
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

    function getResultSchema() : JSONSchema7 {
        if (!job) {
            return;
        }

        const jobTypeDescriptor = jobTypeDescriptors.find(x => x.jobType == job.type);
        const { title, description, ...rest } = jobTypeDescriptor.resultJsonSchema;
        return rest;
    }

    function hasResult() {
        if (!job) {
            return false;
        }

        return job.resultPayload;
    }

    function hasResultPayload() {
        return hasResult() && job.resultPayload.payload && job.resultPayload.payload.data;
    }

    function isErrorResult() {
        return hasResult() && !job.resultPayload.isSuccess && !!job.resultPayload.errorMessage;
    }

    function hasAttachment() {
        return hasResult() && job.resultPayload && job.resultPayload.payload && job.resultPayload.payload.attachment;
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
      {job && <div>
      <p><strong>ID:</strong> {job.jobId}</p>
      <p><strong>Description:</strong> {job.description}</p>
      <p><strong>Status:</strong> {job.status}</p>
      {isErrorResult() && 
       <Card border='danger'>
          <Card.Title>Error</Card.Title>  
          <Card.Body>{job.resultPayload.errorMessage}</Card.Body>
       </Card>}
      {hasResultPayload() && payloadHasProperties(getResultSchema()) &&
       <div>
          <h3>Result</h3>
         <RjfsForm
             schema={getResultSchema()}
             formData={JSON.parse(job.resultPayload.payload.data)}
             disabled={true}
             showErrorList={false}
             liveValidate={true}
             validator={validator}
         >
             <></>
         </RjfsForm>
        </div>}
        {hasAttachment() &&
            <div>File result:  
                <a href={`${import.meta.env.VITE_API_BASE_URL}/JobsAttachments/${job.resultPayload.payload.attachment.id}`}>
                    {job.resultPayload.payload.attachment.fileName}
                </a>
            </div>}
       </div>}
    </div>
    );
}
export default JobDetailsView;