# Aspire Orleans Client-Silo Example

This example shows and Orleans client and silo running in separate processes,
configured for hosting and deployment using .NET Aspire and Aspirate (aspir8).

There are three App projects in this solution:

- **Silo**: The Orleans silo host.
- **OrleansClientApi**: A simple Web project exposing an API and using the Orleans client.
- **NonClientApi**: A simple Web project exposing an API not using the Orleans client.

## Running locally

Start the AppHost project by any of the following methods:

- Run the `AppHost` project in Visual Studio.
- Install the Rider Aspire Plugin in Rider and run the `AppHost: http` configuration in Rider.
- `dotnet run --project AppHost` from the command line.

Running the solution will open the Aspire dashboard in your browser. 
You can use the dashboard to open the Swagger Api endpoints for the 
OrleansClientApi and NonClientApi projects.

## Deploying to Kubernetes

- Launch Rancher Desktop to provide a local Kubernetes cluster (Docker Desktop
also provides a Kubernetes cluster, but Docker Desktop is more difficult to
expose the required ports).
- From the command line, perform the following steps to deploy the solution to Kubernetes:

```bash
cd AppHost
aspirate init
aspirate generate
aspirate apply
```

- In the Rancher Desktop UI, go to the Port Forwarding tab forward:

  - aspire-dashboard (dashboard-ui)
  - nonclientapi (http)
  - orleansclientapi (http)

- For each of the exposed ports, browse to `http://localhost:<port>` to access the services.

- When you are done, run `aspirate destroy` to remove the Kubernetes deployment.