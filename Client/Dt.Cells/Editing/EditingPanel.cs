#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.CellTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{

    internal partial class EditingPanel : Panel
    {
        GcViewport _parentViewport;
        CellPresenterBase _editingCell;
        FrameworkElement _editor1;
        FrameworkElement _editor2;

        public event EventHandler EditingChanged;

        public EditingPanel(GcViewport parent)
        {
            _parentViewport = parent;
            Background = new SolidColorBrush(Colors.White);
            IsHitTestVisible = false;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_editor1 != null)
            {
                _editor1.Measure(availableSize);
            }
            if (_editor2 != null)
            {
                _editor2.Measure(availableSize);
            }
            return _parentViewport.GetViewportSize(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if ((_editingCell != null) && (_parentViewport != null))
            {
                Rect rect = _parentViewport.CalcEditorBounds(EditingRowIndex, EditingColumnIndex, finalSize);
                double left = rect.Left;
                HorizontalAlignment alignment = HorizontalAlignment.Left;
                if (_editingCell.BindingCell != null)
                {
                    float num2 = _editingCell.BindingCell.ActualTextIndent * _parentViewport.Sheet.ZoomFactor;
                    alignment = _editingCell.BindingCell.ToHorizontalAlignment();
                    if (_editingCell.BindingCell.ActualTextIndent > 0)
                    {
                        switch (alignment)
                        {
                            case HorizontalAlignment.Left:
                                left += num2;
                                break;

                            case HorizontalAlignment.Right:
                                left -= num2;
                                break;
                        }
                    }
                }
                Rect rect2 = new Rect(left, rect.Top - 1.0, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height + 1.0);
                if (_editingCell.BindingCell.ActualVerticalAlignment == CellVerticalAlignment.Top)
                {
                    rect2 = new Rect(left, rect.Top - 2.0, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height + 2.0);
                }
                else if (_editingCell.BindingCell.ActualVerticalAlignment == CellVerticalAlignment.Bottom)
                {
                    rect2 = new Rect(left, rect.Top, (rect.Width >= 2.0) ? (rect.Width - 2.0) : 0.0, rect.Height);
                }
                double x = rect2.X;
                double y = rect2.Y;
                IsEditorVisible = (rect2.Width > 0.0) && (rect2.Height > 0.0);
                RectangleGeometry geometry = new RectangleGeometry();
                geometry.Rect = new Rect(rect.X, rect.Y, rect.Width, rect.Height);
                Clip = geometry;

                if (Editor != null)
                {
                    if ((rect2.Width > 1.0) && (rect2.Height > 1.0))
                    {
                        if (_editor1 != null)
                        {
                            _editor1.Width = rect2.Width;
                            _editor1.Height = rect2.Height;
                        }
                        if (_editor2 != null)
                        {
                            _editor2.Width = rect2.Width;
                            _editor2.Height = rect2.Height;
                        }
                    }
                    if ((_editor1 != null) && (_editor1.Visibility == Visibility.Visible))
                    {
                        _editor1.Arrange(new Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height));
                    }
                    if ((_editor2 != null) && (_editor2.Visibility == Visibility.Visible))
                    {
                        _editor2.Arrange(new Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height));
                    }
                }
            }
            return base.ArrangeOverride(finalSize);
        }

        void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            if ((_parentViewport != null) && _parentViewport.IsEditing())
            {
                EditorDirty = true;
                if (EditingChanged != null)
                {
                    EditingChanged(this, EventArgs.Empty);
                }
            }
        }

        internal FrameworkElement GetAvaiableEditor()
        {
            object focusedElement = GetFocusedElement();
            if (object.ReferenceEquals(Editor, _editor1))
            {
                if (_editor2 != null)
                {
                    _editor2.Visibility = Visibility.Visible;
                    if ((focusedElement == _editor1) && (_editor2 != null))
                    {
                        //(_editor2 as TextBox).Focus(FocusState.Programmatic);
                    }
                    if (_editor2 is EditingElement)
                    {
                        (_editor2 as EditingElement).Status = EditorStatus.Ready;
                    }
                }

                if (_editor1 != null)
                {
                    //if (focusedElement == _editor1
                    //    && !ReferenceEquals(GetFocusedElement(), _editor2)
                    //    && _parentViewport != null
                    //    && _parentViewport.Sheet != null
                    //    && _parentViewport.Sheet._host != null)
                    //{
                    //    _parentViewport.Sheet._host.Focus(FocusState.Programmatic);
                    //}
                    _editor1.Visibility = Visibility.Collapsed;
                    if (_editor1 is TextBox)
                    {
                        (_editor1 as TextBox).Text = string.Empty;
                    }
                    if (_editor1 is EditingElement)
                    {
                        (_editor1 as EditingElement).Status = EditorStatus.Ready;
                    }
                }
                return _editor2;
            }
            if (!object.ReferenceEquals(Editor, _editor2))
            {
                return Editor;
            }
            if (_editor1 != null)
            {
                _editor1.Visibility = Visibility.Visible;
                if ((focusedElement == _editor2) && (_editor1 != null))
                {
                    //(_editor1 as TextBox).Focus(FocusState.Programmatic);
                }
                if (_editor1 is EditingElement)
                {
                    (_editor1 as EditingElement).Status = EditorStatus.Ready;
                }
            }
            if (_editor2 != null)
            {
                _editor2.Visibility = Visibility.Collapsed;
                if (_editor2 is TextBox)
                {
                    (_editor2 as TextBox).Text = string.Empty;
                }
                if (_editor2 is EditingElement)
                {
                    (_editor2 as EditingElement).Status = EditorStatus.Ready;
                }
            }
            return _editor1;
        }

        object GetFocusedElement()
        {
            return FocusManager.GetFocusedElement();
        }

        public void InstallEditor(CellPresenterBase cell, bool startEditing = false)
        {
            if (cell == null)
                return;

            FrameworkElement editingElement = cell.GetEditingElement();
            int row = cell.Row;
            int column = cell.Column;
            if (cell.CellLayout != null)
            {
                row = cell.CellLayout.Row;
                column = cell.CellLayout.Column;
            }
            _editingCell = cell;
            EditorDirty = false;
            if (!ReferenceEquals(editingElement, _editor2) && (_editor1 == null))
            {
                _editor1 = editingElement;
            }
            else if (!ReferenceEquals(editingElement, _editor1) && (_editor2 == null))
            {
                _editor2 = editingElement;
            }

            Editor = editingElement;
            Editor.Visibility = Visibility.Visible;
            if (Editor != null)
            {
                if (!Children.Contains(Editor))
                {
                    Children.Add(Editor);
                }
                EditingColumnIndex = column;
                EditingRowIndex = row;
                if (cell.BindingCell.ActualBackground != null)
                {
                    Background = cell.BindingCell.ActualBackground;
                }
                else
                {
                    Background = new SolidColorBrush(Colors.Transparent);
                }
                TextBox editor = Editor as TextBox;
                editor.IsHitTestVisible = false;
                // hdt
                Worksheet ws = _parentViewport.Sheet.Worksheet;
                if (ws != null && ws.LockCell)
                    editor.IsEnabled = false;
                else if (!editor.IsEnabled)
                    editor.IsEnabled = true;

                if (IsHitTestVisible)
                    IsHitTestVisible = false;
                UpateScrollViewSize(editor);
                editor.SelectAll();
                if (editor != null)
                {
                    editor.TextChanged -= EditorTextChanged;
                    editor.TextChanged += EditorTextChanged;
                }
            }
        }

        public void ResumeEditor()
        {
            if (_editingCell != null)
            {
                CellPresenterBase objA = _parentViewport.GetViewportCell(EditingRowIndex, EditingColumnIndex, true);
                if (objA != null)
                {
                    if (!Equals(objA, _editingCell))
                    {
                        Control editor = Editor as Control;
                        if (editor != null)
                        {
                            //editor.Focus(FocusState.Programmatic);
                        }
                        _editingCell = objA;
                    }
                    objA.HideForEditing();
                }
            }
        }

        public void SetBackground(Brush brush)
        {
            Background = brush;
        }

        internal void SetEditorStatus(EditorStatus status)
        {
            if (Editor is EditingElement)
            {
                (Editor as EditingElement).Status = status;
            }
        }

        internal void UpadateEditor()
        {
            bool isWrap = false;
            if ((_editingCell != null) && (_parentViewport != null))
            {
                Size viewportSize = _parentViewport.GetViewportSize();
                CellPresenterBase base2 = _parentViewport.GetViewportCell(EditingRowIndex, EditingColumnIndex, true);
                if ((_editingCell != null) && (_parentViewport._editorPanel != null))
                {
                    Rect rect = _parentViewport.GetCellBounds(EditingRowIndex, EditingColumnIndex, false);
                    Size cellContentSize = new Size(rect.Width, rect.Height);
                    double height = viewportSize.Height - (rect.Top - _parentViewport.Location.Y);
                    if ((rect.Width != 0.0) && (rect.Height != 0.0))
                    {
                        Cell cachedCell = _parentViewport.CellCache.GetCachedCell(EditingRowIndex, EditingColumnIndex);
                        HorizontalAlignment alignment = cachedCell.ToHorizontalAlignment();
                        switch (alignment)
                        {
                            case HorizontalAlignment.Left:
                                {
                                    float indent = cachedCell.ActualTextIndent * _parentViewport.Sheet.ZoomFactor;
                                    double num3 = (viewportSize.Width - rect.Left) + _parentViewport.Location.X;
                                    num3 = Math.Max(Math.Min(num3, viewportSize.Width), 0.0);
                                    Size maxSize = new Size(num3, height);
                                    isWrap = base2.JudgeWordWrap(maxSize, cellContentSize, alignment, indent);
                                    goto Label_02B5;
                                }
                            case HorizontalAlignment.Right:
                                {
                                    float num4 = cachedCell.ActualTextIndent * _parentViewport.Sheet.ZoomFactor;
                                    double num5 = rect.Right - _parentViewport.Location.X;
                                    num5 = Math.Max(Math.Min(num5, viewportSize.Width), 0.0);
                                    Size size4 = new Size(num5, height);
                                    isWrap = _editingCell.JudgeWordWrap(size4, cellContentSize, alignment, num4);
                                    goto Label_02B5;
                                }
                        }
                        if (alignment == HorizontalAlignment.Center)
                        {
                            double num6 = (rect.Left - _parentViewport.Location.X) + (rect.Width / 2.0);
                            if (num6 < 0.0)
                            {
                                num6 = 0.0;
                            }
                            double num7 = viewportSize.Width - num6;
                            if (num7 < 0.0)
                            {
                                num7 = 0.0;
                            }
                            double width = 2.0 * Math.Min(num6, num7);
                            Size size5 = new Size(width, height);
                            isWrap = _editingCell.JudgeWordWrap(size5, cellContentSize, alignment, 0f);
                        }
                    }
                }
            }
        Label_02B5:
            if (Editor != null)
            {
                if (_editor1 != null)
                {
                    UpdateEditingElement(_editor1, isWrap);
                }
                if (_editor2 != null)
                {
                    UpdateEditingElement(_editor2, isWrap);
                }
            }
        }

        void UpateScrollViewSize(TextBox tb)
        {
            string text = tb.Text;
            tb.Text = "Text";
            tb.Text = text;
        }

        public void Update(CellPresenterBase cell)
        {
            int row = cell.Row;
            int column = cell.Column;
            if (cell.CellLayout != null)
            {
                row = cell.CellLayout.Row;
                column = cell.CellLayout.Column;
            }
            _editingCell = cell;
            EditorDirty = false;
            EditingColumnIndex = column;
            EditingRowIndex = row;
            if (cell.BindingCell.ActualBackground != null)
            {
                Background = cell.BindingCell.ActualBackground;
            }
            else
            {
                Background = new SolidColorBrush(Colors.Transparent);
            }
        }

        void UpdateEditingElement(FrameworkElement editElement, bool isWrap)
        {
            EditingElement element = editElement as EditingElement;
            if (element != null)
            {
                element.TextWrapping = isWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        public int EditingColumnIndex { get; private set; }

        public int EditingRowIndex { get; private set; }

        public FrameworkElement Editor { get; private set; }

        public bool EditorDirty { get; set; }

        internal EditorStatus EditorStatus
        {
            get
            {
                if ((Editor != null) && (Editor is EditingElement))
                {
                    return (Editor as EditingElement).Status;
                }
                return EditorStatus.Ready;
            }
        }

        public bool IsEditorVisible { get; set; }
    }

}

