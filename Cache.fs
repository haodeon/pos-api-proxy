module Cache

open System
open System.Threading
open Microsoft.Extensions.Hosting
open Azure.Messaging.ServiceBus

type Service(client: ServiceBusClient) =
    inherit BackgroundService()
    let processor = client.CreateProcessor("alertqueue")

    let MessageHandler (args: ProcessMessageEventArgs) =
        task {
            let body = args.Message.Body.ToString();
            Console.WriteLine(body)
        }
    
    let ErrorHandler (args: ProcessErrorEventArgs) =
        task {
            // the error source tells me at what point in the processing an error occurred
            Console.WriteLine(args.ErrorSource);
            // the fully qualified namespace is available
            Console.WriteLine(args.FullyQualifiedNamespace);
            // as well as the entity path
            Console.WriteLine(args.EntityPath);
            Console.WriteLine(args.Exception.ToString());
        }

    override x.ExecuteAsync(stoppingToken: CancellationToken) =
        task {
            processor.add_ProcessMessageAsync(fun args -> MessageHandler args)
            processor.add_ProcessErrorAsync(fun args -> ErrorHandler args)
            processor.StartProcessingAsync(stoppingToken) |> ignore
            printfn "Hello world"
        }
