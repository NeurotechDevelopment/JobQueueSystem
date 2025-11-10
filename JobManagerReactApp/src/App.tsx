import { Navbar, Nav, Container } from 'react-bootstrap'
import { LinkContainer } from 'react-router-bootstrap'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import Button from 'react-bootstrap/Button'
import Card from 'react-bootstrap/Card'
import RegisteredJobsView from './components/RegisteredJobsView'
import SearchJobsView from './components/SearchJobsView'
import NewJobSelector from './components/NewJobSelector'
import JobDetailsView from './components/JobDetailsView'
import {  useKeycloak } from '@react-keycloak/web';
import "bootstrap-icons/font/bootstrap-icons.css";
import './App.css'

function App() {
    const { keycloak, initialized } = useKeycloak();
    return (
        <BrowserRouter>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand><i className="bi bi-leaf-fill text-info"> </i>Tasks Manager</Navbar.Brand>
                    {initialized && keycloak.authenticated &&
                        <>
                            <Nav className="me-auto">
                                <LinkContainer to="/">
                                    <Nav.Link>All Tasks</Nav.Link>
                                </LinkContainer>
                                <LinkContainer to="/search">
                                    <Nav.Link>Search</Nav.Link>
                                </LinkContainer>
                                <LinkContainer to="/new">
                                    <Nav.Link>Create New Task</Nav.Link>
                                </LinkContainer>
                            </Nav>
                        </>}
                     <Nav className="ms-auto">
                        {!keycloak.authenticated &&
                            <Button onClick={() => keycloak.login()}> Login</Button>}
                        {keycloak.authenticated &&
                            <>
                                Hello, {keycloak.tokenParsed!.name}
                                <Button onClick={() => keycloak.logout()}>Logout ({keycloak.tokenParsed!.preferred_username})</Button>
                            </>}
                     </Nav>
                    </Container>
                </Navbar>

                <Container className="mt-4">
                    {initialized && keycloak.authenticated &&
                    <Routes>
                        <Route path="/" element={<RegisteredJobsView />} />
                        <Route path="/search" element={<SearchJobsView />} />
                        <Route path="/new" element={<NewJobSelector />} />
                        <Route path="/jobs/:jobId" element={<JobDetailsView />} />
                    </Routes>}
                    {!keycloak.authenticated &&
                    <Card border='warning' className='fs-4  text-center'>
                        <Card.Body>Currently you are not logged in. Please use Login button at the top right menu.</Card.Body>
                    </Card>}
                </Container>
        </BrowserRouter>
  )
}

export default App
