// This file is used to configure the runtime environment for the application.
// Docker-compose will create such a file passing environment variables to dockerfile which will generate this file with those settings.
// app-config.ts first attempts to read settings from this file (via window object) and fallbacks to import.meta.env from .env.development file.
// On development this file is not used.