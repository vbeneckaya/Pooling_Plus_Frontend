#Build image
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build

RUN mkdir -p /app

RUN chmod -R 777 /app


WORKDIR /app

COPY /src/backend/ .

RUN dotnet build API/ -c Release -o /app/build
RUN cd /app/build && ls -la

# Runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
EXPOSE 8080

WORKDIR /app

RUN mkdir logs && chmod -R 777 /app

USER 1001

COPY --from=build /app/build .


ENTRYPOINT dotnet Api.dll --urls http://0.0.0.0:5000