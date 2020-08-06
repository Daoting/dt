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
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a collection of custom styles (StyleInfo objects).
    /// </summary>
    public class StyleInfoCollection : IList, ICollection, IEnumerable, IXmlSerializable
    {
        /// <summary>
        /// Default name number.
        /// </summary>
        int defaultNameNumber = 1;
        /// <summary>
        /// Default Name Prefix.
        /// </summary>
        static string defaultNamePrefix = "Style";
        /// <summary>
        /// Represents the read-only collection of default styles.
        /// </summary>
        internal static DefaultStyleCollection defaultStyles = null;
        /// <summary>
        /// The name in map.
        /// </summary>
        Dictionary<string, int> nameIndexLookupTable = new Dictionary<string, int>();
        /// <summary>
        /// the owner of the current CellStyleCollection
        /// </summary>
        object owner;
        /// <summary>
        /// The style version.
        /// </summary>
        static long styleInfoVersion = 0L;
        /// <summary>
        /// The style collection.
        /// </summary>
        SparseArray<object> styles = null;

        /// <summary>
        /// Occurs when a style is added, removed, or changed in the collection.
        /// </summary>
        public event EventHandler<StyleInfoCollectionChangedEventArgs> Changed;

        /// <summary>
        /// Creates a custom style collection (NamedStyleCollection object).
        /// </summary>
        public StyleInfoCollection()
        {
            this.nameIndexLookupTable.Clear();
        }

        /// <summary>
        /// Adds a style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to the collection.
        /// </summary>
        /// <param name="style">The style to add.</param>
        /// <returns>The number of styles in this collection.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// No style specified, or the specified style is null.
        /// </exception>
        public virtual int Add(StyleInfo style)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            StyleInfo info = this.Find(style.Name);
            if (info != null)
            {
                this.Remove(info);
            }
            int count = this.Count;
            if (this.styles == null)
            {
                this.styles = new SparseArray<object>(10);
            }
            else if (count == this.styles.Length)
            {
                this.styles.Length += 10;
            }
            if ((style.Name == null) || (style.Name.Length == 0))
            {
                style.Name = this.GetDefaultName();
            }
            this.styles[count] = style;
            this.nameIndexLookupTable.Add(style.Name, count);
            style.PropertyChanged += new PropertyChangedEventHandler(this.OnStyleInfoPropertyChanged);
            this.RaiseItemAdded(style);
            return count;
        }

        /// <summary>
        /// Adds a style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to the collection as a child
        /// of the specified parent.
        /// </summary>
        /// <param name="name">The name of the style to add.</param>
        /// <param name="parentName">The parent style to add.</param>
        /// <returns>The cell style.</returns>
        public virtual StyleInfo Add(string name, string parentName)
        {
            StyleInfo style = this.Find(name);
            if (style != null)
            {
                this.Remove(style);
            }
            StyleInfo info2 = new StyleInfo(name, parentName);
            if ((info2.Name == null) || (info2.Name.Length == 0))
            {
                info2.Name = this.GetDefaultName();
            }
            this.Add(info2);
            return info2;
        }

        /// <summary>
        /// Adds an array of styles (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to the collection.
        /// </summary>
        /// <param name="c">The <see cref="T:Dt.Cells.Data.StyleInfoCollection" /> or array of StyleInfo objects to add to the collection.</param>
        public virtual void AddRange(ICollection c)
        {
            foreach (object obj2 in c)
            {
                if (obj2 is StyleInfo)
                {
                    this.Add((StyleInfo) obj2);
                }
            }
        }

        /// <summary>
        /// Adds an array of <see cref="T:Dt.Cells.Data.StyleInfo" /> objects to the collection. 
        /// </summary>
        /// <param name="array">The array of styles (<see cref="T:Dt.Cells.Data.StyleInfo" /> objects) to add to the collection.</param>
        public virtual void AddRange(StyleInfo[] array)
        {
            foreach (StyleInfo info in array)
            {
                this.Add(info);
            }
        }

        /// <summary>
        /// Removes all named styles from the collection. 
        /// </summary>
        public virtual void Clear()
        {
            SparseArray<object> styles = this.styles;
            int count = this.Count;
            this.styles = null;
            this.nameIndexLookupTable.Clear();
            for (int i = 0; i < count; i++)
            {
                StyleInfo style = (StyleInfo) styles[i];
                style.PropertyChanged -= new PropertyChangedEventHandler(this.OnStyleInfoPropertyChanged);
                this.RaiseItemRemoved(style);
            }
        }

        /// <summary>
        /// Determines whether the collection contains the specified style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object).
        /// </summary>
        /// <param name="style">Style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) for which to check in the collection.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the specified style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object); otherwise, <c>false</c>.
        /// </returns>
        public virtual bool Contains(StyleInfo style)
        {
            return (this.IndexOf(style) > -1);
        }

        /// <summary>
        /// Copies the styles in the collection to a specified array at a specified position.
        /// </summary>
        /// <param name="namedStyles">The one-dimensional array into which the elements from ICollection are copied. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in the array at which to paste styles.</param>
        public virtual void CopyTo(StyleInfo[] namedStyles, int index)
        {
            this.CopyTo((Array) namedStyles, index);
        }

        /// <summary>
        /// Copies the styles in the collection to a specified array at a specified position.
        /// </summary>
        /// <param name="array">The one-dimensional array into which the elements from ICollection are copied. The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in the array at which to paste styles.</param>
        /// <exception cref="T:System.ArgumentNullException">No array specified, or the specified array is null (Nothing).</exception>
        /// <exception cref="T:System.ArgumentException">The specified array is not valid; must have a rank of one.</exception>
        /// <exception cref="T:System.ArgumentException">The specified array is not valid; must have sufficient length.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">The specified index is out of range; must be greater than zero.</exception>
        public virtual void CopyTo(Array array, int index)
        {
            if (this.styles != null)
            {
                if (array == null)
                {
                    throw new ArgumentNullException(ResourceStrings.StyleInfoCopyToDestionationNullError);
                }
                if (array.Rank != 1)
                {
                    throw new ArgumentException(string.Format(ResourceStrings.StyleInfoCopyToArrayRankGreaterThanOneError, (object[]) new object[] { ((int) array.Rank).ToString() }));
                }
                if ((index >= array.Length) || (this.styles.DataLength > (array.Length - index)))
                {
                    object[] args = new object[2];
                    args[0] = ((int) array.Length).ToString();
                    int num5 = this.styles.DataLength + index;
                    args[1] = ((int) num5).ToString();
                    throw new ArgumentException(string.Format(ResourceStrings.StyleInfoCopyToArrayLengthError, args));
                }
                if (index < 0)
                {
                    throw new IndexOutOfRangeException(string.Format(ResourceStrings.StyleInfoOperationIndexOutOfRangeError, (object[]) new object[] { ((int) index) }));
                }
                try
                {
                    int dataLength = this.styles.DataLength;
                    for (int i = 0; i < dataLength; i++)
                    {
                        array.SetValue(this.styles[i], new int[] { index + i });
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Finds a style <see cref="T:Dt.Cells.Data.StyleInfo" /> object in the collection with the specified name.
        /// </summary>
        /// <param name="name">The name of the style to find.</param>
        /// <returns>The cell style.</returns>
        public virtual StyleInfo Find(string name)
        {
            if ((name != null) && (name.Length > 0))
            {
                foreach (StyleInfo info in DefaultStyles)
                {
                    if (info.Name.Equals(name))
                    {
                        return info;
                    }
                }
                StyleInfo info2 = this[name];
                if (info2 != null)
                {
                    return info2;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the default name.
        /// </summary>
        /// <returns>Default name string</returns>
        string GetDefaultName()
        {
            string str;
            do
            {
                str = defaultNamePrefix + ((int) this.defaultNameNumber).ToString();
                this.defaultNameNumber++;
            }
            while (this.Find(str) != null);
            return str;
        }

        /// <summary>
        /// Gets an IEnumerator object for enumerating through the <see cref="T:Dt.Cells.Data.StyleInfo" /> objects
        /// in the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object for enumerating the <see cref="T:Dt.Cells.Data.StyleInfo" /> objects in the collection.
        /// </returns>
        public virtual IEnumerator GetEnumerator()
        {
            if (this.styles != null)
            {
                return (IEnumerator) this.styles.GetEnumerator();
            }
            return new StyleInfo[0].GetEnumerator();
        }

        /// <summary>
        /// Increases the style info version.
        /// </summary>
        internal static void IncreaseStyleInfoVersion()
        {
            styleInfoVersion += 1L;
        }

        /// <summary>
        /// Returns the index of the specified style in the collection.
        /// </summary>
        /// <param name="style">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) for which to search.</param>
        /// <returns>The style index.</returns>
        public virtual int IndexOf(StyleInfo style)
        {
            if (this.styles == null)
            {
                return -1;
            }
            try
            {
                if (this.nameIndexLookupTable.ContainsKey(style.Name))
                {
                    return this.nameIndexLookupTable[style.Name];
                }
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Inserts a style into the collection at the specified index.
        /// </summary>
        /// <param name="index">Zero-based index at which to insert the named style.</param>
        /// <param name="style">Style to insert into the collection.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified index is out of range; must be between 0 and the total number in the collection.
        /// </exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// No style specified, or the specified style is null.
        /// </exception>
        public virtual void Insert(int index, StyleInfo style)
        {
            if (style == null)
            {
                throw new ArgumentNullException("style");
            }
            StyleInfo info = this.Find(style.Name);
            if (info != null)
            {
                this.Remove(info);
            }
            int count = this.Count;
            if ((0 > index) || (index > count))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.StyleInfoOperationIndexOutOfRangeWithAllowedRangeBehind, (object[]) new object[] { ((int) index), ((int) this.Count) }));
            }
            if (this.styles == null)
            {
                this.styles = new SparseArray<object>(10);
            }
            this.styles.InsertRange(index, 1);
            if ((style.Name == null) || (style.Name.Length == 0))
            {
                style.Name = this.GetDefaultName();
            }
            this.styles[index] = style;
            string[] strArray = new string[this.nameIndexLookupTable.Count];
            this.nameIndexLookupTable.Keys.CopyTo(strArray, 0);
            foreach (string str in strArray)
            {
                if (this.nameIndexLookupTable[str] >= index)
                {
                    this.nameIndexLookupTable[str] = this.nameIndexLookupTable[str] + 1;
                }
            }
            style.PropertyChanged += new PropertyChangedEventHandler(this.OnStyleInfoPropertyChanged);
            this.RaiseItemAdded(style);
            this.nameIndexLookupTable.Add(style.Name, index);
        }

        /// <summary>
        /// Raises the Changed event.
        /// </summary>
        protected virtual void OnChanged(StyleInfoCollectionChangedEventArgs e)
        {
            IncreaseStyleInfoVersion();
            if (this.Changed != null)
            {
                this.Changed(this, e);
            }
        }

        void OnStyleInfoPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaiseItemChanged(sender as StyleInfo, sender as StyleInfo);
        }

        /// <summary>
        /// Calls OnChanged to raise an ItemAdded event for the specified StyleInfo object.
        /// </summary>
        /// <param name="style">
        /// New StyleInfo object
        /// </param>
        void RaiseItemAdded(StyleInfo style)
        {
            this.OnChanged(new StyleInfoCollectionChangedEventArgs(StyleInfoCollectionChangedAction.ItemAdded, null, style));
        }

        /// <summary>
        /// Calls OnChanged to raise an ItemChanged event with the specified arguments.
        /// </summary>
        /// <param name="oldstyle">
        /// Old StyleInfo object
        /// </param>
        /// <param name="newstyle">
        /// New StyleInfo object
        /// </param>
        void RaiseItemChanged(StyleInfo oldstyle, StyleInfo newstyle)
        {
            this.OnChanged(new StyleInfoCollectionChangedEventArgs(StyleInfoCollectionChangedAction.ItemChanged, oldstyle, newstyle));
        }

        /// <summary>
        /// Calls OnChanged to raise an ItemRemoved event for the specified StyleInfo object.
        /// </summary>
        /// <param name="style">
        /// Old StyleInfo object
        /// </param>
        void RaiseItemRemoved(StyleInfo style)
        {
            this.OnChanged(new StyleInfoCollectionChangedEventArgs(StyleInfoCollectionChangedAction.ItemRemoved, style, null));
        }

        /// <summary>
        /// Removes the specified named style from the collection.
        /// </summary>
        /// <param name="style">Style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to remove.</param>
        public virtual void Remove(StyleInfo style)
        {
            int index = this.IndexOf(style);
            if (index > -1)
            {
                this.RemoveAt(index);
            }
        }

        /// <summary>
        /// Removes a named style (StyleInfo object) from the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the style to be removed.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified index is out of range; must be between 0 and the total number in the collection.
        /// </exception>
        public virtual void RemoveAt(int index)
        {
            if ((0 > index) || (index > this.Count))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.StyleInfoOperationIndexOutOfRangeWithAllowedRangeBehind, (object[]) new object[] { ((int) index), ((int) this.Count) }));
            }
            StyleInfo style = (StyleInfo) this.styles[index];
            style.PropertyChanged -= new PropertyChangedEventHandler(this.OnStyleInfoPropertyChanged);
            this.styles.RemoveRange(index, 1);
            this.RaiseItemRemoved(style);
            this.nameIndexLookupTable.Clear();
            for (int i = 0; i < this.styles.Length; i++)
            {
                if (this.styles[i] != null)
                {
                    string name = ((StyleInfo) this.styles[i]).Name;
                    if ((name != null) && (name.Length > 0))
                    {
                        this.nameIndexLookupTable.Add(name, i);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a <see cref="T:Dt.Cells.Data.StyleInfo" /> object to the collection.
        /// </summary>
        /// <param name="value">Specifies the <see cref="T:Dt.Cells.Data.StyleInfo" /> object to add.</param>
        /// <returns>
        /// The index of the element in the collection.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.IList" /> is read-only.
        /// -or-
        /// The <see cref="T:System.Collections.IList" /> has a fixed size.
        /// </exception>
        int IList.Add(object value)
        {
            if (!(value is StyleInfo))
            {
                throw new ArgumentException(string.Format(ResourceStrings.SetOtherTypeToStyleInfoCollectionError, (object[]) new object[] { value.GetType().ToString() }));
            }
            return this.Add((StyleInfo) value);
        }

        /// <summary>
        /// Determines whether the collection contains the specified <see cref="T:Dt.Cells.Data.StyleInfo" /> object.
        /// </summary>
        /// <param name="value">The <see cref="T:Dt.Cells.Data.StyleInfo" /> object to check for in the collection.</param>
        /// <returns>
        /// <c>true</c> if the object is found in the collection; otherwise <c>false</c>.
        /// </returns>
        bool IList.Contains(object value)
        {
            return ((value is StyleInfo) && this.Contains((StyleInfo) value));
        }

        /// <summary>
        /// Returns the index of the specified <see cref="T:Dt.Cells.Data.StyleInfo" /> object.
        /// </summary>
        /// <param name="value">The <see cref="T:Dt.Cells.Data.StyleInfo" /> object for which to search.</param>
        /// <returns>
        /// The index of the object in the collection, or -1 if the object was not found.
        /// </returns>
        int IList.IndexOf(object value)
        {
            if (value is StyleInfo)
            {
                return this.IndexOf((StyleInfo) value);
            }
            return -1;
        }

        /// <summary>
        /// Inserts a style into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which to insert the style.</param>
        /// <param name="value">The style to insert into the collection.</param>
        /// <exception cref="T:System.ArgumentException">The specified object is not valid.</exception>
        void IList.Insert(int index, object value)
        {
            if (!(value is StyleInfo))
            {
                throw new ArgumentException(string.Format(ResourceStrings.SetOtherTypeToStyleInfoCollectionError, (object[]) new object[] { value.GetType().ToString() }));
            }
            this.Insert(index, (StyleInfo) value);
        }

        /// <summary>
        /// Removes the specified <see cref="T:Dt.Cells.Data.StyleInfo" /> object from the collection.
        /// </summary>
        /// <param name="value">The specified <see cref="T:Dt.Cells.Data.StyleInfo" /> object to remove.</param>
        /// <exception cref="T:System.ArgumentException">The specified object is not valid.</exception>
        void IList.Remove(object value)
        {
            if (!(value is StyleInfo))
            {
                throw new ArgumentException(string.Format(ResourceStrings.SetOtherTypeToStyleInfoCollectionError, (object[]) new object[] { value.GetType().ToString() }));
            }
            this.Remove((StyleInfo) value);
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
            Serializer.InitReader(reader);
            this.styles = null;
            SparseArray<StyleInfo> array = new SparseArray<StyleInfo>();
            Serializer.DeserializeArray<StyleInfo>(array, typeof(StyleInfo), reader);
            if (array.DataLength > 0)
            {
                IEnumerator enumerator = (IEnumerator) array.GetEnumerator();
                if (enumerator != null)
                {
                    while (enumerator.MoveNext())
                    {
                        this.Add(enumerator.Current as StyleInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.styles != null)
            {
                Serializer.SerializeObj(this.styles, null, writer);
            }
        }

        /// <summary>
        /// Gets the number of styles (StyleInfo objects) in the collection.
        /// </summary>
        /// <value>The number of styles (StyleInfo objects) in the collection.</value>
        [DefaultValue(0)]
        public virtual int Count
        {
            get
            {
                if (this.styles == null)
                {
                    return 0;
                }
                return this.styles.DataLength;
            }
        }

        /// <summary>
        /// Gets the default style collection.
        /// </summary>
        /// <value>The default style collection</value>
        internal static DefaultStyleCollection DefaultStyles
        {
            get
            {
                if (defaultStyles == null)
                {
                    defaultStyles = new DefaultStyleCollection();
                }
                return defaultStyles;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection has a fixed size  
        /// (this implementation always returns false).
        /// </summary>
        [DefaultValue(false)]
        public bool IsFixedSize
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection is read-only 
        /// (this implementation always returns false).
        /// </summary>
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates whether access to the collection is synchronized 
        /// (this implementation always returns false).
        /// </summary>
        public virtual bool IsSynchronized
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets or sets the style (StyleInfo object) at the specified index in the collection.
        /// </summary>
        /// <value>Returns the cell styles.</value>
        /// <param name="index">The style index.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">The specified index is not valid.</exception>
        /// <exception cref="T:System.ArgumentNullException">No value specified; cannot set a member to null.</exception>
        public virtual StyleInfo this[int index]
        {
            get
            {
                if ((0 > index) || (index >= this.Count))
                {
                    throw new IndexOutOfRangeException("index");
                }
                return (StyleInfo) this.styles[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", ResourceStrings.CannotSetNullToStyleInfo);
                }
                if ((0 > index) || (index >= this.Count))
                {
                    throw new IndexOutOfRangeException("index");
                }
                StyleInfo style = this.Find(value.Name);
                int num = this.IndexOf(style);
                if ((style != null) && (num != index))
                {
                    if (index >= (this.Count - 1))
                    {
                        throw new IndexOutOfRangeException();
                    }
                    this.Remove(style);
                }
                if (this.styles == null)
                {
                    this.styles = new SparseArray<object>(10);
                }
                if (num != index)
                {
                    style = (StyleInfo) this.styles[index];
                }
                this.styles[index] = value;
                this.nameIndexLookupTable.Add(value.Name, index);
                if ((value.Name == null) || (value.Name.Length == 0))
                {
                    value.Name = this.GetDefaultName();
                }
                this.RaiseItemChanged(style, value);
            }
        }

        StyleInfo this[string name]
        {
            get
            {
                try
                {
                    if (this.nameIndexLookupTable.ContainsKey(name))
                    {
                        return this[this.nameIndexLookupTable[name]];
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner</value>
        internal object Owner
        {
            get { return  this.owner; }
            set { this.owner = value; }
        }

        /// <summary>
        /// Gets the style info version.
        /// </summary>
        /// <value>The style info version</value>
        /// <remarks>
        /// The version number will be increased when any style changes.
        /// </remarks>
        internal static long StyleInfoVersion
        {
            get { return  styleInfoVersion; }
        }

        /// <summary>
        /// Gets a collection (NamedStyleCollection object) that can be used to 
        /// synchronize access. 
        /// </summary>
        internal virtual StyleInfoCollection SyncRoot
        {
            get { return  this; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <value>An object that can be used to synchronize access to the collection.</value>
        /// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.</returns>
        object ICollection.SyncRoot
        {
            get { return  this; }
        }

        /// <summary>
        /// Gets or sets the style (StyleInfo object) at the specified index.
        /// </summary>
        /// <value>The style (StyleInfo object) at the specified index.</value>
        /// <param name="index">The style index.</param>
        /// <exception cref="T:System.ArgumentException">The specified object type is not valid; must be a StyleInfo object.</exception>
        object IList.this[int index]
        {
            get { return  this[index]; }
            set
            {
                if (!(value is StyleInfo))
                {
                    throw new ArgumentException(string.Format(ResourceStrings.SetOtherTypeToStyleInfoCollectionError, (object[]) new object[] { value.GetType().ToString() }));
                }
                this[index] = (StyleInfo) value;
            }
        }
    }
}

