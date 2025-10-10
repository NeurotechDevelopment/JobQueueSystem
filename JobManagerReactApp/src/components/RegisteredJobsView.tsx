import { useState, useEffect } from 'react'
import Table from 'react-bootstrap/Table';
import axios from 'axios';
import type { IJobInfo as JobInfo } from '../api/JobContracts';
import './RegisteredJobsView.css'

function RegisteredJobsView() {
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    useEffect(() => {
            fetchJobs();
        },
        []);

    function fetchJobs() {
        axios.get<JobInfo[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository`)
            .then(r => {
                console.log('Fetched jobs ok with axios.');
                setJobs(r.data);
            })
            .catch(err => console.error('Error fetching jobs', err));
    }

    return (<div>
        <h1>Registered Jobs</h1>
                <i
                    className="bi bi-arrow-clockwise"
                    title="Refresh"
                    style={{ fontSize: '1.5rem', cursor: 'pointer' }}
                    onClick={fetchJobs}
                ></i>
        <Table bordered striped responsive hover>
            <thead>
                <tr>
                    <td>JobId</td>
                    <td>Type</td>
                    <td>Description</td>
                    <td>Status</td>
                    <td>IsSuccess</td>
                    <td>Error message</td>
                </tr>
            </thead>
            <tbody>
        {jobs.map((j,i) => (
            <tr key={i}>
                <td>{j.jobId}</td>
                <td>{j.type}</td>
                <td>{j.description}</td>
                <td>{j.status}</td>
                <td>{j.isSuccess ? "true" : (j.isSuccess == null ? "N/A" : "false")}</td>
                <td className="text-truncate">{j.errorMessage}</td>
            </tr>
        ))}
            </tbody>
        </Table>
            </div>);
}

export default RegisteredJobsView;
