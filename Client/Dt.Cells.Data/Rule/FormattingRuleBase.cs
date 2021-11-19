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
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a base rule for conditional rules.
    /// </summary>
    public abstract class FormattingRuleBase : INotifyPropertyChanged, ICloneable, IXmlSerializable
    {
        /// <summary>
        /// Indicates the base condition of the rule.
        /// </summary>
        protected ConditionBase condition;
        int priority;
        CellRange[] ranges;
        bool stopIfTrue;
        StyleInfo style;

        internal event EventHandler<ConditionChangedEventArgs> ConditionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        internal event PropertyChangedEventHandler PropertyChanging;

        /// <summary>
        /// Constructs a formatting base rule as the specified style.
        /// </summary>
        /// <param name="style"></param>
        protected FormattingRuleBase(StyleInfo style)
        {
            this.style = style;
            if (this.style != null)
            {
                this.style.PropertyChanged += new PropertyChangedEventHandler(this.OnStylePropertyChanged);
            }
            this.priority = 1;
            this.stopIfTrue = false;
        }

        internal void AddColumns(int column, int count)
        {
            this.OnPropertyChanging("Ranges");
            if (this.ranges != null)
            {
                for (int i = 0; i < this.ranges.Length; i++)
                {
                    CellRange range = this.ranges[i];
                    if (range.Column >= column)
                    {
                        this.ranges[i] = new CellRange(range.Row, range.Column + count, range.RowCount, range.ColumnCount);
                    }
                    else if ((range.Column < column) && (column < (range.Column + range.ColumnCount)))
                    {
                        this.ranges[i] = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount + count);
                    }
                }
            }
            this.OnPropertyChanged("Ranges");
        }

        internal void AddRows(int row, int count)
        {
            this.OnPropertyChanging("Ranges");
            if (this.ranges != null)
            {
                for (int i = 0; i < this.ranges.Length; i++)
                {
                    CellRange range = this.ranges[i];
                    if (range.Row >= row)
                    {
                        this.ranges[i] = new CellRange(range.Row + count, range.Column, range.RowCount, range.ColumnCount);
                    }
                    else if ((range.Row < row) && (row < (range.Row + range.RowCount)))
                    {
                        this.ranges[i] = new CellRange(range.Row, range.Column, range.RowCount + count, range.ColumnCount);
                    }
                }
            }
            this.OnPropertyChanged("Ranges");
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            FormattingRuleBase base2 = Activator.CreateInstance(base.GetType()) as FormattingRuleBase;
            if (this.ranges != null)
            {
                List<CellRange> list = new List<CellRange>();
                foreach (CellRange range in this.ranges)
                {
                    list.Add(new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount));
                }
                base2.ranges = list.ToArray();
            }
            base2.style = (this.style == null) ? null : new StyleInfo(this.style);
            return base2;
        }

        /// <summary>
        /// Determines whether the range of cells contains the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the rule contains the specified cell; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int row, int column)
        {
            if (this.ranges != null)
            {
                foreach (CellRange range in this.ranges)
                {
                    if (range.Contains(row, column))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>The condition.</returns>
        protected abstract ConditionBase CreateCondition();
        /// <summary>
        /// Returns the cell style of the rule if the cell satisfies the condition.
        /// </summary>
        /// <param name="evaluator"></param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="actual"></param>
        /// <returns>
        /// Returns the conditional <see cref="T:Dt.Cells.Data.StyleInfo" /> object.
        /// </returns>
        public virtual object Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            int num;
            int num2;
            if (!this.Contains(baseRow, baseColumn))
            {
                return null;
            }
            this.InitCondition();
            this.GetBaseCoordinate(out num, out num2);
            this.condition.AdjustOffset(baseRow - num, baseColumn - num2, true);
            object expected = null;
            if (this.condition.Evaluate(evaluator, baseRow, baseColumn, actual))
            {
                expected = this.GetExpected();
            }
            this.condition.AdjustOffset(0, 0, true);
            return expected;
        }

        void GetBaseCoordinate(out int baseRow, out int baseColumn)
        {
            baseRow = 0x7fffffff;
            baseColumn = 0x7fffffff;
            if ((this.ranges != null) && (this.ranges.Length > 0))
            {
                foreach (CellRange range in this.ranges)
                {
                    baseRow = Math.Min(range.Row, baseRow);
                    baseColumn = Math.Min(range.Column, baseColumn);
                }
            }
            else
            {
                baseRow = 0;
                baseColumn = 0;
            }
        }

        /// <summary>
        /// Gets the style of the base rule.
        /// </summary>
        /// <returns>The style of the base rule.</returns>
        protected virtual object GetExpected()
        {
            return this.style;
        }

        /// <summary>
        /// Initial condition for the rule.
        /// </summary>
        protected virtual void InitCondition()
        {
            if (this.condition == null)
            {
                this.Condition = this.CreateCondition();
            }
        }

        internal bool IntersectsInternal(int row, int column, int rowCount, int columnCount)
        {
            int num;
            return this.IntersectsInternal(row, column, rowCount, columnCount, out num);
        }

        internal bool IntersectsInternal(int row, int column, int rowCount, int columnCount, out int rangeIndex)
        {
            if (this.ranges != null)
            {
                for (int i = 0; i < this.ranges.Length; i++)
                {
                    CellRange range = this.ranges[i];
                    if ((range != null) && range.Intersects(row, column, rowCount, columnCount))
                    {
                        rangeIndex = i;
                        return true;
                    }
                }
            }
            rangeIndex = -1;
            return false;
        }

        internal virtual bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (this.Contains(baseRow, baseColumn))
            {
                int num;
                int num2;
                this.InitCondition();
                this.GetBaseCoordinate(out num, out num2);
                this.condition.AdjustOffset(baseRow - num, baseColumn - num2, true);
                return this.condition.Evaluate(evaluator, baseRow, baseColumn, actual);
            }
            return false;
        }

        /// <summary>
        /// Raises the event when the property is changed.
        /// </summary>
        /// <param name="prop">The name of the property.</param>
        protected virtual void OnPropertyChanged(string prop)
        {
            if ((!string.Equals(prop, "Ranges") && !string.Equals(prop, "Style")) && (!string.Equals(prop, "StopIfTure") && !string.Equals(prop, "Priority")))
            {
                this.Condition = null;
            }
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        internal void OnPropertyChanging(string prop)
        {
            if (this.PropertyChanging != null)
            {
                this.PropertyChanging(this, new PropertyChangedEventArgs(prop));
            }
        }

        /// <summary>
        /// Reads xml from the xml reader.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        protected virtual void OnReadXml(XmlReader reader)
        {
            List<CellRange> list = null;
            string name = reader.Name;
            if (name == null)
            {
                goto Label_0104;
            }
            if (name != "Ranges")
            {
                if (name == "Style")
                {
                    goto Label_0071;
                }
                if (name == "Priority")
                {
                    this.priority = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                }
                else if (name == "StopIfTrue")
                {
                    this.stopIfTrue = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                }
                goto Label_0104;
            }
            list = new List<CellRange>();
            using (XmlReader reader2 = Serializer.ExtractNode(reader))
            {
                Serializer.DeserializeList((IList) list, reader2);
                goto Label_0104;
            }
        Label_0071:
            if (this.style != null)
            {
                this.style.PropertyChanged -= new PropertyChangedEventHandler(this.OnStylePropertyChanged);
            }
            this.style = Serializer.DeserializeObj(typeof(StyleInfo), reader) as StyleInfo;
            if (this.style != null)
            {
                this.style.PropertyChanged += new PropertyChangedEventHandler(this.OnStylePropertyChanged);
            }
        Label_0104:
            if (list != null)
            {
                this.ranges = list.ToArray();
            }
        }

        void OnStylePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanging("Style");
            this.OnPropertyChanged("Style");
        }

        /// <summary>
        /// Writes the rule to the xml writer.
        /// </summary>
        /// <param name="writer">The xml writer.</param>
        protected virtual void OnWriteXml(XmlWriter writer)
        {
            if (this.ranges != null)
            {
                Serializer.WriteStartObj("Ranges", writer);
                Serializer.SerializeList(this.ranges, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.style != null)
            {
                Serializer.SerializeObj(this.style, "Style", writer);
            }
            if (this.StopIfTrue)
            {
                Serializer.SerializeObj((bool) this.stopIfTrue, "StopIfTrue", writer);
            }
            Serializer.SerializeObj((int) this.priority, "Priority", writer);
        }

        internal void RemoveColumns(int column, int count)
        {
            if (this.ranges != null)
            {
                this.OnPropertyChanging("Ranges");
                List<CellRange> list = new List<CellRange>();
                int length = this.ranges.Length;
                for (int i = 0; i < length; i++)
                {
                    CellRange range = this.ranges[i];
                    if (range.Column > column)
                    {
                        if ((range.Column + range.ColumnCount) <= (column + count))
                        {
                            list.Add(range);
                        }
                        else
                        {
                            this.ranges[i] = new CellRange(range.Row, range.Column - count, range.RowCount, range.ColumnCount);
                        }
                    }
                    else if ((range.Column <= column) && (column < (range.Column + range.ColumnCount)))
                    {
                        CellRange range2 = new CellRange(range.Row, range.Column, range.RowCount, range.ColumnCount - Math.Min((range.Column + range.ColumnCount) - column, count));
                        if ((range2.ColumnCount == 0) || (range2.RowCount == 0))
                        {
                            list.Add(range);
                        }
                        else
                        {
                            this.ranges[i] = range2;
                        }
                    }
                }
                List<CellRange> list2 = new List<CellRange>(this.ranges);
                foreach (CellRange range3 in list)
                {
                    list2.Remove(range3);
                }
                this.ranges = list2.ToArray();
                this.OnPropertyChanged("Ranges");
            }
        }

        internal void RemoveRows(int row, int count)
        {
            if (this.ranges != null)
            {
                this.OnPropertyChanging("Ranges");
                List<CellRange> list = new List<CellRange>();
                int length = this.ranges.Length;
                for (int i = 0; i < length; i++)
                {
                    CellRange range = this.ranges[i];
                    if (range.Row > row)
                    {
                        if ((range.Row + range.RowCount) <= (row + count))
                        {
                            list.Add(range);
                        }
                        else
                        {
                            this.ranges[i] = new CellRange(range.Row - count, range.Column, range.RowCount, range.ColumnCount);
                        }
                    }
                    else if ((range.Row <= row) && (row < (range.Row + range.RowCount)))
                    {
                        CellRange range2 = new CellRange(range.Row, range.Column, range.RowCount - Math.Min((range.Row + range.RowCount) - row, count), range.ColumnCount);
                        if ((range2.ColumnCount == 0) || (range2.RowCount == 0))
                        {
                            list.Add(range);
                        }
                        else
                        {
                            this.ranges[i] = range2;
                        }
                    }
                }
                List<CellRange> list2 = new List<CellRange>(this.ranges);
                foreach (CellRange range3 in list)
                {
                    list2.Remove(range3);
                }
                this.ranges = list2.ToArray();
                this.OnPropertyChanged("Ranges");
            }
        }

        /// <summary>
        /// Resets the rule. 
        /// </summary>
        protected virtual void Reset()
        {
            this.ranges = null;
            this.Condition = null;
            if (this.style != null)
            {
                this.style.PropertyChanged -= new PropertyChangedEventHandler(this.OnStylePropertyChanged);
            }
            this.style = null;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Reset();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    this.OnReadXml(reader);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.OnWriteXml(writer);
        }

        /// <summary>
        /// Gets a value indicating the return value of Evaluate() method is a styleInfo. 
        /// </summary>
        internal virtual bool CanFormattingStyle
        {
            get { return  true; }
        }

        internal ConditionBase Condition
        {
            get
            {
                if (this.condition == null)
                {
                    this.InitCondition();
                }
                return this.condition;
            }
            private set
            {
                if (this.condition != value)
                {
                    IConditionalFormula[] formulaConditions = this.FormulaConditions;
                    this.condition = value;
                    if (this.ConditionChanged != null)
                    {
                        this.ConditionChanged(this, new ConditionChangedEventArgs(formulaConditions, this.FormulaConditions));
                    }
                }
            }
        }

        internal IConditionalFormula[] FormulaConditions
        {
            get
            {
                if ((this.condition == null) || (this is IConditionalFormula))
                {
                    return null;
                }
                ConditionBase base2 = this.condition;
                Stack<ConditionBase> stack = new Stack<ConditionBase>();
                stack.Push(base2);
                List<IConditionalFormula> list = new List<IConditionalFormula>();
                while (stack.Count > 0)
                {
                    base2 = stack.Pop();
                    RelationCondition condition = base2 as RelationCondition;
                    if (condition != null)
                    {
                        if (condition.Item1 != null)
                        {
                            if (!string.IsNullOrEmpty(condition.Item1.ExpectedFormula))
                            {
                                list.Add(condition.Item1);
                            }
                            else
                            {
                                stack.Push(condition.Item1);
                            }
                        }
                        if (condition.Item2 != null)
                        {
                            if (!string.IsNullOrEmpty(condition.Item2.ExpectedFormula))
                            {
                                list.Add(condition.Item2);
                            }
                            else
                            {
                                stack.Push(condition.Item2);
                            }
                        }
                    }
                    else if ((base2 != null) && !string.IsNullOrEmpty(base2.ExpectedFormula))
                    {
                        list.Add(base2);
                    }
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is deleted.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        internal bool HasNoReference
        {
            get
            {
                if ((this.ranges != null) && (this.ranges.Length > 0))
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the priority of this conditional formatting rule. The value is used to determine
        /// which format should be evaluated and rendered. Lower numeric values have a higher priority
        /// than higher numeric values. The highest priority is 1.
        /// </summary>
        public int Priority
        {
            get { return  this.priority; }
            internal set
            {
                this.OnPropertyChanging("Priority");
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Priority");
                }
                this.priority = value;
                this.OnPropertyChanged("Priority");
            }
        }

        /// <summary>
        /// Gets the cell range collection for the rule.
        /// </summary>
        /// <value>The cell range collection. The value is never null.</value>
        public CellRange[] Ranges
        {
            get
            {
                if (this.ranges == null)
                {
                    this.ranges = new CellRange[0];
                }
                return this.ranges;
            }
            set
            {
                this.OnPropertyChanging("Ranges");
                this.ranges = value;
                this.OnPropertyChanged("Ranges");
            }
        }

        /// <summary>
        /// Gets or sets whether rules with lower priority are applied over this rule. If this property is true 
        /// and this rule evaluates to true, no rules with lower priority are applied over this rule.
        /// </summary>
        public bool StopIfTrue
        {
            get { return  this.stopIfTrue; }
            set
            {
                this.OnPropertyChanging("StopIfTure");
                this.stopIfTrue = value;
                this.OnPropertyChanged("StopIfTure");
            }
        }

        /// <summary>
        /// Gets or sets the style for the rule.
        /// </summary>
        public StyleInfo Style
        {
            get { return  this.style; }
            set
            {
                this.OnPropertyChanging("Style");
                if (this.style != null)
                {
                    this.style.PropertyChanged -= new PropertyChangedEventHandler(this.OnStylePropertyChanged);
                }
                this.style = value;
                if (this.style != null)
                {
                    this.style.PropertyChanged += new PropertyChangedEventHandler(this.OnStylePropertyChanged);
                }
                this.OnPropertyChanged("Style");
            }
        }
    }
}

