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
            printf "Enter a command: "
            let message = Console.ReadLine()
            printfn "Sending Command: %s" message
            writer.WriteLine(message)
            writer.Flush()
            let data = reader.ReadLine()
            
            if data="-1" then
                printfn "Incorrect operation command: The command specified is incorrect"
            elif data="-2" then
                printfn "Number of inputs is less than two."
            elif data="-3" then
                printfn "Number of inputs is more than four."
            elif data="-4" then
                printfn "One or more of the inputs contain(s) non-number(s)."
            elif data="-5" then
                client.Close()
                printfn "Terminating Connection"
                Environment.Exit(0)
            else
                printfn "Received: %s" data
            sendReceive()

        sendReceive()
    with
    | ex -> printfn "Error: %s" ex.Message

[<EntryPoint>]
let main argv =
    client()
    0
