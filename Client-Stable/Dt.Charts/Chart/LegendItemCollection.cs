#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
#endregion

namespace Dt.Charts
{
    public class LegendItemCollection : Collection<LegendItem>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }

        internal LegendItemCollection(ObservableCollection<LegendItem> items) : base((IList<LegendItem>) items)
        {
            //hdt
            items.CollectionChanged += (new NotifyCollectionChangedEventHandler(items_CollectionChanged));
            ((INotifyPropertyChanged)items).PropertyChanged += LegendItemCollection_PropertyChanged;
        }

        void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        void LegendItemCollection_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }
    }
}

