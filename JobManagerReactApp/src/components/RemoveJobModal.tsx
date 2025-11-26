import type { JSX } from 'react'
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';
import useApiClient from '../api/api-client';

export interface RemoveJobModalProps {
    show: boolean,
    jobId: string,
    onCancel: React.MouseEventHandler<HTMLButtonElement | undefined | null> 
    onDelete: React.MouseEventHandler<HTMLButtonElement | undefined | null>
    onDeleteError: (errorMessage: string) => void
}

export default function RemoveJobModal(props: RemoveJobModalProps) : JSX.Element {
    const { removeJob } = useApiClient();

    async function confirmRemoveJob(e: React.MouseEvent<HTMLButtonElement, MouseEvent>) {
        try {
            await removeJob(props.jobId);
            props.onDelete(e);
        } catch(error) { 
            props.onDeleteError(error.message);
        }
    }

    return (
        <Modal show={props.show} onHide={() => props.onCancel}>
            <Modal.Header closeButton>
                <Modal.Title>Confirm Deletion.</Modal.Title>
            </Modal.Header>
            <Modal.Body>
                You are about to delete task with id <strong>{props.jobId}</strong>.<br />
                What would you like to do?
            </Modal.Body>
            <Modal.Footer>
                <Button variant="secondary" onClick={props.onCancel}>
                    Cancel
                </Button>
                <Button variant="danger" onClick={(e) => confirmRemoveJob(e)}>
                    Delete
                </Button>
            </Modal.Footer>
        </Modal>
    )
}