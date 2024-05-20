using System.Collections.Generic;

namespace Dt.Toolkit.Sql
{
    public class FormatConfig
    {
        public static readonly string DefaultIndent = "    ";
        public static readonly int DefaultColumnMaxLength = 50;

        public readonly string indent;
        public readonly int maxColumnLength;
        public readonly Params parameters;
        public readonly bool uppercase;
        public readonly int linesBetweenQueries;
        public readonly bool skipWhitespaceNearBlockParentheses;

        public FormatConfig(
            string indent,
            int maxColumnLength,
            Params parameters,
            bool uppercase,
            int linesBetweenQueries,
            bool skipWhitespaceNearBlockParentheses)
        {
            this.indent = indent;
            this.maxColumnLength = maxColumnLength;
            this.parameters = parameters == null ? Params.Empty : parameters;
            this.uppercase = uppercase;
            this.linesBetweenQueries = linesBetweenQueries;
            this.skipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
        }

        public static FormatConfigBuilder Builder()
        {
            return new FormatConfigBuilder();
        }

        public class FormatConfigBuilder
        {
            private string indent = DefaultIndent;
            private int maxColumnLength = DefaultColumnMaxLength;
            private Params parameters;
            private bool uppercase = true;
            private int linesBetweenQueries;
            private bool skipWhitespaceNearBlockParentheses;

            public FormatConfigBuilder()
            {
            }

            public FormatConfigBuilder Indent(string indent)
            {
                this.indent = indent;
                return this;
            }

            public FormatConfigBuilder MaxColumnLength(int maxColumnLength)
            {
                this.maxColumnLength = maxColumnLength;
                return this;
            }

            public FormatConfigBuilder Params(Params parameters)
            {
                this.parameters = parameters;
                return this;
            }

            public FormatConfigBuilder Params<T>(Dictionary<string, T> parameters)
            {
                return Params(Sql.Params.Of(parameters));
            }

            public FormatConfigBuilder Params<T>(List<T> parameters)
            {
                return Params(Sql.Params.Of(parameters));
            }

            public FormatConfigBuilder Uppercase(bool uppercase)
            {
                this.uppercase = uppercase;
                return this;
            }

            public FormatConfigBuilder LinesBetweenQueries(int linesBetweenQueries)
            {
                this.linesBetweenQueries = linesBetweenQueries;
                return this;
            }

            public FormatConfigBuilder SkipWhitespaceNearBlockParentheses(bool skipWhitespaceNearBlockParentheses)
            {
                this.skipWhitespaceNearBlockParentheses = skipWhitespaceNearBlockParentheses;
                return this;
            }

            public FormatConfig Build()
            {
                return new FormatConfig(
                    indent,
                    maxColumnLength,
                    parameters,
                    uppercase,
                    linesBetweenQueries,
                    skipWhitespaceNearBlockParentheses);
            }
        }
    }
}
