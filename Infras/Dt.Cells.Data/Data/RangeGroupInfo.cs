#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the range grouping information.
    /// </summary>
    public sealed class RangeGroupInfo
    {
        /// <summary>
        /// The children.
        /// </summary>
        Collection<RangeGroupInfo> children;
        /// <summary>
        /// The Owner.
        /// </summary>
        RangeGroup model;
        /// <summary>
        /// The parent group.
        /// </summary>
        RangeGroupInfo parent;

        /// <summary>
        /// Creates a new range group info.
        /// </summary>
        /// <param name="model">The range group model</param>
        /// <param name="start">The starting group index</param>
        /// <param name="end">The ending group index</param>
        /// <param name="level">The level for the group</param>
        internal RangeGroupInfo(RangeGroup model, int start, int end, int level)
        {
            this.model = model;
            this.Start = start;
            this.End = end;
            this.Level = level;
        }

        /// <summary>
        /// Adds the child.
        /// </summary>
        /// <param name="child">The child</param>
        internal void AddChild(RangeGroupInfo child)
        {
            if (child != null)
            {
                if (this.children == null)
                {
                    this.children = new Collection<RangeGroupInfo>();
                }
                this.children.Add(child);
                child.Parent = this;
            }
        }

        /// <summary>
        /// Compares this instance to a specified RangeGroupInfo object and returns an indication of their relative values.
        /// </summary>
        /// <param name="index">The index of the group item.</param>
        /// <returns>
        /// <c>true</c> if the range group contains the specified index; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int index)
        {
            return ((this.Start <= index) && (index <= this.End));
        }

        /// <summary>
        /// Gets the child groups of this outline (range group).
        /// </summary>
        /// <value>The child groups of this outline (range group).</value>
        public ReadOnlyCollection<RangeGroupInfo> Children
        {
            get
            {
                if (this.children == null)
                {
                    return new ReadOnlyCollection<RangeGroupInfo>(new RangeGroupInfo[0]);
                }
                if (this.children.Count == 0)
                {
                    return new ReadOnlyCollection<RangeGroupInfo>(new RangeGroupInfo[0]);
                }
                return new ReadOnlyCollection<RangeGroupInfo>((IList<RangeGroupInfo>) this.children);
            }
        }

        /// <summary>
        /// Gets the ending index of this outline (range group).
        /// </summary>
        /// <value>The ending index of this outline (range group).</value>
        [DefaultValue(-1)]
        public int End { get; private set; }

        /// <summary>
        /// Gets or sets the end index of this outline (range group).
        /// </summary>
        internal int EndInternal
        {
            get { return  this.End; }
            set { this.End = value; }
        }

        /// <summary>
        /// Gets the level of this outline (range group).
        /// </summary>
        /// <value>
        /// The level of this outline (range group).
        /// The default value is 0.
        /// </value>
        [DefaultValue(0)]
        public int Level { get; private set; }

        /// <summary>
        /// Gets the parent of this outline (range group).
        /// </summary>
        /// <value>
        /// The parent of this outline (range group).
        /// The default value is null.
        /// </value>
        [DefaultValue((string) null)]
        public RangeGroupInfo Parent
        {
            get { return  this.parent; }
            private set { this.parent = value; }
        }

        /// <summary>
        /// Gets the start index of this outline (range group).
        /// </summary>
        /// <value>The start index of this outline (range group).</value>
        [DefaultValue(-1)]
        public int Start { get; private set; }

        /// <summary>
        /// Gets or sets the state of this outline (range group).
        /// </summary>
        /// <value>
        /// The state of this outline (range group).
        /// The default value is <see cref="T:Dt.Cells.Data.GroupState">Expanded</see>.
        /// </value>
        [DefaultValue(0)]
        public GroupState State
        {
            get
            {
                if (this.model != null)
                {
                    return this.model.GetState(this);
                }
                return GroupState.Expanded;
            }
            set
            {
                if (this.model != null)
                {
                    this.model.Expand(this, value == GroupState.Expanded);
                }
            }
        }
    }
}

