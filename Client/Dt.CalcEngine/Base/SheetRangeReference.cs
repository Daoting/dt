#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// 
    /// </summary>
    internal class SheetRangeReference : CalcReference
    {
        private readonly List<CalcReference> _references;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="references"></param>
        public SheetRangeReference(ICollection<CalcReference> references)
        {
            if ((references == null) || (references.Count == 0))
            {
                throw new ArgumentNullException();
            }
            CalcReference reference = null;
            foreach (CalcReference reference2 in references)
            {
                if (reference == null)
                {
                    reference = reference2;
                }
                else
                {
                    if ((reference2.GetType() != reference.GetType()) || (reference2.RangeCount != reference.RangeCount))
                    {
                        throw new ArgumentException();
                    }
                    for (int i = 0; i < reference.RangeCount; i++)
                    {
                        if (((reference2.GetColumn(i) != reference.GetColumn(i)) || (reference2.GetColumnCount(i) != reference.GetColumnCount(i))) || ((reference2.GetRow(i) != reference.GetRow(i)) || (reference2.GetRowCount(i) != reference.GetRowCount(i))))
                        {
                            throw new ArgumentException();
                        }
                    }
                }
            }
            this._references = new List<CalcReference>();
            this._references.AddRange(references);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public override int GetColumn(int area)
        {
            return this._references[0].GetColumn(area);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public override int GetColumnCount(int area)
        {
            return this._references[0].GetColumnCount(area);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public override int GetRow(int area)
        {
            return this._references[0].GetRow(area);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public override int GetRowCount(int area)
        {
            return this._references[0].GetRowCount(area);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override CalcReference GetSource()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <returns></returns>
        public CalcReference GetSource(int sourceIndex)
        {
            return this._references[sourceIndex].GetSource();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="rowOffset"></param>
        /// <param name="columnOffset"></param>
        /// <returns></returns>
        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            return this._references[0].GetValue(area, rowOffset, columnOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="area"></param>
        /// <param name="rowOffset"></param>
        /// <param name="columnOffset"></param>
        /// <returns></returns>
        public object GetValue(int sourceIndex, int area, int rowOffset, int columnOffset)
        {
            return this._references[sourceIndex].GetValue(area, rowOffset, columnOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="rowOffset"></param>
        /// <param name="columnOffset"></param>
        /// <returns></returns>
        public override bool IsSubtotal(int area, int rowOffset, int columnOffset)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="area"></param>
        /// <param name="rowOffset"></param>
        /// <param name="columnOffset"></param>
        /// <returns></returns>
        public bool IsSubtotal(int sourceIndex, int area, int rowOffset, int columnOffset)
        {
            return this._references[sourceIndex].IsSubtotal(area, rowOffset, columnOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        public override int RangeCount
        {
            get
            {
                return this._references[0].RangeCount;
            }
        }

        internal List<CalcReference> References
        {
            get
            {
                return this._references;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SheetCount
        {
            get
            {
                return this._references.Count;
            }
        }
    }
}

