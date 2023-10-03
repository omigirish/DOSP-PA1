open System
open System.Net
open System.Net.Sockets
open System.IO
open MathOperations // Import the MathOperations module
let serverPort = 8000

let server () =
    let ipAddress = IPAddress.Parse("127.0.0.1")
    let listener = new TcpListener(ipAddress, serverPort)
    listener.Start()
    printfn "Server listening on port %d" serverPort

    let rec handleClientAsync (client : TcpClient) =
        let stream: NetworkStream = client.GetStream()
        let reader: StreamReader = new System.IO.StreamReader(stream)
        let writer: StreamWriter = new System.IO.StreamWriter(stream)
        async {
            while true do
                let data = reader.ReadLine()
                let parts = data.Split(' ')
                if parts.Length < 3 || parts.Length > 6 then
                    writer.WriteLine("-1") // Incorrect format
                else
                    let command = parts.[0]
                    let operands = List.map int (Array.toList parts.[1..])
                    let result=
                        match command with
                        | "add" -> MathOperations.add operands
                        | "subtract" -> MathOperations.subtract operands
                        | "multiply" -> MathOperations.multiply operands
                        | "divide" -> MathOperations.divide operands
                        | "terminate" -> -5
                        | "exit" -> -5 
                        | _ -> -1 // Invalid command  
                    printfn "Returning result %d" result
                    writer.WriteLine(result.ToString())
                    writer.Flush()
        } 
        
    let rec acceptClients () =
        let client = listener.AcceptTcpClient()
        printfn "Client connected"
        let clientTask = async {
            do! handleClientAsync client
        }
        Async.Start clientTask
        acceptClients ()

    acceptClients ()

[<EntryPoint>]
let main argv =
    server()
    Console.ReadKey() |> ignore
    0
