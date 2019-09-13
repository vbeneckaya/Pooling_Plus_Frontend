#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app


WORKDIR /app

COPY /src/backend/ .

RUN dotnet publish --no-restore API/ -c Release -o /app/build/
ENTRYPOINT ping 8.8.8.8
#ENTRYPOINT  dotnet /app/build/API.dll --urls http://0.0.0.0:5000
# Runtime image
#FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
#EXPOSE 8080

#WORKDIR /app

#RUN mkdir logs && chmod -R 777 /app
#RUN apt-get update && apt install iputils-ping -y

#COPY --from=build /app/build .

#ENTRYPOINT ping 8.8.8.8

#ENTRYPOINT  dotnet API.dll --urls http://0.0.0.0:5000