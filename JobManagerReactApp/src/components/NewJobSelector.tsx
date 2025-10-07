import { useState, useEffect } from 'react'
import * as JobContracts from '../api/JobContracts.ts';
import axios from 'axios';

type JobType = JobContracts.JobType
type IJobTypeDescriptor = JobContracts.IJobTypeDescriptor
function NewJobSelector({ jobType }: { jobType: JobType }) {
    const [jobTypeDescriptor, setJobTypeDescriptor] = useState<IJobTypeDescriptor>();
    useEffect(() => {
        axios.get<IJobTypeDescriptor>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types/${jobType}`)
        .then(r => {
                console.log('Fetched job type descriptor ok with axios.' + r.data);
                setJobTypeDescriptor(r.data);
            })
            .catch(err => {
                console.error('Error fetching job type descriptor', err);
            });
    });


    return (
        <div>
            <h2>New Job Selector Component</h2>
            <p>Selected Job Type: {jobType}</p>
            <p>Description:{ jobTypeDescriptor && jobTypeDescriptor.description}</p>
            <p>PayloadJsonSchema:{jobTypeDescriptor && jobTypeDescriptor.payloadJsonSchema}</p>
            <p>ResultJsonSchema:{jobTypeDescriptor && jobTypeDescriptor.resultJsonSchema}</p>
        </div>
    );
}

export default NewJobSelector;