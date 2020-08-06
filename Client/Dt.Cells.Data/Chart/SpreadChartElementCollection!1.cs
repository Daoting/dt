#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a strongly typed collection for SpreadChartElement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SpreadChartElementCollection<T> : NotifyCollection<T> where T: SpreadChartElement
    {
        ChartArea _area;
        SpreadChartBase _chart;

        internal SpreadChartElementCollection(SpreadChartBase chart, ChartArea area)
        {
            this._chart = chart;
            this._area = area;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(T item)
        {
            base.Add(item);
            item.ChartBase = this._chart;
            this.NotifyChart();
        }

        internal void AddElementInternal(T element)
        {
            base.items.Add(element);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            List<T> list = new List<T>((IEnumerable<T>) base.items);
            base.Clear();
            using (List<T>.Enumerator enumerator = list.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ChartBase = null;
                }
            }
            this.NotifyChart();
        }

        internal void ClearElementsInternal()
        {
            base.ClearInternal();
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public override void Insert(int index, T item)
        {
            base.Insert(index, item);
            item.ChartBase = this._chart;
            this.NotifyChart();
        }

        void NotifyChart()
        {
            if (!base.IsEventSuspended)
            {
                this._chart.NotifyChartAreaChanged(this.ElementArea, "");
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Remove(T item)
        {
            bool flag = base.Remove(item);
            if (flag)
            {
                item.ChartBase = null;
                this.NotifyChart();
            }
            return flag;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public override void RemoveAt(int index)
        {
            T local = default(T);
            if ((index >= 0) && (index < this.Count))
            {
                local = this[index];
            }
            base.RemoveAt(index);
            if (local != null)
            {
                local.ChartBase = null;
                this.NotifyChart();
            }
        }

        internal ChartArea ElementArea
        {
            get { return  this._area; }
        }

        /// <summary>
        /// Gets or sets the SpreadChartElement at the specified index.
        /// </summary>
        public override T this[int index]
        {
            get { return  base[index]; }
            set
            {
                base[index] = value;
                if (value != null)
                {
                    value.ChartBase = this._chart;
                }
            }
        }
    }
}

