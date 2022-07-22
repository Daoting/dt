#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// The path tree object
    /// </summary>
    /// <typeparam name="P">path</typeparam>
    /// <typeparam name="V">value</typeparam>
    internal class PathTree<P, V>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.PathTree`2" /> class.
        /// </summary>
        public PathTree()
        {
            this.Root = new PathTreeNode<P, V>();
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public P[] GetPath(PathTreeNode<P, V> node)
        {
            List<P> list = new List<P>();
            for (PathTreeNode<P, V> node2 = node.Parent; node2 != null; node2 = node.Parent)
            {
                list.Insert(0, node2.GetPath(node));
                node = node2;
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets or sets the root.
        /// </summary>
        /// <value>The root.</value>
        public PathTreeNode<P, V> Root { get; set; }
    }
}

