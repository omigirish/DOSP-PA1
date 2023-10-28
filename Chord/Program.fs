module Main
open Akka.FSharp
open System.Threading 
open ChordMessageTypes
open SupportFunctions
open Node
open Config
open Simulator

[<EntryPoint>]
let main argv =
    // Check if there are exactly two command-line arguments
    if Array.length argv = 2 then
        numNodes <- int argv.[0] 
        numRequests <- int argv.[1] 
        printfn "Simulating Chord Protocol for %d nodesList, where each node sends %d requests....." numNodes numRequests
        
        let mutable nodeIdList = []
        let mutable nodesList = []

        for _ in 1..numNodes do

            // Generate a Random m bit node Identifier
            let mutable nodeId = rand.Next(pown 2 m)

            // If randomly generated Node ID is already assigned to a Node, try a new identifier 
            while List.contains nodeId nodeIdList do
                nodeId <- rand.Next(pown 2 m)

            // Create a New Node Actor to simulate every Node
            let node = spawn system ("Node:" + (string nodeId)) <| Node nodeId

            // Add Node to nodesList
            nodesList <- nodesList @ [node]

            if nodeIdList.Length=0 then
                printfn "Initializing New Chord Ring:  Create()"
                node <! {RandomSearchNodeId=(-1); FirstOrNot=true}
                nodeIdList <- nodeIdList @ [nodeId]
                system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForStabilize,node,1)
                system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForFixfinger,node,2)
                Thread.Sleep(25)
            else
                let rnd = rand.Next(nodeIdList.Length)
                let rndNodeIndex = nodeIdList.[rnd]
                node <! {RandomSearchNodeId=rndNodeIndex; FirstOrNot=false}

                // Add Node to nodesList
                nodeIdList <- nodeIdList @ [nodeId]
                system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForStabilize,node,1)
                system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForFixfinger,node,2)
                Thread.Sleep(25)
        printfn $"all nodesList initialized"

        let calculateNode1 = spawn system "calculate" <| calculateNode

        Thread.Sleep(10000)
        printfn "start request data"
        for message in 1..numRequests do
            for nodeIndex in 1..numNodes do
                let data = ranStr 6
                nodesList.[nodeIndex-1] <! data
            Thread.Sleep(1000)

    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    
    0
