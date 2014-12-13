module Reborn.Compiling

open System
open System.CodeDom
open Reborn.AST

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
    ``params``.TreatWarningsAsErrors <- false
    ``params``.IncludeDebugInformation <- true
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

let body statements (m:CodeMemberMethod) =
    m.Statements.AddRange( Array.ofList statements )
    m

let parameters ps (m:CodeMemberMethod) =
    m.Parameters.AddRange( Array.ofList ps )
    m

let makeParameter name =
    CodeParameterDeclarationExpression("System.Object", name)

let rec expression2dom e : CodeExpression = 
    match e with
    | Value (LitBool b)         -> upcast CodePrimitiveExpression(b) 
    | Value (LitFloat f)        -> upcast CodePrimitiveExpression(f)
    | Value (LitInt i)          -> upcast CodePrimitiveExpression(i)
    | Value (LitString s)       -> upcast CodePrimitiveExpression(s)
    | Variable identifier       -> upcast CodeVariableReferenceExpression(identifier)
    | Application (id, args)    -> upcast (makeCallExpression id args)
    
and makeCallExpression identifier args =
    let args = args |> List.map expression2dom |> Array.ofList
    CodeMethodInvokeExpression(null, identifier, args)
    
let statement2dom s : CodeStatement = 
    match s with
    | Return expr -> 
        upcast (CodeMethodReturnStatement (expression2dom expr))
    | Call (a, b) ->
        let expr = Application (a, b) |> expression2dom
        upcast CodeExpressionStatement(expr)
    | Assignment (identifier, initExpr) ->
        let initExprDom = expression2dom initExpr
        upcast (CodeVariableDeclarationStatement("System.Object", identifier, initExprDom))

let hasNoReturn statements =
    match Seq.last statements with
    | Return _ -> false
    | _ -> true

// Move to "validation" module
let addMissingReturn (statements: Statement list) =
    if (List.length statements) = 0 || (hasNoReturn statements)
    then [Return (Value (LitBool true))] |> List.append statements 
    else statements

    
let declaration2dom d : CodeTypeMember =
    match d with
    | Function (identifier, parms, statements) ->
        ``method`` identifier
        |> returntype "System.Object"
        |> attributes publicStatic
        |> parameters (parms |> List.map makeParameter)
        |> body (statements 
                |> addMissingReturn
                |> List.map statement2dom)
        :> CodeTypeMember

let makeMain () =
    ``method`` "Main"
    |> returntype "System.Void"
    |> attributes publicStatic
    |> parameters [CodeParameterDeclarationExpression("params System.String[]", "args")]
    |> body ([(Call ("run", []))] |> List.map statement2dom)
    :> CodeDom.CodeTypeMember

let addMain ast = (makeMain ()) :: ast


let compileUnitsFromAST ast =
    let (|..>) e f = f [e]
    ast |> List.map declaration2dom
        |> addMain
        |> wrapInClass "Module" nonPublic
        |..> wrapInNamespace "Rebol"
        |..> wrapInCompileUnit
        |> Array.create 1