import Keycloak from 'keycloak-js'
import { appSettings } from './app-config';

export const keycloak = new Keycloak({
    url: appSettings.keycloakEndpoint,
    realm: appSettings.keycloakRealm,
    clientId: appSettings.keycloakClientId
});

export const keycloakInitOptions = {
    onLoad: 'check-sso',                      // perform a silent session 
    flow: 'standard',                         // use the OIDC Authorization Code Flow
    pkceMethod: 'S256',                       // enforce PKCE for extra SPA security
    silentCheckSsoRedirectUri: `${window.location.origin}/silent-check-sso.html`,
    checkLoginIframe: true,                   // enable the hidden iframe for silent token refresh
    checkLoginIframeInterval: 30,             // polling interval in seconds
    enableLogging: true,                      // turn on adapter debug logging
};