FROM node:20-alpine AS base
WORKDIR /app
COPY JobManagerReactApp/package*.json ./
RUN npm install
COPY JobManagerReactApp/ ./
RUN npm run build

FROM nginx:stable-alpine
COPY --from=base /app/dist /usr/share/nginx/html
COPY JobManagerReactApp/generate-runtime-config.sh /usr/local/bin/generate-runtime-config.sh
RUN chmod +x /usr/local/bin/generate-runtime-config.sh
EXPOSE 80

ENTRYPOINT ["/usr/local/bin/generate-runtime-config.sh"]
CMD ["nginx", "-g", "daemon off;"]