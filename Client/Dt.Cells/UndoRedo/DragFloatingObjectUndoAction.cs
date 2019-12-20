#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
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
        private MoveFloatingObjectExtent _movingExtent;
        private SpreadChart[] _savedCharts;
        private FloatingObject[] _savedObjects;
        private Picture[] _savedPictures;
        private Worksheet _worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="extent">The extent.</param>
        public DragFloatingObjectUndoAction(Worksheet worksheet, MoveFloatingObjectExtent extent)
        {
            this._worksheet = worksheet;
            this._movingExtent = extent;
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
            if (this.CanExecute(parameter))
            {
                SheetView view = parameter as SheetView;
                try
                {
                    view.SuspendFloatingObjectsInvalidate();
                    this.SaveState();
                    List<SpreadChart> list = new List<SpreadChart>();
                    List<Picture> list2 = new List<Picture>();
                    List<FloatingObject> list3 = new List<FloatingObject>();
                    foreach (string str in this._movingExtent.Names)
                    {
                        SpreadChart chart = this._worksheet.FindChart(str);
                        Picture picture = this._worksheet.FindPicture(str);
                        FloatingObject obj2 = this._worksheet.FindFloatingObject(str);
                        FloatingObject pastedObject = null;
                        if (chart != null)
                        {
                            SpreadChart item = chart.Clone() as SpreadChart;
                            pastedObject = item;
                            item.Location = new Windows.Foundation.Point(chart.Location.X + this._movingExtent.OffsetX, chart.Location.Y + this._movingExtent.OffsetY);
                            item.Size = chart.Size;
                            item.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateChartName(this._worksheet);
                            chart.IsSelected = false;
                            item.IsSelected = true;
                            list.Add(item);
                            view.Worksheet.Charts.Add(item);
                        }
                        if (picture != null)
                        {
                            Picture picture2 = picture.Clone() as Picture;
                            pastedObject = picture2;
                            picture2.Location = new Windows.Foundation.Point(picture.Location.X + this._movingExtent.OffsetX, picture.Location.Y + this._movingExtent.OffsetY);
                            picture2.Size = picture.Size;
                            picture2.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GeneratePictureName(this._worksheet);
                            picture.IsSelected = false;
                            picture2.IsSelected = true;
                            list2.Add(picture2);
                            view.Worksheet.Pictures.Add(picture2);
                        }
                        if (obj2 != null)
                        {
                            FloatingObject obj4 = obj2.Clone() as FloatingObject;
                            pastedObject = obj4;
                            obj4.Location = new Windows.Foundation.Point(obj2.Location.X + this._movingExtent.OffsetX, obj2.Location.Y + this._movingExtent.OffsetY);
                            obj4.Size = obj2.Size;
                            obj4.Name = Dt.Cells.UndoRedo.GenerateNameHelper.GenerateFloatingObjectName(this._worksheet);
                            obj2.IsSelected = false;
                            obj4.IsSelected = true;
                            list3.Add(obj4);
                            view.Worksheet.FloatingObjects.Add(obj4);
                        }
                        if (pastedObject != null)
                        {
                            view.RaiseFloatingObjectPasted(this._worksheet, pastedObject);
                        }
                    }
                    if (list.Count > 0)
                    {
                        this._savedCharts = list.ToArray();
                    }
                    if (list2.Count > 0)
                    {
                        this._savedPictures = list2.ToArray();
                    }
                    if (list3.Count > 0)
                    {
                        this._savedObjects = list3.ToArray();
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
            SheetView view = parameter as SheetView;
            try
            {
                view.SuspendFloatingObjectsInvalidate();
                this.SaveState();
                foreach (string str in this._movingExtent.Names)
                {
                    this._worksheet.FindChart(str);
                    if ((this._savedCharts != null) && (this._savedCharts.Length > 0))
                    {
                        foreach (SpreadChart chart in this._savedCharts)
                        {
                            this._worksheet.Charts.Remove(chart);
                        }
                    }
                    if ((this._savedPictures != null) && (this._savedPictures.Length > 0))
                    {
                        foreach (Picture picture in this._savedPictures)
                        {
                            this._worksheet.Pictures.Remove(picture);
                        }
                    }
                    if ((this._savedObjects != null) && (this._savedObjects.Length > 0))
                    {
                        foreach (FloatingObject obj2 in this._savedObjects)
                        {
                            this._worksheet.FloatingObjects.Remove(obj2);
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

