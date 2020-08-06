#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number condition.
    /// </summary>
    internal sealed class AreaCondition : ConditionBase
    {
        const double Epsilon = 1E-10;

        /// <summary>
        /// Creates a new number condition.
        /// </summary>
        public AreaCondition() : this(null, null)
        {
        }

        /// <summary>
        /// Creates a new number condition of the specified type with the specified expected value.
        /// </summary>
        /// <param name="expected">The expected number.</param>
        /// <param name="formula">The expected formula.</param>
        internal AreaCondition(string expected, string formula) : base(expected, formula)
        {
            if (base.ExpectedValueObject != null)
            {
                base.ExpectedValueObject.ExpectArrayValue = true;
            }
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            return (base.Clone() as AreaCondition);
        }

        public override bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actualObj)
        {
            object obj2 = actualObj.GetValue();
            if ((obj2 == null) || obj2.Equals(string.Empty))
            {
                return this.IgnoreBlank;
            }
            object[] objArray = this.GetValidList(evaluator, baseRow, baseColumn);
            object obj3 = (obj2 is string) ? new GeneralFormatter().Parse((string) (obj2 as string)) : obj2;
            object obj4 = obj3;
            if (obj3 is TimeSpan)
            {
                TimeSpan span = (TimeSpan) obj3;
                obj3 = new DateTime(0x76b, 12, 30, span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
            }
            if (objArray != null)
            {
                foreach (object obj5 in objArray)
                {
                    if ((obj5 == null) && (obj2 == null))
                    {
                        return true;
                    }
                    if (obj5 != null)
                    {
                        if (this.IsNumber(obj5) && this.IsNumber(obj3))
                        {
                            double num;
                            double num2;
                            if ((double.TryParse(obj5.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num) && double.TryParse(obj3.ToString(), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num2)) && (Math.Round(Math.Abs((double)(num - num2)), 10) <= 1E-10))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            if (obj5.Equals(obj3))
                            {
                                return true;
                            }
                            if (((obj5 is TimeSpan) && (obj4 is TimeSpan)) && obj5.Equals(obj4))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public static AreaCondition FromFormula(string formula)
        {
            return new AreaCondition(string.Empty, formula);
        }

        public static AreaCondition FromSource(string expected)
        {
            return new AreaCondition(expected, null);
        }

        /// <summary>
        /// Gets the valid list.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <returns>The valid list.</returns>
        public object[] GetValidList(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            List<object> list = new List<object>();
            if (!string.IsNullOrEmpty(base.ExpectedFormula))
            {
                object obj2 = base.GetExpected(evaluator, baseRow, baseColumn, true);
                if (obj2 is Array)
                {
                    IEnumerator enumerator = (obj2 as Array).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        list.Add(enumerator.Current);
                    }
                }
                else
                {
                    list.Add(obj2);
                }
            }
            else if (!string.IsNullOrEmpty((string) (base.ExpectedValue as string)))
            {
                string expectedValue = (string) (base.ExpectedValue as string);
                string listSeparator = CultureInfo.InvariantCulture.TextInfo.ListSeparator;
                string[] strArray = new string[] { listSeparator, ((char) this.ExpressionListSeparator).ToString() };
                string[] strArray2 = expectedValue.Split(strArray, (StringSplitOptions) StringSplitOptions.None);
                if (strArray2 != null)
                {
                    GeneralFormatter formatter = new GeneralFormatter();
                    foreach (string str3 in strArray2)
                    {
                        if ((str3 != null) && !(str3 == string.Empty))
                        {
                            string str = str3.Trim(new char[] { DefaultTokens.Space });
                            if ((str != null) && (str != string.Empty))
                            {
                                list.Add(formatter.Parse(str));
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }

        bool IsNumber(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is string)
            {
                return false;
            }
            return ((((((value is double) || (value is float)) || ((value is int) || (value is long))) || (((value is short) || (value is decimal)) || ((value is sbyte) || (value is ulong)))) || ((value is uint) || (value is ushort))) || (value is byte));
        }

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            if (base.ExpectedValueObject != null)
            {
                base.ExpectedValueObject.ExpectArrayValue = true;
            }
        }

        protected override void Reset()
        {
            base.Reset();
        }

        object TryValue(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            double? nullable = FormatConverter.TryDouble(obj, false);
            if (nullable.HasValue)
            {
                int num2;
                long num3;
                double d = nullable.Value;
                if (Math.Abs((double) (d - Math.Floor(d))) != 0.0)
                {
                    return obj;
                }
                if (NumberHelper.TryInteger(d, out num2))
                {
                    return (int) num2;
                }
                if (NumberHelper.TryLong(d, out num3))
                {
                    return (long) num3;
                }
            }
            return obj;
        }

        public override Type ExpectedValueType
        {
            get { return  typeof(string); }
        }

        /// <summary>
        /// Gets the expression list separator.
        /// </summary>
        /// <value>The expression list separator.</value>
        [DefaultValue('\0')]
        char ExpressionListSeparator
        {
            get { return  DefaultTokens.EndCharOfArray; }
        }
    }
}

