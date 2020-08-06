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
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the data bar set rule.
    /// </summary>
    public sealed class DataBarRule : ScaleRule
    {
        Windows.UI.Color axisColor;
        DataBarAxisPosition axisPosition;
        Windows.UI.Color borderColor;
        Windows.UI.Color color;
        BarDirection direction;
        bool gradient;
        IActualValue lastActualValue;
        double maxScale;
        double minScale;
        Windows.UI.Color negativeBorderColor;
        Windows.UI.Color negativeFillColor;
        bool showBarOnly;
        bool showBorder;
        bool useNegativeBorderColor;
        bool useNegativeFillColor;

        /// <summary>
        /// Constructs a data bar rule.
        /// </summary>
        public DataBarRule()
        {
            this.Init(Windows.UI.Color.FromArgb(0xff, 0x63, 0x8e, 0xc6));
            base.scales[0] = new ScaleValue(ScaleValueType.Automin, null);
            base.scales[2] = new ScaleValue(ScaleValueType.Automax, null);
        }

        internal DataBarRule(ScaleValueType minType, object minValue, ScaleValueType maxType, object maxValue, Windows.UI.Color color) : base(new ScaleValue(minType, minValue), null, new ScaleValue(maxType, maxValue))
        {
            this.Init(color);
        }

        internal double? CalcuteMaxValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, int index, IActualValue actual)
        {
            ScaleValue value2 = base.scales[index];
            CellRange range = null;
            if (value2 != null)
            {
                if ((value2.Type != ScaleValueType.Formula) && (value2.Type != ScaleValueType.Percentile))
                {
                    return base.GetActualValue(evaluator, baseRow, baseColumn, 2, actual);
                }
                for (int i = 0; i < base.Ranges.Length; i++)
                {
                    if (base.Ranges[i].IntersectRow(baseRow))
                    {
                        range = base.Ranges[i];
                        break;
                    }
                }
                if (range != null)
                {
                    return base.GetActualValue(evaluator, range.Row, range.Column, 2, actual);
                }
            }
            return null;
        }

        internal double? CalcuteMinValue(ICalcEvaluator evaluator, int baseRow, int baseColumn, int index, IActualValue actual)
        {
            ScaleValue value2 = base.scales[index];
            CellRange range = null;
            if (value2 != null)
            {
                if ((value2.Type != ScaleValueType.Formula) && (value2.Type != ScaleValueType.Percentile))
                {
                    return base.GetActualValue(evaluator, baseRow, baseColumn, 0, actual);
                }
                for (int i = 0; i < base.Ranges.Length; i++)
                {
                    if (base.Ranges[i].IntersectRow(baseRow))
                    {
                        range = base.Ranges[i];
                        break;
                    }
                }
                if (range != null)
                {
                    return base.GetActualValue(evaluator, range.Row, range.Column, 0, actual);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new object of the data bar set rule.
        /// </summary>
        /// <param name="minType">The minimum type.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxType">The maximum type.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="Color">The color.</param>
        /// <returns>A new data bar set rule.</returns>
        public static DataBarRule Create(ScaleValueType minType, object minValue, ScaleValueType maxType, object maxValue, Windows.UI.Color Color)
        {
            return new DataBarRule(minType, minValue, maxType, maxValue, Color);
        }

        /// <summary>
        /// Returns a drawing object based on a specified parameter.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actual">The current value.</param>
        /// <returns>The data bar object.</returns>
        public override object Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            this.lastActualValue = actual;
            object val = actual.GetValue(baseRow, baseColumn);
            if (val != null)
            {
                double? nullable = ConditionValueConverter.TryDouble(val);
                if (!nullable.HasValue)
                {
                    return null;
                }
                double currentValue = nullable.Value;
                double? nullable2 = this.CalcuteMinValue(evaluator, baseRow, baseColumn, 0, actual);
                double? nullable3 = this.CalcuteMaxValue(evaluator, baseRow, baseColumn, 2, actual);
                if (nullable2.HasValue && nullable3.HasValue)
                {
                    double? nullable5 = nullable2;
                    double? nullable6 = nullable3;
                    if ((((double) nullable5.GetValueOrDefault()) > ((double) nullable6.GetValueOrDefault())) && (nullable5.HasValue & nullable6.HasValue))
                    {
                        double? nullable4 = nullable3;
                        nullable3 = nullable2;
                        nullable2 = nullable4;
                    }
                    double axisScale = -1.0;
                    double scale = this.EvaluateScale(currentValue, nullable2.Value, nullable3.Value, out axisScale);
                    Windows.UI.Color fillColor = ((currentValue < 0.0) && this.useNegativeFillColor) ? this.negativeFillColor : this.color;
                    return new DataBarDrawingObject(baseRow, baseColumn, fillColor, ((currentValue < 0.0) && this.useNegativeBorderColor) ? this.negativeBorderColor : this.borderColor, this.showBorder, this.axisColor, this.gradient, this.direction, axisScale, scale, this.showBarOnly);
                }
            }
            return null;
        }

        double EvaluateAutoScale(double currentValue, double minValue, double maxValue, out double axisScale)
        {
            double num = Math.Abs((double) (maxValue - minValue));
            if ((maxValue > 0.0) && (minValue >= 0.0))
            {
                axisScale = 0.0;
                if (minValue == maxValue)
                {
                    if (currentValue < minValue)
                    {
                        return 0.0;
                    }
                    if (currentValue > maxValue)
                    {
                        return 1.0;
                    }
                    return 0.5;
                }
                if (currentValue <= minValue)
                {
                    return 0.0;
                }
                if (currentValue >= maxValue)
                {
                    return 1.0;
                }
                return ((currentValue - minValue) / num);
            }
            if ((maxValue > 0.0) && (minValue < 0.0))
            {
                axisScale = Math.Abs(minValue) / num;
                if (currentValue >= maxValue)
                {
                    return (1.0 - axisScale);
                }
                if (currentValue <= minValue)
                {
                    return -axisScale;
                }
                if (currentValue == 0.0)
                {
                    return 0.0;
                }
                return ((currentValue - 0.0) / num);
            }
            if ((maxValue <= 0.0) && (minValue < 0.0))
            {
                axisScale = 1.0;
                if (maxValue == minValue)
                {
                    if (currentValue < minValue)
                    {
                        return -1.0;
                    }
                    if (currentValue > maxValue)
                    {
                        return 0.0;
                    }
                    return -0.5;
                }
                if (currentValue >= maxValue)
                {
                    return 0.0;
                }
                if (currentValue <= minValue)
                {
                    return -1.0;
                }
                return ((currentValue - maxValue) / num);
            }
            if ((maxValue == 0.0) && (minValue == 0.0))
            {
                axisScale = 0.5;
                if (currentValue > 0.0)
                {
                    return 0.5;
                }
                if (currentValue < 0.0)
                {
                    return -0.5;
                }
                return 0.0;
            }
            axisScale = -1.0;
            return -1.0;
        }

        double EvaluateMidScale(double currentValue, double minValue, double maxValue, out double axisScale)
        {
            axisScale = 0.5;
            double num = Math.Abs((double) (maxValue - minValue));
            if ((maxValue > 0.0) && (minValue >= 0.0))
            {
                if (maxValue == minValue)
                {
                    return 0.5;
                }
                if (currentValue >= maxValue)
                {
                    return 0.5;
                }
                if (currentValue <= minValue)
                {
                    return ((minValue / maxValue) * 0.5);
                }
                return (Math.Abs((double) (currentValue / maxValue)) * 0.5);
            }
            if ((maxValue > 0.0) && (minValue < 0.0))
            {
                double num2 = (maxValue > Math.Abs(minValue)) ? 0.5 : (maxValue / num);
                double num3 = (maxValue > Math.Abs(minValue)) ? (minValue / num) : -0.5;
                if (currentValue > 0.0)
                {
                    if (currentValue >= maxValue)
                    {
                        return num2;
                    }
                    return ((currentValue / maxValue) * num2);
                }
                if (currentValue >= 0.0)
                {
                    return 0.0;
                }
                if (currentValue <= minValue)
                {
                    return num3;
                }
                return ((currentValue / minValue) * num3);
            }
            if ((maxValue <= 0.0) && (minValue < 0.0))
            {
                if (maxValue == minValue)
                {
                    return -0.5;
                }
                if (currentValue >= maxValue)
                {
                    return ((-maxValue / minValue) * 0.5);
                }
                if (currentValue <= minValue)
                {
                    return -0.5;
                }
                return ((-currentValue / minValue) * 0.5);
            }
            if ((maxValue == 0.0) && (minValue == 0.0))
            {
                return 0.0;
            }
            axisScale = -1.0;
            return -1.0;
        }

        double EvaluateNoneScale(double currentValue, double minValue, double maxValue, out double axisScale)
        {
            axisScale = 0.0;
            if (maxValue == minValue)
            {
                if (currentValue < minValue)
                {
                    return 0.0;
                }
                if (currentValue > maxValue)
                {
                    return 1.0;
                }
                return 0.5;
            }
            if (currentValue >= maxValue)
            {
                return 1.0;
            }
            if (currentValue <= minValue)
            {
                return 0.0;
            }
            return ((currentValue - minValue) / (maxValue - minValue));
        }

        double EvaluateScale(double currentValue, double minValue, double maxValue, out double axisScale)
        {
            if (this.axisPosition == DataBarAxisPosition.Automatic)
            {
                return this.EvaluateAutoScale(currentValue, minValue, maxValue, out axisScale);
            }
            if (this.axisPosition == DataBarAxisPosition.CellMidPoint)
            {
                return this.EvaluateMidScale(currentValue, minValue, maxValue, out axisScale);
            }
            return this.EvaluateNoneScale(currentValue, minValue, maxValue, out axisScale);
        }

        void Init(Windows.UI.Color color)
        {
            this.minScale = 0.0;
            this.maxScale = 1.0;
            this.gradient = true;
            this.color = color;
            this.showBorder = false;
            this.borderColor = Colors.Black;
            this.direction = BarDirection.LeftToRight;
            this.negativeFillColor = Colors.Red;
            this.useNegativeFillColor = true;
            this.negativeBorderColor = Colors.Black;
            this.useNegativeBorderColor = false;
            this.axisPosition = DataBarAxisPosition.Automatic;
            this.axisColor = Colors.Black;
            this.showBarOnly = false;
        }

        internal override bool IsConditionEvaluateToTrue(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual)
        {
            if (base.condition != null)
            {
                this.lastActualValue = actual;
                object val = actual.GetValue(baseRow, baseColumn);
                if (val != null)
                {
                    double? nullable = ConditionValueConverter.TryDouble(val);
                    if (!nullable.HasValue)
                    {
                        return false;
                    }
                    double currentValue = nullable.Value;
                    double? nullable2 = this.CalcuteMinValue(evaluator, baseRow, baseColumn, 0, actual);
                    double? nullable3 = this.CalcuteMaxValue(evaluator, baseRow, baseColumn, 2, actual);
                    if (nullable2.HasValue && nullable3.HasValue)
                    {
                        double? nullable5 = nullable2;
                        double? nullable6 = nullable3;
                        if ((((double) nullable5.GetValueOrDefault()) > ((double) nullable6.GetValueOrDefault())) && (nullable5.HasValue & nullable6.HasValue))
                        {
                            double? nullable4 = nullable3;
                            nullable3 = nullable2;
                            nullable2 = nullable4;
                        }
                        double axisScale = -1.0;
                        this.EvaluateScale(currentValue, nullable2.Value, nullable3.Value, out axisScale);
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

                case "Type2":
                    base.scales[2] = new ScaleValue((ScaleValueType) Serializer.DeserializeObj(typeof(ScaleValueType), reader), base.scales[2].Value);
                    return;

                case "Value0":
                    base.scales[0] = new ScaleValue(base.scales[0].Type, Serializer.DeserializeObj(null, reader));
                    return;

                case "Value2":
                    base.scales[2] = new ScaleValue(base.scales[2].Type, Serializer.DeserializeObj(null, reader));
                    return;

                case "MaxScale":
                    this.maxScale = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "MinScale":
                    this.minScale = (double) ((double) Serializer.DeserializeObj(typeof(double), reader));
                    return;

                case "Gradient":
                    this.gradient = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "Color":
                    this.color = (Windows.UI.Color) Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "ShowBorder":
                    this.showBorder = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "BorderColor":
                    this.borderColor = (Windows.UI.Color) Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "BarDirection":
                    this.direction = (BarDirection) Serializer.DeserializeObj(typeof(BarDirection), reader);
                    return;

                case "NegativeFillColor":
                    this.negativeFillColor = (Windows.UI.Color) Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "UseNegativeFillColor":
                    this.useNegativeFillColor = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "NegativeBorderColor":
                    this.negativeBorderColor = (Windows.UI.Color) Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "UseNegativeBorderColor":
                    this.useNegativeBorderColor = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "AxisPosition":
                    this.axisPosition = (DataBarAxisPosition) Serializer.DeserializeObj(typeof(DataBarAxisPosition), reader);
                    return;

                case "AxisColor":
                    this.axisColor = (Windows.UI.Color) Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                    return;

                case "ShowBarOnly":
                    this.showBarOnly = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
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
            Serializer.SerializeObj(base.scales[2].Type, "Type2", writer);
            Serializer.WriteStartObj("Value0", writer);
            Serializer.WriteTypeAttr(base.scales[0].Value, writer);
            Serializer.SerializeObj(base.scales[0].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.WriteStartObj("Value2", writer);
            Serializer.WriteTypeAttr(base.scales[2].Value, writer);
            Serializer.SerializeObj(base.scales[2].Value, null, writer);
            Serializer.WriteEndObj(writer);
            Serializer.SerializeObj((double) this.maxScale, "MaxScale", writer);
            Serializer.SerializeObj((double) this.minScale, "MinScale", writer);
            Serializer.SerializeObj((bool) this.gradient, "Gradient", writer);
            Serializer.SerializeObj(this.color, "Color", writer);
            Serializer.SerializeObj((bool) this.showBorder, "ShowBorder", writer);
            Serializer.SerializeObj(this.borderColor, "BorderColor", writer);
            Serializer.SerializeObj(this.direction, "BarDirection", writer);
            Serializer.SerializeObj(this.negativeFillColor, "NegativeFillColor", writer);
            Serializer.SerializeObj((bool) this.useNegativeFillColor, "UseNegativeFillColor", writer);
            Serializer.SerializeObj(this.negativeBorderColor, "NegativeBorderColor", writer);
            Serializer.SerializeObj((bool) this.useNegativeBorderColor, "UseNegativeBorderColor", writer);
            Serializer.SerializeObj(this.axisPosition, "AxisPosition", writer);
            Serializer.SerializeObj(this.axisColor, "AxisColor", writer);
            if (this.showBarOnly)
            {
                Serializer.SerializeObj((bool) this.showBarOnly, "ShowBarOnly", writer);
            }
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.Init(Windows.UI.Color.FromArgb(0xff, 0x63, 0x8e, 0xc6));
        }

        /// <summary>
        /// Gets or sets the axis color of the data bar set rule.
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "Black")]
        public Windows.UI.Color AxisColor
        {
            get { return  this.axisColor; }
            set
            {
                if (this.axisColor != value)
                {
                    this.axisColor = value;
                    this.OnPropertyChanged("AxisColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the axis positon.
        /// </summary>
        [DefaultValue(0)]
        public DataBarAxisPosition AxisPosition
        {
            get { return  this.axisPosition; }
            set
            {
                if (this.axisPosition != value)
                {
                    this.axisPosition = value;
                    this.OnPropertyChanged("AxisPosition");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the border.
        /// </summary>
        /// <value>
        /// The color of the border.
        /// </value>
        [DefaultValue(typeof(Windows.UI.Color), "Black")]
        public Windows.UI.Color BorderColor
        {
            get { return  this.borderColor; }
            set
            {
                if (this.borderColor != value)
                {
                    this.borderColor = value;
                    this.OnPropertyChanged("BorderColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the postive fill color of the data bar set rule.
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "99, 142, 198")]
        public Windows.UI.Color Color
        {
            get { return  this.color; }
            set
            {
                if (this.color != value)
                {
                    this.color = value;
                    this.OnPropertyChanged("Color");
                }
            }
        }

        /// <summary>
        /// Gets or sets the bar direction.
        /// </summary>
        [DefaultValue(0)]
        public BarDirection DataBarDirection
        {
            get { return  this.direction; }
            set
            {
                if (this.direction != value)
                {
                    this.direction = value;
                    this.OnPropertyChanged("DataBarDirection");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this is a gradient.
        /// </summary>
        /// <value>
        /// <c>true</c> if gradient; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool Gradient
        {
            get { return  this.gradient; }
            set
            {
                if (this.gradient != value)
                {
                    this.gradient = value;
                    this.OnPropertyChanged("Gradient");
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum type.
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
        /// Gets or sets the maximum value.
        /// </summary>
        public object MaximumValue
        {
            get
            {
                if (this.lastActualValue != null)
                {
                    if (this.MaximumType == ScaleValueType.HighestValue)
                    {
                        return base.GetHighestValue(this.lastActualValue);
                    }
                    if (this.MaximumType == ScaleValueType.LowestValue)
                    {
                        return base.GetLowestValue(this.lastActualValue);
                    }
                }
                return base.scales[2].Value;
            }
            set
            {
                base.scales[2] = new ScaleValue(base.scales[2].Type, value);
                this.OnPropertyChanged("MaximumValue");
            }
        }

        /// <summary>
        /// Gets or sets the max scale.
        /// </summary>
        internal double MaxScale
        {
            get { return  this.maxScale; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(ResourceStrings.ConditionalFormatDataBarRuleScaleOutOfRangeError, (object[]) new object[] { "MaxScale" }));
                }
                if (this.maxScale != value)
                {
                    this.maxScale = value;
                    this.OnPropertyChanged("MaxScale");
                }
            }
        }

        /// <summary>
        /// Gets or sets the minimum type.
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
        /// Gets or sets the minimum value.
        /// </summary>
        public object MinimumValue
        {
            get
            {
                if (this.lastActualValue != null)
                {
                    if (this.MinimumType == ScaleValueType.HighestValue)
                    {
                        return base.GetHighestValue(this.lastActualValue);
                    }
                    if (this.MinimumType == ScaleValueType.LowestValue)
                    {
                        return base.GetLowestValue(this.lastActualValue);
                    }
                }
                return base.scales[0].Value;
            }
            set
            {
                base.scales[0] = new ScaleValue(base.scales[0].Type, value);
                this.OnPropertyChanged("MinimumValue");
            }
        }

        /// <summary>
        /// Gets or sets the min scale.
        /// </summary>
        internal double MinScale
        {
            get { return  this.minScale; }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(ResourceStrings.ConditionalFormatDataBarRuleScaleOutOfRangeError, (object[]) new object[] { "MinScale" }));
                }
                if (this.minScale != value)
                {
                    this.minScale = value;
                    this.OnPropertyChanged("MinScale");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the negative border.
        /// </summary>
        /// <value>
        /// The color of the negative border.
        /// </value>
        [DefaultValue(typeof(Windows.UI.Color), "Black")]
        public Windows.UI.Color NegativeBorderColor
        {
            get { return  this.negativeBorderColor; }
            set
            {
                if (this.negativeBorderColor != value)
                {
                    this.negativeBorderColor = value;
                    this.OnPropertyChanged("NegativeBorderColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the negative fill.
        /// </summary>
        /// <value>
        /// The color of the negative fill.
        /// </value>
        [DefaultValue(typeof(Windows.UI.Color), "Red")]
        public Windows.UI.Color NegativeFillColor
        {
            get { return  this.negativeFillColor; }
            set
            {
                if (this.negativeFillColor != value)
                {
                    this.negativeFillColor = value;
                    this.OnPropertyChanged("NegativeFillColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the bar.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowBarOnly
        {
            get { return  this.showBarOnly; }
            set
            {
                if (this.showBarOnly != value)
                {
                    this.showBarOnly = value;
                    this.OnPropertyChanged("ShowBarOnly");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to paint the border.
        /// </summary>
        /// <value>
        /// <c>true</c> to paint the border; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool ShowBorder
        {
            get { return  this.showBorder; }
            set
            {
                if (this.showBorder != value)
                {
                    this.showBorder = value;
                    this.OnPropertyChanged("ShowBorder");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the negative border color is used to paint the border for the negative value.
        /// </summary>
        /// <value>
        /// <c>true</c> if the negative border color is used to paint the border for the negative value; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool UseNegativeBorderColor
        {
            get { return  this.useNegativeBorderColor; }
            set
            {
                if (this.useNegativeBorderColor != value)
                {
                    this.useNegativeBorderColor = value;
                    this.OnPropertyChanged("UseNegativeBorderColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the negative fill color is used to paint the negative value.
        /// </summary>
        /// <value>
        /// <c>true</c> if the negative fill color is used to paint the negative value; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        public bool UseNegativeFillColor
        {
            get { return  this.useNegativeFillColor; }
            set
            {
                if (this.useNegativeFillColor != value)
                {
                    this.useNegativeFillColor = value;
                    this.OnPropertyChanged("UseNegativeFillColor");
                }
            }
        }
    }
}

