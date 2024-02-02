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
using Serilog.Data;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Parsing;
using System;
using System.IO;
using System.Net;
#endregion

namespace Dt.Core.HtmlLog
{
    class ThemedDisplayValueFormatter : ThemedValueFormatter
    {
        public ThemedDisplayValueFormatter(HtmlTheme theme)
            : base(theme)
        {
        }

        public override ThemedValueFormatter SwitchTheme(HtmlTheme theme)
        {
            return new ThemedDisplayValueFormatter(theme);
        }

        protected override int VisitScalarValue(ThemedValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));
            return FormatLiteralValue(scalar, state.Output, state.Format);
        }

        protected override int VisitSequenceValue(ThemedValueFormatterState state, SequenceValue sequence)
        {
            if (sequence is null)
                throw new ArgumentNullException(nameof(sequence));

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('[');

            var delim = string.Empty;
            for (var index = 0; index < sequence.Elements.Count; ++index)
            {
                if (delim.Length != 0)
                {
                    using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                        state.Output.Write(delim);
                }

                delim = ", ";
                Visit(state, sequence.Elements[index]);
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write(']');

            return 0;
        }

        protected override int VisitStructureValue(ThemedValueFormatterState state, StructureValue structure)
        {
            var count = 0;

            if (structure.TypeTag != null)
            {
                using (ApplyStyle(state.Output, HtmlThemeStyle.Name))
                    state.Output.Write(structure.TypeTag);

                state.Output.Write(' ');
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('{');

            var delim = string.Empty;
            for (var index = 0; index < structure.Properties.Count; ++index)
            {
                if (delim.Length != 0)
                {
                    using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                        state.Output.Write(delim);
                }

                delim = ", ";

                var property = structure.Properties[index];

                using (ApplyStyle(state.Output, HtmlThemeStyle.Name))
                    state.Output.Write(property.Name);

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write('=');

                count += Visit(state.Nest(), property.Value);
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('}');

            return count;
        }

        protected override int VisitDictionaryValue(ThemedValueFormatterState state, DictionaryValue dictionary)
        {
            var count = 0;

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('{');

            var delim = string.Empty;
            foreach (var element in dictionary.Elements)
            {
                if (delim.Length != 0)
                {
                    using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                        state.Output.Write(delim);
                }

                delim = ", ";

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write('[');

                using (ApplyStyle(state.Output, HtmlThemeStyle.String))
                    count += Visit(state.Nest(), element.Key);

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write("]=");

                count += Visit(state.Nest(), element.Value);
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('}');

            return count;
        }

        public int FormatLiteralValue(ScalarValue scalar, TextWriter output, string format)
        {
            var value = scalar.Value;
            var count = 0;

            if (value is null)
            {
                using (ApplyStyle(output, HtmlThemeStyle.Null))
                    output.Write("null");
                return count;
            }

            if (value is string str)
            {
                using (ApplyStyle(output, HtmlThemeStyle.String))
                {
                    if (format != "l")
                        JsonValueFormatter.WriteQuotedJsonString(str, output);
                    else
                        output.Write(str);
                }
                return count;
            }

            if (value is ValueType)
            {
                if (value is int || value is uint || value is long || value is ulong ||
                    value is decimal || value is byte || value is sbyte || value is short ||
                    value is ushort || value is float || value is double)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Number))
                        scalar.Render(output, format);
                    return count;
                }

                if (value is bool b)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Boolean))
                        output.Write(b);

                    return count;
                }

                if (value is char ch)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Scalar))
                    {
                        output.Write('\'');
                        output.Write(ch);
                        output.Write('\'');
                    }
                    return count;
                }
            }

            using (ApplyStyle(output, HtmlThemeStyle.Scalar))
                scalar.Render(output, format);

            return count;
        }
    }
}
