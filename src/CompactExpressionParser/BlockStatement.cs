namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class BlockStatement : Statement
    {
        public BlockStatement(Statement[] statements)
        {
            Statements = statements;
        }

        public Statement[] Statements { get; }
    }
}
