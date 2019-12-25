#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections.ObjectModel;
#endregion

namespace Dt.Charts
{
    public class DataSeriesCollection : ObservableCollection<DataSeries>
    {
        ChartData _data;

        internal event EventHandler Changed;

        public DataSeriesCollection()
        {
        }

        internal DataSeriesCollection(ChartData data)
        {
            _data = data;
        }

        protected override void ClearItems()
        {
            if (_data != null)
            {
                foreach (IDataSeriesInfo info in this)
                {
                    _data.RemoveSeries(info);
                }
            }
            base.ClearItems();
            FireChanged(this, EventArgs.Empty);
        }

        void FireChanged(object sender, EventArgs e)
        {
            if (Changed != null)
            {
                Changed(sender, e);
            }
        }

        protected override void InsertItem(int index, DataSeries item)
        {
            if (_data != null)
            {
                _data.AddSeries(item);
            }
            base.InsertItem(index, item);
            FireChanged(this, EventArgs.Empty);
        }

        protected override void RemoveItem(int index)
        {
            if (_data != null)
            {
                _data.RemoveSeries(base[index]);
            }
            base.RemoveItem(index);
            FireChanged(this, EventArgs.Empty);
        }

        protected override void SetItem(int index, DataSeries item)
        {
            if (_data != null)
            {
                _data.AddSeries(item);
            }
            base.SetItem(index, item);
            FireChanged(this, EventArgs.Empty);
        }
    }
}

