#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a cell in the GcSpreadSheet component. 
    /// </summary>
    public sealed class Cell
    {
        bool _cacheStyleObject;
        Column _colobj;
        int _columnSpan;
        Row _rowobj;
        int rowSpan;
        SheetArea _sheetArea;
        StyleInfo _styleInfo;
        int _viewcolumn;
        int _viewcolumn2;
        int _viewrow;
        int _viewrow2;
        Worksheet _worksheet;


        internal Cell(Worksheet worksheet, int row, int column, SheetArea sheetArea)
        {
            this.rowSpan = 1;
            this._columnSpan = 1;
            this._worksheet = worksheet;
            this._viewrow = this._viewrow2 = row;
            this._viewcolumn = this._viewcolumn2 = column;
            this._sheetArea = sheetArea;
        }

        internal Cell(Worksheet worksheet, int row, int column, int row2, int column2, SheetArea sheetArea)
        {
            this.rowSpan = 1;
            this._columnSpan = 1;
            this._worksheet = worksheet;
            this._viewrow = row;
            this._viewcolumn = column;
            this._viewrow2 = row2;
            this._viewcolumn2 = column2;
            this._sheetArea = sheetArea;
        }

        Cell(int row, int column)
        {
            this.rowSpan = 1;
            this._columnSpan = 1;
            this._viewrow = this._viewrow2 = row;
            this._viewcolumn = this._viewcolumn2 = column;
        }

        Cell(int row, int column, int row2, int column2)
        {
            this.rowSpan = 1;
            this._columnSpan = 1;
            this._viewrow = row;
            this._viewcolumn = column;
            this._viewrow2 = row2;
            this._viewcolumn2 = column2;
        }

        /// <summary>
        /// Occurs when a property value changes. 
        /// Remarks
        /// Puts all the delegate functions in a matrix to avoid a memory leak. 
        /// </summary>
        public event EventHandler<CellChangedEventArgs> Changed
        {
            add
            {
                if (this._worksheet != null)
                {
                    this._worksheet.CellChanged += value;
                }
            }
            remove
            {
                if (this._worksheet != null)
                {
                    this._worksheet.CellChanged -= value;
                }
            }
        }

        /// <summary>
        /// Gets an object that describes the displayed background of a cell. 
        /// </summary>
        public Brush ActualBackground
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    if (this._styleInfo.IsBackgroundSet())
                    {
                        return this._styleInfo.Background;
                    }
                    if (this._styleInfo.IsBackgroundThemeColorSet())
                    {
                        string name = this._styleInfo.BackgroundThemeColor;
                        Brush brush = null;
                        if ((!string.IsNullOrEmpty(name) && (this._worksheet != null)) && ((this._worksheet.Workbook != null) && (this._worksheet.Workbook.CurrentTheme != null)))
                        {
                            brush = new SolidColorBrush(this._worksheet.Workbook.GetThemeColor(name));
                        }
                        return brush;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the displayed left cell border. 
        /// </summary>
        public BorderLine ActualBorderBottom
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.BorderBottom;
            }
        }

        /// <summary>
        /// Gets the displayed left border of a cell. 
        /// </summary>
        public BorderLine ActualBorderLeft
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.BorderLeft;
            }
        }

        /// <summary>
        /// Gets the displayed right cell border. 
        /// </summary>
        public BorderLine ActualBorderRight
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.BorderRight;
            }
        }

        /// <summary>
        /// Gets the displayed top border of a cell. 
        /// </summary>
        public BorderLine ActualBorderTop
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.BorderTop;
            }
        }

        /// <summary>
        /// Gets the current cell data converter. 
        /// </summary>
        public DataValidator ActualDataValidator
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.DataValidator;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the user can currently set focus to the cell using the keyboard or mouse. 
        /// </summary>
        public bool ActualFocusable
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.Focusable;
            }
        }

        /// <summary>
        /// Gets or sets the displayed font family of the cell. 
        /// </summary>
        public FontFamily ActualFontFamily
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    if (this._styleInfo.IsFontFamilySet())
                    {
                        return this._styleInfo.FontFamily;
                    }
                    if (this._styleInfo.IsFontThemeSet())
                    {
                        string fontTheme = this._styleInfo.FontTheme;
                        IThemeSupport worksheet = this._worksheet;
                        if (worksheet != null)
                        {
                            return worksheet.GetThemeFont(fontTheme);
                        }
                    }
                }
                return this._styleInfo.FontFamily;
            }
        }

        /// <summary>
        /// Gets or sets the displayed cell font size in points. 
        /// </summary>
        public double ActualFontSize
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.FontSize;
            }
        }

        /// <summary>
        /// Gets or sets the current degree to which a font is condensed or expanded. 
        /// </summary>
        public FontStretch ActualFontStretch
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.FontStretch;
            }
        }

        /// <summary>
        /// Gets or sets the displayed cell font style. 
        /// </summary>
        public Windows.UI.Text.FontStyle ActualFontStyle
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.FontStyle;
            }
        }

        /// <summary>
        /// Gets or sets the displayed weight for the cell font. 
        /// </summary>
        public FontWeight ActualFontWeight
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.FontWeight;
            }
        }

        /// <summary>
        /// Gets an object that describes the displayed foreground of a cell. 
        /// </summary>
        public Brush ActualForeground
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    if (this._styleInfo.IsForegroundSet())
                    {
                        return this._styleInfo.Foreground;
                    }
                    if (this._styleInfo.IsForegroundThemeColorSet())
                    {
                        string name = this._styleInfo.ForegroundThemeColor;
                        SolidColorBrush brush = null;
                        if ((!string.IsNullOrEmpty(name) && (this._worksheet != null)) && ((this._worksheet.Workbook != null) && (this._worksheet.Workbook.CurrentTheme != null)))
                        {
                            brush = new SolidColorBrush(this._worksheet.Workbook.GetThemeColor(name));
                        }
                        return brush;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the current cell formatter. 
        /// </summary>
        public IFormatter ActualFormatter
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.Formatter;
            }
        }

        /// <summary>
        /// Gets or sets the displayed horizontal alignment of the cell contents. 
        /// </summary>
        public CellHorizontalAlignment ActualHorizontalAlignment
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.HorizontalAlignment;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether a cell is currently marked as locked from editing. 
        /// </summary>
        public bool ActualLocked
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.Locked;
            }
        }

        /// <summary>
        /// Gets or sets the current value that indicates whether the text shrinks to fit in the cell. 
        /// </summary>
        public bool ActualShrinkToFit
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.ShrinkToFit;
            }
        }
        public bool ActualStrikethrough
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return (((this._styleInfo != null) && this._styleInfo.IsStrikethroughSet()) && this._styleInfo.Strikethrough);
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the user can currently set focus to the cell using the Tab key. 
        /// </summary>
        public bool ActualTabStop
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.TabStop;
            }
        }

        /// <summary>
        /// Gets or sets the current amount used to indent the cell text. 
        /// </summary>
        [DefaultValue(0)]
        public int ActualTextIndent
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.TextIndent;
            }
        }

        public bool ActualUnderline
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return (((this._styleInfo != null) && this._styleInfo.IsUnderlineSet()) && this._styleInfo.Underline);
            }
        }

        /// <summary>
        /// Gets or sets the displayed vertical alignment of the cell contents. 
        /// </summary>
        public CellVerticalAlignment ActualVerticalAlignment
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.VerticalAlignment;
            }
        }

        /// <summary>
        /// Gets or sets the current value that indicates whether the text wraps in the cell. 
        /// </summary>
        public bool ActualWordWrap
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetActualStyleInfo(this._viewrow, this._viewcolumn);
                }
                return this._styleInfo.WordWrap;
            }
        }

        /// <summary>
        /// Gets or sets the background for a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Background;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Background = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Background");
            }
        }

        /// <summary>
        /// Gets or sets the background theme color for a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public string BackgroundThemeColor
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsBackgroundThemeColorSet())
                {
                    return this._styleInfo.BackgroundThemeColor;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.BackgroundThemeColor = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("BackgroundThemeColor");
            }
        }

        /// <summary>
        /// Gets or sets the right cell border. 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderBottom
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.BorderBottom;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.BorderBottom = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("BorderBottom");
            }
        }

        /// <summary>
        /// Gets or sets the left cell border. 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderLeft
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.BorderLeft;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.BorderLeft = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("BorderLeft");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderRight
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.BorderRight;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.BorderRight = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("BorderRight");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderTop
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.BorderTop;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.BorderTop = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("BorderTop");
            }
        }

        /// <summary>
        /// Gets the column that contains this cell. 
        /// </summary>
        public Column Column
        {
            get
            {
                if (this._colobj == null)
                {
                    this._colobj = new Column(this._worksheet, this._viewcolumn, this._viewcolumn2, this._sheetArea);
                }
                return this._colobj;
            }
        }

        /// <summary>
        /// Gets or sets the number of columns spanned by this cell. 
        /// </summary>
        [DefaultValue(1)]
        public int ColumnSpan
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    CellRange range = this._worksheet.GetSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                    if (((range != null) && (range.Row == this._viewrow)) && (range.Column == this._viewcolumn))
                    {
                        return range.ColumnCount;
                    }
                }
                return 1;
            }
            set
            {
                bool flag = false;
                if ((this._worksheet == null) && (0 < value))
                {
                    flag = this._columnSpan != value;
                    this._columnSpan = value;
                }
                else if (((this._worksheet != null) && this.IsValidIndex(this._viewrow, this._viewcolumn)) && (0 < value))
                {
                    CellRange range = this._worksheet.GetSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                    if ((range != null) && ((range.Row != this._viewrow) || (range.Column != this._viewcolumn)))
                    {
                        range = null;
                    }
                    if (range != null)
                    {
                        if ((value == 1) && (range.RowCount == 1))
                        {
                            this._worksheet.RemoveSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                        }
                        else
                        {
                            this._worksheet.AddSpanCell(this._viewrow, this._viewcolumn, range.RowCount, value, this._sheetArea);
                        }
                        flag = true;
                    }
                    else if (value != 1)
                    {
                        this._worksheet.AddSpanCell(this._viewrow, this._viewcolumn, 1, value, this._sheetArea);
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("ColumnSpan");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data validator for a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public DataValidator DataValidator
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    StyleInfo info = this._worksheet.GetDirectInfo(this._viewrow, this._viewcolumn, null, this._sheetArea);
                    if (info != null)
                    {
                        return info.DataValidator;
                    }
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.DataValidator;
                }
                return null;
            }
            set
            {
                DataValidator validator = value;
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            if (validator != null)
                            {
                                this._styleInfo.DataValidator = validator.Clone(i - this._viewrow, j - this._viewcolumn);
                            }
                            else
                            {
                                this._styleInfo.DataValidator = null;
                            }
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("DataValidator");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can set focus to the cell using the keyboard or mouse. 
        /// </summary>
        [DefaultValue(true)]
        public bool Focusable
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Focusable;
                }
                return true;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Focusable = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Focusable");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.FontFamily;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontFamily = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontFamily");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((double)9.0)]
        public double FontSize
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.FontSize;
                }
                return DefaultStyleCollection.DefaultFontSize;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontSize = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontSize");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string FontTheme
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontThemeSet())
                {
                    return this._styleInfo.FontTheme;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontTheme = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontTheme");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Foreground
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Foreground;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Foreground = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Foreground");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string ForegroundThemeColor
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsForegroundThemeColorSet())
                {
                    return this._styleInfo.ForegroundThemeColor;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ForegroundThemeColor = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("ForegroundThemeColor");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public IFormatter Formatter
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Formatter;
                }
                return null;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Formatter = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Formatter");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string Formula
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetFormula(this._viewrow, this._viewcolumn, 1, 1, this._sheetArea, false);
                }
                return string.Empty;
            }
            set
            {
                if (this._worksheet != null)
                {
                    if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                    {
                        int rowCount = (this._viewrow2 - this._viewrow) + 1;
                        int columnCount = (this._viewcolumn2 - this._viewcolumn) + 1;
                        this._worksheet.SetFormula(this._viewrow, this._viewcolumn, rowCount, columnCount, this._sheetArea, value, false);
                    }
                    this.RaisePropertyChanged("Formula");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(3)]
        public CellHorizontalAlignment HorizontalAlignment
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsHorizontalAlignmentSet())
                {
                    return this._styleInfo.HorizontalAlignment;
                }
                if ((this._sheetArea != SheetArea.ColumnHeader) && (this._sheetArea != (SheetArea.CornerHeader | SheetArea.RowHeader)))
                {
                    return CellHorizontalAlignment.General;
                }
                return CellHorizontalAlignment.Center;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.HorizontalAlignment = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("HorizontalAlignment");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the cell value is valid. 
        /// </summary>
        public bool IsValid
        {
            get { return  this.IsValidValue(this.Value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether a cell is marked as locked from editing. 
        /// </summary>
        [DefaultValue(true)]
        public bool Locked
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                return ((this._styleInfo != null) && this._styleInfo.Locked);
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Locked = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Locked");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Cells Parent
        {
            get
            {
                if (this._worksheet != null)
                {
                    switch (this._sheetArea)
                    {
                        case SheetArea.Cells:
                            return this._worksheet.Cells;

                        case (SheetArea.CornerHeader | SheetArea.RowHeader):
                            return this._worksheet.RowHeader.Cells;

                        case SheetArea.ColumnHeader:
                            return this._worksheet.ColumnHeader.Cells;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the parent style for a cell. 
        /// </summary>
        [DefaultValue("")]
        public string ParentStyleName
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.Parent;
                }
                return string.Empty;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Parent = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("ParentStyleName");
            }
        }

        /// <summary>
        /// Gets the row that contains this cell. 
        /// </summary>
        public Row Row
        {
            get
            {
                if (this._rowobj == null)
                {
                    this._rowobj = new Row(this._worksheet, this._viewrow, this._viewrow2, this._sheetArea);
                }
                return this._rowobj;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int RowSpan
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    CellRange range = this._worksheet.GetSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                    if (((range != null) && (range.Row == this._viewrow)) && (range.Column == this._viewcolumn))
                    {
                        return range.RowCount;
                    }
                }
                return 1;
            }
            set
            {
                bool flag = false;
                if ((this._worksheet == null) && (0 < value))
                {
                    flag = this.rowSpan != value;
                    this.rowSpan = value;
                }
                else if (((this._worksheet != null) && this.IsValidIndex(this._viewrow, this._viewcolumn)) && (0 < value))
                {
                    CellRange range = this._worksheet.GetSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                    if ((range != null) && ((range.Row != this._viewrow) || (range.Column != this._viewcolumn)))
                    {
                        range = null;
                    }
                    if (range != null)
                    {
                        if ((value == 1) && (range.ColumnCount == 1))
                        {
                            this._worksheet.RemoveSpanCell(this._viewrow, this._viewcolumn, this._sheetArea);
                        }
                        else
                        {
                            this._worksheet.AddSpanCell(this._viewrow, this._viewcolumn, value, range.ColumnCount, this._sheetArea);
                        }
                        flag = true;
                    }
                    else if (value != 1)
                    {
                        this._worksheet.AddSpanCell(this._viewrow, this._viewcolumn, value, 1, this._sheetArea);
                        flag = true;
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("RowSpan");
                }
            }
        }

        /// <summary>
        /// Gets the sheet area that the cell belongs to. 
        /// </summary>
        public SheetArea SheetArea
        {
            get { return  this._sheetArea; }
        }

        /// <summary>
        /// Gets or sets whether to support shrink to fit. 
        /// </summary>
        [DefaultValue(false)]
        public bool ShrinkToFit
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                return ((this._styleInfo != null) && this._styleInfo.ShrinkToFit);
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ShrinkToFit = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("ShrinkToFit");
            }
        }

        /// <summary>
        /// Gets or sets sparkline information for a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public Sparkline Sparkline
        {
            get
            {
                if ((this._sheetArea == SheetArea.Cells) && this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetSparkline(this._viewrow, this._viewcolumn);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the cell range of the sparkline. 
        /// </summary>
        public CellRange SparklineDataRange
        {
            get
            {
                Sparkline sparkline = this.Sparkline;
                if (sparkline != null)
                {
                    return ConvertDataRefferenceToCellRange(sparkline.DataReference);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the axis range of the sparkline. 
        /// </summary>
        public CellRange SparklineDateAxisRange
        {
            get
            {
                Sparkline sparkline = this.Sparkline;
                if (sparkline != null)
                {
                    return ConvertDataRefferenceToCellRange(sparkline.DateAxisReference);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether Strikethrough take effect. 
        /// </summary>
        [DefaultValue(false)]
        public bool Strikethrough
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                return ((this._styleInfo != null) && this._styleInfo.Strikethrough);
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Strikethrough = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Strikethrough");
            }
        }

        /// <summary>
        /// Gets or sets the custom style for the cell. 
        /// </summary>
        [DefaultValue("")]
        public string StyleName
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetStyleName(this._viewrow, this._viewcolumn, this._sheetArea);
                }
                return string.Empty;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this._worksheet.GetStyleName(i, j, this._sheetArea);
                            this._worksheet.SetStyleName(i, j, this._sheetArea, value);
                        }
                    }
                }
                this.RaisePropertyChanged("StyleName");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the user can set focus to the cell using the Tab key. 
        /// </summary>
        [DefaultValue(true)]
        public bool TabStop
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.TabStop;
                }
                return true;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.TabStop = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("TabStop");
            }
        }

        /// <summary>
        /// Gets or sets an application-defined tag value for a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public object Tag
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetTag(this._viewrow, this._viewcolumn, this._sheetArea);
                }
                return null;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                        {
                            if (this.IsValidIndex(i, j) && (this._worksheet.GetTag(i, j, this._sheetArea) != value))
                            {
                                this._worksheet.SetTag(i, j, this._sheetArea, value);
                                flag = true;
                            }
                        }
                    }
                    if (flag)
                    {
                        this.RaisePropertyChanged("Tag");
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string Text
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetText(this._viewrow, this._viewcolumn, this._sheetArea);
                }
                return string.Empty;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                        {
                            if (this.IsValidIndex(i, j) && (this._worksheet.GetText(i, j, this._sheetArea) != value))
                            {
                                if ((value != null) && (value.Length == 0))
                                {
                                    value = null;
                                }
                                this._worksheet.SetText(i, j, this._sheetArea, value);
                                flag = true;
                            }
                        }
                    }
                    if (flag)
                    {
                        this.RaisePropertyChanged("Text");
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the amount to indent the cell text. 
        /// </summary>
        [DefaultValue(0)]
        public int TextIndent
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if (this._styleInfo != null)
                {
                    return this._styleInfo.TextIndent;
                }
                return 0;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.TextIndent = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("TextIndent");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Underline
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                return ((this._styleInfo != null) && this._styleInfo.Underline);
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.Underline = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("Underline");
            }
        }

        /// <summary>
        /// Gets or sets the value in a cell. 
        /// </summary>
        [DefaultValue((string)null)]
        public object Value
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    return this._worksheet.GetValue(this._viewrow, this._viewcolumn, this._sheetArea);
                }
                return null;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                        {
                            if (this.IsValidIndex(i, j))
                            {
                                this._worksheet.SetValue(i, j, this._sheetArea, value, false);
                            }
                        }
                    }
                    this.RaisePropertyChanged("Value");
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the cell contents. 
        /// </summary>
        [DefaultValue(1)]
        public CellVerticalAlignment VerticalAlignment
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsVerticalAlignmentSet())
                {
                    return this._styleInfo.VerticalAlignment;
                }
                return CellVerticalAlignment.Center;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.VerticalAlignment = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("VerticalAlignment");
            }
        }

        /// <summary>
        /// Gets or sets whether to support word wrap. 
        /// </summary>
        [DefaultValue(false)]
        public bool WordWrap
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                return ((this._styleInfo != null) && this._styleInfo.WordWrap);
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.WordWrap = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("WordWrap");
            }
        }

        /// <summary>
        /// Gets the worksheet that the cell belongs to. 
        /// </summary>
        public Worksheet Worksheet
        {
            get { return  this._worksheet; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(FontStretch.Normal)]
        public FontStretch FontStretch
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontStretchSet())
                {
                    return this._styleInfo.FontStretch;
                }
                return FontStretch.Normal;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontStretch = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontStretch");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(Windows.UI.Text.FontStyle.Normal)]
        public Windows.UI.Text.FontStyle FontStyle
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontStyleSet())
                {
                    return this._styleInfo.FontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontStyle = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontStyle");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontWeight FontWeight
        {
            get
            {
                if (this.IsValidIndex(this._viewrow, this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewrow, this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontWeightSet())
                {
                    return this._styleInfo.FontWeight;
                }
                return FontWeights.Normal;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.FontWeight = value;
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
                this.RaisePropertyChanged("FontWeight");
            }
        }

        /// <summary>
        /// Caches the style object for performance reasons. 
        /// </summary>
        /// <param name="cache">if set to true all properties in the cell are cached.</param>
        /// <returns></returns>
        public bool CacheStyleObject(bool cache)
        {
            bool cacheStyleObject = this._cacheStyleObject;
            if (this._cacheStyleObject != cache)
            {
                this._cacheStyleObject = cache;
                if (cache && (this._styleInfo == null))
                {
                    this._styleInfo = this._worksheet.GetActualStyleInfo(this._viewrow, this._viewcolumn, this._sheetArea);
                }
            }
            return cacheStyleObject;
        }

        static CellRange ConvertDataRefferenceToCellRange(CalcExpression dataRefference)
        {
            CalcRangeExpression expression = dataRefference as CalcRangeExpression;
            if (expression != null)
            {
                return new CellRange(expression.StartRow, expression.StartColumn, (expression.EndRow - expression.StartRow) + 1, (expression.EndColumn - expression.StartColumn) + 1);
            }
            CalcExternalRangeExpression expression2 = dataRefference as CalcExternalRangeExpression;
            if (expression2 != null)
            {
                return new CellRange(expression2.StartRow, expression2.StartColumn, (expression2.EndRow - expression2.StartRow) + 1, (expression2.EndColumn - expression2.StartColumn) + 1);
            }
            return null;
        }

        public override bool Equals(object o)
        {
            if (!(o is Cell))
            {
                return false;
            }
            Cell cell = (Cell)o;
            return (((((this._worksheet == cell._worksheet) && (this._viewrow == cell._viewrow)) && ((this._viewcolumn == cell._viewcolumn) && (this._viewrow2 == cell._viewrow2))) && (this._viewcolumn2 == cell._viewcolumn2)) && (this._sheetArea == cell._sheetArea));
        }

        /// <summary>
        /// Fills the style information with the current style properties for a cell. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        void GetActualStyleInfo(int row, int column)
        {
            if (!this._cacheStyleObject || (this._styleInfo == null))
            {
                this._styleInfo = this._worksheet.GetActualStyleInfo(row, column, this._sheetArea);
            }
        }

        /// <summary>
        /// Fills the style information with the current style properties for a cell. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        void GetStyleInfo(int row, int column)
        {
            if (this._styleInfo == null)
            {
                this._styleInfo = new StyleInfo();
            }
            else if (!this._cacheStyleObject)
            {
                this._styleInfo.Reset();
            }
            if (!this._cacheStyleObject)
            {
                this._worksheet.GetDirectInfo(row, column, this._styleInfo, this._sheetArea);
            }
        }

        /// <summary>
        /// Checks whether the specified row and column indexes in the view are valid coordinates in the sheet's data model, and if so sets the row and column fields to the model row and column indexes. 
        /// </summary>
        /// <param name="viewrow"></param>
        /// <param name="viewcolumn"></param>
        /// <returns></returns>
        bool IsValidIndex(int viewrow, int viewcolumn)
        {
            if (this._cacheStyleObject)
            {
                return true;
            }
            if (this._worksheet != null)
            {
                int rowCount = this._worksheet.GetRowCount(this._sheetArea);
                int columnCount = this._worksheet.GetColumnCount(this._sheetArea);
                if (((0 <= viewrow) && (viewrow < rowCount)) && ((0 <= viewcolumn) && (viewcolumn < columnCount)))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified value is valid. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool IsValidValue(object value)
        {
            if (this._sheetArea == SheetArea.Cells)
            {
                if (this._worksheet == null)
                {
                    return false;
                }
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j) && !this._worksheet.IsValid(i, j, value))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (this._worksheet != null)
            {
                int rowCount = (this._viewrow2 - this._viewrow) + 1;
                int columnCount = (this._viewcolumn2 - this._viewcolumn) + 1;
                this._worksheet.RaiseCellChanged(propertyName, this.Row.Index, this.Column.Index, rowCount, columnCount, this.SheetArea);
            }
        }

        /// <summary>
        /// Resets the background color for the cell and causes the cell to inherit the background color from the default cell. 
        /// </summary>
        public void ResetBackground()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBackground();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBackground();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("Background");
        }

        /// <summary>
        /// Resets the background theme color for the cell and causes the cell to inherit the background theme color from the default cell. 
        /// </summary>
        public void ResetBackgroundThemeColor()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBackgroundThemeColor();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBackgroundThemeColor();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("BackgroundThemeColor");
        }

        /// <summary>
        /// Resets the bottom cell border for the cell and causes the cell to inherit the cell border from the default cell. 
        /// </summary>
        public void ResetBorderBottom()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBorderBottom();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBorderBottom();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("BorderBottom");
        }

        /// <summary>
        /// Resets the left cell border for the cell and causes the cell to inherit the cell border from the default cell. 
        /// </summary>
        public void ResetBorderLeft()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBorderLeft();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBorderLeft();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("BorderLeft");
        }

        /// <summary>
        /// Resets the right cell border for the cell and causes the cell to inherit the cell border from the default cell. 
        /// </summary>
        public void ResetBorderRight()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBorderRight();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBorderRight();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("BorderRight");
        }

        /// <summary>
        /// Resets the top cell border for the cell and causes the cell to inherit the cell border from the default cell. 
        /// </summary>
        public void ResetBorderTop()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetBorderTop();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetBorderTop();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("BorderTop");
        }

        /// <summary>
        /// Resets the data validator for the cell to null. 
        /// </summary>
        public void ResetDataValidator()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetDataValidator();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("DataValidator");
        }

        /// <summary>
        /// Resets whether the cell can receive focus to its default value. 
        /// </summary>
        public void ResetFocusable()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetFocusable();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetFocusable();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("Focusable");
        }

        /// <summary>
        /// Resets the font family for the cell and causes the cell to inherit the font family from the default cell. 
        /// </summary>
        public void ResetFontFamily()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontFamily();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontFamily");
        }

        /// <summary>
        /// Resets the font size for the cell and causes the cell to inherit the font size from the default cell. 
        /// </summary>
        public void ResetFontSize()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontSize();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontSize");
        }

        /// <summary>
        /// Resets the font stretch for the cell and causes the cell to inherit the font stretch from the default cell. 
        /// </summary>
        public void ResetFontStretch()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontStretch();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontStretch");
        }

        /// <summary>
        /// Resets the font style for the cell and causes the cell to inherit the font style from the default cell. 
        /// </summary>
        public void ResetFontStyle()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontStyle();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontStyle");
        }

        /// <summary>
        /// Resets the font theme for the cell and causes the cell to inherit the font theme from the default cell. 
        /// </summary>
        public void ResetFontTheme()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontTheme();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontTheme");
        }

        /// <summary>
        /// Resets the font weight for the cell and causes the cell to inherit the font weight from the default cell. 
        /// </summary>
        public void ResetFontWeight()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetFontWeight();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("FontWeight");
        }

        /// <summary>
        /// Resets the text (foreground) color for the cell and causes the cell to inherit the text color from the default cell. 
        /// </summary>
        public void ResetForeground()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetForeground();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetForeground();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("Foreground");
        }

        /// <summary>
        /// Resets the text (foreground) theme color for the cell and causes the cell to inherit the text (foreground) color from the default cell. 
        /// </summary>
        public void ResetForegroundThemeColor()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetForegroundThemeColor();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetForegroundThemeColor();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("ForegroundThemeColor");
        }

        /// <summary>
        /// Resets the formatter for the cell and causes the cell to inherit the formatter from the default cell. 
        /// </summary>
        public void ResetFormatter()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetFormatter();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetFormatter();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("Formatter");
        }

        /// <summary>
        /// Resets the horizontal alignment for the cell and makes the cell inherit the horizontal alignment from the default cell. 
        /// </summary>
        public void ResetHorizontalAlignment()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetHorizontalAlignment();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("HorizontalAlignment");
        }

        /// <summary>
        /// Resets the locked state for the cell and causes the cell to inherit the locked state from the default cell. 
        /// </summary>
        public void ResetLocked()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetLocked();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetLocked();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("Locked");
        }

        /// <summary>
        /// Resets the ParentStyleName object for the cell to an empty string. 
        /// </summary>
        public void ResetParentStyleName()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetParent();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetParent();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("ParentStyleName");
        }

        /// <summary>
        /// Resets the shrink to fit setting for the cell and causes the cell to inherit the shrink to fit setting from the default cell. 
        /// </summary>
        public void ResetShrinkToFit()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetShrinkToFit();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("ShrinkToFit");
        }

        /// <summary>
        /// Resets the StyleName object for the cell to empty. 
        /// </summary>
        public void ResetStyleName()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        object obj2 = this._worksheet.GetStyleObject(i, j, this._sheetArea);
                        if (obj2 is string)
                        {
                            this._worksheet.SetStyleName(i, j, this._sheetArea, "");
                        }
                        else if (obj2 is StyleInfo)
                        {
                            ((StyleInfo)obj2).ResetName();
                        }
                    }
                }
            }
            this.RaisePropertyChanged("StyleName");
        }

        /// <summary>
        /// Resets whether the user can set focus to the cell using the Tab key, to its default value. 
        /// </summary>
        public void ResetTabStop()
        {
            if (this._worksheet == null)
            {
                this._styleInfo.ResetTabStop();
            }
            else
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                    {
                        if (this.IsValidIndex(i, j))
                        {
                            this.GetStyleInfo(i, j);
                            this._styleInfo.ResetTabStop();
                            this.SetStyleInfo(i, j);
                        }
                    }
                }
            }
            this.RaisePropertyChanged("TabStop");
        }

        /// <summary>
        /// Resets the text for the cell to empty. 
        /// </summary>
        public void ResetText()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this._worksheet.SetText(i, j, this._sheetArea, string.Empty);
                    }
                }
            }
            this.RaisePropertyChanged("Text");
        }

        /// <summary>
        /// Resets the text indent for the cell and causes the cell to inherit the text indent from the default cell. 
        /// </summary>
        public void ResetTextIndent()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetTextIndent();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("TextIndent");
        }

        /// <summary>
        /// Resets the cell value to empty. 
        /// </summary>
        public void ResetValue()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this._worksheet.SetValue(i, j, this._sheetArea, null);
                    }
                }
            }
            this.RaisePropertyChanged("Value");
        }

        /// <summary>
        /// Resets the vertical alignment for the cell and makes the cell inherit the vertical alignment from the default cell. 
        /// </summary>
        public void ResetVerticalAlignment()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetVerticalAlignment();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("VerticalAlignment");
        }

        /// <summary>
        /// Resets the word wrap setting for the cell and causes the cell to inherit the word wrap setting from the default cell. 
        /// </summary>
        public void ResetWordWrap()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                for (int j = this._viewcolumn; j <= this._viewcolumn2; j++)
                {
                    if (this.IsValidIndex(i, j))
                    {
                        this.GetStyleInfo(i, j);
                        this._styleInfo.ResetWordWrap();
                        this.SetStyleInfo(i, j);
                    }
                }
            }
            this.RaisePropertyChanged("WordWrap");
        }

        /// <summary>
        /// Sets the style information into the style model for a cell. 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        void SetStyleInfo(int row, int column)
        {
            this._worksheet.SetStyleObject(row, column, this._sheetArea, this._styleInfo);
        }

        /// <summary>
        /// Gets a string that contains an absolute reference to this cell in the current reference style. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if ((this._viewrow == this._viewrow2) && (this._viewcolumn == this._viewcolumn2))
            {
                CalcCellExpression expression = new CalcCellExpression(this._viewrow, this._viewcolumn);
                return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
            }
            CalcRangeExpression expression2 = new CalcRangeExpression(this._viewrow, this._viewcolumn, this._viewrow2, this._viewcolumn2);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression2, 0, 0);
        }

        /// <summary>
        /// Gets a string that contains a relative reference to this cell in the current reference style. 
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <returns></returns>
        public string ToString(Cell relativeTo)
        {
            int index = 0;
            int num2 = 0;
            if (((relativeTo != null) && (relativeTo.Row != null)) && (relativeTo.Column != null))
            {
                index = relativeTo.Row.Index;
                num2 = relativeTo.Column.Index;
            }
            if ((this._viewrow == this._viewrow2) && (this._viewcolumn == this._viewcolumn2))
            {
                CalcCellExpression expression = new CalcCellExpression(this._viewrow - index, this._viewcolumn - num2, true, true);
                return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
            }
            int row = Math.Min(this._viewrow, this._viewrow2);
            int rowCount = (Math.Max(this._viewrow, this._viewrow2) - row) + 1;
            int column = Math.Min(this._viewcolumn, this._viewcolumn2);
            int columnCount = (Math.Max(this._viewcolumn, this._viewcolumn2) - column) + 1;
            row -= index;
            column -= num2;
            CalcRangeExpression expression2 = CalcExpressionHelper.CreateRangeExpressionByCount(row, column, rowCount, columnCount, true, true, true, true);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression2, 0, 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}