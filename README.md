# Handling dependencies in a F# Giraffe application

## Isn't handling dependencies a solved problem?

The main programming paradigm i C# is dependency injection. So one of the questions I frequently encounter
when discussing F# is "How do you do dependency injection in F#?"

The easy answer to that is "The same way as you would do in C#". You create MVC and Minimal API apps using F# 
the same way as you would in C#. Register your services during startup, and create a class for your controller
with the required dependencies as constructor parameters. Or create a minimal API definition using the 
`[<FromServices>]`-attribute like in C#.  

But that is just a part of the answer. [Giraffe](https://giraffe.wiki/) is my go-to framework for building
web apps using F#, and it offers no definitive solution out of the box for handling dependencies.

## 3 ways of doing dependency handling using Giraffe

*  Baseline: [No dependency injection](../NoDi/NoDi.fs)
*  [Getting dependencies from HttpContext](../FromCtx/FromCtx.fs)
*  [Injecting dependencies to WebApp](../ToWebApp/ToWebApp.fs)
*  [Injecting functions as record to WebApp](../DiRecord/DiRecord.fs)

