# CompactExpressionParser
A lightweight and simple expression parser for .NET. Distributed as library and C# source code package.

## Expression Syntax
For the most part, the expression syntax is pre-defined by the implementation. However, the unary and binary operators are user-defined. Here are some examples of valid expressions:

```
"Hello World!"
```

```
MD5("Hello World!")
```

```
#("Hello World!")
```

```
#"Hello World!"
```

```
(5 + 2) >= 7
```

```
status IN [1,2,3,7]
```

### Literals
`true`, `false` are parsed into boolean values.
`"Hello World!"`, `"Escaped\nsequence with \u07 special chars."` are string literals.
`192`, `0.91`, `-19` are numeric literals, which may be either of type `double` or `long`.

### Identifier
`myIdentifier` (anything starting with a letter, which is not `true`, `false`, or one of the user-defined operators)

### Binary operators
`5 + 6`, `5 == 6`

### Unary operators
`!5`, `~"example"`, `#myIdentifier`

### Member access
`myIdentifier.myProperty`, `"example".length`, `getObject().myMethod()`

### Invocations
`hashCode("with this string")`, `hashCode("with this string", salt, true)`

### Subscripts
`myList[0]`, `myList["id"]`, `myList[ (5 + offset) ]`

### Lists
`[1, 2, 3]`, `[var1, 7+1, [1,2,3]]`
