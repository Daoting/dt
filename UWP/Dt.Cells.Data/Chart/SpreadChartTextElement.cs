#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.Xls.Chart;

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// A spread chart element that displays text.
    /// </summary>
    public class SpreadChartTextElement : SpreadChartElement
    {
        Dt.Xls.Chart.RichText _richText;
        string _text;
        IExcelTextFormat _textFormat;
        string _textFormula;
        DataOrientation? _textOrientation;
        SheetCellRange _textRange;
        CalcExpression _textReference;
        StringSeriesCollection _textSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadChartTextElement" /> class.
        /// </summary>
        public SpreadChartTextElement()
        {
            this._textSeries = new StringSeriesCollection();
            this._textOrientation = 0;
        }

        internal SpreadChartTextElement(SpreadChartBase chart) : base(chart)
        {
            this._textSeries = new StringSeriesCollection();
            this._textOrientation = 0;
        }

        internal override void OnChartChanged()
        {
            base.OnChartChanged();
            this.UpdateTextReference();
        }

        internal virtual void OnResumeAfterDeserialization()
        {
            if (!string.IsNullOrEmpty(this._textFormula))
            {
                this.UpdateTextReference();
            }
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "Text")
                {
                    if (str != "TextFormula")
                    {
                        return;
                    }
                }
                else
                {
                    this._text = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                    return;
                }
                this._textFormula = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
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

        internal void ResumeAfterDeserialization()
        {
            base.SuspendEvents();
            this.OnResumeAfterDeserialization();
            base.ResumeEvents();
        }

        internal void UpdateTextReference()
        {
            SpreadChartBase chart = this.ChartBase;
            if (chart != null)
            {
                this.TextReference = FormulaUtility.Formula2Expression(chart.Sheet, this.TextFormula);
            }
        }

        void UpdateTextSeries()
        {
            if (this._textSeries.DataSeries != null)
            {
                this._textSeries.RefreshData();
            }
            else
            {
                this._textSeries.DataSeries = new TextDataSeries(this);
            }
        }

        void UpdateTextStyle()
        {
            if (this.TextForamt != null)
            {
                double? richTextFontSize = RichTextUtility.GetRichTextFontSize(this.TextForamt.TextParagraphs);
                if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                {
                    this.FontSize = richTextFontSize.Value;
                }
                this.FontStyle = RichTextUtility.GetRichTextFontStyle(this.TextForamt.TextParagraphs);
                this.FontWeight = RichTextUtility.GetRichTextFontWeight(this.TextForamt.TextParagraphs, FontWeights.Bold);
                SpreadChartBase chart = this.ChartBase;
                if (chart != null)
                {
                    this.Foreground = RichTextUtility.GetRichTextFill(this.TextForamt.TextParagraphs, chart.Worksheet.Workbook);
                }
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (!string.IsNullOrEmpty(this._textFormula))
            {
                Serializer.SerializeObj(this._textFormula, "TextFormula", writer);
            }
            else if (!string.IsNullOrEmpty(this.Text))
            {
                Serializer.SerializeObj(this.Text, "Text", writer);
            }
        }

        /// <summary>
        /// Gets the actual font family.
        /// </summary>
        public virtual Windows.UI.Xaml.Media.FontFamily ActualFontFamily
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontFamilySet)
                {
                    return base._styleInfo.FontFamily;
                }
                if (((base.ThemeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsFontFamilySet)
                {
                    return base.ThemeContext.GetThemeFont(this.FontTheme);
                }
                if (this._richText != null)
                {
                    Windows.UI.Xaml.Media.FontFamily richTextFamily = RichTextUtility.GetRichTextFamily(this.RichText.TextParagraphs);
                    if (richTextFamily != null)
                    {
                        return richTextFamily;
                    }
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualFontFamily;
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
        public virtual double ActualFontSize
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontSizeSet)
                {
                    return base._styleInfo.FontSize;
                }
                if (this.RichText != null)
                {
                    double? richTextFontSize = RichTextUtility.GetRichTextFontSize(this.RichText.TextParagraphs);
                    if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                    {
                        return richTextFontSize.Value;
                    }
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualFontSize;
                }
                return DefaultStyleCollection.DefaultFontSize;
            }
        }

        /// <summary>
        /// Gets the actual font stretch.
        /// </summary>
        public virtual Windows.UI.Text.FontStretch ActualFontStretch
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStretchSet)
                {
                    return base._styleInfo.FontStretch;
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualFontStretch;
                }
                return Windows.UI.Text.FontStretch.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font style.
        /// </summary>
        public virtual Windows.UI.Text.FontStyle ActualFontStyle
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontStyleSet)
                {
                    return base._styleInfo.FontStyle;
                }
                if (this.RichText != null)
                {
                    Windows.UI.Text.FontStyle richTextFontStyle = RichTextUtility.GetRichTextFontStyle(this.RichText.TextParagraphs);
                    if (richTextFontStyle != Windows.UI.Text.FontStyle.Normal)
                    {
                        return richTextFontStyle;
                    }
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualFontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
        }

        /// <summary>
        /// Gets the actual font weight.
        /// </summary>
        public virtual Windows.UI.Text.FontWeight ActualFontWeight
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsFontWeightSet)
                {
                    return base._styleInfo.FontWeight;
                }
                if (this.RichText != null)
                {
                    Windows.UI.Text.FontWeight richTextFontWeight = RichTextUtility.GetRichTextFontWeight(this.RichText.TextParagraphs);
                    if (!richTextFontWeight.Equals(FontWeights.Normal))
                    {
                        return richTextFontWeight;
                    }
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualFontWeight;
                }
                return FontWeights.Normal;
            }
        }

        /// <summary>
        /// Gets the actual foreground.
        /// </summary>
        public virtual Brush ActualForeground
        {
            get
            {
                if ((base._styleInfo != null) && base._styleInfo.IsForegroundSet)
                {
                    return base._styleInfo.Foreground;
                }
                if (this.RichText != null)
                {
                    return RichTextUtility.GetRichTextFill(this.RichText.TextParagraphs, base.ChartBase.Worksheet.Workbook);
                }
                if (((base.ThemeContext != null) && (base._styleInfo != null)) && base._styleInfo.IsForegroundThemeColorSet)
                {
                    Windows.UI.Color color = base.ThemeContext.GetThemeColor(this.ForegroundThemeColor);
                    return new SolidColorBrush(color);
                }
                if (base.ChartBase != null)
                {
                    return base.ChartBase.ActualForeground;
                }
                return null;
            }
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
                if (base._styleInfo != null)
                {
                    return base._styleInfo.Foreground;
                }
                return null;
            }
            set { base.StyleInfo.Foreground = value; }
        }

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
                    return null;
                }
                return base._styleInfo.ForegroundThemeColor;
            }
            set { base.StyleInfo.ForegroundThemeColor = value; }
        }

        internal virtual Dt.Xls.Chart.RichText RichText
        {
            get { return  this._richText; }
            set
            {
                if (value != this._richText)
                {
                    this._richText = value;
                    if (this._richText != null)
                    {
                        double? richTextFontSize = RichTextUtility.GetRichTextFontSize(this._richText.TextParagraphs);
                        if (richTextFontSize.HasValue && richTextFontSize.HasValue)
                        {
                            this.FontSize = richTextFontSize.Value;
                        }
                        FontWeight = RichTextUtility.GetRichTextFontWeight(this._richText.TextParagraphs);
                    }
                    this._text = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public virtual string Text
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this._text))
                {
                    return this._text;
                }
                if (this.RichText != null)
                {
                    List<string> list = new List<string>();
                    foreach (TextParagraph paragraph in this.RichText.TextParagraphs)
                    {
                        StringBuilder builder = new StringBuilder();
                        if (paragraph.TextRuns != null)
                        {
                            foreach (TextRun run in paragraph.TextRuns)
                            {
                                builder.Append(run.Text);
                            }
                        }
                        list.Add(builder.ToString());
                        list.Add("\r\n");
                    }
                    return string.Join(" ", (IEnumerable<string>) list).TrimEnd("\r\n".ToCharArray());
                }
                if ((this._textSeries == null) || (this._textSeries.Count <= 0))
                {
                    return string.Empty;
                }
                object obj2 = this._textSeries[0];
                string str = (obj2 != null) ? obj2.ToString() : string.Empty;
                for (int i = 1; i < this._textSeries.Count; i++)
                {
                    object obj3 = this._textSeries[i];
                    str = str + " " + ((obj3 != null) ? obj3.ToString() : string.Empty);
                }
                return str;
            }
            set
            {
                if (value != this.Text)
                {
                    this._text = value;
                    this._richText = null;
                    this._textFormula = null;
                    this._textReference = null;
                    this._textOrientation = null;
                    this._textSeries.DataSeries = null;
                    ((ISpreadChartElement) this).NotifyElementChanged("Text");
                }
            }
        }

        internal IExcelTextFormat TextForamt
        {
            get { return  this._textFormat; }
            set
            {
                if (value != this._textFormat)
                {
                    this._textFormat = value;
                    this.UpdateTextStyle();
                }
            }
        }

        /// <summary>
        /// Gets or sets the text formula.
        /// </summary>
        /// <value>
        /// The text formula.
        /// </value>
        public virtual string TextFormula
        {
            get { return  this._textFormula; }
            set
            {
                if (value != this.TextFormula)
                {
                    if (!value.StartsWith("="))
                    {
                        this._textFormula = "=" + value;
                    }
                    else
                    {
                        this._textFormula = value;
                    }
                    SpreadChartBase chart = this.ChartBase;
                    if (chart != null)
                    {
                        this.TextReference = FormulaUtility.Formula2Expression(chart.Sheet, value);
                    }
                    this._text = null;
                    ((ISpreadChartElement) this).NotifyElementChanged("TextFormula");
                }
            }
        }

        internal SheetCellRange TextRange
        {
            get { return  this._textRange; }
            private set
            {
                if (value != this.TextRange)
                {
                    if (value == null)
                    {
                        this._textRange = null;
                        this._textOrientation = 0;
                    }
                    else
                    {
                        if ((value.RowCount > 1) && (value.ColumnCount > 1))
                        {
                            throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                        }
                        this._textRange = value;
                        this._textOrientation = new DataOrientation?((value.ColumnCount == 1) ? DataOrientation.Vertical : DataOrientation.Horizontal);
                    }
                }
            }
        }

        CalcExpression TextReference
        {
            get { return  this._textReference; }
            set
            {
                if (value != this.TextReference)
                {
                    this._textReference = value;
                    SpreadChartBase chart = this.ChartBase;
                    if (chart != null)
                    {
                        SheetCellRange range = SpreadChartUtility.ExtractRange(chart.Sheet, value);
                        if (range == null)
                        {
                            throw new ArgumentException(ResourceStrings.NeedSingleCellRowColumn);
                        }
                        this.TextRange = range;
                        this.UpdateTextSeries();
                    }
                }
            }
        }

        class TextDataSeries : IDataSeries
        {
            SpreadChartTextElement _textElement;

            public TextDataSeries(SpreadChartTextElement textElement)
            {
                this._textElement = textElement;
            }

            public Dt.Cells.Data.DataOrientation? DataOrientation
            {
                get { return  this._textElement._textOrientation; }
            }

            public CalcExpression DataReference
            {
                get { return  this._textElement._textReference; }
            }

            public bool DisplayHiddenData
            {
                get { return  true; }
            }

            public Dt.Cells.Data.EmptyValueStyle EmptyValueStyle
            {
                get { return  Dt.Cells.Data.EmptyValueStyle.Connect; }
            }

            public ICalcEvaluator Evaluator
            {
                get
                {
                    if (this._textElement.ChartBase != null)
                    {
                        return this._textElement.ChartBase.Sheet;
                    }
                    return null;
                }
            }
        }
    }
}

