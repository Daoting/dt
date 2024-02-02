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
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a base collection that notifies listeners of dynamic changes when items are added and removed 
    /// or the entire collection object is reset. 
    /// </summary>
    public abstract class NotifyCollectionBase<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// The items.
        /// </summary>
        protected List<T> items;
        /// <summary>
        /// The suspend state
        /// </summary>
        WorkingState suspendState;

        /// <summary>
        /// Occurs when the items list of the collection has changed or the collection is reset.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new notify collection.
        /// </summary>
        protected NotifyCollectionBase()
        {
            this.items = new List<T>();
            this.suspendState = new WorkingState();
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The new item.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only. </exception>
        public virtual void Add(T item)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            this.items.Add(item);
            if (!this.IsEventSuspended)
            {
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Add, item, this.items.Count - 1));
                this.RaisePropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        /// <summary>
        /// Adds the items in the specified collection to the end of the collection.
        /// </summary>
        /// <param name="items">The collection whose items should be added to the end of the collection.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only. </exception>
        public virtual void AddRange(IList<T> items)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            foreach (T local in items)
            {
                this.Add(local);
            }
        }

        /// <summary>
        /// Removes all items from this collection.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        public virtual void Clear()
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            this.ClearInternal();
            if (!this.IsEventSuspended)
            {
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Reset));
                this.RaisePropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        /// <summary>
        /// Clears the internal.
        /// </summary>
        protected virtual void ClearInternal()
        {
            this.items.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specified item.
        /// </summary>
        /// <param name="item">The item in the collection for which to search.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the specified item; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(T item)
        {
            return this.items.Contains(item);
        }

        /// <summary>
        /// Copies all the items in the current collection to the specified array starting at the specified destination index.
        /// </summary>
        /// <param name="array">The destination array.</param>
        /// <param name="arrayIndex">The destination index.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> for the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>) this.items.GetEnumerator();
        }

        /// <summary>
        /// Finds the index for a specified item.
        /// </summary>
        /// <param name="item">The item for which to find the index.</param>
        /// <returns>Returns a zero-based index for the specified item.</returns>
        public int IndexOf(T item)
        {
            return this.items.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at a specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the item.</param>
        /// <param name="item">The new item.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        public virtual void Insert(int index, T item)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            this.items.Insert(index, item);
            if (!this.IsEventSuspended)
            {
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Add, item, index));
                this.RaisePropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        /// <summary>
        /// Raises the CollectionChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance that contains the event data.</param>
        protected internal virtual void RaiseCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(sender, e);
            }
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="T:System.ComponentModel.PropertyChangedEventArgs" /> instance that contains the event data.</param>
        protected virtual void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(sender, e);
            }
        }

        /// <summary>
        /// Removes the specified item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        /// <returns><c>true</c> if the item is successfully removed; otherwise, <c>false</c>.</returns>
        public virtual bool Remove(T item)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            int index = this.items.IndexOf(item);
            bool flag = this.items.Remove(item);
            if (!this.IsEventSuspended && flag)
            {
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Remove, item, index));
                this.RaisePropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
            return flag;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the collection.</exception>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        public virtual void RemoveAt(int index)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            if ((index < 0) || (index >= this.items.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            object obj2 = this.items[index];
            this.items.RemoveAt(index);
            if (!this.IsEventSuspended)
            {
                this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Remove, obj2, index));
                this.RaisePropertyChanged(this, new PropertyChangedEventArgs("Count"));
            }
        }

        /// <summary>
        /// Resumes the collection Changed event.
        /// </summary>
        public void ResumeEvent()
        {
            this.suspendState.Release();
        }

        /// <summary>
        /// Suspends the collection Changed event.
        /// </summary>
        public void SuspendEvent()
        {
            this.suspendState.AddRef();
        }

        /// <summary>
        /// Copies the elements of the collection to a <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the collection. 
        /// The <see cref="T:System.Array" /> must have zero-based indexing.
        /// </param>
        /// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null. </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is less than zero. </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array" /> is multidimensional or <paramref name="index" /> is equal to or greater than the 
        /// length of <paramref name="array" /> or the number of elements in the source collection is greater than the available 
        /// space from <paramref name="index" /> to the end of the destination <paramref name="array" />. 
        /// </exception>
        /// <exception cref="T:System.ArgumentException">The type of the source collection cannot be cast automatically to the type of the destination <paramref name="array" />. </exception>
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if ((index < 0) && (index >= this.items.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.items.ToArray().CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection.
        /// </summary>
        /// <param name="value">The item to add to the collection.</param>
        /// <returns>
        /// The position at which the new item was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only or the collection has a fixed size. </exception>
        int IList.Add(object value)
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            if (value is T)
            {
                this.Add((T) value);
            }
            else
            {
                this.Add(default(T));
            }
            return (this.Count - 1);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only. </exception>
        void IList.Clear()
        {
            if (this.IsReadOnly)
            {
                throw new NotSupportedException(ResourceStrings.ModifyReadonlyCollectionError);
            }
            this.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains a specific value.
        /// </summary>
        /// <param name="value">The item to locate in the collection.</param>
        /// <returns>
        /// <c>true</c> if the item is found in the collection; otherwise, <c>false</c>.
        /// </returns>
        bool IList.Contains(object value)
        {
            return ((value is T) && this.Contains((T) value));
        }

        /// <summary>
        /// Determines the index of a specific item in the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns>
        /// Returns the index of <paramref name="item" /> if found in the list; otherwise, -1.
        /// </returns>
        int IList.IndexOf(object item)
        {
            if (item is T)
            {
                return this.IndexOf((T) item);
            }
            return -1;
        }

        /// <summary>
        /// Inserts an item in the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The item to insert into the collection.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the collection. </exception>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only or the collection has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="item" /> is a null reference in the collection.</exception>
        void IList.Insert(int index, object item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item is T)
            {
                this.Insert(index, (T) item);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the collection.
        /// </summary>
        /// <param name="item">The item to remove from the collection.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only or the collection has a fixed size. </exception>
        void IList.Remove(object item)
        {
            if (item is T)
            {
                this.Remove((T) item);
            }
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the collection.</exception>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        void IList.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        /// <summary>
        /// Converts the collection to an array.
        /// </summary>
        /// <returns>Returns an array object that contains all the items in the collection.</returns>
        public virtual T[] ToArray()
        {
            T[] localArray = new T[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                localArray[i] = this[i];
            }
            return localArray;
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value>The number of elements contained in the collection. The default value is 0.</value>
        [DefaultValue(0)]
        public virtual int Count
        {
            get { return  this.items.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is event suspended.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is event suspended; otherwise, <c>false</c>.
        /// </value>
        internal bool IsEventSuspended
        {
            get { return  this.suspendState.IsWorking; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if the collection is read-only; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public virtual bool IsReadOnly
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value>The item index.</value>
        public virtual T this[int index]
        {
            get { return  this.items[index]; }
            set
            {
                if (!object.Equals(this.items[index], value))
                {
                    object obj2 = this.items[index];
                    this.items[index] = value;
                    if (!this.IsEventSuspended)
                    {
                        this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Replace, value, obj2, index));
                        this.RaisePropertyChanged(this, new PropertyChangedEventArgs("item"));
                    }
                }
            }
        }

        /// <summary>
        /// Gets the items of the collection.
        /// </summary>
        /// <value>The items of the collection.</value>
        protected List<T> Items
        {
            get { return  this.items; }
        }

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        /// <value>The number of elements contained in the collection. The default value is 0.</value>
        [DefaultValue(0)]
        int ICollection.Count
        {
            get { return  this.Count; }
        }

        /// <summary>
        /// Gets a value that indicates whether access to the collection is synchronized (thread safe).
        /// </summary>
        /// <value>
        /// <c>true</c> if access to the collection is synchronized (thread safe); otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        bool ICollection.IsSynchronized
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the collection. The default value is null.</value>
        [DefaultValue((string) null)]
        object ICollection.SyncRoot
        {
            get { return  null; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection has a fixed size.
        /// </summary>
        /// <value>
        /// <c>true</c> if the collection has a fixed size; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        bool IList.IsFixedSize
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection is read-only.
        /// </summary>
        /// <value>
        /// <c>true</c> if the collection is read-only; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        bool IList.IsReadOnly
        {
            get { return  this.IsReadOnly; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index in the collection.
        /// </summary>
        /// <value>The item index in the collection.</value>
        object IList.this[int index]
        {
            get { return  this[index]; }
            set
            {
                if (value is T)
                {
                    this[index] = (T) value;
                }
                else
                {
                    this[index] = default(T);
                }
            }
        }
    }
}

