using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Dt.Toolkit.Sql
{
    public class AbstractFormatter : IDialectConfigurator
    {
        private readonly FormatConfig cfg;
        private readonly Indentation indentation;
        private readonly InlineBlock inlineBlock;
        private readonly Params parameters;
        protected Token previousReservedToken;
        private JSLikeList<Token> tokens;
        private int index;

        public AbstractFormatter(FormatConfig cfg)
        {
            this.cfg = cfg;
            indentation = new Indentation(cfg.indent);
            inlineBlock = new InlineBlock(cfg.maxColumnLength);
            parameters = cfg.parameters;
            previousReservedToken = null;
            index = 0;
        }

        public Tokenizer Tokenizer()
        {
            return new Tokenizer(DoDialectConfig());
        }

        protected virtual Token TokenOverride(Token token)
        {
            return token;
        }

        public string Format(string query)
        {
            tokens = Tokenizer().Tokenize(query);
            var formattedQuery = GetFormattedQueryFromTokens();

            return formattedQuery.Trim();
        }

        private string GetFormattedQueryFromTokens()
        {
            var formattedQuery = "";

            var _index = -1;
            foreach (Token t in tokens)
            {
                index = ++_index;

                var token = TokenOverride(t);

                if (token.type == TokenTypes.LINE_COMMENT)
                {
                    formattedQuery = FormatLineComment(token, formattedQuery);
                }
                else if (token.type == TokenTypes.BLOCK_COMMENT)
                {
                    formattedQuery = FormatBlockComment(token, formattedQuery);
                }
                else if (token.type == TokenTypes.RESERVED_TOP_LEVEL)
                {
                    formattedQuery = FormatToplevelReservedWord(token, formattedQuery);
                    previousReservedToken = token;
                }
                else if (token.type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT)
                {
                    formattedQuery = FormatTopLevelReservedWordNoIndent(token, formattedQuery);
                    previousReservedToken = token;
                }
                else if (token.type == TokenTypes.RESERVED_NEWLINE)
                {
                    formattedQuery = FormatNewlineReservedWord(token, formattedQuery);
                    previousReservedToken = token;
                }
                else if (token.type == TokenTypes.RESERVED)
                {
                    formattedQuery = FormatWithSpaces(token, formattedQuery);
                    previousReservedToken = token;
                }
                else if (token.type == TokenTypes.OPEN_PAREN)
                {
                    formattedQuery = FormatOpeningParentheses(token, formattedQuery);
                }
                else if (token.type == TokenTypes.CLOSE_PAREN)
                {
                    formattedQuery = FormatClosingParentheses(token, formattedQuery);
                }
                else if (token.type == TokenTypes.PLACEHOLDER)
                {
                    formattedQuery = FormatPlaceholder(token, formattedQuery);
                }
                else if (token.value.Equals(","))
                {
                    formattedQuery = FormatComma(token, formattedQuery);
                }
                else if (token.value.Equals(":"))
                {
                    formattedQuery = FormatWithSpaceAfter(token, formattedQuery);
                }
                else if (token.value.Equals("."))
                {
                    formattedQuery = FormatWithoutSpaces(token, formattedQuery);
                }
                else if (token.value.Equals(";"))
                {
                    formattedQuery = FormatQuerySeparator(token, formattedQuery);
                }
                else
                {
                    formattedQuery = FormatWithSpaces(token, formattedQuery);
                }
            }

            return formattedQuery;
        }

        private string FormatLineComment(Token token, string query)
        {
            return AddNewline(query + Show(token));
        }

        private string FormatBlockComment(Token token, string query)
        {
            return AddNewline(AddNewline(query) + IndentComment(token.value));
        }

        private string IndentComment(string comment)
        {
            return comment.Replace("\n", "\n" + indentation.GetIndent());
        }

        private string FormatTopLevelReservedWordNoIndent(Token token, string query)
        {
            indentation.DecreaseTopLevel();
            query = AddNewline(query) + EqualizeWhitespace(Show(token));
            return AddNewline(query);
        }

        private string FormatToplevelReservedWord(Token token, string query)
        {
            indentation.DecreaseTopLevel();

            query = AddNewline(query);

            indentation.IncreaseTopLevel();

            query += EqualizeWhitespace(Show(token));
            return AddNewline(query);
        }

        private string FormatNewlineReservedWord(Token token, string query)
        {
            if (Token.IsAnd(token) && Token.IsBetween(TokenLookBehind(2)))
            {
                return FormatWithSpaces(token, query);
            }

            return AddNewline(query) + EqualizeWhitespace(Show(token)) + " ";
        }

        private static string EqualizeWhitespace(string str)
        {
            return Regex.Replace(str, @"\s+", " ");
        }

        private static readonly HashSet<TokenTypes> PreserveWhitespaceFor =
            new HashSet<TokenTypes> {
                TokenTypes.OPEN_PAREN,
                TokenTypes.LINE_COMMENT,
                TokenTypes.OPERATOR,
                TokenTypes.RESERVED_NEWLINE};

        private string FormatOpeningParentheses(Token token, string query)
        {
            if (string.IsNullOrEmpty(token.whitespaceBefore)
                && (TokenLookBehind() == default || !PreserveWhitespaceFor.Contains(TokenLookBehind().type)))
            {
                query = query.TrimEnd();
            }

            query += Show(token);

            inlineBlock.BeginIfPossible(tokens, index);

            if (!inlineBlock.IsActive())
            {
                indentation.IncreaseBlockLevel();
                if (!cfg.skipWhitespaceNearBlockParentheses)
                {
                    query = AddNewline(query);
                }
            }

            return query;
        }

        private string FormatClosingParentheses(Token token, string query)
        {
            if (inlineBlock.IsActive())
            {
                inlineBlock.End();
                return FormatWithSpaceAfter(token, query);
            }
            else
            {
                indentation.DecreaseBlockLevel();

                if (!cfg.skipWhitespaceNearBlockParentheses)
                {
                    return FormatWithSpaces(token, AddNewline(query));
                }

                return FormatWithoutSpaces(token, query);
            }
        }

        private string FormatPlaceholder(Token token, string query)
        {
            return query + parameters.Get(token) + " ";
        }

        private string FormatComma(Token token, string query)
        {
            query = query.TrimEnd() + Show(token) + " ";
            return inlineBlock.IsActive() || Token.IsLimit(previousReservedToken) ? query : AddNewline(query);
        }

        private string FormatWithSpaceAfter(Token token, string query)
        {
            return query.TrimEnd() + Show(token) + " ";
        }

        private string FormatWithoutSpaces(Token token, string query)
        {
            return query.TrimEnd() + Show(token);
        }

        private string FormatWithSpaces(Token token, string query)
        {
            return query + Show(token) + " ";
        }

        private string FormatQuerySeparator(Token token, string query)
        {
            indentation.ResetIndentation();
            return query.TrimEnd()
                + Show(token)
                + Utils.Repeat("\n", cfg.linesBetweenQueries == default ? 1 : cfg.linesBetweenQueries);
        }

        private string Show(Token token)
        {
            if (cfg.uppercase
                && (token.type == TokenTypes.RESERVED
                    || token.type == TokenTypes.RESERVED_TOP_LEVEL
                    || token.type == TokenTypes.RESERVED_TOP_LEVEL_NO_INDENT
                    || token.type == TokenTypes.RESERVED_NEWLINE
                    || token.type == TokenTypes.OPEN_PAREN
                    || token.type == TokenTypes.CLOSE_PAREN))
            {
                return token.value.ToUpper();
            }

            return token.value;
        }

        private string AddNewline(string query)
        {
            query = query.TrimEnd();
            if (!query.EndsWith("\n"))
            {
                query += "\n";
            }

            return query + indentation.GetIndent();
        }

        protected Token TokenLookBehind()
        {
            return TokenLookBehind(1);
        }

        protected Token TokenLookBehind(int n)
        {
            return tokens.Get(index - n);
        }

        protected Token TokenLookAhead()
        {
            return TokenLookAhead(1);
        }

        protected Token TokenLookAhead(int n)
        {
            return tokens.Get(index + n);

        }

        public virtual DialectConfig DoDialectConfig()
        {
            return doDialectConfigFunc.Invoke();
        }

        public Func<DialectConfig> doDialectConfigFunc;
    }
}
