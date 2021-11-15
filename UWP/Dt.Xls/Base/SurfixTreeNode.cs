#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    internal class SurfixTreeNode
    {
        internal SurfixTreeNode()
        {
            this.Children = new Dictionary<char, SurfixTreeNode>();
        }

        internal bool Contains(char c)
        {
            using (Dictionary<char, SurfixTreeNode>.Enumerator enumerator = this.Children.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    //hdt
                    if (c == this.Item)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool TryGetChild(char item, out SurfixTreeNode node)
        {
            if (this.Children.ContainsKey(item))
            {
                node = this.Children[item];
                return true;
            }
            node = null;
            return false;
        }

        internal Dictionary<char, SurfixTreeNode> Children { get; private set; }

        internal bool IsEnd { get; set; }

        internal char Item { get; set; }
    }
}

