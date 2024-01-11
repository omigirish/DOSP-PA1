module Main
open Akka.FSharp
open System.Threading 
open ChordMessageTypes
open SupportFunctions
open Node
open Config
open Simulator

let mutable nodeIdList = []
let mutable nodesList = []

let Create(node,nodeId) =
    printfn "Creating First Node with Id %d..." nodeId
    node <! CreateFingerTable { RandomSearchNodeId=(-1); FirstOrNot=true }

    // Add Node to nodesList
    nodeIdList <- nodeIdList @ [nodeId]
    // Periodically Call Stabilize()
    system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForStabilize,node,Request 1)
    // Periodically Call FixFingers()
    system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForFixfinger,node,Request 2)
    //Pause for 25 ms
    Thread.Sleep(25)

let Join(node,nodeId) =
    printfn "Adding Node with Id %d..." nodeId
    let rnd = rand.Next(nodeIdList.Length)
    let rndNodeIndex = nodeIdList.[rnd]
    node <! CreateFingerTable {RandomSearchNodeId=rndNodeIndex; FirstOrNot=false}

    // Add Node to nodesList
    nodeIdList <- nodeIdList @ [nodeId]
    // Periodically Call Stabilize()
    system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForStabilize,node,Request 1)
    // Periodically Call FixFingers()
    system.Scheduler.ScheduleTellRepeatedly(startTime,delayTimeForFixfinger,node,Request 2)
    //Pause for 25 ms
    Thread.Sleep(25)

let CreateChord() =

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
                    Create(node,nodeId)
                else
                    Join(node,nodeId)

            printfn "\n----Chord Ring Created with %d Nodes----\n" numNodes



[<EntryPoint>]
let main argv =
    // Check if there are exactly two command-Line arguments
    if Array.length argv = 2 then
        numNodes <- int argv.[0] 
        numRequests <- int argv.[1] 
        printfn "Simulating Chord Protocol for %d nodes, where each node sends %d requests....." numNodes numRequests
        
        //Create the Chord Ring
        CreateChord()
        printfn "Simulation Start -> Each Node Sends %d Lookup Requests" numRequests
        spawn system "Simulate" <| SimulateNode |> ignore
        // Pause for 1s
        Thread.Sleep(10000)

        for _ in 1..numRequests do
            for nodeIndex in 1..numNodes do
                let data = ranStr 6
                nodesList.[nodeIndex-1] <! RequestData data
            Thread.Sleep(1000)

    else
        printfn "Please provide exactly two command-Line arguments: 'dotnet run numNodes numRequests'"
    
    0
