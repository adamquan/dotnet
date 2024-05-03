# .NET Core App OpenTelemetry Auto-Instrumentation & Grafana Cloud

This is a demo of **the .NET Core Apps** OpenTelemetry [auto-instrumentation](https://opentelemetry.io/docs/languages/net/automatic/).

The demo app contains two interconnected microservices. Both will be instrumented using OpenTelemetry automatically: 

* Order Service - The microservices that creates an order for the passed item code and quantity.
* Inventory Service - The microservices that verifies whether the requested quantity available in inventory and reduce the quantity when the inventory is claimed.

![architecture.png](https://github.com/adamquan/dotnet/blob/main/images/architecture.png)

The Order service will call the Inventory service to verify and also claim the requested quantity for the order for the item code.

These two .NET core apps will be running in docker containers. We add OpenTelemetry auto-instrumentation in these projects and observe the trace in Grafana Cloud.

### Configure your .NET Core Application

Since we are using auto-instrumentation, no code change is needed to collect OpenTelemetry traces.

## Run

You can run the applications using the docker-compose file.

```bash
docker-compose up --build
```

Now create an order using the Order Service. This Order service will verify the inventory, create an order, and claim the inventory to get processed.

```bash
curl --location --request POST 'http://localhost:5500/api/Order' \
--header 'Content-Type: application/json' \
--data-raw '{
    "itemCode": "PIZZA",
    "quantity": "1",
    "username": "Siva"
}'
```

Navigate to the Grafana Cloud UI, and see all the traces.

![auto.png](https://github.com/adamquan/dotnet/blob/main/images/auto.png)
