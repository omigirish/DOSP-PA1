module Simulator
open Akka.FSharp
open Config
open System

let calculateNode (mailbox: Actor<_>) = 
    let mutable totalHop = 0
    let mutable finisheNode = 0
    let rec loop() = actor{
        let! message = mailbox.Receive()
        match box message with
        | :? int as hop ->
            totalHop <- totalHop+hop
            finisheNode <- finisheNode+1
            if finisheNode=numNodes then
                let averageHop = (float totalHop) / (float (numNodes*numRequests))
                printfn $"The average hop to deliver a message is {averageHop} hops"
                Environment.Exit 1  
        | _ ->
            printfn "Unkown message"
        return! loop()
    }
    loop()