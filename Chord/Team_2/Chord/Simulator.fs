module Simulator
open Akka.FSharp
open Config
open ChordMessageTypes
open System.Collections.Generic
open SupportFunctions
open System.IO

let SimulateNode (mailbox: Actor<_>) = 
    let mutable hopCount = 0
    let mutable completedNodes = 0
    let nodeIdHopsDictionary = new Dictionary<int, int>()

    let rec loop() = actor{
        let! message = mailbox.Receive()
        match message with
        | UpdateHops nodehops ->
            let (N,H) = nodehops
            nodeIdHopsDictionary.Add(N, H)
            hopCount <- hopCount+H
            completedNodes <- completedNodes+1
            if completedNodes=numNodes then
                for kvp in nodeIdHopsDictionary do
                    printfn "Node %d: Average Hops = %f" kvp.Key ( (float kvp.Value)/ (float numRequests) ) 
                let avgHop = (float hopCount) / (float (numNodes*numRequests))
                printfn "The average No. of hops per node per message is %f hops" avgHop
                    
                appendDataToCsv numNodes numRequests avgHop


        return! loop()
    }
    loop()