open System
open System.Net.Sockets
open System.IO

let serverAddress = "127.0.0.1"
let serverPort = 8000

let client () =
    try
        let client = new TcpClient(serverAddress, serverPort)
        let stream = client.GetStream()
        let reader = new StreamReader(stream)
        let writer = new StreamWriter(stream)

        let receiveHelloMessage () =
            let helloMessage = reader.ReadLine()
            printfn "Server: %s" helloMessage

        receiveHelloMessage()

        let rec sendReceive () =
            printf "Enter a command: "
            let message = Console.ReadLine()
            
            try
                printfn "Sending Command: %s" message
                writer.WriteLine(message)
                writer.Flush()
                let data = reader.ReadLine()
                if String.IsNullOrEmpty(data) then
                    printfn "Server has terminated connection....exit"
                    client.Close()
                    Environment.Exit(0)
                if data = "-1" then
                    printfn "Incorrect operation command: The command specified is incorrect"
                elif data = "-2" then
                    printfn "Number of inputs is less than two."
                elif data = "-3" then
                    printfn "Number of inputs is more than four."
                elif data = "-4" then
                    printfn "One or more of the inputs contain(s) non-number(s)."
                elif data = "-5" then
                    printfn "bye"
                    client.Close()
                    Environment.Exit(0)
                else
                    printfn "Server Response: %s" data
                sendReceive()
            with | :? System.IO.IOException as ex ->
                    printfn "Server has terminated connection....exit"
                    client.Close()
                    Environment.Exit(0)

        sendReceive()
    with
    | ex -> printfn "Error: %s" ex.Message

[<EntryPoint>]
let main argv =
    client()
    0
