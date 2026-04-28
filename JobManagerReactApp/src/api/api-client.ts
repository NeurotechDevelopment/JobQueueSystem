import axios from 'axios'
import type { IJobTypeDescriptor as JobTypeDescriptor, IJob as Job, JobType, IJobInfo as JobInfo, IJobRequest } from './JobContracts';
import { useKeycloak } from '@react-keycloak/web';
import { appSettings } from '../app-config';

export default function useApiClient() {
    const { keycloak } = useKeycloak();

    const getJobList = async () => {
        return axios.get<JobInfo[]>(`${appSettings.apiBaseUrl}/JobsRepository/jobs`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJob = async (jobId: string) => {
        return axios.get<Job>(`${appSettings.apiBaseUrl}/JobsRepository/jobs/${jobId}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const removeJob = async (jobId: string) => {
        return axios.delete<number>(`${appSettings.apiBaseUrl}/JobsRepository/jobs/${jobId}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJobTypeDescriptor = async (jobType: JobType) => {
        return axios.get<JobTypeDescriptor>(`${appSettings.apiBaseUrl}/JobsRepository/job-types/${jobType}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const getJobTypeDescriptors = async() => {
        return axios.get<JobTypeDescriptor[]>(`${appSettings.apiBaseUrl}/JobsRepository/job-types`, {
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
            return axios.post(`${appSettings.jobProducerApi}/create-job-file`, formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                    Authorization: `Bearer ${keycloak.token}`
                }
            });
        }

        return axios.post(`${appSettings.jobProducerApi}/create-job`, jobRequest, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    const genTempFileLink = async (fileId: string) => {
        return axios.get<string>(`${appSettings.apiBaseUrl}/JobsAttachments/stream/generate-temp-link/${fileId}`, {
            headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        });
    }

    return { getJobList, getJob, removeJob, getJobTypeDescriptor, getJobTypeDescriptors, postJobRequest, genTempFileLink }
}