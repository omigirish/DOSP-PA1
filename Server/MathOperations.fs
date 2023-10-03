module MathOperations

let add (numbers: int list) =
    let count = List.length numbers
    if count < 2 then
        -2
    elif count > 4 then
        -3
    else
        List.sum numbers

let subtract (numbers: int list) =
    let count = List.length numbers
    if count < 2 then
        -2
    elif count > 4 then
        -3
    else
        match numbers with
        | [] -> 0
        | x::xs -> List.fold (fun acc n -> acc - n) x xs

let multiply (numbers: int list) =
    let count = List.length numbers
    if count < 2 then
        -2
    elif count > 4 then
        -3
    else
        List.fold (fun acc n -> acc * n) 1 numbers

let divide (numbers: int list) =
    let count = List.length numbers
    if count < 2 then
        -2
    elif count > 4 then
        -3
    else
        match numbers with
        | [] -> -2
        | x::xs -> List.fold (fun acc n -> if n = 0 then -1 else acc / n) x xs
