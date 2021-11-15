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
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// A base class for all the elements of the chart.
    /// </summary>
    public class SpreadChartElement : ISpreadChartElement, IXmlSerializable
    {
        SpreadChartBase _chart;
        internal static SolidColorBrush _defaultAutomaticFill = null;
        internal static SolidColorBrush _defaultAutomaticStroke = null;
        bool _isAutomaticFill;
        bool _isAutomaticStroke;
        static readonly object _lock = new object();
        internal FloatingObjectStyleInfo _styleInfo;
        WorkingState _suspendState;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartElement" /> class.
        /// </summary>
        public SpreadChartElement()
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
            this._suspendState = new WorkingState();
        }

        internal SpreadChartElement(SpreadChartBase chart)
        {
            this._isAutomaticFill = true;
            this._isAutomaticStroke = true;
            this._suspendState = new WorkingState();
            this._chart = chart;
        }

        internal virtual void AfterReadXml()
        {
        }

        internal virtual void BeforeReadXml()
        {
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged(e.PropertyName);
        }

        void ISpreadChartElement.NotifyElementChanged(string changed)
        {
            if (((ISpreadChartElement) this).Chart != null)
            {
                ((ISpreadChartElement) this).Chart.NotifyChartAreaChanged(((ISpreadChartElement) this).Area, changed);
            }
        }

        internal bool IsEventsSuspend()
        {
            return this._suspendState.IsWorking;
        }

        internal void NotifyChartChanged(string changed)
        {
            ((ISpreadChartElement) this).NotifyElementChanged(changed);
        }

        internal virtual void OnChartChanged()
        {
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
                    this._styleInfo.PropertyChanged += new PropertyChangedEventHandler(this.FormatInfo_PropertyChanged);
                    return;
                }
                this._isAutomaticFill = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Resets the value of the Fill property.
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
        /// Resets the value of the FillThemeColor property.
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
        /// Resets the foreground.
        /// </summary>
        public void ResetForeground()
        {
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetForeground();
            }
        }

        /// <summary>
        /// Resets the color of the foreground theme.
        /// </summary>
        public void ResetForegroundThemeColor()
        {
            if (this._styleInfo != null)
            {
                this._styleInfo.ResetForegroundThemeColor();
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

        internal void ResumeEvents()
        {
            this._suspendState.Release();
        }

        internal virtual void SetChartInternal(SpreadChartBase chart)
        {
            this._chart = chart;
        }

        internal void SuspendEvents()
        {
            this._suspendState.AddRef();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.BeforeReadXml();
            while (reader.Read())
            {
                this.ReadXmlInternal(reader);
            }
            this.AfterReadXml();
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
        /// Gets the actual brush that specifies how the element's interior is painted.
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
                    color = Dt.Cells.Data.ColorHelper.UpdateColor(color, this.FillDrawingColorSettings, false);
                    return new SolidColorBrush(color);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the actual brush that specifies how the element's outline is painted.
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
                    return this.Stroke;
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
        /// Gets the fill brush that is created automatically.
        /// </summary>
        /// <value>
        /// The fill brush that is created automatically.
        /// </value>
        public virtual Brush AutomaticFill
        {
            get { return  DefaultAutomaticFill; }
        }

        /// <summary>
        /// Gets the stroke brush that is created automatically.
        /// </summary>
        /// <value>
        /// The stroke brush that is created automatically.
        /// </value>
        public virtual Brush AutomaticStroke
        {
            get { return  DefaultAutomaticStroke; }
        }

        internal virtual Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.DataPoint; }
        }

        internal SpreadChartBase ChartBase
        {
            get { return  ((ISpreadChartElement) this).Chart; }
            set { ((ISpreadChartElement) this).Chart = value; }
        }

        internal Dt.Xls.Chart.CompoundLineType CompoundLineType { get; set; }

        internal static SolidColorBrush DefaultAutomaticFill
        {
            get
            {
                if (_defaultAutomaticFill == null)
                {
                    lock (_lock)
                    {
                        if (_defaultAutomaticFill == null)
                        {
                            _defaultAutomaticFill = new SolidColorBrush(Colors.White);
                        }
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
                        {
                            _defaultAutomaticStroke = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0x86, 0x86, 0x86));
                        }
                    }
                }
                return _defaultAutomaticStroke;
            }
        }

        /// <summary>
        /// Gets or sets the brush that specifies how the element's interior is painted.
        /// </summary>
        /// <value>
        /// A brush that describes how the element's interior is painted.
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
                this._isAutomaticFill = false;
                this.StyleInfo.Fill = value;
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
                this._isAutomaticFill = false;
                this.StyleInfo.FillThemeColor = value;
            }
        }

        Dt.Cells.Data.ChartArea ISpreadChartElement.Area
        {
            get { return  this.ChartArea; }
        }

        SpreadChartBase ISpreadChartElement.Chart
        {
            get { return  this._chart; }
            set
            {
                this._chart = value;
                this.OnChartChanged();
            }
        }

        internal LineEndStyle HeadLineEndStyle { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to automatically create the fill brush.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatically creating the fill brush; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticFill
        {
            get { return  this._isAutomaticFill; }
            set
            {
                if (value != this.IsAutomaticFill)
                {
                    this._isAutomaticFill = value;
                    this.NotifyChartChanged("IsAutomaticFill");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to automatically create the outline brush.
        /// </summary>
        /// <value>
        /// <c>true</c> if automatically creating the outline brush; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutomaticStroke
        {
            get { return  this._isAutomaticStroke; }
            set
            {
                if (value != this.IsAutomaticStroke)
                {
                    this._isAutomaticStroke = value;
                    this.NotifyChartChanged("IsAutomaticStroke");
                }
            }
        }

        internal EndLineJoinType JoinType { get; set; }

        internal Dt.Xls.Chart.LineDashType LineDashType { get; set; }

        internal EndLineCap LineEndingCap { get; set; }

        internal Dt.Xls.Chart.PenAlignment PenAlignment { get; set; }

        /// <summary>
        /// Gets or sets the brush that specifies how the element outline is painted.
        /// </summary>
        /// <value>
        /// A brush that specifies how the element outline is painted. 
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
                this._isAutomaticStroke = false;
                this.StyleInfo.Stroke = value;
            }
        }

        /// <summary>
        /// Gets or sets the pattern of dashes and gaps that is used to outline elements.
        /// </summary>
        /// <value>
        /// A value that specify the pattern of dashes and gaps.
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
                this._isAutomaticStroke = false;
                this.StyleInfo.StrokeThemeColor = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the element outline.
        /// </summary>
        /// <value>
        /// The width of the element outline.
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
            set { this._styleInfo = value; }
        }

        internal LineEndStyle TailLineEndStyle { get; set; }

        internal IThemeSupport ThemeContext
        {
            get
            {
                if ((((ISpreadChartElement) this).Chart != null) && (((ISpreadChartElement) this).Chart.Worksheet != null))
                {
                    return ((ISpreadChartElement) this).Chart.Worksheet;
                }
                return null;
            }
        }
    }
}

