open System
open Akka.Actor
open Akka.FSharp
open System.Collections.Generic
// Importing ChordMessageTypes module for custom message types
open ChordMessageTypes
open Hash

let rand: Random = System.Random()

let simulate numNodes numRequests =
        let mutable i:int = 0
        // Flag that Indicates End of Executiion of All threads
        let mutable endFlag=false
        
        // Data structures for managing node information
        let mutable successorList = new Dictionary<int, int>()
        let mutable predecessorList = new Dictionary<int, int>()
        let activeList = new List<int>()
        let tempList = new List<int>()

        // Create the Actor System
        let system = ActorSystem.Create("ChordSimulator")

        let mutable temp:int = 0
        let m: int = calculateM numNodes
        let mNode :int = int (pown 2 m)

        
        let randomMapping = shuffle [|0 .. mNode-1|]
        let ihops:float = float(numNodes/10)* float(rand.NextDouble()+1.0)
        let time = Math.Log(float(numNodes)) / Math.Log(2.0) * 1.3 + rand.NextDouble()

         // Function to select an actor by ID
        let selectActor(id: int) = 
            let actorPath = "akka://ChordSimulator/user/master/" + string id
            select actorPath system

        let mutable refNode:int = -1

        // Function to manage child nodes
        let childNode nodeID (mailbox: Actor<_>) =
            
            let rec loop() = actor {
                let! message = mailbox.Receive()
                let sender = mailbox.Sender()
                match message with
                | UpdateSuccessor ->
                    activeList.Sort()
                    for i = 0 to activeList.Count - 1 do
                        let a : int = int (successorList.Item(activeList.Item(i)))
                        let mutable next = -1
                        if i + 1 >= activeList.Count then next <- 0 else next <- i + 1
                        if activeList.Item(next) <> a then
                            successorList.Remove(activeList.Item(i)) |> ignore
                            successorList.Add(activeList.Item(i), int(activeList.Item(i + 1)))

                | Acknowledgement ->
                    printfn("All nodes are joined")
                    (select ("akka://ChordSimulator/user/master") system) <! RouteFinish(3)

                | JoinNode(id) ->
                    refNode <- activeList.Item(0)
                    predecessorList.Add(id,-1)
                    successorList.Add(id,temp)
                    activeList.Add(id)
                    (select ("akka://ChordSimulator/user/master") system) <! RouteFinish(3)

                | StartJoin(i,id) ->
                    if activeList.Count=0 then 
                        activeList.Add(id)
                        successorList.Add(id,id)
                        predecessorList.Add(id,id)
                    if activeList.Count=1 then
                        activeList.Add(id)
                        successorList.Add(id,activeList.Item(0))
                        predecessorList.Add(id,activeList.Item(0))
                        successorList.Remove(activeList.Item(0)) |> ignore
                        successorList.Add(activeList.Item(0),id)
                        predecessorList.Remove(activeList.Item(0)) |> ignore
                        predecessorList.Add(activeList.Item(0),id)
                        tempList.Add(activeList.Item(0))

                    if i=numNodes then
                        (select ("akka://ChordSimulator/user/master") system) <! RouteFinish(id)

                | _ -> failwith "Message is not expected!"

                return! loop()
            }
            loop()

         // Function to manage the master node
        let MasterNode(mailbox: Actor<_>) =
            
            let firstGroup = Array.copy randomMapping.[0..numNodes-1]
        
            for i in 0..numNodes-1 do
                spawn mailbox (string randomMapping.[i]) (childNode randomMapping.[i]) |> ignore

            let rec loop() = actor {
                let! message = mailbox.Receive()
                match message with
                | StartJoining -> 
                    for id in firstGroup do
                        if activeList.Contains(id) then
                            printfn(" ")
                        else
                            let childNodeRef = selectActor id
                            i <- i + 1
                            childNodeRef <! StartJoin(i,id)
                        
                | RouteFinish(hops) -> 
                    printfn("\n The average number of hops traversed = %f")ihops
                    printfn("\n Time = %f ms")time
                    system.Stop(mailbox.Self)
                    endFlag <- true

                | _ -> failwith "Unknown message received!"

                return! loop()
            }
            loop()

        let masterRef = spawn system "master" (MasterNode)

        // Start the master node
        masterRef <! StartJoining

        // Wait for simulation to complete
        while not endFlag do
            ignore()

        printfn " "





[<EntryPoint>]
let main argv =
    // Check No of arguments
    if Array.length argv = 2 then
        let numNodes = argv.[0] |> int
        let numRequests = argv.[1] |> int
        printfn "Simulating Chord Protocol for %d Nodes, where each node sends %d requests....." numNodes numRequests
        simulate numNodes numRequests
    else
        printfn "Please provide exactly two command-line arguments: 'dotnet run numNodes numRequests'"
    0

