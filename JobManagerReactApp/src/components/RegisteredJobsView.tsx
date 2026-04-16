import { useState, useEffect } from 'react'
import Alert from 'react-bootstrap/Alert'
import type { IJobInfo as JobInfo } from '../api/JobContracts'
import RemoveJobModal from './RemoveJobModal'
import useApiClient from '../api/api-client'
import SimpleTableJobsResult from './SimpleTableJobsResult'
import type { AlertState } from '../utils/notification'
import { getNotificationIcon } from '../utils/notification'
import { parseErrorMessage } from '../utils/error';

function RegisteredJobsView() {
    const { getJobList } = useApiClient();
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    const [showDeleteModal, setShowDeleteModal] = useState<boolean>(false);
    const [selectedJob, setSelectedJob] = useState<JobInfo | undefined>();
    const [notificationBar, setNotificationBar] = useState<AlertState>({ show: false, type: 'success', message: '' });

    useEffect(() => {
        (async () => fetchJobs())();
    }, []);

    async function fetchJobs() {
        try {
            const response = await getJobList();
            setJobs(response.data);
            setNotificationBar({ show: false, type: 'success', message: '' });
        } catch (err) {
            setNotificationBar({ show: true, type: 'danger', message: `Error fetching jobs. ${parseErrorMessage(err)}` });
        }
    }

    // Show modal for deleting a job with id jobId.
    function confirmJobRemoval(jobId: string) {
        const job = jobs.find(j => j.jobId === jobId);
        if (!job) {
            return;
        }
        setShowDeleteModal(true);
        setSelectedJob(job);
    }

    function handleDeleteError(errorMessage: string) {
        setShowDeleteModal(false);
        setNotificationBar({ show: true, type: 'danger', message: 'Error deleting job. ' + errorMessage });
    }

    // Invoked by RemoveJobModal upon deletion and close.
    async function handleDeleteJob(jobId: string) {
        setShowDeleteModal(false);
        setNotificationBar({ show: true, type: 'success', message: `Successfully deleted task with id ${jobId}` });

        try {
            await fetchJobs();
        } catch (err) {
            setNotificationBar({ show: true, type: 'danger', message: `Error deleting job. ${parseErrorMessage(err)}` });
        }
    }

    return (
        <div>
            {notificationBar.show && <Alert variant={notificationBar.type}><i className={getNotificationIcon(notificationBar.type)}></i>{notificationBar.message}</Alert>}
            <h3>Registered tasks</h3>
            <i
                className="bi bi-arrow-clockwise"
                title="Refresh"
                style={{ fontSize: '1.5rem', cursor: 'pointer' }}
                onClick={fetchJobs}
            ></i>
            {selectedJob && <RemoveJobModal show={showDeleteModal} jobId={selectedJob.jobId!} onDelete={() => handleDeleteJob(selectedJob.jobId!)} onCancel={() => setShowDeleteModal(false)} onDeleteError={handleDeleteError} />}
            <SimpleTableJobsResult jobsResult={jobs} onDeleteJob={confirmJobRemoval} />
        </div>);
}

export default RegisteredJobsView;
