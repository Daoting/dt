#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.ComponentModel;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a condition format.
    /// </summary>
    /// <remarks>
    /// The user can add a condition format using a string (for example, "[&gt;10]", "[&gt;=0]").
    /// <list type="table">
    /// <listheader><term>Condition format</term><description>Example</description></listheader>  
    /// <item><term>NFPartCond</term><description>[ NFPartCompOper NFPartCondNum ]</description></item>  
    /// <item><term>NFPartCompOper</term><description>(&lt; [= / &gt;]) / = / (&gt; [=])</description></item>  
    /// <item><term>NFPartCondNum</term><description>[-] NFPartIntNum [INTL-CHAR-DECIMAL-SEP NFPartIntNum] [NFPartExponential NFPartIntNum]</description></item>  
    /// </list>
    /// </remarks>
    internal sealed class ConditionFormatPart : FormatPartBase
    {
        /// <summary>
        /// the compare operator.
        /// </summary>
        GeneralCompareType compareOperator;
        /// <summary>
        /// the value.
        /// </summary>
        double value;

        /// <summary>
        /// Creates a new condition format with the specified string expression.
        /// </summary>
        /// <param name="token">The string expression for this format.</param>
        /// <remarks>Examples of conditional string expressions are "[&gt;10]" and "[&gt;=0]".</remarks>
        public ConditionFormatPart(string token) : base(token)
        {
            string str = DefaultTokens.TrimSquareBracket(token);
            if (string.IsNullOrEmpty(str))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            StringBuilder builder = null;
            int num = 0;
            while (num < str.Length)
            {
                char c = str[num];
                if (!DefaultTokens.IsOperator(c))
                {
                    break;
                }
                if (builder == null)
                {
                    builder = new StringBuilder();
                }
                builder.Append(c);
                num++;
            }
            if (builder == null)
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            string str2 = builder.ToString();
            builder = null;
            switch (str2)
            {
                case "<":
                    this.compareOperator = GeneralCompareType.LessThan;
                    break;

                case "<=":
                    this.compareOperator = GeneralCompareType.LessThanOrEqualsTo;
                    break;

                case "=":
                    this.compareOperator = GeneralCompareType.EqualsTo;
                    break;

                case ">=":
                    this.compareOperator = GeneralCompareType.GreaterThanOrEqualsTo;
                    break;

                case ">":
                    this.compareOperator = GeneralCompareType.GreaterThan;
                    break;

                case "<>":
                    this.compareOperator = GeneralCompareType.NotEqualsTo;
                    break;

                default:
                    throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            while (num < str.Length)
            {
                char ch2 = str[num];
                if (DefaultTokens.IsOperator(ch2))
                {
                    throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
                }
                if (builder == null)
                {
                    builder = new StringBuilder();
                }
                builder.Append(ch2);
                num++;
            }
            if (builder == null)
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
            if (!double.TryParse(builder.ToString(), out this.value))
            {
                throw new ArgumentException(ResourceStrings.FormatterIllegaTokenError);
            }
        }

        /// <summary>
        /// Determines whether the format string is valid.
        /// </summary>
        /// <param name="token">The token to evaluate.</param>
        /// <returns>
        /// <c>true</c> if the specified format contains the text; otherwise, <c>false</c>.
        /// </returns>
        internal static bool EvaluateFormat(string token)
        {
            if ((token == null) || (token == string.Empty))
            {
                return false;
            }
            string str = DefaultTokens.TrimSquareBracket(token);
            return (((str != null) && !(str == string.Empty)) && DefaultTokens.IsOperator(str[0]));
        }

        /// <summary>
        /// Determines whether the specified value is <c>true</c>.
        /// </summary>
        /// <param name="value">Value to compare</param>
        /// <returns>
        /// <c>true</c> if the specified value is <c>true</c>; otherwise, <c>false</c>.
        /// </returns>
        internal bool IsMeetCondition(double value)
        {
            switch (this.compareOperator)
            {
                case GeneralCompareType.EqualsTo:
                    return (value == this.value);

                case GeneralCompareType.NotEqualsTo:
                    return !(value == this.value);

                case GeneralCompareType.GreaterThan:
                    return (value > this.value);

                case GeneralCompareType.GreaterThanOrEqualsTo:
                    return (value >= this.value);

                case GeneralCompareType.LessThan:
                    return (value < this.value);

                case GeneralCompareType.LessThanOrEqualsTo:
                    return (value <= this.value);
            }
            return false;
        }

        /// <summary>
        /// Returns a string that represents the current conditional format.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:Dt.Cells.Data.ConditionFormatPart" /> object.
        /// </returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            switch (this.compareOperator)
            {
                case GeneralCompareType.EqualsTo:
                    builder.Append("=");
                    break;

                case GeneralCompareType.NotEqualsTo:
                    builder.Append("<>");
                    break;

                case GeneralCompareType.GreaterThan:
                    builder.Append(">");
                    break;

                case GeneralCompareType.GreaterThanOrEqualsTo:
                    builder.Append(">=");
                    break;

                case GeneralCompareType.LessThan:
                    builder.Append("<");
                    break;

                case GeneralCompareType.LessThanOrEqualsTo:
                    builder.Append("<=");
                    break;

                default:
                    throw new FormatException();
            }
            builder.Append(this.value);
            return DefaultTokens.AddSquareBracket(builder.ToString());
        }

        /// <summary>
        /// Gets the comparison operator type.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.Cells.Data.GeneralCompareType" /> enumeration that specifies the comparison operator.
        /// The default value is <see cref="T:Dt.Cells.Data.GeneralCompareType">GreaterThanOrEquals</see>.
        /// </value>
        [DefaultValue(3)]
        public GeneralCompareType CompareOperator
        {
            get { return  this.compareOperator; }
        }

        /// <summary>
        /// Gets the number value of the conditional format.
        /// </summary>
        /// <value>The number value of the conditional format. The default value is 0.</value>
        [DefaultValue(0)]
        public double Value
        {
            get { return  this.value; }
        }
    }
}

