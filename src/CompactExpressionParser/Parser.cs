using System;
using System.Collections.Generic;
using System.Linq;

namespace CompactExpressionParser
{
    public class Parser
    {
        private readonly HashSet<string> mUnaryOperators = new HashSet<string>();
        private readonly Dictionary<string, int> mBinaryOperators = new Dictionary<string, int>();

        private readonly string[] mAllOperators;

        public Parser(string[] unaryOperators, string[] binaryOperators)
        {
            for (int i = 0; i < unaryOperators.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(unaryOperators[i]))
                    throw new ArgumentException("The given unary operator is not valid. It must not be null or white-space.");

                if (char.IsLetter(unaryOperators[i][0]))
                    throw new ArgumentException($"The given unary operator '{unaryOperators[i]}' is not valid. It must not start with a letter.");

                mUnaryOperators.Add(unaryOperators[i]);
            }
            for (int i = 0; i < binaryOperators.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(binaryOperators[i]))
                    throw new ArgumentException("The given binary operator is not valid. It must not be null or white-space.");

                mBinaryOperators[binaryOperators[i]] = binaryOperators.Length - i;
            }

            mAllOperators = unaryOperators.Concat(binaryOperators).ToArray();
        }

        public Expression Parse(string input)
        {
            var stream = new TokenStream(input, mAllOperators);
            stream.MoveNext();
            if (stream.Current.Type == TokenType.End)
                throw new Exception("Expression must not be empty.");

            Expression expr = ParseL1(stream);
            if (stream.Current.Type != TokenType.End)
                throw new Exception($"Unexpected token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber}.");
            return expr;
        }

        private Expression ParseL1(TokenStream stream)
        {
            List<Expression> operands = new List<Expression>();
            List<KeyValuePair<string, int>> operators = new List<KeyValuePair<string, int>>();
            operands.Add(ParseL2(stream));
            while (stream.Current.Type == TokenType.Operator)
            {
                string op = stream.Current.StringValue;
                if (!mBinaryOperators.TryGetValue(op, out int precedence))
                    throw new Exception($"Unary operator '{op}' at position {stream.Current.Position} in line {stream.Current.LineNumber} cannot be used after operand.");
                operators.Add(new KeyValuePair<string, int>(op, precedence));
                stream.MoveNext();
                operands.Add(ParseL2(stream));
            }

            while (operators.Count > 0)
            {
                // find operator with highest precedence
                int maxPrecIndex = 0;
                for (int i = 1; i < operators.Count; i++)
                {
                    if (operators[i].Value > operators[maxPrecIndex].Value)
                        maxPrecIndex = i;
                }

                Expression newOperand = new BinaryOperator(operands[maxPrecIndex], operators[maxPrecIndex].Key, operands[maxPrecIndex + 1], operands[maxPrecIndex].LineNumber, operands[maxPrecIndex].Position);
                operands[maxPrecIndex] = newOperand;
                operands.RemoveAt(maxPrecIndex + 1);
                operators.RemoveAt(maxPrecIndex);
            }

            return operands[0];
        }

        private Expression ParseL2(TokenStream stream)
        {
            Expression operand = ParseL3(stream);

            // any number of trailing things
            while (true)
            {
                // invocation
                if (stream.Current.Type == TokenType.OpeningParenthesis)
                {
                    int line = stream.Current.LineNumber;
                    int pos = stream.Current.Position;
                    stream.MoveNext(); // skip '('
                    List<Expression> arguments = new List<Expression>();
                    for (; ; )
                    {
                        arguments.Add(ParseL1(stream));
                        if (stream.Current.Type == TokenType.ClosingParenthesis)
                        {
                            stream.MoveNext(); // skip ')'
                            operand = new Invocation(operand, arguments, line, pos);
                            break;
                        }
                        if (stream.Current.Type != TokenType.Comma)
                            throw new Exception($"Unexpected token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber} in invocation arguments.");
                        stream.MoveNext(); // skip ','
                    }
                }
                // subscript
                else if (stream.Current.Type == TokenType.OpeningBrackets)
                {
                    int line = stream.Current.LineNumber;
                    int pos = stream.Current.Position;
                    stream.MoveNext(); // skip '['
                    List<Expression> arguments = new List<Expression>();
                    for (; ; )
                    {
                        arguments.Add(ParseL1(stream));
                        if (stream.Current.Type == TokenType.ClosingBrackets)
                        {
                            stream.MoveNext(); // skip ']'
                            operand = new Subscript(operand, arguments, line, pos);
                            break;
                        }
                        if (stream.Current.Type != TokenType.Comma)
                            throw new Exception($"Unexpected token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber} in subscript arguments.");
                        stream.MoveNext(); // skip ','
                    }
                }
                else
                    break;
            }

            return operand;
        }

        private Expression ParseL3(TokenStream stream)
        {
            if (stream.Current.Type == TokenType.Operator && mUnaryOperators.Contains(stream.Current.StringValue))
            {
                string op = stream.Current.StringValue;
                int line = stream.Current.LineNumber;
                int pos = stream.Current.Position;
                stream.MoveNext(); // skip unary operator
                Expression expr = ParseL2(stream);
                return new UnaryOperator(op, expr, line, pos);
            }
            //if (stream.Current.Type == TokenType.Identifier && stream.Next.Type == TokenType.OpeningParenthesis)
            //{
            //    int line = stream.Current.LineNumber;
            //    int pos = stream.Current.Position;
            //    string methodName = stream.Current.StringValue;
            //    stream.MoveNext(); // goto '('
            //    stream.MoveNext(); // skip '('
            //    List<Expression> arguments = new List<Expression>();
            //    for (; ; )
            //    {
            //        arguments.Add(ParseL1(stream));
            //        if (stream.Current.Type == TokenType.ClosingParenthesis)
            //        {
            //            stream.MoveNext(); // skip ')'
            //            return new Invocation(methodName, arguments, line, pos);
            //        }
            //        if (stream.Current.Type != TokenType.Comma)
            //            throw new Exception($"Unexpected token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber} in invocation list of method '{methodName}'.");
            //        stream.MoveNext(); // skip ','
            //    }
            //}
            if (stream.Current.Type == TokenType.OpeningBrackets)
            {
                int line = stream.Current.LineNumber;
                int pos = stream.Current.Position;
                stream.MoveNext(); // skip '['
                List<Expression> items = new List<Expression>();
                for (; ; )
                {
                    items.Add(ParseL1(stream));
                    if (stream.Current.Type == TokenType.ClosingBrackets)
                    {
                        stream.MoveNext(); // skip ']'
                        return new List(items.ToArray(), line, pos);
                    }
                    if (stream.Current.Type != TokenType.Comma)
                        throw new Exception($"Unexpected token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber} between items of list expression.");
                    stream.MoveNext(); // skip ','
                }
            }
            if (stream.Current.Type == TokenType.OpeningParenthesis)
            {
                stream.MoveNext(); // skip '('
                Expression result = ParseL1(stream);
                if (stream.Current.Type != TokenType.ClosingParenthesis)
                    throw new Exception($"Expected ')', but found {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber}.");
                stream.MoveNext(); // skip ')'
                return result;
            }
            if (stream.Current.Type == TokenType.Identifier)
            {
                int line = stream.Current.LineNumber;
                int pos = stream.Current.Position;
                string value = stream.Current.StringValue;
                stream.MoveNext(); // skip identifier
                if (value == "true")
                    return new Literal(true, line, pos);
                else if (value == "false")
                    return new Literal(false, line, pos);
                else
                    return new Identifier(value, line, pos);
            }
            else if (stream.Current.Type == TokenType.StringLiteral)
            {
                int line = stream.Current.LineNumber;
                int pos = stream.Current.Position;
                object value = stream.Current.StringValue;
                stream.MoveNext(); // skip literal
                return new Literal(value, line, pos);
            }
            else if (stream.Current.Type == TokenType.NumberLiteral)
            {
                int line = stream.Current.LineNumber;
                int pos = stream.Current.Position;
                object value = stream.Current.NumberValue;
                stream.MoveNext(); // skip literal
                return new Literal(value, line, pos);
            }
            else
                throw new Exception($"Unexpected input token {stream.Current} at position {stream.Current.Position} in line {stream.Current.LineNumber}.");
        }
    }
}
