namespace CompactExpressionParser
{
    public class UnaryOperator : Expression
    {
        public UnaryOperator(string op, Expression expr, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Operator = op;
            Operand = expr;
        }

        public Expression Operand { get; }
        public string Operator { get; }

        public override string ToString()
        {
            return $"{Operator}({Operand})";
        }
    }
}
