open System
open Akka.FSharp
open Topology
open Util
open Simulator
open GossipNode
open PushSumNode

[<EntryPoint>]
let main argv =
    if Array.length argv = 3 then
        let system = System.create "GossipSystem" (Configuration.load())

        // Each actor keeps track of rumors and how many times it has heard the rumor. It stops transmitting once it has heard the rumor maxCount times 
        let maxCount = 10

        // Read Topology from Command Line Arguments
        let topology = argv.[1]

        //For 2D-based topologies you round up until you get a square
        let numNodes = roundNodes (int argv.[0]) topology

        // Read the Algorithm name i.e Gossip or Push Sum
        let algorithm = argv.[2]
        printfn "Simulating %s Algorithm for %d nodes....." algorithm numNodes

        // File Path to save Results
        let filepath =  "results.csv"

        // Create the specified topology
        let topologyMap = CreateTopology numNodes topology
        if topologyMap=Map.empty then
            printfn "Invalid Topology, Possible values are full, 2D, line, imp3D"
        else
            // Initialize Timer
            let stopWatch = Diagnostics.Stopwatch()

            // Spawn the Simulator actor
            let counterRef = spawn system "counter" (Simulator 0 numNodes filepath stopWatch)

            // Run the simulation using the user specified algorithm
            match algorithm with
            | "gossip" ->
                // Gossip Algorithm
                // Create specified numNodes and randomly pick 1 to start the algorithm
                let workerRef =
                    [ 1 .. numNodes ]
                    |> List.map (fun nodeID ->
                        let name = "worker" + string nodeID
                        spawn system name (gossip maxCount topologyMap nodeID counterRef topology numNodes))
                    |> pickRandom
                
                // Start the timer
                stopWatch.Start()

                // Send the rumour message
                workerRef <! "heardRumor"

            | "pushsum" ->
                // Push Sum Algorithm
                // Initialize all the actors
                let workerRef =
                    [ 1 .. numNodes ]
                    |> List.map (fun nodeID ->
                        let name = "worker" + string nodeID
                        (spawn system name (pushSum topologyMap nodeID counterRef topology numNodes)))
                
                // Start the timer
                stopWatch.Start()
                
                // Send message
                workerRef |> List.iter (fun item -> item <! Initialize)
                | _ -> printfn "Invalid Algorithm Name, Possible values are gossip and checksum"
            
            // Wait till all the actors are terminated
            system.WhenTerminated.Wait()
    else
        printfn "Incorrect CommandLine Arguments, expected dotnet run <numNodes> <topology> <algorithm>"
    0 // return an integer exit code
            // Each actor will have a flag to describe its active state