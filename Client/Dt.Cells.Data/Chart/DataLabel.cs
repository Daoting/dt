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
using System.Globalization;
using System.Text;
using System.Xml;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the text label element that displayed for a data point value.
    /// </summary>
    public class DataLabel : SpreadChartTextElement, IDataPoint
    {
        Dt.Cells.Data.DataLabelSettings _dataLabelSettings;
        SpreadDataSeries _dataSeries;
        IFormatter _formatter;
        bool _isFormatterSet;
        int _pointIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DataLabel" /> class.
        /// </summary>
        public DataLabel()
        {
        }

        internal DataLabel(SpreadDataSeries dataSeries, int pointIndex) : base(dataSeries.Chart)
        {
            this.Init();
            this._dataSeries = dataSeries;
            this._pointIndex = pointIndex;
        }

        void DataLabelSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.NotifyChartChanged(e.PropertyName);
        }

        internal string GetFormatedText(double? value)
        {
            if (!value.HasValue || double.IsNaN(value.Value))
            {
                return string.Empty;
            }
            if (this.ActualFormatter != null)
            {
                return this.ActualFormatter.Format(value);
            }
            return Convert.ToString(value, (IFormatProvider) CultureInfo.CurrentCulture);
        }

        internal string GetPercentageText(double? value)
        {
            if (!value.HasValue)
            {
                return string.Empty;
            }
            IFormatter actualFormatter = this.ActualFormatter;
            if ((actualFormatter == null) || ((actualFormatter != null) && (actualFormatter.FormatString == "General")))
            {
                actualFormatter = new GeneralFormatter("0%");
            }
            return actualFormatter.Format(value);
        }

        void Init()
        {
            this._pointIndex = -1;
            this._dataSeries = null;
        }

        bool IsPieChart()
        {
            return ((this.DataSeries != null) && this.DataSeries.ChartType.ToString().ToLower().Contains("pie"));
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "PointIndex")
                {
                    if (str != "DataLabelSettings")
                    {
                        return;
                    }
                }
                else
                {
                    this._pointIndex = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                    return;
                }
                this._dataLabelSettings = (Dt.Cells.Data.DataLabelSettings) Serializer.DeserializeObj(typeof(Dt.Cells.Data.DataLabelSettings), reader);
                this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
            }
        }

        internal void SetParentSeriesInternal(SpreadDataSeries series)
        {
            this._dataSeries = series;
            this.SetChartInternal(series.Chart);
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            Serializer.SerializeObj((int) this._pointIndex, "PointIndex", writer);
            if (this._dataLabelSettings != null)
            {
                Serializer.SerializeObj(this._dataLabelSettings, "DataLabelSettings", writer);
            }
        }

        /// <summary>
        /// Gets the actual data label settings.
        /// </summary>
        /// <value>
        /// The actual data label settings.
        /// </value>
        public Dt.Cells.Data.DataLabelSettings ActualDataLabelSettings
        {
            get
            {
                Dt.Cells.Data.DataLabelSettings settings = new Dt.Cells.Data.DataLabelSettings();
                Dt.Cells.Data.DataLabelSettings actualDataLabelSettings = this._dataSeries.ActualDataLabelSettings;
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowCategoryNameSet)
                {
                    settings.ShowCategoryName = this.DataLabelSettings.ShowCategoryName;
                }
                else if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsShowCategoryNameSet)
                {
                    settings.ShowCategoryName = actualDataLabelSettings.ShowCategoryName;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowValueSet)
                {
                    settings.ShowValue = this.DataLabelSettings.ShowValue;
                }
                else if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsShowValueSet)
                {
                    settings.ShowValue = actualDataLabelSettings.ShowValue;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowSeriesNameSet)
                {
                    settings.ShowSeriesName = this.DataLabelSettings.ShowSeriesName;
                }
                else if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsShowSeriesNameSet)
                {
                    settings.ShowSeriesName = actualDataLabelSettings.ShowSeriesName;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowBubbleSizeSet)
                {
                    settings.ShowBubbleSize = this.DataLabelSettings.ShowBubbleSize;
                }
                else if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsShowBubbleSizeSet)
                {
                    settings.ShowBubbleSize = actualDataLabelSettings.ShowBubbleSize;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsShowPercentSet)
                {
                    settings.ShowPercent = this.DataLabelSettings.ShowPercent;
                }
                else if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsShowPercentSet)
                {
                    settings.ShowPercent = actualDataLabelSettings.ShowPercent;
                }
                if ((this.DataLabelSettings != null) && this.DataLabelSettings.IsSeparatorSet)
                {
                    settings.Separator = this.DataLabelSettings.Separator;
                    return settings;
                }
                if ((actualDataLabelSettings != null) && actualDataLabelSettings.IsSeparatorSet)
                {
                    settings.Separator = actualDataLabelSettings.Separator;
                }
                return settings;
            }
        }

        /// <summary>
        /// Gets the actual brush that specifies how the element's interior is painted.
        /// </summary>
        public override Brush ActualFill
        {
            get
            {
                if ((base._styleInfo != null) && (base._styleInfo.IsFillSet || base._styleInfo.IsFillThemeColorSet))
                {
                    return base.ActualFill;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFill;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual font family.
        /// </summary>
        public override FontFamily ActualFontFamily
        {
            get
            {
                if ((base._styleInfo != null) && (base._styleInfo.IsFontFamilySet || base._styleInfo.IsFontThemeSet))
                {
                    return base.ActualFontFamily;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFontFamily;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual size of the font.
        /// </summary>
        /// <value>
        /// The actual size of the font.
        /// </value>
        public override double ActualFontSize
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontSizeSet)
                {
                    return base._styleInfo.FontSize;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFontSize;
                }
                return -1.0;
            }
        }

        /// <summary>
        /// Gets the actual font stretch.
        /// </summary>
        public override FontStretch ActualFontStretch
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStretchSet)
                {
                    return base._styleInfo.FontStretch;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFontStretch;
                }
                return FontStretch.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font style.
        /// </summary>
        public override FontStyle ActualFontStyle
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStyleSet)
                {
                    return base._styleInfo.FontStyle;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFontStyle;
                }
                return FontStyle.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font weight.
        /// </summary>
        public override FontWeight ActualFontWeight
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontWeightSet)
                {
                    return base._styleInfo.FontWeight;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualFontWeight;
                }
                return FontWeights.Normal;
            }
        }

        /// <summary>
        /// Gets the actual foreground.
        /// </summary>
        public override Brush ActualForeground
        {
            get
            {
                IColorFormatter actualFormatter = this.ActualFormatter as IColorFormatter;
                if ((actualFormatter != null) && actualFormatter.HasFormatedColor)
                {
                    Color? nullable;
                    actualFormatter.Format(this.Value, out nullable);
                    if (nullable.HasValue && (this.Chart != null))
                    {
                        return new SolidColorBrush(nullable.Value);
                    }
                }
                if ((base._styleInfo != null) && (base._styleInfo.IsForegroundSet || base._styleInfo.IsForegroundThemeColorSet))
                {
                    return base.ActualForeground;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualForeground;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual formatter.
        /// </summary>
        /// <value>
        /// The actual formatter.
        /// </value>
        public IFormatter ActualFormatter
        {
            get
            {
                if (this._isFormatterSet)
                {
                    return this.Formatter;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.Formatter;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual brush that specifies how the element's outline is painted.
        /// </summary>
        public override Brush ActualStroke
        {
            get
            {
                if ((base._styleInfo != null) && (base._styleInfo.IsStrokeSet || base._styleInfo.IsStrokeThemeColorSet))
                {
                    return base.ActualStroke;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.ActualStroke;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual type of the outline.
        /// </summary>
        /// <value>
        /// The actual type of the outline.
        /// </value>
        public StrokeDashType ActualStrokeDashType
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsStrokeDashTypeSet)
                {
                    return base.StrokeDashType;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.StrokeDashType;
                }
                return StrokeDashType.None;
            }
        }

        /// <summary>
        /// Gets the actual width of the outline.
        /// </summary>
        /// <value>
        /// The actual width of the outline.
        /// </value>
        public double ActualStrokeThickness
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsStrokeThicknessSet)
                {
                    return base.StrokeThickness;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.DataLabelStyle.StrokeThickness;
                }
                return 1.0;
            }
        }

        internal SpreadChart Chart
        {
            get { return  (base.ChartBase as SpreadChart); }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.DataLabel; }
        }

        /// <summary>
        /// Gets the data label settings.
        /// </summary>
        /// <value>
        /// The data label settings.
        /// </value>
        public Dt.Cells.Data.DataLabelSettings DataLabelSettings
        {
            get
            {
                if (this._dataLabelSettings == null)
                {
                    this._dataLabelSettings = new Dt.Cells.Data.DataLabelSettings();
                    this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                }
                return this._dataLabelSettings;
            }
            set
            {
                if (value != this.DataLabelSettings)
                {
                    if (this._dataLabelSettings != null)
                    {
                        this._dataLabelSettings.PropertyChanged -= new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                    }
                    this._dataLabelSettings = value;
                    if (this._dataLabelSettings != null)
                    {
                        this._dataLabelSettings.PropertyChanged += new PropertyChangedEventHandler(this.DataLabelSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the data series.
        /// </summary>
        /// <value>
        /// The data series.
        /// </value>
        public SpreadDataSeries DataSeries
        {
            get { return  this._dataSeries; }
            internal set { this._dataSeries = value; }
        }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        public IFormatter Formatter
        {
            get { return  this._formatter; }
            set
            {
                if (value != this._formatter)
                {
                    this._isFormatterSet = true;
                    this._formatter = value;
                    base.NotifyChartChanged("Formatter");
                }
            }
        }

        /// <summary>
        /// Gets the percentage.
        /// </summary>
        /// <value>
        /// The percentage.
        /// </value>
        public double? Percentage
        {
            get
            {
                double num = 0.0;
                foreach (double num2 in this.DataSeries.Values)
                {
                    num += num2;
                }
                double? nullable = this.Value;
                double num3 = num;
                if (!nullable.HasValue)
                {
                    return null;
                }
                return new double?(((double) nullable.GetValueOrDefault()) / num3);
            }
        }

        /// <summary>
        /// Gets the index of the point.
        /// </summary>
        /// <value>
        /// The index of the point.
        /// </value>
        public int PointIndex
        {
            get { return  this._pointIndex; }
            internal set { this._pointIndex = value; }
        }

        /// <summary>
        /// Gets the index of the series.
        /// </summary>
        /// <value>
        /// The index of the series.
        /// </value>
        public int SeriesIndex
        {
            get
            {
                if ((this.Chart != null) && (this.DataSeries != null))
                {
                    return this.Chart.DataSeries.IndexOf(this.DataSeries);
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public override string Text
        {
            get
            {
                if (!this.Value.HasValue || double.IsNaN(this.Value.Value))
                {
                    return string.Empty;
                }
                string name = string.Empty;
                string str2 = string.Empty;
                string formatedText = string.Empty;
                string percentageText = string.Empty;
                Dt.Cells.Data.DataLabelSettings actualDataLabelSettings = this.ActualDataLabelSettings;
                StringBuilder builder = new StringBuilder();
                if (actualDataLabelSettings != null)
                {
                    if (actualDataLabelSettings.ShowSeriesName)
                    {
                        name = this.DataSeries.Name;
                        builder.Append(name);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                    if (actualDataLabelSettings.ShowCategoryName)
                    {
                        AxisItemsCollection items = null;
                        IFormatter labelFormatter = null;
                        if (this.Chart != null)
                        {
                            if (SpreadChartUtility.IsBarChart(this.Chart.ChartType))
                            {
                                if (this.Chart.AxisY != null)
                                {
                                    items = this.Chart.AxisY.Items;
                                    labelFormatter = this.Chart.AxisY.LabelFormatter;
                                }
                            }
                            else if (this.Chart.AxisX != null)
                            {
                                items = this.Chart.AxisX.Items;
                                labelFormatter = this.Chart.AxisX.LabelFormatter;
                            }
                        }
                        if (((items != null) && (this.PointIndex < items.Count)) && (items[this.PointIndex] != null))
                        {
                            if (labelFormatter != null)
                            {
                                str2 = labelFormatter.Format(items[this.PointIndex]);
                            }
                            else
                            {
                                str2 = items[this.PointIndex].ToString();
                            }
                        }
                        builder.Append(str2);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                    if (actualDataLabelSettings.ShowValue)
                    {
                        formatedText = this.GetFormatedText(this.Value);
                        builder.Append(formatedText);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                    if (this.IsPieChart() && actualDataLabelSettings.ShowPercent)
                    {
                        percentageText = this.GetPercentageText(this.Percentage);
                        builder.Append(percentageText);
                        builder.Append(actualDataLabelSettings.Separator);
                    }
                }
                return builder.ToString().Trim(actualDataLabelSettings.Separator.ToCharArray());
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public double? Value
        {
            get
            {
                if (((this._dataSeries != null) && (this._pointIndex >= 0)) && (this._pointIndex < this._dataSeries.Values.Count))
                {
                    return new double?(this._dataSeries.Values[this._pointIndex]);
                }
                return null;
            }
        }
    }
}

