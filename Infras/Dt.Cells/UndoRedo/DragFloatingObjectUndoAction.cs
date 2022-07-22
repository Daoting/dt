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
using Windows.Foundation;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class DragFloatingObjectUndoAction : ActionBase, IUndo
    {
        MoveFloatingObjectExtent _movingExtent;
        SpreadChart[] _savedCharts;
        FloatingObject[] _savedObjects;
        Picture[] _savedPictures;
        Worksheet _worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="extent">The extent.</param>
        public DragFloatingObjectUndoAction(Worksheet worksheet, MoveFloatingObjectExtent extent)
        {
            _worksheet = worksheet;
            _movingExtent = extent;
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
                Excel view = parameter as Excel;
                try
                {
                    view.SuspendFloatingObjectsInvalidate();
                    SaveState();
                    List<SpreadChart> list = new List<SpreadChart>();
                    List<Picture> list2 = new List<Picture>();
                    List<FloatingObject> list3 = new List<FloatingObject>();
                    foreach (string str in _movingExtent.Names)
                    {
                        SpreadChart chart = _worksheet.FindChart(str);
                        Picture picture = _worksheet.FindPicture(str);
                        FloatingObject obj2 = _worksheet.FindFloatingObject(str);
                        FloatingObject pastedObject = null;
                        if (chart != null)
                        {
                            SpreadChart item = chart.Clone() as SpreadChart;
                            pastedObject = item;
                            item.Location = new Point(chart.Location.X + _movingExtent.OffsetX, chart.Location.Y + _movingExtent.OffsetY);
                            item.Size = chart.Size;
                            item.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateChartName(_worksheet);
                            chart.IsSelected = false;
                            item.IsSelected = true;
                            list.Add(item);
                            view.ActiveSheet.Charts.Add(item);
                        }
                        if (picture != null)
                        {
                            Picture picture2 = picture.Clone() as Picture;
                            pastedObject = picture2;
                            picture2.Location = new Point(picture.Location.X + _movingExtent.OffsetX, picture.Location.Y + _movingExtent.OffsetY);
                            picture2.Size = picture.Size;
                            picture2.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GeneratePictureName(_worksheet);
                            picture.IsSelected = false;
                            picture2.IsSelected = true;
                            list2.Add(picture2);
                            view.ActiveSheet.Pictures.Add(picture2);
                        }
                        if (obj2 != null)
                        {
                            FloatingObject obj4 = obj2.Clone() as FloatingObject;
                            pastedObject = obj4;
                            obj4.Location = new Point(obj2.Location.X + _movingExtent.OffsetX, obj2.Location.Y + _movingExtent.OffsetY);
                            obj4.Size = obj2.Size;
                            obj4.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateFloatingObjectName(_worksheet);
                            obj2.IsSelected = false;
                            obj4.IsSelected = true;
                            list3.Add(obj4);
                            view.ActiveSheet.FloatingObjects.Add(obj4);
                        }
                        if (pastedObject != null)
                        {
                            view.RaiseFloatingObjectPasted(_worksheet, pastedObject);
                        }
                    }
                    if (list.Count > 0)
                    {
                        _savedCharts = list.ToArray();
                    }
                    if (list2.Count > 0)
                    {
                        _savedPictures = list2.ToArray();
                    }
                    if (list3.Count > 0)
                    {
                        _savedObjects = list3.ToArray();
                    }
                }
                finally
                {
                    view.ResumeFloatingObjectsInvalidate();
                }
                view.InvalidateFloatingObjectLayout();
            }
        }

        /// <summary>
        /// Saves the state for undoing the command or operation.
        /// </summary>
        public void SaveState()
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.dragFloatingObj;
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
            Excel view = parameter as Excel;
            try
            {
                view.SuspendFloatingObjectsInvalidate();
                SaveState();
                foreach (string str in _movingExtent.Names)
                {
                    _worksheet.FindChart(str);
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
                }
            }
            finally
            {
                view.ResumeFloatingObjectsInvalidate();
            }
            view.InvalidateFloatingObjectLayout();
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

