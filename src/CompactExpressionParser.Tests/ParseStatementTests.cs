using NUnit.Framework;
using System;
using System.Linq;

namespace CompactExpressionParser.Tests
{
    [TestFixture]
    public class ParseStatementTests
    {
        private static readonly Parser PARSER = new Parser(new[] { "!", "++" }, new[] { "*", "+", "->", "-", "IN", "=" });

        [TestCase("{ 5; }", "{{LT5;}}")]
        [TestCase("t = 4; t = t + 3;", "{IDt;LT4;BO=;IDt;IDt;LT3;BO+;BO=;}")]
        public void Parse(string input, string expectedRewritten)
        {
            var tree = PARSER.ParseStatements(input);
            string rewritten = Rewrite(tree);
            Console.WriteLine(rewritten);
            Assert.That(rewritten, Is.EqualTo(expectedRewritten));
        }

        internal static string Rewrite(Statement ex)
        {
            switch (ex)
            {
                case SingleStatement single:
                    return ParseExpressionTests.Rewrite(single.Expression) + ";";
                case BlockStatement block:
                    return "{" + string.Concat(block.Statements.Select(Rewrite)) + "}";
                case ConditionalStatement iff:
                    return "if(" + iff.Condition + ")" + Rewrite(iff.True) + "else" + (iff.False != null ? Rewrite(iff.False) : "{}");
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
