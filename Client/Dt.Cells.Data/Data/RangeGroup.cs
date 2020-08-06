#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a range group for the worksheet.
    /// </summary>
    public sealed class RangeGroup : IMultipleSupport, IXmlSerializable
    {
        /// <summary>
        /// The direction.
        /// </summary>
        RangeGroupDirection direction;
        RangeGroupItemInfo head;
        /// <summary>
        /// The rows and columns information of the range group.
        /// </summary>
        SparseArray<RangeGroupItemInfo> items;
        /// <summary>
        /// The root range group.
        /// </summary>
        RangeGroupInfo rootCached;
        /// <summary>
        /// The state of suspend adding group.
        /// </summary>
        WorkingState suspendAddingGroup;
        /// <summary>
        /// The state of suspend adding group.
        /// </summary>
        WorkingState suspendEvent;
        RangeGroupItemInfo tail;

        internal event EventHandler Changed;

        /// <summary>
        /// Creates an empty range group.
        /// </summary>
        public RangeGroup() : this(0, null)
        {
        }

        /// <summary>
        /// Creates a new range group with a specified number of items.
        /// </summary>
        /// <param name="count">The count of rows or columns.</param>
        /// <param name="sortedIndexAdapter">The sorted index adapter.</param>
        internal RangeGroup(int count, Dt.Cells.Data.SortedIndexAdapter sortedIndexAdapter)
        {
            this.direction = RangeGroupDirection.Forward;
            this.suspendAddingGroup = new WorkingState();
            this.suspendEvent = new WorkingState();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.items = new SparseArray<RangeGroupItemInfo>(count);
            this.SortedIndexAdapter = sortedIndexAdapter;
        }

        /// <summary>
        /// Re-creates the range group.
        /// </summary>
        /// <returns>The range group</returns>
        RangeGroupInfo CreateRangeGroup()
        {
            RangeGroupInfo info = new RangeGroupInfo(this, 0, this.Count - 1, -1);
            GroupedItemIndexEnumerator e = new GroupedItemIndexEnumerator(this);
            while (e.MoveNext())
            {
                RangeGroupInfo child = this.CreateRangeGroup(e, 0);
                if ((child != null) && (child.Level > -1))
                {
                    info.AddChild(child);
                }
            }
            return info;
        }

        /// <summary>
        /// Creates the range group.
        /// </summary>
        /// <param name="e">The enumerator</param>
        /// <param name="level">The level</param>
        /// <returns>The range group</returns>
        RangeGroupInfo CreateRangeGroup(GroupedItemIndexEnumerator e, int level)
        {
            RangeGroupInfo info = null;
            do
            {
                int current = e.Current;
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(current);
                RangeGroupItemInfo info2 = this.items[modelIndexFromViewIndex];
                int nextToCurrent = e.NextToCurrent;
                if (info2.Level >= level)
                {
                    if ((info2.Level >= level) && (info == null))
                    {
                        info = new RangeGroupInfo(this, current, current, level);
                    }
                    if (info2.Level > level)
                    {
                        RangeGroupInfo child = this.CreateRangeGroup(e, level + 1);
                        if (e.Current > -1)
                        {
                            current = e.Current;
                            nextToCurrent = e.NextToCurrent;
                        }
                        else
                        {
                            current = child.End;
                            nextToCurrent = -1;
                        }
                        info.AddChild(child);
                    }
                    if (current > info.EndInternal)
                    {
                        info.EndInternal = current;
                    }
                    if (this.IsGroupEnd(current, nextToCurrent, level))
                    {
                        return info;
                    }
                }
            }
            while (e.MoveNext());
            return info;
        }

        /// <summary>
        /// Determines whether the specified outline (range group) is equal to the current range group.
        /// </summary>
        /// <param name="obj">The range group to compare with the current range group.</param>
        /// <returns>
        /// <c>true</c> if the specified range group is equal to the current range group; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            RangeGroup group = obj as RangeGroup;
            if (group == null)
            {
                return false;
            }
            if (((this.items == null) && (group.items != null)) && (group.items.Length > 0))
            {
                return false;
            }
            if (((group.items == null) && (this.items != null)) && (this.items.Length > 0))
            {
                return false;
            }
            if (!this.RangeGroupItemInfoEquals(this.head, group.head))
            {
                return false;
            }
            if (!this.RangeGroupItemInfoEquals(this.tail, group.tail))
            {
                return false;
            }
            if ((this.items != null) && (group.items != null))
            {
                if (this.direction == group.direction)
                {
                    if (this.items.Length != group.items.Length)
                    {
                        return false;
                    }
                    if (this.items.DataLength != group.items.DataLength)
                    {
                        return false;
                    }
                    int index = this.items.FirstNonEmptyIndex();
                    for (int i = group.items.FirstNonEmptyIndex(); index == i; i = group.items.NextNonEmptyIndex(i))
                    {
                        if ((index == -1) && (i == -1))
                        {
                            goto Label_0115;
                        }
                        index = this.items.NextNonEmptyIndex(index);
                    }
                }
                return false;
            }
        Label_0115:
            return true;
        }

        /// <summary>
        /// Expands or collapses the specified outline (range group) of rows or columns.
        /// </summary>
        /// <param name="groupInfo">The group information of the range group.</param>
        /// <param name="expand">Whether to expand the group.</param>
        public void Expand(RangeGroupInfo groupInfo, bool expand)
        {
            if (groupInfo == null)
            {
                throw new ArgumentNullException("groupInfo");
            }
            int index = -1;
            switch (this.Direction)
            {
                case RangeGroupDirection.Backward:
                    index = groupInfo.Start - 1;
                    break;

                case RangeGroupDirection.Forward:
                    index = groupInfo.End + 1;
                    break;
            }
            this.SetCollapsed(index, !expand);
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Expands all outlines (range groups), by the specified level.
        /// </summary>
        /// <param name="level">The level of the outline to expand or collapse.</param>
        /// <param name="expand">Whether to expand the groups.</param>
        public void Expand(int level, bool expand)
        {
            if (level < -1)
            {
                throw new ArgumentOutOfRangeException("level");
            }
            GroupedItemIndexEnumerator enumerator = new GroupedItemIndexEnumerator(this);
            while (enumerator.MoveNext())
            {
                this.Expand(enumerator.Current, level, expand);
            }
        }

        /// <summary>
        /// Expands or collapses an outline (range group) of rows or columns, by the specified level and index.
        /// </summary>
        /// <param name="index">The summary row or column index of the outline to expand or collapse.</param>
        /// <param name="level">The level of the outline to expand or collapse.</param>
        /// <param name="expand">Whether to expand the group.</param>
        public void Expand(int index, int level, bool expand)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (level < -1)
            {
                throw new ArgumentOutOfRangeException("level");
            }
            RangeGroupInfo groupInfo = this.Find(index, level);
            if (groupInfo != null)
            {
                this.Expand(groupInfo, expand);
            }
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Gets the outline (range group) with a specified group level and row or column index.
        /// </summary>
        /// <param name="index">The index of the row or column.</param>
        /// <param name="level">The level of the outline (range group).</param>
        /// <returns>Returns the specified range group.</returns>
        public RangeGroupInfo Find(int index, int level)
        {
            if (this.rootCached == null)
            {
                return null;
            }
            if (level == -1)
            {
                return this.rootCached;
            }
            if (!this.IsIndexValid(index))
            {
                return null;
            }
            return this.Find(this.rootCached, index, level);
        }

        /// <summary>
        /// Finds the group
        /// </summary>
        /// <param name="group">The group</param>
        /// <param name="index">The index</param>
        /// <param name="level">The level</param>
        /// <returns>The range group</returns>
        RangeGroupInfo Find(RangeGroupInfo group, int index, int level)
        {
            if (group != null)
            {
                if (group.Contains(index) && (group.Level == level))
                {
                    return group;
                }
                if (group.Children != null)
                {
                    foreach (RangeGroupInfo info in group.Children)
                    {
                        RangeGroupInfo info2 = this.Find(info, index, level);
                        if (info2 != null)
                        {
                            return info2;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the collapsed internal.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal bool GetCollapsed(int index)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            return ((info != null) && info.Collapsed);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the level of a specified row or column.
        /// </summary>
        /// <param name="index">The index of the row or column.</param>
        /// <returns>Returns the level for the row or column.</returns>
        /// <remarks>Level's start index is from 0.</remarks>
        public int GetLevel(int index)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            if (info != null)
            {
                return info.Level;
            }
            return -1;
        }

        /// <summary>
        /// Gets the level internal.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns></returns>
        internal int GetLevelInternal(int index)
        {
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            if (info != null)
            {
                return (info.Level + 1);
            }
            return 0;
        }

        /// <summary>
        /// Gets the number of the deepest level.
        /// </summary>
        /// <returns>Returns the number of the deepest level.</returns>
        /// <remarks>The level index starts from 0.</remarks>
        public int GetMaxLevel()
        {
            int num = -1;
            GroupedItemIndexEnumerator enumerator = new GroupedItemIndexEnumerator(this);
            while (enumerator.MoveNext())
            {
                int level = this.GetLevel(enumerator.Current);
                if (num < level)
                {
                    num = level;
                }
            }
            return num;
        }

        /// <summary>
        /// Gets the model index from view index.
        /// </summary>
        /// <param name="index">View index</param>
        /// <returns></returns>
        internal int GetModelIndexFromViewIndex(int index)
        {
            if (this.SortedIndexAdapter != null)
            {
                return this.SortedIndexAdapter.GetModelIndexFromViewIndex(index);
            }
            return index;
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <param name="group">The group</param>
        /// <returns>The group state</returns>
        internal GroupState GetState(RangeGroupInfo group)
        {
            int index = -1;
            switch (this.Direction)
            {
                case RangeGroupDirection.Backward:
                    index = group.Start - 1;
                    break;

                case RangeGroupDirection.Forward:
                    index = group.End + 1;
                    break;
            }
            RangeGroupItemInfo head = null;
            if (index < 0)
            {
                head = this.head;
            }
            if ((index > -1) && (index < this.Count))
            {
                int viewIndexFromModelIndex = this.GetViewIndexFromModelIndex(index);
                head = this.items[viewIndexFromModelIndex];
            }
            else if (index >= this.Count)
            {
                head = this.tail;
            }
            if ((head != null) && head.Collapsed)
            {
                return GroupState.Collapsed;
            }
            return GroupState.Expanded;
        }

        /// <summary>
        /// Gets the view index from the model index.
        /// </summary>
        /// <param name="index">Model index</param>
        /// <returns></returns>
        internal int GetViewIndexFromModelIndex(int index)
        {
            if (this.SortedIndexAdapter != null)
            {
                return this.SortedIndexAdapter.GetViewIndexFromModelIndex(index);
            }
            return index;
        }

        /// <summary>
        /// Adds a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="index">The starting index where adding begins.</param>
        /// <param name="count">The number of rows or columns to add.</param>
        void IMultipleSupport.Add(int index, int count)
        {
            if (count > 0)
            {
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
                this.items.InsertRange(modelIndexFromViewIndex, count);
                if (!this.suspendAddingGroup.IsWorking && (index > 0))
                {
                    int num2 = this.GetModelIndexFromViewIndex(index - 1);
                    RangeGroupItemInfo head = null;
                    if (num2 < 0)
                    {
                        head = this.head;
                    }
                    else
                    {
                        head = this.items[num2];
                    }
                    if (head != null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            RangeGroupItemInfo info2 = new RangeGroupItemInfo {
                                Collapsed = head.Collapsed,
                                Level = head.Level
                            };
                            this.items[index + i] = info2;
                        }
                    }
                }
                this.rootCached = null;
                this.Refresh();
            }
        }

        /// <summary>
        /// Clears a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="from">The starting index where clearing begins.</param>
        /// <param name="count">The number of rows or columns to clear.</param>
        void IMultipleSupport.Clear(int from, int count)
        {
        }

        /// <summary>
        /// Copies a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="from">The starting index where copying begins.</param>
        /// <param name="to">The destination index.</param>
        /// <param name="count">The number of rows or columns to copy.</param>
        void IMultipleSupport.Copy(int from, int to, int count)
        {
            if (count > 0)
            {
                if ((this.SortedIndexAdapter == null) || !this.SortedIndexAdapter.IsSorted)
                {
                    if (from < 0)
                    {
                        from = 0;
                    }
                    if (to < 0)
                    {
                        to = 0;
                    }
                    if (from == to)
                    {
                        return;
                    }
                    List<KeyValuePair<int, RangeGroupItemInfo>> list = new List<KeyValuePair<int, RangeGroupItemInfo>>();
                    for (int i = this.items.NextNonEmptyIndex(from - 1); (i >= 0) && (i < (from + count)); i = this.items.NextNonEmptyIndex(i))
                    {
                        RangeGroupItemInfo info = new RangeGroupItemInfo(this.items[i]);
                        list.Add(new KeyValuePair<int, RangeGroupItemInfo>(i - from, info));
                    }
                    this.items.Clear(to, count);
                    if (list.Count > 0)
                    {
                        foreach (KeyValuePair<int, RangeGroupItemInfo> pair in list)
                        {
                            this.items[to + pair.Key] = pair.Value;
                        }
                    }
                }
                this.rootCached = null;
                this.Refresh();
            }
        }

        /// <summary>
        /// Moves a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="from">The starting index where moving begins.</param>
        /// <param name="to">The destination index.</param>
        /// <param name="count">The number of rows or columns to move.</param>
        void IMultipleSupport.Move(int from, int to, int count)
        {
            if (count > 0)
            {
                if ((this.SortedIndexAdapter == null) || !this.SortedIndexAdapter.IsSorted)
                {
                    if (from < 0)
                    {
                        from = 0;
                    }
                    if (to < 0)
                    {
                        to = 0;
                    }
                    if (from == to)
                    {
                        return;
                    }
                    List<KeyValuePair<int, RangeGroupItemInfo>> list = new List<KeyValuePair<int, RangeGroupItemInfo>>();
                    for (int i = this.items.NextNonEmptyIndex(from - 1); (i >= 0) && (i < (from + count)); i = this.items.NextNonEmptyIndex(i))
                    {
                        RangeGroupItemInfo info = new RangeGroupItemInfo(this.items[i]);
                        list.Add(new KeyValuePair<int, RangeGroupItemInfo>(i - from, info));
                    }
                    this.items.Clear(from, count);
                    this.items.Clear(to, count);
                    if (list.Count > 0)
                    {
                        foreach (KeyValuePair<int, RangeGroupItemInfo> pair in list)
                        {
                            this.items[to + pair.Key] = pair.Value;
                        }
                    }
                }
                this.rootCached = null;
                this.Refresh();
            }
        }

        /// <summary>
        /// Removes a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="index">The starting index where removing begins.</param>
        /// <param name="count">The number of rows or columns to remove.</param>
        void IMultipleSupport.Remove(int index, int count)
        {
            if (count > 0)
            {
                if ((this.SortedIndexAdapter == null) || !this.SortedIndexAdapter.IsSorted)
                {
                    this.items.RemoveRange(index, count);
                }
                this.rootCached = null;
                this.Refresh();
            }
        }

        /// <summary>
        /// Swaps a range of rows or columns in the outline (range group).
        /// </summary>
        /// <param name="from">The starting index where swapping begins.</param>
        /// <param name="to">The destination index.</param>
        /// <param name="count">The number of rows or columns to swap.</param>
        void IMultipleSupport.Swap(int from, int to, int count)
        {
        }

        /// <summary>
        /// Groups a range of rows or columns into an outline (range group) from a specified start index using a specified amount.
        /// </summary>
        /// <param name="index">The group starting index.</param>
        /// <param name="count">The number of rows or columns to group.</param>
        public void Group(int index, int count)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (!this.IsIndexValid((index + count) - 1))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.rootCached = null;
            for (int i = 0; i < count; i++)
            {
                int num2 = index + i;
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(num2);
                if (this.items[modelIndexFromViewIndex] == null)
                {
                    RangeGroupItemInfo info = new RangeGroupItemInfo {
                        Level = 0
                    };
                    this.items[modelIndexFromViewIndex] = info;
                }
                else
                {
                    RangeGroupItemInfo local1 = this.items[modelIndexFromViewIndex];
                    local1.Level++;
                }
            }
            this.rootCached = this.CreateRangeGroup();
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Determines whether the range group at the specified index is collapsed.
        /// </summary>
        /// <param name="index">The index of the row or column in the range group.</param>
        /// <returns>
        /// <c>true</c> if the specified row or column is collapsed; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCollapsed(int index)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            int level = this.GetLevel(index);
            if (level > -1)
            {
                RangeGroupInfo parent = this.Find(index, level);
                for (int i = -1; i <= level; i++)
                {
                    if (parent != null)
                    {
                        if (parent.State == GroupState.Collapsed)
                        {
                            return true;
                        }
                        parent = parent.Parent;
                        if (parent == null)
                        {
                            break;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the outline (range group) is empty.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this range group is empty; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEmpty()
        {
            if ((this.items != null) && (this.items.DataLength > 0))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines whether [is group end] [the specified index].
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="indexNext">The index next</param>
        /// <param name="processLevel">The process level</param>
        /// <returns>
        /// <c>true</c> if [is group end] [the specified index]; otherwise, <c>false</c>
        /// </returns>
        bool IsGroupEnd(int index, int indexNext, int processLevel)
        {
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            if (!this.IsIndexValid(indexNext))
            {
                return true;
            }
            int num2 = this.GetModelIndexFromViewIndex(indexNext);
            RangeGroupItemInfo info2 = this.items[num2];
            if (info2 == null)
            {
                return true;
            }
            if (info2.Level < info.Level)
            {
                int num3 = info.Level - info2.Level;
                int num4 = info.Level - processLevel;
                if (num3 == num4)
                {
                    return false;
                }
                if ((num4 >= 0) && (num4 < num3))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether [is index valid] [the specified index].
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>
        /// <c>true</c> if [is index valid] [the specified index]; otherwise, <c>false</c>
        /// </returns>
        bool IsIndexValid(int index)
        {
            return ((index >= -1) && (index < this.Count));
        }

        void RaiseChangedEvent()
        {
            if (this.Changed != null)
            {
                this.Changed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Ranges the group item info equals.
        /// </summary>
        /// <param name="item1">The item1</param>
        /// <param name="item2">The item2</param>
        /// <returns></returns>
        bool RangeGroupItemInfoEquals(RangeGroupItemInfo item1, RangeGroupItemInfo item2)
        {
            if (item1 == null)
            {
                return (item2 == null);
            }
            if (item2 == null)
            {
                return false;
            }
            return ((item1.Level == item2.Level) && (item1.Collapsed == item2.Collapsed));
        }

        /// <summary>
        /// Refreshes this range group.
        /// </summary>
        internal void Refresh()
        {
            if (!this.IsEmpty())
            {
                this.rootCached = this.CreateRangeGroup();
            }
        }

        internal void ResumeAdding()
        {
            this.suspendAddingGroup.Release();
        }

        /// <summary>
        /// Sets the collapsed level.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="collapsed">If set to <c>true</c>, [collapsed].</param>
        internal void SetCollapsed(int index, bool collapsed)
        {
            if (index < 0)
            {
                if (this.head == null)
                {
                    this.head = new RangeGroupItemInfo();
                }
                this.head.Collapsed = collapsed;
            }
            else if ((index > -1) && (index < this.Count))
            {
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
                if (this.items[modelIndexFromViewIndex] == null)
                {
                    this.items[modelIndexFromViewIndex] = new RangeGroupItemInfo();
                }
                this.items[modelIndexFromViewIndex].Collapsed = collapsed;
            }
            else if (index >= this.Count)
            {
                if (this.tail == null)
                {
                    this.tail = new RangeGroupItemInfo();
                }
                this.tail.Collapsed = collapsed;
            }
            this.Refresh();
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Sets the collapsed internal.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="collapsed">If set to <c>true</c>, [collapsed]</param>
        internal void SetCollapsedInternal(int index, bool collapsed)
        {
            if (this.items == null)
            {
                this.items = new SparseArray<RangeGroupItemInfo>();
            }
            if (this.items.Length <= index)
            {
                this.items.Length = index + 1;
            }
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            if (info == null)
            {
                info = new RangeGroupItemInfo();
            }
            info.Collapsed = collapsed;
            this.items[modelIndexFromViewIndex] = info;
            this.rootCached = null;
        }

        /// <summary>
        /// Sets the count for the group.
        /// </summary>
        /// <param name="count">The new number of row count</param>
        internal void SetCount(int count)
        {
            this.rootCached = null;
            int num = count - this.Count;
            if (num > 0)
            {
                this.items.InsertRange(this.items.Length, num);
            }
            else if (num < 0)
            {
                this.items.RemoveRange((this.items.Length - num) - 1, num);
            }
        }

        /// <summary>
        /// Sets the item level.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="level">The item level.</param>
        internal void SetLevel(int index, int level)
        {
            if (index < 0)
            {
                if (this.head == null)
                {
                    this.head = new RangeGroupItemInfo();
                }
                this.head.Level = level;
            }
            else if ((index > -1) && (index < this.Count))
            {
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
                if (this.items[modelIndexFromViewIndex] == null)
                {
                    this.items[modelIndexFromViewIndex] = new RangeGroupItemInfo();
                }
                this.items[modelIndexFromViewIndex].Level = level;
            }
            else if (index >= this.Count)
            {
                if (this.tail == null)
                {
                    this.tail = new RangeGroupItemInfo();
                }
                this.tail.Level = level;
            }
            this.Refresh();
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Sets the level internal.
        /// </summary>
        /// <param name="index">The index</param>
        /// <param name="level">The level</param>
        internal void SetLevelInternal(int index, int level)
        {
            if (this.items == null)
            {
                this.items = new SparseArray<RangeGroupItemInfo>();
            }
            if (this.items.Length <= index)
            {
                this.items.Length = index + 1;
            }
            int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(index);
            RangeGroupItemInfo info = this.items[modelIndexFromViewIndex];
            if (info == null)
            {
                info = new RangeGroupItemInfo();
            }
            info.Level = level - 1;
            this.items[modelIndexFromViewIndex] = info;
            this.rootCached = null;
        }

        /// <summary>
        /// Suspends the adding.
        /// </summary>
        /// <returns></returns>
        internal void SuspendAdding()
        {
            this.suspendAddingGroup.AddRef();
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            this.items = null;
            this.direction = RangeGroupDirection.Forward;
            while (reader.Read())
            {
                bool flag2;
                int num;
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "HeadCollapsed")
                    {
                        if (str == "TailCollapsed")
                        {
                            goto Label_00C4;
                        }
                        if (str == "Items")
                        {
                            goto Label_00ED;
                        }
                        if (str == "Direction")
                        {
                            goto Label_0261;
                        }
                    }
                    else
                    {
                        bool flag = Serializer.ReadAttributeBoolean("value", false, reader);
                        this.head = new RangeGroupItemInfo();
                        this.head.Collapsed = flag;
                    }
                }
                continue;
            Label_00C4:
                flag2 = Serializer.ReadAttributeBoolean("value", false, reader);
                this.tail = new RangeGroupItemInfo();
                this.tail.Collapsed = flag2;
                continue;
            Label_00ED:
                num = Serializer.ReadAttributeInt("length", 0, reader);
                if (num <= 0)
                {
                    continue;
                }
                this.items = new SparseArray<RangeGroupItemInfo>(num);
                XmlReader reader2 = Serializer.ExtractNode(reader);
                while (reader2.Read())
                {
                    if ((reader2.Name != "Item") || (reader2.NodeType != ((XmlNodeType) ((int) XmlNodeType.Element))))
                    {
                        continue;
                    }
                    int? nullable = Serializer.ReadAttributeInt("index1", reader2);
                    int? nullable2 = Serializer.ReadAttributeInt("index2", reader2);
                    int num2 = 0;
                    bool flag3 = false;
                    if (!nullable.HasValue || !nullable2.HasValue)
                    {
                        throw new FormatException(ResourceStrings.RangeGroupSerializeError);
                    }
                    XmlReader reader3 = Serializer.ExtractNode(reader2);
                    while (reader3.Read())
                    {
                        string str2;
                        if ((reader2.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str2 = reader3.Name) != null))
                        {
                            if (str2 != "Level")
                            {
                                if (str2 == "Collapsed")
                                {
                                    goto Label_01CE;
                                }
                                if (str2 == "Hidden")
                                {
                                    goto Label_01DF;
                                }
                            }
                            else
                            {
                                num2 = Serializer.ReadAttributeInt("value", 0, reader3);
                            }
                        }
                        continue;
                    Label_01CE:
                        flag3 = Serializer.ReadAttributeBoolean("value", false, reader3);
                        continue;
                    Label_01DF:
                        Serializer.ReadAttributeBoolean("value", false, reader3);
                    }
                    reader3.Close();
                    for (int i = nullable.Value; i <= nullable2.Value; i++)
                    {
                        RangeGroupItemInfo info = new RangeGroupItemInfo {
                            Collapsed = flag3,
                            Level = num2
                        };
                        this.items[i] = info;
                    }
                }
                reader2.Close();
                continue;
            Label_0261:
                this.direction = Serializer.ReadAttributeEnum<RangeGroupDirection>("value", RangeGroupDirection.Forward, reader);
            }
            this.rootCached = null;
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            int num4;
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (this.head != null)
            {
                Serializer.SerializeObj((bool) this.head.Collapsed, "HeadCollapsed", writer);
            }
            if (this.tail != null)
            {
                Serializer.SerializeObj((bool) this.tail.Collapsed, "TailCollapsed", writer);
            }
            if (this.items == null)
            {
                goto Label_0165;
            }
            writer.WriteStartElement("Items");
            Serializer.WriteAttribute("length", this.items.Length, writer);
            int num = -1;
            int num2 = -1;
            int index = -1;
            RangeGroupItemInfo info = null;
        Label_0088:
            num4 = this.items.NextNonEmptyIndex(index);
            bool flag = false;
            if (num4 > (index + 1))
            {
                flag = true;
            }
            index = num4;
            if (num == -1)
            {
                num = index;
                num2 = num;
            }
            if (info == null)
            {
                info = this.items[num2];
            }
            RangeGroupItemInfo info2 = null;
            if (!flag && (index > -1))
            {
                info2 = this.items[index];
                if ((info.Level == info2.Level) && (info.Collapsed == info2.Collapsed))
                {
                    num2 = index;
                    goto Label_0158;
                }
            }
            if (info != null)
            {
                writer.WriteStartElement("Item");
                Serializer.WriteAttribute("index1", num, writer);
                Serializer.WriteAttribute("index2", num2, writer);
                Serializer.SerializeObj((int) info.Level, "Level", writer);
                Serializer.SerializeObj((bool) info.Collapsed, "Collapsed", writer);
                writer.WriteEndElement();
            }
            num = index;
            num2 = num;
            info = info2;
        Label_0158:
            if (index > -1)
            {
                goto Label_0088;
            }
            writer.WriteEndElement();
        Label_0165:
            Serializer.SerializeObj(this.direction, "Direction", writer);
        }

        /// <summary>
        /// Removes all outlines (range groups).
        /// </summary>
        public void Ungroup()
        {
            this.rootCached = null;
            int count = this.Count;
            this.items = new SparseArray<RangeGroupItemInfo>(count);
            this.rootCached = this.CreateRangeGroup();
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Removes a range of rows or columns from the outline (range group) at the specified start index by a specified amount.
        /// </summary>
        /// <param name="index">The group starting index.</param>
        /// <param name="count">The number of rows or columns to remove.</param>
        public void Ungroup(int index, int count)
        {
            if (!this.IsIndexValid(index))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (!this.IsIndexValid((index + count) - 1))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            this.rootCached = null;
            for (int i = 0; i < count; i++)
            {
                int num2 = index + i;
                int modelIndexFromViewIndex = this.GetModelIndexFromViewIndex(num2);
                if ((this.items[modelIndexFromViewIndex] != null) && (this.items[modelIndexFromViewIndex].Level > -1))
                {
                    RangeGroupItemInfo local1 = this.items[modelIndexFromViewIndex];
                    local1.Level--;
                }
            }
            this.rootCached = this.CreateRangeGroup();
            this.RaiseChangedEvent();
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count</value>
        internal int Count
        {
            get
            {
                if (this.items == null)
                {
                    return 0;
                }
                return this.items.Length;
            }
            set
            {
                if (this.items == null)
                {
                    this.items = new SparseArray<RangeGroupItemInfo>();
                }
                this.items.Length = value;
            }
        }

        /// <summary>
        /// Gets the state information of the current range group.
        /// </summary>
        public RangeGroupData Data
        {
            get { return  new RangeGroupData(this); }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>
        /// A value that specifies the location of the range group summary row or column.
        /// The default value is <see cref="T:Dt.Cells.Data.RangeGroupDirection">Forward</see>.
        /// </value>
        [DefaultValue(1)]
        public RangeGroupDirection Direction
        {
            get { return  this.direction; }
            set
            {
                if (value != this.direction)
                {
                    this.direction = value;
                    this.RaiseChangedEvent();
                }
            }
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        int IMultipleSupport.Count
        {
            get
            {
                if (this.items == null)
                {
                    return 0;
                }
                return this.items.Length;
            }
        }

        /// <summary>
        /// Gets the group items
        /// </summary>
        internal SparseArray<RangeGroupItemInfo> Items
        {
            get { return  this.items; }
        }

        /// <summary>
        /// Gets or sets the sheet.
        /// </summary>
        /// <value>The sheet</value>
        internal Dt.Cells.Data.SortedIndexAdapter SortedIndexAdapter { get; set; }

        /// <summary>
        /// Represent an enumerator for all items of groups from head to tail.
        /// </summary>
        class GroupedItemIndexEnumerator : IEnumerator<int>, IEnumerator, IDisposable
        {
            /// <summary>
            /// The current.
            /// </summary>
            int currentIndex = -1;
            /// <summary>
            /// Indicates whether end of reading.
            /// </summary>
            bool isEOF;
            /// <summary>
            /// The owner.
            /// </summary>
            Dt.Cells.Data.RangeGroup rangeGroup;

            /// <summary>
            /// Initializes a new instance of the class.
            /// </summary>
            /// <param name="rangeGroup">The range group</param>
            internal GroupedItemIndexEnumerator(Dt.Cells.Data.RangeGroup rangeGroup)
            {
                this.rangeGroup = rangeGroup;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns>
            /// True if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
            /// </returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public virtual bool MoveNext()
            {
                if (!this.isEOF)
                {
                    if (this.RangeGroup == null)
                    {
                        return false;
                    }
                    if (this.RangeGroup.items == null)
                    {
                        return false;
                    }
                    if ((this.RangeGroup.SortedIndexAdapter == null) || !this.RangeGroup.SortedIndexAdapter.IsSorted)
                    {
                        if (this.Current == -1)
                        {
                            this.Current = this.RangeGroup.items.FirstNonEmptyIndex();
                        }
                        else
                        {
                            this.Current = this.RangeGroup.items.NextNonEmptyIndex(this.Current);
                        }
                    }
                    else
                    {
                        bool flag = false;
                        for (int i = this.currentIndex + 1; i < this.RangeGroup.items.Length; i++)
                        {
                            int modelIndexFromViewIndex = this.RangeGroup.GetModelIndexFromViewIndex(i);
                            if (this.RangeGroup.items[modelIndexFromViewIndex] != null)
                            {
                                flag = true;
                                this.Current = i;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            this.Current = -1;
                        }
                    }
                    if (this.Current > -1)
                    {
                        return true;
                    }
                    this.isEOF = true;
                }
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            void IEnumerator.Reset()
            {
                this.isEOF = false;
                this.Current = -1;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            void IDisposable.Dispose()
            {
                throw new InvalidOperationException();
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator (view index).
            /// </summary>
            /// <value></value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public int Current
            {
                get { return  this.currentIndex; }
                private set { this.currentIndex = value; }
            }

            /// <summary>
            /// Gets the item index that is next to the current position.
            /// </summary>
            /// <value>The index next to the current position.</value>
            public int NextToCurrent
            {
                get { return  (this.Current + 1); }
            }

            /// <summary>
            /// Gets the range group.
            /// </summary>
            /// <value>The range group</value>
            Dt.Cells.Data.RangeGroup RangeGroup
            {
                get { return  this.rangeGroup; }
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            /// <value></value>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            object IEnumerator.Current
            {
                get
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Represents state information of the range group.
        /// </summary>
        public class RangeGroupData
        {
            RangeGroup parent;

            internal RangeGroupData(RangeGroup parent)
            {
                this.parent = parent;
            }

            /// <summary>
            /// Gets whether the item is collapsed.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <returns></returns>
            public bool GetCollapsed(int index)
            {
                return this.parent.GetCollapsed(index);
            }

            /// <summary>
            /// Gets the level of a specified row or column.
            /// </summary>
            /// <param name="index">The index of the row or column.</param>
            /// <returns>Returns the level for the row or column.</returns>
            public int GetLevel(int index)
            {
                return this.parent.GetLevel(index);
            }

            /// <summary>
            /// Sets the collapsed level.
            /// </summary>
            /// <param name="index">The index.</param>
            /// <param name="collapsed">If set to <c>true</c>, [collapsed].</param>
            public void SetCollapsed(int index, bool collapsed)
            {
                this.parent.SetCollapsed(index, collapsed);
            }

            /// <summary>
            /// Sets the item level.
            /// </summary>
            /// <param name="index">The item index.</param>
            /// <param name="level">The item level.</param>
            public void SetLevel(int index, int level)
            {
                this.parent.SetLevel(index, level);
            }
        }

        /// <summary>
        /// Represents a row info of range group.
        /// </summary>
        internal sealed class RangeGroupItemInfo
        {
            public RangeGroupItemInfo()
            {
                this.Level = -1;
                this.Collapsed = false;
            }

            public RangeGroupItemInfo(RangeGroup.RangeGroupItemInfo info)
            {
                if (!object.ReferenceEquals(this, info))
                {
                    this.Level = info.Level;
                    this.Collapsed = info.Collapsed;
                }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this is collapsed.
            /// </summary>
            /// <value>
            /// <c>true</c> if collapsed; otherwise, <c>false</c>.
            /// </value>
            public bool Collapsed { get; set; }

            /// <summary>
            /// Gets or sets the outline level.
            /// </summary>
            /// <value>The outline level.</value>
            public int Level { get; set; }
        }
    }
}

