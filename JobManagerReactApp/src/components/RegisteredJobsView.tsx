import { useState, useEffect } from 'react'
import Alert from 'react-bootstrap/Alert'
import type { IJobInfo as JobInfo } from '../api/JobContracts'
import RemoveJobModal from './RemoveJobModal'
import useApiClient from '../api/api-client'
import SimpleTableJobsResult from './SimpleTableJobsResult'

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
            setTopNotification({ show: false, type: 'success', message: '' });
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
        <SimpleTableJobsResult jobsResult={jobs} onDeleteJob={confirmJobRemoval} />
      </div>);
}

export default RegisteredJobsView;
