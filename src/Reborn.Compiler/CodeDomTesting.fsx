#r "bin\debug\FParsecCS.dll"
#r "bin\debug\FParsec.dll"
#load "AST.fs"
#load "Parsing.fs"
#load "Compiling.fs"

open System
open FParsec
open Reborn.AST
open Reborn.Parsing
open Reborn.Compiling

let csc = createCSharpProvider ()
let parms = getCompilerParams ()

let statements = match "foo = true" |> run pprogram with
| Success(result, _, _) -> List.map statement2dom result
| Failure(errMsg, _, _) -> failwith errMsg


let compileUnit2 = 
    [Function ("run", 
               [],
               [(Assignment ("bar", Value (LitInt 42)))
                (Call ("method2", [(Variable "bar")]))
                (Return (Variable "bar"))])
     Function ("method2", 
               ["p1"],
               [(Call ("System.Console.WriteLine", [(Value (LitString "test: {0}"))
                                                    (Variable "p1")]))])]
    |> compileUnitsFromAST

csc.CompileAssemblyFromDom(parms, compileUnit2)
|> printCompilerResult





