
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

type Initialize = {
    RandomSearchNodeId : int
    FirstOrNot : bool
}

type NodeMessage =
    | FixFinger of FixFinger
    | FixFingerRes of FixFingerRes
    | RequestInfo of RequestInfo
    | ResultInfo of ResultInfo
    | Notify of Notify
    | Stabilize of Stabilize
    | StabilizeResponse of StabilizeResponse
    | Initialize of Initialize
    | RequestData of string
    | Request of int

type SimulatorMessage =
    | UpdateHops of (int*int)