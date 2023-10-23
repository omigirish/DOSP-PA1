module MathOperations

let add (numbers: string list) =
    try
        let intList = List.map int numbers
        let count = List.length intList
        if count < 2 then
            -2
        elif count > 4 then
            -3
        else
            List.sum intList
    with
    | :? System.FormatException ->
        // Handle the case where one or more inputs cannot be converted to integers
        -4

let subtract (numbers: string list) =
    try
        let intList = List.map int numbers
        let count = List.length intList
        if count < 2 then
            -2
        elif count > 4 then
            -3
        else
            match intList with
            | [] -> 0
            | x::xs -> List.fold (fun acc n -> acc - n) x xs
    with
    | :? System.FormatException ->
        // Handle the case where one or more inputs cannot be converted to integers
        -4

let multiply (numbers: string list) =
    try
        let intList = List.map int numbers
        let count = List.length intList
        if count < 2 then
            -2
        elif count > 4 then
            -3
        else
            List.fold (fun acc n -> acc * n) 1 intList
    with
    | :? System.FormatException ->
        // Handle the case where one or more inputs cannot be converted to integers
        -4

let divide (numbers: string list) =
    try
        let intList = List.map int numbers
        let count = List.length intList
        if count < 2 then
            -2
        elif count > 4 then
            -3
        else
            match intList with
            | [] -> -2
            | x::xs -> List.fold (fun acc n -> if n = 0 then -1 else acc / n) x xs
    with
    | :? System.FormatException ->
        // Handle the case where one or more inputs cannot be converted to integers
        -4
