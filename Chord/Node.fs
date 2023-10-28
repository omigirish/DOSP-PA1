module Node
open Akka.FSharp
open ChordMessageTypes
open Config
open System
open System.Security.Cryptography


let Node (nodeId:int) (mailbox: Actor<_>) = 
    let myId = nodeId
    let mutable curFixFingerIndex = 1
    let mutable myFingerTable = Array.create m [||]
    let mutable predecessorNodeId = -1
    let mutable successorNodeId = nodeId
    let mutable totalHop = 0
    let mutable finisheMessage = 0
    
    let rec loop() = actor{
        let! message = mailbox.Receive()
        match box message with
        | :? string as requestData->
            let content = System.Text.Encoding.ASCII.GetBytes requestData
            let bytes = SHA1.Create().ComputeHash(content)
            let targetKey = (int bytes.[bytes.Length-1]) + (int bytes.[bytes.Length-2]*(pown 2 8))
            let request = {TargetKey=targetKey; TargetKeyIndex=(-1); RequesterNodeId=myId; Hop=0}
            mailbox.Self<!request
        | :? int as request ->
            if request=1 then 
                let successorNode = system.ActorSelection("akka://project3/user/"+("Node:" + (string successorNodeId)))
                let stabilizeSuccessorRequest = {RequesterNodeId=myId}
                successorNode <! stabilizeSuccessorRequest
            else 
                if curFixFingerIndex>m then
                    curFixFingerIndex <- 1
                let fixFingerRequest = {TargetFinger=myFingerTable.[curFixFingerIndex-1].[0]; TargetFingerIndex=curFixFingerIndex; RequesterNodeId=myId}
                mailbox.Self <! fixFingerRequest
                curFixFingerIndex <- curFixFingerIndex+1

        | :? RequestInfo as requestInfo ->
            let targetKey = requestInfo.TargetKey
            let requesterNodeId = requestInfo.RequesterNodeId
            let targetKeyIndex = requestInfo.TargetKeyIndex
            let mutable hop = requestInfo.Hop

            let requesterNode = system.ActorSelection("akka://project3/user/"+( "Node:" + (string requesterNodeId)))
            if hop<>(-1) then
                hop <- hop+1
            if myId=successorNodeId || targetKey=myId then 
                let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex;ResultNodeId=myId; Hop=hop}
                requesterNode <! resultInfo
            elif (myId<successorNodeId && targetKey>myId && targetKey<=successorNodeId)
                ||(myId>successorNodeId && (targetKey>myId || targetKey<=successorNodeId)) then
                let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex; ResultNodeId=successorNodeId; Hop=hop}
                requesterNode <! resultInfo
            else 
                let mutable countdown = m-1
                let mutable breakLoop = false
                while countdown>=0 && not breakLoop do
                    let tempFingerNodeId = myFingerTable.[countdown].[1]
                    if (myId<tempFingerNodeId && (targetKey>=tempFingerNodeId || myId>targetKey)) 
                       ||(myId>tempFingerNodeId && (targetKey>=tempFingerNodeId && myId>targetKey)) then
                        let tempFingerNode = system.ActorSelection("akka://project3/user/"+( "Node:" + (string tempFingerNodeId)))
                        let forwardRequest = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex; RequesterNodeId=requesterNodeId; Hop=hop}
                        tempFingerNode <! forwardRequest
                        breakLoop <- true
                    elif myId=tempFingerNodeId then
                        let resultInfo = {TargetKey=targetKey; TargetKeyIndex=targetKeyIndex;ResultNodeId=tempFingerNodeId; Hop=hop}
                        requesterNode <! resultInfo
                    countdown <- countdown-1
        | :? ResultInfo as resultInfo ->
            let targetKey = resultInfo.TargetKey
            let targetKeyIndex = resultInfo.TargetKeyIndex
            let resultNodeId = resultInfo.ResultNodeId
            let hop = resultInfo.Hop 
            if hop<>(-1) then
                totalHop <- totalHop + hop
                finisheMessage <- finisheMessage + 1
                if finisheMessage=numRequests then
                    let calculateNode = system.ActorSelection("akka://project3/user/calculate")
                    calculateNode <! totalHop
                
            elif targetKeyIndex<>(-1) then
                if targetKeyIndex=1 then
                    successorNodeId <- resultNodeId
                myFingerTable.[targetKeyIndex-1].[1] <- resultNodeId

        | :? FixFingerRequestInfo as fixFingerRequestInfo ->
            let targetFinger = fixFingerRequestInfo.TargetFinger
            let targetFingerIndex = fixFingerRequestInfo.TargetFingerIndex
            let requesterNodeId = fixFingerRequestInfo.RequesterNodeId

            let requesterNode = system.ActorSelection("akka://project3/user/"+( "Node:" + (string requesterNodeId)))
            if myId=successorNodeId || targetFinger=myId then 
                let resultInfo = {TargetFinger=targetFinger; TargetFingerIndex=targetFingerIndex;ResultNodeId=myId}
                requesterNode <! resultInfo
            elif (myId<successorNodeId && targetFinger>myId && targetFinger<=successorNodeId)
                ||(myId>successorNodeId && (targetFinger>myId || targetFinger<=successorNodeId)) then
                let resultInfo = {TargetFinger=targetFinger; TargetFingerIndex=targetFingerIndex;ResultNodeId=successorNodeId}
                requesterNode <! resultInfo
            else 
                let mutable countdown = m-1
                let mutable breakLoop = false
                while countdown>=0 && not breakLoop do
                    let tempFingerNodeId = myFingerTable.[countdown].[1]
                    if (myId<tempFingerNodeId && (targetFinger>=tempFingerNodeId || myId>targetFinger)) 
                       ||(myId>tempFingerNodeId && (targetFinger>=tempFingerNodeId && myId>targetFinger)) then
                        let tempFingerNode = system.ActorSelection("akka://project3/user/"+ ( "Node:" + (string tempFingerNodeId)))
                        let forwardRequest = {TargetFinger=targetFinger; TargetFingerIndex=targetFingerIndex; RequesterNodeId=requesterNodeId}
                        tempFingerNode <! forwardRequest
                        breakLoop <- true
                    elif myId=tempFingerNodeId then
                        let resultInfo = {TargetFinger=targetFinger; TargetFingerIndex=targetFingerIndex;ResultNodeId=tempFingerNodeId}
                        requesterNode <! resultInfo
                    countdown <- countdown-1
        | :? FixFingerResponseInfo as fixFingerResponseInfo ->
            let targetFinger = fixFingerResponseInfo.TargetFinger
            let targetFingerIndex = fixFingerResponseInfo.TargetFingerIndex
            let resultNodeId = fixFingerResponseInfo.ResultNodeId
            myFingerTable.[targetFingerIndex-1].[1] <- resultNodeId

        | :? StabilizeSuccessorRequest as stabilizeSuccessorRequest ->
            let requesterNodeId = stabilizeSuccessorRequest.RequesterNodeId
            let requesterNode = system.ActorSelection("akka://project3/user/"+("Node:" + (string requesterNodeId)))
            if predecessorNodeId=(-1) then
                predecessorNodeId <- myId
            let stabilizeSuccessorResponse = {PotentialSuccessor=predecessorNodeId}
            requesterNode <! stabilizeSuccessorResponse
        | :? StabilizeSuccessorResponse as stabilizeSuccessorResponse ->
            let potentialSuccessor = stabilizeSuccessorResponse.PotentialSuccessor
            if potentialSuccessor<>successorNodeId then
                if myId=successorNodeId then
                    successorNodeId <- potentialSuccessor
                if (myId<successorNodeId && potentialSuccessor>myId && potentialSuccessor<successorNodeId) 
                   || (myId>successorNodeId && (potentialSuccessor>myId || potentialSuccessor<successorNodeId))then
                    successorNodeId <- potentialSuccessor
            let updatePredecessorNotification = {PotentialPredecessor=myId}
            let successorNode = system.ActorSelection("akka://project3/user/"+ ("Node:" + (string successorNodeId)))
            successorNode <! updatePredecessorNotification
        | :? UpdatePredecessorNotification as updatePredecessorNotification ->
            let potentialPredecessor = updatePredecessorNotification.PotentialPredecessor
            if predecessorNodeId<>potentialPredecessor then
                if myId=predecessorNodeId || predecessorNodeId=(-1) then
                    predecessorNodeId <- potentialPredecessor 
                if (predecessorNodeId<myId && potentialPredecessor>predecessorNodeId && potentialPredecessor<myId) 
                   ||(predecessorNodeId>myId && (potentialPredecessor>predecessorNodeId || potentialPredecessor<myId))then
                    predecessorNodeId <- potentialPredecessor 

        | :? InitializationInfo as initialization ->
            let randomSearchNodeId = initialization.RandomSearchNodeId
            let firstOrNot = initialization.FirstOrNot
            for i in 1..m do
                let insertKey = (myId + pown 2 (i-1)) % (pown 2 m)
                myFingerTable.[i-1] <- [|insertKey;myId|]
            if not firstOrNot then
                let randomSearchNode = system.ActorSelection("akka://project3/user/"+("Node:" + (string randomSearchNodeId)))
                for i in 1..m do
                    let requestKey = (myId + pown 2 (i-1)) % (pown 2 m)
                    let requestInfo = {TargetKey=requestKey; TargetKeyIndex=i; RequesterNodeId=myId; Hop=(-1)}
                    randomSearchNode <! requestInfo

        | _ ->
            printfn "Unkown message"
            

        return! loop()
    }
    loop()