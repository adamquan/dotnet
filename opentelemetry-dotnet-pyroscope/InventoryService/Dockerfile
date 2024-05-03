FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InventoryService/InventoryService.csproj", "InventoryService/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"
COPY . .
WORKDIR "/src/InventoryService"
RUN dotnet build "InventoryService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic MzE1MDgxOmdsY19leUp2SWpvaU5qQXhNemd3SWl3aWJpSTZJbk4wWVdOckxUTXhOVEE0TVMxdmRHeHdMWGR5YVhSbExXSmxlV3hoSWl3aWF5STZJbGxWV0dvek5uSTVOVTlNTTJRMGFFeHJPRTgwTlhBMlJDSXNJbTBpT25zaWNpSTZJblZ6SW4xOQ=="
ENV OTEL_EXPORTER_OTLP_ENDPOINT="https://otlp-gateway-prod-us-central-0.grafana.net/otlp"
ENV OTEL_METRICS_EXPORTER=none
ENV OTEL_LOGS_EXPORTER=none
ENV OTEL_METRICS_EXPORTER="otlp"
ENV OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
ENV OTEL_RESOURCE_ATTRIBUTES="service.version=1.0,deployment.environment=production"
#ENV OTEL_SERVICE_NAME="inventory-service-sdk"

ENTRYPOINT ["dotnet", "InventoryService.dll"]
