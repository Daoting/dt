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
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the base chart implementation.
    /// </summary>
    public abstract class SpreadChartBase : SpreadChartShapeBase, IThemeContextSupport
    {
        Dt.Cells.Data.ChartTitle _chartTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        public SpreadChartBase() : base(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        /// <param name="name">The chart name.</param>
        public SpreadChartBase(string name) : base(name, 0.0, 0.0, 200.0, 200.0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartBase" /> class.
        /// </summary>
        /// <param name="name">The chart name.</param>
        /// <param name="x">The x location of the chart.</param>
        /// <param name="y">The y location of the chart.</param>
        /// <param name="width">The width of the chart.</param>
        /// <param name="height">The height of the chart.</param>
        public SpreadChartBase(string name, double x, double y, double width, double height) : base(name, x, y, width, height)
        {
        }

        internal override void Init(string name, double x, double y, double width, double height)
        {
            base.Init(name, x, y, width, height);
            this._chartTitle = null;
        }

        internal virtual void NotifyChartAreaChanged(ChartArea chartArea, string changed)
        {
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if (((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "ChartTitle"))
            {
                this._chartTitle = (Dt.Cells.Data.ChartTitle) Serializer.DeserializeObj(typeof(Dt.Cells.Data.ChartTitle), reader);
                this._chartTitle.SetChartInternal(this);
            }
        }

        internal virtual void ResetElementsFont()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetFontFamily();
                this._chartTitle.ResetFontTheme();
            }
        }

        internal virtual void ResetElementsFontSize()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetFontSize();
            }
        }

        internal virtual void ResetElementsFontStretch()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetFontStretch();
            }
        }

        internal virtual void ResetElementsFontStyle()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetFontStyle();
            }
        }

        internal virtual void ResetElementsFontWeight()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetFontWeight();
            }
        }

        internal virtual void ResetElementsForeground()
        {
            if (this._chartTitle != null)
            {
                this._chartTitle.ResetForeground();
                this._chartTitle.ResetForegroundThemeColor();
            }
        }

        /// <summary>
        /// Resets the font family.
        /// </summary>
        public void ResetFontFamily()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontFamily();
            }
        }

        /// <summary>
        /// Resets the size of the font.
        /// </summary>
        public void ResetFontSize()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontSize();
            }
        }

        /// <summary>
        /// Resets the font stretch.
        /// </summary>
        public void ResetFontStretch()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontStretch();
            }
        }

        /// <summary>
        /// Resets the font style.
        /// </summary>
        public void ResetFontStyle()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontStyle();
            }
        }

        /// <summary>
        /// Resets the font theme.
        /// </summary>
        public void ResetFontTheme()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontTheme();
            }
        }

        /// <summary>
        /// Resets the font weight.
        /// </summary>
        public void ResetFontWeight()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetFontWeight();
            }
        }

        /// <summary>
        /// Resets the foreground.
        /// </summary>
        public void ResetForeground()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetForeground();
            }
        }

        /// <summary>
        /// Resets the color of the foreground theme.
        /// </summary>
        public void ResetForegroundThemeColor()
        {
            if (base._styleInfo != null)
            {
                base._styleInfo.ResetForegroundThemeColor();
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._chartTitle != null)
            {
                Serializer.SerializeObj(this._chartTitle, "ChartTitle", writer);
            }
        }

        /// <summary>
        /// Gets the actual font family of a chart.
        /// </summary>
        public Windows.UI.Xaml.Media.FontFamily ActualFontFamily
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontFamilySet)
                {
                    return base._styleInfo.FontFamily;
                }
                if (((base._themeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsFontThemeSet)
                {
                    Windows.UI.Xaml.Media.FontFamily themeFont = null;
                    if (!string.IsNullOrEmpty(this.FontTheme))
                    {
                        themeFont = base._themeContext.GetThemeFont(this.FontTheme);
                    }
                    if (themeFont == null)
                    {
                        themeFont = base._themeContext.GetThemeFont(SpreadChartShapeBase.FONTTHEME_BODY);
                    }
                    return themeFont;
                }
                if (((base.Sheet != null) && (base.Sheet.Workbook != null)) && ((base._styleInfo == null) || (!base._styleInfo.IsFontFamilySet && !base._styleInfo.IsFontThemeSet)))
                {
                    SpreadTheme currentTheme = base.Sheet.Workbook.CurrentTheme;
                    if ((currentTheme != null) && (currentTheme.BodyFontFamily != null))
                    {
                        return currentTheme.BodyFontFamily;
                    }
                }
                return DefaultStyleCollection.DefaultFontFamily;
            }
        }

        /// <summary>
        /// Gets the actual font size of a chart.
        /// </summary>
        public double ActualFontSize
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontSizeSet)
                {
                    return this.FontSize;
                }
                return DefaultStyleCollection.DefaultFontSize;
            }
        }

        /// <summary>
        /// Gets the actual font stretch of a chart.
        /// </summary>
        public Windows.UI.Text.FontStretch ActualFontStretch
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStretchSet)
                {
                    return base._styleInfo.FontStretch;
                }
                return Windows.UI.Text.FontStretch.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font style of a chart.
        /// </summary>
        public Windows.UI.Text.FontStyle ActualFontStyle
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStyleSet)
                {
                    return base._styleInfo.FontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font weight of a chart.
        /// </summary>
        public Windows.UI.Text.FontWeight ActualFontWeight
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontWeightSet)
                {
                    return base._styleInfo.FontWeight;
                }
                return FontWeights.Normal;
            }
        }

        /// <summary>
        /// Gets the actual foreground of a chart.
        /// </summary>
        public Brush ActualForeground
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsForegroundSet)
                {
                    return base._styleInfo.Foreground;
                }
                if (((base._themeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsForegroundThemeColorSet)
                {
                    Windows.UI.Color color = base._themeContext.GetThemeColor(this.ForegroundThemeColor);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        public Dt.Cells.Data.ChartTitle ChartTitle
        {
            get { return  this._chartTitle; }
            set
            {
                if (value != null)
                {
                    value.TitleType = TitleType.ChartTitle;
                    if (((value._styleInfo != null) && !value._styleInfo.IsFontSizeSet) || (value._styleInfo == null))
                    {
                        value.FontSize = UnitHelper.PointToPixel(18.0);
                    }
                    if (((value._styleInfo != null) && !value._styleInfo.IsFontWeightSet) || (value._styleInfo == null))
                    {
                        value.FontWeight = FontWeights.Bold;
                    }
                }
                this._chartTitle = value;
                if ((this._chartTitle != null) && !object.ReferenceEquals(this._chartTitle.Chart, this))
                {
                   ((ISpreadChartElement)(this._chartTitle)).Chart  = this;
                }
                base.RaisePropertyChanged("ChartTitle");
            }
        }

        /// <summary>
        /// Gets or sets the font family of the chart.
        /// </summary>
        public Windows.UI.Xaml.Media.FontFamily FontFamily
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontFamily;
                }
                return null;
            }
            set
            {
                this.ResetElementsFont();
                base.StyleInfo.FontFamily = value;
            }
        }

        /// <summary>
        /// Gets or sets the font size of the chart in points.
        /// </summary>
        public double FontSize
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontSize;
                }
                return -1.0;
            }
            set
            {
                this.ResetElementsFontSize();
                base.StyleInfo.FontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the font stretch of the chart.
        /// </summary>
        public Windows.UI.Text.FontStretch FontStretch
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontStretch;
                }
                return Windows.UI.Text.FontStretch.Normal;
            }
            set
            {
                this.ResetElementsFontStretch();
                base.StyleInfo.FontStretch = value;
            }
        }

        /// <summary>
        /// Gets or sets the font style of the chart.
        /// </summary>
        public Windows.UI.Text.FontStyle FontStyle
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
            set
            {
                this.ResetElementsFontStyle();
                base.StyleInfo.FontStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the font theme of the chart.
        /// </summary>
        public string FontTheme
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontTheme;
                }
                return null;
            }
            set
            {
                this.ResetElementsFont();
                base.StyleInfo.FontTheme = value;
            }
        }

        /// <summary>
        /// Gets or sets the font weight of the chart.
        /// </summary>
        public Windows.UI.Text.FontWeight FontWeight
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontWeight;
                }
                return FontWeights.Normal;
            }
            set
            {
                this.ResetElementsFontWeight();
                base.StyleInfo.FontWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets an object that describes the foreground for a chart.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the foreground for a chart.</value>
        public Brush Foreground
        {
            get
            {
                if (base._styleInfo == null)
                {
                    return null;
                }
                return base._styleInfo.Foreground;
            }
            set
            {
                this.ResetElementsForeground();
                base.StyleInfo.Foreground = value;
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:System.String" /> object that describes the foreground for a chart.
        /// </summary>
        /// <value>The <see cref="T:System.String" /> object that describes the foreground for a chart.</value>
        public string ForegroundThemeColor
        {
            get
            {
                if (base._styleInfo == null)
                {
                    return null;
                }
                return base._styleInfo.ForegroundThemeColor;
            }
            set
            {
                this.ResetElementsForeground();
                base.StyleInfo.ForegroundThemeColor = value;
            }
        }
    }
}

