#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System.Collections.Specialized;
#endregion

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// TreeView数据视图
    /// </summary>
    internal class TvDataView
    {
        TreeView _owner;
        ITreeData _data;

        #region 构造方法
        public TvDataView(TreeView p_owner, ITreeData p_data)
        {
            _owner = p_owner;
            _data = p_data;
            //if (_data is INotifyCollectionChanged coll)
            //    coll.CollectionChanged += OnDataCollectionChanged;
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
                TvItem fixedItem = new TvItem(_owner, _owner.FixedRoot, null);
                foreach (var item in rootData)
                {
                    TvItem ti = new TvItem(_owner, item, fixedItem);
                    BuildChildren(ti);
                    fixedItem.Children.Add(ti);
                }
                // 根节点状态
                if (fixedItem.Children.Count > 0 || _owner.IsDynamicLoading)
                    fixedItem.ExpandedState = TvItemExpandedState.NotExpanded;
                else
                    fixedItem.ExpandedState = TvItemExpandedState.Hide;
                rootItems.Add(fixedItem);
            }
            else
            {
                foreach (var item in rootData)
                {
                    TvItem ti = new TvItem(_owner, item, null);
                    BuildChildren(ti);
                    rootItems.Add(ti);
                }
            }

            // 自动展开第一个节点
            if (!_owner.IsDynamicLoading && rootItems[0].Children.Count > 0)
                rootItems[0].IsExpanded = true;
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
        void BuildChildren(TvItem p_parent)
        {
            foreach (var item in _data.GetTreeItemChildren(p_parent.Data))
            {
                TvItem ti = new TvItem(_owner, item, p_parent);
                BuildChildren(ti);
                p_parent.Children.Add(ti);
            }

            // 节点状态
            if (p_parent.Children.Count > 0 || _owner.IsDynamicLoading)
                p_parent.ExpandedState = TvItemExpandedState.NotExpanded;
            else
                p_parent.ExpandedState = TvItemExpandedState.Hide;
        }

        void OnDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            Refresh();
        }
    }
}