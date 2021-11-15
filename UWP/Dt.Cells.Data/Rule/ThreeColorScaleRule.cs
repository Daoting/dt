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
    /// Represents the three color scale rule.
    /// </summary>
    public sealed class ThreeColorScaleRule : ScaleRule
    {
        IActualValue lastActualValue;

        /// <summary>
        /// Constructs a three color scale.
        /// </summary>
        public ThreeColorScaleRule()
        {
            this.Init();
        }

        internal ThreeColorScaleRule(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType midType, object midValue, Windows.UI.Color midColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor) : base(new ScaleValue(minType, minValue), new ScaleValue(midType, midValue), new ScaleValue(maxType, maxValue))
        {
            base.Expected[0] = minColor;
            base.Expected[1] = midColor;
            base.Expected[2] = maxColor;
        }

        /// <summary>
        /// Creates a new three color scale with the specified parameters.
        /// </summary>
        /// <param name="minType">The minimum scale type.</param>
        /// <param name="minValue">The minimum scale value.</param>
        /// <param name="minColor">The minimum scale color.</param>
        /// <param name="midType">The midpoint scale type.</param>
        /// <param name="midValue">The midpoint scale value.</param>
        /// <param name="midColor">The midpoint scale color.</param> 
        /// <param name="maxType">The maximum scale type.</param>
        /// <param name="maxValue">The maximum scale value.</param>
        /// <param name="maxColor">The maximum scale color.</param>
        /// <returns>The new three color scale rule</returns>
        public static ThreeColorScaleRule Create(ScaleValueType minType, object minValue, Windows.UI.Color minColor, ScaleValueType midType, object midValue, Windows.UI.Color midColor, ScaleValueType maxType, object maxValue, Windows.UI.Color maxColor)
        {
            return new ThreeColorScaleRule(minType, minValue, minColor, midType, midValue, midColor, maxType, maxValue, maxColor);
        }

        /// <summary>
        /// Returns the specified value of the rule if the cell meets the condition.
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
                double? nullable2 = base.GetActualValue(evaluator, baseRow, baseColumn, 1, actual);
                double? nullable3 = base.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                if ((nullable.HasValue && nullable3.HasValue) && nullable2.HasValue)
                {
                    double? nullable4 = nullable;
                    double? nullable5 = nullable3;
                    if ((((double) nullable4.GetValueOrDefault()) > ((double) nullable5.GetValueOrDefault())) && (nullable4.HasValue & nullable5.HasValue))
                    {
                        return null;
                    }
                    double num4 = num;
                    double? nullable6 = nullable;
                    if ((num4 < ((double) nullable6.GetValueOrDefault())) && nullable6.HasValue)
                    {
                        return this.MinimumColor;
                    }
                    double num5 = num;
                    double? nullable7 = nullable3;
                    if ((num5 >= ((double) nullable7.GetValueOrDefault())) && nullable7.HasValue)
                    {
                        return this.MaximumColor;
                    }
                    if ((nullable.Value <= num) && (num <= nullable2.Value))
                    {
                        return ScaleRule.EvaluateColor(ScaleRule.Evaluate2Scale(num, nullable.Value, nullable2.Value), this.MinimumColor, this.MidpointColor);
                    }
                    return ScaleRule.EvaluateColor(ScaleRule.Evaluate2Scale(num, nullable2.Value, nullable3.Value), this.MidpointColor, this.MaximumColor);
                }
            }
            return null;
        }

        void Init()
        {
            base.scales = new ScaleValue[] { new ScaleValue(ScaleValueType.LowestValue, null), new ScaleValue(ScaleValueType.Percentile, (int) 50), new ScaleValue(ScaleValueType.HighestValue, null) };
            base.expected = new object[] { Windows.UI.Color.FromArgb(0xff, 0xf8, 0x69, 0x6b), Windows.UI.Color.FromArgb(0xff, 0xff, 0xeb, 0x84), Windows.UI.Color.FromArgb(0xff, 0x63, 190, 0x7b) };
        }

        internal override bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (base.condition != null)
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
                        return false;
                    }
                    double? nullable = base.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                    double? nullable2 = base.GetActualValue(evaluator, baseRow, baseColumn, 1, actual);
                    double? nullable3 = base.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                    if ((nullable.HasValue && nullable3.HasValue) && nullable2.HasValue)
                    {
                        double? nullable4 = nullable;
                        double? nullable5 = nullable3;
                        if ((((double) nullable4.GetValueOrDefault()) > ((double) nullable5.GetValueOrDefault())) && (nullable4.HasValue & nullable5.HasValue))
                        {
                            return false;
                        }
                        double num2 = num;
                        double? nullable6 = nullable;
                        if ((num2 >= ((double) nullable6.GetValueOrDefault())) || !nullable6.HasValue)
                        {
                            double num3 = num;
                            double? nullable7 = nullable3;
                            if ((num3 >= ((double) nullable7.GetValueOrDefault())) && nullable7.HasValue)
                            {
                                return true;
                            }
                            if ((nullable.Value <= num) && (num <= nullable2.Value))
                            {
                                ScaleRule.Evaluate2Scale(num, nullable.Value, nullable2.Value);
                                return true;
                            }
                            ScaleRule.Evaluate2Scale(num, nullable2.Value, nullable3.Value);
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            switch (reader.Name)
            {
                case "Type0":
                    base.scales[0] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[0].Value);
                    return;

                case "Type1":
                    base.scales[1] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[1].Value);
                    return;

                case "Type2":
                    base.scales[2] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[2].Value);
                    return;

                case "Value0":
                    base.scales[0] = new ScaleValue(base.scales[0].Type, Serializer.DeserializeObj(null, reader));
                    return;

                case "Value1":
                    base.scales[1] = new ScaleValue(base.scales[1].Type, Serializer.DeserializeObj(null, reader));
                    return;

                case "Value2":
                    base.scales[2] = new ScaleValue(base.scales[2].Type, Serializer.DeserializeObj(null, reader));
                    return;

                case "Color0":
                    base.expected[0] = Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "Color1":
                    base.expected[1] = Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "Color2":
                    base.expected[2] = Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            Serializer.SerializeObj(base.scales[0].Type, "Type0", writer);
            Serializer.SerializeObj(base.scales[1].Type, "Type1", writer);
            Serializer.SerializeObj(base.scales[2].Type, "Type2", writer);
            Serializer.WriteStartObj("Value0", writer);
            Serializer.WriteTypeAttr(base.scales[0].Value, writer);
            Serializer.SerializeObj(base.scales[0].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("Value1", writer);
            Serializer.WriteTypeAttr(base.scales[1].Value, writer);
            Serializer.SerializeObj(base.scales[1].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("Value2", writer);
            Serializer.WriteTypeAttr(base.scales[2].Value, writer);
            Serializer.SerializeObj(base.scales[2].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.SerializeObj(base.expected[0], "Color0", writer);
            Serializer.SerializeObj(base.expected[1], "Color1", writer);
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
        /// Gets or sets the maximum scale color.
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
        /// Gets or sets maximum scale type.
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
        /// Gets or sets the midpoint scale color.
        /// </summary>
        public Windows.UI.Color MidpointColor
        {
            get { return  (Windows.UI.Color) base.expected[1]; }
            set
            {
                base.expected[1] = value;
                this.OnPropertyChanged("MidpointColor");
            }
        }

        /// <summary>
        /// Gets or sets the midpoint scale type.
        /// </summary>
        public ScaleValueType MidpointType
        {
            get { return  base.scales[1].Type; }
            set
            {
                base.scales[1] = new ScaleValue(value, base.scales[1].Value);
                this.OnPropertyChanged("MidpointType");
            }
        }

        /// <summary>
        /// Gets or sets the midpoint scale value.
        /// </summary>
        public object MidpointValue
        {
            get
            {
                if (this.lastActualValue != null)
                {
                    if (this.MidpointType == ScaleValueType.HighestValue)
                    {
                        return base.GetHighestValue(this.lastActualValue);
                    }
                    if (this.MidpointType == ScaleValueType.LowestValue)
                    {
                        return base.GetLowestValue(this.lastActualValue);
                    }
                }
                return base.scales[1].Value;
            }
            set
            {
                base.scales[1] = new ScaleValue(base.scales[1].Type, value);
                this.OnPropertyChanged("MidpointValue");
            }
        }

        /// <summary>
        /// Gets or sets the minimum color scale.
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
        /// Gets or sets the minimum scale type.
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

