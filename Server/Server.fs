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

    // Maintain a client ID counter
    let mutable clientIdCounter = 0

    let rec handleClientAsync (client : TcpClient) =
        // Increment the client ID counter for each new client
        let clientId = clientIdCounter
        clientIdCounter <- clientIdCounter + 1

        let stream: NetworkStream = client.GetStream()
        let reader: StreamReader = new StreamReader(stream)
        let writer: StreamWriter = new StreamWriter(stream)
        
        async {
            let mutable isClientClosed = false
            while not isClientClosed do
                try
                    let data = reader.ReadLine()
                    if data = null then
                        // Client closed the connection
                        isClientClosed <- true
                    else
                        let parts = data.Split(' ')

                        if parts.Length = 0 then
                            writer.WriteLine("-1") // Incorrect Operation Command

                        elif parts.[0] = "bye" then
                            writer.WriteLine("-5")
                            writer.Flush()
                            printfn "Client %d: Terminating the connection with client" clientId
                            client.Close()
                            isClientClosed <- true

                        elif parts.[0] = "terminate" then
                            writer.WriteLine("-5") // Terminate client connection
                            writer.Flush()
                            client.Close()
                            Environment.Exit(0)

                        else
                            let command: string = parts.[0]
                            let operands: int list = List.map int (Array.toList parts.[1..])
                            let result: int=
                                match command with
                                | "add" -> MathOperations.add operands
                                | "subtract" -> MathOperations.subtract operands
                                | "multiply" -> MathOperations.multiply operands
                                | "divide" -> MathOperations.divide operands
                                | _ -> -1 // Invalid command  
                            printfn "Responding to Client %d with result %d" clientId result
                            writer.WriteLine(result.ToString())
                            writer.Flush()
                with
                | :? System.ObjectDisposedException ->
                    // Handle the exception when the client closes the connection
                    isClientClosed <- true
        } 
        
    let rec acceptClients () =
        let client: TcpClient = listener.AcceptTcpClient()
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
