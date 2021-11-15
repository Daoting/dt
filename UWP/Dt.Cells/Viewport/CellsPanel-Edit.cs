#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CellsPanel : Panel
    {
        public bool IsEditing()
        {
            return _editorLayer.Editor.Opacity == 1.0;
        }

        public void PrepareCellEditing(int row, int column)
        {
#if UWP
            CellItem editingCell = GetViewportCell(row, column, true);
            if (editingCell != null)
                _editorLayer.PrepareEditor(editingCell);
#endif
        }

        public bool StartCellEditing(int row, int column, bool selectAll, string defaultText, EditorStatus status)
        {
            return StartTextInput(row, column, status, true, selectAll, defaultText);
        }

        public bool StartTextInput(int row, int column, EditorStatus status)
        {
            return StartTextInput(row, column, status, false, false, null);
        }

        public bool StartTextInput(int row, int column, EditorStatus status, bool canModifyTextBox, bool selectAll = false, string defaultText = null)
        {
            if (IsEditing())
                return true;

            CellItem cell = GetViewportCell(row, column, true);
            if (cell == null)
                return false;

            ShowSheetCell(row, column);
            if (Excel.RaiseEditStarting(row, column))
                return false;

            _editorLayer.ShowEditor(cell, status);
            _editorLayer.EditingChanged += new EventHandler(_editorPanel_EdtingChanged);

            if (canModifyTextBox)
            {
                var editor = _editorLayer.Editor;
                if (defaultText != null)
                    editor.Text = defaultText;

                if (selectAll)
                {
                    editor.SelectAll();
                }
                else
                {
                    editor.SelectionStart = editor.Text.Length;
                }
            }
            return true;
        }

        public void StopCellEditing()
        {
            _editorLayer.EditingChanged -= new EventHandler(_editorPanel_EdtingChanged);
            _editorBounds = new Rect();
            _editorLayer.HideEditor();
            Excel.RaiseEditEnd(_activeRow, _activeCol);
        }

        void _editorPanel_EdtingChanged(object sender, EventArgs e)
        {
            Excel.RaiseEditChange(_activeRow, _activeCol);
        }
    }
}

