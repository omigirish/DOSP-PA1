
module ChordMessageTypes
open Akka.Actor

// Type definitions
type NodeMessage =
    | Create of (int * IActorRef)
    | Join of (int * IActorRef * int * IActorRef)
    | Lookup of (int * int * IActorRef)
    | AddInFingerTable of int 
    | Done

type SimulatorMessage = 
    | CreateChord
    | JoinChord
    | CreateFingerTable
    | Lookups
    | FoundKey of (IActorRef * int * int * IActorRef)