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
using System.Collections.Specialized;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a container for a collection of SpreadChart.
    /// </summary>
    public sealed class SpreadCharts : FloatingObjectCollection<SpreadChart>
    {
        internal SpreadCharts(IFloatingObjectSheet sheet) : base(sheet)
        {
        }

        void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (base.Worksheet != null)
            {
                base.Worksheet.RaiseChartCollectionChanged();
            }
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            if ((base.items != null) && (base.items.Count > 0))
            {
                using (List<SpreadChart>.Enumerator enumerator = base.items.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Raises the collection changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected internal override void RaiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.OnCollectionChanged(e);
            base.RaiseCollectionChanged(sender, e);
        }
    }
}

