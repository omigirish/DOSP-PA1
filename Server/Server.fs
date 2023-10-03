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

            writer.WriteLine("Hello!")
            writer.Flush()
            while true do
                let data = reader.ReadLine()
                let parts = data.Split(' ')

                if parts.Length=0 then
                    writer.WriteLine("-1") // Incorrect Operation Command

                elif parts.[0]="bye" then
                    writer.WriteLine("-5")
                    writer.Flush()
                    client.Close()

                elif parts.[0]="terminate" then
                    writer.WriteLine("-5") // Terminate client connection
                    writer.Flush()
                    client.Close()
                    Environment.Exit(0)

                else
                    let command = parts.[0]
                    let operands = List.map int (Array.toList parts.[1..])
                    let result=
                        match command with
                        | "add" -> MathOperations.add operands
                        | "subtract" -> MathOperations.subtract operands
                        | "multiply" -> MathOperations.multiply operands
                        | "divide" -> MathOperations.divide operands
                        | _ -> -1 // Invalid command  
                    printfn "Returning result %d" result
                    writer.WriteLine(result.ToString())
                    writer.Flush()
        } 
        
    let rec acceptClients () =
        let client = listener.AcceptTcpClient()
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
