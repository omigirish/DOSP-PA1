open System
open System.Net
open System.Net.Sockets
open System.IO
open MathOperations

let serverPort = 8000

let server () =
    let ipAddress = IPAddress.Parse("127.0.0.1")
    let listener = new TcpListener(ipAddress, serverPort)
    listener.Start()
    printfn "Server is running and listening on port %d" serverPort

    let mutable clientIdCounter = 1

    let sendHelloMessage (writer: StreamWriter) =
        writer.WriteLine("Hello!")
        writer.Flush()

    let rec handleClientAsync (client : TcpClient) =
        let clientId = clientIdCounter
        clientIdCounter <- clientIdCounter + 1

        let stream: NetworkStream = client.GetStream()
        let reader: StreamReader = new StreamReader(stream)
        let writer: StreamWriter = new StreamWriter(stream)

        sendHelloMessage writer

        async {
            let mutable isClientClosed = false
            while not isClientClosed do
                try
                    let data = reader.ReadLine()
                    if data = null then
                        isClientClosed <- true
                    else
                        let parts = data.Split(' ')
                        printfn "Received: %s" data

                        if parts.Length = 0 then
                            writer.WriteLine("-1")
                        elif parts.[0] = "bye" then
                            writer.WriteLine("-5")
                            writer.Flush()
                            printfn "Responding to Client %d with result -5" clientId
                            client.Close()
                            isClientClosed <- true
                        elif parts.[0] = "terminate" then
                            writer.WriteLine("-5")
                            writer.Flush()
                            client.Close()
                            printfn "Responding to Client %d with result: -5" clientId
                            printfn "exit"
                            Environment.Exit(0)
                        else
                            let command: string = parts.[0]
                            let operands = Array.toList parts.[1..] 
                            let result: int =
                                match command.ToLower() with
                                | "add" -> MathOperations.add operands
                                | "subtract" -> MathOperations.subtract operands
                                | "multiply" -> MathOperations.multiply operands
                                | "divide" -> MathOperations.divide operands
                                | _ -> -1
                            printfn "Responding to Client %d with result: %d" clientId result
                            writer.WriteLine(result.ToString())
                            writer.Flush()
                with
                | :? System.ObjectDisposedException ->
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
