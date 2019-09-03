using System;
using System.Text;

namespace CompactExpressionParser
{
    public class TokenStream
    {
        public TokenStream(string input, string[] operators)
        {
            mInput = input;
            mOperators = operators;
            mMaxOperatorChars = 0;
            foreach (string op in operators)
            {
                if (op.Length > mMaxOperatorChars)
                    mMaxOperatorChars = op.Length;
            }
            mCurrent.Type = TokenType.End;
            mNext.Type = TokenType.End;
            mCurrentLine = 1;
            mCurrentLineStartOffset = 0;
            MoveNext();
        }

        public void MoveNext()
        {
            mCurrent = mNext;
            SkipWhitespace();

            SetLineAndPosInfoForNext();
            if (mCurrentInputOffset >= mInput.Length)
            {
                mNext.Type = TokenType.End;
                return;
            }

            char c = mInput[mCurrentInputOffset];
            if (c == '(')
            {
                mNext.Type = TokenType.OpeningParenthesis;
                mCurrentInputOffset++;
                return;
            }
            if (c == ')')
            {
                mNext.Type = TokenType.ClosingParenthesis;
                mCurrentInputOffset++;
                return;
            }
            if (c == '{')
            {
                mNext.Type = TokenType.OpeningBraces;
                mCurrentInputOffset++;
                return;
            }
            if (c == '}')
            {
                mNext.Type = TokenType.ClosingBraces;
                mCurrentInputOffset++;
                return;
            }
            if (c == '[')
            {
                mNext.Type = TokenType.OpeningBrackets;
                mCurrentInputOffset++;
                return;
            }
            if (c == ']')
            {
                mNext.Type = TokenType.ClosingBrackets;
                mCurrentInputOffset++;
                return;
            }
            if (c == '"')
            {
                mNext.StringValue = ParseStringLiteral();
                mNext.Type = TokenType.StringLiteral;
                return;
            }
            if (c == ';')
            {
                mNext.Type = TokenType.Semicolon;
                mCurrentInputOffset++;
                return;
            }
            if (c == ',')
            {
                mNext.Type = TokenType.Comma;
                mCurrentInputOffset++;
                return;
            }
            if (c == '.')
            {
                mNext.Type = TokenType.Dot;
                mCurrentInputOffset++;
                return;
            }
            if (char.IsDigit(c) || ((c == '-' || c == '+') && char.IsDigit(Lookahead()) && NumberLiteralMayFollow()))
            {
                mNext.NumberValue = ParseNumber();
                mNext.Type = TokenType.NumberLiteral;
                return;
            }
            string bestMatchingOperator = null;
            foreach (string op in mOperators)
            {
                if (OperatorMatches(op))
                {
                    if (bestMatchingOperator == null || bestMatchingOperator.Length < op.Length)
                    {
                        bestMatchingOperator = op;
                        if (bestMatchingOperator.Length == mMaxOperatorChars)
                            break;
                    }
                }
            }
            if (bestMatchingOperator != null)
            {
                mCurrentInputOffset += bestMatchingOperator.Length;
                mNext.StringValue = bestMatchingOperator;
                mNext.Type = TokenType.Operator;
                return;
            }
            if (char.IsLetter(c))
            {
                mNext.StringValue = ParseIdentifier();
                mNext.Type = TokenType.Identifier;
                return;
            }
            throw new Exception($"Unexpected character '{mInput[mCurrentInputOffset]}' in line {mNext.LineNumber} at position {mNext.Position} in string \"{mInput}\".");
        }

        private bool NumberLiteralMayFollow()
        {
            return mCurrent.Type == TokenType.End || mCurrent.Type == TokenType.Operator || mCurrent.Type == TokenType.OpeningParenthesis || mCurrent.Type == TokenType.Comma;
        }

        private void SetLineAndPosInfoForNext()
        {
            mNext.LineNumber = mCurrentLine;
            mNext.Position = mCurrentInputOffset - mCurrentLineStartOffset;
        }

        private double ParseNumber()
        {
            int startIndex = mCurrentInputOffset;
            if (mInput[mCurrentInputOffset] == '-' || mInput[mCurrentInputOffset] == '+')
                mCurrentInputOffset++;

            while (mCurrentInputOffset < mInput.Length && (char.IsDigit(mInput[mCurrentInputOffset]) || (mInput[mCurrentInputOffset] == '.' && char.IsDigit(Lookahead()))))
                mCurrentInputOffset++;
            string numberStr = mInput.Substring(startIndex, mCurrentInputOffset - startIndex);
            double result;
            if (!double.TryParse(numberStr, out result))
                throw new Exception($"Invalid number '{numberStr}' in line {mNext.LineNumber} at position {mNext.Position} in string \"{mInput}\".");
            return result;
        }

        private static bool TryHexTextToInt(string text, int start, int end, out int value)
        {
            value = 0;

            for (int i = start; i < end; i++)
            {
                char ch = text[i];
                int chValue;

                if (ch <= 57 && ch >= 48)
                {
                    chValue = ch - 48;
                }
                else if (ch <= 70 && ch >= 65)
                {
                    chValue = ch - 55;
                }
                else if (ch <= 102 && ch >= 97)
                {
                    chValue = ch - 87;
                }
                else
                {
                    value = 0;
                    return false;
                }

                value += chValue << ((end - 1 - i) * 4);
            }

            return true;
        }

        private string ParseStringLiteral()
        {
            if (mInput[mCurrentInputOffset] != '"')
                throw new InvalidOperationException("ParseStringLiteral must only be called if a string sequence has started.");

            mCurrentInputOffset++;
            StringBuilder sb = new StringBuilder();
            bool escaped = false;

            while (true)
            {
                if (mCurrentInputOffset == mInput.Length)
                    throw new Exception($"Unexpected end of input while parsing string literal starting at position {mNext.Position} in line {mNext.LineNumber}.");

                char c = mInput[mCurrentInputOffset++];
                if (escaped)
                {
                    switch (c)
                    {
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case '\\':
                        case '"':
                        case '\'':
                        case '/':
                            sb.Append(c);
                            break;
                        case 'u':

                            if (mCurrentInputOffset + 4 > mInput.Length)
                                throw new Exception("Unexpected end of input while parsing unicode escape sequence in string literal.");

                            if (!TryHexTextToInt(mInput, mCurrentInputOffset, mCurrentInputOffset + 4, out int charValue))
                                throw new Exception($"Invalid unicode escape sequence \\u{mInput.Substring(mCurrentInputOffset, 4)} in string literal at position {mCurrentInputOffset - mCurrentLineStartOffset} in line {mCurrentLine}.");

                            mCurrentInputOffset += 4;
                            sb.Append(Convert.ToChar(charValue));
                            break;
                        default:
                            throw new Exception($"Invalid escape sequence \\{c} in string literal at position {mCurrentInputOffset - mCurrentLineStartOffset} in line {mCurrentLine}.");
                    }
                    escaped = false;
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else if (c < 32)
                {
                    throw new Exception($"Control character 0x{((int)c).ToString("x").PadLeft(2, '0')} at position {mCurrentInputOffset - mCurrentLineStartOffset} in line {mCurrentLine} must be escaped in string literal with \\u{((int)c).ToString("x").PadLeft(4, '0')}.");
                }
                else if (c == '\"')
                {
                    break;
                }
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private string ParseIdentifier()
        {
            int startIndex = mCurrentInputOffset;
            while (mCurrentInputOffset < mInput.Length && (char.IsLetterOrDigit(mInput[mCurrentInputOffset]) || mInput[mCurrentInputOffset] == '_'))
                mCurrentInputOffset++;
            return mInput.Substring(startIndex, mCurrentInputOffset - startIndex);
        }

        private void SkipWhitespace()
        {
            while (mCurrentInputOffset < mInput.Length && char.IsWhiteSpace(mInput[mCurrentInputOffset]))
            {
                if (mInput[mCurrentInputOffset] == '\n')
                {
                    mCurrentLine++;
                    mCurrentLineStartOffset = mCurrentInputOffset + 1;
                }
                mCurrentInputOffset++;
            }
        }

        private char Lookahead()
        {
            if (mCurrentInputOffset + 1 < mInput.Length)
                return mInput[mCurrentInputOffset + 1];
            else
                return char.MinValue;
        }

        private bool OperatorMatches(string seq)
        {
            for (int i = 0; i < seq.Length; i++)
            {
                int inputIndex = mCurrentInputOffset + i;
                if (inputIndex >= mInput.Length)
                    return false;
                if (mInput[inputIndex] != seq[i])
                    return false;
            }
            int nextIndex = mCurrentInputOffset + seq.Length;
            if (nextIndex < mInput.Length && char.IsLetter(mInput[nextIndex]) && char.IsLetterOrDigit(seq[seq.Length - 1]))
                return false;
            return true;
        }

        private readonly string[] mOperators;
        private readonly string mInput;
        private int mCurrentInputOffset;
        private int mCurrentLine;
        private int mCurrentLineStartOffset;
        private int mMaxOperatorChars;
        private Token mCurrent, mNext;

        public Token Current { get { return mCurrent; } }
        public Token Next { get { return mNext; } }
    }
}
