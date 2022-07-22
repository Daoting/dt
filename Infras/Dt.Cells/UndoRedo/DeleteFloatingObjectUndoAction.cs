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
using System.Collections.Generic;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteFloatingObjectUndoAction : ActionBase, IUndo
    {
        FloatingObjectExtent _deleteExtent;
        SpreadChart[] _savedCharts;
        FloatingObject[] _savedObjects;
        Picture[] _savedPictures;
        Worksheet _worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="extent">The extent.</param>
        public DeleteFloatingObjectUndoAction(Worksheet worksheet, FloatingObjectExtent extent)
        {
            _worksheet = worksheet;
            _deleteExtent = extent;
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
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                Excel excel = parameter as Excel;
                try
                {
                    excel.SuspendFloatingObjectsInvalidate();
                    SaveState();
                    for (int i = 0; i < _deleteExtent.Names.Length; i++)
                    {
                        string name = _deleteExtent.Names[i];
                        SpreadChart chart = _worksheet.FindChart(name);
                        if (chart != null)
                        {
                            _worksheet.RemoveChart(name);
                            chart.IsSelected = false;
                        }
                        else
                        {
                            Picture picture = _worksheet.FindPicture(name);
                            if (picture != null)
                            {
                                _worksheet.RemovePicture(name);
                                picture.IsSelected = false;
                            }
                            else
                            {
                                FloatingObject obj2 = _worksheet.FindFloatingObject(name);
                                if (obj2 != null)
                                {
                                    _worksheet.RemoveFloatingObject(name);
                                    obj2.IsSelected = false;
                                }
                            }
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
            foreach (string str in _deleteExtent.Names)
            {
                SpreadChart chart = _worksheet.FindChart(str);
                if (chart != null)
                {
                    list.Add(chart);
                }
                else
                {
                    Picture picture = _worksheet.FindPicture(str);
                    if (picture != null)
                    {
                        list3.Add(picture);
                    }
                    else
                    {
                        FloatingObject obj2 = _worksheet.FindFloatingObject(str);
                        if (obj2 != null)
                        {
                            list2.Add(obj2);
                        }
                    }
                }
            }
            _savedCharts = list.ToArray();
            _savedPictures = list3.ToArray();
            _savedObjects = list2.ToArray();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.deleteFloatingObj;
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
            Excel excel = parameter as Excel;
            try
            {
                excel.SuspendFloatingObjectsInvalidate();
                if ((_savedCharts != null) && (_savedCharts.Length > 0))
                {
                    foreach (SpreadChart chart in _savedCharts)
                    {
                        chart.IsSelected = true;
                    }
                    _worksheet.Charts.AddRange(_savedCharts);
                }
                if ((_savedPictures != null) && (_savedPictures.Length > 0))
                {
                    foreach (Picture picture in _savedPictures)
                    {
                        picture.IsSelected = true;
                    }
                    _worksheet.Pictures.AddRange(_savedPictures);
                }
                if ((_savedObjects != null) && (_savedObjects.Length > 0))
                {
                    foreach (FloatingObject obj2 in _savedObjects)
                    {
                        obj2.IsSelected = true;
                    }
                    _worksheet.FloatingObjects.AddRange(_savedObjects);
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

