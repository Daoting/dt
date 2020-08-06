#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the single data point that may have several data values.
    /// </summary>
    public class DataPoint : SpreadChartElement, IDataPoint
    {
        SpreadDataSeries _dataSeries;
        bool _invertIfNegative;
        bool _invertIfNegativeSet;
        bool _isBubble3D;
        Brush _negativeFill;
        bool _negativeFillSet;
        string _negativeFillThemeColor;
        bool _negativeFillThemeColorSet;
        int _pointIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DataPoint" /> class.
        /// </summary>
        public DataPoint()
        {
            this._isBubble3D = true;
        }

        internal DataPoint(SpreadDataSeries dataSeries, int pointIndex) : base(dataSeries.Chart)
        {
            this._isBubble3D = true;
            this.Init();
            this._dataSeries = dataSeries;
            this._pointIndex = pointIndex;
        }

        internal override void BeforeReadXml()
        {
            base.BeforeReadXml();
            this.ResetInvertIfNegative();
            this.ResetNegativeFill();
            this.ResetNegativeFillThemeColor();
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.NotifyChartChanged(e.PropertyName);
        }

        void Init()
        {
            this._pointIndex = -1;
            this._dataSeries = null;
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "PointIndex")
                {
                    if (str != "InvertIfNegative")
                    {
                        if (str == "NegativeFill")
                        {
                            this._negativeFillSet = true;
                            this._negativeFill = (Brush) Serializer.DeserializeObj(typeof(Brush), reader);
                            return;
                        }
                        if (str == "NegativeFillColor")
                        {
                            this._negativeFillThemeColorSet = true;
                            this._negativeFillThemeColor = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                        }
                        return;
                    }
                }
                else
                {
                    this._pointIndex = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                    return;
                }
                this._invertIfNegativeSet = true;
                this._invertIfNegative = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Resets the InvertIfNegative property.
        /// </summary>
        public void ResetInvertIfNegative()
        {
            this._invertIfNegativeSet = false;
            this._invertIfNegative = false;
        }

        /// <summary>
        /// Resets the NegativeFill property.
        /// </summary>
        public void ResetNegativeFill()
        {
            this._negativeFill = null;
            this._negativeFillSet = false;
        }

        /// <summary>
        /// Resets the NegativeFillThemeColor property.
        /// </summary>
        public void ResetNegativeFillThemeColor()
        {
            this._negativeFillThemeColor = null;
            this._negativeFillThemeColorSet = false;
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
            if (this._invertIfNegativeSet)
            {
                Serializer.SerializeObj((bool) this._invertIfNegative, "InvertIfNegative", writer);
            }
            if (this._negativeFillSet)
            {
                if (this._negativeFill == null)
                {
                    writer.WriteStartElement("NegativeFill");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._negativeFill, "NegativeFill", writer);
                }
            }
            if (this._negativeFillThemeColorSet)
            {
                if (this._negativeFillThemeColor == null)
                {
                    writer.WriteStartElement("NegativeFillColor");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._negativeFillThemeColor, "NegativeFillColor", writer);
                }
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
                if ((this.DataSeries != null) && this.DataSeries.IsAutomaticFill)
                {
                    return this.AutomaticFill;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.ActualFill;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the current formatter.
        /// </summary>
        /// <value>An <see cref="T:Dt.Cells.Data.IFormatter" /> object that specifies the actual formatter for the data point.</value>
        public IFormatter ActualFormatter
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFormatterSet)
                {
                    return base._styleInfo.Formatter;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.Formatter;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the actual value that indicates whether to invert if the value is negative.
        /// </summary>
        /// <value>
        /// <c>true</c> if inverted when value is negative; otherwise, <c>false</c>.
        /// </value>
        public bool ActualInvertIfNegative
        {
            get
            {
                if (this._invertIfNegativeSet)
                {
                    return this._invertIfNegative;
                }
                return ((this.DataSeries != null) && this.DataSeries.InvertIfNegative);
            }
        }

        /// <summary>
        /// Gets the actual fill brush for negative values.
        /// </summary>
        /// <value>
        /// The actual fill brush for negative values.
        /// </value>
        public Brush ActualNegativeFill
        {
            get
            {
                if (this._negativeFillSet)
                {
                    return this._negativeFill;
                }
                if (this._negativeFillThemeColorSet && (base.ThemeContext != null))
                {
                    Windows.UI.Color themeColor = base.ThemeContext.GetThemeColor(this._negativeFillThemeColor);
                    return new SolidColorBrush(themeColor);
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.ActualNegativeFill;
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
                if ((this.DataSeries != null) && this.DataSeries.IsAutomaticStroke)
                {
                    return this.AutomaticStroke;
                }
                if (this.DataSeries != null)
                {
                    return this.DataSeries.ActualStroke;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual value that indicates the pattern of dashes and gaps that is used for the data point outline.
        /// </summary>
        /// <value>
        /// The actual value that indicates the pattern of dashes and gaps that is used for the data point outline.
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
                    return this.DataSeries.StrokeDashType;
                }
                return StrokeDashType.None;
            }
        }

        /// <summary>
        /// Gets or sets the actual width of the element's outline.
        /// </summary>
        /// <value>
        /// The actual width of the element's outline.
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
                    return this.DataSeries.StrokeThickness;
                }
                return 1.0;
            }
        }

        /// <summary>
        /// Gets the fill brush that is created automatically.
        /// </summary>
        public override Brush AutomaticFill
        {
            get
            {
                if ((this.DataSeries != null) && (((this.DataSeries.ChartType == SpreadChartType.Pie) || (this.DataSeries.ChartType == SpreadChartType.PieExploded)) || ((this.DataSeries.ChartType == SpreadChartType.PieDoughnut) || (this.DataSeries.ChartType == SpreadChartType.PieExplodedDoughnut))))
                {
                    Brush automaticColor = this.DataSeries.GetAutomaticColor((double) this.PointIndex, (double) this.DataSeries.Values.Count);
                    if (automaticColor != null)
                    {
                        return automaticColor;
                    }
                }
                return this.DataSeries.AutomaticFill;
            }
        }

        /// <summary>
        /// Gets the brush stroke that is created automatically.
        /// </summary>
        public override Brush AutomaticStroke
        {
            get
            {
                if ((this.Chart != null) && (((this.DataSeries.ChartType == SpreadChartType.Pie) || (this.DataSeries.ChartType == SpreadChartType.PieExploded)) || ((this.DataSeries.ChartType == SpreadChartType.PieDoughnut) || (this.DataSeries.ChartType == SpreadChartType.PieExplodedDoughnut))))
                {
                    Brush automaticColor = this.DataSeries.GetAutomaticColor((double) this.PointIndex, (double) this.DataSeries.Values.Count);
                    if (automaticColor != null)
                    {
                        return automaticColor;
                    }
                }
                return this.DataSeries.AutomaticStroke;
            }
        }

        internal SpreadChart Chart
        {
            get { return  (base.ChartBase as SpreadChart); }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.DataPoint; }
        }

        internal Dt.Cells.Data.DataMarker DataMarker { get; set; }

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

        internal int? Explosion { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to invert if the value is negative.
        /// </summary>
        /// <value>
        /// <c>true</c> if inverted when value is negative; otherwise, <c>false</c>.
        /// </value>
        public bool InvertIfNegative
        {
            get { return  this._invertIfNegative; }
            set
            {
                this._invertIfNegativeSet = true;
                if (value != this.InvertIfNegative)
                {
                    this._invertIfNegative = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("InvertIfNegative");
                }
            }
        }

        internal bool IsBubble3D
        {
            get { return  this._isBubble3D; }
            set { this._isBubble3D = value; }
        }

        /// <summary>
        /// Gets or sets the fill brush for negative values.
        /// </summary>
        /// <value>
        /// The negative fill brush.
        /// </value>
        public Brush NegativeFill
        {
            get { return  this._negativeFill; }
            set
            {
                this._negativeFillThemeColor = null;
                this._negativeFillThemeColorSet = false;
                if (value != this.NegativeFill)
                {
                    this._negativeFillSet = true;
                    this._negativeFill = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("NegativeFill");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the fill theme for negative values.
        /// </summary>
        /// <value>
        /// The fill theme for negative values.
        /// </value>
        public string NegativeFillThemeColor
        {
            get { return  this._negativeFillThemeColor; }
            set
            {
                this._negativeFill = null;
                this._negativeFillSet = false;
                if (value != this.NegativeFillThemeColor)
                {
                    this._negativeFillThemeColorSet = true;
                    this._negativeFillThemeColor = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("NegativeFillThemeColor");
                }
            }
        }

        internal Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

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
                    this.Chart.DataSeries.IndexOf(this.DataSeries);
                }
                return -1;
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

