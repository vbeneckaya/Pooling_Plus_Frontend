#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN cd  /src/backend/API 

RUN dotnet publish -c Release -o build/API

EXPOSE 8080

ENTRYPOINT dotnet build/API/API.dll --urls http://0.0.0.0:5000
