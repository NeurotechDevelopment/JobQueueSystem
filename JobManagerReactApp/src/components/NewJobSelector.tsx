import { useState, useEffect } from 'react'
import type { JSX } from 'react'
import Dropdown from 'react-bootstrap/Dropdown';
import RjfsForm from "@rjsf/core";
import Form from 'react-bootstrap/Form';
import validator from "@rjsf/validator-ajv8"
import Card from 'react-bootstrap/Card';
import Button from 'react-bootstrap/Button';
import Alert from 'react-bootstrap/Alert';
import type {
    JobType,
    IJobTypeDescriptor as JobTypeDescriptor,
    IJobRequest as JobRequest
} from '../api/JobContracts';
import { payloadHasProperties } from '../api/JobContracts';
import useApiClient from '../api/api-client';
import type { JSONSchema7 } from "json-schema";
import JobCreatedConfirmation from './JobCreatedConfirmation';
import type { AlertState } from '../utils/notification'
import { getNotificationIcon } from '../utils/notification';
import { parseErrorMessage } from '../utils/error';

// Function component.
function NewJobSelector(): JSX.Element {
    const { getJobTypeDescriptors, postJobRequest } = useApiClient();

    // Initially fetch all job type descriptors
    const [jobTypeDescriptors, setJobTypeDescriptors] = useState<JobTypeDescriptor[]>([]);

    // Currently selected job type descriptor from dropdown
    const [jobTypeDescriptor, setJobTypeDescriptor] = useState<JobTypeDescriptor>();

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

    // Used to determine whether to show create page or confirmation page.
    const [jobRequestCreated, setJobRequestCreated] = useState<JobRequest | null>(null);

    useEffect(() => {
        (async () => {
            try {
                const response = await getJobTypeDescriptors();
                console.log('Fetched job type descriptor ok with axios.' + response.data);
                setJobTypeDescriptors(response.data);
            } catch (err) {
                console.error('Error fetching job type descriptor', err);
                setAlertState({ show: true, type: 'danger', message: `Error fetching job type descriptor. ${parseErrorMessage(err)}` });
            }
        })();
    }, []);

    // Remove title and description properties from schema that rjfs will render automatically which look ugly. We just want clean form.
    // TODO: strip these once when fetching all jobs type descriptors.
    function stripRedundantSchemaProps(schema: JSONSchema7) {
        const { title, description, ...rest } = schema;
        return rest;
    }

    // When user selects a job type from dropdown, set it as current job type descriptor
    function handleSelect(eventKey: string | null) {
        const jobType: JobType = eventKey as unknown as JobType;
        const currentJobType = jobTypeDescriptors.find(j => j.jobType === jobType)!;
        currentJobType.payloadJsonSchema = stripRedundantSchemaProps(currentJobType.payloadJsonSchema);
        setJobTypeDescriptor(currentJobType);
        setDescription('');
        setFile(null);
    }

    function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
        setFile(e.target.files?.[0] ?? null);
    }

    function resetPage() {

        setJobTypeDescriptor(undefined);
        setJobRequestCreated(null);
        setAlertState({ show: false, type: 'success', message: '' }); // hide any previous alerts        
    }

    async function handleSubmit(e: React.FormEvent) {
        e.preventDefault();
        if (!jobTypeDescriptor) return;

        const jobRequest: JobRequest = {
            jobId: crypto.randomUUID(),
            type: jobTypeDescriptor.jobType,
            description: description,
            payload: {
                data: payload ? JSON.stringify(payload) : undefined
            }
        }

        try {
            setIsSubmitting(true);
            const result = await postJobRequest(jobRequest, file);
            console.log('Task created successfully.', result.data);
            setAlertState({ show: true, type: 'success', message: 'Task created successfully.' });
            setJobRequestCreated(jobRequest);

        } catch (err) {
            setAlertState({ show: true, type: 'danger', message: `Error creating job: ${parseErrorMessage(err)}` });
        } finally {
            setIsSubmitting(false);
        }
    }

    if (jobRequestCreated == null) {
        return (
            <div>
                <Alert show={alertState.show} variant={alertState.type}><i className={getNotificationIcon(alertState.type)}></i>{alertState.message}</Alert>
                <h3>Create new task</h3>
                <Dropdown className="mt-5" onSelect={handleSelect}>
                    <Dropdown.Toggle variant="primary" id="dropdown-basic">
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
                                        key={jobTypeDescriptor.jobType}
                                        type="file"
                                        accept={jobTypeDescriptor.allowedAttachments.map(x => `.${x}`).join(',')}
                                        onChange={handleFileChange}
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
                                variant="primary"
                                type="submit"
                                disabled={isSubmitting}
                            >Create</Button>
                        </Form>

                    }
                </div>
            </div>
        );
    } else {
        return (
            <JobCreatedConfirmation JobRequest={jobRequestCreated} onCreateNewJob={resetPage} />
        );
    }
}

export default NewJobSelector;