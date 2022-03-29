namespace CompactExpressionParser
{
    internal enum TokenType
    {
        OpeningParenthesis,
        ClosingParenthesis,
        OpeningBrackets,
        ClosingBrackets,
        OpeningBraces,
        ClosingBraces,
        StringLiteral,
        IntegerLiteral,
        FloatLiteral,
        Identifier,
        Dot,
        Semicolon,
        Comma,
        Operator,
        End
    }
}
