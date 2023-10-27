
module ChordMessageTypes
open Akka.Actor

// Type definitions
type ActorMsg = 
    | RouteFinish of int
    | StartJoining
    | JoinNode of int
    | Acknowledgement
    | StartJoin of int * int 
    | Stabilize of int
    | Notify of int
    | FixFingers of int
    | CheckPredecessor of int
    | UpdateSuccessor