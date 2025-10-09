import { useState, useEffect } from 'react'
import Dropdown from 'react-bootstrap/Dropdown';
import RjfsForm from "@rjsf/core";
import Form from 'react-bootstrap/Form';
import validator from "@rjsf/validator-ajv8"
import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import Alert from 'react-bootstrap/Alert';
import * as JobContracts from '../api/JobContracts.ts';
import axios from 'axios';
import './NewJobSelector.css'
import type { JSONSchema7 } from "json-schema";

// Type aliases for better readability and definitions for state variables
type JobType = JobContracts.JobType
type IJobTypeDescriptor = JobContracts.IJobTypeDescriptor
type JobRequest = JobContracts.IJobRequest
type AlertType = 'success' | 'danger';
type AlertState = {
    show: boolean;
    type: AlertType;
    message: string;
};

// Function component.
function NewJobSelector() {
    // Initially fetch all job type descriptors
    const [jobTypeDescriptors, setJobTypeDescriptors] = useState<IJobTypeDescriptor[]>([]);

    // Currently selected job type descriptor from dropdown
    const [jobTypeDescriptor, setJobTypeDescriptor] = useState<IJobTypeDescriptor>();

    // Currently filled payload for the selected job type in rjsf form
    const [payload, setPayload] = useState<any>();

    // Task description
    const [description, setDescription] = useState<string>();

    // Whether the form is being submitted. Used to disable the submit button to prevent multiple submissions.
    const [isSubmitting, setIsSubmitting] = useState<boolean>();

    // File attachment
    const [file, setFile] = useState<File | null>(null);

    // To display notification bar at the top: info or error
    const [alertState, setAlertState] = useState<AlertState>({ show: false, type: 'success', message: '' });

    useEffect(() => {
        axios.get<IJobTypeDescriptor[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types`)
            .then(r => {
                console.log('Fetched job type descriptor ok with axios.' + r.data);
                console.log(r.data);
                setJobTypeDescriptors(r.data);
            })
            .catch(err => {
                console.error('Error fetching job type descriptor', err);
            });
    }, []);

    // Remove title and description properties from schema that rjfs will render automatically which look ugly. We just want clean form.
    // TODO: strip these once when fetching all jobs type descriptors.
    function stripRedundantSchemaProps(schema: JSONSchema7) {
        const { title, description, ...rest } = schema;
        return rest;
    }

    // Check if the payload schema has any properties. If not, don't render rjfs form.
    function payloadHasProperties(schema: JSONSchema7): boolean {
        if (schema.type === 'object' && schema.properties) {
            return Object.keys(schema.properties).length > 0;
        }
        return false;
    }

    // When user selects a job type from dropdown, set it as current job type descriptor
    function handleSelect(jobType: JobType) {
        let currentJobType = jobTypeDescriptors.find(j => j.jobType === jobType)!;
        currentJobType.payloadJsonSchema = stripRedundantSchemaProps(currentJobType.payloadJsonSchema);
        setJobTypeDescriptor(currentJobType);
        setDescription('');
        setFile(null);
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (!jobTypeDescriptor) return;

        setIsSubmitting(true);

        const jobRequest: JobRequest = {
            jobId: crypto.randomUUID(),
            type: jobTypeDescriptor.jobType,
            description: description,
            payload: {
                data: payload ? JSON.stringify(payload) : undefined
            }
        }

        if (file) {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('jobRequest', new Blob([JSON.stringify(jobRequest)], { type: 'application/json' }));
            axios.post(`${import.meta.env.VITE_JOB_PRODUCER_API}/create-job-file`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
                .then(r => {
                    console.log('Job created with file attachment successfully.', r.data);
                    setAlertState({ show: true, type: 'success', message: 'Job created with file attachment successfully.' });
                })
                .catch(err => {
                    console.error('Error creating job with file attachment', err);
                    setAlertState({ show: true, type: 'danger', message: 'Error creating job with file attachment: ' + err.message });
                })
                .finally(() => {
                    setIsSubmitting(false);
                });
        } else {
            axios.post(`${import.meta.env.VITE_JOB_PRODUCER_API}/create-job`, jobRequest)
                .then(r => {
                    console.log('Job created successfully.', r.data);
                    setAlertState({ show: true, type: 'success', message: 'Job created successfully.' });
                })
                .catch(err => {
                    console.error('Error creating job', err);
                    setAlertState({ show: true, type: 'danger', message: 'Error creating job: ' + err.message });
                })
                .finally(() => {
                    setIsSubmitting(false);
                });
        }

        setIsSubmitting(false);
    }

    return (
        <div>
            <Alert show={alertState.show} variant={alertState.type} dismissible>{alertState.message}</Alert>
            <h2>Create new task</h2>
            <Dropdown className="mt-5" onSelect={handleSelect}>
                <Dropdown.Toggle variant="success" id="dropdown-basic">
                    Select task type
                </Dropdown.Toggle>

                <Dropdown.Menu>
                    {jobTypeDescriptors.map(jd => (
                        <Dropdown.Item key={jd.jobType} eventKey={jd.jobType}>{jd.jobType}</Dropdown.Item>
                    ))}
                </Dropdown.Menu>
            </Dropdown>
                <div>
                    <Card className="mt-4 shadow-sm">
                    <Card.Title>Selected task type</Card.Title>
                    {jobTypeDescriptor && <Card.Body>
                        <Card.Title>{jobTypeDescriptor.jobType}</Card.Title>
                        <Card.Text className="text-muted">{jobTypeDescriptor.description}</Card.Text>
                    </Card.Body>}
                    {!jobTypeDescriptor && <Card.Body>
                        <Card.Title>No task type selected.</Card.Title>
                        <Card.Text className="text-muted">Select task type in dropdown above to start creating new task.</Card.Text>
                    </Card.Body>}
                    </Card>
                {jobTypeDescriptor &&
                    <Form className="mt-5" onSubmit={handleSubmit}>
                        <Form.Group className="mb-3" controlId="description">
                            <Form.Label>Description</Form.Label>
                            <Form.Control as="textarea" rows={2} onChange={e => setDescription(e.target.value)} value={description} />
                        </Form.Group>
                        {(jobTypeDescriptor.allowedAttachments.length > 0) &&
                            <Form.Group controlId="formFile" className="mb-3">
                                <Form.Label>Attachment</Form.Label>
                                <Form.Control
                                    type="file"
                                    accept={jobTypeDescriptor.allowedAttachments.map(x => `.${x}`)}
                                    onChange={e => setFile(e.target.files ? e.target.files[0] : null)}
                                />
                            </Form.Group>
                        }
                        {payloadHasProperties(jobTypeDescriptor.payloadJsonSchema)
                            &&
                            (<div>
                                <h4>Parameters:</h4>

                                <RjfsForm
                                    validator={validator}
                                    schema={jobTypeDescriptor.payloadJsonSchema}
                                    tagName="div" // render as div for prevent default form behaviour
                                    onChange={(e) => setPayload(e.formData)} //
                                >
                                    {/* Empty fragment will prevent RjfsForm from displaying the Submit button */}
                                    <></>
                                </RjfsForm>
                            </div>)}
                        <Button
                            className="mb-5"
                            variant="success"
                            type="submit"
                            disabled={isSubmitting}
                        >Create</Button>
                    </Form>
                    
                }
                </div>            
        </div>
        );
}

export default NewJobSelector;