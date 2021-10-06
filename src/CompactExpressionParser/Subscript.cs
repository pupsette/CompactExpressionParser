using System.Collections.Generic;

namespace CompactExpressionParser
{
    /// <summary>
    /// This class represents a subscript expression. The syntax of a subscript
    /// expression is using brackets like this: <c>expression[otherExpression]</c>.
    /// </summary>
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class Subscript : Expression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Subscript"/>.
        /// </summary>
        /// <param name="indexable">The expression at the left side of the brackets.</param>
        /// <param name="arguments">The argument expressions inside the brackets.</param>
        /// <param name="lineNumber">The line number where this operator occurred. The first line has line number 1.</param>
        /// <param name="position">The zero-based position within the line where this operator occurred.</param>
        public Subscript(Expression indexable, List<Expression> arguments, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Indexable = indexable;
            Arguments = arguments.ToArray();
        }

        /// <summary>
        /// The expression at the left side of the brackets.
        /// </summary>
        public Expression Indexable { get; }

        /// <summary>
        /// The argument expressions inside the brackets.
        /// </summary>
        public Expression[] Arguments { get; }

        /// <summary>
        /// Returns a string representation of this expression.
        /// </summary>
        /// <returns>A string representation of this expression.</returns>
        public override string ToString()
        {
            return Indexable.ToString() + "[" + string.Join<Expression>(", ", Arguments) + "]";
        }
    }
}
