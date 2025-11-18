import type { JSX } from 'react'
import { Link } from 'react-router-dom';
import Card from 'react-bootstrap/Card';
import type { IJobRequest } from '../api/JobContracts';

export interface JobCreatedConfirmationProps {
    JobRequest: IJobRequest,
    onCreateNewJob: () => void
}

export default function JobCreatedConfirmation(props: JobCreatedConfirmationProps): JSX.Element {

    return (
        <Card>
            <p>You have created a task with id <strong>{props.JobRequest.jobId}</strong> of type <strong>{props.JobRequest.type}</strong>.</p>
            <p>It has been enqueued and should be registered with the system shortly. Once registered, you should see it in a <Link to="/">tasks list</Link> page.</p>
            <p>Click <a href='#' onClick={props.onCreateNewJob}>here</a > if you would like to create another task.</p>
        </Card>
    )
}