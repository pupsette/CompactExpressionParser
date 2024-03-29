﻿namespace CompactExpressionParser
{
#if COMPACTEXPRESSIONPARSER_PUBLIC
    public
#else
    internal
#endif
    sealed class MemberAccess : Expression
    {
        public MemberAccess(Expression source, string memberName, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Source = source;
            MemberName = memberName;
        }

        public Expression Source { get; }

        public string MemberName { get; }

        public override string ToString()
        {
            return Source.ToString() + "." + MemberName;
        }
    }
}
