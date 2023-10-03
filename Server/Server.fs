open System
open System.Net
open System.Net.Sockets
open System.IO

let serverPort = 8000

let server () =
    let ipAddress = IPAddress.Parse("127.0.0.1")
    let listener = new TcpListener(ipAddress, serverPort)
    listener.Start()
    printfn "Server listening on port %d" serverPort

    let rec handleClientAsync (client : TcpClient) =
        let stream = client.GetStream()
        let reader = new StreamReader(stream)
        let writer = new StreamWriter(stream)
        async {
            while true do
                let data = reader.ReadLine()
                printfn "Received: %s" data
                writer.WriteLine("Server: " + data)
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
