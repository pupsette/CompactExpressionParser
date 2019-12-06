namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class ConditionalStatement : Statement
    {
        public ConditionalStatement(Expression condition, Statement trueStatement, Statement falseStatement)
        {
            Condition = condition;
            True = trueStatement;
            False = falseStatement;
        }

        public Expression Condition { get; }
        public Statement True { get; }
        public Statement False { get; }
    }
}
