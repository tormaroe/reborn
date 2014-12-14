**Reborn** is a programming language and the .NET compiler for that language.

Current state: *Inception phase (and a bit of a mess)*

This is a language project just for fun and for gaining experience with language and compiler construction. The result is not intended for any kind of production use.

Some details:

* Inspired by Rebol, Lisp, and functional programming, but probably does not really look like anything you will have seen before
* Compiler implemented in F#
* FParsec is used for parsing source code
* System.CodeDom (C# provider) is used for generating target code
* At least partial interop to/from other .NET languages possible

## Early code sample

A solution to Project Euler problem #1 in Reborn using a functional programming style:

    [multiple-of d x] is [zero [% x d]]
    
    [multiple-3-or-5 x] is [or [multiple-of 3 x] 
                               [multiple-of 5 y]]
    
    [multiples from to] 
     is [fold 0 add [filter multiple-3-or-5 
                            [range from to]]]
    
    [euler1 from to]
     is [println "Sum of multiples of 3 or 5 from _ to _ is _" 
                 from to [multiples from to]]
    
    [run] is [euler1 1 999] 