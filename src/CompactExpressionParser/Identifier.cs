namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class Identifier : Expression
    {
        public Identifier(string identifier, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Name = identifier;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }
    }
}
