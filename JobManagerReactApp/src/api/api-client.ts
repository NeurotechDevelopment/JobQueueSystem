import axios from 'axios'
import type { IJobTypeDescriptor as JobTypeDescriptor, IJob as Job, JobType, IJobInfo as JobInfo, IJobRequest } from './JobContracts';
import { useKeycloak } from '@react-keycloak/web';

export default function useApiClient() {
    const { keycloak } = useKeycloak();

    const getJobList = async () => {
        return axios.get<JobInfo[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/jobs`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJob = async (jobId: string) => {
        return axios.get<Job>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/jobs/${jobId}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJobTypeDescriptor = async (jobType: JobType) => {
        return axios.get<JobTypeDescriptor>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types/${jobType}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJobTypeDescriptors = async() => {
        return axios.get<JobTypeDescriptor[]>(`${import.meta.env.VITE_API_BASE_URL}/JobsRepository/job-types`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const postJobRequest = async (jobRequest: IJobRequest, file: File | null) => {
        if (file) {
            const formData = new FormData();
            formData.append('file', file);
            formData.append('jobRequest', JSON.stringify(jobRequest));
            return axios.post(`${import.meta.env.VITE_JOB_PRODUCER_API}/create-job-file`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${keycloak.token}`
                }
            });
        }

        return axios.post(`${import.meta.env.VITE_JOB_PRODUCER_API}/create-job`, jobRequest, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const genTempFileLink = async (fileId: string) => {
        return axios.get<string>(`${import.meta.env.VITE_API_BASE_URL}/JobsAttachments/stream/generate-temp-link/${fileId}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    return { getJobList, getJob, getJobTypeDescriptor, getJobTypeDescriptors, postJobRequest, genTempFileLink }
}