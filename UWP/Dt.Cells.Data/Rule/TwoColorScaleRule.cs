#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the two color scale rule.
    /// </summary>
    public sealed class TwoColorScaleRule : ScaleRule
    {
        IActualValue lastActualValue;

        /// <summary>
        /// Constructs a two color scale rule.
        /// </summary>
        public TwoColorScaleRule()
        {
            this.Init();
        }

        internal TwoColorScaleRule(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor) : base(new ScaleValue(minType, minValue), null, new ScaleValue(maxType, maxValue))
        {
            base.expected[0] = minColor;
            base.expected[2] = maxColor;
        }

        /// <summary>
        /// Creates a new two color scale with the specified parameters.
        /// </summary>
        /// <param name="minType">The minimum scale type.</param>
        /// <param name="minValue">The minimum scale value.</param>
        /// <param name="minColor">The minimum scale color.</param>
        /// <param name="maxType">The maximum scale type.</param>
        /// <param name="maxValue">The maximum scale value.</param>
        /// <param name="maxColor">The maximum scale color.</param>
        /// <returns>The new two color scale rule.</returns>
        public static TwoColorScaleRule Create(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor)
        {
            return new TwoColorScaleRule(minType, minValue, minColor, maxType, maxValue, maxColor);
        }

        /// <summary>
        /// Returns a specified value of the rule if the cell satisfies the condition.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The row index.</param>
        /// <param name="baseColumn">The column index.</param>
        /// <param name="actual">The current value.</param>
        /// <returns>Returns the conditional number value.</returns>
        public override object Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            this.lastActualValue = actual;
            object obj2 = actual.GetValue(baseRow, baseColumn);
            if (obj2 != null)
            {
                double num;
                try
                {
                    num = ConditionValueConverter.ToDouble(obj2);
                }
                catch (InvalidCastException)
                {
                    return null;
                }
                double? nullable = base.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                double? nullable2 = base.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                if (nullable.HasValue && nullable2.HasValue)
                {
                    return ScaleRule.EvaluateColor(ScaleRule.Evaluate2Scale(num, nullable.Value, nullable2.Value), this.MinimumColor, this.MaximumColor);
                }
            }
            return null;
        }

        void Init()
        {
            base.scales = new ScaleValue[3];
            base.scales[0] = new ScaleValue(ScaleValueType.LowestValue, null);
            base.scales[2] = new ScaleValue(ScaleValueType.HighestValue, null);
            base.expected = new object[3];
            base.expected[0] = Colors.Transparent;
            base.expected[2] = Windows.UI.Color.FromArgb(0xff, 0x63, 190, 0x7b);
        }

        internal override bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (base.condition == null)
            {
                return false;
            }
            this.lastActualValue = actual;
            object obj2 = actual.GetValue(baseRow, baseColumn);
            if (obj2 != null)
            {
                try
                {
                    ConditionValueConverter.ToDouble(obj2);
                }
                catch (InvalidCastException)
                {
                    return false;
                }
                double? nullable = base.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                double? nullable2 = base.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                if (nullable.HasValue && nullable2.HasValue)
                {
                    return true;
                }
            }
            return true;
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
                if (name != "Type0")
                {
                    if (name != "Type2")
                    {
                        if (name == "Value0")
                        {
                            base.scales[0] = new ScaleValue(base.scales[0].Type, Serializer.DeserializeObj(null, reader));
                            return;
                        }
                        if (name == "Value2")
                        {
                            base.scales[2] = new ScaleValue(base.scales[2].Type, Serializer.DeserializeObj(null, reader));
                            return;
                        }
                        if (name == "Color0")
                        {
                            base.expected[0] = Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                            return;
                        }
                        if (name == "Color2")
                        {
                            base.expected[2] = Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                        }
                        return;
                    }
                }
                else
                {
                    base.scales[0] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[0].Value);
                    return;
                }
                base.scales[2] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[2].Value);
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            Serializer.SerializeObj(base.scales[0].Type, "Type0", writer);
            Serializer.SerializeObj(base.scales[2].Type, "Type2", writer);
            Serializer.WriteStartObj("Value0", writer);
            Serializer.WriteTypeAttr(base.scales[0].Value, writer);
            Serializer.SerializeObj(base.scales[0].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("Value2", writer);
            Serializer.WriteTypeAttr(base.scales[2].Value, writer);
            Serializer.SerializeObj(base.scales[2].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.SerializeObj(base.expected[0], "Color0", writer);
            Serializer.SerializeObj(base.expected[2], "Color2", writer);
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.Init();
        }

        /// <summary>
        /// Gets or sets the maximum color scale.
        /// </summary>
        public Windows.UI.Color MaximumColor
        {
            get { return  (Windows.UI.Color) base.expected[2]; }
            set
            {
                base.expected[2] = value;
                this.OnPropertyChanged("MaximumColor");
            }
        }

        /// <summary>
        /// Gets or sets the maximum scale type.
        /// </summary>
        public ScaleValueType MaximumType
        {
            get { return  base.scales[2].Type; }
            set
            {
                base.scales[2] = new ScaleValue(value, base.scales[2].Value);
                this.OnPropertyChanged("MaximumType");
            }
        }

        /// <summary>
        /// Gets or sets the maximum scale value.
        /// </summary>
        public object MaximumValue
        {
            get { return  base.scales[2].Value; }
            set
            {
                base.scales[2] = new ScaleValue(base.scales[2].Type, value);
                this.OnPropertyChanged("MaximumValue");
            }
        }

        /// <summary>
        /// Gets or sets the minimum scale color.
        /// </summary>
        public Windows.UI.Color MinimumColor
        {
            get { return  (Windows.UI.Color) base.expected[0]; }
            set
            {
                base.expected[0] = value;
                this.OnPropertyChanged("MinimumColor");
            }
        }

        /// <summary>
        /// Gets or sets the type of minimum scale.
        /// </summary>
        public ScaleValueType MinimumType
        {
            get { return  base.scales[0].Type; }
            set
            {
                base.scales[0] = new ScaleValue(value, base.scales[0].Value);
                this.OnPropertyChanged("MinimumType");
            }
        }

        /// <summary>
        /// Gets or sets the minimum scale value.
        /// </summary>
        public object MinimumValue
        {
            get { return  base.scales[0].Value; }
            set
            {
                base.scales[0] = new ScaleValue(base.scales[0].Type, value);
                this.OnPropertyChanged("MinimumValue");
            }
        }
    }
}

