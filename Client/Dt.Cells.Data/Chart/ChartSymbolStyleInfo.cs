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
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines how you want the chart symbol to look.
    /// </summary>
    public class ChartSymbolStyleInfo : IXmlSerializable
    {
        bool _isAutomaticFill;
        bool _isAutomaticStroke;
        internal FloatingObjectStyleInfo _styleInfo;
        internal ISpreadChartElement _symbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartSymbolStyleInfo" /> class.
        /// </summary>
        public ChartSymbolStyleInfo()
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
        }

        internal ChartSymbolStyleInfo(ISpreadChartElement symbol)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
            this._symbol = symbol;
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this._symbol != null)
            {
                this._symbol.NotifyElementChanged(e.PropertyName);
            }
        }

        internal virtual void ReadXmlInternal(XmlReader reader)
        {
            string str;
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
                    this._styleInfo = (FloatingObjectStyleInfo) Serializer.DeserializeObj(typeof(FloatingObjectStyleInfo), reader);
                    return;
                }
                this._isAutomaticFill = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        internal void SetSymbolInternal(ISpreadChartElement symbol)
        {
            this._symbol = symbol;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                this.ReadXmlInternal(reader);
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlInternal(writer);
        }

        internal virtual void WriteXmlInternal(XmlWriter writer)
        {
            if (this._styleInfo != null)
            {
                Serializer.SerializeObj(this._styleInfo, "FloatingObjectStyleInfo", writer);
            }
            if (!this._isAutomaticFill)
            {
                Serializer.SerializeObj((bool) this._isAutomaticFill, "IsAutomaticFill", writer);
            }
            if (!this._isAutomaticStroke)
            {
                Serializer.SerializeObj((bool) this._isAutomaticStroke, "IsAutomaticStroke", writer);
            }
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
        /// Gets the actual fill.
        /// </summary>
        public virtual Brush ActualFill
        {
            get
            {
                if (this._isAutomaticFill)
                {
                    return this.AutomaticFill;
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFillSet)
                {
                    return this._styleInfo.Fill;
                }
                if (((this.ThemeContext != null) && (this._styleInfo != null)) && this._styleInfo.IsFillThemeColorSet)
                {
                    Windows.UI.Color color = this.ThemeContext.GetThemeColor(this.FillThemeColor);
                    color = ColorHelper.UpdateColor(color, this.FillDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                if (this.Chart != null)
                {
                    return this.Chart.ActualFill;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual stroke.
        /// </summary>
        public virtual Brush ActualStroke
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
                if (((this.ThemeContext != null) && (this._styleInfo != null)) && this._styleInfo.IsStrokeThemeColorSet)
                {
                    Windows.UI.Color color = this.ThemeContext.GetThemeColor(this.StrokeThemeColor);
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.StrokeDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the automatic fill.
        /// </summary>
        /// <value>
        /// The automatic fill.
        /// </value>
        public Brush AutomaticFill
        {
            get { return  null; }
        }

        /// <summary>
        /// Gets the automatic stroke.
        /// </summary>
        /// <value>
        /// The automatic stroke.
        /// </value>
        public Brush AutomaticStroke
        {
            get { return  null; }
        }

        internal SpreadChart Chart
        {
            get
            {
                if (this._symbol != null)
                {
                    return (this._symbol.Chart as SpreadChart);
                }
                return null;
            }
        }

        internal ISpreadChartElement ChartElement
        {
            get { return  this._symbol; }
            set
            {
                if (value != this._symbol)
                {
                    this._symbol = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the fill.
        /// </summary>
        /// <value>
        /// The fill.
        /// </value>
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
                this.StyleInfo.Fill = value;
                this._isAutomaticFill = false;
            }
        }

        internal SpreadDrawingColorSettings FillDrawingColorSettings { get; set; }

        /// <summary>
        /// Gets or sets the color of the fill theme.
        /// </summary>
        /// <value>
        /// The color of the fill theme.
        /// </value>
        public string FillThemeColor
        {
            get
            {
                if (this._styleInfo != null)
                {
                    return this._styleInfo.FillThemeColor;
                }
                return string.Empty;
            }
            set
            {
                this.StyleInfo.FillThemeColor = value;
                this._isAutomaticFill = false;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to automatically create the fill.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatically creating the fill; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticFill
        {
            get { return  this._isAutomaticFill; }
            set
            {
                if (value != this.IsAutomaticFill)
                {
                    this._isAutomaticFill = value;
                    if (this._symbol != null)
                    {
                        this._symbol.NotifyElementChanged("AutomaticFill");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to automatically create the border.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatically creating the border; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticStroke
        {
            get { return  this._isAutomaticStroke; }
            set
            {
                if (value != this.IsAutomaticStroke)
                {
                    this._isAutomaticStroke = value;
                    if (this._symbol != null)
                    {
                        this._symbol.NotifyElementChanged("AutomaticStroke");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the stroke.
        /// </summary>
        /// <value>
        /// The stroke.
        /// </value>
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
                this.StyleInfo.Stroke = value;
                this._isAutomaticStroke = false;
            }
        }

        /// <summary>
        /// Gets or sets the stroke dashes.
        /// </summary>
        /// <value>
        /// The stroke dashes.
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
        /// Gets or sets the color of the stroke theme.
        /// </summary>
        /// <value>
        /// The color of the stroke theme.
        /// </value>
        public string StrokeThemeColor
        {
            get
            {
                if (this._styleInfo == null)
                {
                    return string.Empty;
                }
                return this._styleInfo.StrokeThemeColor;
            }
            set
            {
                this.StyleInfo.StrokeThemeColor = value;
                this._isAutomaticStroke = false;
            }
        }

        /// <summary>
        /// Gets or sets the stroke thickness.
        /// </summary>
        /// <value>
        /// The stroke thickness.
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

        internal IThemeSupport ThemeContext
        {
            get
            {
                if ((this._symbol != null) && (this._symbol.Chart != null))
                {
                    return ((IThemeContextSupport) this._symbol.Chart).GetContext();
                }
                return null;
            }
        }
    }
}

