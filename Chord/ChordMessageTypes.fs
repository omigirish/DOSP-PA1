
module ChordMessageTypes

type FixFinger = {
    TargetFinger : int 
    Next : int 
    RequesterNodeId : int
}

type FixFingerRes = {
    TargetFinger : int 
    Next : int 
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

type Notify = {
    PotentialPredecessor : int
}

type Stabilize = {
    RequesterNodeId : int
}

type StabilizeResponse = {
    PotentialSuccessor : int
}

type Init = {
    RandomSearchNodeId : int
    FirstOrNot : bool
}