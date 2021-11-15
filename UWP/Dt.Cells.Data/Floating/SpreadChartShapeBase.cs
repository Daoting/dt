#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base chart and shape implementation.
    /// </summary>
    public abstract class SpreadChartShapeBase : FloatingObject, IThemeContextSupport
    {
        internal static SolidColorBrush _defaultAutomaticFill = null;
        internal static SolidColorBrush _defaultAutomaticStroke = null;
        bool _isAutomaticFill;
        bool _isAutomaticStroke;
        static readonly object _lock = new object();
        internal FloatingObjectStyleInfo _styleInfo;
        internal IThemeSupport _themeContext;
        internal static string FONTTHEME_BODY = "Body";
        internal static string FONTTHEME_HEADING = "Headings";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        public SpreadChartShapeBase() : base(string.Empty)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        /// <param name="name">The name of the chart.</param>
        public SpreadChartShapeBase(string name) : base(name, 0.0, 0.0, 200.0, 200.0)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartShapeBase" /> class.
        /// </summary>
        /// <param name="name">The chart name.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        public SpreadChartShapeBase(string name, double x, double y) : base(name, x, y)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        /// <param name="name">The chart name.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        public SpreadChartShapeBase(string name, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.RaisePropertyChanged("Style");
        }

        IThemeSupport IThemeContextSupport.GetContext()
        {
            return this._themeContext;
        }

        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            if (this._themeContext != context)
            {
                this._themeContext = context;
            }
        }

        internal override void Init(string name, double x, double y, double width, double height)
        {
            base.Init(name, x, y, width, height);
            this._styleInfo = null;
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        internal override void OnOwnerChanged()
        {
            base.OnOwnerChanged();
            ((IThemeContextSupport) this).SetContext(base.Worksheet);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "FloatingObjectStyleInfo")
                {
                    if (str != "IsAutomaticFill")
                    {
                        if (str == "IsAutomaticStroke")
                        {
                            this._isAutomaticStroke = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                            return;
                        }
                        if (str == "FillDrawingColorSettings")
                        {
                            this.FillDrawingColorSettings = (SpreadDrawingColorSettings) Serializer.DeserializeObj(typeof(SpreadDrawingColorSettings), reader);
                            return;
                        }
                        if (str == "StrokeDrawingColorSettings")
                        {
                            this.StrokeDrawingColorSettings = (SpreadDrawingColorSettings) Serializer.DeserializeObj(typeof(SpreadDrawingColorSettings), reader);
                        }
                        return;
                    }
                }
                else
                {
                    this._styleInfo = Serializer.DeserializeObj(typeof(FloatingObjectStyleInfo), reader) as FloatingObjectStyleInfo;
                    return;
                }
                this._isAutomaticFill = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Resets the corner radius.
        /// </summary>
        public void ResetCornerRadius()
        {
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetCornerRadius();
            }
        }

        /// <summary>
        /// Resets the fill.
        /// </summary>
        public void ResetFill()
        {
            this._isAutomaticFill = true;
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetFill();
            }
        }

        /// <summary>
        /// Resets the color of the fill theme.
        /// </summary>
        public void ResetFillThemeColor()
        {
            this._isAutomaticFill = true;
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetFillThemeColor();
            }
        }

        /// <summary>
        /// Resets the stroke.
        /// </summary>
        public void ResetStroke()
        {
            this._isAutomaticStroke = true;
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetStroke();
            }
        }

        /// <summary>
        /// Resets the type of the stroke dash.
        /// </summary>
        public void ResetStrokeDashType()
        {
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetStrokeDashType();
            }
        }

        /// <summary>
        /// Resets the color of the stroke theme.
        /// </summary>
        public void ResetStrokeThemeColor()
        {
            this._isAutomaticStroke = true;
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetStrokeThemeColor();
            }
        }

        /// <summary>
        /// Resets the stroke thickness.
        /// </summary>
        public void ResetStrokeThickness()
        {
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetStrokeThickness();
            }
        }

        internal override void SetOwnerInternal(IList owner)
        {
            base.SetOwnerInternal(owner);
            ((IThemeContextSupport) this).SetContext(base.Worksheet);
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._styleInfo != null)
            {
                Serializer.SerializeObj(this._styleInfo, "FloatingObjectStyleInfo", writer);
            }
            Serializer.SerializeObj((bool) this.IsAutomaticFill, "IsAutomaticFill", writer);
            Serializer.SerializeObj((bool) this.IsAutomaticStroke, "IsAutomaticStroke", writer);
            if (this.FillDrawingColorSettings != null)
            {
                Serializer.SerializeObj(this.FillDrawingColorSettings, "FillDrawingColorSettings", writer);
            }
            if (this.StrokeDrawingColorSettings != null)
            {
                Serializer.SerializeObj(this.StrokeDrawingColorSettings, "StrokeDrawingColorSettings", writer);
            }
        }

        /// <summary>
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the background for a chart.
        /// </summary>
        public Brush ActualFill
        {
            get
            {
                if (this._isAutomaticFill)
                {
                    return this.AutomaticFill;
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFillSet)
                {
                    return this.Fill;
                }
                if (((this._themeContext != null) && (this._styleInfo != null)) && this._styleInfo.IsFillThemeColorSet)
                {
                    Windows.UI.Color color = this._themeContext.GetThemeColor(this.FillThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.FillDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the outline for a chart.
        /// </summary>
        public Brush ActualStroke
        {
            get
            {
                if (this._isAutomaticStroke)
                {
                    return this.AutomaticStroke;
                }
                if ((this._styleInfo != null) && this._styleInfo.IsStrokeSet)
                {
                    return this._styleInfo.Stroke;
                }
                if (((this._themeContext != null) && (this._styleInfo != null)) && this._styleInfo.IsStrokeThemeColorSet)
                {
                    Windows.UI.Color color = this._themeContext.GetThemeColor(this.StrokeThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.StrokeDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic background for a chart.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic background for a chart.</value>
        public virtual Brush AutomaticFill
        {
            get { return  DefaultAutomaticFill; }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic outline for a chart.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic outline for a chart.</value>
        public virtual Brush AutomaticStroke
        {
            get { return  DefaultAutomaticStroke; }
        }

        /// <summary>
        /// Gets or sets a value that represents the degree to which the corners of a chart are rounded.
        /// </summary>
        /// <value>
        /// The corner radius for the chart.
        /// </value>
        public double CornerRadius
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return -1.0;
                }
                return this._styleInfo.CornerRadius;
            }
            set { this.StyleInfo.CornerRadius = value; }
        }

        internal static SolidColorBrush DefaultAutomaticFill
        {
            get
            {
                if (_defaultAutomaticFill == null)
                {
                    lock (_lock)
                    {
                        if (_defaultAutomaticFill == null)
                            _defaultAutomaticFill = new SolidColorBrush(Colors.White);
                    }
                }
                return _defaultAutomaticFill;
            }
        }

        internal static SolidColorBrush DefaultAutomaticStroke
        {
            get
            {
                if (_defaultAutomaticStroke == null)
                {
                    lock (_lock)
                    {
                        if (_defaultAutomaticStroke == null)
                            _defaultAutomaticStroke = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x86, 0x86, 0x86));
                    }
                }
                return _defaultAutomaticStroke;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the background for a chart.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the background for a chart.</value>
        public Brush Fill
        {
            get
            {
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Fill;
                }
                return null;
            }
            set
            {
                this._isAutomaticFill = false;
                this.StyleInfo.Fill = value;
            }
        }

        internal SpreadDrawingColorSettings FillDrawingColorSettings { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="T:System.String" /> object that describes the background for a chart.
        /// </summary>
        /// <value>The <see cref="T:System.String" /> object that describes the background for a chart.</value>
        public string FillThemeColor
        {
            get
            {
                if (this._styleInfo != null)
                {
                    return this._styleInfo.FillThemeColor;
                }
                return null;
            }
            set
            {
                this._isAutomaticFill = false;
                this.StyleInfo.FillThemeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the chart is filled automatically.
        /// </summary>
        /// <value>
        /// <c>true</c> to fill the chart automatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticFill
        {
            get { return  this._isAutomaticFill; }
            set
            {
                if (value != this.IsAutomaticFill)
                {
                    this._isAutomaticFill = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the chart is outlined automatically.
        /// </summary>
        /// <value>
        /// <c>true</c> if the chart is outlined automatically; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticStroke
        {
            get { return  this._isAutomaticStroke; }
            set
            {
                if (value != this.IsAutomaticStroke)
                {
                    this._isAutomaticStroke = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the outline for a chart.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the outline for a chart.</value>
        public Brush Stroke
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return null;
                }
                return this._styleInfo.Stroke;
            }
            set
            {
                this._isAutomaticStroke = false;
                this.StyleInfo.Stroke = value;
            }
        }

        /// <summary>
        /// Gets or sets the pattern of dashes and gaps that is used to outline the chart.
        /// </summary>
        /// <value>
        /// The StrokeDashType value for the chart outline.
        /// </value>
        public Dt.Cells.Data.StrokeDashType StrokeDashType
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return Dt.Cells.Data.StrokeDashType.None;
                }
                return this._styleInfo.StrokeDashType;
            }
            set { this.StyleInfo.StrokeDashType = value; }
        }

        internal SpreadDrawingColorSettings StrokeDrawingColorSettings { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="T:System.String" /> object that describes the outline for a chart.
        /// </summary>
        /// <value>The <see cref="T:System.String" /> object that describes the outline for a chart.</value>
        public string StrokeThemeColor
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return null;
                }
                return this._styleInfo.StrokeThemeColor;
            }
            set
            {
                this._isAutomaticStroke = false;
                this.StyleInfo.StrokeThemeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the chart outline.
        /// </summary>
        /// <value>
        /// The width of the chart outline
        /// </value>
        public double StrokeThickness
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return 1.0;
                }
                return this._styleInfo.StrokeThickness;
            }
            set { this.StyleInfo.StrokeThickness = value; }
        }

        internal FloatingObjectStyleInfo StyleInfo
        {
            get
            {
                if (this._styleInfo == null)
                {
                    this._styleInfo = new FloatingObjectStyleInfo();
                    this._styleInfo.PropertyChanged += new PropertyChangedEventHandler(this.FormatInfo_PropertyChanged);
                }
                return this._styleInfo;
            }
        }
    }
}

