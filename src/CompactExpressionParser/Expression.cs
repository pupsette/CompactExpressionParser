namespace CompactExpressionParser
{
    /// <summary>
    /// This is the base class for expressions.
    /// </summary>
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    abstract class Expression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Expression"/>.
        /// </summary>
        /// <param name="lineNumber">The line number where this expression occurred.</param>
        /// <param name="position">The character position within the line where this expression occurred.</param>
        protected Expression(int lineNumber, int position)
        {
            LineNumber = lineNumber;
            Position = position;
        }

        /// <summary>
        /// The line number where this expression occurred. The first line has line number 1.
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        /// The zero-based position within the line where this expression occurred.
        /// </summary>
        public int Position { get; }
    }
}
