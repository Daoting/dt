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
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a relation condition.
    /// </summary>
    public sealed class RelationCondition : ConditionBase
    {
        RelationCompareType compareType;
        ConditionBase item1;
        ConditionBase item2;

        /// <summary>
        /// Creates a new relation condition.
        /// </summary>
        public RelationCondition() : this(RelationCompareType.And, null, null)
        {
        }

        /// <summary>
        /// Creates a new relation condition with a specified relation type.
        /// </summary>
        /// <param name="compareType">The relation between the first and second condition.</param>
        /// <param name="item1">The first condition.</param>
        /// <param name="item2">The second condition.</param>
        internal RelationCondition(RelationCompareType compareType, ConditionBase item1, ConditionBase item2) : base(null, null)
        {
            this.compareType = compareType;
            this.item1 = item1;
            this.item2 = item2;
        }

        internal override void AdjustOffset(int rowOffset, int columnOffset, bool overrideOld = true)
        {
            base.AdjustOffset(rowOffset, columnOffset, overrideOld);
            if (this.item1 != null)
            {
                this.item1.AdjustOffset(rowOffset, columnOffset, overrideOld);
            }
            if (this.item2 != null)
            {
                this.item2.AdjustOffset(rowOffset, columnOffset, overrideOld);
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
            RelationCondition condition = base.Clone() as RelationCondition;
            condition.item1 = (this.item1 != null) ? (this.item1.Clone() as ConditionBase) : null;
            condition.item2 = (this.item2 != null) ? (this.item2.Clone() as ConditionBase) : null;
            condition.compareType = this.compareType;
            return condition;
        }

        /// <summary>
        /// Creates a RelationCondition object.
        /// </summary>
        /// <param name="compareType">Type of the comparison.</param>
        /// <param name="item1">The first condition.</param>
        /// <param name="item2">The second condition.</param>
        /// <returns>The RelationCondition object.</returns>
        public static RelationCondition Create(RelationCompareType compareType, ConditionBase item1, ConditionBase item2)
        {
            return new RelationCondition(compareType, item1, item2);
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
            switch (this.compareType)
            {
                case RelationCompareType.Or:
                    if (!((this.item1 != null) && this.item1.Evaluate(evaluator, baseRow, baseColumn, actualObj)))
                    {
                        return ((this.item2 != null) && this.item2.Evaluate(evaluator, baseRow, baseColumn, actualObj));
                    }
                    return true;

                case RelationCompareType.And:
                    if ((this.item1 != null) && this.item1.Evaluate(evaluator, baseRow, baseColumn, actualObj))
                    {
                        if (!((this.item2 != null) && this.item2.Evaluate(evaluator, baseRow, baseColumn, actualObj)))
                        {
                            return false;
                        }
                        return true;
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "Type")
                {
                    if (name != "Item1")
                    {
                        if (name == "Item2")
                        {
                            this.item2 = Serializer.DeserializeObj(null, reader) as ConditionBase;
                        }
                        return;
                    }
                }
                else
                {
                    this.compareType = (RelationCompareType) Serializer.DeserializeObj(typeof(RelationCompareType), reader);
                    return;
                }
                this.item1 = Serializer.DeserializeObj(null, reader) as ConditionBase;
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.compareType != RelationCompareType.And)
            {
                Serializer.SerializeObj(this.compareType, "Type", writer);
            }
            if (this.item1 != null)
            {
                Serializer.WriteStartObj("Item1", writer);
                Serializer.WriteTypeAttr(this.item1, writer);
                Serializer.SerializeObj(this.item1, null, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.item2 != null)
            {
                Serializer.WriteStartObj("Item2", writer);
                Serializer.WriteTypeAttr(this.item2, writer);
                Serializer.SerializeObj(this.item2, null, writer);
                Serializer.WriteEndObj(writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = RelationCompareType.And;
            this.item1 = null;
            this.item2 = null;
        }

        /// <summary>
        /// Gets the relation type for the first and second conditions.
        /// </summary>
        /// <value>
        /// The relation type between the first and second conditions.
        /// The default value is <see cref="T:Dt.Cells.Data.RelationCompareType">And</see>.
        /// </value>
        [DefaultValue(1)]
        public RelationCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets or sets whether the condition ignores empty values.
        /// </summary>
        /// <value>
        /// <c>true</c> if this condition ignores empty values; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public override bool IgnoreBlank
        {
            get { return  base.IgnoreBlank; }
            set
            {
                if (this.Item1 != null)
                {
                    this.Item1.IgnoreBlank = value;
                }
                if (this.Item2 != null)
                {
                    this.Item2.IgnoreBlank = value;
                }
                base.IgnoreBlank = value;
            }
        }

        /// <summary>
        /// Gets the first condition.
        /// </summary>
        /// <value>The first condition. The default value is null.</value>
        [DefaultValue((string) null)]
        public ConditionBase Item1
        {
            get { return  this.item1; }
            set { this.item1 = value; }
        }

        /// <summary>
        /// Gets the second condition.
        /// </summary>
        /// <value>The second condition. The default value is null.</value>
        [DefaultValue((string) null)]
        public ConditionBase Item2
        {
            get { return  this.item2; }
            set { this.item2 = value; }
        }
    }
}

