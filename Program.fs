open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Azure
open Azure.Identity
open Cache

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services
        .AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    |> ignore

    builder.Services.AddAzureClients(fun clientBuilder ->
        clientBuilder.UseCredential(new DefaultAzureCredential(true)) |> ignore

        clientBuilder.AddServiceBusClient(builder.Configuration.GetSection("ServiceBus"))
        |> ignore)

    builder.Services.AddControllers() |> ignore
    builder.Services.AddHostedService<Service>() |> ignore
    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore
    app.MapReverseProxy() |> ignore
    app.Run()

    0 // Exit code
