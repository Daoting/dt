#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents an individual <see cref="T:GcSpreadSheet" /> base cell.
    /// </summary>
    public abstract partial class CellItemBase : Control
    {
        Cell _bindingCell;
        CellLayout _cellLayout;
        int _column = -1;
        protected bool _isInvalidating;
        CellOverflowLayout _overflowLayout;

        internal abstract void ApplyState();

        internal abstract void CleanUpBeforeDiscard();

        internal abstract bool TryUpdateVisualTree();

        internal virtual bool HasEditingElement()
        {
            return false;
        }

        internal virtual void SetEditingElement(FrameworkElement p_editor)
        {
        }

        internal virtual FrameworkElement GetEditingElement()
        {
            return null;
        }

        internal virtual void HideForEditing()
        {
        }

        internal virtual void UnHideForEditing()
        {
        }

        internal virtual void RemoveInvalidDataPresenter()
        {
        }

        internal Cell GetBindingCell()
        {
            if ((OwningRow == null) || (OwningRow.OwningPresenter == null))
            {
                return null;
            }
            int row = Row;
            int column = Column;
            if (_cellLayout != null)
            {
                row = _cellLayout.Row;
                column = _cellLayout.Column;
            }
            return OwningRow.OwningPresenter.CellCache.GetCachedCell(row, column);
        }




#if ANDROID
        new
#endif
        internal virtual void Invalidate()
        {
            _isInvalidating = true;
            InvalidateMeasure();
        }

        protected void ApplyStyle(TextBlock p_tb)
        {

        }

        internal void UpdateBindingCell()
        {
            Cell bindingCell = GetBindingCell();
            _bindingCell = bindingCell;
        }

        void WalkTree(DependencyObject p)
        {
            if (p is Border)
            {
                ((Border)p).BorderThickness = new Thickness(0.0);
            }
            int childrenCount = VisualTreeHelper.GetChildrenCount(p);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(p, i);
                WalkTree(child);
            }
        }

        internal Cell BindingCell
        {
            get
            {
                if (_bindingCell == null)
                {
                    _bindingCell = GetBindingCell();
                }
                return _bindingCell;
            }
        }

        internal CellLayout CellLayout
        {
            get { return _cellLayout; }
            set
            {
                if (!object.Equals(_cellLayout, value))
                {
                    _cellLayout = value;
                    InvalidateMeasure();
                }
            }
        }

        internal CellOverflowLayout CellOverflowLayout
        {
            get { return _overflowLayout; }
            set
            {
                if (!object.Equals(_overflowLayout, value))
                {
                    _overflowLayout = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates the column index.
        /// </summary>
        public int Column
        {
            get { return _column; }
            internal set
            {
                if (value != _column)
                {
                    _column = value;
                    base.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates that the cell's viewport is active. 
        /// </summary>
        protected virtual bool IsActive
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is the active cell.
        /// </summary>
        protected virtual bool IsCurrent
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value that indicates that the mouse is over the cell.
        /// </summary>
        protected virtual bool IsMouseOver
        {
            get { return false; }
        }

        internal virtual bool IsRecylable
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value that indicates that the cell is selected.
        /// </summary>
        protected virtual bool IsSelected
        {
            get { return false; }
        }

        internal RowItem OwningRow { get; set; }

        /// <summary>
        /// Gets a value that indicates the row index.
        /// </summary>
        public int Row
        {
            get { return OwningRow.Row; }
        }

        internal SheetView SheetView
        {
            get
            {
                if ((OwningRow != null) && (OwningRow.OwningPresenter != null))
                {
                    return OwningRow.OwningPresenter.Sheet;
                }
                return null;
            }
        }

        internal bool ShowContent { get; set; } = true;

        internal double ZoomFactor
        {
            get
            {
                SheetView sheetView = SheetView;
                if (sheetView != null)
                    return (double)sheetView.ZoomFactor;
                return 1.0;
            }
        }

        internal Size GetPreferredEditorSize(Size maxSize, Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            if (((OwningRow == null) ? null : OwningRow.OwningPresenter) == null)
            {
                return new Size();
            }
            //if (!OwningRow.OwningPresenter.Sheet.CanEditOverflow || (_cellType == null))
            //{
            //    return new Size(cellContentSize.Width, cellContentSize.Height);
            //}
            double num = Math.Min(maxSize.Width, cellContentSize.Width);
            Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(maxSize, true, null), ZoomFactor);
            size.Width += 2.0;
            string text = "T";
            Size size2 = CalcStringSize(new Size(2147483647.0, 2147483647.0), false, text);
            size.Width += size2.Width;
            if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            {
                size.Width += indent;
            }
            return new Size(Math.Max(num, size.Width), Math.Max(cellContentSize.Height, size.Height));
        }

        internal bool JudgeWordWrap(Size maxSize, Size cellContentSize, HorizontalAlignment alignment, float indent)
        {
            return false;
            //if (((((OwningRow == null) ? null : OwningRow.OwningPresenter) == null) || !OwningRow.OwningPresenter.Sheet.CanEditOverflow) || (_cellType == null))
            //{
            //    return false;
            //}
            //double num = Math.Min(maxSize.Width, cellContentSize.Width);
            //Size size = MeasureHelper.ConvertTextSizeToExcelCellSize(CalcStringSize(new Size(2147483647.0, 2147483647.0), false, null), ZoomFactor);
            //size.Width += 2.0;
            //if (((alignment == HorizontalAlignment.Left) || (alignment == HorizontalAlignment.Right)) && (num < (size.Width + indent)))
            //{
            //    size.Width += indent;
            //}
            //return (maxSize.Width < size.Width);
        }

        Size CalcStringSize(Size maxSize, bool allowWrap, string text = null)
        {
            //if (_cellType.HasEditingElement())
            //{
            //    TextBox editingElement = _cellType.GetEditingElement() as TextBox;
            //    if ((editingElement != null) && !string.IsNullOrEmpty(editingElement.Text))
            //    {
            //        Cell bindingCell = BindingCell;
            //        if (bindingCell != null)
            //        {
            //            FontFamily actualFontFamily = bindingCell.ActualFontFamily;
            //            if (actualFontFamily == null)
            //            {
            //                actualFontFamily = editingElement.FontFamily;
            //            }
            //            object textFormattingMode = null;
            //            double fontSize = bindingCell.ActualFontSize * ZoomFactor;
            //            if (fontSize < 0.0)
            //            {
            //                fontSize = editingElement.FontSize;
            //            }
            //            return MeasureHelper.MeasureText((text == null) ? editingElement.Text : text, actualFontFamily, fontSize, bindingCell.ActualFontStretch, bindingCell.ActualFontStyle, bindingCell.ActualFontWeight, maxSize, allowWrap, textFormattingMode, SheetView.UseLayoutRounding, ZoomFactor);
            //        }
            //    }
            //}
            return new Size();
        }

        internal static bool ApplyValueToCell(
            SheetView sheetView,
            Cell bindingCell,
            bool allowFormula,
            object editorValue,
            Type valueType,
            out bool isFormulaApplied,
            out string appliedFormula)
        {
            isFormulaApplied = false;
            appliedFormula = null;
            if (bindingCell == null)
                return true;

            if (ContainsArrayFormula(bindingCell.Worksheet.FindFormulas(bindingCell.Row.Index, bindingCell.Column.Index, 1, 1)))
            {
                return false;
            }

            string str = editorValue as string;
            if (allowFormula
                && str != null
                && str.StartsWith("=")
                && str.Length > 1)
            {
                appliedFormula = str.TrimStart(new char[] { '=' });
                try
                {
                    isFormulaApplied = true;
                    bindingCell.Formula = appliedFormula;
                }
                catch
                {
                    return false;
                }
                return true;
            }

            if (!string.IsNullOrEmpty(bindingCell.Formula))
            {
                bindingCell.Formula = null;
            }
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    if (str.StartsWith("'="))
                    {
                        str = str.Substring(1);
                    }
                    IFormatter actualFormatter = bindingCell.ActualFormatter;
                    if ((actualFormatter != null) && !(actualFormatter is AutoFormatter))
                    {
                        object obj2 = actualFormatter.Parse(str);
                        object obj3 = null;
                        if (obj2 == null)
                        {
                            obj3 = str;
                        }
                        else
                        {
                            obj3 = obj2;
                        }
                        obj3 = sheetView.RaiseCellValueApplying(bindingCell.Row.Index, bindingCell.Column.Index, obj3);
                        bindingCell.Value = obj3;
                    }
                    else
                    {
                        UpdateFormatter(str, bindingCell, valueType);
                    }
                    goto Label_0139;
                }
                catch (InvalidCastException)
                {
                    bindingCell.Value = editorValue as string;
                    goto Label_0139;
                }
            }
            bindingCell.Value = null;
        Label_0139:
            return true;
        }

        static bool ContainsArrayFormula(object[,] formulas)
        {
            if (formulas != null)
            {
                for (int i = 0; i < formulas.GetLength(0); i++)
                {
                    CellRange range = formulas[i, 0] as CellRange;
                    if ((range != null) && ((range.RowCount > 1) || (range.ColumnCount > 1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void UpdateFormatter(string text, Cell cell, Type cacheValueType)
        {
            object obj2 = null;
            GeneralFormatter preferredDisplayFormatter = new GeneralFormatter().GetPreferredDisplayFormatter(text, out obj2) as GeneralFormatter;
            object obj3 = obj2;
            if (((cell.ActualFormatter != null) && (obj2 != null)) && ((cell.ActualFormatter is AutoFormatter) && !preferredDisplayFormatter.FormatString.Equals("General")))
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            else if (cell.ActualFormatter == null)
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            cell.Value = obj3;
        }
    }
}

