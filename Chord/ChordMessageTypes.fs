
module ChordMessageTypes

type FixFingerRequestInfo = {
    TargetFinger : int 
    TargetFingerIndex : int 
    RequesterNodeId : int
}

type FixFingerResponseInfo = {
    TargetFinger : int 
    TargetFingerIndex : int 
    ResultNodeId : int
}

type RequestInfo = {
    TargetKey : int
    TargetKeyIndex : int
    RequesterNodeId : int
    Hop : int
}

type ResultInfo = {
    TargetKey : int
    TargetKeyIndex : int
    ResultNodeId : int
    Hop : int
}

type UpdatePredecessorNotification = {
    PotentialPredecessor : int
}

type StabilizeSuccessorRequest = {
    RequesterNodeId : int
}

type StabilizeSuccessorResponse = {
    PotentialSuccessor : int
}

type InitializationInfo = {
    RandomSearchNodeId : int
    FirstOrNot : bool
}