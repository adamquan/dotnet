# .NET Core App OpenTelemetry Instrumentation & Grafana Cloud

This is a demo of **the .NET Core Apps** OpenTelemetry instrumentation. This repo was cloned from [opentelemetry-dotnet-demo](https://github.com/ksivamuthu/opentelemetry-dotnet-demo) and modified to showcase both auto-instrumentation, OTEL .Net SDK and [Grafana Cloud](https://grafana.com/products/cloud/).

* [Auto instrumentation](https://github.com/adamquan/dotnet/tree/main/opentelemetry-dotnet-auto) with OTEL [auto-instrumentation](https://opentelemetry.io/docs/languages/net/automatic/).
* [Manual instrumentation](https://github.com/adamquan/dotnet/tree/main/opentelemetry-dotnet-grafana) with [.NET SDK](https://opentelemetry.io/docs/languages/net/automatic/custom/)

The demo app contains two interconnected microservices:

* Order Service - The microservices that creates an order for the passed item code and quantity.
* Inventory Service - The microservices that verifies whether the requested quantity available in inventory and reduce the quantity when the inventory is claimed.

![architecture.png](https://github.com/adamquan/dotnet/blob/main/images/architecture.png)

The Order service will call the Inventory service to verify and also claim the requested quantity for the order for the item code.

These two .NET core apps will be running in docker containers. We add OpenTelemetry instrumentation in these projects and observe the trace in Grafana Cloud.