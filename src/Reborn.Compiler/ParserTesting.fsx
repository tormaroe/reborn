#r "bin\debug\FParsecCS.dll"
#r "bin\debug\FParsec.dll"

//#r "bin\debug\Reborn.Compiler.exe"
#load "AST.fs"
#load "Parsing.fs"

open FParsec.CharParsers
open Reborn.AST
open Reborn.Parsing

let test p str =
    match run p str with
    | Success(result, _, _)   -> printfn "Success: %A" result
    | Failure(errorMsg, _, _) -> printfn "Failure: %s" errorMsg

test pliteral "12"
test pliteral "-12"
test pliteral "+12"
test pliteral "-12.0"
test pliteral "-12.2"
test pliteral ".2"

test pliteral "true"
test pliteral "false"

test pliteral "\"abc 123\""
test pliteral "\"abc\\n123\""

test pvariable "abba_asd-2$"

test pexpr "true"

test passignment "foo = bar"

// TODO: comments does not work

test pprogram " 1 \"foo\"  foo = true"