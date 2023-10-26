module Chord
open System
open Akka.Actor
open Akka.FSharp

// Create an actor system named "system" with default configuration.
let system = System.create "system" (Configuration.defaultConfig())

// Define message types that the actors will send and receive.
type GreeterMsg =
    | Hello of string
    | Goodbye of string


let chord() =
    // Create a 'greeter' actor using the spawn function.
    let greeter = spawn system "greeter" <| fun mailbox ->
        let rec loop() = actor {
            let! msg = mailbox.Receive()
            match msg with
            | Hello name -> printf "Hello, %s!\n" name
            | Goodbye name -> printf "Goodbye, %s!\n" name

            return! loop()
        }
        loop()

    // Send Hello and Goodbye messages to the 'greeter' actor.
    greeter <! Hello "Joe"       // or greeter.Tell(Hello "Joe") 
    greeter <! Goodbye "Joe"     // or greeter.Tell(Goodbye "Joe") 

    // Read a line from the console (for interaction) and ignore the result.
    System.Console.ReadLine() |> ignore

[<EntryPoint>]
let main argv =
    // Check if there are exactly two command-line arguments
    if Array.length argv = 2 then
        let numNodes = argv.[0] |> int
        let numRequests = argv.[1] |> int
        printfn "Simulating Chord Protocol for %d Nodes, where each node sends %d requests....." numNodes numRequests
        chord()
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    0
