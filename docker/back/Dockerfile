#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app


WORKDIR /app

COPY /src/backend/ .
COPY /src/backend/ /app/build/

RUN dotnet build API/ -c Release -o /app/build/API


#Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
EXPOSE 8080

WORKDIR /app

#RUN mkdir logs && chmod -R 777 /app
#RUN apt-get update && apt install iputils-ping -y

COPY --from=build /app/build .
RUN ls -la API/
#ENTRYPOINT ping 8.8.8.8
RUN cd API/
ENTRYPOINT  dotnet API.dll --urls http://0.0.0.0:5000
