
// Base forms
+ (alias add)
- (alias sub)
* (alias mul)
/ (alias div)
% (alias mod)
= (alias eq, equal)
gt
lt
str
^ (alias not)
and
or
list
map (possible >)
filter (possible <)
fold (possibly <>)
range (possibly #)

[zero x] is [= 0 x]

[identity x] is x

[println format .. args] is result when [
	[String.Format format args] => result
	[Console.WriteLine result]
]

[inc x] is [+ x 1]
[dec x] is [- x 1]

[gte a b] is [or [gt a b] [eq a b]]
[lte a b] is [or [lt a b] [eq a b]]

same as (just an idea):
[gte a b] is [or <gt> <eq>]

[fold init f list] is init when [
  << list => elem
    [f init elem] => init
  >>
]

[map f list] is list2 when [
  [] => list2
  << list => elem
    [append [f elem] list2] => list2
  >>
]

[map f list] is [fold [] {acc x -> [append [f x] acc]} list]

[filter p list] is [
  fold [] 
       {acc x -> ? [p x] 
       	           [append x acc] 
       	           acc} 
       list
]
