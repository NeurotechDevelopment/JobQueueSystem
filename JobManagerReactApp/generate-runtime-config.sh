#!/bin/sh
set -eu

# Read environment variables from docker-compose
# Write to runtime-config.js
cat > /usr/share/nginx/html/runtime-config.js <<EOF
window.__APP_CONFIG__ = {
    apiBaseUrl: "${VITE_API_BASE_URL}",
    jobProducerApi: "${VITE_JOB_PRODUCER_API}",
    odataUrl: "${VITE_ODATA_URL}",
    keycloakEndpoint: "${VITE_KEYCLOAK_ENDPOINT}",
    keycloakRealm: "${VITE_KEYCLOAK_REALM}",
    keycloakClientId: "${VITE_KEYCLOAK_CLIENT_ID}"
}
EOF
exec "$@"