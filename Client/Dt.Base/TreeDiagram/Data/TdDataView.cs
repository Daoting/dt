#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Specialized;
#endregion

namespace Dt.Base.TreeDiagrams
{
    /// <summary>
    /// TreeDiagram数据视图
    /// </summary>
    internal class TdDataView
    {
        TreeDiagram _owner;
        ITreeData _data;

        #region 构造方法
        public TdDataView(TreeDiagram p_owner, ITreeData p_data)
        {
            _owner = p_owner;
            _data = p_data;
            if (_data is INotifyCollectionChanged coll)
                coll.CollectionChanged += OnDataCollectionChanged;
            Refresh();
        }
        #endregion

        /// <summary>
        /// 刷新视图列表
        /// </summary>
        public void Refresh()
        {
            var rootData = _data.GetTreeRoot();
            if (rootData == null && _owner.FixedRoot == null)
            {
                _owner.ClearItems();
                return;
            }

            var rootItems = _owner.RootItems;
            rootItems.Clear();
            if (_owner.FixedRoot != null)
            {
                // 固定根节点
                TdItem fixedItem = new TdItem(_owner, _owner.FixedRoot, null);
                foreach (var item in rootData)
                {
                    TdItem ti = new TdItem(_owner, item, fixedItem);
                    BuildChildren(ti);
                    fixedItem.Children.Add(ti);
                }
                rootItems.Add(fixedItem);
            }
            else
            {
                foreach (var item in rootData)
                {
                    TdItem ti = new TdItem(_owner, item, null);
                    BuildChildren(ti);
                    rootItems.Add(ti);
                }
            }
            _owner.LoadItems();
        }

        /// <summary>
        /// 卸载数据
        /// </summary>
        public void Unload()
        {
        }

        /// <summary>
        /// 递归加载子节点
        /// </summary>
        /// <param name="p_parent"></param>
        void BuildChildren(TdItem p_parent)
        {
            foreach (var item in _data.GetTreeItemChildren(p_parent.Data))
            {
                TdItem ti = new TdItem(_owner, item, p_parent);
                BuildChildren(ti);
                p_parent.Children.Add(ti);
            }
        }

        void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Refresh();
        }
    }
}