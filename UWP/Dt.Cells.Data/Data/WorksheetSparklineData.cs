#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class WorksheetSparklineData : ISparklineData
    {
        int anchorLine = -1;
        object[,] cachedData;
        int end;
        int start;

        public event EventHandler DataChanged;

        protected WorksheetSparklineData()
        {
        }

        internal void ClearCache()
        {
            this.cachedData = null;
        }

        void GetStartAndEnd()
        {
            switch (this.DataOrientation)
            {
                case Dt.Cells.Data.DataOrientation.Vertical:
                {
                    CalcRangeExpression dataReference = this.DataReference as CalcRangeExpression;
                    if (dataReference != null)
                    {
                        this.start = dataReference.StartRow;
                        this.end = dataReference.EndRow;
                        this.anchorLine = dataReference.StartColumn;
                    }
                    CalcExternalRangeExpression expression2 = this.DataReference as CalcExternalRangeExpression;
                    if (expression2 == null)
                    {
                        break;
                    }
                    this.start = expression2.StartRow;
                    this.end = expression2.EndRow;
                    this.anchorLine = expression2.StartColumn;
                    return;
                }
                case Dt.Cells.Data.DataOrientation.Horizontal:
                {
                    CalcRangeExpression expression3 = this.DataReference as CalcRangeExpression;
                    if (expression3 != null)
                    {
                        this.start = expression3.StartColumn;
                        this.end = expression3.EndColumn;
                        this.anchorLine = expression3.StartRow;
                    }
                    CalcExternalRangeExpression expression4 = this.DataReference as CalcExternalRangeExpression;
                    if (expression4 != null)
                    {
                        this.start = expression4.StartColumn;
                        this.end = expression4.EndColumn;
                        this.anchorLine = expression4.StartRow;
                    }
                    break;
                }
                default:
                    return;
            }
        }

        public object GetValue(int index)
        {
            if ((this.Sheet != null) && (this.DataReference != null))
            {
                if (this.cachedData == null)
                {
                    this.cachedData = this.Sheet.EvaluateExpression(this.DataReference, 0, 0, 0, 0, true) as object[,];
                    for (int i = 0; i < this.cachedData.GetLength(0); i++)
                    {
                        for (int j = 0; j < this.cachedData.GetLength(1); j++)
                        {
                            object val = this.cachedData[i, j];
                            if (val != null)
                            {
                                double? nullable = FormatConverter.TryDouble(val, false);
                                if (!nullable.HasValue)
                                {
                                    this.cachedData[i, j] = null;
                                }
                                else
                                {
                                    this.cachedData[i, j] = (double) nullable.Value;
                                }
                            }
                        }
                    }
                }
                if (!this.ShowHiddenData && !this.IsValueVisible(index))
                {
                    return (double) double.NaN;
                }
                if (this.cachedData != null)
                {
                    switch (this.DataOrientation)
                    {
                        case Dt.Cells.Data.DataOrientation.Vertical:
                            return this.cachedData[0, index];

                        case Dt.Cells.Data.DataOrientation.Horizontal:
                            return this.cachedData[index, 0];
                    }
                }
            }
            return null;
        }

        bool IsValueVisible(int index)
        {
            if (!this.ShowHiddenData)
            {
                Worksheet sheet = this.Sheet as Worksheet;
                if (sheet != null)
                {
                    switch (this.DataOrientation)
                    {
                        case Dt.Cells.Data.DataOrientation.Vertical:
                            if (sheet.GetActualColumnWidth(this.anchorLine, SheetArea.Cells) <= 0.0)
                            {
                                return false;
                            }
                            return (sheet.GetActualRowHeight(this.start + index, SheetArea.Cells) > 0.0);

                        case Dt.Cells.Data.DataOrientation.Horizontal:
                            if (sheet.GetActualRowHeight(this.anchorLine, SheetArea.Cells) <= 0.0)
                            {
                                return false;
                            }
                            return (sheet.GetActualColumnWidth(this.start + index, SheetArea.Cells) > 0.0);
                    }
                }
            }
            return true;
        }

        internal void OnDataChanged()
        {
            if (this.DataChanged != null)
            {
                this.DataChanged(this, EventArgs.Empty);
            }
        }

        public int Count
        {
            get
            {
                if ((this.DataReference is CalcCellExpression) || (this.DataReference is CalcExternalCellExpression))
                {
                    return 1;
                }
                this.GetStartAndEnd();
                if (this.end != this.start)
                {
                    return ((this.end - this.start) + 1);
                }
                return 0;
            }
        }

        internal abstract Dt.Cells.Data.DataOrientation DataOrientation { get; }

        internal abstract CalcExpression DataReference { get; }

        internal abstract ICalcEvaluator Sheet { get; }

        internal virtual bool ShowHiddenData
        {
            get { return  false; }
        }
    }
}

