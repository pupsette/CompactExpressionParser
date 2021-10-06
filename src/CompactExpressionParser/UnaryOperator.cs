namespace CompactExpressionParser
{
    /// <summary>
    /// This class represents an unary operator in the expression tree.
    /// </summary>
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class UnaryOperator : Expression
    {
        /// <summary>
        /// Initializes an new instance of <see cref="UnaryOperator"/>.
        /// </summary>
        /// <param name="op">The name or symbol of the unary operator.</param>
        /// <param name="expr">The operand.</param>
        /// <param name="lineNumber">The line number where this operator occurred. The first line has line number 1.</param>
        /// <param name="position">The zero-based position within the line where this operator occurred.</param>
        public UnaryOperator(string op, Expression expr, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Operator = op;
            Operand = expr;
        }

        /// <summary>
        /// The operand of this unary operator.
        /// </summary>
        public Expression Operand { get; }

        /// <summary>
        /// The name or symbol of this unary operator.
        /// </summary>
        public string Operator { get; }

        /// <summary>
        /// Returns a string representation of this expression.
        /// </summary>
        /// <returns>A string representation of this expression.</returns>
        public override string ToString()
        {
            return $"{Operator}({Operand})";
        }
    }
}
