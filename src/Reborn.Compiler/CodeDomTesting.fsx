open System

// CHECK WHICH PROVIDERS ARE AVAILABLE

let compilers = CodeDom.Compiler.CodeDomProvider.GetAllCompilerInfo()
compilers |> Array.iter (fun c -> c.GetLanguages() |> printfn "%A")

// COMPILE FROM SOURCE WITH OPTIONS!

let csc = CodeDom.Compiler.CodeDomProvider.CreateProvider "c#"
let ``params`` = new CodeDom.Compiler.CompilerParameters()
``params``.GenerateExecutable <- true
``params``.GenerateInMemory <- false
``params``.IncludeDebugInformation <- true
//``params``.MainClass = ""
``params``.OutputAssembly = ".\\test.exe"
//``params``.ReferencedAssemblies.Add ""
``params``.TreatWarningsAsErrors <- true
//``params``.CompilerOptions <- "" // commandline arguments
let result = csc.CompileAssemblyFromSource(``params``, [|
                                                            @"class Foo { 
                                                                static void Main(){
                                                                }
                                                              }"
                                                        |])

printfn "Compiler returned %A" result.NativeCompilerReturnValue
for e in result.Errors do
    printfn "(%A) Error line %A col %A: %s" 
        e.ErrorNumber e.Line e.Column e.ErrorText
printfn "Assembly at %s" result.PathToAssembly
for o in result.Output do
    printfn "Compiler said: %s" o

// CODEDOM TEST

let ns = CodeDom.CodeNamespace "MyNamespace"
let cl = CodeDom.CodeTypeDeclaration "MyClass"
cl.TypeAttributes <- Reflection.TypeAttributes.NotPublic

//let fi = CodeDom.CodeMemberField("System.Int32", "_field")
//fi.Attributes <- CodeDom.MemberAttributes.Private

//cl.Members.Add fi

let main = CodeDom.CodeMemberMethod()
main.Attributes <- CodeDom.MemberAttributes.Public ||| CodeDom.MemberAttributes.Static
main.ReturnType <- CodeDom.CodeTypeReference "System.Void"
main.Name <- "Main"

cl.Members.Add main

ns.Types.Add cl

let compileUnit = CodeDom.CodeCompileUnit()
compileUnit.Namespaces.Add ns

let result2 = csc.CompileAssemblyFromDom(``params``, [|compileUnit|])

printfn "Compiler returned %A" result2.NativeCompilerReturnValue
for e in result2.Errors do
    printfn "(%A) Error line %A col %A: %s" 
        e.ErrorNumber e.Line e.Column e.ErrorText
printfn "Assembly at %s" result2.PathToAssembly
for o in result2.Output do
    printfn "Compiler said: %s" o












