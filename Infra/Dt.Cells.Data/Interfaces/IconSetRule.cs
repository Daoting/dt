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
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the icon set rule.
    /// </summary>
    public sealed class IconSetRule : FormattingRuleBase, ICachableRule
    {
        bool cahced;
        double? highestValueCached;
        IconCriterion[] iconCriteria;
        Dt.Cells.Data.IconSetType iconSetType;
        double? lowestValueCached;
        bool reverseIconOrder;
        bool showIconOnly;

        /// <summary>
        /// Creates a new icon set rule.
        /// </summary>
        public IconSetRule() : this(Dt.Cells.Data.IconSetType.ThreeArrowsColored)
        {
        }

        /// <summary>
        /// Creates a new icon set rule with a specified icon set type.
        /// </summary>
        /// <param name="iconSetType">Type of icon set.</param>
        internal IconSetRule(Dt.Cells.Data.IconSetType iconSetType) : base(null)
        {
            this.Init(iconSetType);
        }

        internal double? CalculateFormula(ICalcEvaluator evaluator, int baseRow, int baseColumn, string formula)
        {
            return ConditionValueConverter.TryDouble(ValueObject.FromFormula(formula).GetValue(evaluator, baseRow, baseColumn));
        }

        void CalculateLowestValueAndHighestValue(IActualValue actualValue, out double? min, out double? max, ICellRange[] ranges)
        {
            min = 0;
            max = 0;
            if (ranges != null)
            {
                foreach (ICellRange range in ranges)
                {
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        int row = i + range.Row;
                        for (int j = 0; j < range.ColumnCount; j++)
                        {
                            int column = j + range.Column;
                            object val = actualValue.GetValue(row, column);
                            if (val != null)
                            {
                                double? nullable = ConditionValueConverter.TryDouble(val);
                                if (nullable.HasValue)
                                {
                                    double num5 = nullable.Value;
                                    if (!min.HasValue)
                                    {
                                        min = new double?(num5);
                                    }
                                    if (!max.HasValue)
                                    {
                                        max = new double?(num5);
                                    }
                                    double num7 = num5;
                                    double? nullable2 = min;
                                    if ((num7 < ((double) nullable2.GetValueOrDefault())) && nullable2.HasValue)
                                    {
                                        min = new double?(num5);
                                    }
                                    double num8 = num5;
                                    double? nullable3 = max;
                                    if ((num8 > ((double) nullable3.GetValueOrDefault())) && nullable3.HasValue)
                                    {
                                        max = new double?(num5);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal double? CalculatePercent(ICalcEvaluator evaluator, int baseRow, int baseColumn, object value, IActualValue actualValue)
        {
            double? nullable = this.CalculateValue(evaluator, baseRow, baseColumn, value);
            if ((nullable.HasValue && (0.0 <= nullable.Value)) && (nullable.Value <= 100.0))
            {
                this.TryCache(actualValue);
                double? lowestValueCached = this.lowestValueCached;
                double? highestValueCached = this.highestValueCached;
                if (lowestValueCached.HasValue && highestValueCached.HasValue)
                {
                    return new double?(lowestValueCached.Value + (((highestValueCached.Value - lowestValueCached.Value) * nullable.Value) / 100.0));
                }
            }
            return null;
        }

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
                double? nullable2 = ConditionValueConverter.TryDouble(evaluator.EvaluateExpression(expression, 0, 0, baseRow, baseColumn, false));
                if (nullable2.HasValue)
                {
                    num += nullable2.Value;
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
        /// Creates a new icon rule set based on the specified parameter.
        /// </summary>
        /// <param name="iconSetType">The icon set type.</param>
        /// <returns>Returns the icon set rule object.</returns>
        public static IconSetRule Create(Dt.Cells.Data.IconSetType iconSetType)
        {
            return new IconSetRule(iconSetType);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return null;
        }

        /// <summary>
        /// Returns a specified value of the rule if the cell satisfies the condition.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="actual">The current object.</param>
        /// <returns>Returns an icon object.</returns>
        public override object Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            object obj2 = actual.GetValue(baseRow, baseColumn);
            if (obj2 == null)
            {
                return null;
            }
            double num = 0.0;
            if (!FormatConverter.IsNumber(obj2))
            {
                return null;
            }
            double? nullable = FormatConverter.TryDouble(obj2, false);
            if (!nullable.HasValue)
            {
                return null;
            }
            num = nullable.Value;
            int num2 = 0;
            if ((this.iconSetType >= Dt.Cells.Data.IconSetType.ThreeArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.ThreeSymbolsUncircled))
            {
                num2 = 3;
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FourArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FourTrafficLights))
            {
                num2 = 4;
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FiveArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FiveBoxes))
            {
                num2 = 5;
            }
            if (this.iconCriteria == null)
            {
                return (int) 0;
            }
            double maxValue = double.MaxValue;
            for (int i = num2 - 1; i > 0; i--)
            {
                if (i >= (this.iconCriteria.Length + 1))
                {
                    return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(0), this.showIconOnly);
                }
                IconCriterion criterion = this.iconCriteria[i - 1];
                if ((criterion == null) || (criterion.Value == null))
                {
                    return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(0), this.showIconOnly);
                }
                double? nullable2 = this.GetActualValue(evaluator, baseRow, baseColumn, i - 1, actual);
                if (!nullable2.HasValue)
                {
                    return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(0), this.showIconOnly);
                }
                if (criterion.IsGreaterThanOrEqualTo)
                {
                    if ((num < maxValue) && (num >= nullable2.Value))
                    {
                        return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(i), this.showIconOnly);
                    }
                }
                else if ((num < maxValue) && (num > nullable2.Value))
                {
                    return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(i), this.showIconOnly);
                }
            }
            return new IconDrawingObject(baseRow, baseColumn, this.iconSetType, this.modifyIconIndex(0), this.showIconOnly);
        }

        internal double? GetActualValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, int index, IActualValue actual)
        {
            IconCriterion criterion = this.iconCriteria[index];
            if (criterion != null)
            {
                switch (criterion.IconValueType)
                {
                    case IconValueType.Number:
                        return this.CalculateValue(evaluator, baseRow, baseColumn, criterion.Value);

                    case IconValueType.Percent:
                        return this.CalculatePercent(evaluator, baseRow, baseColumn, criterion.Value, actual);

                    case IconValueType.Percentile:
                        return this.CalculatePercentile(evaluator, baseRow, baseColumn, criterion.Value);

                    case IconValueType.Formula:
                        return this.CalculateValue(evaluator, baseRow, baseColumn, criterion.Value);
                }
            }
            return null;
        }

        void ICachableRule.ClearCache()
        {
            this.ClearCache();
        }

        void Init(Dt.Cells.Data.IconSetType iconSetType)
        {
            this.showIconOnly = false;
            this.reverseIconOrder = false;
            this.InitIconSetType(iconSetType);
        }

        void InitIconSetType(Dt.Cells.Data.IconSetType iconSetType)
        {
            this.iconSetType = iconSetType;
            if ((this.iconSetType >= Dt.Cells.Data.IconSetType.ThreeArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.ThreeSymbolsUncircled))
            {
                this.iconCriteria = new IconCriterion[] { new IconCriterion(true, IconValueType.Percent, (int) 0x21), new IconCriterion(true, IconValueType.Percent, (int) 0x43) };
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FourArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FourTrafficLights))
            {
                this.iconCriteria = new IconCriterion[] { new IconCriterion(true, IconValueType.Percent, (int) 0x19), new IconCriterion(true, IconValueType.Percent, (int) 50), new IconCriterion(true, IconValueType.Percent, (int) 0x4b) };
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FiveArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FiveBoxes))
            {
                this.iconCriteria = new IconCriterion[] { new IconCriterion(true, IconValueType.Percent, (int) 20), new IconCriterion(true, IconValueType.Percent, (int) 40), new IconCriterion(true, IconValueType.Percent, (int) 60), new IconCriterion(true, IconValueType.Percent, (int) 80) };
            }
            else if (this.iconSetType > Dt.Cells.Data.IconSetType.FiveBoxes)
            {
                this.iconCriteria = new IconCriterion[0];
            }
        }

        internal override bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (base.condition == null)
            {
                return false;
            }
            object obj2 = actual.GetValue(baseRow, baseColumn);
            if (obj2 == null)
            {
                return false;
            }
            double num = 0.0;
            if (!FormatConverter.IsNumber(obj2))
            {
                return false;
            }
            double? nullable = FormatConverter.TryDouble(obj2, false);
            if (!nullable.HasValue)
            {
                return false;
            }
            num = nullable.Value;
            int num2 = 0;
            if ((this.iconSetType >= Dt.Cells.Data.IconSetType.ThreeArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.ThreeSymbolsUncircled))
            {
                num2 = 3;
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FourArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FourTrafficLights))
            {
                num2 = 4;
            }
            else if ((this.iconSetType >= Dt.Cells.Data.IconSetType.FiveArrowsColored) && (this.iconSetType <= Dt.Cells.Data.IconSetType.FiveBoxes))
            {
                num2 = 5;
            }
            if (this.iconCriteria != null)
            {
                double maxValue = double.MaxValue;
                for (int i = num2 - 1; i > 0; i--)
                {
                    if (i >= (this.iconCriteria.Length + 1))
                    {
                        return true;
                    }
                    IconCriterion criterion = this.iconCriteria[i - 1];
                    if ((criterion == null) || (criterion.Value == null))
                    {
                        return true;
                    }
                    double? nullable2 = this.GetActualValue(evaluator, baseRow, baseColumn, i - 1, actual);
                    if (!nullable2.HasValue)
                    {
                        return true;
                    }
                    if (criterion.IsGreaterThanOrEqualTo)
                    {
                        if ((num < maxValue) && (num >= nullable2.Value))
                        {
                            return true;
                        }
                    }
                    else if ((num < maxValue) && (num > nullable2.Value))
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        static bool IsFormula(object val)
        {
            return ((val is string) && val.ToString().StartsWith("="));
        }

        int modifyIconIndex(int index)
        {
            int num = this.iconCriteria.Length + 1;
            if (this.reverseIconOrder && (num > 2))
            {
                return ((num - 1) - index);
            }
            return index;
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

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "IconSetType")
                {
                    if (name != "ReverseIconOrder")
                    {
                        if (name == "ShowIconOnly")
                        {
                            this.showIconOnly = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                            return;
                        }
                        if (name == "IconCriteria")
                        {
                            List<IconCriterion> list = new List<IconCriterion>();
                            Serializer.DeserializeList((IList) list, reader);
                            if (list != null)
                            {
                                this.iconCriteria = list.ToArray();
                            }
                        }
                        return;
                    }
                }
                else
                {
                    this.iconSetType = (Dt.Cells.Data.IconSetType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.IconSetType), reader);
                    return;
                }
                this.reverseIconOrder = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            Serializer.SerializeObj(this.iconSetType, "IconSetType", writer);
            if (this.reverseIconOrder)
            {
                Serializer.SerializeObj((bool) this.reverseIconOrder, "ReverseIconOrder", writer);
            }
            if (this.showIconOnly)
            {
                Serializer.SerializeObj((bool) this.showIconOnly, "ShowIconOnly", writer);
            }
            Serializer.WriteStartObj("IconCriteria", writer);
            if ((this.iconCriteria != null) && (this.iconCriteria.Length > 0))
            {
                Serializer.SerializeList(this.iconCriteria, writer);
            }
            Serializer.WriteEndObj(writer);
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.iconCriteria = null;
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
                this.CalculateLowestValueAndHighestValue(actual, out this.lowestValueCached, out this.highestValueCached, base.Ranges);
                this.cahced = true;
            }
        }

        internal override bool CanFormattingStyle
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets or sets the icon criteria. 
        /// </summary>
        /// <value>The icon criteria array.</value>
        public IconCriterion[] IconCriteria
        {
            get { return  this.iconCriteria; }
        }

        /// <summary>
        /// Gets  or sets the type of the icon set.
        /// </summary>
        /// <value>
        /// A value that specifies the type of icon set.
        /// The default value is <see cref="P:Dt.Cells.Data.IconSetRule.IconSetType">ThreeArrowsColored</see>.
        /// </value>
        [DefaultValue(0)]
        public Dt.Cells.Data.IconSetType IconSetType
        {
            get { return  this.iconSetType; }
            set
            {
                if (value != this.iconSetType)
                {
                    this.iconSetType = value;
                    this.InitIconSetType(this.iconSetType);
                    this.OnPropertyChanged("IconSetType");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to use the reverse icon order.
        /// </summary>
        [DefaultValue(false)]
        public bool ReverseIconOrder
        {
            get { return  this.reverseIconOrder; }
            set
            {
                if (this.reverseIconOrder != value)
                {
                    this.reverseIconOrder = value;
                }
                this.OnPropertyChanged("ReverseIconOrder");
            }
        }

        /// <summary>
        /// Gets or sets whether to show the icon only.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowIconOnly
        {
            get { return  this.showIconOnly; }
            set
            {
                if (this.showIconOnly != value)
                {
                    this.showIconOnly = value;
                }
                this.OnPropertyChanged("ShowIconOnly");
            }
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

