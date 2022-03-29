using NUnit.Framework;
using System;
using System.Linq;

namespace CompactExpressionParser.Tests
{
    [TestFixture]
    public class ParseExpressionTests
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
            var tree = PARSER.ParseExpression(input);
            string rewritten = Rewrite(tree);
            Console.WriteLine(rewritten);
            Assert.That(rewritten, Is.EqualTo(expectedTree));
        }

        [TestCase("-1.25", -1.25)]
        [TestCase("-1.00", -1)]
        [TestCase("100.5", 100.5)]
        [TestCase("+100.5", 100.5)]
        public void ParseToFloatingPoint(string input, double expected)
        {
            var tree = PARSER.ParseExpression(input);
            Assert.That((tree as Literal).Value, Is.InstanceOf<double>());
            Assert.That((tree as Literal).Value, Is.EqualTo(expected));
        }

        [TestCase("-100", -100)]
        [TestCase("+1", 1)]
        [TestCase("10058746871235", 10058746871235)]
        public void ParseToInteger(string input, long expected)
        {
            var tree = PARSER.ParseExpression(input);
            Assert.That((tree as Literal).Value, Is.InstanceOf<long>());
            Assert.That((tree as Literal).Value, Is.EqualTo(expected));
        }

        [TestCase("=|==","5=5", "LT5;LT5;BO=")]
        [TestCase("=|==", "5=5 == 5", "LT5;LT5;BO=;LT5;BO==")]
        [TestCase("=|==", "5=5==5", "LT5;LT5;BO=;LT5;BO==")]
        [TestCase("+|-|==|and|or", "andor", "IDandor")]
        [TestCase("+|-|==|and|or", "id.andy", "IDid;MAandy")]
        [TestCase("+|-|==|and|or", "id+id", "IDid;IDid;BO+")]
        [TestCase("+|-|==|and|or", "5 == 5 and6+6==12 or true", "LT5;LT5;BO==;LT6;LT6;BO+;LT12;BO==;BOand;LTTrue;BOor")]
        public void OperatorMatching(string operatorsStr, string input, string expectedTree)
        {
            var parser = new Parser(null, operatorsStr.Split('|'));
            var tree = parser.ParseExpression(input);
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
            TestDelegate parseAction = () => PARSER.ParseExpression(input);
            Assert.That(parseAction, Throws.Exception);
        }

        [TestCase("=|==", "5===5")]
        [TestCase("+|-|==|and|or", "1 andor 2")]
        [TestCase("+|-|==|and|or", "id andid")]
        [TestCase("+|-|==|and|or", "id.and")]
        public void Parsing_Should_Fail_With_Special_Operators(string operatorsStr, string input)
        {
            var parser = new Parser(null, operatorsStr.Split('|'));
            TestDelegate parseAction = () => parser.ParseExpression(input);
            Assert.That(parseAction, Throws.Exception);
        }

        internal static string Rewrite(Expression ex)
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
