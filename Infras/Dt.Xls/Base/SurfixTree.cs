#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Xls
{
    internal class SurfixTree
    {
        private SurfixTreeNode root = new SurfixTreeNode();

        internal SurfixTree()
        {
        }

        internal void AddItem(string item)
        {
            int num = 0;
            SurfixTreeNode root = this.root;
            while (num < item.Length)
            {
                if (!root.Children.ContainsKey(item[num]))
                {
                    SurfixTreeNode node2 = new SurfixTreeNode();
                    root.Children.Add(item[num], node2);
                    root = node2;
                }
                else
                {
                    root = root.Children[item[num]];
                }
                num++;
            }
            root.IsEnd = true;
        }

        internal bool ContainsWord(string data)
        {
            int num = 0;
            SurfixTreeNode root = this.root;
            while (num < data.Length)
            {
                if (!root.Children.ContainsKey(data[num]))
                {
                    return false;
                }
                root = root.Children[data[num]];
                num++;
            }
            return root.IsEnd;
        }

        internal string GetFirstMatch(string input, int offset)
        {
            SurfixTreeNode root = this.root;
            StringBuilder builder = new StringBuilder();
            while (offset < input.Length)
            {
                if (!root.Children.ContainsKey(input[offset]))
                {
                    return null;
                }
                root = root.Children[input[offset]];
                builder.Append(input[offset]);
                if (root.IsEnd)
                {
                    return builder.ToString();
                }
                offset++;
            }
            return null;
        }
    }
}

