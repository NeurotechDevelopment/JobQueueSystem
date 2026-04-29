FROM node:20-alpine AS base
WORKDIR /app
COPY JobManagerReactApp/package*.json ./
RUN npm install
COPY JobManagerReactApp/ ./
RUN npm run build

FROM nginx:stable-alpine
# Copy produced build of our app from previous layer to nginx site folder
COPY --from=base /app/dist /usr/share/nginx/html
# Copy/replace default nginx config with ours that contains proxy
COPY JobManagerReactApp/nginx.docker.default.conf /etc/nginx/conf.d/default.conf
# Copy script that generates runtime-config.js from env vars
COPY JobManagerReactApp/generate-runtime-config.sh /usr/local/bin/generate-runtime-config.sh
# Make sure it is executable
RUN chmod +x /usr/local/bin/generate-runtime-config.sh
EXPOSE 80

ENTRYPOINT ["/usr/local/bin/generate-runtime-config.sh"]
CMD ["nginx", "-g", "daemon off;"]