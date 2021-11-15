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
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base class for 2D chart symbols.
    /// </summary>
    public class DataMarker : SpreadChartElement, IDataPoint
    {
        SpreadDataSeries _dataSeries;
        Windows.Foundation.Size _markerSize;
        Dt.Cells.Data.MarkerType _markerType;
        int _pointIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.DataMarker" /> class.
        /// </summary>
        public DataMarker()
        {
            this._markerSize = Windows.Foundation.Size.Empty;
        }

        internal DataMarker(SpreadDataSeries dataSeries, int pointIndex) : base(dataSeries.Chart)
        {
            this._markerSize = Windows.Foundation.Size.Empty;
            this.Init();
            this._dataSeries = dataSeries;
            this._pointIndex = pointIndex;
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
                    if (str != "MarkerType")
                    {
                        return;
                    }
                }
                else
                {
                    this._pointIndex = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                    return;
                }
                this._markerType = (Dt.Cells.Data.MarkerType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.MarkerType), reader);
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
                    return this.DataSeries.DataMarkerStyle.ActualFill;
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
        /// Gets the actual size of the marker.
        /// </summary>
        /// <value>
        /// The actual size of the marker.
        /// </value>
        public Windows.Foundation.Size ActualMarkerSize
        {
            get
            {
                if (this._markerSize != Windows.Foundation.Size.Empty)
                {
                    return this._markerSize;
                }
                return this.DataSeries.MarkerSize;
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
                    return this.DataSeries.DataMarkerStyle.Stroke;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual value that indicates the pattern of dashes and gaps that is used to outline a data marker.
        /// </summary>
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
                    return this.DataSeries.DataMarkerStyle.StrokeDashType;
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
                    return this.DataSeries.DataMarkerStyle.StrokeThickness;
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
            get { return  Dt.Cells.Data.ChartArea.DataMarker; }
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
        }

        internal Dt.Cells.Data.MarkerType ExcelMarkerType { get; set; }

        /// <summary>
        /// Gets or sets the size of the marker.
        /// </summary>
        /// <value>
        /// The size of the marker.
        /// </value>
        public Windows.Foundation.Size MarkerSize
        {
            get { return  this._markerSize; }
            set
            {
                if (value != this.MarkerSize)
                {
                    this._markerSize = value;
                    base.NotifyChartChanged("MarkerSize");
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the marker.
        /// </summary>
        /// <value>
        /// The type of the marker.
        /// </value>
        public Dt.Cells.Data.MarkerType MarkerType
        {
            get
            {
                if (this.DataSeries != null)
                {
                    return this.DataSeries.MarkerType;
                }
                return Dt.Cells.Data.MarkerType.Automatic;
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

