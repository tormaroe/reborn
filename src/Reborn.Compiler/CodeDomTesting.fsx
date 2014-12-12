//#load "AST.fs"
//#load "Parsing.fs"
#load "Compiling.fs"

open System
open Reborn.Compiling

let csc = createCSharpProvider ()
let parms = getCompilerParams ()

let main = ``method`` "Main"
           |> returntype "System.Void"
           |> attributes publicStatic

let (|..>) e f = f [e]

let compileUnit =
    main
    |..> wrapInClass "MyClass" nonPublic
    |..> wrapInNamespace "MyNamespace"
    |..> wrapInCompileUnit

csc.CompileAssemblyFromDom(parms, [|compileUnit|])
|> printCompilerResult












