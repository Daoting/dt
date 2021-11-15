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
using System.Xml;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a color condition.
    /// </summary>
    public sealed class ColorCondition : ConditionBase
    {
        ColorCompareType compareType;

        /// <summary>
        /// Creates a new style condition.
        /// </summary>
        public ColorCondition() : this(ColorCompareType.BackgroundColor, Colors.Transparent)
        {
        }

        /// <summary>
        /// Creates a new style condition with the specified comparison type and expected color.
        /// </summary>
        /// <param name="compareType">The style comparison type.</param>
        /// <param name="expected">The expected color.</param>
        internal ColorCondition(ColorCompareType compareType, Windows.UI.Color expected) : base(expected, null)
        {
            this.compareType = compareType;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            ColorCondition condition = base.Clone() as ColorCondition;
            condition.compareType = this.compareType;
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
            switch (this.compareType)
            {
                case ColorCompareType.BackgroundColor:
                    return this.IsEqualsBackgroundColor(actualObj);

                case ColorCompareType.ForegroundColor:
                    return this.IsEqualsForegroundColor(actualObj);
            }
            return false;
        }

        /// <summary>
        /// Creates a color condition from the color.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected color.</param>
        /// <returns>The ColorCondition object.</returns>
        public static ColorCondition FromColor(ColorCompareType compareType, Windows.UI.Color expected)
        {
            return new ColorCondition(compareType, expected);
        }

        bool IsEqualsBackgroundColor(IActualValue actualObj)
        {
            Windows.UI.Color? nullable = base.GetExpectedColor(null, 0, 0);
            if (nullable.HasValue)
            {
                Windows.UI.Color? backgroundColor = actualObj.GetBackgroundColor();
                if (backgroundColor.HasValue)
                {
                    return ((((backgroundColor.Value.A == nullable.Value.A) && (backgroundColor.Value.R == nullable.Value.R)) && (backgroundColor.Value.G == nullable.Value.G)) && (backgroundColor.Value.B == nullable.Value.B));
                }
                Windows.UI.Color? defaultBackgroundColor = actualObj.GetDefaultBackgroundColor();
                return (nullable.Value == (defaultBackgroundColor.HasValue ? defaultBackgroundColor.Value : Colors.Transparent));
            }
            return this.IgnoreBlank;
        }

        bool IsEqualsForegroundColor(IActualValue actualObj)
        {
            Windows.UI.Color? nullable = base.GetExpectedColor(null, 0, 0);
            if (nullable.HasValue)
            {
                Windows.UI.Color? foregroundColor = actualObj.GetForegroundColor();
                if (foregroundColor.HasValue)
                {
                    return ((((foregroundColor.Value.A == nullable.Value.A) && (foregroundColor.Value.R == nullable.Value.R)) && (foregroundColor.Value.G == nullable.Value.G)) && (foregroundColor.Value.B == nullable.Value.B));
                }
                Windows.UI.Color? defaultForegroundColor = actualObj.GetDefaultForegroundColor();
                return (nullable.Value == (defaultForegroundColor.HasValue ? defaultForegroundColor.Value : Colors.Black));
            }
            return this.IgnoreBlank;
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Type"))
            {
                this.compareType = (ColorCompareType) Serializer.DeserializeObj(typeof(ColorCompareType), reader);
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.compareType != ColorCompareType.BackgroundColor)
            {
                Serializer.SerializeObj(this.compareType, "Type", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = ColorCompareType.BackgroundColor;
        }

        /// <summary>
        /// Gets the comparison type for this condition.
        /// </summary>
        /// <value>
        /// The comparison type for this condition.
        /// The default value is <see cref="T:Dt.Cells.Data.ColorCompareType">BackgroundColor</see>.
        /// </value>
        [DefaultValue(0)]
        public ColorCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get { return  typeof(Windows.UI.Color); }
        }
    }
}

