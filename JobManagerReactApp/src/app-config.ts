export type AppSettings = {
    apiBaseUrl: string,
    jobProducerApi: string,
    odataUrl: string,
    keycloakEndpoint: string,
    keycloakRealm: string,
    keycloakClientId: string
}

declare global {
    interface Window {
        // Runtime config loaded from runtime-config.js before React bootstrap.
        // In containers, this file can be generated/overwritten at startup.
        __APP_CONFIG__?: Partial<AppSettings>;
    }
}

const getAppSettings =  ():AppSettings =>  {
    const runtimeConfig = window.__APP_CONFIG__ ?? {};
    return {
      apiBaseUrl: runtimeConfig.apiBaseUrl ?? import.meta.env.VITE_API_BASE_URL ?? '',
      jobProducerApi: runtimeConfig.jobProducerApi ?? import.meta.env.VITE_JOB_PRODUCER_API ?? '',
      odataUrl: runtimeConfig.odataUrl ?? import.meta.env.VITE_ODATA_URL ?? '',
      keycloakEndpoint: runtimeConfig.keycloakEndpoint ?? import.meta.env.VITE_KEYCLOAK_ENDPOINT ?? '',
      keycloakRealm: runtimeConfig.keycloakRealm ?? import.meta.env.VITE_KEYCLOAK_REALM ?? '',
      keycloakClientId: runtimeConfig.keycloakClientId ?? import.meta.env.VITE_KEYCLOAK_CLIENT_ID ?? ''
    }
}

export const appSettings = getAppSettings();