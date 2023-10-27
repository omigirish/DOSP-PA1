module Vars
open Akka.Actor

let mutable m = 0
let mutable Nodes: (int * IActorRef)[] = [||]
let mutable flag = true
let mutable sum = 0
let mutable average = 0.0

let Chord = ActorSystem.Create("ChordSystem")
