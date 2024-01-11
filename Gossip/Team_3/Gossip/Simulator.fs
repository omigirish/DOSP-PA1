module Simulator
open Akka.FSharp
open System
open Util

type SimulatorMessage =
    | TerminateGossip of String * int
    | TerminatePushSum of int * float * String * String * int

type Result = { Algorithm:String; Topology: String; NumberOfNodesConverged: int; TimeElapsed: float; }

let Simulator initCount numNodes (filepath: string) (stopWatch: Diagnostics.Stopwatch) (mailbox: Actor<SimulatorMessage>) =
    let rec loop count (dataframeList: Result list) =
        actor {
            let! message = mailbox.Receive()

            match message with
            | TerminateGossip (topology:String,totalNodes:int) ->

                printfn "Nodes converged: %d" (count + 1)
                let printRecord: Result = { Algorithm="Gossip"; Topology=topology; NumberOfNodesConverged = count + 1; TimeElapsed = (float stopWatch.ElapsedMilliseconds)/(float 1000); }
                
                if (count + 1 = numNodes) then
                    stopWatch.Stop()
                    printfn "Gossip Algorithm convergence time %f s" (float stopWatch.ElapsedMilliseconds/float 1000)
                    
                    if numNodes=totalNodes then
                        appendDataToCsv printRecord.Algorithm printRecord.Topology printRecord.NumberOfNodesConverged printRecord.TimeElapsed
                    
                    mailbox.Context.System.Terminate() |> ignore

                return! loop (count + 1) (List.append dataframeList [printRecord])

            | TerminatePushSum (nodeID, avg, algorithmName, topology:String, totalNodes:int) ->
            
                printfn "Node %d has been converged s/w=%f)" nodeID avg
                let printRecord: Result = { Algorithm="Push Sum"; Topology=topology; NumberOfNodesConverged = count + 1; TimeElapsed = (float stopWatch.ElapsedMilliseconds)/(float 1000) }
                
                if (count + 1 = numNodes) then
                    stopWatch.Stop()
                    printfn "Push Sum Algorithm convergence time %f s" (float stopWatch.ElapsedMilliseconds/ float 1000)
                    
                    if numNodes=totalNodes then
                        appendDataToCsv printRecord.Algorithm printRecord.Topology printRecord.NumberOfNodesConverged (printRecord.TimeElapsed)

                    mailbox.Context.System.Terminate() |> ignore

                return! loop (count + 1) (List.append dataframeList [printRecord])

                }

    loop initCount []

