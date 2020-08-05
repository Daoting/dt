#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines how you want the chart label to appear.
    /// </summary>
    public class ChartLabelStyleInfo : ChartSymbolStyleInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartLabelStyleInfo" /> class.
        /// </summary>
        public ChartLabelStyleInfo()
        {
        }

        internal ChartLabelStyleInfo(ISpreadChartElement label) : base(label)
        {
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if (((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "ForegroundDrawingColorSettings"))
            {
                this.ForegroundColorSettings = (SpreadDrawingColorSettings) Serializer.DeserializeObj(typeof(SpreadDrawingColorSettings), reader);
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this.ForegroundColorSettings != null)
            {
                Serializer.SerializeObj(this.ForegroundColorSettings, "ForegroundDrawingColorSettings", writer);
            }
        }

        /// <summary>
        /// Gets the actual font family.
        /// </summary>
        public Windows.UI.Xaml.Media.FontFamily ActualFontFamily
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontFamilySet)
                {
                    return base._styleInfo.FontFamily;
                }
                if (((base.ThemeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsFontThemeSet)
                {
                    return base.ThemeContext.GetThemeFont(base._styleInfo.FontTheme);
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualFontFamily;
                }
                return DefaultStyleCollection.DefaultFontFamily;
            }
        }

        /// <summary>
        /// Gets the actual size of the font.
        /// </summary>
        /// <value>
        /// The actual size of the font.
        /// </value>
        public double ActualFontSize
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontSizeSet)
                {
                    return base._styleInfo.FontSize;
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualFontSize;
                }
                return -1.0;
            }
        }

        /// <summary>
        /// Gets the actual font stretch.
        /// </summary>
        public Windows.UI.Text.FontStretch ActualFontStretch
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStretchSet)
                {
                    return base._styleInfo.FontStretch;
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualFontStretch;
                }
                return Windows.UI.Text.FontStretch.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font style.
        /// </summary>
        public Windows.UI.Text.FontStyle ActualFontStyle
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStyleSet)
                {
                    return base._styleInfo.FontStyle;
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualFontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font weight.
        /// </summary>
        public Windows.UI.Text.FontWeight ActualFontWeight
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontWeightSet)
                {
                    return base._styleInfo.FontWeight;
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualFontWeight;
                }
                return FontWeights.Normal;
            }
        }

        /// <summary>
        /// Gets the actual foreground.
        /// </summary>
        public Brush ActualForeground
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsForegroundSet)
                {
                    return base._styleInfo.Foreground;
                }
                if (((base.ThemeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsForegroundThemeColorSet)
                {
                    Windows.UI.Color color = base.ThemeContext.GetThemeColor(this.ForegroundThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.ForegroundColorSettings, false);
                    return new SolidColorBrush(color);
                }
                if (base.Chart != null)
                {
                    return base.Chart.ActualForeground;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// </summary>
        /// <value>
        /// The corner radius.
        /// </value>
        internal double CornerRadius
        {
            get
            {
                if (base._styleInfo == null)
                {
                    return -1.0;
                }
                return base._styleInfo.CornerRadius;
            }
            set { base.StyleInfo.CornerRadius = value; }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>
        /// The font family.
        /// </value>
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
            set { base.StyleInfo.FontFamily = value; }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
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
            set { base.StyleInfo.FontSize = value; }
        }

        /// <summary>
        /// Gets or sets the font stretch.
        /// </summary>
        /// <value>
        /// The font stretch.
        /// </value>
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
            set { base.StyleInfo.FontStretch = value; }
        }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
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
            set { base.StyleInfo.FontStyle = value; }
        }

        /// <summary>
        /// Gets or sets the font theme.
        /// </summary>
        /// <value>
        /// The font theme.
        /// </value>
        public string FontTheme
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.FontTheme;
                }
                return string.Empty;
            }
            set { base.StyleInfo.FontTheme = value; }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
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
            set { base.StyleInfo.FontWeight = value; }
        }

        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
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
            set { base.StyleInfo.Foreground = value; }
        }

        internal SpreadDrawingColorSettings ForegroundColorSettings { get; set; }

        /// <summary>
        /// Gets or sets the color of the foreground theme.
        /// </summary>
        /// <value>
        /// The color of the foreground theme.
        /// </value>
        public string ForegroundThemeColor
        {
            get
            {
                if (base._styleInfo == null)
                {
                    return string.Empty;
                }
                return base._styleInfo.ForegroundThemeColor;
            }
            set { base.StyleInfo.ForegroundThemeColor = value; }
        }

        /// <summary>
        /// Gets or sets the formatter.
        /// </summary>
        /// <value>
        /// The formatter.
        /// </value>
        public IFormatter Formatter
        {
            get
            {
                if (base._styleInfo != null)
                {
                    return base._styleInfo.Formatter;
                }
                return null;
            }
            set { base.StyleInfo.Formatter = value; }
        }
    }
}

