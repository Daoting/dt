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
    /// Represents an average condition.
    /// </summary>
    public sealed class AverageCondition : ConditionBase, IRangesDependent
    {
        ICellRange[] ranges;
        ValueObject stdValueObject;
        AverageConditionType type;

        /// <summary>
        /// Creates a new average condition.
        /// </summary>
        public AverageCondition() : this(AverageConditionType.Above, null)
        {
        }

        /// <summary>
        /// Creates a new average condition of the specified type for the specified cell ranges.
        /// </summary>
        /// <param name="type">The average condition type.</param>
        /// <param name="ranges"></param>
        internal AverageCondition(AverageConditionType type, ICellRange[] ranges) : base(null, null)
        {
            this.type = type;
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
            AverageCondition condition = base.Clone() as AverageCondition;
            condition.type = this.type;
            condition.ranges = ConditionBase.CloneRanges(this.ranges);
            return condition;
        }

        /// <summary>
        /// Creates an AverageCondition object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="ranges">The ranges.</param>
        /// <returns>The AverageCondition object.</returns>
        public static AverageCondition Create(AverageConditionType type, ICellRange[] ranges)
        {
            return new AverageCondition(type, ranges);
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
            if (this.IgnoreBlank && (obj2 == null))
            {
                return true;
            }
            this.RebuildFormula(evaluator);
            double? nullable = base.GetExpectedDouble(evaluator, baseRow, baseColumn);
            double? nullable2 = null;
            object val = (this.stdValueObject != null) ? this.stdValueObject.GetValue(evaluator, baseRow, baseColumn) : null;
            if (val != null)
            {
                nullable2 = ConditionValueConverter.TryDouble(val);
            }
            if (ConditionValueConverter.IsNumber(obj2))
            {
                double num = ConditionValueConverter.ToDouble(obj2);
                if (nullable.HasValue)
                {
                    switch (this.type)
                    {
                        case AverageConditionType.Above:
                            return (num > nullable.Value);

                        case AverageConditionType.Below:
                            return (num < nullable.Value);

                        case AverageConditionType.EqualOrAbove:
                            return (num >= nullable.Value);

                        case AverageConditionType.EqualOrBelow:
                            return (num <= nullable.Value);

                        case AverageConditionType.Above1StdDev:
                        {
                            this.stdValueObject.GetValue(evaluator, baseRow, baseColumn);
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num2 = num;
                            double? nullable3 = nullable;
                            double num3 = nullable2.Value;
                            double? nullable5 = nullable3.HasValue ? new double?(((double) nullable3.GetValueOrDefault()) + num3) : null;
                            if (num2 <= ((double) nullable5.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable5.HasValue;
                        }
                        case AverageConditionType.Below1StdDev:
                        {
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num4 = num;
                            double? nullable6 = nullable;
                            double num5 = nullable2.Value;
                            double? nullable8 = nullable6.HasValue ? new double?(((double) nullable6.GetValueOrDefault()) - num5) : null;
                            if (num4 >= ((double) nullable8.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable8.HasValue;
                        }
                        case AverageConditionType.Above2StdDev:
                        {
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num6 = num;
                            double? nullable9 = nullable;
                            double num7 = 2.0 * nullable2.Value;
                            double? nullable11 = nullable9.HasValue ? new double?(((double) nullable9.GetValueOrDefault()) + num7) : null;
                            if (num6 <= ((double) nullable11.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable11.HasValue;
                        }
                        case AverageConditionType.Below2StdDev:
                        {
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num8 = num;
                            double? nullable12 = nullable;
                            double num9 = 2.0 * nullable2.Value;
                            double? nullable14 = nullable12.HasValue ? new double?(((double) nullable12.GetValueOrDefault()) - num9) : null;
                            if (num8 >= ((double) nullable14.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable14.HasValue;
                        }
                        case AverageConditionType.Above3StdDev:
                        {
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num10 = num;
                            double? nullable15 = nullable;
                            double num11 = 3.0 * nullable2.Value;
                            double? nullable17 = nullable15.HasValue ? new double?(((double) nullable15.GetValueOrDefault()) + num11) : null;
                            if (num10 <= ((double) nullable17.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable17.HasValue;
                        }
                        case AverageConditionType.Below3StdDev:
                        {
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            double num12 = num;
                            double? nullable18 = nullable;
                            double num13 = 3.0 * nullable2.Value;
                            double? nullable20 = nullable18.HasValue ? new double?(((double) nullable18.GetValueOrDefault()) - num13) : null;
                            if (num12 >= ((double) nullable20.GetValueOrDefault()))
                            {
                                return false;
                            }
                            return nullable20.HasValue;
                        }
                    }
                }
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
            List<CellRange> list = new List<CellRange>();
            string name = reader.Name;
            if (name == null)
            {
                goto Label_0068;
            }
            if (name != "Ranges")
            {
                if (name == "Type")
                {
                    goto Label_004D;
                }
                goto Label_0068;
            }
            using (XmlReader reader2 = Serializer.ExtractNode(reader))
            {
                Serializer.DeserializeList((IList) list, reader2);
                goto Label_0068;
            }
        Label_004D:
            this.type = (AverageConditionType) Serializer.DeserializeObj(typeof(AverageConditionType), reader);
        Label_0068:
            this.ranges = list.ToArray();
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.type != AverageConditionType.Above)
            {
                Serializer.SerializeObj(this.type, "Type", writer);
            }
            if (this.ranges != null)
            {
                Serializer.WriteStartObj("Ranges", writer);
                Serializer.SerializeList(this.ranges, writer);
                Serializer.WriteEndObj(writer);
            }
        }

        void RebuildFormula(ICalcEvaluator evaluator)
        {
            base.ClearExpected();
            this.stdValueObject = null;
            if (this.ranges != null)
            {
                base.ExpectedExpression = evaluator.CreateExpression("CalcAverageFunction", this.ranges);
                switch (this.type)
                {
                    case AverageConditionType.Above1StdDev:
                    case AverageConditionType.Below1StdDev:
                    case AverageConditionType.Above2StdDev:
                    case AverageConditionType.Below2StdDev:
                    case AverageConditionType.Above3StdDev:
                    case AverageConditionType.Below3StdDev:
                    {
                        object expression = evaluator.CreateExpression("CalcStDevFunction", this.ranges);
                        this.stdValueObject = ValueObject.FromExpression(expression);
                        break;
                    }
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.type = AverageConditionType.Above;
            this.ranges = null;
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
        /// Gets the average condition type.
        /// </summary>
        /// <value>
        /// Returns the type of average condition.
        /// The default value is <see cref="T:Dt.Cells.Data.AverageConditionType">Above</see>.
        /// </value>
        [DefaultValue(0)]
        public AverageConditionType Type
        {
            get { return  this.type; }
            set { this.type = value; }
        }
    }
}

