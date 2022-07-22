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
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a unique condition.
    /// </summary>
    internal sealed class UniqueCondition : ConditionBase, IRangesDependent
    {
        ICellRange[] ranges;

        /// <summary>
        /// Creates a new unique condition.
        /// </summary>
        public UniqueCondition() : this(true, null)
        {
        }

        /// <summary>
        /// Creates a new unique condition, and sets whether to check for duplicate data.
        /// </summary>
        /// <param name="duplicated">Whether to check for duplicate data.</param>
        /// <param name="ranges"></param>
        /// <remarks>If the <paramref name="duplicated" /> parameter is set to <c>true</c>, the condition checks for duplicate data.</remarks>
        internal UniqueCondition(bool duplicated, ICellRange[] ranges) : base((bool) duplicated, null)
        {
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
            UniqueCondition condition = base.Clone() as UniqueCondition;
            condition.ranges = ConditionBase.CloneRanges(this.ranges);
            return condition;
        }

        /// <summary>
        /// Creates a UniqueCondition object.
        /// </summary>
        /// <param name="duplicated">if set to <c>true</c> accepts duplicated value.</param>
        /// <param name="ranges">The ranges.</param>
        /// <returns>The UniqueCondition object.</returns>
        public static UniqueCondition Create(bool duplicated, ICellRange[] ranges)
        {
            return new UniqueCondition(duplicated, ranges);
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
            if (ConditionValueConverter.IsNumber(obj2))
            {
                obj2 = (double) ConditionValueConverter.TryDouble(obj2).Value;
            }
            bool? nullable = base.GetExpectedBoolean(evaluator, baseRow, baseColumn);
            if (!nullable.HasValue)
            {
                return false;
            }
            Worksheet worksheet = evaluator as Worksheet;
            List<object> list = null;
            if (worksheet != null)
            {
                list = GetDuplicated(actualObj, this.ranges, worksheet.RowCount, worksheet.ColumnCount);
            }
            else
            {
                list = GetDuplicated(actualObj, this.ranges, 0x7fffffff, 0x7fffffff);
            }
            if (list != null)
            {
                if (list.Contains(obj2))
                {
                    return nullable.Value;
                }
                if (nullable.Value)
                {
                    return false;
                }
                return true;
            }
            if (nullable.Value)
            {
                return false;
            }
            return true;
        }

        static List<object> GetDuplicated(IActualValue actualValue, ICellRange[] ranges, int maxRowCount = 0x7fffffff, int maxColumnCount = 0x7fffffff)
        {
            Dictionary<object, int> dictionary = new Dictionary<object, int>();
            if (ranges != null)
            {
                foreach (ICellRange range in ranges)
                {
                    int num = Math.Min(range.RowCount, maxRowCount);
                    for (int i = 0; i < num; i++)
                    {
                        int row = i + range.Row;
                        int num4 = Math.Min(range.ColumnCount, maxColumnCount);
                        for (int j = 0; j < num4; j++)
                        {
                            int column = j + range.Column;
                            object obj2 = actualValue.GetValue(row, column);
                            if (obj2 != null)
                            {
                                if (ConditionValueConverter.IsNumber(obj2))
                                {
                                    obj2 = (double) ConditionValueConverter.TryDouble(obj2).Value;
                                }
                                int num7 = 0;
                                dictionary.TryGetValue(obj2, out num7);
                                dictionary[obj2] = num7 + 1;
                            }
                        }
                    }
                }
            }
            List<object> list = new List<object>();
            foreach (KeyValuePair<object, int> pair in dictionary)
            {
                if (pair.Value > 1)
                {
                    list.Add(pair.Key);
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
            string str;
            base.OnReadXml(reader);
            List<CellRange> list = new List<CellRange>();
            if (((str = reader.Name) != null) && (str == "Ranges"))
            {
                using (XmlReader reader2 = Serializer.ExtractNode(reader))
                {
                    Serializer.DeserializeList((IList) list, reader2);
                }
            }
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
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.ranges = null;
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get { return  typeof(bool); }
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
    }
}

