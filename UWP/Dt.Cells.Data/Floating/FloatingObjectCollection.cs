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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a collection for a floating object.
    /// </summary>
    public class FloatingObjectCollection<T> : NotifyCollection<T>, IRangeSupport, IDisposable where T: FloatingObject
    {
        IFloatingObjectSheet _sheet;

        internal FloatingObjectCollection(IFloatingObjectSheet innerSheet)
        {
            this.Sheet = innerSheet;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="T:System.InvalidOperationException">item must have name!</exception>
        public override void Add(T item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                throw new InvalidOperationException("item must have name!");
            }
            this.CheckObjectExists(item.Name);
            item.Owner = this;
            base.Add(item);
        }

        internal void AttachSheetEvents()
        {
            if (this.Sheet != null)
            {
                this.Sheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
                this.Sheet.ColumnChanged += new EventHandler<SheetChangedEventArgs>(this.InnerSheet_ColumnChanged);
                this.Sheet.RowChanged += new EventHandler<SheetChangedEventArgs>(this.InnerSheet_RowChanged);
                this.Sheet.PropertyChanged += new PropertyChangedEventHandler(this.InnerSheet_PropertyChanged);
            }
        }

        void CheckObjectExists(string name)
        {
            if ((this.Sheet.FindChart(name) != null) || (this.Sheet.FindFloatingObject(name) != null))
            {
                throw new ArgumentException(string.Format("The {0} already exists in sheet!", (object[]) new object[] { name }));
            }
        }

        /// <summary>
        /// Clears the internal.
        /// </summary>
        protected override void ClearInternal()
        {
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Owner = null;
                }
            }
            base.ClearInternal();
        }

        internal void DetachSheetEvents()
        {
            if (this.Sheet != null)
            {
                this.Sheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
                this.Sheet.ColumnChanged -= new EventHandler<SheetChangedEventArgs>(this.InnerSheet_ColumnChanged);
                this.Sheet.RowChanged -= new EventHandler<SheetChangedEventArgs>(this.InnerSheet_RowChanged);
                this.Sheet.PropertyChanged -= new PropertyChangedEventHandler(this.InnerSheet_PropertyChanged);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.DetachSheetEvents();
            this.OnDisposed();
        }

        /// <summary>
        /// Finds the floating object with specified name.
        /// </summary>
        /// <param name="name">The name of the floating object.</param>
        /// <returns></returns>
        public T Find(string name)
        {
            foreach (T local in this)
            {
                if ((local != null) && (local.Name == name))
                {
                    return local;
                }
            }
            return default(T);
        }

        void IRangeSupport.AddColumns(int column, int count)
        {
            this.SuspendItemsEvent();
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    // hdt 类型转换
                    ((IRangeSupport)(enumerator.Current)).AddColumns(column, count);
                }
            }
            this.ResumeItemsEvent();
        }

        void IRangeSupport.AddRows(int row, int count)
        {
            this.SuspendItemsEvent();
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ((IRangeSupport)(enumerator.Current)).AddRows(row, count);
                }
            }
            this.ResumeItemsEvent();
        }

        void IRangeSupport.Clear(int row, int column, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void IRangeSupport.RemoveColumns(int column, int count)
        {
            this.SuspendItemsEvent();
            List<T> list = new List<T>();
            List<T> list2 = new List<T>();
            foreach (T local in this)
            {
                if (local != null)
                {
                    if (!this.IsCoverObject(-1, column, this._sheet.RowCount, count, local) || (!local.DynamicMove && !local.DynamicSize))
                    {
                        ((IRangeSupport)local).RemoveColumns(column, count);
                        list.Add(local);
                    }
                    else
                    {
                        list2.Add(local);
                    }
                }
            }
            if (list.Count != base.items.Count)
            {
                base.items = list;
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Remove, (IList) list2, -1));
            }
            this.ResumeItemsEvent();
        }

        void IRangeSupport.RemoveRows(int row, int count)
        {
            this.SuspendItemsEvent();
            List<T> list = new List<T>();
            List<T> list2 = new List<T>();
            foreach (T local in this)
            {
                if (local != null)
                {
                    if (!this.IsCoverObject(row, -1, count, this._sheet.ColumnCount, local) || (!local.DynamicMove && !local.DynamicSize))
                    {
                        ((IRangeSupport)local).RemoveRows(row, count);
                        list.Add(local);
                    }
                    else
                    {
                        list2.Add(local);
                    }
                }
            }
            if (list.Count != base.items.Count)
            {
                base.items = list;
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Remove, (IList) list2, -1));
            }
            this.ResumeItemsEvent();
        }

        void IRangeSupport.Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            throw new NotImplementedException();
        }

        void InnerSheet_ColumnChanged(object sender, SheetChangedEventArgs e)
        {
            this.SuspendItemsEvent();
            if ((e.PropertyName == "Width") || (e.PropertyName == "Axis"))
            {
                this.NotifyColumnWidthChanged();
            }
            this.ResumeItemsEvent();
        }

        void InnerSheet_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.SuspendItemsEvent();
            if (e.PropertyName == "ColumnRangeGroup")
            {
                this.NotifyColumnWidthChanged();
            }
            else if (((e.PropertyName == "RowRangeGroup") || (e.PropertyName == "RowFilter")) || (e.PropertyName == "TableFilter"))
            {
                this.NotifyRowHeightChanged();
            }
            this.ResumeItemsEvent();
        }

        void InnerSheet_RowChanged(object sender, SheetChangedEventArgs e)
        {
            this.SuspendItemsEvent();
            if ((e.PropertyName == "Height") || (e.PropertyName == "Axis"))
            {
                this.NotifyRowHeightChanged();
            }
            this.ResumeItemsEvent();
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        /// <exception cref="T:System.InvalidOperationException">item must have name!</exception>
        public override void Insert(int index, T item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                throw new InvalidOperationException("item must have name!");
            }
            this.CheckObjectExists(item.Name);
            item.Owner = this;
            base.Insert(index, item);
        }

        bool IsCoverObject(int row, int column, int rowCount, int columnCount, T floatingObject)
        {
            CellRange range = floatingObject.Range;
            return ((((row <= range.Row) && (column <= range.Column)) && ((row + rowCount) >= (range.Row + range.RowCount))) && ((column + columnCount) >= (range.Column + range.ColumnCount)));
        }

        void NotifyCellValueChanged(int row, int column)
        {
        }

        void NotifyColumnWidthChanged()
        {
            this.UpdateObjectsLayout();
        }

        void NotifyRowHeightChanged()
        {
            this.UpdateObjectsLayout();
        }

        internal virtual void OnDisposed()
        {
        }

        internal void ReadXmlInternal(XmlReader reader)
        {
            Serializer.InitReader(reader);
            XmlReader reader2 = Serializer.ExtractNode(reader);
            List<T> list = new List<T>();
            Serializer.DeserializeList((IList) list, reader2);
            base.items.AddRange((IEnumerable<T>) list);
            reader2.Close();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Remove(T item)
        {
            bool flag = base.Remove(item);
            if (flag)
            {
                item.Owner = null;
            }
            return flag;
        }

        /// <summary>
        /// Removes the attribute.
        /// </summary>
        /// <param name="index">The index.</param>
        public override void RemoveAt(int index)
        {
            T local = default(T);
            if ((index >= 0) && (index < this.Count))
            {
                local = base.items[index];
            }
            base.RemoveAt(index);
            if (local != null)
            {
                local.Owner = null;
            }
        }

        void ReplaceObject(int index, T floatingObject)
        {
            if (index <= -1)
            {
                throw new ArgumentException(string.Format(ResourceStrings.FailedFoundWorksheetError, (object[]) new object[] { floatingObject.Name }));
            }
            this.CheckObjectExists(floatingObject.Name);
            T local = base[index];
            if (local != null)
            {
                local.Owner = null;
            }
            base.items[index] = floatingObject;
            floatingObject.Owner = this;
        }

        void ReplaceObject(string name, T floatingObject)
        {
            int index = -1;
            for (int i = 0; i < base.Items.Count; i++)
            {
                IFloatingObject obj2 = base.Items[i];
                if ((obj2 != null) && (obj2.Name == name))
                {
                    index = i;
                    break;
                }
            }
            this.ReplaceObject(index, floatingObject);
        }

        internal void ResumeAfterDeserialization()
        {
            if (this._sheet != null)
            {
                foreach (T local in this)
                {
                    local.SetOwnerInternal(this);
                    local.ResumeAfterDeserialization();
                }
            }
        }

        internal void ResumeItemsEvent()
        {
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResumeEvents();
                }
            }
        }

        void Sheet_CellChanged(object sender, CellChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                this.NotifyCellValueChanged(e.Row, e.Column);
            }
        }

        internal void SuspendItemsEvent()
        {
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.SuspendEvents();
                }
            }
        }

        void UpdateObjectsLayout()
        {
            this.SuspendItemsEvent();
            using (IEnumerator<T> enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.UpdateFloatingObjectLayoutOnRowColumnChanged();
                }
            }
            this.ResumeItemsEvent();
        }

        /// <summary>
        /// Gets or sets the floating object with the specified name.
        /// </summary>
        /// <value>
        /// The floating object.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException">value</exception>
        public T this[string name]
        {
            get { return  this.Find(name); }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.ReplaceObject(name, value);
            }
        }

        /// <summary>
        /// Gets or sets the floating object at the specified index.
        /// </summary>
        /// <value>
        /// The floating object.
        /// </value>
        /// <param name="index">The index of floating object.</param>
        /// <returns></returns>
        public override T this[int index]
        {
            get { return  base[index]; }
            set { this.ReplaceObject(index, value); }
        }

        internal IFloatingObjectSheet Sheet
        {
            get { return  this._sheet; }
            set
            {
                if (this._sheet != value)
                {
                    this.DetachSheetEvents();
                    this._sheet = value;
                    this.AttachSheetEvents();
                }
            }
        }

        internal Dt.Cells.Data.Worksheet Worksheet
        {
            get { return  (this.Sheet as Dt.Cells.Data.Worksheet); }
        }
    }
}

