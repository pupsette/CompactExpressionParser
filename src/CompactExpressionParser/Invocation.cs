using System.Collections.Generic;

namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class Invocation : Expression
    {
        public Invocation(Expression callable, List<Expression> arguments, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Callable = callable;
            Arguments = arguments.ToArray();
        }

        public Expression Callable { get; }

        public Expression[] Arguments { get; }

        public override string ToString()
        {
            return Callable.ToString() + "(" + string.Join<Expression>(", ", Arguments) + ")";
        }
    }
}
