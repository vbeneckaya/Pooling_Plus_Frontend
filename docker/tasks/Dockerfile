#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app

WORKDIR /app

COPY /src/backend/ .

COPY /docker/tasks/cron/cron.d /etc/cron.d
COPY /docker/tasks/cron/cron.sh /app/
RUN chmod +x /app/cron.sh
RUN touch /var/log/cron.log

RUN dotnet publish Tasks/ -c Release -o /app/build/
RUN nohup sh -c 'while true; do sh cron.sh; sleep 60; done;' &
CMD ping 8.8.8.8