#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Globalization;
using System.Text;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Creates a cell address as text, given specified row and column numbers.
    /// </summary>
    public class CalcAddressFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if ((i != 2) && (i != 3))
            {
                return (i == 4);
            }
            return true;
        }

        private void AppendA1Letter(StringBuilder sb, int coord, bool relative)
        {
            if (!relative)
            {
                sb.Append("$");
            }
            int length = sb.Length;
            while (coord > 0)
            {
                char ch = (char)(0x41 + ((coord - 1) % 0x1a));
                sb.Insert(length, ((char) ch).ToString());
                coord = (coord - 1) / 0x1a;
            }
        }

        private void AppendA1Number(StringBuilder sb, int coord, bool relative)
        {
            if (!relative)
            {
                sb.Append("$");
            }
            sb.Append(coord);
        }

        private void AppendExternalName(StringBuilder sb, string name)
        {
            if ((name != null) && (0 < name.Length))
            {
                bool flag = !char.IsLetter(name[0]) && (name[0] != '_');
                for (int i = 1; !flag && (i < name.Length); i++)
                {
                    flag = !char.IsLetterOrDigit(name[i]) && (name[i] != '_');
                }
                if (flag)
                {
                    sb.Append("'");
                    sb.Append(name.Replace("'", "''"));
                    sb.Append("'");
                }
                else
                {
                    sb.Append(name);
                }
                sb.Append("!");
            }
        }

        private void AppendR1C1Number(StringBuilder sb, string prefix, int coord, bool relative)
        {
            sb.Append(prefix);
            if (relative)
            {
                if (coord != 0)
                {
                    sb.Append("[");
                    sb.Append(((int) coord).ToString(CultureInfo.InvariantCulture.NumberFormat));
                    sb.Append("]");
                }
            }
            else
            {
                sb.Append(coord);
            }
        }

        /// <summary>
        /// Creates a cell address as text, given specified row and column numbers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 5 items: row_num, column_num, [abs_num], [a1], [sheet_text].
        /// </para>
        /// <para>
        /// Row_num is the row number to use in the cell reference.
        /// </para>
        /// <para>
        /// Column_num is the column number to use in the cell reference.
        /// </para>
        /// <para>
        /// [Abs_num] specifies the type of reference to return
        /// </para>
        /// <para>
        /// [A1] is a logical value that specifies the A1 or R1C1 reference style.
        /// If a1 is <see langword="true" /> or omitted, ADDRESS returns an A1-style
        /// reference; if <see langword="false" />, ADDRESS returns an R1C1-style reference.
        /// </para>
        /// <para>
        /// [Sheet_text] is text specifying the name of the worksheet to be used
        /// as the external reference. If sheet_text is omitted, no sheet name is used.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int coord = CalcConvert.ToInt(args[0]);
            int num2 = CalcConvert.ToInt(args[1]);
            int num3 = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToInt(args[2]) : 1;
            bool flag = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToBool(args[3]) : true;
            string name = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToString(args[4]) : "";
            bool relative = (((num3 == 3) || (num3 == 4)) || (num3 == 7)) || (num3 == 8);
            bool flag3 = (((num3 == 2) || (num3 == 4)) || (num3 == 6)) || (num3 == 8);
            StringBuilder sb = new StringBuilder();
            if (((coord < 1) && (flag || !relative)) || (coord > 0x100000))
            {
                return CalcErrors.Value;
            }
            if (((num2 < 1) && (flag || !flag3)) || (num2 > 0x4000))
            {
                return CalcErrors.Value;
            }
            if ((num3 < 1) || (8 < num3))
            {
                return CalcErrors.Value;
            }
            this.AppendExternalName(sb, name);
            if (flag)
            {
                this.AppendA1Letter(sb, num2, flag3);
                this.AppendA1Number(sb, coord, relative);
            }
            else
            {
                this.AppendR1C1Number(sb, "R", coord, relative);
                this.AppendR1C1Number(sb, "C", num2, flag3);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 5;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "ADDRESS";
            }
        }
    }
}

