#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-01-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// 根节点集合
    /// </summary>
    internal class TvRootItems : List<TvItem>
    {
        TreeView _owner;
        int _cntExpanded = -1;

        public TvRootItems(TreeView p_owner)
        {
            _owner = p_owner;
        }

        /// <summary>
        /// 失效重绘
        /// </summary>
        public void Invalidate()
        {
            _cntExpanded = -1;
            if (_owner.Panel != null)
                _owner.Panel.InvalidateMeasure();
        }

        /// <summary>
        /// 获取要显示的节点数
        /// </summary>
        /// <returns></returns>
        public int GetExpandedCount()
        {
            if (_cntExpanded > -1)
                return _cntExpanded;

            _cntExpanded = 0;
            foreach (var item in this)
            {
                AddExpandedCount(item);
            }
            return _cntExpanded;
        }

        /// <summary>
        /// 枚举所有要显示的节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TvItem> GetExpandedItems()
        {
            foreach (var item in this)
            {
                yield return item;
                if (item.IsExpanded && item.Children.Count > 0)
                {
                    foreach (var ti in GetExpandedChildren(item))
                    {
                        yield return ti;
                    }
                }
            }
        }

        /// <summary>
        /// 遍历所有节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TvItem> GetAllItems()
        {
            foreach (var item in this)
            {
                yield return item;
                if (item.Children.Count > 0)
                {
                    foreach (var ti in GetAllChild(item))
                    {
                        yield return ti;
                    }
                }
            }
        }

        /// <summary>
        /// 获取节点在可视节点(已展开)中的序号
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        public int GetVerIndex(TvItem p_item)
        {
            ItemIndex size = new ItemIndex { Taget = p_item };
            foreach (var item in this)
            {
                if (FindItemIndex(item, size))
                    break;
            }
            return size.Index;
        }

        /// <summary>
        /// 非虚拟行时节点在面板的垂直位置
        /// </summary>
        /// <param name="p_item"></param>
        /// <param name="p_panel"></param>
        /// <returns></returns>
        public Rect GetExpandedPostion(TvItem p_item, TvPanel p_panel)
        {
            ItemPosition pos = new ItemPosition { Taget = p_item, Panel = p_panel };
            foreach (var item in this)
            {
                if (FindItemPostion(item, pos))
                    break;
            }
            return new Rect(0, pos.Top, 0, pos.Height);
        }

        IEnumerable<TvItem> GetAllChild(TvItem p_item)
        {
            foreach (var child in p_item.Children)
            {
                yield return child;
                if (child.Children.Count > 0)
                {
                    foreach (var ti in GetAllChild(child))
                    {
                        yield return ti;
                    }
                }
            }
        }

        IEnumerable<TvItem> GetExpandedChildren(TvItem p_item)
        {
            foreach (var child in p_item.Children)
            {
                yield return child;
                if (child.IsExpanded && child.Children.Count > 0)
                {
                    foreach (var ti in GetExpandedChildren(child))
                    {
                        yield return ti;
                    }
                }
            }
        }

        void AddExpandedCount(TvItem p_item)
        {
            _cntExpanded++;
            if (p_item.IsExpanded)
            {
                foreach (var child in p_item.Children)
                {
                    AddExpandedCount(child);
                }
            }
        }

        bool FindItemIndex(TvItem p_item, ItemIndex p_tgt)
        {
            if (p_item == p_tgt.Taget)
                return true;

            p_tgt.Index++;
            if (p_item.IsExpanded)
            {
                foreach (var child in p_item.Children)
                {
                    if (FindItemIndex(child, p_tgt))
                        return true;
                }
            }
            return false;
        }

        bool FindItemPostion(TvItem p_item, ItemPosition p_pos)
        {
            var elem = (UIElement)p_pos.Panel.Children[p_pos.Index];
            if (p_item == p_pos.Taget)
            {
                p_pos.Height = elem.DesiredSize.Height;
                return true;
            }

            p_pos.Top += elem.DesiredSize.Height;
            p_pos.Index++;
            if (p_item.Children.Count == 0)
                return false;

            foreach (var child in p_item.Children)
            {
                if (FindItemPostion(child, p_pos))
                    return true;
            }
            return false;
        }

        class ItemIndex
        {
            public int Index = 0;
            public TvItem Taget;
        }

        class ItemPosition
        {
            public int Index = 0;
            public double Top = 0;
            public double Height = 0;
            public TvItem Taget;
            public TvPanel Panel;
        }
    }
}