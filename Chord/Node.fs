module Node
open Akka.FSharp
open ChordMessageTypes
open Config
open System.Security.Cryptography
open ChordMessageTypes

let Node (nodeId:int) (mailbox: Actor<_>) = 
    let Id = nodeId
    let mutable currentFinger = 1
    let mutable FingerTable = Array.create m [||]
    let mutable predecessorId = -1
    let mutable successorId = nodeId
    let mutable hopCount = 0
    let mutable mssgCount = 0
    
    let rec loop() = actor{

        let! message = mailbox.Receive()

        match message with
        | RequestData requestData->
            let content = System.Text.Encoding.ASCII.GetBytes requestData
            let bytes = SHA1.Create().ComputeHash(content)
            let targetKey = (int bytes.[bytes.Length-1]) + (int bytes.[bytes.Length-2]*(pown 2 8))
            let request = {TargetKey=targetKey; TargetKeyIndex=(-1); RequesterNodeId=Id; Hop=0}
            mailbox.Self<! RequestInfo request

        | Request request->
            // Request 1 triggers Stabilize
            if request=1 then 
                let successorNode = system.ActorSelection("akka://ChordSimulation/user/"+("Node:" + (string successorId)))
                let stabilize = {RequesterNodeId=Id}
                successorNode <! Stabilize stabilize
            // Request 2 triggers FixFingers
            else 
                if currentFinger>m then
                    currentFinger <- 1
                let fixFingerRequest = {TargetFinger=FingerTable.[currentFinger-1].[0]; Next=currentFinger; RequesterNodeId=Id}
                mailbox.Self <! FixFinger fixFingerRequest
                currentFinger <- currentFinger+1

        | RequestInfo requestInfo ->
            let targetKey = requestInfo.TargetKey
            let requesterNodeId = requestInfo.RequesterNodeId
            let targetKeyIndex = requestInfo.TargetKeyIndex
            let mutable hop = requestInfo.Hop
   
            let requesterNode = system.ActorSelection("akka://ChordSimulation/user/"+( "Node:" + (string requesterNodeId)))
            if hop<>(-1) then
                hop <- hop+1
                
            if Id=successorId || targetKey=Id then 
                let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex;ResultNodeId=Id; Hop=hop}
                requesterNode <! ResultInfo resultInfo       

            elif (Id<successorId && targetKey>Id && targetKey<=successorId)||(Id>successorId && (targetKey>Id || targetKey<=successorId)) then
                let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex; ResultNodeId=successorId; Hop=hop}
                requesterNode <! ResultInfo resultInfo

            else 
                let mutable countdown = m-1
                let mutable breakLoop = false

                while countdown>=0 && not breakLoop do
                    let tempFingerNodeId = FingerTable.[countdown].[1]

                    if (Id<tempFingerNodeId && (targetKey>=tempFingerNodeId || Id>targetKey)) 
                       ||(Id>tempFingerNodeId && (targetKey>=tempFingerNodeId && Id>targetKey)) then
                        let tempFingerNode = system.ActorSelection("akka://ChordSimulation/user/"+( "Node:" + (string tempFingerNodeId)))
                        let forwardRequest = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex; RequesterNodeId=requesterNodeId; Hop=hop}
                        tempFingerNode <! RequestInfo forwardRequest
                        breakLoop <- true

                    elif Id=tempFingerNodeId then
                        let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex;ResultNodeId=tempFingerNodeId; Hop=hop}
                        requesterNode <! ResultInfo resultInfo
                    countdown <- countdown-1

        | ResultInfo resultInfo ->
            let targetKeyIndex = resultInfo.TargetKeyIndex
            let resultNodeId = resultInfo.ResultNodeId
            let hop = resultInfo.Hop 

            if hop<>(-1) then
                hopCount <- hopCount + hop
                mssgCount <- mssgCount + 1

                if mssgCount=numRequests then
                    let SimulateNode = system.ActorSelection("akka://ChordSimulation/user/Simulate")
                    SimulateNode <! UpdateHops (Id,hopCount)
                    // printfn "Node-%d, Avg hops%f" Id ((float hopCount)/ (float numRequests)) 
                
            elif targetKeyIndex<>(-1) then

                if targetKeyIndex=1 then
                    successorId <- resultNodeId
                FingerTable.[targetKeyIndex-1].[1] <- resultNodeId

        | FixFinger fixFinger ->
            let targetFinger = fixFinger.TargetFinger
            let Next = fixFinger.Next
            let requesterNodeId = fixFinger.RequesterNodeId

            let requesterNode = system.ActorSelection("akka://ChordSimulation/user/"+( "Node:" + (string requesterNodeId)))
            if Id=successorId || targetFinger=Id then 
                let resultInfo = {TargetFinger=targetFinger; Next=Next;ResultNodeId=Id}
                requesterNode <! FixFingerRes resultInfo

            elif (Id<successorId && targetFinger>Id && targetFinger<=successorId)
                ||(Id>successorId && (targetFinger>Id || targetFinger<=successorId)) then
                let resultInfo = {TargetFinger=targetFinger; Next=Next;ResultNodeId=successorId}
                requesterNode <! FixFingerRes resultInfo

            else 
                let mutable countdown = m-1
                let mutable breakLoop = false
                while countdown>=0 && not breakLoop do
                    let tempFingerNodeId = FingerTable.[countdown].[1]
                    if (Id<tempFingerNodeId && (targetFinger>=tempFingerNodeId || Id>targetFinger)) 
                       ||(Id>tempFingerNodeId && (targetFinger>=tempFingerNodeId && Id>targetFinger)) then
                        let tempFingerNode = system.ActorSelection("akka://ChordSimulation/user/"+ ( "Node:" + (string tempFingerNodeId)))
                        let forwardRequest = {TargetFinger=targetFinger; Next=Next; RequesterNodeId=requesterNodeId}
                        tempFingerNode <! FixFinger forwardRequest
                        breakLoop <- true
                    elif Id=tempFingerNodeId then
                        let resultInfo = {TargetFinger=targetFinger; Next=Next;ResultNodeId=tempFingerNodeId}
                        requesterNode <! FixFingerRes resultInfo
                    countdown <- countdown-1

        | FixFingerRes fixFingerRes ->
            let Next = fixFingerRes.Next
            let resultNodeId = fixFingerRes.ResultNodeId
            FingerTable.[Next-1].[1] <- resultNodeId

        | Stabilize stabilize ->
            let requesterNodeId = stabilize.RequesterNodeId
            let requesterNode = system.ActorSelection("akka://ChordSimulation/user/"+("Node:" + (string requesterNodeId)))
            if predecessorId=(-1) then
                predecessorId <- Id
            let stabilizeResponse = {PotentialSuccessor=predecessorId}
            requesterNode <! StabilizeResponse stabilizeResponse

        | StabilizeResponse stabilizeResponse ->
            let potentialSuccessor = stabilizeResponse.PotentialSuccessor
            if potentialSuccessor<>successorId then
                if Id=successorId then
                    successorId <- potentialSuccessor
                if (Id<successorId && potentialSuccessor>Id && potentialSuccessor<successorId) 
                   || (Id>successorId && (potentialSuccessor>Id || potentialSuccessor<successorId))then
                    successorId <- potentialSuccessor
            let notify = {PotentialPredecessor=Id}
            let successorNode = system.ActorSelection("akka://ChordSimulation/user/"+ ("Node:" + (string successorId)))
            successorNode <! Notify notify

        | Notify notify ->
            let potentialPredecessor = notify.PotentialPredecessor
            if predecessorId<>potentialPredecessor then
                if Id=predecessorId || predecessorId=(-1) then
                    predecessorId <- potentialPredecessor 
                if (predecessorId<Id && potentialPredecessor>predecessorId && potentialPredecessor<Id) 
                   ||(predecessorId>Id && (potentialPredecessor>predecessorId || potentialPredecessor<Id))then
                    predecessorId <- potentialPredecessor 

        | CreateFingerTable Initializeialization ->
            let randomSearchNodeId = Initializeialization.RandomSearchNodeId
            let firstOrNot = Initializeialization.FirstOrNot
            for i in 1..m do
                let insertKey = (Id + pown 2 (i-1)) % (pown 2 m)
                FingerTable.[i-1] <- [|insertKey;Id|]
            if not firstOrNot then
                let randomSearchNode = system.ActorSelection("akka://ChordSimulation/user/"+("Node:" + (string randomSearchNodeId)))
                for i in 1..m do
                    let requestKey = (Id + pown 2 (i-1)) % (pown 2 m)
                    let requestInfo = {TargetKey=requestKey; TargetKeyIndex=i; RequesterNodeId=Id; Hop=(-1)}
                    randomSearchNode <! RequestInfo requestInfo         

        return! loop()
    }
    loop()