module NoDi.App

open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe

let mutable currentSum = 0

let addHandler (x: int) =
    currentSum <- currentSum + x
    text $"Current sum is {currentSum}"

let webApp =
    choose
        [ GET >=> choose [ routef "/add/%i" addHandler ]
          setStatusCode 404 >=> text "Not Found" ]

let errorHandler (ex: Exception) (logger: ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")

    clearResponse
    >=> setStatusCode 500
    >=> text ex.Message

type Startup(env: IHostEnvironment) =
    member _.ConfigureServices(services: IServiceCollection) =
        services
            .AddGiraffe() |> ignore

    member _.ConfigureLogging(builder: ILoggingBuilder) =
        builder
            .AddConsole()
            .AddDebug() |> ignore

    member _.Configure(app: IApplicationBuilder) =
        (match env.IsDevelopment() with
         | true -> app.UseDeveloperExceptionPage()
         | false ->
             app
                 .UseGiraffeErrorHandler(errorHandler)
                 .UseHttpsRedirection())
            .UseGiraffe(webApp)

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)
    let startup = Startup(builder.Environment)
    startup.ConfigureServices(builder.Services)
    let app = builder.Build()
    startup.Configure(app)
    app.Run()
    0
