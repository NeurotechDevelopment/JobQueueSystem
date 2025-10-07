import { Navbar, Nav, Container } from "react-bootstrap";
import { LinkContainer } from "react-router-bootstrap";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import RegisteredJobsView from './components/RegisteredJobsView'
import NewJobSelector from './components/NewJobSelector'
import { JobType } from './api/JobContracts.ts'

import "bootstrap-icons/font/bootstrap-icons.css";
import './App.css'

function App() {
    return (
        <BrowserRouter>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand>Job Manager</Navbar.Brand>
                    <Nav className="me-auto">
                        <LinkContainer to="/">
                            <Nav.Link>Jobs</Nav.Link>
                        </LinkContainer>
                        <LinkContainer to="/new">
                            <Nav.Link>Create Job</Nav.Link>
                        </LinkContainer>
                    </Nav>
                </Container>
            </Navbar>

            <Container className="mt-4">
                <Routes>
                    <Route path="/" element={<RegisteredJobsView />} />
                    <Route path="/new" element={<NewJobSelector jobType={JobType.ConvertExcelToPdf} />} />
                </Routes>
            </Container>
        </BrowserRouter>
  )
}

export default App
