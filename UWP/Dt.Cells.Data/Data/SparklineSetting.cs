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
using System.Threading;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a setting of an Excel-like sparkline.
    /// </summary>
    public class SparklineSetting : IThemeContextSupport
    {
        IThemeSupport _context;
        Windows.UI.Color axisColor = Colors.Black;
        string axisThemeColor = string.Empty;
        EmptyValueStyle displayEmptyValueStyle;
        bool displayHidden = true;
        bool displayXAxis;
        Windows.UI.Color firstMarkerColor = Colors.Brown;
        string firstMarkerThemeColor = string.Empty;
        double groupMaxValue;
        double groupMinValue;
        Windows.UI.Color highMarkerColor = Colors.Blue;
        string highMarkerThemeColor = string.Empty;
        Windows.UI.Color lastMarkerColor = Colors.Green;
        string lastMarkerThemeColor = string.Empty;
        double lineWeight = 1.0;
        Windows.UI.Color lowMarkerColor = Colors.Blue;
        string lowMarkerThemeColor = string.Empty;
        double manualMax = 3.0;
        double manualMin;
        Windows.UI.Color markersColor = Colors.Brown;
        string markersThemeColor = string.Empty;
        SparklineAxisMinMax maxAxisType;
        SparklineAxisMinMax minAxisType;
        Windows.UI.Color negativeColor = Colors.Red;
        string negativeThemeColor = string.Empty;
        bool rightToLeft;
        Windows.UI.Color seriesColor = Windows.UI.Color.FromArgb(0xff, 0x24, 0x40, 0x62);
        string seriesThemeColor = string.Empty;
        bool showFirst;
        bool showHigh;
        bool showLast;
        bool showLow;
        bool showMarkers;
        bool showNegative;

        /// <summary>
        /// Occurs when the property has changed.
        /// </summary>
        /// <remarks></remarks>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public SparklineSetting()
        {
            this.Init();
        }

        internal SparklineSetting Clone()
        {
            return new SparklineSetting { 
                displayEmptyValueStyle = this.displayEmptyValueStyle, displayHidden = this.displayHidden, displayXAxis = this.displayXAxis, firstMarkerColor = this.firstMarkerColor, groupMaxValue = this.groupMaxValue, groupMinValue = this.groupMinValue, highMarkerColor = this.highMarkerColor, lastMarkerColor = this.lastMarkerColor, lineWeight = this.lineWeight, lowMarkerColor = this.lowMarkerColor, manualMax = this.manualMax, manualMin = this.manualMin, markersColor = this.markersColor, maxAxisType = this.maxAxisType, minAxisType = this.minAxisType, negativeColor = this.negativeColor, 
                rightToLeft = this.rightToLeft, seriesColor = this.seriesColor, showFirst = this.showFirst, showHigh = this.showHigh, showLast = this.showLast, showLow = this.showLast, showMarkers = this.showMarkers, showNegative = this.showNegative, axisThemeColor = this.axisThemeColor, firstMarkerThemeColor = this.firstMarkerThemeColor, highMarkerThemeColor = this.highMarkerThemeColor, lastMarkerThemeColor = this.lastMarkerThemeColor, lowMarkerThemeColor = this.lowMarkerThemeColor, markersThemeColor = this.markersThemeColor, negativeThemeColor = this.negativeThemeColor, seriesThemeColor = this.seriesThemeColor
             };
        }

        /// <summary>
        /// Causes the event to occur.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void FireChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Gets the maximum value of the group.
        /// </summary>
        /// <returns>The maximum value of the group.</returns>
        public double GetGroupMaxValue()
        {
            return this.GroupMaxValue;
        }

        /// <summary>
        /// Gets the minimum value of the group.
        /// </summary>
        /// <returns>The minimum value of the group.</returns>
        public double GetGroupMinValue()
        {
            return this.GroupMinValue;
        }

        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>Returns the theme context.</returns>
        IThemeSupport IThemeContextSupport.GetContext()
        {
            return this._context;
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context object.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            if (this._context != context)
            {
                this._context = context;
            }
        }

        internal void Init()
        {
            this.axisColor = Colors.Black;
            this.firstMarkerColor = Windows.UI.Color.FromArgb(0xff, 0x95, 0xb3, 0xd7);
            this.highMarkerColor = Colors.Blue;
            this.lastMarkerColor = Windows.UI.Color.FromArgb(0xff, 0x95, 0xb3, 0xd7);
            this.lowMarkerColor = Colors.Blue;
            this.markersColor = Windows.UI.Color.FromArgb(0xff, 0x24, 0x40, 0x62);
            this.negativeColor = Colors.Brown;
            this.seriesColor = Windows.UI.Color.FromArgb(0xff, 0x24, 0x40, 0x62);
            this.axisThemeColor = string.Empty;
            this.firstMarkerThemeColor = string.Empty;
            this.highMarkerThemeColor = string.Empty;
            this.lastMarkerThemeColor = string.Empty;
            this.lowMarkerThemeColor = string.Empty;
            this.markersThemeColor = string.Empty;
            this.negativeThemeColor = string.Empty;
            this.seriesThemeColor = string.Empty;
            this.displayEmptyValueStyle = EmptyValueStyle.Gaps;
            this.rightToLeft = false;
            this.displayHidden = false;
            this.displayXAxis = false;
            this.showFirst = false;
            this.showHigh = false;
            this.showLast = false;
            this.showLow = false;
            this.showNegative = false;
            this.showMarkers = false;
            this.manualMax = 0.0;
            this.manualMin = 0.0;
            this.maxAxisType = SparklineAxisMinMax.Individual;
            this.minAxisType = SparklineAxisMinMax.Individual;
            this.groupMaxValue = 0.0;
            this.groupMinValue = 0.0;
            this.lineWeight = 1.0;
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance that contains the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        /// <summary> 
        /// Gets or sets the color of the axis. 
        /// </summary> 
        /// <value> 
        /// The color of the axis. 
        /// </value>
        [DefaultValue(typeof(Windows.UI.Color), "Black")]
        public Windows.UI.Color AxisColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.axisThemeColor))
                {
                    return this._context.GetThemeColor(this.axisThemeColor);
                }
                return this.axisColor;
            }
            set
            {
                if (this.axisColor != value)
                {
                    this.axisColor = value;
                    this.axisThemeColor = null;
                    this.FireChanged("AxisColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the axis.
        /// </summary>
        /// <value>The theme color of the axis.</value>
        [DefaultValue("")]
        public string AxisThemeColor
        {
            get { return  this.axisThemeColor; }
            set
            {
                if (this.axisThemeColor != value)
                {
                    this.axisThemeColor = value;
                    this.FireChanged("AxisThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets how to display the empty cells. 
        /// </summary> 
        /// <value> 
        /// The display empty cells as value. 
        /// </value>
        [DefaultValue(0)]
        public EmptyValueStyle DisplayEmptyCellsAs
        {
            get { return  this.displayEmptyValueStyle; }
            set
            {
                if (this.displayEmptyValueStyle != value)
                {
                    this.displayEmptyValueStyle = value;
                    this.FireChanged("DisplayEmptyValueStyle");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that indicates whether data in hidden cells is plotted for the sparklines in this sparkline group. 
        /// </summary> 
        /// <value> 
        /// <c>true</c> if the hidden data is plotted; otherwise, <c>false</c>. The default is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool DisplayHidden
        {
            get { return  this.displayHidden; }
            set
            {
                if (this.displayHidden != value)
                {
                    this.displayHidden = value;
                    this.FireChanged("DisplayHidden");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that indicates whether the horizontal axis is displayed for each sparkline in this sparkline group. 
        /// </summary> 
        /// <value> 
        /// <c>true</c> to display the horizontal axis; otherwise, <c>false</c>. 
        /// </value>
        [DefaultValue(false)]
        public bool DisplayXAxis
        {
            get { return  this.displayXAxis; }
            set
            {
                if (this.displayXAxis != value)
                {
                    this.displayXAxis = value;
                    this.FireChanged("DisplayXAxis");
                }
            }
        }

        /// <summary> 
        /// Gets or sets the color of the first data point for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "149, 179, 215")]
        public Windows.UI.Color FirstMarkerColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.firstMarkerThemeColor))
                {
                    return this._context.GetThemeColor(this.firstMarkerThemeColor);
                }
                return this.firstMarkerColor;
            }
            set
            {
                if (this.firstMarkerColor != value)
                {
                    this.firstMarkerColor = value;
                    this.firstMarkerThemeColor = null;
                    this.FireChanged("FirstMarkerColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the first marker.
        /// </summary>
        /// <value>The theme color of the first marker.</value>
        [DefaultValue("")]
        public string FirstMarkerThemeColor
        {
            get { return  this.firstMarkerThemeColor; }
            set
            {
                if (this.firstMarkerThemeColor != value)
                {
                    this.firstMarkerThemeColor = value;
                    this.FireChanged("FirstMarkerThemeColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the cached max value of a group. Use it internally.
        /// </summary>
        /// <value>
        /// The group max value.
        /// </value>
        internal double GroupMaxValue
        {
            get { return  this.groupMaxValue; }
            set
            {
                this.groupMaxValue = value;
                this.FireChanged("GroupMaxValue");
            }
        }

        /// <summary>
        /// Gets or sets the cached min value of a group. Use it internally.
        /// </summary>
        /// <value>
        /// The min group value.
        /// </value>
        internal double GroupMinValue
        {
            get { return  this.groupMinValue; }
            set
            {
                this.groupMinValue = value;
                this.FireChanged("GroupMinValue");
            }
        }

        /// <summary> 
        /// Gets or sets the color of the highest data point for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "BlueViolet")]
        public Windows.UI.Color HighMarkerColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.highMarkerThemeColor))
                {
                    return this._context.GetThemeColor(this.highMarkerThemeColor);
                }
                return this.highMarkerColor;
            }
            set
            {
                if (this.highMarkerColor != value)
                {
                    this.highMarkerColor = value;
                    this.highMarkerThemeColor = null;
                    this.FireChanged("HighMarkerColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the high marker.
        /// </summary>
        /// <value>The theme color of the high marker.</value>
        [DefaultValue("")]
        public string HighMarkerThemeColor
        {
            get { return  this.highMarkerThemeColor; }
            set
            {
                if (this.highMarkerThemeColor != value)
                {
                    this.highMarkerThemeColor = value;
                    this.FireChanged("HighMarkerThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets the color of the last data point for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "149, 179, 215")]
        public Windows.UI.Color LastMarkerColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.lastMarkerThemeColor))
                {
                    return this._context.GetThemeColor(this.lastMarkerThemeColor);
                }
                return this.lastMarkerColor;
            }
            set
            {
                if (this.lastMarkerColor != value)
                {
                    this.lastMarkerColor = value;
                    this.lastMarkerThemeColor = null;
                    this.FireChanged("LastMarkerColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the last marker.
        /// </summary>
        /// <value>The theme color of the last marker.</value>
        [DefaultValue("")]
        public string LastMarkerThemeColor
        {
            get { return  this.lastMarkerThemeColor; }
            set
            {
                if (this.lastMarkerThemeColor != value)
                {
                    this.lastMarkerThemeColor = value;
                    this.FireChanged("LastMarkerThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the line weight for each sparkline in the sparkline group, where the line weight is measured in points. The weight must be greater than or equal to zero, and must be less than or equal to 3 (LineSeries only supports line weight values in the range of 0.0-&gt;3.0). 
        /// </summary>
        [DefaultValue((double) 1.0)]
        public double LineWeight
        {
            get { return  this.lineWeight; }
            set
            {
                if (this.lineWeight != value)
                {
                    this.lineWeight = value;
                    this.FireChanged("LineWeight");
                }
            }
        }

        /// <summary> 
        /// Gets or sets the color of the lowest data point for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "Blue")]
        public Windows.UI.Color LowMarkerColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.lowMarkerThemeColor))
                {
                    return this._context.GetThemeColor(this.lowMarkerThemeColor);
                }
                return this.lowMarkerColor;
            }
            set
            {
                if (this.lowMarkerColor != value)
                {
                    this.lowMarkerColor = value;
                    this.lowMarkerThemeColor = null;
                    this.FireChanged("LowMarkerColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the low marker.
        /// </summary>
        /// <value>The theme color of the low marker.</value>
        [DefaultValue("")]
        public string LowMarkerThemeColor
        {
            get { return  this.lowMarkerThemeColor; }
            set
            {
                if (this.lowMarkerThemeColor != value)
                {
                    this.lowMarkerThemeColor = value;
                    this.FireChanged("LowMarkerThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the maximum for the vertical axis that is shared across all sparklines in this sparkline group. The axis is zero if maxAxisType does not equal custom. 
        /// </summary>
        [DefaultValue((double) 0.0)]
        public double ManualMax
        {
            get { return  this.manualMax; }
            set
            {
                if (this.manualMax != value)
                {
                    this.manualMax = value;
                    this.FireChanged("ManualMax");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the minimum for the vertical axis that is shared across all sparklines in this sparkline group. The axis is zero if minAxisType does not equal custom. 
        /// </summary>
        [DefaultValue((double) 0.0)]
        public double ManualMin
        {
            get { return  this.manualMin; }
            set
            {
                if (this.manualMin != value)
                {
                    this.manualMin = value;
                    this.FireChanged("ManualMin");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the color of the data markers for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "36, 64, 98")]
        public Windows.UI.Color MarkersColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.markersThemeColor))
                {
                    return this._context.GetThemeColor(this.markersThemeColor);
                }
                return this.markersColor;
            }
            set
            {
                if (this.markersColor != value)
                {
                    this.markersColor = value;
                    this.markersThemeColor = null;
                    this.FireChanged("MarkersColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the markers.
        /// </summary>
        /// <value>The theme color of the markers.</value>
        [DefaultValue("")]
        public string MarkersThemeColor
        {
            get { return  this.markersThemeColor; }
            set
            {
                if (this.markersThemeColor != value)
                {
                    this.markersThemeColor = value;
                    this.FireChanged("MarkersThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies how the vertical axis maximum is calculated for the sparklines in this sparkline group. 
        /// </summary>
        [DefaultValue(0)]
        public SparklineAxisMinMax MaxAxisType
        {
            get { return  this.maxAxisType; }
            set
            {
                if (this.maxAxisType != value)
                {
                    this.maxAxisType = value;
                    this.FireChanged("MaxAxisType");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies how the vertical axis minimum is calculated for the sparklines in this sparkline group. 
        /// </summary>
        [DefaultValue(0)]
        public SparklineAxisMinMax MinAxisType
        {
            get { return  this.minAxisType; }
            set
            {
                if (this.minAxisType != value)
                {
                    this.minAxisType = value;
                    this.FireChanged("MinAxisType");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the color of the negative data points for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "Brown")]
        public Windows.UI.Color NegativeColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.negativeThemeColor))
                {
                    return this._context.GetThemeColor(this.negativeThemeColor);
                }
                return this.negativeColor;
            }
            set
            {
                if (this.negativeColor != value)
                {
                    this.negativeColor = value;
                    this.negativeThemeColor = null;
                    this.FireChanged("NegativeColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the negative point.
        /// </summary>
        /// <value>The theme color of the negative point.</value>
        [DefaultValue("")]
        public string NegativeThemeColor
        {
            get { return  this.negativeThemeColor; }
            set
            {
                if (this.negativeThemeColor != value)
                {
                    this.negativeThemeColor = value;
                    this.FireChanged("NegativeThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies whether each sparkline in the sparkline group is displayed in a right-to-left manner. 
        /// </summary>
        [DefaultValue(false)]
        public bool RightToLeft
        {
            get { return  this.rightToLeft; }
            set
            {
                if (this.rightToLeft != value)
                {
                    this.rightToLeft = value;
                    this.FireChanged("RightToLeft");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies the color for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(typeof(Windows.UI.Color), "36, 64, 98")]
        public Windows.UI.Color SeriesColor
        {
            get
            {
                if ((this._context != null) && !string.IsNullOrEmpty(this.seriesThemeColor))
                {
                    return this._context.GetThemeColor(this.seriesThemeColor);
                }
                return this.seriesColor;
            }
            set
            {
                if (this.seriesColor != value)
                {
                    this.seriesColor = value;
                    this.seriesThemeColor = null;
                    this.FireChanged("SeriesColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the theme color of the series.
        /// </summary>
        /// <value>The theme color of the series.</value>
        [DefaultValue("")]
        public string SeriesThemeColor
        {
            get { return  this.seriesThemeColor; }
            set
            {
                if (this.seriesThemeColor != value)
                {
                    this.seriesThemeColor = value;
                    this.FireChanged("SeriesThemeColor");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that indicates whether the first data point is formatted differently for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowFirst
        {
            get { return  this.showFirst; }
            set
            {
                if (this.showFirst != value)
                {
                    this.showFirst = value;
                    this.FireChanged("ShowFirst");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies whether the data points with the highest value are formatted differently for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowHigh
        {
            get { return  this.showHigh; }
            set
            {
                if (this.showHigh != value)
                {
                    this.showHigh = value;
                    this.FireChanged("ShowHigh");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that indicates whether the last data point is formatted differently for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowLast
        {
            get { return  this.showLast; }
            set
            {
                if (this.showLast != value)
                {
                    this.showLast = value;
                    this.FireChanged("ShowLast");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies whether the data points with the lowest value are formatted differently for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowLow
        {
            get { return  this.showLow; }
            set
            {
                if (this.showLow != value)
                {
                    this.showLow = value;
                    this.FireChanged("ShowLow");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies whether data markers are displayed for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowMarkers
        {
            get { return  this.showMarkers; }
            set
            {
                if (this.showMarkers != value)
                {
                    this.showMarkers = value;
                    this.FireChanged("ShowMarkers");
                }
            }
        }

        /// <summary> 
        /// Gets or sets a value that specifies whether the negative data points are formatted differently for each sparkline in this sparkline group. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShowNegative
        {
            get { return  this.showNegative; }
            set
            {
                if (this.showNegative != value)
                {
                    this.showNegative = value;
                    this.FireChanged("ShowNegative");
                }
            }
        }
    }
}

