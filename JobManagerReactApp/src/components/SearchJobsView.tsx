import { useState, type JSX } from 'react'
import Button from 'react-bootstrap/Button'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import Form from 'react-bootstrap/Form'
import Row from 'react-bootstrap/Row'
import Col from 'react-bootstrap/Col'
import Accordion from 'react-bootstrap/Accordion'
import { type IJobInfo, JobType as IJobType, JobStatus as IJobStatus } from '../api/JobContracts'
import { AxiosClient, type AxiosRequestConfig } from '@odata2ts/http-client-axios'
import { ContractsService } from '../odata/ContractsService'
import { type Job, JobStatus, JobType } from '../odata/ContractsModel'
import { QFilterExpression } from "@odata2ts/odata-query-objects";
import { useKeycloak } from '@react-keycloak/web';
import SimpleTableJobsResult from './SimpleTableJobsResult'

const SearchJobsView = (): JSX.Element => {
    const { keycloak } = useKeycloak();
    const [ jobsResult, setJobsResult ] = useState<Job[] | null>();

    // Form search fields
    const [ jobId, setJobId ] = useState<string>('');
    const [ jobStatus, setJobStatus ] = useState<JobStatus | null>(null);
    const [ jobType, setJobType ] = useState<JobType | null>(null);
    const [ receivedAtAfter, setReceivedAtAfter ] = useState<Date | null>(null);
    const [ description, setDescription ] = useState<string>('');

    const clearFilter = () : void => {
        setJobId('');
        setJobStatus(null);
        setJobType(null);
        setReceivedAtAfter(null);
        setReceivedAtAfter(null);
        setDescription('');
    }

    const performSearch = async() : Promise<void> => {
        const config: AxiosRequestConfig = {
        headers: {
                Authorization: `Bearer ${keycloak.token}`
            }
        }
        const httpClient = new AxiosClient(config);
        const odataUrl = `${import.meta.env.VITE_ODATA_URL}`
        const contractsService = new ContractsService(httpClient, odataUrl);
        const result = await contractsService.Jobs().query((builder, qJob) => {
            let filterExpression : QFilterExpression = new QFilterExpression();
        
            if (jobId && jobId.trim() !== '') {
                filterExpression = filterExpression.and(qJob.JobId.eq(jobId));
            }

            if (jobStatus) {
                filterExpression = filterExpression.and(qJob.Status.eq(jobStatus));
            }

            if (jobType) {
                filterExpression = filterExpression.and(qJob.Type.eq(jobType));
            }

            if (receivedAtAfter) {
                filterExpression = filterExpression.and(qJob.ReceivedAt.ge(receivedAtAfter.toISOString()));
            }

            if (description) {
                filterExpression = filterExpression.and(qJob.Description.contains(description))
            }

            return builder.filter(filterExpression)
        });

        setJobsResult(result.data.value)
    }

    return (
        <div>
            <Accordion defaultActiveKey="0">
                    <Accordion.Item eventKey="0">
                        <Accordion.Header>
                            <h4 className="mb-0">Search Filter</h4>
                        </Accordion.Header>
                        <Accordion.Body>
                            <Form>
                                {/* Row 1 */}
                                <Row className="g-3">
                                    <Col md={4}>
                                        <Form.Group controlId="searchForm.TaskId">
                                            <Form.Label>Task ID</Form.Label>
                                            <Form.Control
                                                type="text"
                                                placeholder="00000000-0000-0000-0000-000000000000"
                                                onChange={ (e) => setJobId(e.target.value)}
                                                value={jobId}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col md={4}>
                                        <Form.Group controlId="searchForm.TaskType">
                                            <Form.Label>Task Type</Form.Label>
                                            <Form.Select
                                             onChange={(e) => {
                                                if (e.target.selectedIndex > 0) 
                                                    setJobType(e.target.value as unknown as JobType)
                                                else
                                                    setJobType(null)
                                                }}
                                                value={jobType ?? ''}>
                                                <option>Select task type</option>
                                                {Object.keys(JobType)
                                                    .filter((key) => isNaN(Number(key)))
                                                    .map((jt) => (
                                                        <option key={jt} value={jt}>
                                                            {jt}
                                                        </option>
                                                    ))}
                                            </Form.Select>
                                        </Form.Group>
                                    </Col>
                                    <Col md={4}>
                                        <Form.Group controlId="searchForm.TaskStatus">
                                            <Form.Label>Status</Form.Label>
                                            <Form.Select onChange={(e) => {
                                                if (e.target.selectedIndex > 0)                                                
                                                    setJobStatus(e.target.value as unknown as JobStatus)
                                                else
                                                    setJobStatus(null)
                                            }}
                                            value={jobStatus ?? ''}
                                            >
                                                <option>Select status</option>
                                                {Object.keys(JobStatus)
                                                    .filter((key) => isNaN(Number(key)))
                                                    .map((jt) => (
                                                        <option key={jt} value={jt}>
                                                            {jt}
                                                        </option>
                                                    ))}
                                            </Form.Select>
                                        </Form.Group>
                                    </Col>
                                </Row>

                                {/* Row 2 */}
                                <Row className="g-3 mt-2">
                                    <Col md={4}>
                                        <Form.Group controlId="searchForm.TaskCreatedAt">
                                            <Form.Label>Registered After</Form.Label>
                                            <Form.Control type="date" 
                                             value={receivedAtAfter != null ? receivedAtAfter.toISOString().substring(0, 10) : ''} 
                                             onChange={(e) => setReceivedAtAfter(new Date(e.target.value))}
                                            />
                                        </Form.Group>
                                    </Col>
                                    <Col md={8}>
                                        <Form.Group controlId="searchForm.TaskDescription">
                                            <Form.Label>Description</Form.Label>
                                            <Form.Control as="textarea" rows={1} value={description} onChange={(e) => setDescription(e.target.value)} />
                                        </Form.Group>
                                    </Col>
                                </Row>

                                {/* Buttons aligned to the right */}
                                <div className="d-flex justify-content-end mt-3">
                                    <ButtonGroup>
                                        <Button variant="primary" onClick={performSearch}>Search</Button>
                                        <Button variant="secondary" onClick={clearFilter}>Clear</Button>
                                    </ButtonGroup>
                                </div>
                            </Form>
                        </Accordion.Body>
                    </Accordion.Item>
            </Accordion>
            <hr/>
            <h4>Search Results</h4>
            {jobsResult &&
            <SimpleTableJobsResult jobsResult={jobsResult.map((x) : IJobInfo =>  ({ 
                                                        jobId: x.JobId,
                                                        description: x.Description,
                                                        type: x.Type as unknown as IJobType,
                                                        status: x.Status as unknown as IJobStatus,
                                                        isSuccess: x.IsSuccess
                                                    }
                                                    ))} onDeleteJob={() => {}} />
            }
        </div>
    )
}

export default SearchJobsView