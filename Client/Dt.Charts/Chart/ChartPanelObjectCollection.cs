#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.ObjectModel;
#endregion

namespace Dt.Charts
{
    public class ChartPanelObjectCollection : ObservableCollection<ChartPanelObject>
    {
        ChartPanel _panel;

        internal ChartPanelObjectCollection(ChartPanel panel)
        {
            _panel = panel;
        }

        /// <summary>
        /// XamlTyp用，hdt
        /// </summary>
        public ChartPanelObjectCollection()
        { }

        protected override void ClearItems()
        {
            foreach (ChartPanelObject obj2 in this)
            {
                _panel.DetachEvents(obj2, obj2.Action);
                obj2.Panel = null;
            }
            base.ClearItems();
            _panel.BaseChildren.Clear();
        }

        protected override void InsertItem(int index, ChartPanelObject item)
        {
            base.InsertItem(index, item);
            _panel.AttachEvents(item, item.Action);
            _panel.BaseChildren.Insert(index, item);
            item.Panel = _panel;
        }

        protected override void RemoveItem(int index)
        {
            ChartPanelObject obj2 = base[index];
            _panel.DetachEvents(obj2, obj2.Action);
            base.RemoveItem(index);
            _panel.BaseChildren.RemoveAt(index);
            obj2.Panel = null;
        }

        protected override void SetItem(int index, ChartPanelObject item)
        {
            _panel.DetachEvents(base[index], base[index].Action);
            base[index].Panel = null;
            base.SetItem(index, item);
            _panel.BaseChildren[index] = item;
            _panel.AttachEvents(item, item.Action);
            item.Panel = _panel;
        }
    }
}

