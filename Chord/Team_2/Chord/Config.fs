module Config
open Akka.Actor
open System

let mutable numNodes = 0|> int
let mutable numRequests = 0 |> int

let system = ActorSystem.Create("ChordSimulation")
let rand = System.Random()

let m = 20

let startTime = TimeSpan.FromSeconds(0.05)
let delayTimeForStabilize = TimeSpan.FromSeconds(0.01)
let delayTimeForFixfinger = TimeSpan.FromSeconds(0.02)