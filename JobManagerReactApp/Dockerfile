FROM node:20-alpine AS base
WORKDIR /app
COPY JobManagerReactApp/package*.json ./
RUN npm install
COPY JobManagerReactApp/ ./
RUN npm run build

FROM nginx:stable-alpine
COPY --from=base /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
