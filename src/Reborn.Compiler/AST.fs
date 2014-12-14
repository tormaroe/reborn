module Reborn.AST

// http://fssnip.net/lf

type Identifier = string
type Param = Identifier

// TODO: other literals
// * list
// * hashmap
// * regex
// * datetime
// * path / url

type Literal =
    | LitString of string
    | LitInt of int
    | LitFloat of float
    | LitBool of bool

type Expr =
    | Value of Literal
    | Variable of Identifier
    | Application of Identifier * Arg list
    | If of Expr * Expr * Expr       // ? e1 e2 e3
    | When of Expr * Expr            // ?? e1 e2
    | Lambda of Param list * Expr    // { p1 p2 -> expr }

and Arg = Expr

type Statement =
    | Assignment of Identifier * Expr   // 123 => x
    | Call of Identifier * Arg list     // [f 1 2 3]
    | Return of Expr                    // [f 1 2] is expr
    | While of Expr * Statement list    // << exp s1 s2 s3 >>
    | For of Identifier * Expr * Statement list // << exp => id s1 s2 >>

(*
[simple] is "ok"
[identity x] is x
[run] is [bar 2]
[bar x] is y when [
  [inc x] => x2
  [foo x x2] => y
  [println "foo _ _ => _" x x2 y]
]
[foo a b] is [+ a [* b 2]]
*)
type Declaration =
    // ADD RETURN EXPRESSION TO FUNCTION DECL. ??
    | Function of Identifier * Param list * Statement list

type Program = Statement list