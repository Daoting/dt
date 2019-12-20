#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// The <see cref="T:GrapeCity.Windows.SpreadSheet.UI.UndoManager" /> class is used to perform the
    /// <see cref="T:GrapeCity.Windows.SpreadSheet.UI.UndoRedo.ActionBase" /> actions. The <see cref="T:GrapeCity.Windows.SpreadSheet.UI.UndoRedo.ActionBase" /> actions 
    /// can be undone and redone if the <see cref="P:GrapeCity.Windows.SpreadSheet.UI.UndoRedo.ActionBase.CanUndo" /> property is <c>true</c>.
    /// </summary>
    public class UndoManager
    {
        /// <summary>
        /// Occurs when an undo or redo action occurs.
        /// </summary>
        public event EventHandler<UndoRedoEventArgs> Changed;

        /// <summary>
        /// Initializes a new instance of the UndoManager class.
        /// </summary>
        /// <param name="context">The context of the <see cref="T:UndoManager" /> class.</param>
        /// <param name="maxLength">The maximum level of the actions that can be undone.</param>
        /// <param name="allowUndo">if set to <c>true</c> allow undo; otherwise, do not allow undo.</param>
        /// <remarks>
        /// -1 indicates the maximum level is unlimited.
        /// </remarks>
        public UndoManager(object context, int maxLength, bool allowUndo)
        {
            this.Context = context;
            if (maxLength < 0)
            {
                maxLength = 0x7fffffff;
            }
            this.MaxLength = maxLength;
            this.UndoList = new Stack<ICommand>();
            this.RedoList = new Stack<ICommand>();
            this.AllowUndo = allowUndo;
        }

        /// <summary>
        /// Performs an <see cref="T:Action" /> and adds it to the undo list if the <see cref="T:Action" /> can be undone.
        /// </summary>
        /// <param name="action">
        /// The <see cref="T:Action" />.
        /// </param>
        public bool Do(ICommand action)
        {
            if (action == null)
            {
                return false;
            }
            bool flag = true;
            try
            {
                action.Execute(this.Context);
            }
            catch
            {
                flag = false;
            }
            if (this.Context is SheetView)
            {
                (this.Context as SheetView).ResumeInvalidate();
            }
            if (this.AllowUndo)
            {
                IUndo undo = action as IUndo;
                if (!flag || (undo == null))
                {
                    return flag;
                }
                if ((this.MaxLength > 0) && (this.UndoList.Count >= this.MaxLength))
                {
                    this.ShrinkUndoList((this.UndoList.Count - this.MaxLength) + 1);
                }
                this.UndoList.Push(action);
                this.RedoList.Clear();
                this.RaiseChanged(UndoRedoOperation.Do, action.ToString());
            }
            return flag;
        }

        /// <summary>
        /// Returns all redo commands.
        /// </summary>
        public ICommand[] GetRedoList()
        {
            return this.RedoList.ToArray();
        }

        /// <summary>
        /// Returns all undo commands.
        /// </summary>
        public ICommand[] GetUndoList()
        {
            return this.UndoList.ToArray();
        }

        private void RaiseChanged(UndoRedoOperation undoRedo, string action)
        {
            if (this.Changed != null)
            {
                this.Changed(this, new UndoRedoEventArgs(undoRedo, action));
            }
        }

        /// <summary>
        /// Performs a redo action.
        /// </summary>
        /// <returns>
        /// Returns true if the redo action succeeds; otherwise, returns false.
        /// </returns>
        public bool Redo()
        {
            bool flag = true;
            if (this.AllowUndo && this.CanRedo)
            {
                ICommand command = this.RedoList.Peek();
                try
                {
                    command.Execute(this.Context);
                }
                catch
                {
                    flag = false;
                }
                if (flag)
                {
                    this.UndoList.Push(this.RedoList.Pop());
                    this.RaiseChanged(UndoRedoOperation.Redo, command.ToString());
                }
            }
            return flag;
        }

        /// <summary>
        /// Shrink the undo list by removing the specified count of items from the bottom of the stack.
        /// </summary>
        /// <param name="count">
        /// The specified count of items.
        /// </param>
        private void ShrinkUndoList(int count)
        {
            ICommand[] commandArray = new ICommand[this.UndoList.Count];
            this.UndoList.CopyTo(commandArray, 0);
            Array.Reverse(commandArray);
            this.UndoList.Clear();
            for (int i = count; i < commandArray.Length; i++)
            {
                this.UndoList.Push(commandArray[i]);
            }
        }

        /// <summary>
        /// Performs an undo action.
        /// </summary>
        /// <returns>
        /// Returns true if the undo action succeeds; otherwise, returns false.
        /// </returns>
        public bool Undo()
        {
            bool flag = true;
            if (this.AllowUndo && this.CanUndo)
            {
                IUndo undo = this.UndoList.Peek() as IUndo;
                try
                {
                    if ((undo != null) && undo.CanUndo)
                    {
                        flag = undo.Undo(this.Context);
                    }
                }
                catch
                {
                    flag = false;
                }
                if (this.Context is SheetView)
                {
                    (this.Context as SheetView).ResumeInvalidate();
                }
                if (flag)
                {
                    this.RedoList.Push(this.UndoList.Pop());
                    this.RaiseChanged(UndoRedoOperation.Undo, undo.ToString());
                }
            }
            return flag;
        }

        internal bool AllowUndo { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="T:UndoManager" /> can redo an action.
        /// </summary>
        public bool CanRedo
        {
            get { return  (this.RedoList.Count > 0); }
        }

        /// <summary>
        /// Gets a value that indicates whether the <see cref="T:UndoManager" /> can undo an action.
        /// </summary>
        public bool CanUndo
        {
            get { return  (this.UndoList.Count > 0); }
        }

        /// <summary>
        /// Gets the context of the <see cref="T:Dt.Cells.UI.UndoManager" /> which is used to execute the actions. 
        /// </summary>
        public object Context { get; private set; }

        /// <summary>
        /// Gets the maximum level of actions that can be undone.
        /// </summary>
        internal int MaxLength { get; private set; }

        /// <summary>
        /// Gets a list of the actions that can be redone.
        /// </summary>
        internal Stack<ICommand> RedoList { get; set; }

        /// <summary>
        /// Gets a list of the actions that can be undone.
        /// </summary>
        internal Stack<ICommand> UndoList { get; set; }
    }
}

