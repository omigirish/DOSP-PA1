module Simulator
open Akka.FSharp
open System
open ChordMessageTypes
open Vars
open Hash
open Node

// Define Actor Simulator
let simulator (numNodes: int) (numRequests: int) (mailbox: Actor<_>) =
    let rec loop state = actor {
        let! message = mailbox.Receive()
        let r = Random()

        match message with
        | CreateChord ->
            // m <- (Math.Log2(float numNodes) |> int)
            for i in 1..numNodes do
                let id = getRandomId()
                let node  = spawn Chord ("Node" + (generateHash id)) (Node(id))
                Nodes <- Array.append Nodes [|(id, node)|]
                node <! Create (id, mailbox.Self)
            mailbox.Self <! JoinChord
        | JoinChord ->
            for i in 0..numNodes - 1 do 
                let mutable successori = i + 1
                let mutable predecessori = i - 1
                let node = Array.get Nodes i
                if i = (numNodes - 1) then 
                    successori <- 0
                if i = 0 then 
                    predecessori <- (numNodes - 1)
                let sucR = Array.get Nodes (successori)
                let preR = Array.get Nodes (predecessori)
                snd node <! Join (fst sucR, snd sucR, fst preR, snd preR)
                Threading.Thread.Sleep(150)
            mailbox.Self <! CreateFingerTable
        | CreateFingerTable ->
            for i in 0..numNodes - 1 do 
                let x = Array.get Nodes (i)
                snd x <! AddInFingerTable(fst x)
                Threading.Thread.Sleep(150)
            mailbox.Self <! Lookups 
        | Lookups ->
            let mutable key = 0
            for i in 0..numNodes - 1 do
                key <- key + (fst (Array.get Nodes i))
            key <- key / Nodes.Length
            key <- key |> int
            for i in 0..numNodes - 1 do
                for j in 1..(numRequests) do
                    (snd (Array.get Nodes i)) <!  Lookup (fst(Array.get Nodes (numNodes - 1)) - 1, 0, (snd (Array.get Nodes i)))
                    flag <- true
                    Threading.Thread.Sleep(400)
        | FoundKey (ref, key, hopCount, requestor) ->
            sum <- sum + hopCount
            average <- (sum |> float) / (numNodes  * (numRequests) |> float)  
            printfn "Input %A:Set key%A on node %A with number of hops = %A , Average:%A" requestor key ref hopCount average

        return! loop()
    }

    loop ()