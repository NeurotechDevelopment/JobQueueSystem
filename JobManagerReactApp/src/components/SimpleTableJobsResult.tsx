import Table from 'react-bootstrap/Table'
import Button from 'react-bootstrap/Button'
import ButtonGroup from 'react-bootstrap/ButtonGroup'
import OverlayTrigger from 'react-bootstrap/OverlayTrigger'
import Tooltip from 'react-bootstrap/Tooltip'
import { Link } from 'react-router-dom'
import { type IJobInfo } from '../api/JobContracts'
import { type JSX } from 'react'
import './SimpleTableJobsResult.css'

export interface SimpleTableJobsResultProps {
    jobsResult: IJobInfo[];
    onDeleteJob: (jobId: string) => void;
}

const SimpleTableJobsResult = ({ jobsResult, onDeleteJob }: SimpleTableJobsResultProps): JSX.Element => {

    if (!jobsResult || jobsResult.length === 0) {
        return <div>No jobs found.</div>;
    }
    return (<Table bordered striped hover responsive="xs">
        <colgroup>
            <col style={{ width: "32%" }} />
            <col style={{ width: "10%" }} />
            <col style={{ width: "36%" }} />
            <col style={{ width: "7%" }} />
            <col style={{ width: "7%" }} />
            <col style={{ width: "8%" }} />
        </colgroup>
        <thead>
            <tr>
                <th>JobId</th>
                <th>Type</th>
                <th>Description</th>
                <th>Status</th>
                <th>Success</th>
                <th>Actions</th>
            </tr>
        </thead>
        <tbody>
            {jobsResult.map((j, i) => (
                <tr key={i}>
                    <td><Link to={`/jobs/${j.jobId}`}>{j.jobId}</Link></td>
                    <td>{j.type}</td>
                    <td className='text-truncate'>
                        <OverlayTrigger
                            placement="auto"
                            delay={{ show: 250, hide: 400 }}
                            overlay={<Tooltip id="description-tooltip">
                                {j.description}
                            </Tooltip>}
                        >
                            <span>{j.description}</span>
                        </OverlayTrigger>
                    </td>
                    <td>{j.status}</td>
                    <td>{j.isSuccess ? "true" : (j.isSuccess == null ? "N/A" : "false")}</td>
                    <td>
                        <ButtonGroup aria-label="Basic example">
                            <Button as={Link} to={`/jobs/${j.jobId}`} variant="light"><i className="bi bi-eye text-success"></i></Button>
                            <Button variant="light" onClick={() => onDeleteJob(j.jobId!)}><a><i className="bi bi-trash text-danger"></i></a></Button>
                        </ButtonGroup>
                    </td>
                </tr>))}
        </tbody>
    </Table>
    );
}
export default SimpleTableJobsResult;