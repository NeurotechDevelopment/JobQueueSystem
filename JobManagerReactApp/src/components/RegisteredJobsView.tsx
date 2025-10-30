import { useState, useEffect } from 'react'
import Table from 'react-bootstrap/Table'
import Alert from 'react-bootstrap/Alert'
import { Link } from 'react-router-dom'
import type { IJobInfo as JobInfo } from '../api/JobContracts'
import Button from 'react-bootstrap/Button'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import RemoveJobModal from './RemoveJobModal'
import useApiClient from '../api/api-client'
import './RegisteredJobsView.css'

type AlertType = 'success' | 'danger';
type AlertState = {
    show: boolean;
    type: AlertType;
    message: string;
};
function RegisteredJobsView() {
    const { getJobList } = useApiClient();
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [selectedJob, setSelectedJob] = useState<JobInfo | undefined>();
    const [topNotification, setTopNotification] = useState<AlertState>({ show: false, type: 'success', message: '' });

    useEffect(() => {
        (async () => fetchJobs())();
        }, []);

    async function fetchJobs() {
        try {
            const response = await getJobList();
            setJobs(response.data);
        } catch (err) {
            setTopNotification({ show: true, type: 'danger', message: 'Error fetching jobs.' + err.message });
        }
    }

    // Show modal for deleting a job with id jobId.
    function confirmJobRemoval(jobId: string) {
        const job = jobs.find(j => j.jobId === jobId);
        if (!job) {
            return;
        }
        setSelectedJob(job);
        setShowDeleteModal(true);        
    }

    // Invoked by RemoveJobModal upon deletion and close.
    async function handleDeleteJob(jobId: string) {
        setShowDeleteModal(false);
        setTopNotification({ show: true, type: 'success', message: `Successfully deleted task with id ${jobId}` });

        try {
            await fetchJobs();
        } catch (err) {
            setTopNotification({ show: true, type: 'danger', message: 'Error deleting job.' + err.message });
        }
    }

    return (
      <div>
        {topNotification.show && <Alert dismissible variant={topNotification.type}>{topNotification.message}</Alert>}
        <h3>Registered tasks</h3>
        <i
            className="bi bi-arrow-clockwise"
            title="Refresh"
            style={{ fontSize: '1.5rem', cursor: 'pointer' }}
            onClick={fetchJobs}
        ></i>
            {selectedJob && <RemoveJobModal show={showDeleteModal} jobId={selectedJob.jobId!} onDelete={() => handleDeleteJob(selectedJob.jobId!)} onCancel={() => setShowDeleteModal(false)} />}
        <Table bordered striped hover responsive="xs">
            <colgroup>
              <col style={{ width: "32%" }} />
              <col style={{ width: "10%" }} />
              <col style={{ width: "36%" }} />
              <col style={{ width: "7%" }} />
              <col style={{ width: "7%" }} />
              <col style={{ width: "8%" }} />
            </colgroup>
            <thead>
                <tr>
                    <th>JobId</th>
                    <th>Type</th>
                    <th>Description</th>
                    <th>Status</th>
                    <th>Success</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
        {jobs.map((j,i) => (
            <tr key={i}>
                <td><Link to={`/jobs/${j.jobId}`}>{j.jobId}</Link></td>
                <td>{j.type}</td>
                <td>{j.description }</td>
                <td>{j.status}</td>
                <td>{j.isSuccess ? "true" : (j.isSuccess == null ? "N/A" : "false")}</td>
                <td>
                    <ButtonGroup aria-label="Basic example">
                        <Button variant="light"><Link to={`/jobs/${j.jobId}`}><i className="bi bi-eye text-success"></i></Link></Button>
                        <Button variant="light" onClick={() => confirmJobRemoval(j.jobId!)}><a><i className="bi bi-trash text-danger"></i></a></Button>
                    </ButtonGroup>
                </td>
            </tr>
        ))}
            </tbody>
        </Table>
      </div>);
}

export default RegisteredJobsView;
