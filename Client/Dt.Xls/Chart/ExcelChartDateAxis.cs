#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the category axis of the chart.
    /// </summary>
    public class ExcelChartDateAxis : IExcelChartDateAxis, IExcelChartAxis
    {
        private bool _isAutoMajorTimeUnit = true;
        private bool _isAutoMajorUnit = true;
        private bool _isAutoMinorTimeUnit = true;
        private bool _isAutoMinorUnit = true;
        private AxisTimeUnit _majorTimeUnit;
        private int _majorUnit = 1;
        private AxisTimeUnit _minorTimeUnit;
        private int _minorUnit = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelChartDateAxis" /> class.
        /// </summary>
        public ExcelChartDateAxis()
        {
            this.Type = ExcelChartAxisType.Date;
            this.IsAutomaticCategoryAxis = true;
            this.Crosses = AxisCrosses.AutoZero;
            this.MajorTickMark = TickMark.Cross;
            this.MinorTickMark = TickMark.Cross;
            this.CrosssAt = double.NaN;
            this.Delete = true;
        }

        private string GetTickLabelPositionStringForExcel(Dt.Xls.Chart.TickLabelPosition position)
        {
            switch (position)
            {
                case Dt.Xls.Chart.TickLabelPosition.None:
                    return "none";

                case Dt.Xls.Chart.TickLabelPosition.High:
                    return "high";

                case Dt.Xls.Chart.TickLabelPosition.Low:
                    return "low";

                case Dt.Xls.Chart.TickLabelPosition.NextTo:
                    return "nextTo";
            }
            return "none";
        }

        private string GetTickMarkStringForExcel(TickMark tickMark)
        {
            switch (tickMark)
            {
                case TickMark.None:
                    return "none";

                case TickMark.Inside:
                    return "in";

                case TickMark.Outside:
                    return "out";

                case TickMark.Cross:
                    return "cross";
            }
            return "none";
        }

        private Dt.Xls.Chart.TickLabelPosition ReadTickLabelPosition(string tickPosition)
        {
            if (tickPosition != "none")
            {
                if (tickPosition == "high")
                {
                    return Dt.Xls.Chart.TickLabelPosition.High;
                }
                if (tickPosition == "low")
                {
                    return Dt.Xls.Chart.TickLabelPosition.Low;
                }
                if (tickPosition == "nextTo")
                {
                    return Dt.Xls.Chart.TickLabelPosition.NextTo;
                }
            }
            return Dt.Xls.Chart.TickLabelPosition.None;
        }

        private TickMark ReadTickMark(string tickMark)
        {
            if (tickMark == "cross")
            {
                return TickMark.Cross;
            }
            if (tickMark == "in")
            {
                return TickMark.Inside;
            }
            if (tickMark == "out")
            {
                return TickMark.Outside;
            }
            return TickMark.None;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "axId")
                {
                    this.Id = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "scaling")
                {
                    Dt.Xls.Chart.Scaling scaling = new Dt.Xls.Chart.Scaling();
                    scaling.ReadXml(element);
                    this.Scaling = scaling;
                }
                else if (element.Name.LocalName == "delete")
                {
                    this.Delete = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "axPos")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", null))
                    {
                        case "b":
                            this.AxisPosition = Dt.Xls.Chart.AxisPosition.Bottom;
                            break;

                        case "l":
                            this.AxisPosition = Dt.Xls.Chart.AxisPosition.Left;
                            break;

                        case "r":
                            this.AxisPosition = Dt.Xls.Chart.AxisPosition.Right;
                            break;

                        case "t":
                            this.AxisPosition = Dt.Xls.Chart.AxisPosition.Top;
                            break;
                    }
                }
                else if (element.Name.LocalName == "majorGridlines")
                {
                    ExcelGridLine line = new ExcelGridLine();
                    line.ReadXml(element, mFolder, xFile);
                    this.MajorGridlines = line;
                }
                else if (element.Name.LocalName == "minorGridlines")
                {
                    ExcelGridLine line2 = new ExcelGridLine();
                    line2.ReadXml(element, mFolder, xFile);
                    this.MinorGridlines = line2;
                }
                else if (element.Name.LocalName == "title")
                {
                    ExcelChartTitle title = new ExcelChartTitle();
                    title.ReadXml(element, mFolder, xFile);
                    this.AxisTitle = title;
                }
                else if (element.Name.LocalName == "numFmt")
                {
                    Dt.Xls.Chart.NumberFormat format = ChartCommonSimpleNodeHelper.ReadNumberFormatNode(element);
                    this.NumberFormat = format.NumberFormatCode;
                    this.NumberFormatLinked = format.LinkToSource;
                }
                else if (element.Name.LocalName == "majorTickMark")
                {
                    this.MajorTickMark = this.ReadTickMark(element.GetAttributeValueOrDefaultOfStringType("val", "cross"));
                }
                else if (element.Name.LocalName == "minorTickMark")
                {
                    this.MinorTickMark = this.ReadTickMark(element.GetAttributeValueOrDefaultOfStringType("val", "cross"));
                }
                else if (element.Name.LocalName == "tickLblPos")
                {
                    this.TickLabelPosition = this.ReadTickLabelPosition(element.GetAttributeValueOrDefaultOfStringType("val", "none"));
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format2 = new ExcelChartFormat();
                    format2.ReadXml(element, mFolder, xFile);
                    this.ShapeFormat = format2;
                }
                else if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format3 = new ExcelTextFormat();
                    format3.ReadXml(element, mFolder, xFile);
                    this.TextFormat = format3;
                }
                else if (element.Name.LocalName == "crossAx")
                {
                    this.CrossAx = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "crosses")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "autoZero"))
                    {
                        case "autoZero":
                            this.Crosses = AxisCrosses.AutoZero;
                            break;

                        case "max":
                            this.Crosses = AxisCrosses.Max;
                            break;

                        case "min":
                            this.Crosses = AxisCrosses.Min;
                            break;
                    }
                }
                else if (element.Name.LocalName == "crossesAt")
                {
                    this.CrosssAt = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "auto")
                {
                    this.IsAutomaticCategoryAxis = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "lblOffset")
                {
                    this.LabelOffset = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "majorUnit")
                {
                    this._isAutoMajorUnit = false;
                    this._majorUnit = element.GetAttributeValueOrDefaultOfInt32Type("val", 1);
                }
                else if (element.Name.LocalName == "minorUnit")
                {
                    this._isAutoMinorUnit = false;
                    this._minorUnit = element.GetAttributeValueOrDefaultOfInt32Type("val", 1);
                }
                else if (element.Name.LocalName == "baseTimeUnit")
                {
                    AxisTimeUnit days = AxisTimeUnit.Days;
                    Enum.TryParse<AxisTimeUnit>(element.GetAttributeValueOrDefaultOfStringType("val", "days"), true, out days);
                    this.BaseTimeUnit = days;
                }
                else if (element.Name.LocalName == "majorTimeUnit")
                {
                    this._isAutoMajorTimeUnit = false;
                    AxisTimeUnit result = AxisTimeUnit.Days;
                    Enum.TryParse<AxisTimeUnit>(element.GetAttributeValueOrDefaultOfStringType("val", "days"), true, out result);
                    this.MajorTimeUnit = result;
                }
                else if (element.Name.LocalName == "minorTimeUnit")
                {
                    this._isAutoMinorTimeUnit = false;
                    AxisTimeUnit unit3 = AxisTimeUnit.Days;
                    Enum.TryParse<AxisTimeUnit>(element.GetAttributeValueOrDefaultOfStringType("val", "days"), true, out unit3);
                    this.MinorTimeUnit = unit3;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "dateAx", null);
            writer.WriteLeafElementWithAttribute("axId", null, "c", "val", ((int) this.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            if (this.Scaling != null)
            {
                (this.Scaling as Dt.Xls.Chart.Scaling).WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteLeafElementWithAttribute("delete", null, "c", "val", this.Delete ? "1" : "0");
            string str = "b";
            if (this.AxisPosition == Dt.Xls.Chart.AxisPosition.Left)
            {
                str = "l";
            }
            else if (this.AxisPosition == Dt.Xls.Chart.AxisPosition.Right)
            {
                str = "r";
            }
            else if (this.AxisPosition == Dt.Xls.Chart.AxisPosition.Top)
            {
                str = "t";
            }
            writer.WriteLeafElementWithAttribute("axPos", null, "c", "val", str);
            if (this.MajorGridlines != null)
            {
                (this.MajorGridlines as ExcelGridLine).WriteXml(writer, mFolder, chartFile, "majorGridlines");
            }
            if (this.MinorGridlines != null)
            {
                (this.MinorGridlines as ExcelGridLine).WriteXml(writer, mFolder, chartFile, "minorGridlines");
            }
            if (this.AxisTitle != null)
            {
                (this.AxisTitle as ExcelChartTitle).WriteXml(writer, mFolder, chartFile);
            }
            if (!string.IsNullOrWhiteSpace(this.NumberFormat) || !this.NumberFormatLinked)
            {
                Dt.Xls.Chart.NumberFormat numberFormat = new Dt.Xls.Chart.NumberFormat {
                    LinkToSource = this.NumberFormatLinked,
                    NumberFormatCode = this.NumberFormat
                };
                ChartCommonSimpleNodeHelper.WriteNummberFormatNode(writer, numberFormat);
            }
            writer.WriteLeafElementWithAttribute("majorTickMark", null, "c", "val", this.GetTickMarkStringForExcel(this.MajorTickMark));
            writer.WriteLeafElementWithAttribute("minorTickMark", null, "c", "val", this.GetTickMarkStringForExcel(this.MinorTickMark));
            writer.WriteLeafElementWithAttribute("tickLblPos", null, "c", "val", this.GetTickLabelPositionStringForExcel(this.TickLabelPosition));
            if (this.ShapeFormat != null)
            {
                (this.ShapeFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.TextFormat != null)
            {
                (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.CrossAx != 0)
            {
                writer.WriteLeafElementWithAttribute("crossAx", null, "c", "val", ((int) this.CrossAx).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!double.IsNaN(this.CrosssAt))
            {
                writer.WriteLeafElementWithAttribute("crossesAt", null, "c", "val", ((double) this.CrosssAt).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            else
            {
                string str2 = "autoZero";
                switch (this.Crosses)
                {
                    case AxisCrosses.AutoZero:
                        str2 = "autoZero";
                        break;

                    case AxisCrosses.Max:
                        str2 = "max";
                        break;

                    case AxisCrosses.Min:
                        str2 = "min";
                        break;
                }
                writer.WriteLeafElementWithAttribute("crosses", null, "c", "val", str2);
            }
            writer.WriteLeafElementWithAttribute("auto", null, "c", "val", this.IsAutomaticCategoryAxis ? "1" : "0");
            if (this.LabelOffset != 0)
            {
                writer.WriteLeafElementWithAttribute("lblOffset", null, "c", "val", ((int) this.LabelOffset).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteLeafElementWithAttribute("baseTimeUnit", null, "c", "val", this.BaseTimeUnit.ToString().ToCamelCase());
            if (!this._isAutoMajorUnit)
            {
                writer.WriteLeafElementWithAttribute("majorUnit", null, "c", "val", ((int) this._majorUnit).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!this._isAutoMinorUnit)
            {
                writer.WriteLeafElementWithAttribute("minorUnit", null, "c", "val", ((int) this._minorUnit).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if ((this.MajorTimeUnit != AxisTimeUnit.Days) && !this._isAutoMajorTimeUnit)
            {
                writer.WriteLeafElementWithAttribute("majorTimeUnit", null, "c", "val", this.MajorTimeUnit.ToString().ToCamelCase());
            }
            if ((this.MinorTimeUnit != AxisTimeUnit.Days) && !this._isAutoMinorTimeUnit)
            {
                writer.WriteLeafElementWithAttribute("minorTimeUnit", null, "c", "val", this.MinorTimeUnit.ToString().ToCamelCase());
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the position for an axis.
        /// </summary>
        public Dt.Xls.Chart.AxisPosition AxisPosition { get; set; }

        /// <summary>
        /// Specifies a title for the Axis
        /// </summary>
        public IExcelChartTitle AxisTitle { get; set; }

        /// <summary>
        /// Specifies the base time unit
        /// </summary>
        public AxisTimeUnit BaseTimeUnit { get; set; }

        /// <summary>
        /// Specifies the ID of axis that this axis cross.
        /// </summary>
        public int CrossAx { get; set; }

        /// <summary>
        /// Specifies how this axis crosses the perpendicular axis.
        /// </summary>
        public AxisCrosses Crosses { get; set; }

        /// <summary>
        /// Specifies where on the axis the perpendicular axis crosses. The units are dependent on the type of axis.
        /// </summary>
        /// <remarks>
        /// When  the AxisType is Value, the value is a decimal number on the value axis.
        /// When the AxisType is Date, the date is defined as a integer number of days relative to the base data of the current date base.
        /// When the AxisType is Category, the value is an integer category number, starting with 1 as the first category.
        /// </remarks>
        public double CrosssAt { get; set; }

        /// <summary>
        /// Specifies that chart element specifies by its containing element shall be deleted from the chart.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Identify used to mark this axis.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is auto major time uint.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto major time uint; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoMajorTimeUint
        {
            get { return  this._isAutoMajorTimeUnit; }
            set { this._isAutoMajorTimeUnit = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is auto major unit.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto major unit; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoMajorUnit
        {
            get { return  this._isAutoMajorUnit; }
            set { this._isAutoMajorUnit = value; }
        }

        /// <summary>
        /// Specifies this axis is a date or text axis based on the data that is used for the axis lables. not a specific choice.
        /// </summary>
        public bool IsAutomaticCategoryAxis { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is auto minor time unit.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto minor time unit; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoMinorTimeUnit
        {
            get { return  this._isAutoMinorTimeUnit; }
            set { this._isAutoMinorTimeUnit = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is auto minor unit.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is auto minor unit; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoMinorUnit
        {
            get { return  this._isAutoMinorUnit; }
            set { this._isAutoMinorUnit = value; }
        }

        /// <summary>
        /// Specifies the distance of lables from the axis.
        /// </summary>
        /// <remarks>
        /// The value should be between 0 and 1000.
        /// </remarks>
        public int LabelOffset { get; set; }

        /// <summary>
        /// Specifies major gridlines.
        /// </summary>
        public IExcelGridLines MajorGridlines { get; set; }

        /// <summary>
        /// Specifies the major tick marks for the axis.
        /// </summary>
        public TickMark MajorTickMark { get; set; }

        /// <summary>
        /// Specifies the major time unit
        /// </summary>
        public AxisTimeUnit MajorTimeUnit
        {
            get
            {
                if (!this._isAutoMajorTimeUnit)
                {
                    return this._majorTimeUnit;
                }
                return this.BaseTimeUnit;
            }
            set
            {
                this._isAutoMajorTimeUnit = false;
                this._majorTimeUnit = value;
            }
        }

        /// <summary>
        /// Specifies the distance between major ticks.
        /// </summary>
        public int MajorUnit
        {
            get { return  this._majorUnit; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Value must be greater than or equal to 1");
                }
                this._isAutoMajorUnit = false;
                this._majorUnit = value;
            }
        }

        /// <summary>
        /// Specifies minor gridlines.
        /// </summary>
        public IExcelGridLines MinorGridlines { get; set; }

        /// <summary>
        /// Specifies the minor tick marks for the axis.
        /// </summary>
        public TickMark MinorTickMark { get; set; }

        /// <summary>
        /// Specifies the minor time unit
        /// </summary>
        public AxisTimeUnit MinorTimeUnit
        {
            get
            {
                if (!this._isAutoMinorTimeUnit)
                {
                    return this._minorTimeUnit;
                }
                return this.BaseTimeUnit;
            }
            set
            {
                this._isAutoMinorTimeUnit = false;
                this._minorTimeUnit = value;
            }
        }

        /// <summary>
        /// Specifies the distance betwwen minor ticks.
        /// </summary>
        public int MinorUnit
        {
            get { return  this._minorUnit; }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("Value must be greater than or equal to 1");
                }
                this._isAutoMinorUnit = false;
                this._minorUnit = value;
            }
        }

        /// <summary>
        /// Specifies the number format for the data label.
        /// </summary>
        public string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whethere the data label use the same number formats as the cells that contain the data for the associated data point.
        /// </summary>
        public bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Specifies additional axis scale settings.
        /// </summary>
        public IScaling Scaling { get; set; }

        /// <summary>
        /// Specify the axis format
        /// </summary>
        public IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }

        /// <summary>
        /// Specifies the possible positions for the tick labels.
        /// </summary>
        public Dt.Xls.Chart.TickLabelPosition TickLabelPosition { get; set; }

        /// <summary>
        /// Specifies the axis type.
        /// </summary>
        public ExcelChartAxisType Type { get; internal set; }
    }
}

