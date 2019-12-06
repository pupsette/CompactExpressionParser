namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class SingleStatement : Statement
    {
        public SingleStatement(Expression expression)
        {
            Expression = expression;
        }

        public Expression Expression { get; }
    }
}
