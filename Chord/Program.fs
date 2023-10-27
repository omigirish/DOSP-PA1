module ChordSimulator
open Akka
open Akka.FSharp
open FSharp.Collections
open System.Threading
open ChordMessageTypes
open Simulator
open Vars

// Start of the program
[<EntryPoint>]
let main argv =
    // Check if there are exactly two command-line arguments
    if Array.length argv = 2 then
        let numNodes = argv.[0] |> int
        let numRequests = argv.[1] |> int
        printfn "Simulating Chord Protocol for %d Nodes, where each node sends %d requests....." numNodes numRequests
        spawn  Chord "ChordSimulator" (simulator numNodes numRequests) <! CreateChord
        Thread.Sleep(18000)
        

        // Read a line from the console (for interaction) and ignore the result.
        System.Console.ReadLine() |> ignore
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    
    0 
