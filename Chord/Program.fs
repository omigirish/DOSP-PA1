module Chord

// Import Required Packages
open System
open Akka.Actor
open Akka.FSharp
open Hash
open ChordMessageTypes
open Vars

// Create an actor system named "Chord Simulation" with default configuration.
let ChordSimulatorSystem = System.create "ChordSimulation" (Configuration.defaultConfig())

type FingerEntry = { Id: int; NodeId: int }

// Define the Node Actor
let Node (id: int) (mailbox: Actor<_>) =
   
    // Node State
    let Id: int = id
    let mutable Successor: (int * IActorRef) = (-1, null)
    let mutable Predecessor: (int * IActorRef) = (-1, null)
    // let Stabilize =
    //     let (Sid, S) = Successor
    //     S <! SendPredecessor
    //     // Handle the response
    //     let response =
    //         let responseTask = S <? Actor 
    //         responseTask

        // let X = response
        // let (Xid, Xref) = X
        // (Sid, S) = S
        // if Xid>= n && Xid<=Sid then
        //     Successor <- X
    // Initialize Finger Tables
    let mutable FingerTable: (int * IActorRef)[] = [||]
    for i in 0 .. m - 1 do
        let start = (id + int (pown 2 i)) % (pown 2 m)  // Calculate the start value
        FingerTable <- Array.append FingerTable [|(int start, null)|]  // Initialize the entry with null

    let rec loop() = actor {
        let! msg = mailbox.Receive()
        match msg with
        | Create ->
            printfn "Node Created with ID: %d" Id
            Predecessor <- (-1, null)
            Successor <- (id, mailbox.Self)

        | Join (oldNodeID, oldNode) ->
            printfn "Joining Node with ID: %d" Id
            Predecessor <- (-1, null)
            printfn "%d.findSucessor(%d)"  oldNodeID Id
            oldNode <! FindSuccessor (oldNodeID, oldNode)

        | Lookup key ->
            printfn "NodeID: %d KVC %d" Id KeyValuePairs.Count
        | AssignKey item ->
            KeyValuePairs.Add(item.Key, item.Value)  // Insert the key-value pair

        | FindSuccessor (id: int, requestingNode: IActorRef) ->
            let (Sid, _) = Successor
            printfn "Successor ID: %d Node Id:%d" Sid Id
            if Id=Sid then
                printfn("Only one node in Chord")
                requestingNode <! UpdateSuccessor Successor

            else if id > Id && id < Sid then
                printfn "Belongs"
                requestingNode <! UpdateSuccessor Successor
            else
                printfn "Nope doesnt"
                // Search the local table for the highest predecessor of id
                for i in [m-1 .. -1 .. 1] do
                    printf "%d" i
                    let (start: int, S) = FingerTable.[i]
                    if start > Id && start <= id then
                        S <! FindSuccessor (id,requestingNode)
                mailbox.Self <! FindSuccessor (Id, mailbox.Self)

            requestingNode <! UpdateSuccessor (Id, mailbox.Self)

        | UpdateSuccessor S-> 
            Successor <- S

        | Notify ndash-> 
            let (NdId, Ndref) = ndash
            let (SId, S) = Successor
            if Predecessor = (-1,null) || (NdId >= Id && NdId <= SId ) then
                Predecessor <- ndash
        | SendPredecessor ->
            mailbox.Sender() <! Predecessor

        return! loop()
    }
    loop()

// Create a 'node' actor using the spawn function.
let createNode (id: int) =
    spawn ChordSimulatorSystem ("Node" + id.ToString()) (Node id)

[<EntryPoint>]
let main argv =
    // Check if there are exactly two command-line arguments
    if Array.length argv = 2 then
        let numNodes = argv.[0] |> int
        let numRequests = argv.[1] |> int
        printfn "Simulating Chord Protocol for %d Nodes, where each node sends %d requests....." numNodes numRequests

        // Create the First Node
        let firstId = getRandomId()
        printfn "%d" firstId
        let firstNode = createNode (firstId)
        firstNode <! Create
        nodes <- Array.append nodes [|(firstId, firstNode)|]
        // Read key-value pairs from the CSV file
        let keyValuePairs = readCsvFile "data.csv"
        for kvp in keyValuePairs do
            let keyHash: int64 = getHash (int kvp.Key)
            firstNode <! AssignKey { Key = keyHash; Value = kvp.Value }

        // Join the other nodes
        for i in 1 .. numNodes-1 do
            let id = getRandomId()
            let newNode = createNode id
            nodes <- Array.append nodes [| (id, newNode) |]
            newNode <! Join (firstId, firstNode)

    
        // let smallestNodeId =
        //     nodes
        //     |> Array.minBy (fun (nodeId, _) -> nodeId)

        // // Iterate over keyValuePairs and assign them to nodes
        // for kvp in keyValuePairs do
        //     let keyHash = getHash (int kvp.Key)
        //     let assignedNode =
        //         nodes
        //         |> Array.filter (fun (nodeId, _) -> nodeId >= int keyHash)
        //         |> getMinByOrDefault (fun (nodeId, _) -> nodeId)
        //     // Extract the actor reference from the tuple
        //     match assignedNode with
        //     | Some (nodeId, _) ->
        //         // Extract the actor reference from the tuple
        //         let (_, nodeRef) = assignedNode.Value
        //         // Send the message to the actor reference
        //         nodeRef <! AssignKey kvp

        //     | None ->
        //         let (_, nodeRef) = smallestNodeId
        //         // Send the message to the actor reference
        //         nodeRef <! AssignKey kvp


        // // Introduce a delay in milliseconds (e.g., 5000ms or 5 seconds)
        // let delayMilliseconds = 5000
        // Thread.Sleep(delayMilliseconds)

        // // Iterate through each node and print the assigned key-value pairs
        // for (id, nodeRef) in nodes do
        //     // Iterate through the dictionary and print key-value pairs
        //     nodeRef <! Lookup "10"
        //     let delayMilliseconds = 1500
        //     Thread.Sleep(delayMilliseconds)

        // Read a line from the console (for interaction) and ignore the result.
        System.Console.ReadLine() |> ignore
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    0
