module Vars
open Akka.Actor

// Define Global Variables for the System
let mutable m = 20
let mutable nodes: (int * IActorRef)[] = [||]
let mutable flag = true
let mutable sum = 0
let mutable average = 0.0