module SupportFunctions
open System
open System.IO
open FSharp.Data


let ranStr n = 
    let r = Random()
    let chars = Array.concat([[|'a' .. 'z'|];[|'0' .. '9'|]])
    let sz = Array.length chars in
    String(Array.init n (fun _ -> chars.[r.Next sz]))


// Function to append data to the CSV file
let appendDataToCsv numNodes numRequests avgHop =
    let filePath = "./output.csv"
    
    // Format the data as a CSV line
    let csvLine = sprintf "%d,%d,%f" numNodes numRequests avgHop

    use file = new System.IO.StreamWriter(filePath, true)
    file.WriteLine(csvLine)
    file.Close()
