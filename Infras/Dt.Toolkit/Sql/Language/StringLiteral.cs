using System.Collections.Generic;
using System.Linq;

namespace Dt.Toolkit.Sql
{
    class StringLiteral
    {
        public static readonly string BackQuote = "``";
        public static readonly string DoubleQuote = "\"\"";
        public static readonly string UDoubleQuote = "U&\"\"";
        public static readonly string USingleQuote = "U&''";
        public static readonly string ESingleQuote = "E''";
        public static readonly string NSingleQuote = "N''";
        public static readonly string QSingleQuote = "Q''";
        public static readonly string SingleQuote = "''";
        public static readonly string Brace = "{}";
        public static readonly string Dollar = "$$";
        public static readonly string Bracket = "[]";

        private static readonly Dictionary<string, string> Literals;

        static StringLiteral()
        {
            Literals = Preset.Presets.ToList()
                .ToDictionary(preset => preset.GetKey(), preset => preset.GetRegex());
        }

        public static string Get(string key) => Literals[key];

        private class Preset
        {
            public static readonly Preset BackQuote = new Preset(
                StringLiteral.BackQuote,
                "((`[^`]*($|`))+)"
                );

            public static readonly Preset DoubleQuote = new Preset(
                StringLiteral.DoubleQuote,
                "((\"[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*(\"|$))+)"
                );

            public static readonly Preset Bracket = new Preset(
                StringLiteral.Bracket,
                "((\\[[^\\]]*($|\\]))(\\][^\\]]*($|\\]))*)"
                );

            public static readonly Preset Brace = new Preset(
                StringLiteral.Brace,
                "((\\{[^\\}]*($|\\}))+)"
                );

            public static readonly Preset SingleQuote = new Preset(
                StringLiteral.SingleQuote,
                "(('[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );

            public static readonly Preset NSingleQuote = new Preset(
                StringLiteral.NSingleQuote,
                "((N'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );

            public static readonly Preset QSingleQuote = new Preset(
                StringLiteral.QSingleQuote,
                "(?i)" +
                string.Join(
                    "|",
                    "((n?q'\\{(?:(?!\\}'|\\\\).)*\\}')+)",
                    "((n?q'\\[(?:(?!\\]'|\\\\).)*\\]')+)",
                    "((n?q'<(?:(?!>'|\\\\).)*>')+)",
                    "((n?q'\\((?:(?!\\)'|\\\\).)*\\)')+)"));

            public static readonly Preset ESingleQuote = new Preset(
                StringLiteral.ESingleQuote,
                "((E'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );

            public static readonly Preset USingleQUote = new Preset(
                StringLiteral.USingleQuote,
                "((U&'[^'\\\\]*(?:\\\\.[^'\\\\]*)*('|$))+)"
                );

            public static readonly Preset UDoubleQuote = new Preset(
                StringLiteral.UDoubleQuote,
                "((U&\"[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*(\"|$))+)"
                );

            public static readonly Preset Dollar = new Preset(
                StringLiteral.Dollar,
                "((?<tag>\\$\\w*\\$)[\\s\\S]*?(?:\\k<tag>|$))"
                );

            public static IEnumerable<Preset> Presets
            {
                get
                {
                    yield return BackQuote;
                    yield return DoubleQuote;
                    yield return Bracket;
                    yield return Brace;
                    yield return SingleQuote;
                    yield return NSingleQuote;
                    yield return QSingleQuote;
                    yield return ESingleQuote;
                    yield return USingleQUote;
                    yield return UDoubleQuote;
                    yield return Dollar;

                }
            }

            public readonly string key;
            public readonly string regex;

            public Preset(string key, string regex)
            {
                this.key = key;
                this.regex = regex;
            }

            public string GetKey() => key;

            public string GetRegex() => regex;
        }
    }
}
