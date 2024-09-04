using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dt.Toolkit.Sql
{
    public class Tokenizer
    {
        private readonly Regex NUMBER_PATTERN;
        private readonly Regex OPERATOR_PATTERN;

        private readonly Regex BLOCK_COMMENT_PATTERN;
        private readonly Regex LINE_COMMENT_PATTERN;

        private readonly Regex RESERVED_TOP_LEVEL_PATTERN;
        private readonly Regex RESERVED_TOP_LEVEL_NO_INDENT_PATTERN;
        private readonly Regex RESERVED_NEWLINE_PATTERN;
        private readonly Regex RESERVED_PLAIN_PATTERN;

        private readonly Regex WORD_PATTERN;
        private readonly Regex STRING_PATTERN;

        private readonly Regex OPEN_PAREN_PATTERN;
        private readonly Regex CLOSE_PAREN_PATTERN;

        private readonly Regex INDEXED_PLACEHOLDER_PATTERN;
        private readonly Regex IDENT_NAMED_PLACEHOLDER_PATTERN;
        private readonly Regex STRING_NAMED_PLACEHOLDER_PATTERN;

        public Tokenizer(DialectConfig cfg)
        {
            NUMBER_PATTERN = new Regex(
                "^((-\\s*)?[0-9]+(\\.[0-9]+)?([eE]-?[0-9]+(\\.[0-9]+)?)?|0x[0-9a-fA-F]+|0b[01]+)\\b");

            OPERATOR_PATTERN = new Regex(
                RegexUtil.CreateOperatorRegex(
                    new JSLikeList<string>(new List<string> { "<>", "<=", ">=" }).With(cfg.operators)));

            BLOCK_COMMENT_PATTERN = new Regex("^(/\\*(?s).*?(?:\\*/|$))");
            LINE_COMMENT_PATTERN = new Regex(
                RegexUtil.CreateLineCommentRegex(new JSLikeList<string>(cfg.lineCommentTypes)));

            RESERVED_TOP_LEVEL_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedTopLevelWords)));
            RESERVED_TOP_LEVEL_NO_INDENT_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedTopLevelWordsNoIndent)));
            RESERVED_NEWLINE_PATTERN =
                new Regex(
                    RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedNewlineWords)));
            RESERVED_PLAIN_PATTERN =
                new Regex(RegexUtil.CreateReservedWordRegex(new JSLikeList<string>(cfg.reservedWords)));

            WORD_PATTERN =
                new Regex(RegexUtil.CreateWordRegex(new JSLikeList<string>(cfg.specialWordChars)));
            STRING_PATTERN =
                new Regex(RegexUtil.CreateStringRegex(new JSLikeList<string>(cfg.stringTypes)));

            OPEN_PAREN_PATTERN =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.openParens)));
            CLOSE_PAREN_PATTERN =
                new Regex(RegexUtil.CreateParenRegex(new JSLikeList<string>(cfg.closeParens)));

            INDEXED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.indexedPlaceholderTypes), "[0-9]*");
            IDENT_NAMED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.namedPlaceholderTypes), "[a-zA-Z0-9._$]+");
            STRING_NAMED_PLACEHOLDER_PATTERN =
                RegexUtil.CreatePlaceholderRegexPattern(
                    new JSLikeList<string>(cfg.namedPlaceholderTypes),
                    RegexUtil.CreateStringPattern(new JSLikeList<string>(cfg.stringTypes)));
        }

        public JSLikeList<Token> Tokenize(string input)
        {
            var tokens = new List<Token>();
            Token token = null;

            while (!string.IsNullOrEmpty(input))
            {
                var findBeforeWhitespace = FindBeforeWhitespace(input);
                var whitespaceBefore = findBeforeWhitespace[0];
                input = findBeforeWhitespace[1];

                if (!string.IsNullOrEmpty(input))
                {
                    token = GetNextToken(input, token);
                    input = input.Substring(token.value.Length);
                    tokens.Add(token.WithWhitespaceBefore(whitespaceBefore));
                }
            }

            return new JSLikeList<Token>(tokens);
        }

        private static string[] FindBeforeWhitespace(string input)
        {
            var index = input.TakeWhile(char.IsWhiteSpace).Count();
            return new[] { input.Substring(0, index), input.Substring(index) };
        }

        private Token GetNextToken(string input, Token previousToken)
        {
            return Utils.FirstNotnull(
                () => GetCommentToken(input),
                () => GetStringToken(input),
                () => GetOpenParenToken(input),
                () => GetCloseParenToken(input),
                () => GetPlaceholderToken(input),
                () => GetNumberToken(input),
                () => GetReservedWordToken(input, previousToken),
                () => GetWordToken(input),
                () => GetOperatorToken(input));
        }

        private Token GetCommentToken(string input)
        {
            return Utils.FirstNotnull(
                () => GetLineCommentToken(input),
                () => GetBlockCommentToken(input));
        }

        private Token GetLineCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.LINE_COMMENT, LINE_COMMENT_PATTERN);
        }

        private Token GetBlockCommentToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.BLOCK_COMMENT, BLOCK_COMMENT_PATTERN);
        }

        private Token GetStringToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.STRING, STRING_PATTERN);
        }

        private Token GetOpenParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPEN_PAREN, OPEN_PAREN_PATTERN);
        }

        private Token GetCloseParenToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.CLOSE_PAREN, CLOSE_PAREN_PATTERN);
        }

        private Token GetPlaceholderToken(string input)
        {
            return Utils.FirstNotnull(
                () => GetIdentNamedPlaceholderToken(input),
                () => GetStringNamedPlaceholderToken(input),
                () => GetIndexedPlaceholderToken(input));
        }

        private Token GetIdentNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, IDENT_NAMED_PLACEHOLDER_PATTERN, v => v.Substring(1));
        }

        private Token GetStringNamedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input,
                STRING_NAMED_PLACEHOLDER_PATTERN,
                v => GetEscapedPlaceholderKey(
                    v.Substring(2, v.Length - 3), v.Substring(v.Length - 1)));
        }

        private Token GetIndexedPlaceholderToken(string input)
        {
            return GetPlaceholderTokenWithKey(
                input, INDEXED_PLACEHOLDER_PATTERN, v => v.Substring(1));
        }

        private static Token GetPlaceholderTokenWithKey(string input, Regex regex, Func<string, string> parseKey)
        {
            var token = GetTokenOnFirstMatch(input, TokenTypes.PLACEHOLDER, regex);
            return token?.WithKey(parseKey.Invoke(token.value));
        }

        private static string GetEscapedPlaceholderKey(string key, string quoteChar)
        {
            return key.Replace(RegexUtil.EscapeRegExp("\\") + quoteChar, quoteChar);
        }

        private Token GetNumberToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.NUMBER, NUMBER_PATTERN);
        }

        private Token GetOperatorToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.OPERATOR, OPERATOR_PATTERN);
        }

        private Token GetReservedWordToken(string input, Token previousToken)
        {
            if (previousToken?.value != null && previousToken.value.Equals("."))
            {
                return null;
            }

            return Utils.FirstNotnull(
                () => GetToplevelReservedToken(input),
                () => GetNewlineReservedToken(input),
                () => GetTopLevelReservedTokenNoIndent(input),
                () => GetPlainReservedToken(input));
        }

        private Token GetToplevelReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL, RESERVED_TOP_LEVEL_PATTERN);
        }

        private Token GetNewlineReservedToken(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_NEWLINE, RESERVED_NEWLINE_PATTERN);
        }

        private Token GetTopLevelReservedTokenNoIndent(string input)
        {
            return GetTokenOnFirstMatch(
                input, TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT, RESERVED_TOP_LEVEL_NO_INDENT_PATTERN);
        }

        private Token GetPlainReservedToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.RESERVED, RESERVED_PLAIN_PATTERN);
        }

        private Token GetWordToken(string input)
        {
            return GetTokenOnFirstMatch(input, TokenTypes.WORD, WORD_PATTERN);
        }

        private static string GetFirstMatch(string input, Regex regex)
        {
            return regex?.Match(input).Value ?? string.Empty;
        }

        private static Token GetTokenOnFirstMatch(string input, TokenTypes type, Regex regex)
        {
            var match = GetFirstMatch(input, regex);
            return match.Equals(string.Empty) ? default : new Token(type, match);
        }
    }
}
