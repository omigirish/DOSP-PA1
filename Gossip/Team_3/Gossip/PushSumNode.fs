module PushSumNode
open Akka.FSharp
open System
open Simulator
open Util

type PushSumMessage =
    | Initialize
    | Message of float * float
    | Round

let pushSum (topologyMap: Map<_, _>) (nodeID: int) (counterRef: Akka.Actor.ICanTell) (topology:String) (numNodes:int) (mailbox: Actor<_>) = 
    
    let rec loop (sNode: float) (wNode: float) (sSum: float) (wSum: float) (count: int) (isTransmitting: bool) = actor {
        
        if isTransmitting then
            
            let! message = mailbox.Receive ()
            
            match message with
            | Initialize ->
                mailbox.Self <! Message (float nodeID, 1.0)
                mailbox.Context.System.Scheduler.ScheduleTellRepeatedly (
                    TimeSpan.FromMilliseconds(0.0),
                    TimeSpan.FromMilliseconds(25.0),
                    mailbox.Self,
                    Round
                )
                return! loop (float nodeID) 1.0 0.0 0.0 0 isTransmitting

            | Message (s, w) ->
                return! loop sNode wNode (sSum + s) (wSum + w) count isTransmitting

            | Round ->
                // Select a random neighbor
                let neighborID = getRandomNeighborID topologyMap nodeID
                let neighborPath = @"akka://GossipSystem/user/worker" + string neighborID
                let neighborRef = mailbox.Context.ActorSelection(neighborPath)
                
                // Send (s/2, w/2) to itself
                mailbox.Self <! Message (sSum / 2.0, wSum / 2.0)

                // Send (s/2, w/2) to randomly selected neighbor
                neighborRef <! Message (sSum / 2.0, wSum / 2.0)

                // Check convergence : s/w should not change more than 1.0e-10 for 3 consecutive rounds
                if(abs ((sSum / wSum) - (sNode / wNode)) < 1.0e-10) then
                    let newCount = count + 1
                    if newCount = 10 then
                        counterRef <! TerminatePushSum (nodeID, sSum / wSum, "Push Sum", topology, numNodes)
                        return! loop sSum wSum 0.0 0.0 newCount false
                    else
                        return! loop (sSum / 2.0) (wSum / 2.0) 0.0 0.0 newCount isTransmitting 
                else
                    return! loop (sSum / 2.0) (wSum / 2.0) 0.0 0.0 0 isTransmitting
    }
    loop (float nodeID) 1.0 0.0 0.0 0 true

