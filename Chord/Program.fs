module Chord
// Import Required Packages
open System
open Akka.Actor
open Akka.FSharp
open Hash
open System.Collections.Generic
open System.Threading

// Create an actor system named "Chord Simulation" with default configuration.
let ChordSimulatorSystem = System.create "ChordSimulation" (Configuration.defaultConfig())

// Define Global Variables for the System
let mutable m = 0
let mutable nodes: (int * IActorRef)[] = [||]
let mutable flag = true
let mutable sum = 0
let mutable average = 0.0

// Define message types that the actors will send and receive.
// Messages sent by the nodes in the Chord
type NodeMessage =
    | Create// Create a Node
    | Join // Join a node into an exisiting Chord Ring
    | Lookup of string // Lookup a given key
    | AddInFingerTable// Add new entry into the finger table
    | AssignKey of KeyValue //Assign Key Value Pair to Node
    | Done 

type FingerEntry = { Id: int; NodeId: int }
type KeyValue = { Key: string; Value: string }


// Define the Node Actor
let Node (id:int) (mailbox:Actor<_>) = 
    // Peer State
    let mutable Id: int = -1;
    let mutable Successor: (int * IActorRef)=(-1, null);
    let mutable Predecessor: (int * IActorRef)=(-1, null);
    let mutable FingerTable: (int * IActorRef) []=[||];
    let mutable KeyValuePairs = Dictionary<string, string>()

    let rec loop() = actor {
        let! msg = mailbox.Receive()
        match msg with
        | Create -> 
            printf "Node Created with identifier: %d\n" id
            Id <- id
        | Join -> printf "Joining, %d\n" id
        | Lookup key -> 
            printfn "NodeID: %d KVC %d" Id KeyValuePairs.Count        
        | AssignKey item -> 
            KeyValuePairs.Add(item.Key, item.Value) // Insert the key-value pair
        | AddInFingerTable -> printf "AddInFingerTable" 
        | Done -> printf "Done"

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
        
        // Create numNodes number of nodes
        for i in 1..numNodes do
            let id = getRandomId()
            nodes <- Array.append nodes [| (id, createNode id) |]

        for (id, node: IActorRef) in nodes do
            node <! Create 
        
        // Distribute Hash Table among nodes

        // Read from the CSV file
        let keyValuePairs = readCsvFile "data.csv"

        let smallestNodeId =
            nodes
            |> Array.minBy (fun (nodeId, _) -> nodeId)

        // Iterate over keyValuePairs and assign them to nodes
        for kvp in keyValuePairs do
            let keyHash = getHash (int kvp.Key)
            let assignedNode =
                nodes
                |> Array.filter (fun (nodeId, _) -> nodeId >= int keyHash)
                |> getMinByOrDefault (fun (nodeId, _) -> nodeId)
             // Extract the actor reference from the tuple
            match assignedNode with
            | Some (nodeId, _) ->
                // Extract the actor reference from the tuple
                let (_, nodeRef) = assignedNode.Value
                // Send the message to the actor reference
                nodeRef <! AssignKey kvp

            | None ->
                let (_, nodeRef) = smallestNodeId
                // Send the message to the actor reference
                nodeRef <! AssignKey kvp
    
    
        // Read a line from the console (for interaction) and ignore the result.
        System.Console.ReadLine() |> ignore     
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    0
