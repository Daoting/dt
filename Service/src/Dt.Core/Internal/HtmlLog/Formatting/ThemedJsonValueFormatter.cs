#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Events;
using Serilog.Formatting.Json;
using System;
using System.Globalization;
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    class ThemedJsonValueFormatter : ThemedValueFormatter
    {
        readonly ThemedDisplayValueFormatter _displayFormatter;

        public ThemedJsonValueFormatter(HtmlTheme theme)
            : base(theme)
        {
            _displayFormatter = new ThemedDisplayValueFormatter(theme);
        }

        public override ThemedValueFormatter SwitchTheme(HtmlTheme theme)
        {
            return new ThemedJsonValueFormatter(theme);
        }

        protected override int VisitScalarValue(ThemedValueFormatterState state, ScalarValue scalar)
        {
            if (scalar is null)
                throw new ArgumentNullException(nameof(scalar));

            // At the top level, for scalar values, use "display" rendering.
            if (state.IsTopLevel)
                return _displayFormatter.FormatLiteralValue(scalar, state.Output, state.Format);

            return FormatLiteralValue(scalar, state.Output);
        }

        protected override int VisitSequenceValue(ThemedValueFormatterState state, SequenceValue sequence)
        {
            if (sequence == null)
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
                Visit(state.Nest(), sequence.Elements[index]);
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write(']');

            return 0;
        }

        protected override int VisitStructureValue(ThemedValueFormatterState state, StructureValue structure)
        {
            var count = 0;

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
                    JsonValueFormatter.WriteQuotedJsonString(property.Name, state.Output);

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write(": ");

                count += Visit(state.Nest(), property.Value);
            }

            if (structure.TypeTag != null)
            {
                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write(delim);

                using (ApplyStyle(state.Output, HtmlThemeStyle.Name))
                    JsonValueFormatter.WriteQuotedJsonString("$type", state.Output);

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write(": ");

                using (ApplyStyle(state.Output, HtmlThemeStyle.String))
                    JsonValueFormatter.WriteQuotedJsonString(structure.TypeTag, state.Output);
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

                var style = element.Key.Value == null
                    ? HtmlThemeStyle.Null
                    : element.Key.Value is string
                        ? HtmlThemeStyle.String
                        : HtmlThemeStyle.Scalar;

                using (ApplyStyle(state.Output, style))
                    JsonValueFormatter.WriteQuotedJsonString((element.Key.Value ?? "null").ToString(), state.Output);

                using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                    state.Output.Write(": ");

                count += Visit(state.Nest(), element.Value);
            }

            using (ApplyStyle(state.Output, HtmlThemeStyle.TertiaryText))
                state.Output.Write('}');

            return count;
        }

        int FormatLiteralValue(ScalarValue scalar, TextWriter output)
        {
            var value = scalar.Value;
            var count = 0;

            if (value == null)
            {
                using (ApplyStyle(output, HtmlThemeStyle.Null))
                    output.Write("null");
                return count;
            }

            if (value is string str)
            {
                using (ApplyStyle(output, HtmlThemeStyle.String))
                    JsonValueFormatter.WriteQuotedJsonString(str, output);
                return count;
            }

            if (value is ValueType)
            {
                if (value is int || value is uint || value is long || value is ulong || value is decimal || value is byte || value is sbyte || value is short || value is ushort)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Number))
                        output.Write(((IFormattable)value).ToString(null, CultureInfo.InvariantCulture));
                    return count;
                }

                if (value is double d)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Number))
                    {
                        if (double.IsNaN(d) || double.IsInfinity(d))
                            JsonValueFormatter.WriteQuotedJsonString(d.ToString(CultureInfo.InvariantCulture), output);
                        else
                            output.Write(d.ToString("R", CultureInfo.InvariantCulture));
                    }
                    return count;
                }

                if (value is float f)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Number))
                    {
                        if (double.IsNaN(f) || double.IsInfinity(f))
                            JsonValueFormatter.WriteQuotedJsonString(f.ToString(CultureInfo.InvariantCulture), output);
                        else
                            output.Write(f.ToString("R", CultureInfo.InvariantCulture));
                    }
                    return count;
                }

                if (value is bool b)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Boolean))
                        output.Write(b ? "true" : "false");

                    return count;
                }

                if (value is char ch)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Scalar))
                        JsonValueFormatter.WriteQuotedJsonString(ch.ToString(), output);
                    return count;
                }

                if (value is DateTime || value is DateTimeOffset)
                {
                    using (ApplyStyle(output, HtmlThemeStyle.Scalar))
                    {
                        output.Write('"');
                        output.Write(((IFormattable)value).ToString("O", CultureInfo.InvariantCulture));
                        output.Write('"');
                    }
                    return count;
                }
            }

            using (ApplyStyle(output, HtmlThemeStyle.Scalar))
                JsonValueFormatter.WriteQuotedJsonString(value.ToString(), output);

            return count;
        }
    }
}
