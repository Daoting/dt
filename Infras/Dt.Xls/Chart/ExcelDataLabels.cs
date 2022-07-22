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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the settings for the data labels for an entire series or the entire chart.
    /// </summary>
    public class ExcelDataLabels : IExcelDataLabels
    {
        private List<IExcelDataLabel> _dataLableList;
        private int _orientation;
        private DataLabelPosition _position;
        private bool _showBubbleSize;
        private bool _showCategoryName;
        private bool _showLeaderLines;
        private bool _showLegendKey;
        private bool _showPercentage;
        private bool _showSeriesName;
        private bool _showValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelDataLabels" /> class.
        /// </summary>
        public ExcelDataLabels()
        {
            this.Delete = true;
            this.NumberFormatLinked = true;
            this.NumberFormat = "General";
            this.Separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            this.Position = DataLabelPosition.BestFit;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "dLbl")
                {
                    ExcelDataLabel label = new ExcelDataLabel();
                    label.ReadXml(element, mFolder, xFile);
                    this.DataLabelList.Add(label);
                }
                else if (element.Name.LocalName == "delete")
                {
                    this.Delete = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "numFmt")
                {
                    Dt.Xls.Chart.NumberFormat format = ChartCommonSimpleNodeHelper.ReadNumberFormatNode(element);
                    this.NumberFormat = format.NumberFormatCode;
                    this.NumberFormatLinked = format.LinkToSource;
                }
                else if (element.Name.LocalName == "dLblPos")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "bestFit"))
                    {
                        case "b":
                            this.Position = DataLabelPosition.Bottom;
                            break;

                        case "bestFit":
                            this.Position = DataLabelPosition.BestFit;
                            break;

                        case "ctr":
                            this.Position = DataLabelPosition.Center;
                            break;

                        case "inBase":
                            this.Position = DataLabelPosition.InsideBase;
                            break;

                        case "inEnd":
                            this.Position = DataLabelPosition.InsideEnd;
                            break;

                        case "l":
                            this.Position = DataLabelPosition.Left;
                            break;

                        case "outEnd":
                            this.Position = DataLabelPosition.OutsideEnd;
                            break;

                        case "r":
                            this.Position = DataLabelPosition.Right;
                            break;

                        case "t":
                            this.Position = DataLabelPosition.Top;
                            break;
                    }
                }
                else if (element.Name.LocalName == "separator")
                {
                    this.Separator = element.Value;
                }
                else if (element.Name.LocalName == "showLegendKey")
                {
                    this.ShowLegendKey = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showVal")
                {
                    this.ShowValue = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showCatName")
                {
                    this.ShowCategoryName = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showSerName")
                {
                    this.ShowSeriesName = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showPercent")
                {
                    this.ShowPercentage = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showBubbleSize")
                {
                    this.ShowBubbleSize = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showLeaderLines")
                {
                    this.ShowLeaderLines = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "leaderLines")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "spPr")
                        {
                            ExcelChartFormat format2 = new ExcelChartFormat();
                            format2.ReadXml(element2, mFolder, xFile);
                            this.LeaderLineFormat = format2;
                        }
                    }
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format3 = new ExcelChartFormat();
                    format3.ReadXml(element, mFolder, xFile);
                    this.ShapeFormat = format3;
                }
                else if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format4 = new ExcelTextFormat();
                    format4.ReadXml(element, mFolder, xFile);
                    this.TextFormat = format4;
                }
            }
            if (!this.IsShowLeaderLinesSet)
            {
                this._showLeaderLines = false;
            }
            if (!this.IsShowLegendKeySet)
            {
                this._showLegendKey = false;
            }
            if (!this.IsShowBubbleSizeSet)
            {
                this._showBubbleSize = false;
            }
            if (!this.IsShowCategoryNameSet)
            {
                this._showCategoryName = false;
            }
            if (!this.IsShowPercentageSet)
            {
                this._showPercentage = false;
            }
            if (!this.IsShowSeriesNameSet)
            {
                this._showSeriesName = false;
            }
            if (!this.IsShowValueSet)
            {
                this._showValue = false;
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "dLbls", null);
            if ((this.DataLabelList != null) && (this.DataLabelList.Count > 0))
            {
                foreach (ExcelDataLabel label in this.DataLabelList)
                {
                    label.WriteXml(writer, mFolder, chartFile);
                }
            }
            if (this.ShapeFormat != null)
            {
                (this.ShapeFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.TextFormat != null)
            {
                (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (!string.IsNullOrWhiteSpace(this.NumberFormat) || !this.NumberFormatLinked)
            {
                Dt.Xls.Chart.NumberFormat numberFormat = new Dt.Xls.Chart.NumberFormat {
                    LinkToSource = this.NumberFormatLinked,
                    NumberFormatCode = this.NumberFormat
                };
                ChartCommonSimpleNodeHelper.WriteNummberFormatNode(writer, numberFormat);
            }
            string str = "bestFit";
            switch (this.Position)
            {
                case DataLabelPosition.Bottom:
                    str = "b";
                    break;

                case DataLabelPosition.BestFit:
                    str = "bestFit";
                    break;

                case DataLabelPosition.Center:
                    str = "ctr";
                    break;

                case DataLabelPosition.InsideBase:
                    str = "inBase";
                    break;

                case DataLabelPosition.InsideEnd:
                    str = "inEnd";
                    break;

                case DataLabelPosition.Left:
                    str = "l";
                    break;

                case DataLabelPosition.OutsideEnd:
                    str = "outEnd";
                    break;

                case DataLabelPosition.Right:
                    str = "r";
                    break;

                case DataLabelPosition.Top:
                    str = "t";
                    break;
            }
            if (this.Position != DataLabelPosition.BestFit)
            {
                writer.WriteLeafElementWithAttribute("dLblPos", null, "c", "val", str);
            }
            if (!this.Delete)
            {
                writer.WriteLeafElementWithAttribute("delete", null, "c", "val", "0");
            }
            if (this.Separator != CultureInfo.CurrentCulture.TextInfo.ListSeparator)
            {
                writer.WriteLeafElementWithAttribute("separator", null, "c", "val", this.Separator);
            }
            writer.WriteLeafElementWithAttribute("showLegendKey", null, "c", "val", this.ShowLegendKey ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showVal", null, "c", "val", this.ShowValue ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showCatName", null, "c", "val", this.ShowCategoryName ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showSerName", null, "c", "val", this.ShowSeriesName ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showPercent", null, "c", "val", this.ShowPercentage ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showBubbleSize", null, "c", "val", this.ShowBubbleSize ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showLeaderLines", null, "c", "val", this.ShowLeaderLines ? "1" : "0");
            if (this.LeaderLineFormat != null)
            {
                using (writer.WriteElement("leaderLines", null, "c"))
                {
                    (this.LeaderLineFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// The data label items in this IDataLabel collection
        /// </summary>
        public List<IExcelDataLabel> DataLabelList
        {
            get
            {
                if (this._dataLableList == null)
                {
                    this._dataLableList = new List<IExcelDataLabel>();
                }
                return this._dataLableList;
            }
        }

        /// <summary>
        /// Specifies that the chart element specifies by its containing element shall be deleted from the chart.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// specifies whether value of Position is set by user.
        /// </summary>
        internal bool IsOrientationSet { get; set; }

        /// <summary>
        /// specifies whether value of Position is set by user.
        /// </summary>
        internal bool IsPositionSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowBubbleSize is set by user.
        /// </summary>
        internal bool IsShowBubbleSizeSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowCategoryName is set by user.
        /// </summary>
        internal bool IsShowCategoryNameSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowLeaderLines is set by user.
        /// </summary>
        internal bool IsShowLeaderLinesSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowLegendKey is set by user.
        /// </summary>
        internal bool IsShowLegendKeySet { get; set; }

        /// <summary>
        /// specifies whether value of ShowPercentage is set by user.
        /// </summary>
        internal bool IsShowPercentageSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowSeriesName is set by user.
        /// </summary>
        internal bool IsShowSeriesNameSet { get; set; }

        /// <summary>
        /// specifies whether value of ShowValue is set by user.
        /// </summary>
        internal bool IsShowValueSet { get; set; }

        /// <summary>
        /// Specifies the leader lines format.
        /// </summary>
        public IExcelChartFormat LeaderLineFormat { get; set; }

        /// <summary>
        /// Specifies the number format for the data label.
        /// </summary>
        public string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whethere the data label use the same number formats as the cells that contain the data for the associated data point.
        /// </summary>
        public bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Specifies the text orientation of the data lable.
        /// </summary>
        /// <remarks>
        /// The value should be between -90 and 90.
        /// </remarks>
        public int Orientation
        {
            get { return  this._orientation; }
            set
            {
                this._orientation = value;
                this.IsOrientationSet = true;
            }
        }

        /// <summary>
        /// Specifies the position of the data label.
        /// </summary>
        public DataLabelPosition Position
        {
            get { return  this._position; }
            set
            {
                this._position = value;
                this.IsPositionSet = true;
            }
        }

        /// <summary>
        /// Specifies text that shall be used to separate the parts of the data label, the default is comma.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Specifies the formatting options for the data label.
        /// </summary>
        public IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies whether the data label show the bubble size.
        /// </summary>
        public bool ShowBubbleSize
        {
            get { return  this._showBubbleSize; }
            set
            {
                this._showBubbleSize = value;
                this.IsShowBubbleSizeSet = true;
            }
        }

        /// <summary>
        /// Specifies whether the data label show the category name.
        /// </summary>
        public bool ShowCategoryName
        {
            get { return  this._showCategoryName; }
            set
            {
                this._showCategoryName = value;
                this.IsShowCategoryNameSet = true;
            }
        }

        /// <summary>
        /// Specifies leader lines shall be shown for data label
        /// </summary>
        public bool ShowLeaderLines
        {
            get { return  this._showLeaderLines; }
            set
            {
                this._showLeaderLines = value;
                this.IsShowLeaderLinesSet = true;
            }
        }

        /// <summary>
        /// Specifies whether the data label show the legend key.
        /// </summary>
        public bool ShowLegendKey
        {
            get { return  this._showLegendKey; }
            set
            {
                this._showLegendKey = value;
                this.IsShowLegendKeySet = true;
            }
        }

        /// <summary>
        /// Specifies whether the data label show the percentage.
        /// </summary>
        public bool ShowPercentage
        {
            get { return  this._showPercentage; }
            set
            {
                this._showPercentage = value;
                this.IsShowPercentageSet = true;
            }
        }

        /// <summary>
        /// Specifies whether the data label show the series name.
        /// </summary>
        public bool ShowSeriesName
        {
            get { return  this._showSeriesName; }
            set
            {
                this._showSeriesName = value;
                this.IsShowSeriesNameSet = true;
            }
        }

        /// <summary>
        /// Specifies whether the data label show the value.
        /// </summary>
        public bool ShowValue
        {
            get { return  this._showValue; }
            set
            {
                this._showValue = value;
                this.IsShowValueSet = true;
            }
        }

        /// <summary>
        /// Specifies the text of the data label.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Specifies the text format for the data label.
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }
    }
}

