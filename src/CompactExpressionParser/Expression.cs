namespace CompactExpressionParser
{
    public class Expression
    {
        public Expression(int lineNumber, int position)
        {
            LineNumber = lineNumber;
            Position = position;
        }

        public int LineNumber { get; }
        public int Position { get; }
    }
}
