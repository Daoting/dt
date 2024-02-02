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
using Windows.Foundation;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    internal class ResizeFloatingObjectUndoAction : ActionBase, IUndo
    {
        ResizeFloatingObjectExtent _resizingExtent;
        Rect[] _savedRects;
        Worksheet _worksheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.MoveFloatingObjectUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="extent">The extent.</param>
        public ResizeFloatingObjectUndoAction(Worksheet worksheet, ResizeFloatingObjectExtent extent)
        {
            _worksheet = worksheet;
            _resizingExtent = extent;
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
                    for (int i = 0; i < _resizingExtent.Names.Length; i++)
                    {
                        string name = _resizingExtent.Names[i];
                        FloatingObject obj2 = _worksheet.FindChart(name);
                        if (obj2 == null)
                        {
                            obj2 = _worksheet.FindPicture(name);
                        }
                        if (obj2 == null)
                        {
                            obj2 = _worksheet.FindFloatingObject(name);
                        }
                        if (obj2 != null)
                        {
                            Rect rect = _resizingExtent.ResizedRects[i];
                            obj2.Size = new Size(rect.Width, rect.Height);
                            obj2.Location = new Point(rect.X, rect.Y);
                        }
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
            List<Rect> list = new List<Rect>();
            foreach (string str in _resizingExtent.Names)
            {
                FloatingObject obj2 = _worksheet.FindChart(str);
                if (obj2 == null)
                {
                    obj2 = _worksheet.FindFloatingObject(str);
                }
                if (obj2 == null)
                {
                    obj2 = _worksheet.FindPicture(str);
                }
                if (obj2 != null)
                {
                    list.Add(new Rect(obj2.Location, obj2.Size));
                }
            }
            _savedRects = list.ToArray();
        }

        public override string ToString()
        {
            return ResourceStrings.resizeFloatingObj;
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
                for (int i = 0; i < _resizingExtent.Names.Length; i++)
                {
                    string name = _resizingExtent.Names[i];
                    FloatingObject obj2 = _worksheet.FindChart(name);
                    if (obj2 == null)
                    {
                        obj2 = _worksheet.FindPicture(name);
                    }
                    if (obj2 == null)
                    {
                        obj2 = _worksheet.FindFloatingObject(name);
                    }
                    if (obj2 != null)
                    {
                        Rect rect = _savedRects[i];
                        obj2.Location = new Point(rect.X, rect.Y);
                        obj2.Size = new Size(rect.Width, rect.Height);
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

