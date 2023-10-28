
module ChordMessageTypes
open Akka.Actor
open System.Collections.Generic

let mutable KeyValuePairs = Dictionary<int64, string>()
type KeyValue = { Key: int64; Value: string }

// Define message types that the actors will send and receive.
// Messages sent by the nodes in the Chord
type NodeMessage =
    | Create  // Create a Node
    | Join of (int * IActorRef)  // Join a node into an existing Chord Ring
    | Lookup of string  // Lookup a given key
    | FindSuccessor of (int * IActorRef) 
    | UpdateSuccessor of (int * IActorRef) 
    | AssignKey of KeyValue  // Assign Key-Value Pair to Node
    | Notify of (int* IActorRef)
    | SendPredecessor 

type ResponseMessages =
    | Actor of (int* IActorRef)