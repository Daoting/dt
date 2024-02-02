#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;
using System;
using System.IO;
using System.Net;
#endregion

namespace Dt.Core.HtmlLog
{
    /// <summary>
    /// Implements the {Level} element.
    /// can now have a fixed width applied to it, as well as casing rules.
    /// Width is set through formats like "u3" (uppercase three chars),
    /// "w1" (one lowercase char), or "t4" (title case four chars).
    /// </summary>
    static class LevelOutputFormat
    {
        static readonly string[][] TitleCaseLevelMap =
        {
            new[] { "V", "Vb", "Vrb", "Verb" },
            new[] { "D", "De", "Dbg", "Dbug" },
            new[] { "I", "In", "Inf", "Info" },
            new[] { "W", "Wn", "Wrn", "Warn" },
            new[] { "E", "Er", "Err", "Eror" },
            new[] { "F", "Fa", "Ftl", "Fatl" },
        };

        static readonly string[][] LowercaseLevelMap =
        {
            new[] { "v", "vb", "vrb", "verb" },
            new[] { "d", "de", "dbg", "dbug" },
            new[] { "i", "in", "inf", "info" },
            new[] { "w", "wn", "wrn", "warn" },
            new[] { "e", "er", "err", "eror" },
            new[] { "f", "fa", "ftl", "fatl" },
        };

        static readonly string[][] UppercaseLevelMap =
        {
            new[] { "V", "VB", "VRB", "VERB" },
            new[] { "D", "DE", "DBG", "DBUG" },
            new[] { "I", "IN", "INF", "INFO" },
            new[] { "W", "WN", "WRN", "WARN" },
            new[] { "E", "ER", "ERR", "EROR" },
            new[] { "F", "FA", "FTL", "FATL" },
        };

        public static string GetLevelMoniker(LogEventLevel value, string format = null)
        {
            if (format is null || format.Length != 2 && format.Length != 3)
                return Casing.Format(value.ToString(), format);

            // Using int.Parse() here requires allocating a string to exclude the first character prefix.
            // Junk like "wxy" will be accepted but produce benign results.
            var width = format[1] - '0';
            if (format.Length == 3)
            {
                width *= 10;
                width += format[2] - '0';
            }

            if (width < 1)
                return string.Empty;

            if (width > 4)
            {
                var stringValue = value.ToString();
                if (stringValue.Length > width)
                    stringValue = stringValue.Substring(0, width);
                return Casing.Format(stringValue);
            }

            var index = (int)value;
            if (index >= 0 && index <= (int)LogEventLevel.Fatal)
            {
                switch (format[0])
                {
                    case 'w':
                        return LowercaseLevelMap[index][width - 1];
                    case 'u':
                        return UppercaseLevelMap[index][width - 1];
                    case 't':
                        return TitleCaseLevelMap[index][width - 1];
                }
            }

            return Casing.Format(value.ToString(), format);
        }
    }

    static class Casing
    {
        /// <summary>
        /// Apply upper or lower casing to <paramref name="value"/> when <paramref name="format"/> is provided.
        /// Returns <paramref name="value"/> when no or invalid format provided.
        /// </summary>
        /// <param name="value">Provided string for formatting.</param>
        /// <param name="format">Format string.</param>
        /// <returns>The provided <paramref name="value"/> with formatting applied.</returns>
        public static string Format(string value, string format = null)
        {
            switch (format)
            {
                case "u":
                    return value.ToUpperInvariant();
                case "w":
                    return value.ToLowerInvariant();
                default:
                    return value;
            }
        }
    }
}
