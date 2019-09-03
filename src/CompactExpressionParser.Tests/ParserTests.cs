using NUnit.Framework;
using System;
using System.Linq;

namespace CompactExpressionParser.Tests
{
    [TestFixture]
    public class ParserTests
    {
        private static readonly Parser PARSER = new Parser(new[] { "!", "++" }, new[] { "*", "+", "->", "-", "IN" });

        [TestCase("\"\"", "LT")]
        [TestCase("\"\"+5", "LT;LT5;BO+")]
        [TestCase("\"\"+-5", "LT;LT-5;BO+")]
        [TestCase("\"\"+ +5", "LT;LT5;BO+")]
        [TestCase("\"\"+ ++5", "LT;LT5;UO++;BO+")]
        [TestCase("\"\" -> Whatsoever", "LT;IDWhatsoever;BO->")]
        [TestCase("ToString()", "IDToString()")]
        [TestCase(" ToString (  \t )", "IDToString()")]
        [TestCase("5(what)", "LT5(IDwhat)")]
        [TestCase("5(what).ToString()", "LT5(IDwhat);MAToString()")]
        [TestCase("status IN [1,2,3 , 9]", "IDstatus;LS[LT1,LT2,LT3,LT9];BOIN")]
        [TestCase("[var1, 7+1, [1,2,3]]", "LS[IDvar1,LT7;LT1;BO+,LS[LT1,LT2,LT3]]")]
        public void Parse(string input, string expectedTree)
        {
            var tree = PARSER.Parse(input);
            string rewritten = Rewrite(tree);
            Console.WriteLine(rewritten);
            Assert.That(rewritten, Is.EqualTo(expectedTree));
        }

        [TestCase("-")]
        [TestCase("")]
        [TestCase("\"sdasde'")]
        [TestCase("\"sdasde")]
        public void Parsing_Should_Fail(string input)
        {
            TestDelegate parseAction = () => PARSER.Parse(input);
            Assert.That(parseAction, Throws.Exception);
        }

        private static string Rewrite(Expression ex)
        {
            switch (ex)
            {
                case Literal literal:
                    return "LT" + literal.Value;
                case Identifier identifier:
                    return "ID" + identifier.Name;
                case List list:
                    return "LS[" + string.Join(",", list.Items.Select(Rewrite)) + "]";
                case BinaryOperator binOp:
                    return Rewrite(binOp.Operand1) + ";" + Rewrite(binOp.Operand2) + ";" + "BO" + binOp.Operator;
                case UnaryOperator unOp:
                    return Rewrite(unOp.Operand) + ";" + "UO" + unOp.Operator;
                case Invocation callOp:
                    return Rewrite(callOp.Callable) + "(" + string.Join(",", callOp.Arguments.Select(Rewrite)) + ")";
                case Subscript subsc:
                    return Rewrite(subsc.Indexable) + "[" + string.Join(",", subsc.Arguments.Select(Rewrite)) + "]";
                case MemberAccess member:
                    return Rewrite(member.Source) + ";MA" + member.MemberName;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
