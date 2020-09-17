#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a zoom undo action on the sheet.
    /// </summary>
    public class ZoomUndoAction : ActionBase, IUndo
    {
        float prevZoomFactor;
        Worksheet worksheet;
        float zoomFactor;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ZoomUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The zoomed worksheet.</param>
        /// <param name="newZoomFactor">The new zoom factor on the worksheet.</param>
        public ZoomUndoAction(Worksheet sheet, float newZoomFactor)
        {
            worksheet = sheet;
            if (newZoomFactor < 0.1f)
            {
                newZoomFactor = 0.1f;
            }
            else if (newZoomFactor > 4f)
            {
                newZoomFactor = 4f;
            }
            zoomFactor = newZoomFactor;
            prevZoomFactor = -1f;
        }

        /// <summary>
        /// Defines the method that determines whether the action can be executed in its current state.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            Excel excel = parameter as Excel;
            return (((excel != null) && excel.CanUserZoom) && (excel.ZoomFactor != zoomFactor));
        }

        /// <summary>
        /// Executes the zoom action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        public override void Execute(object parameter)
        {
            SaveState();
            Excel sheetView = parameter as Excel;
            if (((sheetView == null) || !sheetView.CanUserZoom) || (sheetView.ZoomFactor == zoomFactor))
            {
                throw new ActionFailedException(this);
            }
            if (sheetView.IsEditing)
            {
                sheetView.StopCellEditing(false);
            }
            base.SuspendInvalidate(parameter);
            try
            {
                worksheet.ZoomFactor = zoomFactor;
            }
            finally
            {
                base.ResumeInvalidate(parameter);
            }
            RefreshUI(sheetView);
        }

        void RefreshUI(object sheetView)
        {
            var excel = sheetView as Excel;
            if (excel != null)
            {
                excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells);
                excel.RefreshRange(-1, -1, -1, -1, SheetArea.ColumnHeader);
                excel.RefreshRange(-1, -1, -1, -1, SheetArea.CornerHeader | SheetArea.RowHeader);
                excel.RefreshFloatingObjects();
                excel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            if (worksheet != null)
            {
                prevZoomFactor = worksheet.ZoomFactor;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionZoom;
        }

        /// <summary>
        /// Undoes the zoom action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if undoing the action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            if (worksheet == null)
            {
                return false;
            }
            base.SuspendInvalidate(parameter);
            try
            {
                worksheet.ZoomFactor = prevZoomFactor;
            }
            finally
            {
                base.ResumeInvalidate(parameter);
            }
            Excel sheetView = parameter as Excel;
            if (sheetView != null)
            {
                RefreshUI(sheetView);
            }
            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  ((worksheet != null) && (prevZoomFactor > 0f)); }
        }
    }
}

