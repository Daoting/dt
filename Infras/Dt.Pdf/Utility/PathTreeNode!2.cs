#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// The node of path tree
    /// </summary>
    /// <typeparam name="P">path</typeparam>
    /// <typeparam name="V">value</typeparam>
    internal class PathTreeNode<P, V>
    {
        private PathTreeNode<P, V> parent;
        private readonly Dictionary<P, PathTreeNode<P, V>> subNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.PathTreeNode`2" /> class.
        /// </summary>
        public PathTreeNode()
        {
            this.subNodes = new Dictionary<P, PathTreeNode<P, V>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.Utility.PathTreeNode`2" /> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public PathTreeNode(V data)
        {
            this.subNodes = new Dictionary<P, PathTreeNode<P, V>>();
            this.Data = data;
        }

        /// <summary>
        /// Determines whether the specified path contains path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// <c>true</c> if the specified path contains path; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPath(P path)
        {
            return this.subNodes.ContainsKey(path);
        }

        /// <summary>
        /// Determines whether the specified paths contains path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns>
        /// <c>true</c> if the specified paths contains path; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsPath(P[] paths)
        {
            if ((paths == null) || (paths.Length <= 0))
            {
                throw new PdfArgumentNullException("paths");
            }
            P path = paths[0];
            if (!this.ContainsPath(path))
            {
                return false;
            }
            if (paths.Length == 1)
            {
                return true;
            }
            P[] destinationArray = new P[paths.Length - 1];
            Array.Copy(paths, 1, destinationArray, 0, destinationArray.Length);
            return this[path].ContainsPath(destinationArray);
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public P GetPath(PathTreeNode<P, V> node)
        {
            foreach (KeyValuePair<P, PathTreeNode<P, V>> pair in this.subNodes)
            {
                if (pair.Value == node)
                {
                    return pair.Key;
                }
            }
            throw new PdfInternalException("Can not get path from node");
        }

        /// <summary>
        /// Visits the specified paths.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public PathTreeNode<P, V> Visit(P[] paths)
        {
            PathTreeNode<P, V> node;
            if ((paths == null) || (paths.Length <= 0))
            {
                throw new PdfArgumentNullException("paths");
            }
            P path = paths[0];
            if (this.ContainsPath(path))
            {
                node = this[path];
            }
            else
            {
                node = new PathTreeNode<P, V> {
                    parent = (PathTreeNode<P, V>) this
                };
                this.subNodes.Add(path, node);
            }
            if (paths.Length == 1)
            {
                return node;
            }
            P[] destinationArray = new P[paths.Length - 1];
            Array.Copy(paths, 1, destinationArray, 0, destinationArray.Length);
            return node.Visit(destinationArray);
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public V Data { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Pdf.Utility.PathTreeNode`2" /> with the specified path.
        /// </summary>
        /// <value></value>
        public PathTreeNode<P, V> this[P path]
        {
            get { return  this.subNodes[path]; }
            set
            {
                if (this.ContainsPath(path))
                {
                    this.subNodes[path].parent = null;
                }
                if (value != null)
                {
                    value.parent = (PathTreeNode<P, V>) this;
                }
                this.subNodes[path] = value;
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public PathTreeNode<P, V> Parent
        {
            get { return  this.parent; }
        }
    }
}

