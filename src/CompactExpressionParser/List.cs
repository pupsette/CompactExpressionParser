namespace CompactExpressionParser
{
    public class List : Expression
    {
        public List(Expression[] items, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Items = items;
        }

        public Expression[] Items { get; }

        public override string ToString()
        {
            return "List [" + string.Join<Expression>(", ", Items) + "]";
        }
    }
}
