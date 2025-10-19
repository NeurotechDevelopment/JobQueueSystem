import { useState, useEffect } from 'react'
import Table from 'react-bootstrap/Table';
import Alert from 'react-bootstrap/Alert';
import { Link } from 'react-router-dom'
import type { IJobInfo as JobInfo } from '../api/JobContracts';
import useApiClient from '../api/api-client';
import './RegisteredJobsView.css'

function RegisteredJobsView() {
    const { getJobList } = useApiClient();
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    const [fetchErrorMessage, setFetchErrorMessage] = useState<string>();
    useEffect(() => {
        (async () => fetchJobs())();
        }, []);

    async function fetchJobs() {
        try {
            const response = await getJobList();
            console.log('Fetched jobs ok with apiClient.');
            setJobs(response.data);
            setFetchErrorMessage(undefined);
        } catch (err) {
            console.error('Error fetching jobs', err)
            setFetchErrorMessage('Error fetching jobs.' + err.message);
        }
    }

    return (
      <div>
        {fetchErrorMessage && <Alert variant='danger'>{fetchErrorMessage}</Alert>}
        <h3>Registered tasks</h3>
        <i
            className="bi bi-arrow-clockwise"
            title="Refresh"
            style={{ fontSize: '1.5rem', cursor: 'pointer' }}
            onClick={fetchJobs}
        ></i>
        <Table bordered striped hover responsive="xs">
            <colgroup>
              <col style={{ width: "28%" }} />
              <col style={{ width: "10%" }} />
              <col style={{ width: "48%" }} />
              <col style={{ width: "7%" }} />
              <col style={{ width: "7%" }} />
            </colgroup>
            <thead>
                <tr>
                    <th>JobId</th>
                    <th>Type</th>
                    <th>Description</th>
                    <th>Status</th>
                    <th>Success</th>
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
            </tr>
        ))}
            </tbody>
        </Table>
      </div>);
}

export default RegisteredJobsView;
