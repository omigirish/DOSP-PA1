module Topology
open FindNeighbour
open Util

let Create_Line_Topology numNodes =
    let mutable map = Map.empty
    [ 1 .. numNodes ]
    |> List.map (fun nodeID ->
        let listNeighbors = List.filter (fun y -> (y = nodeID + 1 || y = nodeID - 1)) [ 1 .. numNodes ]
        map <- map.Add(nodeID, listNeighbors))
    |> ignore
    map

let Create_2D_Grid_Topology numNodes =
    let mutable map = Map.empty
    [ 1 .. numNodes ]
    |> List.map (fun nodeID ->
        let listNeighbors = find_2D_Neighbour nodeID numNodes
        map <- map.Add(nodeID, listNeighbors))
    |> ignore
    map

let Create_Imperfect_3D_Grid_Topology numNodes =
    let mutable map = Map.empty
    [ 1 .. numNodes ]
    |> List.map (fun nodeID ->
        let mutable listNeighbors = find_3D_Neighbour nodeID numNodes
        let random =
            [ 1 .. numNodes ]
            |> List.filter (fun m -> m <> nodeID && not (listNeighbors |> List.contains m))
            |> pickRandom
        let listNeighbors = random :: listNeighbors
        map <- map.Add(nodeID, listNeighbors))
    |> ignore
    map

let Create_Full_Network_Topology numNodes =
    let mutable map = Map.empty
    [ 1 .. numNodes ]
    |> List.map (fun nodeID ->
        let listNeighbors = List.filter (fun y -> nodeID <> y) [ 1 .. numNodes ]
        map <- map.Add(nodeID, listNeighbors))
    |> ignore
    map

let CreateTopology numNodes topology =
    let mutable map = Map.empty
    match topology with
    | "line" -> Create_Line_Topology numNodes
    | "2D" -> Create_2D_Grid_Topology numNodes
    | "imp3D" -> Create_Imperfect_3D_Grid_Topology numNodes
    | "full" -> Create_Full_Network_Topology numNodes
    | _ -> Map.empty
