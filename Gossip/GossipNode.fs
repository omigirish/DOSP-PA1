module GossipNode
open Akka.FSharp
open System
open Simulator
open Util

let gossip maxCount (topologyMap: Map<_, _>) (nodeID: int) (counterRef: Akka.Actor.ICanTell) (topology:String) (numNodes:int) (mailbox: Actor<_>)  = 
    let rec loop (count: int) = actor {
        let! message = mailbox.Receive ()

        match message with
        | "heardRumor" ->
            // If the rumor is heard zero times
            if count = 0 then
                mailbox.Context.System.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromMilliseconds(25.0),
                    mailbox.Self,
                    "spreadRumor"
                )
                // Tell the counter that it has heard the rumor and start spreading it
                counterRef <! TerminateGossip (topology,numNodes)
                return! loop (count + 1)

            // Increment the heard rumor count by 1
            else
                return! loop (count + 1)

        | "spreadRumor" ->
            // Stop spreading the rumor if an actor has heard the rumor atleast 10 times
            if count >= maxCount then
                return! loop count
            
            // Select a random neighbor and send message "heardRumor"
            else
                let neighborID = getRandomNeighborID topologyMap nodeID
                let neighborPath = @"akka://GossipSystem/user/worker" + string neighborID
                let neighborRef = mailbox.Context.ActorSelection(neighborPath)
                neighborRef <! "heardRumor"
                // Start scheduler to wake up at next time step
                mailbox.Context.System.Scheduler.ScheduleTellOnce(
                    TimeSpan.FromMilliseconds(25.0),
                    mailbox.Self,
                    "spreadRumor"
                )
                return! loop count
        | _ ->
            printfn "Node %d has received unhandled message" nodeID
            return! loop count
    }
    loop 0