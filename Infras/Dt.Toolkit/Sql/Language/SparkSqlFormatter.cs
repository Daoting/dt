using System.Collections.Generic;

namespace Dt.Toolkit.Sql
{
    class SparkSqlFormatter : AbstractFormatter
    {
        private static readonly List<string> ReservedWords = new List<string>{
        "ALL",
        "ALTER",
        "ANALYSE",
        "ANALYZE",
        "ARRAY_ZIP",
        "ARRAY",
        "AS",
        "ASC",
        "AVG",
        "BETWEEN",
        "CASCADE",
        "CASE",
        "CAST",
        "COALESCE",
        "COLLECT_LIST",
        "COLLECT_SET",
        "COLUMN",
        "COLUMNS",
        "COMMENT",
        "CONSTRAINT",
        "CONTAINS",
        "CONVERT",
        "COUNT",
        "CUME_DIST",
        "CURRENT ROW",
        "CURRENT_DATE",
        "CURRENT_TIMESTAMP",
        "DATABASE",
        "DATABASES",
        "DATE_ADD",
        "DATE_SUB",
        "DATE_TRUNC",
        "DAY_HOUR",
        "DAY_MINUTE",
        "DAY_SECOND",
        "DAY",
        "DAYS",
        "DECODE",
        "DEFAULT",
        "DELETE",
        "DENSE_RANK",
        "DESC",
        "DESCRIBE",
        "DISTINCT",
        "DISTINCTROW",
        "DIV",
        "DROP",
        "ELSE",
        "ENCODE",
        "END",
        "EXISTS",
        "EXPLAIN",
        "EXPLODE_OUTER",
        "EXPLODE",
        "FILTER",
        "FIRST_VALUE",
        "FIRST",
        "FIXED",
        "FLATTEN",
        "FOLLOWING",
        "FROM_UNIXTIME",
        "FULL",
        "GREATEST",
        "GROUP_CONCAT",
        "HOUR_MINUTE",
        "HOUR_SECOND",
        "HOUR",
        "HOURS",
        "IF",
        "IFNULL",
        "IN",
        "INSERT",
        "INTERVAL",
        "INTO",
        "IS",
        "LAG",
        "LAST_VALUE",
        "LAST",
        "LEAD",
        "LEADING",
        "LEAST",
        "LEVEL",
        "LIKE",
        "MAX",
        "MERGE",
        "MIN",
        "MINUTE_SECOND",
        "MINUTE",
        "MONTH",
        "NATURAL",
        "NOT",
        "NOW()",
        "NTILE",
        "NULL",
        "NULLIF",
        "OFFSET",
        "ON DELETE",
        "ON UPDATE",
        "ON",
        "ONLY",
        "OPTIMIZE",
        "OVER",
        "PERCENT_RANK",
        "PRECEDING",
        "RANGE",
        "RANK",
        "REGEXP",
        "RENAME",
        "RLIKE",
        "ROW",
        "ROWS",
        "SECOND",
        "SEPARATOR",
        "SEQUENCE",
        "SIZE",
        "STRING",
        "STRUCT",
        "SUM",
        "TABLE",
        "TABLES",
        "TEMPORARY",
        "THEN",
        "TO_DATE",
        "TO_JSON",
        "TO",
        "TRAILING",
        "TRANSFORM",
        "TRUE",
        "TRUNCATE",
        "TYPE",
        "TYPES",
        "UNBOUNDED",
        "UNIQUE",
        "UNIX_TIMESTAMP",
        "UNLOCK",
        "UNSIGNED",
        "USING",
        "VARIABLES",
        "VIEW",
        "WHEN",
        "WITH",
        "YEAR_MONTH"};

        private static readonly List<string> ReservedTopLevelWords =
            new List<string>{
                "ADD",
                "AFTER",
                "ALTER COLUMN",
                "ALTER DATABASE",
                "ALTER SCHEMA",
                "ALTER TABLE",
                "CLUSTER BY",
                "CLUSTERED BY",
                "DELETE FROM",
                "DISTRIBUTE BY",
                "FROM",
                "GROUP BY",
                "HAVING",
                "INSERT INTO",
                "INSERT",
                "LIMIT",
                "OPTIONS",
                "ORDER BY",
                "PARTITION BY",
                "PARTITIONED BY",
                "RANGE",
                "ROWS",
                "SELECT",
                "SET CURRENT SCHEMA",
                "SET SCHEMA",
                "SET",
                "TBLPROPERTIES",
                "UPDATE",
                "USING",
                "VALUES",
                "WHERE",
                "WINDOW"};

        private static readonly List<string> ReservedTopLevelWordsNoIndent =
            new List<string> { "EXCEPT ALL", "EXCEPT", "INTERSECT ALL", "INTERSECT", "UNION ALL", "UNION" };

        private static readonly List<string> ReservedNewlineWords =
            new List<string>{
                "AND",
                "CREATE OR",
                "CREATE",
                "ELSE",
                "LATERAL VIEW",
                "OR",
                "OUTER APPLY",
                "WHEN",
                "XOR",
                "JOIN",
                "INNER JOIN",
                "LEFT JOIN",
                "LEFT OUTER JOIN",
                "RIGHT JOIN",
                "RIGHT OUTER JOIN",
                "FULL JOIN",
                "FULL OUTER JOIN",
                "CROSS JOIN",
                "NATURAL JOIN",
                "ANTI JOIN",
                "SEMI JOIN",
                "LEFT ANTI JOIN",
                "LEFT SEMI JOIN",
                "RIGHT OUTER JOIN",
                "RIGHT SEMI JOIN",
                "NATURAL ANTI JOIN",
                "NATURAL FULL OUTER JOIN",
                "NATURAL INNER JOIN",
                "NATURAL LEFT ANTI JOIN",
                "NATURAL LEFT OUTER JOIN",
                "NATURAL LEFT SEMI JOIN",
                "NATURAL OUTER JOIN",
                "NATURAL RIGHT OUTER JOIN",
                "NATURAL RIGHT SEMI JOIN",
                "NATURAL SEMI JOIN"};

        public override DialectConfig DoDialectConfig()
        {
            return DialectConfig.Builder()
                .ReservedWords(ReservedWords)
                .ReservedTopLevelWords(ReservedTopLevelWords)
                .ReservedTopLevelWordsNoIndent(ReservedTopLevelWordsNoIndent)
                .ReservedNewlineWords(ReservedNewlineWords)
                .StringTypes(
                    new List<string>{
                        StringLiteral.DoubleQuote,
                        StringLiteral.SingleQuote,
                        StringLiteral.BackQuote,
                        StringLiteral.Brace})
                .OpenParens(new List<string> { "(", "CASE" })
                .CloseParens(new List<string> { ")", "END" })
                .IndexedPlaceholderTypes(new List<string> { "?" })
                .NamedPlaceholderTypes(new List<string> { "$" })
                .LineCommentTypes(new List<string> { "--" })
                .Operators(new List<string> { "!=", "<=>", "&&", "||", "==", "->" })
                .Build();
        }

        protected override Token TokenOverride(Token token)
        {
            if (Token.IsWindow(token))
            {
                var aheadToken = TokenLookAhead();
                if (aheadToken != null && aheadToken.type == TokenTypes.OPEN_PAREN)
                {
                    return new Token(TokenTypes.RESERVED, token.value);
                }
            }

            if (Token.IsEnd(token))
            {
                var backToken = TokenLookBehind();
                if (backToken != null
                    && backToken.type == TokenTypes.OPERATOR
                    && backToken.value.Equals("."))
                {
                    return new Token(TokenTypes.WORD, token.value);
                }
            }

            return token;
        }

        public SparkSqlFormatter(FormatConfig cfg) : base(cfg)
        {
        }
    }
}
