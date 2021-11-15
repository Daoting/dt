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
    /// Represents a row in the component
    /// </summary>
    public sealed class Row
    {
        internal const int _MAXROWHEIGHT = 0x98967f;
        AxisInfo _axisStyle;
        bool _isdefault;
        SheetArea _sheetArea;
        StyleInfo _styleInfo;
        int _viewrow;
        int _viewrow2;
        Worksheet _worksheet;


        internal Row(Worksheet worksheet, int row, SheetArea sheetArea)
        {
            this._viewrow = -1;
            this._viewrow2 = -1;
            this._styleInfo = new StyleInfo();
            this._axisStyle = new AxisInfo();
            this._worksheet = worksheet;
            this._viewrow = this._viewrow2 = row;
            this._sheetArea = sheetArea;
            this._isdefault = row == -1;
        }
        
        internal Row(Worksheet worksheet, int row, int row2, SheetArea sheetArea)
        {
            this._viewrow = -1;
            this._viewrow2 = -1;
            this._styleInfo = new StyleInfo();
            this._axisStyle = new AxisInfo();
            this._worksheet = worksheet;
            this._viewrow = row;
            this._viewrow2 = row2;
            this._sheetArea = sheetArea;
            this._isdefault = (row == -1) && (row2 == -1);
        }

        /// <summary>
        /// Occurs when a property value changes. 
        /// </summary>
        public event EventHandler<SheetChangedEventArgs> Changed
        {
            add
            {
                if (this._worksheet != null)
                {
                    this._worksheet.RowChanged += value;
                }
            }
            remove
            {
                if (this._worksheet != null)
                {
                    this._worksheet.RowChanged -= value;
                }
            }
        }
        
        public double ActualHeight
        {
            get
            {
                if (this._worksheet != null)
                {
                    return this._worksheet.GetActualRowHeight(this._viewrow, this._sheetArea);
                }
                return this.Height;
            }
        }

        public bool ActualVisible
        {
            get
            {
                if (this._worksheet != null)
                {
                    return this._worksheet.GetActualRowVisible(this._viewrow, this._sheetArea);
                }
                return this.IsVisible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(null)]
        public Brush Background
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Background;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.Background = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("Background");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string BackgroundThemeColor
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.BackgroundThemeColor;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.BackgroundThemeColor = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("BackgroundThemeColor");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderBottom
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.BorderBottom;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.BorderBottom != value)
                            {
                                this._styleInfo.BorderBottom = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("BorderBottom");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderLeft
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.BorderLeft;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.BorderLeft != value)
                            {
                                this._styleInfo.BorderLeft = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("BorderLeft");
                }
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.BorderRight;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.BorderRight != value)
                            {
                                this._styleInfo.BorderRight = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("BorderRight");
                }
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.BorderTop;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.BorderTop != value)
                            {
                                this._styleInfo.BorderTop = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("BorderTop");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool CanUserResize
        {
            get { return  (((this._isdefault || this.IsValid(this._viewrow)) && (this._worksheet != null)) && this._worksheet.GetRowResizable(this._viewrow, this._sheetArea)); }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if ((this._isdefault || this.IsValid(i)) && (this._worksheet.GetRowResizable(i, this._sheetArea) != value))
                        {
                            this._worksheet.SetRowResizable(i, this._sheetArea, value);
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("CanUserResize");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public DataValidator DataValidator
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.DataValidator;
            }
            set
            {
                DataValidator validator = value;
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.DataValidator = (validator == null) ? null : validator.Clone(i - this._viewrow, 0);
                        this.SetStyleInfo(i);
                    }
                }
                this.RaisePropertyChanged("DataValidator");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool Focusable
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Focusable;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.Focusable != value)
                            {
                                this._styleInfo.Focusable = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Focusable");
                }
            }
        }


        public FontFamily FontFamily
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.FontFamily;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.FontFamily = value;
                        this.SetStyleInfo(i);
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
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.FontSize;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.FontSize = value;
                        this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontThemeSet())
                {
                    return this._styleInfo.FontTheme;
                }
                return null;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.FontTheme = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Foreground;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.Foreground != value)
                            {
                                this._styleInfo.Foreground = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Foreground");
                }
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.ForegroundThemeColor;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.ForegroundThemeColor = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Formatter;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.Formatter != value)
                            {
                                this._styleInfo.Formatter = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Formatter");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string Formula
        {
            get
            {
                if (this.IsValid(this._viewrow) && (this._worksheet != null))
                {
                    object[,] objArray = this._worksheet.FindFormulas(this.Index, -1, (this.Index2 - this.Index) + 1, -1);
                    if ((objArray == null) || (objArray.Length <= 0))
                    {
                        return "";
                    }
                    for (int i = 0; i < (objArray.Length / objArray.Rank); i++)
                    {
                        CellRange range = objArray[i, 0] as CellRange;
                        if ((((range.Column == -1) && (range.ColumnCount == -1)) && ((range.Row == this.Index) && (range.RowCount == ((this.Index2 - this.Index) + 1)))) && (objArray[i, 1] != null))
                        {
                            return (string)((string)objArray[i, 1]);
                        }
                    }
                }
                return "";
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this._worksheet.SetFormula(this._viewrow, -1, (this._viewrow2 - this._viewrow) + 1, -1, value);
                        }
                    }
                }
                this.RaisePropertyChanged("Formula");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(20)]
        public double Height
        {
            get
            {
                if ((this._isdefault || this.IsValid(this._viewrow)) && (this._worksheet != null))
                {
                    return this._worksheet.GetRowHeight(this._viewrow, this._sheetArea);
                }
                return 0.0;
            }
            set
            {
                if ((value < -1.0) || (value > 9999999.0))
                {
                    throw new ArgumentOutOfRangeException("value", ResourceStrings.InvalidColumnWidth);
                }
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this._worksheet.SetRowHeight(i, this._sheetArea, value);
                        }
                    }
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
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.HorizontalAlignment;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.HorizontalAlignment = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("HorizontalAlignment");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index
        {
            get { return  this._viewrow; }
        }

        /// <summary>
        /// Gets the ending index for the range of rows. 
        /// </summary>
        public int Index2
        {
            get { return  this._viewrow2; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool IsFilteredOut
        {
            get { return  ((((this._sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) || (this._sheetArea == SheetArea.Cells)) && (this._worksheet != null)) && this._worksheet.IsRowFilteredOut(this._viewrow)); }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool IsVisible
        {
            get { return  (((this._isdefault || this.IsValid(this._viewrow)) && (this._worksheet != null)) && this._worksheet.GetRowVisible(this._viewrow, this._sheetArea)); }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this._worksheet.SetRowVisible(i, this._sheetArea, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string Label
        {
            get
            {
                if (((this._sheetArea != (SheetArea.CornerHeader | SheetArea.RowHeader)) && (this._sheetArea != SheetArea.Cells)) || !this.IsValid(this._viewrow))
                {
                    return null;
                }
                int autoTextIndex = this._worksheet.RowHeader.AutoTextIndex;
                if ((autoTextIndex < 0) || (autoTextIndex >= this._worksheet.RowHeaderColumnCount))
                {
                    autoTextIndex = this._worksheet.RowHeaderColumnCount - 1;
                }
                return this._worksheet.GetRowLabel(this._viewrow, autoTextIndex);
            }
            set
            {
                if (((this._sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) || (this._sheetArea == SheetArea.Cells)) && this.IsValid(this._viewrow))
                {
                    if (this._worksheet != null)
                    {
                        int autoTextIndex = this._worksheet.RowHeader.AutoTextIndex;
                        if ((autoTextIndex < 0) || (autoTextIndex >= this._worksheet.RowHeaderColumnCount))
                        {
                            autoTextIndex = this._worksheet.RowHeaderColumnCount - 1;
                        }
                        for (int i = this._viewrow; i <= this._viewrow2; i++)
                        {
                            this._worksheet.SetRowLabel(i, autoTextIndex, !string.IsNullOrEmpty(value) ? value : null);
                        }
                    }
                    this.RaisePropertyChanged("Label");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Locked
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Locked;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.Locked != value)
                            {
                                this._styleInfo.Locked = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Locked");
                }
            }
        }
        
        public Rows Parent
        {
            get
            {
                if (this._worksheet != null)
                {
                    switch (this._sheetArea)
                    {
                        case SheetArea.Cells:
                            return this._worksheet.Rows;

                        case (SheetArea.CornerHeader | SheetArea.RowHeader):
                            return this._worksheet.RowHeader.Rows;

                        case SheetArea.ColumnHeader:
                            return this._worksheet.ColumnHeader.Rows;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string ParentStyleName
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.Parent;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.Parent != value)
                            {
                                this._styleInfo.Parent = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("ParentStyleName");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool ShrinkToFit
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.ShrinkToFit;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.ShrinkToFit = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("ShrinkToFit");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string StyleName
        {
            get
            {
                if (!this._isdefault && !this.IsValid(this._viewrow))
                {
                    return string.Empty;
                }
                return this._worksheet.GetStyleName(this._viewrow, -1, this._sheetArea);
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if ((this._isdefault || this.IsValid(i)) && (this._worksheet.GetStyleName(i, -1, this._sheetArea) != value))
                        {
                            this._worksheet.SetStyleName(i, -1, this._sheetArea, value);
                            flag = true;
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("StyleName");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool TabStop
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.TabStop;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            if (this._styleInfo.TabStop != value)
                            {
                                this._styleInfo.TabStop = value;
                                this.SetStyleInfo(i);
                                flag = true;
                            }
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("TabStop");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public object Tag
        {
            get
            {
                if ((this._isdefault || this.IsValid(this._viewrow)) && (this._worksheet != null))
                {
                    return this._worksheet.GetTag(this._viewrow, -1, this._sheetArea);
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
                        if ((this._isdefault || this.IsValid(i)) && (this._worksheet.GetTag(i, -1, this._sheetArea) != value))
                        {
                            this._worksheet.SetTag(i, -1, this._sheetArea, value);
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

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public int TextIndent
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.TextIndent;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.TextIndent = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("TextIndent");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public CellVerticalAlignment VerticalAlignment
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.VerticalAlignment;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.VerticalAlignment = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("VerticalAlignment");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool WordWrap
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.WordWrap;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewrow; i <= this._viewrow2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.WordWrap = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("WordWrap");
            }
        }


        public FontStretch FontStretch
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.FontStretch;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.FontStretch = value;
                        this.SetStyleInfo(i);
                    }
                }
                this.RaisePropertyChanged("FontStretch");
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public Windows.UI.Text.FontStyle FontStyle
        {
            get
            {
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.FontStyle;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.FontStyle = value;
                        this.SetStyleInfo(i);
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
                if (this.IsValid(this._viewrow))
                {
                    this.GetStyleInfo(this._viewrow);
                }
                return this._styleInfo.FontWeight;
            }
            set
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.FontWeight = value;
                        this.SetStyleInfo(i);
                    }
                }
                this.RaisePropertyChanged("FontWeight");
            }
        }

        public override bool Equals(object o)
        {
            if (!(o is Row))
            {
                return false;
            }
            Row row = (Row)o;
            return ((((this._worksheet == row._worksheet) && (this._viewrow == row._viewrow)) && (this._viewrow2 == row._viewrow2)) && (this._sheetArea == row._sheetArea));
        }

        void GetAxisStyle(int row)
        {
            this._axisStyle = this._worksheet.GetRowAxisStyle(row, this._sheetArea);
        }

        void GetStyleInfo(int row)
        {
            this._styleInfo.Reset();
            this._worksheet.GetDirectInfo(row, -1, this._styleInfo, this._sheetArea);
        }

        public string GetText(int column)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                return this._worksheet.GetText(this.Index, column, this._sheetArea);
            }
            return string.Empty;
        }

        public object GetValue(int column)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                return this._worksheet.GetValue(this.Index, column, this._sheetArea);
            }
            return null;
        }

        bool IsValid(int viewrow)
        {
            if (this._worksheet != null)
            {
                int rowCount = this._worksheet.GetRowCount(this._sheetArea);
                return (viewrow < rowCount);
            }
            return false;
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (this._worksheet != null)
            {
                this._worksheet.RaiseRowChanged(propertyName, this.Index, (this.Index2 - this.Index) + 1, this._sheetArea, SheetChangedEventAction.Updated);
            }
        }

        public void Remove()
        {
            if (!this._isdefault && this.IsValid(this._viewrow))
            {
                for (int i = (this._viewrow2 - this._viewrow) + 1; i > 0; i--)
                {
                    this._worksheet.RemoveRows(this._viewrow, 1, this._sheetArea);
                }
            }
        }

        public void ResetBackground()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetBackground();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("Background");
        }

        public void ResetBackgroundThemeColor()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.ResetBackgroundThemeColor();
                        this.SetStyleInfo(i);
                    }
                }
            }
            this.RaisePropertyChanged("BackgroundThemeColor");
        }

        public void ResetBorderBottom()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetBorderBottom();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("BorderBottom");
        }

        public void ResetBorderLeft()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetBorderLeft();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("BorderLeft");
        }

        public void ResetBorderRight()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetBorderRight();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("BorderRight");
        }

        public void ResetBorderTop()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetBorderTop();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("BorderTop");
        }

        public void ResetCanUserResize()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    if (this._axisStyle != null)
                    {
                        this._axisStyle.ResetResizable();
                        this.SetAxisStyle(i);
                    }
                }
            }
            this.RaisePropertyChanged("CanUserResize");
        }

        public void ResetDataValidator()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.ResetDataValidator();
                        this.SetStyleInfo(i);
                    }
                }
            }
            this.RaisePropertyChanged("DataValidator");
        }

        public void ResetFocusable()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetFocusable();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("Focusable");
        }

        public void ResetFontFamily()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    if (this._styleInfo != null)
                    {
                        this._styleInfo.ResetFontFamily();
                    }
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("FontFamily");
        }

        public void ResetFontSize()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    if (this._styleInfo != null)
                    {
                        this._styleInfo.ResetFontSize();
                    }
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("FontSize");
        }

        public void ResetFontStretch()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    if (this._styleInfo != null)
                    {
                        this._styleInfo.ResetFontStretch();
                    }
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("FontStretch");
        }

        public void ResetFontStyle()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    if (this._styleInfo != null)
                    {
                        this._styleInfo.ResetFontStyle();
                    }
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("FontStyle");
        }

        public void ResetFontTheme()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.ResetFontTheme();
                        this.SetStyleInfo(i);
                    }
                }
            }
            this.RaisePropertyChanged("FontTheme");
        }


        public void ResetFontWeight()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    if (this._styleInfo != null)
                    {
                        this._styleInfo.ResetFontWeight();
                    }
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("FontWeight");
        }

        public void ResetForeground()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetForeground();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("Foreground");
        }

        public void ResetForegroundThemeColor()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.ResetForegroundThemeColor();
                        this.SetStyleInfo(i);
                    }
                }
            }
            this.RaisePropertyChanged("ForegroundThemeColor");
        }

        public void ResetFormatter()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetFormatter();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("Formatter");
        }

        public void ResetHeight()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    if (this._axisStyle != null)
                    {
                        this._axisStyle.ResetSize();
                        this.SetAxisStyle(i);
                    }
                }
            }
            this.RaisePropertyChanged("Height");
        }

        public void ResetHorizontalAlignment()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetHorizontalAlignment();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("HorizontalAlignment");
        }

        public void ResetIsVisible()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    if (this._axisStyle != null)
                    {
                        this._axisStyle.ResetVisible();
                        this.SetAxisStyle(i);
                    }
                }
            }
            this.RaisePropertyChanged("IsVisible");
        }

        public void ResetLabel()
        {
            if (((this._sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) || (this._sheetArea == SheetArea.Cells)) && ((this._viewrow != -1) && this.IsValid(this._viewrow)))
            {
                int rowHeaderAutoTextIndex = this._worksheet.RowHeaderAutoTextIndex;
                if (rowHeaderAutoTextIndex == -1)
                {
                    rowHeaderAutoTextIndex = this._worksheet.RowHeaderColumnCount - 1;
                }
                this._worksheet.SetRowLabel(this._viewrow, rowHeaderAutoTextIndex, null);
                this.RaisePropertyChanged("Label");
            }
        }

        public void ResetLocked()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetLocked();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("Locked");
        }

        public void ResetParentStyleName()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetParent();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("ParentStyleName");
        }

        public void ResetShrinkToFit()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetShrinkToFit();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("ShrinkToFit");
        }

        public void ResetStyleName()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewrow; i <= this._viewrow2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        object obj2 = this._worksheet.GetStyleObject(i, -1, this._sheetArea);
                        if (obj2 is string)
                        {
                            this._worksheet.SetStyleName(i, -1, this._sheetArea, "");
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

        public void ResetTabStop()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetTabStop();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("TabStop");
        }

        public void ResetTextIndent()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetTextIndent();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("TextIndent");
        }

        public void ResetVerticalAlignment()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetVerticalAlignment();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("VerticalAlignment");
        }

        public void ResetWordWrap()
        {
            for (int i = this._viewrow; i <= this._viewrow2; i++)
            {
                if (this.IsValid(i))
                {
                    this.GetStyleInfo(i);
                    this._styleInfo.ResetWordWrap();
                    this.SetStyleInfo(i);
                }
            }
            this.RaisePropertyChanged("WordWrap");
        }

        void SetAxisStyle(int row)
        {
            this._worksheet.SetRowAxisStyleInternal(row, this._sheetArea, this._axisStyle);
        }

        void SetStyleInfo(int row)
        {
            this._worksheet.SetStyleObject(row, -1, this._sheetArea, this._styleInfo);
        }

        public void SetText(int column, string text)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                this._worksheet.SetText(this.Index, column, this._sheetArea, text);
            }
        }

        public void SetValue(int column, object value)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                this._worksheet.SetValue(this.Index, column, this._sheetArea, value);
            }
        }

        public override string ToString()
        {
            CalcRangeExpression expression = new CalcRangeExpression(this._viewrow, this._viewrow2, false, false, true);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
        }

        public string ToString(Row relativeTo)
        {
            int index = 0;
            if (relativeTo != null)
            {
                index = relativeTo.Index;
            }
            int row = Math.Min(this._viewrow, this._viewrow2);
            int rowCount = (Math.Max(this._viewrow, this._viewrow2) - row) + 1;
            row -= index;
            CalcRangeExpression expression = CalcExpressionHelper.CreateRangeExpressionByCount(row, -1, rowCount, -1, true, true, true, true);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}