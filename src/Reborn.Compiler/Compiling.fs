module Reborn.Compiling

open System
open System.CodeDom

let createCSharpProvider () =
    CodeDom.Compiler.CodeDomProvider.CreateProvider "c#"

let getCompilerParams () =
    let ``params`` = new CodeDom.Compiler.CompilerParameters()
    ``params``.GenerateExecutable <- true
    ``params``.GenerateInMemory <- false
    ``params``.IncludeDebugInformation <- true
    //``params``.MainClass = ""
    ``params``.OutputAssembly <- ".\\test.exe"
    //``params``.ReferencedAssemblies.Add ""
    ``params``.TreatWarningsAsErrors <- true
    //``params``.CompilerOptions <- "" // commandline arguments
    ``params``

let printCompilerResult (res:Compiler.CompilerResults) =
    for e in res.Errors do
        printfn "(%A) Error line %A col %A: %s" 
            e.ErrorNumber e.Line e.Column e.ErrorText
    printfn "Assembly at %s" res.PathToAssembly
    printfn (if res.NativeCompilerReturnValue = 0
             then "COMPILATION SUCCESSFUL"
             else "COMPILATION FAILED!")

let publicStatic = MemberAttributes.Public ||| MemberAttributes.Static
let nonPublic = Reflection.TypeAttributes.NotPublic

let wrap x elemAction elems =
    let elemAction' = elemAction x
    elems |> List.iter elemAction'
    x

let wrapInNamespace name =
    wrap (CodeNamespace name)
         (fun ns t -> ns.Types.Add t |> ignore)
        
let wrapInCompileUnit namespaces =
    namespaces
    |> wrap (CodeCompileUnit())
            (fun cu n -> cu.Namespaces.Add n |> ignore)

let wrapInClass name attrs members =
    let c = CodeTypeDeclaration name
    c.TypeAttributes <- attrs
    c.Attributes <- MemberAttributes.Static // No effect ?
    members |> wrap c (fun c m -> c.Members.Add m |> ignore)

let ``method`` name =
    let m = CodeMemberMethod ()
    m.Name <- name
    m

let returntype (t:string) (m:CodeMemberMethod) =
    m.ReturnType <- CodeTypeReference t
    m

let attributes (a:MemberAttributes) (m:CodeMemberMethod) =
    m.Attributes <- a
    m
