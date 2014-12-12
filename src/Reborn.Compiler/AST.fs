module Reborn.AST

// http://fssnip.net/lf

type Identifier = string
type Param = Identifier

type Literal =
    | LitString of string
    | LitInt of int
    | LitFloat of float
    | LitBool of bool

type Expr =
    | Value of Literal
    | Variable of Identifier
    | Lambda of Param list * Expr list
    | Application of Identifier * Arg list

and Arg = Expr

type Statement =
    | Assignment of Identifier * Expr
    | ExpressionStatement of Expr

type Program = Statement list