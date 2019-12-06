namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class BinaryOperator : Expression
    {
        public BinaryOperator(Expression expr1, string op, Expression expr2, int lineNumber, int position)
            : base (lineNumber, position)
        {
            Operand1 = expr1;
            Operator = op;
            Operand2 = expr2;
        }

        public Expression Operand1 { get; }
        public string Operator { get; }
        public Expression Operand2 { get; }

        public override string ToString()
        {
            return $"({Operand1} {Operator} {Operand2})";
        }
    }
}
