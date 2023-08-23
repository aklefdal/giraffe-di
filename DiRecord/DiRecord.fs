module DiRecord.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open DiRecord.Storage

let addHandler (storage: StorageService) (x: int) : HttpHandler =
    fun (next: HttpFunc) (ctx: HttpContext) ->
        task {
            let! currentSum = storage.add x
            return! text $"Current sum is {currentSum}" next ctx
        }

let webApp ( storage: StorageService)=
    choose
        [ GET >=> choose [ routef "/add/%i" (addHandler storage) ]
          setStatusCode 404 >=> text "Not Found" ]

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

type Startup(env: IHostEnvironment) =
    let config =
        let builder =
            ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
        builder.Build()

    member _.ConfigureServices(services: IServiceCollection) =
        services
            .AddGiraffe() |> ignore

    member _.ConfigureLogging(builder: ILoggingBuilder) =
        builder
            .AddConsole()
            .AddDebug() |> ignore

    member _.Configure(app: IApplicationBuilder) =
        let storage = config |> createStorageService
        (match env.IsDevelopment() with
         | true -> app.UseDeveloperExceptionPage()
         | false ->
             app
                 .UseGiraffeErrorHandler(errorHandler)
                 .UseHttpsRedirection())
            .UseGiraffe(webApp storage)

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let startup = Startup(builder.Environment)
    startup.ConfigureServices(builder.Services)
    let app = builder.Build()
    startup.Configure(app)
    app.Run()
    0
