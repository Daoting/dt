#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class NameInfoCollection : IList<NameInfo>, ICollection<NameInfo>, IEnumerable<NameInfo>, IEnumerable
    {
        /// <summary>
        /// the names.
        /// </summary>
        IDictionary<string, NameInfo> names = ((IDictionary<string, NameInfo>) new Dictionary<string, NameInfo>(new NameKeyComparer()));

        internal event EventHandler<NameInfoCollectionChangedEventArgs> Changed;

        /// <summary>
        /// Adds a specified name information object to the NameInfoCollection class.
        /// </summary>
        /// <param name="nameInfo">The name information object to add to the <see cref="T:Dt.Cells.Data.NameInfoCollection" /> class.</param>
        public void Add(NameInfo nameInfo)
        {
            if (nameInfo == null)
            {
                throw new ArgumentNullException("item");
            }
            if (nameInfo.Name == null)
            {
                throw new ArgumentException(ResourceStrings.NamedStyleInfoNameNullError);
            }
            if (!CalcParser.ValidateName(nameInfo.Name))
            {
                throw new ArgumentException(ResourceStrings.NamedStyleInfoInvalidNameError);
            }
            NameInfo info = this.Find(nameInfo.Name);
            if (info != null)
            {
                this.names.Remove(info.Name);
            }
            this.names.Add(nameInfo.Name, nameInfo);
            this.RaiseNameInfoCollectionChanged(nameInfo, NameInfoCollectionChangedAction.Add);
        }

        /// <summary>
        /// Removes all name information from the name information collection.
        /// </summary>
        public void Clear()
        {
            if (this.names != null)
            {
                foreach (KeyValuePair<string, NameInfo> pair in this.names)
                {
                    this.RaiseNameInfoCollectionChanged(pair.Value, NameInfoCollectionChangedAction.Remove);
                }
                this.names.Clear();
            }
        }

        /// <summary>
        /// Determines whether the name information collection contains a specified name information object.
        /// </summary>
        /// <param name="nameInfo">The <see cref="T:Dt.Cells.Data.NameInfo" /> object to locate in the <see cref="T:Dt.Cells.Data.NameInfoCollection" /> class.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="nameInfo" /> is found in the <see cref="T:Dt.Cells.Data.NameInfoCollection" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(NameInfo nameInfo)
        {
            if (nameInfo == null)
            {
                return false;
            }
            if (nameInfo.Name == null)
            {
                return false;
            }
            return this.Contains(nameInfo.Name);
        }

        /// <summary>
        /// Determines whether the name information collection contains a specific name information object.
        /// </summary>
        /// <param name="name">The name of the <see cref="T:Dt.Cells.Data.NameInfo" /> object to locate in the <see cref="T:Dt.Cells.Data.NameInfoCollection" />.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="name" /> is found in the <see cref="T:Dt.Cells.Data.NameInfoCollection" />; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            if (name == null)
            {
                return false;
            }
            return (this.Find(name) != null);
        }

        /// <summary>
        /// Gets the name information for a custom name on this sheet.
        /// </summary>
        /// <param name="name">The custom name to get.</param>
        /// <returns>Returns the NameInfo object.</returns>
        public NameInfo Find(string name)
        {
            NameInfo info;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            if ((this.names != null) && this.names.TryGetValue(name, out info))
            {
                return info;
            }
            return null;
        }

        public string[] GetNames()
        {
            return Enumerable.ToArray<string>((IEnumerable<string>) this.names.Keys);
        }

        void RaiseNameInfoCollectionChanged(NameInfo nameInfo, NameInfoCollectionChangedAction action)
        {
            if (this.Changed != null)
            {
                this.Changed(this, new NameInfoCollectionChangedEventArgs(nameInfo, action));
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the name information collection.
        /// </summary>
        /// <param name="nameInfo">The object to remove from the <see cref="T:Dt.Cells.Data.NameInfoCollection" />.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="nameInfo" /> was successfully removed from the <see cref="T:Dt.Cells.Data.NameInfoCollection" />; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="nameInfo" /> is not found in the original <see cref="T:Dt.Cells.Data.NameInfoCollection" /> object.
        /// </returns>
        public bool Remove(NameInfo nameInfo)
        {
            if (nameInfo == null)
            {
                return false;
            }
            if (nameInfo.Name == null)
            {
                return false;
            }
            return this.Remove(nameInfo.Name);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the name information collection.
        /// </summary>
        /// <param name="name">The string name of the <see cref="T:Dt.Cells.Data.NameInfo" /> object to remove from the <see cref="T:Dt.Cells.Data.NameInfoCollection" />.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="name" /> was successfully removed from the <see cref="T:Dt.Cells.Data.NameInfoCollection" />; otherwise, <c>false</c>. This method also returns <c>false</c> if <paramref name="name" /> is not found in the original <see cref="T:Dt.Cells.Data.NameInfoCollection" /> object.
        /// </returns>
        public bool Remove(string name)
        {
            if (name != null)
            {
                NameInfo nameInfo = this.Find(name);
                if (nameInfo != null)
                {
                    this.names.Remove(nameInfo.Name);
                    this.RaiseNameInfoCollectionChanged(nameInfo, NameInfoCollectionChangedAction.Remove);
                    return true;
                }
            }
            return false;
        }

        void ICollection<NameInfo>.Clear()
        {
            throw new NotSupportedException();
        }

        void ICollection<NameInfo>.CopyTo(NameInfo[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        IEnumerator<NameInfo> IEnumerable<NameInfo>.GetEnumerator()
        {
            return this.names.Values.GetEnumerator();
        }

        int IList<NameInfo>.IndexOf(NameInfo item)
        {
            throw new NotSupportedException();
        }

        void IList<NameInfo>.Insert(int index, NameInfo item)
        {
            throw new NotSupportedException();
        }

        void IList<NameInfo>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="T:Dt.Cells.Data.NameInfoCollection" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.names.Values.GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements in this collection.
        /// </summary>
        /// <value>The number of elements contained in the collection.</value>
        public int Count
        {
            get { return  this.names.Keys.Count; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Cells.Data.NameInfo" /> object at the specified index.
        /// </summary>
        /// <param name="name">The <see cref="T:Dt.Cells.Data.NameInfo" /> name.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.NameInfo" /> at the specified index.</value>
        [DefaultValue((string) null)]
        public NameInfo this[string name]
        {
            get
            {
                if (this.names.ContainsKey(name))
                {
                    return this.names[name];
                }
                return null;
            }
            set
            {
                if (this.names.ContainsKey(name))
                {
                    this.names[name] = value;
                }
                else
                {
                    this.names.Add(name, value);
                }
            }
        }

        /// <summary>
        /// Gets the internal names dictionary.
        /// </summary>
        /// <value>The names.</value>
        internal IDictionary<string, NameInfo> NamesDictionary
        {
            get { return  this.names; }
        }

        [DefaultValue(false)]
        bool ICollection<NameInfo>.IsReadOnly
        {
            get { return  false; }
        }

        NameInfo IList<NameInfo>.this[int index]
        {
            get
            {
                throw new NotSupportedException();
            }
            set { throw new NotSupportedException(); }
        }

        class NameKeyComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return (string.Compare(x, y, (StringComparison) StringComparison.OrdinalIgnoreCase) == 0);
            }

            public int GetHashCode(string obj)
            {
                if (!string.IsNullOrEmpty(obj))
                {
                    return obj.ToUpperInvariant().GetHashCode();
                }
                return 0;
            }
        }
    }
}

