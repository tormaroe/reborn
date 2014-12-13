module Reborn.Parsing

open FParsec
open Reborn.AST

(* White space *)

let maxCount = System.Int32.MaxValue
let pcomment  : Parser<string, unit> = pstring "//" >>. many1Satisfy ((<>) '\n') 
let pspaces = spaces >>. many (spaces >>. pcomment >>. spaces)
let pmlcomment = pstring "/*" >>. skipCharsTillString "*/" true (maxCount)
let ws = pspaces >>. many (pspaces >>. pmlcomment >>. pspaces) |>> (fun _ -> ())
let str_ws s = pstring s .>> ws

(* Number literals *)

type Lit = NumberLiteralOptions
let numberFormat = Lit.AllowMinusSign 
                    ||| Lit.AllowFraction
                    ||| Lit.AllowFractionWOIntegerPart
                    ||| Lit.AllowPlusSign

let pnumber : Parser<Literal, unit> = 
    numberLiteral numberFormat "number"
    |>> fun nl ->
        if nl.IsInteger then LitInt(int nl.String)
        else LitFloat(float nl.String)

(* Boolean literals *)

let ptrue = str_ws "true" |>> fun _ -> LitBool true
let pfalse = str_ws "false" |>> fun _ -> LitBool false
let pbool = ptrue <|> pfalse

(* String literals *)

let pstringliteral : Parser<Literal, unit> =
    let normalChar = satisfy (fun c -> c <> '\\' && c <> '"')
    let unescape = function
        | 'n' -> '\n'
        | 'r' -> '\r'
        | 't' -> '\t'
        | c   -> c
    let escapedChar = pstring "\\" >>. (anyOf "\\nrt\"" |>> unescape)
    between (pstring "\"") (pstring "\"")
            (manyChars (normalChar <|> escapedChar))
    |>> LitString

(* Main literal parser *)

let pliteral = pnumber <|> pbool <|> pstringliteral

(* Identifier *)

let pidentifier : Parser<Identifier, unit> = 
    let isIdentifierFirstChar c = isLetter c || c = '_'
    let isIdentifierChar c = isLetter c || isDigit c || isAnyOf ['_';'-';'#';'$'] c
    many1Satisfy2L isIdentifierFirstChar isIdentifierChar "identifier"

(* Expr *)

let (pexpr:Parser<Expr,unit>), pexprimpl = createParserForwardedToRef ()

let pvariable = pidentifier |>> Variable

let pvalue = (pliteral |>> fun x -> Value(x)) 
             <|> attempt pvariable

// TODO: Lambda
// TODO: Application

let opp = OperatorPrecedenceParser<Expr,unit,unit>()
pexprimpl := opp.ExpressionParser
let term = pvalue .>> ws <|> between (str_ws "(") (str_ws ")") pexpr
opp.TermParser <- term

(* Statements *)

let passignment = pidentifier .>> ws .>> str_ws "=" .>>. pexpr
                  |>> Assignment

//let pexpressionstatement = pexpr |>> ExpressionStatement

let pstatement = passignment //<|> pexpressionstatement

(* Program *)

let pmanystatements = (many pstatement)

let pprogram = ws >>. pmanystatements