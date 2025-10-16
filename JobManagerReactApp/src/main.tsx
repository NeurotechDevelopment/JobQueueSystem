import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import Keycloak from 'keycloak-js'
import { ReactKeycloakProvider } from "@react-keycloak/web";
import 'bootstrap/dist/css/bootstrap.min.css';
import './index.css'
import App from './App.tsx'

const keycloakProv = new Keycloak({
    url: `${import.meta.env.VITE_KEYCLOAK_ENDPOINT}`,
    realm: import.meta.env.VITE_KEYCLOAK_REALM,
    clientId: import.meta.env.VITE_KEYCLOAK_CLIENT_ID,
});

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <ReactKeycloakProvider authClient={keycloakProv}>
            <App />
        </ReactKeycloakProvider>
  </StrictMode>,
)
