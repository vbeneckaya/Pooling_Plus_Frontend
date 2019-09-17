
#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app

WORKDIR /app

COPY /src/backend/ .

RUN dotnet  publish API/ -c Release -o app/build/

ENTRYPOINT  cd API/app/build && dotnet API.dll --urls http://0.0.0.0:5000