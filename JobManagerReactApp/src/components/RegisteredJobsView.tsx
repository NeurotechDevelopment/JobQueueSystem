import { useState, useEffect } from 'react'
import axios from 'axios';
import * as JobContracts from '../api/JobContracts';

// Define type to omit prefixing the ns
// (This since named export isn't working yet for some reason, I export all types from JobContracts)
type JobInfo = JobContracts.IJobInfo
function RegisteredJobsView() {
    const [jobs, setJobs] = useState<JobInfo[]>([]);
    useEffect(() => {
            axios.get<JobInfo[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository`)
                .then(r => {
                    console.log('Fetched jobs ok with axios.');
                    setJobs(r.data);
                })
                .catch(err => console.error('Error fetching jobs', err));
        },
        []);
    
    return (<div>
        <h1>Registered Jobs</h1>
        <table>
            <thead>
                <tr>
                    <td>JobId</td>
                    <td>Type</td>
                    <td>Status</td>
                    <td>IsSuccess</td>
                    <td>Error message</td>
                </tr>
            </thead>
        <tbody>
        {jobs.map((j,i) => (
            <tr key={i}>
                <td>{j.jobId}</td><td>{j.type}</td><td>{j.status}</td>
                <td>{j.isSuccess ? "true" : (j.isSuccess == null ? "N/A" : "false")}</td><td>{j.errorMessage}</td>
            </tr>
        ))}
        </tbody>
        </table>
            </div>);
}

export default RegisteredJobsView;
