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
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a top 10 condition.
    /// </summary>
    public sealed class Top10Condition : ConditionBase, IRangesDependent
    {
        bool isPercent;
        ICellRange[] ranges;
        Top10ConditionType type;

        /// <summary>
        /// Creates a new top 10 condition.
        /// </summary>
        public Top10Condition() : this(Top10ConditionType.Top, 10, null)
        {
        }

        /// <summary>
        /// Creates a new top 10 condition with the specified type and rank, for the specified cell ranges.
        /// </summary>
        /// <param name="type">The condition type for the rule.</param>
        /// <param name="rank">The rank of the rule.</param>
        /// <param name="ranges">The cell ranges for the condition.</param>
        internal Top10Condition(Top10ConditionType type, int rank, ICellRange[] ranges) : base((int) rank, null)
        {
            this.type = type;
            this.isPercent = false;
            this.ranges = ranges;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            Top10Condition condition = base.Clone() as Top10Condition;
            condition.ranges = ConditionBase.CloneRanges(this.ranges);
            condition.type = this.type;
            condition.isPercent = this.isPercent;
            return condition;
        }

        /// <summary>
        /// Evaluates using the specified evaluator.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actualObj">The actual value object.</param>
        /// <returns><c>true</c> if the result is successful; otherwise, <c>false</c>.</returns>
        public override bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actualObj)
        {
            object obj2 = actualObj.GetValue();
            if (obj2 == null)
            {
                return this.IgnoreBlank;
            }
            int? nullable = base.GetExpectedInt(evaluator, baseRow, baseColumn);
            if (nullable.HasValue)
            {
                List<double> list = null;
                if (this.type == Top10ConditionType.Top)
                {
                    list = GetMaxValues(actualObj, nullable.Value, this.ranges);
                }
                else if (this.type == Top10ConditionType.Bottom)
                {
                    list = GetMinValues(actualObj, nullable.Value, this.ranges);
                }
                if (list != null)
                {
                    double num = 0.0;
                    try
                    {
                        num = ConditionValueConverter.ToDouble(obj2);
                    }
                    catch (InvalidCastException)
                    {
                        return false;
                    }
                    if (list.Contains(num))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a Top10Condition object from the rank.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="rank">The rank.</param>
        /// <param name="ranges">The ranges.</param>
        /// <returns>The Top10Condition object.</returns>
        public static Top10Condition FromRank(Top10ConditionType type, int rank, ICellRange[] ranges)
        {
            return new Top10Condition(type, rank, ranges);
        }

        internal static List<double> GetMaxValues(IActualValue actualValue, int rank, ICellRange[] ranges)
        {
            List<double> list = new List<double>();
            if (ranges != null)
            {
                double maxValue = double.MaxValue;
                int num2 = 0;
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
                                    double num7 = nullable.Value;
                                    if (num2 < rank)
                                    {
                                        list.Add(num7);
                                        if (num7 < maxValue)
                                        {
                                            maxValue = num7;
                                        }
                                        num2++;
                                    }
                                    else if (num7 > maxValue)
                                    {
                                        list.Remove(maxValue);
                                        list.Add(num7);
                                        if (list.IndexOf(maxValue) < 0)
                                        {
                                            maxValue = num7;
                                            for (int k = 0; k < list.Count; k++)
                                            {
                                                if (list[k] < maxValue)
                                                {
                                                    maxValue = list[k];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        internal static List<double> GetMinValues(IActualValue actualValue, int rank, ICellRange[] ranges)
        {
            List<double> list = new List<double>();
            if (ranges != null)
            {
                double minValue = double.MinValue;
                int num2 = 0;
                foreach (ICellRange range in ranges)
                {
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        int row = i + range.Row;
                        for (int j = 0; j < range.ColumnCount; j++)
                        {
                            int column = j + range.Column;
                            object val = actualValue.GetValue(row, column);
                            double num7 = 0.0;
                            if (val != null)
                            {
                                double? nullable = ConditionValueConverter.TryDouble(val);
                                if (nullable.HasValue)
                                {
                                    num7 = nullable.Value;
                                    if (num2 < rank)
                                    {
                                        list.Add(num7);
                                        if (num7 > minValue)
                                        {
                                            minValue = num7;
                                        }
                                        num2++;
                                    }
                                    else if (num7 < minValue)
                                    {
                                        list.Remove(minValue);
                                        list.Add(num7);
                                        if (list.IndexOf(minValue) < 0)
                                        {
                                            minValue = num7;
                                            for (int k = 0; k < list.Count; k++)
                                            {
                                                if (list[k] > minValue)
                                                {
                                                    minValue = list[k];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            List<CellRange> list = new List<CellRange>();
            string name = reader.Name;
            if (name == null)
            {
                goto Label_0092;
            }
            if (name != "Ranges")
            {
                if (name == "Type")
                {
                    goto Label_005A;
                }
                if (name == "IsPercent")
                {
                    this.isPercent = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                }
                goto Label_0092;
            }
            using (XmlReader reader2 = Serializer.ExtractNode(reader))
            {
                Serializer.DeserializeList((IList) list, reader2);
                goto Label_0092;
            }
        Label_005A:
            this.type = (Top10ConditionType) Serializer.DeserializeObj(typeof(Top10ConditionType), reader);
        Label_0092:
            this.ranges = list.ToArray();
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.ranges != null)
            {
                Serializer.WriteStartObj("Ranges", writer);
                Serializer.SerializeList(this.ranges, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.type != Top10ConditionType.Top)
            {
                Serializer.SerializeObj(this.type, "Type", writer);
            }
            if (this.isPercent)
            {
                Serializer.SerializeObj((bool) this.isPercent, "IsPercent", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.type = Top10ConditionType.Top;
            this.isPercent = false;
            this.ranges = null;
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override System.Type ExpectedValueType
        {
            get { return  typeof(int); }
        }

        [DefaultValue(false)]
        internal bool IsPercent
        {
            get { return  this.isPercent; }
            set { this.isPercent = value; }
        }

        /// <summary>
        /// Gets or sets the ranges.
        /// </summary>
        /// <value>The ranges.</value>
        public ICellRange[] Ranges
        {
            get { return  this.ranges; }
            set { this.ranges = value; }
        }

        /// <summary>
        /// Gets or sets the top 10 condition type.
        /// </summary>
        /// <value>
        /// The top 10 condition type.
        /// The default value is <see cref="T:Dt.Cells.Data.Top10ConditionType">Top</see>.
        /// </value>
        [DefaultValue(0)]
        public Top10ConditionType Type
        {
            get { return  this.type; }
            set { this.type = value; }
        }
    }
}

