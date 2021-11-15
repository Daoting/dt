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
    /// Represents a column in the control. 
    /// Implements an accessor pattern to get or set properties to the owner Worksheet object. 
    /// </summary>
    public sealed class Column
    {
        internal const double _MAXCOLUMNWIDTH = 9999999.0;
        AxisInfo _axisStyle;
        bool _isdefault;
        SheetArea _sheetArea;
        StyleInfo _styleInfo;
        int _viewcolumn;
        int _viewcolumn2;
        Worksheet _worksheet;

        internal Column(Worksheet worksheet, int column, SheetArea sheetArea)
        {
            this._viewcolumn = -1;
            this._viewcolumn2 = -1;
            this._styleInfo = new StyleInfo();
            this._axisStyle = new AxisInfo();
            this._worksheet = worksheet;
            this._viewcolumn = this._viewcolumn2 = column;
            this._sheetArea = sheetArea;
            this._isdefault = column == -1;
        }

        internal Column(Worksheet worksheet, int column, int column2, SheetArea sheetArea)
        {
            this._viewcolumn = -1;
            this._viewcolumn2 = -1;
            this._styleInfo = new StyleInfo();
            this._axisStyle = new AxisInfo();
            this._worksheet = worksheet;
            this._viewcolumn = column;
            this._viewcolumn2 = column2;
            this._sheetArea = sheetArea;
            this._isdefault = (column == -1) && (column2 == -1);
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
                    this._worksheet.ColumnChanged += value;
                }
            }
            remove
            {
                if (this._worksheet != null)
                {
                    this._worksheet.ColumnChanged -= value;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether this column is currently visible. 
        /// </summary>
        public bool ActualVisible
        {
            get
            {
                if (this._worksheet != null)
                {
                    return this._worksheet.GetActualColumnVisible(this._viewcolumn, this._sheetArea);
                }
                return this.IsVisible;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ActualWidth
        {
            get
            {
                if (this._worksheet != null)
                {
                    return (double)((float)this._worksheet.GetActualColumnWidth(this._viewcolumn, this._sheetArea));
                }
                return this.Width;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Background;
            }
            set
            {
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.Background = value;
                        this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.BackgroundThemeColor;
            }
            set
            {
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.BackgroundThemeColor = value;
                        this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.BorderBottom;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.BorderBottom != value;
                        if (flag)
                        {
                            this._styleInfo.BorderBottom = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.BorderLeft;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.BorderLeft != value;
                        if (flag)
                        {
                            this._styleInfo.BorderLeft = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.BorderRight;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.BorderRight != value;
                        if (flag)
                        {
                            this._styleInfo.BorderRight = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.BorderTop;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.BorderTop != value;
                        if (flag)
                        {
                            this._styleInfo.BorderTop = value;
                            this.SetStyleInfo(i);
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
            get { return  (((this._isdefault || this.IsValid(this._viewcolumn)) && (this._worksheet != null)) && this._worksheet.GetColumnResizable(this._viewcolumn, this._sheetArea)); }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            flag = this._worksheet.GetColumnResizable(i, this._sheetArea) != value;
                            if (flag)
                            {
                                this._worksheet.SetColumnResizable(i, this._sheetArea, value);
                            }
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
        [DefaultValue("")]
        public string DataField
        {
            get
            {
                if ((((this._sheetArea == SheetArea.ColumnHeader) || (this._sheetArea == SheetArea.Cells)) && (this._isdefault || this.IsValid(this._viewcolumn))) && (this._worksheet != null))
                {
                    return this._worksheet.GetDataColumnName(this._viewcolumn);
                }
                return string.Empty;
            }
            set
            {
                if ((this._sheetArea == SheetArea.ColumnHeader) || (this._sheetArea == SheetArea.Cells))
                {
                    bool flag = false;
                    if (this._worksheet != null)
                    {
                        for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                        {
                            if (this._isdefault || this.IsValid(i))
                            {
                                this._worksheet.BindDataColumn(i, value);
                                flag = true;
                            }
                        }
                        if (flag)
                        {
                            this.RaisePropertyChanged("DataField");
                        }
                    }
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.DataValidator;
            }
            set
            {
                DataValidator validator = value;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        this._styleInfo.DataValidator = (validator == null) ? null : validator.Clone(i - this._viewcolumn, 0);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Focusable;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.Focusable != value;
                        if (flag)
                        {
                            this._styleInfo.Focusable = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Focusable");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontFamily FontFamily
        {
            get
            {
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.FontFamily;
            }
            set
            {
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.FontSize;
            }
            set
            {
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
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
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
        [DefaultValue(FontStretch.Normal)]
        public FontStretch FontStretch
        {
            get
            {
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontStretchSet())
                {
                    return this._styleInfo.FontStretch;
                }
                return FontStretch.Normal;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.FontStretch = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontStyleSet())
                {
                    return this._styleInfo.FontStyle;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.FontStyle = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                if ((this._styleInfo != null) && this._styleInfo.IsFontWeightSet())
                {
                    return this._styleInfo.FontWeight;
                }
                return FontWeights.Normal;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this.GetStyleInfo(i);
                            this._styleInfo.FontWeight = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                this.RaisePropertyChanged("FontWeight");
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Foreground;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.Foreground != value;
                        if (flag)
                        {
                            this._styleInfo.Foreground = value;
                            this.SetStyleInfo(i);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.ForegroundThemeColor;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Formatter;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.Formatter != value;
                        if (flag)
                        {
                            this._styleInfo.Formatter = value;
                            this.SetStyleInfo(i);
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
                if (this.IsValid(this._viewcolumn) && (this._worksheet != null))
                {
                    object[,] objArray = this._worksheet.FindFormulas(-1, this.Index, -1, (this.Index2 - this.Index) + 1);
                    if ((objArray == null) || (objArray.Length <= 0))
                    {
                        return "";
                    }
                    for (int i = 0; i < (objArray.Length / objArray.Rank); i++)
                    {
                        CellRange range = objArray[i, 0] as CellRange;
                        if ((((range.Row == -1) && (range.RowCount == -1)) && ((range.Column == this.Index) && (range.ColumnCount == ((this.Index2 - this.Index) + 1)))) && (objArray[i, 1] != null))
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
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this._worksheet.SetFormula(-1, this._viewcolumn, -1, (this._viewcolumn2 - this._viewcolumn) + 1, value);
                        }
                    }
                }
                this.RaisePropertyChanged("Formula");
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.HorizontalAlignment;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            get { return  this._viewcolumn; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Index2
        {
            get { return  this._viewcolumn2; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool IsVisible
        {
            get { return  (((this._isdefault || this.IsValid(this._viewcolumn)) && (this._worksheet != null)) && this._worksheet.GetColumnVisible(this._viewcolumn, this._sheetArea)); }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this._isdefault || this.IsValid(i))
                        {
                            this._worksheet.SetColumnVisible(i, this._sheetArea, value);
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
                if (((this._sheetArea != SheetArea.ColumnHeader) && (this._sheetArea != SheetArea.Cells)) || !this.IsValid(this._viewcolumn))
                {
                    return null;
                }
                int autoTextIndex = this._worksheet.ColumnHeader.AutoTextIndex;
                if ((autoTextIndex < 0) || (autoTextIndex >= this._worksheet.ColumnHeaderRowCount))
                {
                    autoTextIndex = this._worksheet.ColumnHeaderRowCount - 1;
                }
                return this._worksheet.GetColumnLabel(autoTextIndex, this._viewcolumn);
            }
            set
            {
                if (((this._sheetArea == SheetArea.ColumnHeader) || (this._sheetArea == SheetArea.Cells)) && this.IsValid(this._viewcolumn))
                {
                    int autoTextIndex = this._worksheet.ColumnHeader.AutoTextIndex;
                    if ((autoTextIndex < 0) || (autoTextIndex >= this._worksheet.ColumnHeaderRowCount))
                    {
                        autoTextIndex = this._worksheet.ColumnHeaderRowCount - 1;
                    }
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if (this.IsValid(i))
                        {
                            this._worksheet.SetColumnLabel(autoTextIndex, i, !string.IsNullOrEmpty(value) ? value : null);
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Locked;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.Locked != value;
                        if (flag)
                        {
                            this._styleInfo.Locked = value;
                            this.SetStyleInfo(i);
                        }
                    }
                }
                if (flag)
                {
                    this.RaisePropertyChanged("Locked");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Columns Parent
        {
            get
            {
                if (this._worksheet != null)
                {
                    switch (this._sheetArea)
                    {
                        case SheetArea.Cells:
                            return this._worksheet.Columns;

                        case (SheetArea.CornerHeader | SheetArea.RowHeader):
                            return this._worksheet.RowHeader.Columns;

                        case SheetArea.ColumnHeader:
                            return this._worksheet.ColumnHeader.Columns;
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.Parent;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.Parent != value;
                        if (flag)
                        {
                            this._styleInfo.Parent = value;
                            this.SetStyleInfo(i);
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.ShrinkToFit;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                if (!this._isdefault && !this.IsValid(this._viewcolumn))
                {
                    return string.Empty;
                }
                return this._worksheet.GetStyleName(-1, this._viewcolumn, this._sheetArea);
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if ((this._isdefault || this.IsValid(i)) && (this._worksheet.GetStyleName(-1, i, this._sheetArea) != value))
                    {
                        this._worksheet.SetStyleName(-1, i, this._sheetArea, value);
                        flag = true;
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
                if (this._isdefault || this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.TabStop;
            }
            set
            {
                bool flag = false;
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this.GetStyleInfo(i);
                        flag = this._styleInfo.TabStop != value;
                        if (flag)
                        {
                            this._styleInfo.TabStop = value;
                            this.SetStyleInfo(i);
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
                if ((this._isdefault || this.IsValid(this._viewcolumn)) && (this._worksheet != null))
                {
                    return this._worksheet.GetTag(-1, this._viewcolumn, this._sheetArea);
                }
                return null;
            }
            set
            {
                bool flag = false;
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                    {
                        if ((this._isdefault || this.IsValid(i)) && (this._worksheet.GetTag(-1, i, this._sheetArea) != value))
                        {
                            this._worksheet.SetTag(-1, i, this._sheetArea, value);
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.TextIndent;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.VerticalAlignment;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
        [DefaultValue(60)]
        public double Width
        {
            get
            {
                if ((this._isdefault || this.IsValid(this._viewcolumn)) && (this._worksheet != null))
                {
                    return (double)((float)this._worksheet.GetColumnWidth(this._viewcolumn, this._sheetArea));
                }
                return 0.0;
            }
            set
            {
                if ((value < -1.0) || (value > 9999999.0))
                {
                    throw new ArgumentOutOfRangeException("value", ResourceStrings.InvalidColumnWidth);
                }
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        this._worksheet.SetColumnWidth(i, this._sheetArea, value);
                    }
                }
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
                if (this.IsValid(this._viewcolumn))
                {
                    this.GetStyleInfo(this._viewcolumn);
                }
                return this._styleInfo.WordWrap;
            }
            set
            {
                if (this._worksheet != null)
                {
                    for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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

        public override bool Equals(object o)
        {
            if (!(o is Column))
            {
                return false;
            }
            Column column = (Column)o;
            return ((((this._worksheet == column._worksheet) && (this._viewcolumn == column._viewcolumn)) && (this._viewcolumn2 == column._viewcolumn2)) && (this._sheetArea == column._sheetArea));
        }


        void GetAxisStyle(int column)
        {
            this._axisStyle = this._worksheet.GetColumnAxisStyle(column, this._sheetArea);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        void GetStyleInfo(int column)
        {
            this._styleInfo.Reset();
            this._worksheet.GetDirectInfo(-1, column, this._styleInfo, this._sheetArea);
        }


        public string GetText(int row)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                return this._worksheet.GetText(row, this.Index, this._sheetArea);
            }
            return string.Empty;
        }


        public object GetValue(int row)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                return this._worksheet.GetValue(row, this.Index, this._sheetArea);
            }
            return null;
        }


        bool IsValid(int viewcolumn)
        {
            if (this._worksheet != null)
            {
                int columnCount = this._worksheet.GetColumnCount(this._sheetArea);
                return (viewcolumn < columnCount);
            }
            return false;
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (this._worksheet != null)
            {
                this._worksheet.RaiseColumnChanged(propertyName, this.Index, (this.Index2 - this.Index) + 1, this._sheetArea, SheetChangedEventAction.Updated);
            }
        }

        public void Remove()
        {
            if (!this._isdefault && this.IsValid(this._viewcolumn))
            {
                for (int i = (this._viewcolumn2 - this._viewcolumn) + 1; i > 0; i--)
                {
                    this._worksheet.RemoveColumns(this._viewcolumn, 1, this._sheetArea);
                }
            }
        }


        public void ResetBackground()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    this._axisStyle.ResetResizable();
                    this.SetAxisStyle(i);
                }
            }
            this.RaisePropertyChanged("CanUserResize");
        }
        
        public void ResetDataValidator()
        {
            if (this._worksheet != null)
            {
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
            {
                this.GetStyleInfo(i);
                this._styleInfo.ResetFontStretch();
                this.SetStyleInfo(i);
            }
            this.RaisePropertyChanged("FontStretch");
        }
        
        public void ResetFontStyle()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
        
        public void ResetHorizontalAlignment()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    this._axisStyle.ResetVisible();
                    this.SetAxisStyle(i);
                }
            }
            this.RaisePropertyChanged("IsVisible");
        }

        public void ResetLabel()
        {
            if (((this._sheetArea == SheetArea.ColumnHeader) || (this._sheetArea == SheetArea.Cells)) && ((this._viewcolumn != -1) && this.IsValid(this._viewcolumn)))
            {
                int columnHeaderAutoTextIndex = this._worksheet.ColumnHeaderAutoTextIndex;
                if (columnHeaderAutoTextIndex == -1)
                {
                    columnHeaderAutoTextIndex = this._worksheet.ColumnHeaderRowCount - 1;
                }
                this._worksheet.SetColumnLabel(columnHeaderAutoTextIndex, this._viewcolumn, null);
                this.RaisePropertyChanged("Label");
            }
        }

        public void ResetLocked()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
                for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
                {
                    if (this._isdefault || this.IsValid(i))
                    {
                        object obj2 = this._worksheet.GetStyleObject(-1, i, this._sheetArea);
                        if (obj2 is string)
                        {
                            this._worksheet.SetStyleName(-1, i, this._sheetArea, "");
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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

        public void ResetWidth()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
            {
                if (this._isdefault || this.IsValid(i))
                {
                    this.GetAxisStyle(i);
                    this._axisStyle.ResetSize();
                    this.SetAxisStyle(i);
                }
            }
            this.RaisePropertyChanged("Width");
        }

        public void ResetWordWrap()
        {
            for (int i = this._viewcolumn; i <= this._viewcolumn2; i++)
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

        void SetAxisStyle(int column)
        {
            this._worksheet.SetColumnAxisStyleInternal(column, this._sheetArea, this._axisStyle);
        }

        void SetStyleInfo(int column)
        {
            this._worksheet.SetStyleObject(-1, column, this._sheetArea, this._styleInfo);
        }

        public void SetText(int row, string text)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                this._worksheet.SetText(row, this.Index, this._sheetArea, text);
            }
        }

        public void SetValue(int row, object value)
        {
            if ((this._worksheet != null) && !this._isdefault)
            {
                this._worksheet.SetValue(row, this.Index, this._sheetArea, value);
            }
        }

        public override string ToString()
        {
            CalcRangeExpression expression = new CalcRangeExpression(this._viewcolumn, this._viewcolumn2, false, false, false);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
        }

        public string ToString(Column relativeTo)
        {
            int index = 0;
            if (relativeTo != null)
            {
                index = relativeTo.Index;
            }
            int column = Math.Min(this._viewcolumn, this._viewcolumn2);
            int columnCount = (Math.Max(this._viewcolumn, this._viewcolumn2) - column) + 1;
            column -= index;
            CalcRangeExpression expression = CalcExpressionHelper.CreateRangeExpressionByCount(-1, column, -1, columnCount, true, true, true, true);
            return ((ICalcEvaluator)this._worksheet).Expression2Formula(expression, 0, 0);
        }
    }
}