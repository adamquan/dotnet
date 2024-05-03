ARG SDK_VERSION=8.0
ARG PUBLISH_CONFIGURATION=Release

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:${SDK_VERSION} AS build
WORKDIR /src
COPY ["OrderService/OrderService.csproj", "OrderService/"]
RUN dotnet restore "OrderService/OrderService.csproj"
COPY . .
WORKDIR "/src/OrderService"
RUN dotnet build "OrderService.csproj" -c "${PUBLISH_CONFIGURATION}" -o /app/build

FROM build AS publish
RUN dotnet publish "OrderService.csproj" -c "${PUBLISH_CONFIGURATION}" -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install dependencies
RUN apt-get update && apt-get install -y zip curl
RUN mkdir /otel
RUN curl -L -o /otel/otel-dotnet-install.sh https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/latest/download/otel-dotnet-auto-install.sh
RUN chmod +x /otel/otel-dotnet-install.sh

ENV OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic MzE1MDgxOmdsY19leUp2SWpvaU5qQXhNemd3SWl3aWJpSTZJbk4wWVdOckxUTXhOVEE0TVMxdmRHeHdMWGR5YVhSbExXSmxlV3hoSWl3aWF5STZJbGxWV0dvek5uSTVOVTlNTTJRMGFFeHJPRTgwTlhBMlJDSXNJbTBpT25zaWNpSTZJblZ6SW4xOQ=="
ENV OTEL_EXPORTER_OTLP_ENDPOINT="https://otlp-gateway-prod-us-central-0.grafana.net/otlp"
ENV OTEL_METRICS_EXPORTER="otlp"
ENV OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
ENV OTEL_RESOURCE_ATTRIBUTES="service.version=1.0,deployment.environment=production"
ENV OTEL_SERVICE_NAME="order-service-auto"

ENV OTEL_DOTNET_AUTO_HOME=/otel

RUN /bin/bash /otel/otel-dotnet-install.sh

# Provide necessary permissions for the script to execute
RUN chmod +x /otel/instrument.sh

# Run the env setup script and then start the dotnet application on container startup
ENTRYPOINT ["/bin/bash", "-c", "source /otel/instrument.sh && dotnet OrderService.dll"]