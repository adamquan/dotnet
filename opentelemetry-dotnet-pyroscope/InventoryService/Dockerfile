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

# Pyroscope
COPY --from=pyroscope/pyroscope-dotnet:0.8.14-glibc /Pyroscope.Profiler.Native.so ./Pyroscope.Profiler.Native.so
COPY --from=pyroscope/pyroscope-dotnet:0.8.14-glibc /Pyroscope.Linux.ApiWrapper.x64.so ./Pyroscope.Linux.ApiWrapper.x64.so

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/app/Pyroscope.Profiler.Native.so
ENV LD_PRELOAD=/app/Pyroscope.Linux.ApiWrapper.x64.so

ENV PYROSCOPE_APPLICATION_NAME=InventoryService-sdk
ENV PYROSCOPE_BASIC_AUTH_USER=315081
ENV PYROSCOPE_BASIC_AUTH_PASSWORD=glc_eyJvIjoiNjAxMzgwIiwibiI6InN0YWNrLTMxNTA4MS1ocC13cml0ZS1weXJvc2NvcGUiLCJrIjoiNFFCWUg4RnBHNjhya0gySUszNDU1QzhWIiwibSI6eyJyIjoidXMifX0=
ENV PYROSCOPE_SERVER_ADDRESS=https://profiles-prod-003.grafana.net
ENV PYROSCOPE_LOG_LEVEL=debug
ENV PYROSCOPE_PROFILING_ENABLED=1
ENV PYROSCOPE_PROFILING_ALLOCATION_ENABLED=true
ENV PYROSCOPE_PROFILING_CONTENTION_ENABLED=true
ENV PYROSCOPE_PROFILING_EXCEPTION_ENABLED=true

ENV OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic MzE1MDgxOmdsY19leUp2SWpvaU5qQXhNemd3SWl3aWJpSTZJbk4wWVdOckxUTXhOVEE0TVMxdmRHeHdMWGR5YVhSbExXSmxlV3hoSWl3aWF5STZJbGxWV0dvek5uSTVOVTlNTTJRMGFFeHJPRTgwTlhBMlJDSXNJbTBpT25zaWNpSTZJblZ6SW4xOQ=="
ENV OTEL_EXPORTER_OTLP_ENDPOINT="https://otlp-gateway-prod-us-central-0.grafana.net/otlp"
ENV OTEL_METRICS_EXPORTER=none
ENV OTEL_LOGS_EXPORTER=none
ENV OTEL_METRICS_EXPORTER="otlp"
ENV OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
ENV OTEL_RESOURCE_ATTRIBUTES="service.version=1.0,deployment.environment=production"
#ENV OTEL_SERVICE_NAME="inventory-service-sdk"

ENTRYPOINT ["dotnet", "InventoryService.dll"]
