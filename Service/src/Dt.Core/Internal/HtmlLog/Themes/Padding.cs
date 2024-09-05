#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-01-22 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Parsing;
using System.IO;
#endregion

namespace Dt.Core.HtmlLog
{
    static class Padding
    {
        static readonly char[] PaddingChars = new string(' ', 80).ToCharArray();

        /// <summary>
        /// Writes the provided value to the output, applying direction-based padding when <paramref name="alignment"/> is provided.
        /// </summary>
        /// <param name="output">Output object to write result.</param>
        /// <param name="value">Provided value.</param>
        /// <param name="alignment">The alignment settings to apply when rendering <paramref name="value"/>.</param>
        public static void Apply(TextWriter output, string value, Alignment? alignment)
        {
            if (alignment is null || value.Length >= alignment.Value.Width)
            {
                output.Write(value);
                return;
            }

            var pad = alignment.Value.Width - value.Length;

            if (alignment.Value.Direction == AlignmentDirection.Left)
                output.Write(value);

            if (pad <= PaddingChars.Length)
            {
                output.Write(PaddingChars, 0, pad);
            }
            else
            {
                output.Write(new string(' ', pad));
            }

            if (alignment.Value.Direction == AlignmentDirection.Right)
                output.Write(value);
        }
    }

    static class AlignmentExtensions
    {
        public static Alignment Widen(this Alignment alignment, int amount)
        {
            return new Alignment(alignment.Direction, alignment.Width + amount);
        }
    }
}
