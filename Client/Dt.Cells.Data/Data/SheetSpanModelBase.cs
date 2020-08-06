#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that supports creating a model that represents cell spans. 
    /// </summary>
    public abstract class SheetSpanModelBase
    {
        /// <summary>
        /// A flag used to indicate whether suspend event or not.
        /// </summary>
        int suspendEvent;

        /// <summary>
        /// Occurs when the user makes a change to the model 
        /// that affects the span of cells in the sheet.
        /// </summary>
        public event EventHandler<SheetSpanModelChangedEventArgs> Changed;

        protected SheetSpanModelBase()
        {
        }

        /// <summary>
        /// Adds a cell span to the collection.
        /// </summary>
        /// <param name="row">The row index at which to start the span.</param>
        /// <param name="column">The column index at which to start the span.</param>
        /// <param name="rowCount">The number of rows in the span.</param>
        /// <param name="columnCount">The number of columns in the span.</param>
        /// <returns>
        /// <c>true</c> if the span is added successfully; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Add(int row, int column, int rowCount, int columnCount);
        /// <summary>
        /// Removes all cell spans from the collection.
        /// </summary>
        public abstract void Clear();
        /// <summary>
        /// Finds the cell span with the specified anchor cell in the collection.
        /// </summary>
        /// <param name="row">The row index of the starting cell for the span.</param>
        /// <param name="column">The column index of the starting cell for the span.</param>
        /// <returns>Returns the cell range for the cell span.</returns>
        public abstract CellRange Find(int row, int column);
        /// <summary>
        /// Gets an enumerator for iterating to the next cell span in the collection after the specified span.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="rowCount">The number of rows in the cell span.</param>
        /// <param name="columnCount">The number of columns in the cell span.</param>
        /// <returns>Returns an enumerator to enumerate the span information for this model.</returns>
        public abstract IEnumerator GetEnumerator(int row, int column, int rowCount, int columnCount);
        /// <summary>
        /// Determines whether the model is empty of cell spans.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this model is empty of cell spans; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool IsEmpty();
        /// <summary>
        /// Determines whether the Changed event is suspend.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the Changed event is suspend; otherwise, <c>false</c>
        /// </returns>
        public bool IsEventSuspend()
        {
            return (this.suspendEvent > 0);
        }

        internal void RaiseChanged(SheetSpanModelChangedEventArgs e)
        {
            if ((this.Changed != null) && !this.IsEventSuspend())
            {
                this.Changed(this, e);
            }
        }

        /// <summary>
        /// Removes the cell span with the specified anchor cell from the collection. 
        /// </summary>
        /// <param name="row">The row index of the starting cell for the span.</param>
        /// <param name="column">The column index of the starting cell for the span.</param>
        public abstract void Remove(int row, int column);
        /// <summary>
        /// Resume the Changed event
        /// </summary>
        public void ResumeEvent()
        {
            this.suspendEvent--;
            if (this.suspendEvent < 0)
            {
                this.suspendEvent = 0;
            }
        }

        /// <summary>
        /// Suspend the Changed event
        /// </summary>
        public void SuspendEvent()
        {
            this.suspendEvent++;
        }
    }
}

