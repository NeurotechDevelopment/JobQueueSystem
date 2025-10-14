import { useState, useEffect } from 'react'
import Table from 'react-bootstrap/Table';
import Alert from 'react-bootstrap/Alert';
import { Link } from 'react-router-dom'
import axios from 'axios';
import type { IJobInfo as JobInfo } from '../api/JobContracts';
import './RegisteredJobsView.css'

function RegisteredJobsView() {
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    const [fetchErrorMessage, setFetchErrorMessage] = useState<string>();
    useEffect(() => {
            fetchJobs();
        }, []);

    function fetchJobs() {
        axios.get<JobInfo[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository`)
            .then(r => {
                console.log('Fetched jobs ok with axios.');
                setJobs(r.data);
                setFetchErrorMessage(null);
            })
            .catch(err => {
                console.error('Error fetching jobs', err)
                setFetchErrorMessage('Error fetching jobs.' + err.message);
            });
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
                    <th>IsSuccess</th>
                </tr>
            </thead>
            <tbody>
        {jobs.map((j,i) => (
            <tr key={i}>
                <td><Link to={`/jobs/${j.jobId}`}>{j.jobId}</Link></td>
                <td>{j.type}</td>
                <td>{j.description}</td>
                <td>{j.status}</td>
                <td>{j.isSuccess ? "true" : (j.isSuccess == null ? "N/A" : "false")}</td>
            </tr>
        ))}
            </tbody>
        </Table>
      </div>);
}

export default RegisteredJobsView;
