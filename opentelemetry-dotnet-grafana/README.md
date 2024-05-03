# .NET Core Application OpenTelemetry Instrumentation & Grafana Cloud

Demo of instrumenting **the .NET Core Apps** with OpenTelemetry.

The demo app contains two interconnected microservices. Both will be instrumented using OpenTelemetry .NET SDK: 

* Order Service - The microservices that creates an order for the passed item code and quantity.
* Inventory Service - The microservices that verifies whether the requested quantity available in inventory and reduce the quantity when the inventory is claimed.

![OrderService.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1607114382811/yH1AfrWIg.png)

The Order service will call the Inventory service to verify and also claim the requested quantity for the order for the item code.

These two .NET core apps will be running in docker containers. We add the OpenTelemetry .NET library in these projects and observe the trace in Grafana Cloud.

### Configure your .NET Core Application

ASP.NET Core instrumentation must be enabled at the application startup. Configure the OpenTelemetry Tracing in the ConfigureServices method using the ServiceCollection extension method provided by OpenTelemetry.Extensions.Hosting.

```csharp
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Trace;

var resourceBuilder = ResourceBuilder.CreateDefault().AddService("OrderService");
services.AddOpenTelemetry().WithTracing(builder => builder.AddOtlpExporter()
                    .AddSource("OrderService-sdk")
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter()  
        .ConfigureResource(resource =>
            resource.AddService(
                serviceName: "OrderService-sdk"))
);
```

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

Navigate to the Grafana Cloud UI, and see all the traces.

![Screen Shot 2020-12-04 at 4.03.51 PM.png](https://cdn.hashnode.com/res/hashnode/image/upload/v1607115887365/e87AmzRGB.png)
