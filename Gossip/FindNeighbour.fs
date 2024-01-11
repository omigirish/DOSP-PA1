module FindNeighbour
open System
// Find neighbors of any particular node in a 2D_Grid grid
let find_2D_Neighbour nodeID numNodes =
    
    let SideLength = sqrt (float numNodes) |> int
    [ 1 .. numNodes ]
    |> List.filter (fun y ->
        if (nodeID % SideLength = 0)
            then (y = nodeID - 1 || y = nodeID - SideLength || y = nodeID + SideLength)

        elif (nodeID % SideLength = 1) 
            then (y = nodeID + 1 || y = nodeID - SideLength || y = nodeID + SideLength)

        else (y = nodeID - 1 || y = nodeID + 1 || y = nodeID - SideLength || y = nodeID + SideLength))

// Find neighbors of any particular node in a 3D grid
let find_3D_Neighbour nodeID numNodes =

    let SideLength = Math.Round(Math.Pow((float numNodes), (1.0 / 3.0))) |> int
    [ 1 .. numNodes ]
    |> List.filter (fun y ->

        if (nodeID % SideLength = 0) then

            if (nodeID % (int (float (SideLength) ** 2.0)) = 0) then
                (y = nodeID - 1 || y = nodeID - SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

            elif (nodeID % (int (float (SideLength) ** 2.0)) = SideLength) then
                (y = nodeID - 1 || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

            else
                (y = nodeID - 1 || y = nodeID - SideLength || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))  

        elif (nodeID % SideLength = 1) then

            if (nodeID % (int (float (SideLength) ** 2.0)) = 1) then
                (y = nodeID + 1 || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

            elif (nodeID % (int (float (SideLength) ** 2.0)) = int (float (SideLength) ** 2.0) - SideLength + 1 ) then
                (y = nodeID + 1 || y = nodeID - SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

            else
                (y = nodeID + 1 || y = nodeID - SideLength || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))
        
        elif (nodeID % (int (float (SideLength) ** 2.0)) > 1) && (nodeID % (int (float (SideLength) ** 2.0)) < SideLength) then
            (y = nodeID - 1 || y = nodeID + 1 || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

        elif (nodeID % (int (float (SideLength) ** 2.0)) > int (float (SideLength) ** 2.0) - SideLength + 1) && (nodeID % (int (float (SideLength) ** 2.0)) < (int (float (SideLength) ** 2.0))) then
            (y = nodeID - 1 || y = nodeID + 1 || y = nodeID - SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0)))

        else
            (y = nodeID - 1 || y = nodeID + 1 || y = nodeID - SideLength || y = nodeID + SideLength || y = nodeID - int ((float (SideLength) ** 2.0)) || y = nodeID + int ((float (SideLength) ** 2.0))))

