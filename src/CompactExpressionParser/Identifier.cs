namespace CompactExpressionParser
{
    /// <summary>
    /// This class represents an identifier.
    /// </summary>
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class Identifier : Expression
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Identifier"/>.
        /// </summary>
        /// <param name="identifier">The name of the identifier.</param>
        /// <param name="lineNumber">The line number where this identifier occurred. The first line has line number 1.</param>
        /// <param name="position">The zero-based position within the line where this identifier occurred.</param>
        public Identifier(string identifier, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Name = identifier;
        }

        /// <summary>
        /// The name of the identifier.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Returns a string representation of this expression.
        /// </summary>
        /// <returns>A string representation of this expression.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
