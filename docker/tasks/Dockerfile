
#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app

WORKDIR /app

COPY /src/backend/ .

COPY /docker/tasks/cron/cron.d /etc/cron.d

RUN touch /var/log/cron.log

RUN dotnet publish Tasks/ -c Release -o app/build/

ENTRYPOINT ["tail", "-f", "/var/log/cron.log"]