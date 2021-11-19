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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class ClipboardPasteFloatingObjectUndoAction : ActionBase, IUndo
    {
        SpreadChart[] _savedCharts;
        FloatingObject[] _savedObjects;
        Picture[] _savedPictures;
        FloatingObject[] _sourceFloatingObjects;
        Worksheet _worksheet;
        const double OFFSET = 15.0;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="sourceFloatingObjects">The floating objects.</param>
        public ClipboardPasteFloatingObjectUndoAction(Worksheet worksheet, FloatingObject[] sourceFloatingObjects)
        {
            _worksheet = worksheet;
            _sourceFloatingObjects = sourceFloatingObjects;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return !_worksheet.Protect;
        }

        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                var excel = parameter as Excel;
                try
                {
                    excel.SuspendFloatingObjectsInvalidate();
                    if ((_sourceFloatingObjects != null) && (_sourceFloatingObjects.Length > 0))
                    {
                        Point[] pointArray = new Point[_sourceFloatingObjects.Length];
                        if (excel.HasSelectedFloatingObject())
                        {
                            for (int j = 0; j < _sourceFloatingObjects.Length; j++)
                            {
                                pointArray[j] = new Point(_sourceFloatingObjects[j].Location.X + 15.0, _sourceFloatingObjects[j].Location.Y + 15.0);
                            }
                        }
                        else
                        {
                            double maxValue = double.MaxValue;
                            double num3 = double.MaxValue;
                            foreach (FloatingObject obj2 in _sourceFloatingObjects)
                            {
                                maxValue = Math.Min(obj2.Location.X, maxValue);
                                num3 = Math.Min(obj2.Location.Y, num3);
                            }
                            Point[] pointArray2 = new Point[_sourceFloatingObjects.Length];
                            for (int k = 0; k < _sourceFloatingObjects.Length; k++)
                            {
                                pointArray2[k] = new Point(_sourceFloatingObjects[k].Location.X - maxValue, _sourceFloatingObjects[k].Location.Y - num3);
                            }
                            double num5 = 0.0;
                            double num6 = 0.0;
                            for (int m = 0; m < excel.ActiveSheet.ActiveRowIndex; m++)
                            {
                                num6 += excel.ActiveSheet.GetActualRowHeight(m, SheetArea.Cells);
                            }
                            for (int n = 0; n < excel.ActiveSheet.ActiveColumnIndex; n++)
                            {
                                num5 += excel.ActiveSheet.GetActualColumnWidth(n, SheetArea.Cells);
                            }
                            for (int num9 = 0; num9 < _sourceFloatingObjects.Length; num9++)
                            {
                                pointArray[num9] = new Point(num5 + pointArray2[num9].X, num6 + pointArray2[num9].Y);
                            }
                        }
                        List<SpreadChart> list = new List<SpreadChart>();
                        List<FloatingObject> list2 = new List<FloatingObject>();
                        List<Picture> list3 = new List<Picture>();
                        List<FloatingObject> list4 = new List<FloatingObject>();
                        for (int i = 0; i < _sourceFloatingObjects.Length; i++)
                        {
                            FloatingObject item = _sourceFloatingObjects[i];
                            item.Location = pointArray[i];
                            item.IsSelected = true;
                            if (item is SpreadChart)
                            {
                                item.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateChartName(_worksheet);
                                _worksheet.Charts.Add(item as SpreadChart);
                                list.Add(item as SpreadChart);
                            }
                            else if (item is Picture)
                            {
                                item.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GeneratePictureName(_worksheet);
                                _worksheet.Pictures.Add(item as Picture);
                                list3.Add(item as Picture);
                            }
                            else
                            {
                                item.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateFloatingObjectName(_worksheet);
                                _worksheet.FloatingObjects.Add(item);
                                list2.Add(item);
                            }
                            excel.RaiseFloatingObjectPasted(_worksheet, item);
                            list4.Add(item);
                        }
                        if (list.Count > 0)
                        {
                            _savedCharts = list.ToArray();
                        }
                        if (list3.Count > 0)
                        {
                            _savedPictures = list3.ToArray();
                        }
                        if (list2.Count > 0)
                        {
                            _savedObjects = list2.ToArray();
                        }
                    }
                }
                finally
                {
                    excel.ResumeFloatingObjectsInvalidate();
                    ReadOnlyCollection<CellRange> selections = _worksheet.Selections;
                    if (selections.Count != 0)
                    {
                        foreach (CellRange range in selections)
                        {
                            excel.UpdateHeaderCellsState(range.Row, range.RowCount, range.Column, range.ColumnCount);
                        }
                    }
                }
                excel.InvalidateFloatingObjectLayout();
            }
        }

        /// <summary>
        /// Saves the state for undoing the command or operation.
        /// </summary>
        public void SaveState()
        {
            List<SpreadChart> list = new List<SpreadChart>();
            List<FloatingObject> list2 = new List<FloatingObject>();
            List<Picture> list3 = new List<Picture>();
            foreach (FloatingObject obj2 in _sourceFloatingObjects)
            {
                if (obj2 is SpreadChart)
                {
                    list.Add(obj2 as SpreadChart);
                }
                else if (obj2 is Picture)
                {
                    list3.Add(obj2 as Picture);
                }
                else
                {
                    list2.Add(obj2);
                }
            }
            _savedCharts = list.ToArray();
            _savedObjects = list2.ToArray();
            _savedPictures = list3.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.copyFloatingObj;
        }

        /// <summary>
        /// Undoes the command or operation.
        /// </summary>
        /// <param name="parameter">The parameter to undo the command or operation.</param>
        /// <returns>
        /// <c>true</c> if an undo operation on the command or operation succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            var excel = parameter as Excel;
            try
            {
                excel.SuspendFloatingObjectsInvalidate();
                if ((_savedCharts != null) && (_savedCharts.Length > 0))
                {
                    foreach (SpreadChart chart in _savedCharts)
                    {
                        _worksheet.Charts.Remove(chart);
                    }
                }
                if ((_savedPictures != null) && (_savedPictures.Length > 0))
                {
                    foreach (Picture picture in _savedPictures)
                    {
                        _worksheet.Pictures.Remove(picture);
                    }
                }
                if ((_savedObjects != null) && (_savedObjects.Length > 0))
                {
                    foreach (FloatingObject obj2 in _savedObjects)
                    {
                        _worksheet.FloatingObjects.Remove(obj2);
                    }
                }
                foreach (FloatingObject obj3 in _sourceFloatingObjects)
                {
                    obj3.IsSelected = true;
                }
            }
            finally
            {
                excel.ResumeFloatingObjectsInvalidate();
                ReadOnlyCollection<CellRange> selections = _worksheet.Selections;
                if (selections.Count != 0)
                {
                    foreach (CellRange range in selections)
                    {
                        excel.UpdateHeaderCellsState(range.Row, range.RowCount, range.Column, range.ColumnCount);
                    }
                }
            }
            excel.InvalidateFloatingObjectLayout();
            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the command or operation can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }
    }
}

