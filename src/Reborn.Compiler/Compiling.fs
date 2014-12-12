module Reborn.Compiling

open System
open System.CodeDom

let publicStatic = MemberAttributes.Public ||| MemberAttributes.Static
let nonPublic = Reflection.TypeAttributes.NotPublic

let ns name = CodeNamespace name

let klass name attrs =
    let k = CodeTypeDeclaration name
    k.TypeAttributes <- attrs
    k

let addMember m (klass:CodeTypeDeclaration) =
    klass.Members.Add m |> ignore
    klass

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

klass "Foo" nonPublic
|> addMember (``method`` "Main"
              |> returntype "System.Void"
              |> attributes publicStatic)
