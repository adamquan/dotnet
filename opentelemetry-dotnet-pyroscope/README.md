# .NET Core App Span Profile & Grafana Cloud

This is a demo of instrumenting **the .NET Core Apps** with:
* OpenTelemetry for traces
* [Pyroscope](https://grafana.com/oss/pyroscope/) for profiles
Profiles will be linked to traces using [span profile](https://grafana.com/docs/pyroscope/latest/configure-client/trace-span-profiles/dotnet-span-profiles/).

The demo app contains two interconnected microservices. Both will be instrumented using OpenTelemetry .NET SDK, and Pyroscope: 

* Order Service - The microservices that creates an order for the passed item code and quantity.
* Inventory Service - The microservices that verifies whether the requested quantity available in inventory and reduce the quantity when the inventory is claimed.

![OrderService.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1607114382811/yH1AfrWIg.png)

The Order service will call the Inventory service to verify and also claim the requested quantity for the order for the item code.

These two .NET core apps will be running in docker containers. We add the OpenTelemetry .NET library, and [Pyroscope profiler](https://grafana.com/docs/pyroscope/latest/configure-client/trace-span-profiles/dotnet-span-profiles/#configure-the-pyroscopeopentelemetry-package) in these projects and observe the trace in Grafana Cloud.

### Configure your .NET Core Application

ASP.NET Core instrumentation must be enabled at the application startup, for both OpenTelemetry and Pyroscope. 

To start collecting Span Profiles for your .NET application, you need to include Pyroscope.OpenTelemetry in your code. This package provides a SpanProcessor implementation, which connects the two telemetry signals (traces and profiles) together.

```csharp
            services.AddOpenTelemetry().WithTracing(builder => builder
                                .AddAspNetCoreInstrumentation()
                                .AddSource("OrderService-sdk")
                                .AddOtlpExporter()
                                .AddProcessor(new PyroscopeSpanProcessor())
                                .ConfigureResource(resource =>
                                    resource.AddService(
                                        serviceName: "OrderService-sdk")) // trace service name
            );
```
With the span processor registered, spans created automatically (for example, HTTP handlers) and manually (ActivitySource.StartActivity()) have profiling data associated with them.

Since the app is super simple and load is light, we added some cpu intensive code purposely, to make sure we capature the profiles:

```csharp

        public void KillCore()
        {
            Random rand = new Random();

            Stopwatch watch = new Stopwatch();
            watch.Start();            

            long num = 0;
            while(true)
            {
                num += rand.Next(100, 1000);
                if (num > 1000000) { num = 0; }
                if (watch.ElapsedMilliseconds > 3000) break;
            }
        }
```
Because the traces and profiles are linked by labels, make sure you label them accordingly. For example, for the OrderService:

* In [Dockerfile](https://github.com/adamquan/dotnet/blob/main/opentelemetry-dotnet-pyroscope/OrderService/Dockerfile) we set: `ENV PYROSCOPE_APPLICATION_NAME=OrderService-sdk`
* In [Startup.cs](https://github.com/adamquan/dotnet/blob/main/opentelemetry-dotnet-pyroscope/OrderService/Startup.cs) we also set the service name to: `serviceName: "OrderService-sdk"`

## Run

You can run the applications using the docker-compose file.

```bash
docker-compose up --build
```

Now create an order using the Order Service. This Order service will verify the inventory, create an order, and claim the inventory to get processed.

```bash
curl --location --request POST 'http://localhost:4500/api/Order' \
--header 'Content-Type: application/json' \
--data-raw '{
    "itemCode": "PIZZA",
    "quantity": "1",
    "username": "Siva"
}'
```

Navigate to the Grafana Cloud UI, and see all traces with associated profles.

![span-profile.png](https://github.com/adamquan/dotnet/blob/main/images/span-profile.png)
