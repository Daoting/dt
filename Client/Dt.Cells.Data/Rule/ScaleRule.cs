#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a general scale rule.
    /// </summary>
    public abstract class ScaleRule : FormattingRuleBase, ICachableRule
    {
        bool cahced;
        internal object[] expected;
        double? highestValueCached;
        double? lowestValueCached;
        internal ScaleValue[] scales;

        /// <summary>
        /// Constructs a new scale rule.
        /// </summary>
        protected ScaleRule() : this(new ScaleValue(ScaleValueType.LowestValue, null), new ScaleValue(ScaleValueType.Percent, (int) 50), new ScaleValue(ScaleValueType.HighestValue, null))
        {
        }

        /// <summary>
        /// Creates a new scale rule.
        /// </summary>
        protected ScaleRule(ScaleValue s1, ScaleValue s2, ScaleValue s3) : base(null)
        {
            this.scales = new ScaleValue[] { s1, s2, s3 };
            this.expected = new object[3];
        }

        /// <summary>
        /// Calculates the formula.
        /// </summary>
        /// <param name="evaluator"></param>
        /// <param name="baseRow"></param>
        /// <param name="baseColumn"></param>
        /// <param name="formula">Formula</param>
        /// <returns>Returns the formula.</returns>
        internal double? CalculateFormula(ICalcEvaluator evaluator, int baseRow, int baseColumn, string formula)
        {
            object obj2 = ValueObject.FromFormula(formula).GetValue(evaluator, baseRow, baseColumn);
            try
            {
                return new double?(ConditionValueConverter.ToDouble(obj2));
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the highest value.
        /// </summary>
        /// <param name="actualValue"></param>
        /// <returns>Returns the value.</returns>
        internal double? CalculateHighestValueEx(IActualValue actualValue)
        {
            List<double> list = Top10Condition.GetMaxValues(actualValue, 1, base.Ranges);
            if ((list != null) && (list.Count > 0))
            {
                return new double?(list[0]);
            }
            return null;
        }

        /// <summary>
        /// Calculates the lowest value.
        /// </summary>
        /// <param name="actualValue"></param>
        /// <returns>Returns the value.</returns>
        internal double? CalculateLowestValueEx(IActualValue actualValue)
        {
            List<double> list = Top10Condition.GetMinValues(actualValue, 1, base.Ranges);
            if ((list != null) && (list.Count > 0))
            {
                return new double?(list[0]);
            }
            return null;
        }

        /// <summary>
        /// Calculates the number.
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Returns the value.</returns>
        internal double? CalculateNumber(object value)
        {
            try
            {
                return new double?(ConditionValueConverter.ToDouble(value));
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the percent scale value type.
        /// </summary>
        /// <param name="evaluator">Sheet for value object calculating</param>
        /// <param name="baseRow"></param>
        /// <param name="baseColumn"></param>
        /// <param name="value">Scale value</param>
        /// <param name="actualValue"></param>
        /// <returns>Returns the result of the calculation.</returns>
        internal double? CalculatePercent(ICalcEvaluator evaluator, int baseRow, int baseColumn, object value, IActualValue actualValue)
        {
            double? nullable = this.CalculateValue(evaluator, baseRow, baseColumn, value);
            if ((nullable.HasValue && (0.0 <= nullable.Value)) && (nullable.Value <= 100.0))
            {
                double? lowestValue = this.GetLowestValue(actualValue);
                double? highestValue = this.GetHighestValue(actualValue);
                if (lowestValue.HasValue && highestValue.HasValue)
                {
                    return new double?(lowestValue.Value + (((highestValue.Value - lowestValue.Value) * nullable.Value) / 100.0));
                }
            }
            return null;
        }

        /// <summary>
        /// Calculates the percentile for the specified value.
        /// </summary>
        /// <param name="evaluator"></param>
        /// <param name="baseRow"></param>
        /// <param name="baseColumn"></param>
        /// <param name="value">Value</param>
        /// <returns>Returns the percentile for the value.</returns>
        internal double? CalculatePercentile(ICalcEvaluator evaluator, int baseRow, int baseColumn, object value)
        {
            double? nullable = this.CalculateValue(evaluator, baseRow, baseColumn, value);
            if ((!nullable.HasValue || (0.0 > nullable.Value)) || (nullable.Value > 100.0))
            {
                return null;
            }
            double num = 0.0;
            foreach (CellRange range in base.Ranges)
            {
                object expression = evaluator.CreateExpression("CalcPercentileFunction", new object[] { range, (double) (nullable.Value / 100.0) });
                object obj3 = evaluator.EvaluateExpression(expression, 0, 0, baseRow, baseColumn, false);
                try
                {
                    num += ConditionValueConverter.ToDouble(obj3);
                }
                catch (InvalidCastException)
                {
                }
            }
            return new double?(num / ((double) base.Ranges.Length));
        }

        internal double? CalculateValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, object value)
        {
            if (IsFormula(value))
            {
                return this.CalculateFormula(evaluator, baseRow, baseColumn, TrimFormula((string) (value as string)));
            }
            return ConditionValueConverter.TryDouble(value);
        }

        internal void ClearCache()
        {
            this.lowestValueCached = null;
            this.highestValueCached = null;
            this.cahced = false;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            ScaleRule rule = base.Clone() as ScaleRule;
            rule.scales = new ScaleValue[3];
            this.scales.CopyTo(rule.scales, 0);
            return rule;
        }

        /// <summary>
        /// Creates conditions for the current rule.
        /// </summary>
        /// <returns></returns>
        protected override ConditionBase CreateCondition()
        {
            return null;
        }

        /// <summary>
        /// Evaluates a specified value of the rule if the cell satisfies the condition.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="actual">The current value.</param>
        /// <returns>Returns the conditional number value.</returns>
        public override object Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            this.TryCache(actual);
            if (base.Contains(baseRow, baseColumn))
            {
                object obj2 = actual.GetValue();
                if (obj2 == null)
                {
                    return null;
                }
                try
                {
                    double num = ConditionValueConverter.ToDouble(obj2);
                    double? nullable = this.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                    double? nullable2 = this.GetActualValue(evaluator, baseRow, baseColumn, 1, actual);
                    double? nullable3 = this.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                    if (!nullable2.HasValue)
                    {
                        if (nullable.HasValue && nullable3.HasValue)
                        {
                            double? nullable4 = nullable;
                            double? nullable5 = nullable3;
                            if ((((double) nullable4.GetValueOrDefault()) > ((double) nullable5.GetValueOrDefault())) && (nullable4.HasValue & nullable5.HasValue))
                            {
                                return null;
                            }
                            return (double) Evaluate2Scale(num, nullable.Value, nullable3.Value);
                        }
                    }
                    else if ((nullable.HasValue && nullable3.HasValue) && nullable2.HasValue)
                    {
                        double? nullable6 = nullable;
                        double? nullable7 = nullable3;
                        if ((((double) nullable6.GetValueOrDefault()) > ((double) nullable7.GetValueOrDefault())) && (nullable6.HasValue & nullable7.HasValue))
                        {
                            return null;
                        }
                        double num5 = num;
                        double? nullable8 = nullable;
                        if ((num5 < ((double) nullable8.GetValueOrDefault())) && nullable8.HasValue)
                        {
                            return (double) 0.0;
                        }
                        double num6 = num;
                        double? nullable9 = nullable3;
                        if ((num6 >= ((double) nullable9.GetValueOrDefault())) && nullable9.HasValue)
                        {
                            return (double) 2.0;
                        }
                        Evaluate2Scale(nullable2.Value, nullable.Value, nullable3.Value);
                        if ((nullable.Value <= num) && (num <= nullable2.Value))
                        {
                            return (double) Evaluate2Scale(num, nullable.Value, nullable2.Value);
                        }
                        double num4 = 1.0 + Evaluate2Scale(num, nullable.Value, nullable3.Value);
                        return (double) num4;
                    }
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// Evaluates the scale.
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="min">The minimum</param>
        /// <param name="max">The maximum</param>
        /// <returns></returns>
        internal static double Evaluate2Scale(double value, double min, double max)
        {
            if (value <= min)
            {
                return 0.0;
            }
            if (value >= max)
            {
                return 1.0;
            }
            return ((value - min) / (max - min));
        }

        internal static Windows.UI.Color? EvaluateColor(double value, Windows.UI.Color color1, Windows.UI.Color color2)
        {
            if ((0.0 <= value) && (value <= 1.0))
            {
                double num = (color1.A * (1.0 - value)) + (color2.A * value);
                double num2 = (color1.R * (1.0 - value)) + (color2.R * value);
                double num3 = (color1.G * (1.0 - value)) + (color2.G * value);
                double num4 = (color1.B * (1.0 - value)) + (color2.B * value);
                return new Windows.UI.Color?(Windows.UI.Color.FromArgb((byte) num, (byte) num2, (byte) num3, (byte) num4));
            }
            return null;
        }

        /// <summary>
        /// Calculates the actual value for a specified <see cref="T:Dt.Cells.Data.ScaleRule" />.
        /// </summary>
        /// <returns>Returns the actual value of a specified <see cref="T:Dt.Cells.Data.ScaleRule" /> value.</returns>
        internal double? GetActualValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, int index, IActualValue actual)
        {
            ScaleValue value2 = this.scales[index];
            if (value2 != null)
            {
                switch (value2.Type)
                {
                    case ScaleValueType.Number:
                        return this.CalculateValue(evaluator, baseRow, baseColumn, value2.Value);

                    case ScaleValueType.LowestValue:
                        return this.GetLowestValue(actual);

                    case ScaleValueType.HighestValue:
                        return this.GetHighestValue(actual);

                    case ScaleValueType.Percent:
                        return this.CalculatePercent(evaluator, baseRow, baseColumn, value2.Value, actual);

                    case ScaleValueType.Percentile:
                        return this.CalculatePercentile(evaluator, baseRow, baseColumn, value2.Value);

                    case ScaleValueType.Automin:
                    {
                        double? lowestValue = this.GetLowestValue(actual);
                        if (!lowestValue.HasValue || (lowestValue.Value <= 0.0))
                        {
                            return lowestValue;
                        }
                        return 0.0;
                    }
                    case ScaleValueType.Formula:
                        return this.CalculateValue(evaluator, baseRow, baseColumn, value2.Value);

                    case ScaleValueType.Automax:
                    {
                        double? highestValue = this.GetHighestValue(actual);
                        if (!highestValue.HasValue || (highestValue.Value >= 0.0))
                        {
                            return highestValue;
                        }
                        return 0.0;
                    }
                }
            }
            return null;
        }

        Windows.UI.Color? GetColor(int index)
        {
            object obj2 = this.expected[index];
            if (obj2 is Windows.UI.Color)
            {
                return new Windows.UI.Color?((Windows.UI.Color) obj2);
            }
            return null;
        }

        /// <summary>
        /// Calculates the highest value.
        /// </summary>
        /// <param name="actualValue"></param>
        /// <returns>Returns the value.</returns>
        internal double? GetHighestValue(IActualValue actualValue)
        {
            this.TryCache(actualValue);
            return this.highestValueCached;
        }

        internal double? GetLowestValue(IActualValue actualValue)
        {
            this.TryCache(actualValue);
            return this.lowestValueCached;
        }

        void ICachableRule.ClearCache()
        {
            this.ClearCache();
        }

        internal override bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (base.condition != null)
            {
                this.TryCache(actual);
                if (base.Contains(baseRow, baseColumn))
                {
                    object obj2 = actual.GetValue();
                    if (obj2 == null)
                    {
                        return false;
                    }
                    try
                    {
                        double num = ConditionValueConverter.ToDouble(obj2);
                        double? nullable = this.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                        double? nullable2 = this.GetActualValue(evaluator, baseRow, baseColumn, 1, actual);
                        double? nullable3 = this.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                        if (!nullable2.HasValue)
                        {
                            if (nullable.HasValue && nullable3.HasValue)
                            {
                                double? nullable4 = nullable;
                                double? nullable5 = nullable3;
                                if ((((double) nullable4.GetValueOrDefault()) > ((double) nullable5.GetValueOrDefault())) && (nullable4.HasValue & nullable5.HasValue))
                                {
                                    return false;
                                }
                                return true;
                            }
                        }
                        else if ((nullable.HasValue && nullable3.HasValue) && nullable2.HasValue)
                        {
                            double? nullable6 = nullable;
                            double? nullable7 = nullable3;
                            if ((((double) nullable6.GetValueOrDefault()) > ((double) nullable7.GetValueOrDefault())) && (nullable6.HasValue & nullable7.HasValue))
                            {
                                return false;
                            }
                            double num2 = num;
                            double? nullable8 = nullable;
                            if ((num2 >= ((double) nullable8.GetValueOrDefault())) || !nullable8.HasValue)
                            {
                                double num3 = num;
                                double? nullable9 = nullable3;
                                if ((num3 >= ((double) nullable9.GetValueOrDefault())) && nullable9.HasValue)
                                {
                                    return true;
                                }
                                Evaluate2Scale(nullable2.Value, nullable.Value, nullable3.Value);
                                if ((nullable.Value <= num) && (num <= nullable2.Value))
                                {
                                    return true;
                                }
                            }
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        static bool IsFormula(object val)
        {
            return ((val is string) && val.ToString().StartsWith("="));
        }

        /// <summary>
        /// Raises an event when the property changes.
        /// </summary>
        /// <param name="prop">The property name.</param>
        protected override void OnPropertyChanged(string prop)
        {
            base.OnPropertyChanged(prop);
            if ("Ranges".Equals(prop))
            {
                this.ClearCache();
            }
        }

        static string TrimFormula(string val)
        {
            if (!string.IsNullOrEmpty(val))
            {
                return val.TrimStart(new char[] { '=' });
            }
            return null;
        }

        internal void TryCache(IActualValue actual)
        {
            if (!this.cahced)
            {
                this.lowestValueCached = this.CalculateLowestValueEx(actual);
                this.highestValueCached = this.CalculateHighestValueEx(actual);
                this.cahced = true;
            }
        }

        internal override bool CanFormattingStyle
        {
            get { return  false; }
        }

        internal object[] Expected
        {
            get { return  this.expected; }
        }

        internal ScaleValue[] Scales
        {
            get { return  this.scales; }
        }

        /// <summary>
        /// Gets or sets whether rules with lower priority are applied over this rule. If this property is true 
        /// and this rule evaluates to true, no rules with lower priority are applied over this rule.
        /// </summary>
        public new bool StopIfTrue
        {
            get { return  false; }
        }
    }
}

