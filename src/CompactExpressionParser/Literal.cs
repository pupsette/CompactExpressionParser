namespace CompactExpressionParser
{
    public class Literal : Expression
    {
        public Literal(object value, int lineNumber, int position)
            : base(lineNumber, position)
        {
            Value = value;
        }

        public object Value { get; }

        public override string ToString()
        {
            if (Value is string)
                return EncodeStringLiteral((string)Value);
            return Value?.ToString();
        }

        public static string EncodeStringLiteral(string value)
        {
            return "\"" + value.Replace("\t", "\\t").Replace("\n", "\\n").Replace("\"", "\\\"").Replace("\r", "\\r") + "\"";
        }
    }
}
