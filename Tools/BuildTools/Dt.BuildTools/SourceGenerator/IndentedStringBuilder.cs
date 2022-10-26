#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
#endregion

namespace Dt.BuildTools
{
    /// <summary>
    /// A C# code indented builder.
    /// </summary>
    internal class IndentedStringBuilder
    {
        readonly StringBuilder _stringBuilder;

        public int CurrentLevel { get; private set; }

        public IndentedStringBuilder()
            : this(new StringBuilder())
        {
        }

        public IndentedStringBuilder(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public virtual IDisposable Indent(int count = 1)
        {
            CurrentLevel += count;
            return new DisposableAction(() => CurrentLevel -= count);
        }

        public virtual IDisposable Block(int count = 1)
        {
            var current = CurrentLevel;

            CurrentLevel += count;
            Append("{".Indent(current));
            AppendLine();

            return new DisposableAction(() =>
            {
                CurrentLevel -= count;
                Append("}".Indent(current));
                AppendLine();
            });
        }

        public virtual IDisposable Block(string pattern, params object[] parameters)
        {
            AppendFormat(CultureInfo.InvariantCulture, pattern, parameters);
            AppendLine();

            return Block();
        }

        public virtual void Append(string text)
        {
            _stringBuilder.Append(text);
        }

        public virtual void AppendFormat(IFormatProvider formatProvider, string pattern, params object[] replacements)
        {
            _stringBuilder.AppendFormat(formatProvider, pattern.Indent(CurrentLevel), replacements);
        }

        /// <summary>
        /// Appends a newline.
        /// </summary>
        /// <remarks>
        /// This method presents correct behavior, as opposed to its <see cref="AppendLine(String)"/>
        /// overload. Therefore, this method should be used whenever a newline is desired.
        /// </remarks>
        public virtual void AppendLine()
        {
            _stringBuilder.AppendLine();
        }

        /// <summary>
        /// Appends the given string, *without* appending a newline at the end.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <remarks>
        /// Even though this method seems like it appends a newline, it doesn't. To append a
        /// newline, call <see cref="AppendLine()"/> after this method, as the parameterless
        /// overload has the correct behavior.
        /// </remarks>
        public virtual void AppendLine(string text)
        {
            _stringBuilder.AppendLine(text.Indent(CurrentLevel));
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }
    }

    internal class DisposableAction : IDisposable
    {
        public DisposableAction(Action action)
        {
            Action = action;
        }

        public Action Action { get; private set; }

        public void Dispose()
        {
            Action();
        }
    }

    internal static class StringExtensions
    {
        private static readonly Lazy<Regex> _newLineRegex = new Lazy<Regex>(() => new Regex(@"^", RegexOptions.Multiline));

        public static string Indent(this string text, int indentCount = 1)
        {
            return _newLineRegex.Value.Replace(text, new String('\t', indentCount));
        }

        public static string JoinBy(this IEnumerable<string> items, string joinBy)
        {
            return string.Join(joinBy, items.ToArray());
        }

        public static bool HasValue(this string instance)
        {
            return !string.IsNullOrWhiteSpace(instance);
        }

        public static bool HasValueTrimmed(this string instance)
        {
            return !string.IsNullOrWhiteSpace(instance);
        }
    }
}
