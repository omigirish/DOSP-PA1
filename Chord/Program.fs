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
let mutable m = 20
let mutable nodes: (int * IActorRef)[] = [||]
let mutable flag = true
let mutable sum = 0
let mutable average = 0.0

// Define message types that the actors will send and receive.
// Messages sent by the nodes in the Chord
type NodeMessage =
    | Create  // Create a Node
    | Join of (int * IActorRef)  // Join a node into an existing Chord Ring
    | Lookup of string  // Lookup a given key
    | FindSuccessor of int
    | AddInFingerTable  // Add a new entry to the finger table
    | AssignKey of KeyValue  // Assign Key-Value Pair to Node
    | Stabilize
    | Notify
    | FixFingers
    | Done

type ResponseMessages =
    | SendSuccessor of (int * IActorRef)

type FingerEntry = { Id: int; NodeId: int }
type KeyValue = { Key: string; Value: string }

// Define the Node Actor
let Node (id: int) (mailbox: Actor<_>) =
    // Node State
    let mutable Id: int = -1
    let mutable Successor: (int * IActorRef) = (-1, null)
    let mutable Predecessor: (int * IActorRef) = (-1, null)
    // Initialize Finger Tables
    let mutable FingerTable: (int * IActorRef)[] = [||]
    for i in 0 .. m - 1 do
        printf "%d\n" i
        let start = (id + int (pown 2 i)) % (pown 2 m)  // Calculate the start value
        FingerTable <- Array.append FingerTable [|(int start, null)|]  // Initialize the entry with null

    let mutable KeyValuePairs = Dictionary<string, string>()

    let rec loop() = actor {
        let! msg = mailbox.Receive()
        match msg with
        | Create ->
            Id <- id
            Predecessor <- (-1, null)
            Successor <- (id, mailbox.Self)

        | Join (cnID, cnode) ->
            Predecessor <- (-1, null)
            let responseTask = cnode <? FindSuccessor cnID
            async {
                let! response = responseTask
                Successor <- response
            } |> ignore

        | Lookup key ->
            printfn "NodeID: %d KVC %d" Id KeyValuePairs.Count
        | AssignKey item ->
            KeyValuePairs.Add(item.Key, item.Value)  // Insert the key-value pair
        | AddInFingerTable -> printf "AddInFingerTable"

        | FindSuccessor id ->
            let (Sid, _) = Successor
            if id >= Id && id <= Sid then
                printfn "Belongs"
                mailbox.Sender() <! SendSuccessor Successor
            else
                printfn "Nope doesnt"
                // Search the local table for the highest predecessor of id
                for i in [m-1 .. -1 .. 1] do
                    printf "%d" i
                    let (start: int, S) = FingerTable.[i]
                    if start > Id && start <= id then
                        S <! FindSuccessor id
                mailbox.Self <! FindSuccessor Id

            mailbox.Sender() <! SendSuccessor (Id, mailbox.Self)

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

        // Create the First Node
        let firstId = getRandomId()
        let firstNode = createNode (firstId)
        firstNode <! Create
        nodes <- Array.append nodes [|(firstId, firstNode)|]

        // Join the other nodes
        for i in 2 .. numNodes do
            let id = getRandomId()
            let newNode = createNode (id)
            nodes <- Array.append nodes [| (id, newNode) |]
            newNode <! Join (firstId, firstNode)

        // Read key-value pairs from the CSV file
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

        printf "Data assignments Done"

        // Introduce a delay in milliseconds (e.g., 5000ms or 5 seconds)
        let delayMilliseconds = 5000
        Thread.Sleep(delayMilliseconds)

        // Iterate through each node and print the assigned key-value pairs
        for (id, nodeRef) in nodes do
            // Iterate through the dictionary and print key-value pairs
            nodeRef <! Lookup "10"
            let delayMilliseconds = 1500
            Thread.Sleep(delayMilliseconds)

        // Read a line from the console (for interaction) and ignore the result.
        System.Console.ReadLine() |> ignore
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    0
