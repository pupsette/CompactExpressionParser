using System.Collections.Generic;

namespace CompactExpressionParser
{
    public class Subscript : Expression
    {
        public Subscript(Expression indexable, List<Expression> arguments, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Indexable = indexable;
            Arguments = arguments.ToArray();
        }

        public Expression Indexable { get; }

        public Expression[] Arguments { get; }

        public override string ToString()
        {
            return Indexable.ToString() + "[" + string.Join<Expression>(", ", Arguments) + "]";
        }
    }
}
