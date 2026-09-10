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

# In case max upload size is defined, generate nginx config with the setting
# set :- after variable name to not throw 'unbound variable' due to set -eu instruction and set it to empty as default
if [ -n "${MAX_FILE_SIZE_UPLOAD_IN_BYTES:-}" ]
then
SIZE_IN_MB=$((${MAX_FILE_SIZE_UPLOAD_IN_BYTES} / 1024 / 1024))
cat > /etc/nginx/conf.d/additional-settings.conf <<EOF
client_max_body_size ${SIZE_IN_MB}M;
EOF

fi
exec "$@"