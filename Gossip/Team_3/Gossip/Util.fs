module Util
open System


// Round number of nodes to get perfect square in case of 2D_Grid and imperfect 3D_Grid grid
let roundNodes numNodes topology =
    match topology with
    | "2D" -> Math.Pow (Math.Round (sqrt (float numNodes)), 2.0) |> int  // For 2d grids use nearest squares
    | "imp3d" -> Math.Pow (Math.Round ((float numNodes) ** (1.0 / 3.0)), 3.0)  |> int  // for 3d grids use nearest cube
    | _ -> numNodes // for all other topologies return the same number

// Select random element from a list
let pickRandom (l: List<_>) =
    let r = Random()
    l.[r.Next(l.Length)]

// Select a random Neighbour
let getRandomNeighborID (topologyMap: Map<_, _>) nodeID =
    let (neighborList: List<_>) = (topologyMap.TryFind nodeID).Value
    let random = Random()
    neighborList.[random.Next(neighborList.Length)]

// Function to append data to the CSV file
let appendDataToCsv algorithm topology numNodes (convergenceTime: float)=
    let filePath = "./output.csv"
    
    // Format the data as a CSV Line
    let csvLine = sprintf "%s,%s,%d,%f" algorithm topology numNodes convergenceTime

    use file = new System.IO.StreamWriter(filePath, true)
    file.WriteLine(csvLine)
    file.Close()