using System;
using System.Globalization;

namespace CompactExpressionParser
{
    internal struct Token
    {
        public TokenType Type;
        public string StringValue;
        public double FloatValue;
        public long IntegerValue;
        public int LineNumber;
        public int Position;

        public bool IsNumber { get => Type == TokenType.IntegerLiteral || Type == TokenType.FloatLiteral; }

        public object Number
        {
            get => Type == TokenType.IntegerLiteral ? (object)IntegerValue : (Type == TokenType.FloatLiteral ? (object)FloatValue : throw new InvalidOperationException("Token is not a number."));
        }

        public override string ToString()
        {
            switch (Type)
            {
                case TokenType.StringLiteral:
                case TokenType.Identifier:
                case TokenType.Operator:
                    return $"{Type}({StringValue})";
                case TokenType.FloatLiteral:
                    return $"{Type}({FloatValue.ToString(CultureInfo.InvariantCulture)})";
                case TokenType.IntegerLiteral:
                    return $"{Type}({IntegerValue})";
                default:
                    return Type.ToString();
            }
        }
    }
}
