#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal class ActualValue : IActualValue
    {
        int column;
        int row;
        Worksheet sheet;
        bool sheetValue;
        object value;

        public ActualValue(object value)
        {
            this.value = value;
            this.sheetValue = false;
        }

        public ActualValue(Worksheet sheet, int row, int column)
        {
            this.sheet = sheet;
            this.row = row;
            this.column = column;
            this.sheetValue = true;
        }

        public Color? GetBackgroundColor()
        {
            if ((((this.sheet != null) && (this.row > -1)) && ((this.row < this.sheet.RowCount) && (this.column > -1))) && (this.column < this.sheet.ColumnCount))
            {
                StyleInfo style = this.sheet.GetActualStyleInfo(this.row, this.column, SheetArea.Cells);
                if (style != null)
                {
                    if (!string.IsNullOrEmpty(style.BackgroundThemeColor))
                    {
                        return new Color?(((IThemeSupport) this.sheet).GetThemeColor(style.BackgroundThemeColor));
                    }
                    if (style.Background is SolidColorBrush)
                    {
                        return new Color?(((SolidColorBrush)style.Background).Color);
                    }
                }
            }
            return null;
        }

        public Color? GetDefaultBackgroundColor()
        {
            StyleInfo defaultStyle = this.sheet.DefaultStyle;
            if ((this.sheet != null) && (defaultStyle != null))
            {
                if ((this.sheet.Workbook != null) && defaultStyle.IsBackgroundThemeColorSet())
                {
                    return new Color?(this.sheet.Workbook.GetThemeColor(defaultStyle.BackgroundThemeColor));
                }
                if (defaultStyle.Background is SolidColorBrush)
                {
                    return new Color?(((SolidColorBrush) defaultStyle.Background).Color);
                }
            }
            return null;
        }

        public Color? GetDefaultForegroundColor()
        {
            StyleInfo defaultStyle = this.sheet.DefaultStyle;
            if ((this.sheet != null) && (defaultStyle != null))
            {
                if ((this.sheet.Workbook != null) && defaultStyle.IsForegroundThemeColorSet())
                {
                    return new Color?(this.sheet.Workbook.GetThemeColor(defaultStyle.ForegroundThemeColor));
                }
                if (defaultStyle.Foreground is SolidColorBrush)
                {
                    return new Color?(((SolidColorBrush) defaultStyle.Foreground).Color);
                }
            }
            return null;
        }

        public Color? GetForegroundColor()
        {
            if ((((this.sheet != null) && (this.row > -1)) && ((this.row < this.sheet.RowCount) && (this.column > -1))) && (this.column < this.sheet.ColumnCount))
            {
                StyleInfo style = this.sheet.GetActualStyleInfo(this.row, this.column, SheetArea.Cells);
                if (style != null)
                {
                    if (!string.IsNullOrEmpty(style.ForegroundThemeColor))
                    {
                        return new Color?(((IThemeSupport) this.sheet).GetThemeColor(style.ForegroundThemeColor));
                    }
                    if (style.Foreground is SolidColorBrush)
                    {
                        return new Color?(((SolidColorBrush)style.Foreground).Color);
                    }
                }
            }
            return null;
        }

        public string GetText()
        {
            if (!this.sheetValue)
            {
                return (string) (this.value as string);
            }
            if ((((this.sheet == null) || (this.row <= -1)) || ((this.row >= this.sheet.RowCount) || (this.column <= -1))) || (this.column >= this.sheet.ColumnCount))
            {
                return string.Empty;
            }
            object obj2 = this.sheet.GetValue(this.row, this.column);
            if (obj2 == null)
            {
                return string.Empty;
            }
            SheetArea cells = SheetArea.Cells;
            StorageBlock storage = this.sheet.GetStorage(cells);
            StyleInfo info = this.sheet.GetCompositeStyle(this.row, this.column, cells, storage);
            return this.sheet.Value2Text(obj2, info.Formatter);
        }

        public object GetValue()
        {
            if (!this.sheetValue)
            {
                return this.value;
            }
            if ((((this.sheet != null) && (this.row > -1)) && ((this.row < this.sheet.RowCount) && (this.column > -1))) && (this.column < this.sheet.ColumnCount))
            {
                return this.sheet.GetValue(this.row, this.column);
            }
            return null;
        }

        public object GetValue(NumberFormatType type)
        {
            object val = this.GetValue();
            if (type != NumberFormatType.DateTime)
            {
                return val;
            }
            DateTime? nullable = ConditionValueConverter.TryDateTime(val);
            if (!nullable.HasValue)
            {
                return null;
            }
            return nullable.Value;
        }

        public object GetValue(int row, int column)
        {
            if ((((this.sheet != null) && (row > -1)) && ((row < this.sheet.RowCount) && (column > -1))) && (column < this.sheet.ColumnCount))
            {
                return this.sheet.GetValue(row, column);
            }
            return null;
        }

        public int Column
        {
            get { return  this.column; }
            set { this.column = value; }
        }

        public int Row
        {
            get { return  this.row; }
            set { this.row = value; }
        }

        public Worksheet Sheet
        {
            get { return  this.sheet; }
            set { this.sheet = value; }
        }

        public object Value
        {
            get { return  this.value; }
        }
    }
}

