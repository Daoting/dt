using System;
using System.Text.RegularExpressions;

namespace Dt.Toolkit.Sql
{
    public class Token
    {
        public readonly TokenTypes type;
        public readonly string value;
        public readonly string regex;
        public readonly string whitespaceBefore;
        public readonly string key;

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore, string key)
        {
            this.type = type;
            this.value = value;
            this.regex = regex;
            this.whitespaceBefore = whitespaceBefore;
            this.key = key;
        }

        public Token(TokenTypes type, string value, string regex, string whitespaceBefore)
            : this(type, value, regex, whitespaceBefore, null) { }

        public Token(TokenTypes type, string value, string regex)
            : this(type, value, regex, null) { }

        public Token(TokenTypes type, string value)
            : this(type, value, null) { }

        public Token WithWhitespaceBefore(string whitespaceBefore)
        {
            return new Token(type, value, regex, whitespaceBefore, key);
        }

        public Token WithKey(string key)
        {
            return new Token(type, value, regex, whitespaceBefore, key);
        }

        public override string ToString()
        {
            return "type: " + type + ", value: [" + value + "], regex: /" + regex + "/, key:" + key;
        }

        private static readonly Regex And =
            new Regex("^AND$", RegexOptions.IgnoreCase);
        private static readonly Regex Between =
            new Regex("^BETWEEN$", RegexOptions.IgnoreCase);
        private static readonly Regex Limit =
            new Regex("^LIMIT$", RegexOptions.IgnoreCase);
        private static readonly Regex Set =
            new Regex("^SET$", RegexOptions.IgnoreCase);
        private static readonly Regex By =
            new Regex("^BY$", RegexOptions.IgnoreCase);
        private static readonly Regex Window =
            new Regex("^WINDOW$", RegexOptions.IgnoreCase);
        private static readonly Regex End =
            new Regex("^END$", RegexOptions.IgnoreCase);

        private static Func<Token, bool> IsToken(TokenTypes type, Regex regex)
        {
            return token => token?.type == type && regex.IsMatch(token.value);
        }

        public static bool IsAnd(Token token)
        {
            return IsToken(TokenTypes.RESERVED_NEWLINE, And).Invoke(token);
        }

        public static bool IsBetween(Token token)
        {
            return IsToken(TokenTypes.RESERVED, Between).Invoke(token);
        }

        public static bool IsLimit(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, Limit).Invoke(token);
        }

        public static bool IsSet(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, Set).Invoke(token);
        }

        public static bool IsBy(Token token)
        {
            return IsToken(TokenTypes.RESERVED, By).Invoke(token);
        }

        public static bool IsWindow(Token token)
        {
            return IsToken(TokenTypes.RESERVED_TOP_LEVEL, Window).Invoke(token);
        }

        public static bool IsEnd(Token token)
        {
            return IsToken(TokenTypes.CLOSE_PAREN, End).Invoke(token);
        }
    }
}
