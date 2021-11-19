#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using Dt.CalcEngine.Functions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represent the context for <see cref="T:Dt.CalcEngine.CalcEvaluator" /> which provide variant queries to get needed data.
    /// </summary>
    public class CalcEvaluatorContext
    {
        private int _expandArrayToMultiCallCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="arrayFormulaMode">if set to <c>true</c> [array formula mode].</param>
        /// <param name="baseRowIndex">Index of the base row.</param>
        /// <param name="baseColumnIndex">Index of the base column.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        public CalcEvaluatorContext(ICalcSource source, bool arrayFormulaMode = false, int baseRowIndex = 0, int baseColumnIndex = 0, int rowCount = 1, int columnCount = 1)
        {
            this.Source = source;
            this.ArrayFormulaMode = arrayFormulaMode;
            this.Row = baseRowIndex;
            this.Column = baseColumnIndex;
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
        }

        internal IDisposable EnterExpandArrayToMultiCallMode()
        {
            this._expandArrayToMultiCallCount++;
            return new EnterExpandArrayToMultiCallState(this);
        }

        /// <summary>
        /// Gets a <see cref="T:Dt.CalcEngine.Functions.CalcFunction" /> which function name is <paramref name="name" />.
        /// </summary>
        /// <param name="name">The search name.</param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.Functions.CalcFunction" /> indicates the function instance.
        /// If the <paramref name="name" /> is not recognized, return <see langword="null" />.
        /// </returns>
        public virtual CalcFunction GetFunction(string name)
        {
            if (this.Source == null)
            {
                return null;
            }
            return this.Source.GetFunction(name);
        }

        /// <summary>
        /// Gets a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which indicated by the name.
        /// </summary>
        /// <param name="name">The name to get a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" />.</param>
        /// <returns>A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" />.</returns>
        public virtual CalcExpression GetName(string name)
        {
            if (this.Source == null)
            {
                return null;
            }
            return this.Source.GetDefinedName(name, 0, 0);
        }

        /// <summary>
        /// Gets the reference at specified position which indicated by <paramref name="id" />.
        /// </summary>
        /// <param name="id">A <see cref="T:Dt.CalcEngine.CalcIdentity" /> indicates the identity of a address.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> indicates the reference.
        /// If the <paramref name="id" /> is not recognized, return <see cref="F:Dt.CalcEngine.CalcErrors.Reference" />.
        /// </returns>
        public virtual object GetReference(CalcIdentity id)
        {
            if (id is CalcExternalIdentity)
            {
                CalcExternalIdentity identity = id as CalcExternalIdentity;
                ICalcSource source = identity.Source;
                if (source != null)
                {
                    return source.GetReference(identity.ConvertToLocal());
                }
            }
            else
            {
                if (id is CalcSheetRangeIdentity)
                {
                    IMultiSourceProvider provider = this.Source as IMultiSourceProvider;
                    if (provider == null)
                    {
                        return CalcErrors.Reference;
                    }
                    try
                    {
                        CalcSheetRangeIdentity identity2 = id as CalcSheetRangeIdentity;
                        CalcLocalIdentity identity3 = identity2.ConvertToLocal();
                        List<CalcReference> references = new List<CalcReference>();
                        foreach (ICalcSource source2 in provider.GetCalcSources(identity2.StartSource, identity2.EndSource))
                        {
                            CalcReference item = source2.GetReference(identity3) as CalcReference;
                            if (item != null)
                            {
                                references.Add(item);
                            }
                        }
                        return new SheetRangeReference(references);
                    }
                    catch
                    {
                        return CalcErrors.Reference;
                    }
                }
                if ((this.Source != null) && (id is CalcLocalIdentity))
                {
                    return this.Source.GetReference(id as CalcLocalIdentity);
                }
            }
            return CalcErrors.Reference;
        }

        /// <summary>
        /// Gets the value at specified position which indicated by <paramref name="id" />.
        /// </summary>
        /// <param name="id">A <see cref="T:Dt.CalcEngine.CalcIdentity" /> indicates the identity of a address.</param>
        /// <returns>
        /// An <see cref="T:System.Object" /> indicates the value.
        /// If the <paramref name="id" /> is not recognized, return <see cref="F:Dt.CalcEngine.CalcErrors.Reference" />.
        /// </returns>
        public virtual object GetValue(CalcIdentity id)
        {
            if (id is CalcExternalIdentity)
            {
                CalcExternalIdentity identity = id as CalcExternalIdentity;
                ICalcSource source = identity.Source;
                if (source != null)
                {
                    CalcLocalIdentity identity3;
                    CalcLocalIdentity identity2 = identity.ConvertToLocal();
                    if ((this.ArrayFormulaMode || (this.RowCount > 1)) || (this.ColumnCount > 1))
                    {
                        identity3 = new CalcRangeIdentity(this.Row, this.Column, this.RowCount, this.ColumnCount);
                    }
                    else
                    {
                        identity3 = new CalcCellIdentity(this.Row, this.Column);
                    }
                    return source.GetEvaluatorContext(identity3).GetValue(identity2);
                }
            }
            else if ((this.Source != null) && (id is CalcLocalIdentity))
            {
                CalcRangeIdentity objA = id as CalcRangeIdentity;
                if (object.ReferenceEquals(objA, null))
                {
                    return this.Source.GetValue(id as CalcLocalIdentity);
                }
                if (objA._isFullColumn && objA._isFullRow)
                {
                    return this.Source.GetValue(new CalcCellIdentity(this.Row, this.Column));
                }
                if (objA._isFullColumn)
                {
                    return this.Source.GetValue(new CalcCellIdentity(this.Row, objA._columnIndex));
                }
                if (objA._isFullRow)
                {
                    return this.Source.GetValue(new CalcCellIdentity(objA._rowIndex, this.Column));
                }
                if (((objA._rowCount == 1) && (objA._columnIndex <= this.Column)) && (this.Column < (objA._columnIndex + objA._columnCount)))
                {
                    return this.Source.GetValue(new CalcCellIdentity(objA._rowIndex, this.Column));
                }
                if (((objA._columnCount == 1) && (objA._rowIndex <= this.Row)) && (this.Row < (objA._rowIndex + objA._rowCount)))
                {
                    return this.Source.GetValue(new CalcCellIdentity(this.Row, objA._columnIndex));
                }
                if ((objA._rowCount == 1) && (objA._columnCount == 1))
                {
                    return this.Source.GetValue(new CalcCellIdentity(objA._rowIndex, objA._columnIndex));
                }
                return CalcErrors.Value;
            }
            return CalcErrors.Reference;
        }

        private static bool InRange(int value, int less, int great)
        {
            return ((value >= less) && (value < great));
        }

        private static bool IsIntersected(CalcCellIdentity cellId, CalcRangeIdentity rangeId)
        {
            if (rangeId._isFullColumn && rangeId._isFullRow)
            {
                return true;
            }
            if (rangeId._isFullColumn)
            {
                return InRange(cellId._columnIndex, rangeId._columnIndex, rangeId._columnIndex + rangeId._columnCount);
            }
            if (rangeId._isFullRow)
            {
                return InRange(cellId._rowIndex, rangeId._rowIndex, rangeId._rowIndex + rangeId._rowCount);
            }
            return (InRange(cellId._rowIndex, rangeId._rowIndex, rangeId._rowIndex + rangeId._rowCount) && InRange(cellId._columnIndex, rangeId._columnIndex, rangeId._columnIndex + rangeId._columnCount));
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.CalcEngine.CalcIdentity" /> is intersected with another one.
        /// </summary>
        /// <param name="srcId">The source identity, intersected this with another one.</param>
        /// <param name="destId">The destination identity,</param>
        /// <returns>
        /// <see langword="true" /> if the specified calc identity is intersected; otherwise, <see langword="false" />.
        /// By default, compare two by <see cref="M:System.Object.ReferenceEquals(System.Object,System.Object)" />.
        /// </returns>
        public virtual bool IsIntersected(CalcIdentity srcId, CalcIdentity destId)
        {
            if (srcId == destId)
            {
                return true;
            }
            if (srcId is CalcCellIdentity)
            {
                if (destId is CalcRangeIdentity)
                {
                    return IsIntersected(srcId as CalcCellIdentity, destId as CalcRangeIdentity);
                }
            }
            else if (srcId is CalcRangeIdentity)
            {
                if (destId is CalcCellIdentity)
                {
                    return IsIntersected(destId as CalcCellIdentity, srcId as CalcRangeIdentity);
                }
                if (destId is CalcRangeIdentity)
                {
                    CalcRangeIdentity identity = srcId as CalcRangeIdentity;
                    CalcRangeIdentity identity2 = destId as CalcRangeIdentity;
                    if ((identity._isFullColumn && identity._isFullRow) || (identity2._isFullColumn && identity2._isFullRow))
                    {
                        return true;
                    }
                    if (identity._isFullColumn || identity2._isFullColumn)
                    {
                        if (!InRange(identity._columnIndex, identity2._columnIndex, identity2._columnIndex + identity2._columnCount))
                        {
                            return InRange(identity2._columnIndex, identity._columnIndex, identity._columnIndex + identity._columnCount);
                        }
                        return true;
                    }
                    if (identity._isFullRow || identity2._isFullRow)
                    {
                        if (!InRange(identity._rowIndex, identity2._rowIndex, identity2._rowIndex + identity2._rowCount))
                        {
                            return InRange(identity2._rowIndex, identity._rowIndex, identity._rowIndex + identity._rowCount);
                        }
                        return true;
                    }
                    return ((InRange(identity._rowIndex, identity2._rowIndex, identity2._rowIndex + identity2._rowCount) && InRange(identity._columnIndex, identity2._columnIndex, identity2._columnIndex + identity2._columnCount)) || (InRange(identity2._rowIndex, identity._rowIndex, identity._rowIndex + identity._rowCount) && InRange(identity2._columnIndex, identity._columnIndex, identity._columnIndex + identity._columnCount)));
                }
            }
            else if ((srcId is CalcExternalIdentity) && (destId is CalcExternalIdentity))
            {
                return this.IsIntersected((srcId as CalcExternalIdentity).ConvertToLocal(), (destId as CalcExternalIdentity).ConvertToLocal());
            }
            return false;
        }

        internal CalcEvaluatorContext Offset(int row, int column)
        {
            return new CalcEvaluatorContext(this.Source, this.ArrayFormulaMode, this.Row + row, this.Column + column, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Gets a value indicating whether evaluate expression in array formula mode.
        /// </summary>
        /// <value>
        /// <c>true</c> if evaluate expression in array formula mode]; otherwise, <c>false</c>.
        /// </value>
        public bool ArrayFormulaMode { get; private set; }

        /// <summary>
        /// Gets the column.
        /// </summary>
        /// <value>The column.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        public int ColumnCount { get; private set; }

        internal bool ExpandArrayToMultiCall
        {
            get
            {
                return (this._expandArrayToMultiCallCount > 0);
            }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        /// <value>The row.</value>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value>The source.</value>
        public ICalcSource Source { get; private set; }

        private class EnterExpandArrayToMultiCallState : IDisposable
        {
            private CalcEvaluatorContext _owner;

            public EnterExpandArrayToMultiCallState(CalcEvaluatorContext owner)
            {
                this._owner = owner;
            }

            public void Dispose()
            {
                this._owner._expandArrayToMultiCallCount--;
            }
        }
    }
}

