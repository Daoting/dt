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
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class CovarianceList<T> : IEnumerable<T>, IEnumerable
    {
        List<T> _items;

        public CovarianceList()
        {
            this._items = new List<T>();
        }

        public void AddRange<U>(IEnumerable<U> items) where U: T
        {
            foreach (U local in items)
            {
                this._items.Add(local);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>) this._items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.GetEnumerator();
        }

        public int Count
        {
            get { return  this._items.Count; }
        }
    }
}

