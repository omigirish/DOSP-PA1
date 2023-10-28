module Simulator
open Akka.FSharp
open Config
open System

let SimulateNode (mailbox: Actor<_>) = 
    let mutable hopCount = 0
    let mutable finisheNode = 0
    let rec loop() = actor{
        let! message = mailbox.Receive()
        match box message with
        | :? int as hop ->
            hopCount <- hopCount+hop
            finisheNode <- finisheNode+1
            if finisheNode=numNodes then
                let averageHop = (float hopCount) / (float (numNodes*numRequests))
                printfn $"The average No. of hops per node per message is {averageHop} hops"
                Environment.Exit 1  
        | _ ->
            printfn "Unkown message"
        return! loop()
    }
    loop()