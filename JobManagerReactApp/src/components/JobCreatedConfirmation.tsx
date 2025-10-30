import type { JSX } from 'react'
import { Link } from 'react-router-dom'
import Card from 'react-bootstrap/Card';
import type { JobType, IJobRequest } from '../api/JobContracts';

export default function JobCreatedConfirmation(props: IJobRequest): JSX.Element {

    return (
        <Card>
            <p>You have created a task with id <strong>{props.jobId}</strong> of type <strong>{props.type}</strong>.</p>
            <p>It has been enqueued and should be registered with the system shortly. Once registered, you should see it in a <Link to="/">tasks list</Link> page.</p>
        </Card>
    )
}