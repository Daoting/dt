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
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base.TreeDiagrams
{
    /// <summary>
    /// 根节点集合
    /// </summary>
    internal class TdRootItems : List<TdItem>
    {
        TreeDiagram _owner;

        public TdRootItems(TreeDiagram p_owner)
        {
            _owner = p_owner;
        }

        public void ClearUI()
        {
            foreach (var item in GetAllItems())
            {
                item.ClearUI();
            }
        }

        /// <summary>
        /// 遍历所有节点
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TdItem> GetAllItems()
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

        IEnumerable<TdItem> GetAllChild(TdItem p_item)
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

    }
}