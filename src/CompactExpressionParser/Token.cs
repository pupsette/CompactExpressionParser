namespace CompactExpressionParser
{
    public struct Token
    {
        public TokenType Type;
        public string StringValue;
        public double NumberValue;
        public int LineNumber;
        public int Position;

        public override string ToString()
        {
            switch (Type)
            {
                case TokenType.StringLiteral:
                case TokenType.Identifier:
                case TokenType.Operator:
                    return $"{Type}({StringValue})";
                case TokenType.NumberLiteral:
                    return $"{Type}({NumberValue})";
                default:
                    return Type.ToString();
            }
        }
    }
}
