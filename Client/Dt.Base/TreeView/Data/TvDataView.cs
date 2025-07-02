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
    /// Tv数据视图
    /// </summary>
    internal class TvDataView
    {
        Tv _owner;
        ITreeData _data;
        TvRootItems _rootItems;

        #region 构造方法
        public TvDataView(Tv p_owner, ITreeData p_data)
        {
            _owner = p_owner;
            _data = p_data;
            if (_data is INotifyCollectionChanged coll)
                coll.CollectionChanged += OnDataCollectionChanged;
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

            _owner.ClearAllTvItems();
            _rootItems = _owner.RootItems;
            if (_owner.FixedRoot != null)
            {
                // 固定根节点
                TvItem fixedItem = new TvItem(_owner, _owner.FixedRoot, null);
                if (rootData != null)
                {
                    foreach (var item in rootData)
                    {
                        TvItem ti = new TvItem(_owner, item, fixedItem);
                        BuildChildren(ti);
                        fixedItem.Children.Add(ti);
                    }
                }
                // 根节点状态
                if (fixedItem.Children.Count > 0 || _owner.IsDynamicLoading)
                    fixedItem.ExpandedState = TvItemExpandedState.NotExpanded;
                else
                    fixedItem.ExpandedState = TvItemExpandedState.Hide;
                _rootItems.Add(fixedItem);
            }
            else if (rootData != null)
            {
                foreach (var item in rootData)
                {
                    TvItem ti = new TvItem(_owner, item, null);
                    BuildChildren(ti);
                    _rootItems.Add(ti);
                }
            }

            // 自动展开第一个节点
            if (!_owner.IsDynamicLoading && _rootItems[0].Children.Count > 0)
                _rootItems[0].IsExpanded = true;
            _owner.LoadItems();
        }

        /// <summary>
        /// 卸载数据
        /// </summary>
        public void Destroy()
        {
            if (_data is INotifyCollectionChanged coll)
                coll.CollectionChanged -= OnDataCollectionChanged;
            _data = null;
            _rootItems = null;
            _owner = null;
        }

        /// <summary>
        /// 筛选
        /// </summary>
        public void ApplyFilterFlag()
        {
            if (_owner.FilterCfg == null
                || !_owner.FilterCfg.NeedFiltering
                || _rootItems.Count == 0)
            {
                _owner.RootItems = _rootItems;
                _owner.LoadItems();
                return;
            }

            lock (_rootItems)
            {
                var root = new TvRootItems(_owner);
                _owner.FilterCfg.PrepareFilter(_rootItems[0].Data);
                foreach (var ti in _rootItems)
                {
                    TvItem tiNew = new TvItem(_owner, ti.Data, null);
                    if (IsLeave(ti, tiNew))
                    {
                        tiNew.IsExpanded = true;
                        root.Add(tiNew);
                    }
                }
                
                // 释放上次搜索结果
                if (_rootItems != _owner.RootItems && _owner.RootItems.Count > 0)
                    TvCleaner.Add(_owner.RootItems);
                
                _owner.RootItems = root;
                root.Invalidate();
            }
        }

        bool IsLeave(TvItem p_parent, TvItem p_new)
        {
            bool leave = false;
            if (_owner.FilterCfg.DoFilter(p_parent.Data))
                leave = true;

            foreach (var ti in p_parent.Children)
            {
                TvItem tiNew = new TvItem(_owner, ti.Data, p_new);
                if (IsLeave(ti, tiNew))
                {
                    tiNew.IsExpanded = true;
                    p_new.Children.Add(tiNew);
                    leave = true;
                }
            }
            return leave;
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