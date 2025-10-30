import type { JSX } from 'react'
import Card from 'react-bootstrap/Card';
import { Link } from 'react-router-dom'

export type JobDeletedConfirmationProp = {
    jobId: string
}

export default function JobDeletedConfirmation({ jobId }: JobDeletedConfirmationProp) : JSX.Element {
    return (
        <Card>
            <p>Task with id <strong>{jobId}</strong> was deleted successfully.</p>
            <p>Back to <Link to="/">Registered Tasks</Link></p>
        </Card>
    )
}