open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Azure
open Azure.Identity

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
        |> ignore
    builder.Services.AddAzureClients(fun clientBuilder ->
        clientBuilder.UseCredential(new DefaultAzureCredential()) |> ignore
        clientBuilder.AddServiceBusClientWithNamespace |> ignore
        )
    let app = builder.Build()

    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore
    app.MapReverseProxy() |> ignore
    app.Run()

    0 // Exit code
