module Node
open Akka.FSharp
open Akka.Actor
open ChordMessageTypes
open Vars
open System.Threading

let Node (id: int) (mailbox:Actor<_>) =
    // Node State
    let mutable NodeId: int = id;
    let mutable Successor: (int * IActorRef)=(-1, null);
    let mutable Predecessor: (int * IActorRef)=(-1, null);
    let mutable SimulatorReference: IActorRef = null;
    let mutable FingerTable: (int * IActorRef) []=[||];
    
    let rec loop state = actor {
        
        let! message = mailbox.Receive()
        
        match message with
        | Create (id,master)-> 
            SimulatorReference <- master
            NodeId <- id
        | Join (suci, sucR, prei, preR)->
            Predecessor <- (prei, preR)
            Successor <- (suci, sucR)
        | Lookup (key, hops, requestor) ->
            let mutable hopCount = hops
            let mutable ftfound = false

            if flag && key = NodeId  then 
                flag <- false
                SimulatorReference <! FoundKey((snd Successor), key, hopCount, requestor)
                mailbox.Self <! Done
            elif (key <= (fst Successor)) && (key > NodeId) then
                flag <- false
                SimulatorReference <! FoundKey((snd Successor), key, hopCount, requestor)
                mailbox.Self <! Done
            else
                if  flag && NodeId > (fst Successor) then 
                    hopCount <- hopCount + 1 
                    flag <- false
                    SimulatorReference <! FoundKey((snd (Array.get Nodes (0))), key, hopCount, requestor)
                    mailbox.Self <! Done
                else 
                    let mutable i = m - 1
                    while flag &&  i > 0 do
                        let fingerValue = (fst (Array.get FingerTable i))
                        if  flag && fingerValue < NodeId then
                            flag <- false
                            hopCount <- hopCount+1
                            SimulatorReference <! FoundKey((snd (Array.get Nodes (0))), key, hopCount, requestor)
                        elif flag && (fingerValue <= key && fingerValue > NodeId)  then
                            ftfound <- true
                            hopCount <- hopCount + 1
                            (snd (Array.get FingerTable i)) <! Lookup(key, hopCount, requestor)
                        i <- 0
                        Thread.Sleep(400)
                        hopCount <- hopCount + 1
                        (snd (Array.get FingerTable (m - 1))) <! Lookup(key, hopCount, requestor)
            mailbox.Self <! Done
        | AddInFingerTable(id) ->
            for i in 0..m - 1 do 
                let mutable ft = false
                for j in 0..Nodes.Length - 2 do 
                    let ele = (fst (Array.get Nodes j))
                    let scnd = (fst (Array.get Nodes (j+1)))
                    if ((id + (pown 2 i)) > ele) && ((id + (pown 2 i)) <= scnd) then
                        FingerTable <- Array.append FingerTable [|(scnd , snd (Array.get Nodes (j+1)))|]
                        ft <- true
                if not ft  then
                    //printfn "%A points to %A" (id+(pown 2 i)) 0 
                    FingerTable <- Array.append FingerTable [|(fst (Array.get Nodes (0)), snd (Array.get Nodes (0)))|]
        | Done -> printf "" |> ignore

        return! loop()
    }

    loop ()
