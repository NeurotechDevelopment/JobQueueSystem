import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { keycloak, keycloakInitOptions } from './Keycloak.config'
import { ReactKeycloakProvider } from "@react-keycloak/web";
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css'
import App from './App'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ReactKeycloakProvider authClient={keycloak} initOptions={keycloakInitOptions}>
            <App />
        </ReactKeycloakProvider>
  </StrictMode>,
)
