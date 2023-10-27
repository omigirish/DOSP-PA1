module Hash
open System
open System.Security.Cryptography
open System.IO

type KeyValue = { Key: string; Value: string }

open System.Security.Cryptography

// SHA Hash generator for a 20-bit identifier
let getHash(key: int) =
    let bytesArr = BitConverter.GetBytes(key)
    // Create a SHA-1 hash algorithm instance
    use sha1 = SHA1.Create() 
    let hash = sha1.ComputeHash(bytesArr)
    // Convert the hash bytes to an integer
    let hashInt = BitConverter.ToInt32(hash, 0)
    // Extract the first 20 bits using bitwise AND with a bit mask
    let maskedHash = hashInt &&& ((1 <<< 20) - 1)
    // Convert the masked hash to a decimal representation
    let decimalValue = int64 maskedHash   
    decimalValue

let generateHash(key: int) =
    let hash = SHA1.Create().ComputeHash(BitConverter.GetBytes(key))
    BitConverter.ToString(hash).Replace("-", "").ToLower()


// Function to read the CSV file and parse key-value pairs
let readCsvFile (filePath: string) =
    let lines = File.ReadAllLines(filePath)
    let keyValuePairs =
        lines
        |> Seq.skip 1 // Skip the header row if there is one
        |> Seq.map (fun line ->
            let parts = line.Split(',')
            if parts.Length = 2 then
                { Key = parts.[0].Trim(); Value = parts.[1].Trim() }
            else
                failwith "Invalid CSV format"
        )
    Seq.toList keyValuePairs

let getRandomId () =
    let random = new Random()
    random.Next(1, pown 2 20)

// Define a custom function to find the minimum element or return None for an empty array
let getMinByOrDefault projection arr =
    match Array.isEmpty arr with
    | true -> None
    | false -> Some (Array.minBy projection arr)