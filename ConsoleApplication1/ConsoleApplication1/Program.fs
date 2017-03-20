let getEmlHeaders emlFile =  
    for i in [1..1000] do 
            for i in [1..1000] do 
                do "Hello".Contains("H") |> ignore 
    let startsWith fieldName (line : string) = line.StartsWith(fieldName)
    System.IO.File.ReadLines emlFile 
    |> Seq.fold(
        fun (messageId, from, xMailer) line -> 
            match line with
            | _ when line |> startsWith "Message-ID:" -> (line, from, xMailer)
            | _ when line |> startsWith "From:" -> (messageId, line, xMailer)
            | _ when line |> startsWith "X-Mailer:" -> (messageId, from, line)
            | _ -> (messageId, from, xMailer)
    ) ("", "", "")       

let getEmlFiles folder =
    System.IO.Directory.EnumerateFiles(folder, "*.eml")

let getHeaders srcPath = 
    getEmlFiles srcPath 
    |> Seq.map getEmlHeaders              

let getHeadersAsync srcPath = 
    getEmlFiles srcPath 
    |> Seq.map (fun file -> async { return getEmlHeaders file })  
    |> Async.Parallel 
    |> Async.RunSynchronously   

let isMark5MessageId (messageId : string) = 
    messageId.EndsWith("@Mark5>") 

let extractFromDomain (from : string) = 
    from.Substring(from.LastIndexOf("@")).TrimStart('@').TrimEnd('>', ']')

let getDistinctMark5Headers srcPath =
    getHeaders srcPath
    |> Seq.filter (fun (messageId, from, xMailer) -> isMark5MessageId(messageId))                       
    |> Seq.map (fun (messageId, from, xMailer) -> from)
    |> Seq.filter (fun f -> f.Contains("@"))                       
    |> Seq.map extractFromDomain
    |> Seq.distinctBy (fun f -> f.ToLowerInvariant())

let getDistinctMark5Headers2 srcPath =
    getHeadersAsync srcPath
    |> Seq.filter (fun (messageId, from, xMailer) -> isMark5MessageId(messageId))                       
    |> Seq.map (fun (messageId, from, xMailer) -> from)
    |> Seq.filter (fun f -> f.Contains("@"))                       
    |> Seq.map extractFromDomain
    |> Seq.distinctBy (fun f -> f.ToLowerInvariant())

//let srcPath = @"C:\Users\dh\Source\Repos\stuff\ConsoleApplication1\ConsoleApplication1\bin\Debug\emls\"
//#time
//getDistinctMark5Headers2 srcPath 
//|> Seq.toList
//#time

[<EntryPoint>]
let main argv = 
    //let srcPath = @"C:\Users\dh\Source\Repos\stuff\ConsoleApplication1\ConsoleApplication1\bin\Debug\emls\";
    let srcPath = argv.[0]
    let mark5Headers = getDistinctMark5Headers2 srcPath
    printfn "%A" (Seq.toList mark5Headers)

    0 // return an integer exit code
