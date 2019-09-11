FROM node:10-slim as builder
RUN mkdir /app
WORKDIR /app

COPY src/frontend/ .

RUN npm install
RUN npm run build

FROM nginx AS RESULT

COPY --from=builder /app/build /usr/share/nginx/html

RUN chgrp -R root /var/cache/nginx /var/run /var/log/nginx /run && \
    chmod -R 777 /var /run

COPY ./nginx/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 8080

RUN sed -i.bak 's/^user/#user/' /etc/nginx/nginx.conf
