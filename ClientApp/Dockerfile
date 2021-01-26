# Multi-stage

# 1) Node image for building frontend assets
FROM node:10-alpine as builder
WORKDIR /app
COPY . .
RUN npm install
RUN npm run build

# 2) nginx stage to serve frontend assets
FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
RUN rm /etc/nginx/conf.d/default.conf
COPY --from=builder /app/dist /usr/share/nginx/html
COPY --from=builder /app/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=builder /app/nginx-upstream.conf /etc/nginx/upstream.conf
COPY --from=builder /app/ssl /etc/nginx/ssl

EXPOSE 443
ENTRYPOINT ["nginx","-g","daemon off;"]
