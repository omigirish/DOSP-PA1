// Import Statements
open System
open System.Net.Sockets
open System.IO

// Define Socket Address (Server IP Address and Port Number)
let serverAddress = "127.0.0.1"
let serverPort = 8000

// Define the Client Function to handle Client Functionality
let client () =
    try
        let client = new TcpClient(serverAddress, serverPort)
        let stream = client.GetStream()
        let reader = new StreamReader(stream)
        let writer = new StreamWriter(stream)

        let rec sendReceive () =
            printf "Enter a message (or 'exit' to quit): "
            let message = Console.ReadLine()
            if message.ToLower() = "exit" then
                client.Close()
            else
                writer.WriteLine(message)
                writer.Flush()
                let data = reader.ReadLine()
                printfn "Received: %s" data
                sendReceive()

        sendReceive()
    with
    | ex -> printfn "Error: %s" ex.Message

[<EntryPoint>]
let main argv =
    client()
    0
