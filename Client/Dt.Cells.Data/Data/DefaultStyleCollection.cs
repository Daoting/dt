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
using System.ComponentModel;
using System.Reflection;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a style collection that contains the default <see cref="T:Dt.Cells.Data.StyleInfo" /> objects for the component.
    /// </summary>
    internal sealed class DefaultStyleCollection : IList, ICollection, IEnumerable
    {
        static StyleInfo cellsDefault;
        static StyleInfo columnFooterDefault;
        static StyleInfo columnHeaderDefault;
        static StyleInfo cornerDefault;
        static FontFamily defaultFontFamily;
        static double defaultFontSize;
        static StyleInfo rowHeaderDefault;
        /// <summary>
        /// Represents the array of StyleInfo objects that contain the default styles.
        /// </summary>
        public static StyleInfo[] styles;

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="value">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to add to the collection.</param>
        public void Add(StyleInfo value)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Composes a style using the default styles and
        /// a specified StyleInfo object.
        /// </summary>
        /// <param name="o">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object, StyleInfo object, or string name).</param>
        /// <param name="destInfo">The destination for the composed StyleInfo object, or creates a new StyleInfo object if null.</param>
        /// <returns>The cell style.</returns>
        public StyleInfo Compose(object o, StyleInfo destInfo)
        {
            StyleInfo source = null;
            if (destInfo == null)
            {
                destInfo = new StyleInfo();
            }
            if (o is string)
            {
                source = this.Find((string) ((string) o));
            }
            else if (o is StyleInfo)
            {
                source = (StyleInfo) o;
            }
            destInfo.Compose(source);
            while (((source != null) && (source.Parent != null)) && (source.Parent.Length > 0))
            {
                source = this.Find(source.Parent);
                destInfo.Merge(source);
            }
            return destInfo;
        }

        /// <summary>
        /// Determines whether the collection contains the specified style.
        /// </summary>
        /// <param name="style">The style information.</param>
        /// <returns>
        /// <c>true</c> if the collection contains the specified style; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(StyleInfo style)
        {
            return (this.IndexOf(style) > -1);
        }

        /// <summary>
        /// Copies the styles in the collection to a specified array at a specified location. 
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array into which the elements from this collection are copied. 
        /// The array indexing must be zero-based. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in the array at which to paste the styles.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">No array specified, or the specified array is null (Nothing).</exception>
        /// <exception cref="T:System.ArgumentException">The specified array is invalid; must have a rank of one.</exception>
        /// <exception cref="T:System.ArgumentException">The specified array is invalid; must have sufficient length.</exception>
        /// <exception cref="T:System.IndexOutOfRangeException">The specified index is out of range; must be greater than zero.</exception>
        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (array.Rank != 1)
            {
                throw new ArgumentException(string.Format(ResourceStrings.StyleInfoCopyToArrayRankGreaterThanOneError, (object[]) new object[] { ((int) array.Rank).ToString() }));
            }
            if ((index >= array.Length) || (Styles.Length > (array.Length - index)))
            {
                object[] args = new object[2];
                args[0] = ((int) array.Length).ToString();
                int num3 = Styles.Length + index;
                args[1] = ((int) num3).ToString();
                throw new ArgumentException(string.Format(ResourceStrings.StyleInfoCopyToArrayLengthError, args));
            }
            if (index < 0)
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.StyleInfoOperationIndexOutOfRangeError, (object[]) new object[] { ((int) index) }));
            }
            if (Styles != null)
            {
                try
                {
                    Array.Copy(Styles, 0, array, index, Styles.Length);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Copies the styles in the collection to a specified array at a specified location. 
        /// </summary>
        /// <param name="array">
        /// The one-dimensional array into which the elements from this collection are copied. 
        /// The array indexing must be zero-based.
        /// </param>
        /// <param name="index">
        /// The zero-based index in the array at which to paste the styles.
        /// </param>
        public void CopyTo(StyleInfo[] array, int index)
        {
            this.CopyTo((Array) array, index);
        }

        /// <summary>
        /// Finds a style with the specified name in the collection.
        /// </summary>
        /// <param name="name">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to find.</param>
        /// <returns>The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) in the collection with the specified name, or
        /// null if no StyleInfo object is found.
        /// </returns>
        public StyleInfo Find(string name)
        {
            if (name != null)
            {
                foreach (StyleInfo info in this)
                {
                    if (info.Name.Equals(name))
                    {
                        return info;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Finds the specified name.
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns></returns>
        internal static StyleInfo FindDefaultStyle(string name)
        {
            foreach (StyleInfo info in Styles)
            {
                if (info.Name == name)
                {
                    return info;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets an IEnumerator object for enumerating through the <see cref="T:Dt.Cells.Data.StyleInfo" /> objects
        /// in the collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object for enumerating the styles in the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return Styles.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of the specified StyleInfo object.
        /// </summary>
        /// <param name="style">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) for which to search.</param>
        /// <returns>The style index.</returns>
        public int IndexOf(StyleInfo style)
        {
            int count = this.Count;
            for (int i = 0; i < count; i++)
            {
                if (Styles[i].Equals(style))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="index">The index at which to insert the style.</param>
        /// <param name="style">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to insert in the collection.</param>
        public void Insert(int index, StyleInfo style)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="style">The style (<see cref="T:Dt.Cells.Data.StyleInfo" /> object) to remove from the collection.</param>
        public void Remove(StyleInfo style)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />.</param>
        /// <returns>
        /// The position into which the new element was inserted.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. -or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        int IList.Add(object value)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        void IList.Clear()
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Determines whether the collection contains the specified StyleInfo object.
        /// </summary>
        /// <param name="value">
        /// StyleInfo object to check for in the collection.
        /// </param>
        /// <returns>
        /// True if the object is found in the collection; false otherwise.
        /// </returns>
        bool IList.Contains(object value)
        {
            return ((value is StyleInfo) && this.Contains((StyleInfo) value));
        }

        /// <summary>
        /// Returns the index of the specified style.
        /// </summary>
        /// <param name="value">Style (StyleInfo object) for which to search.</param>
        /// <returns>
        /// Index of the object in the collection, or -1 if the object was not found.
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
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="index">The zero-based index at which the <paramref name="value" /> should be inserted.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to insert into the <see cref="T:System.Collections.IList" />.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. -or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        /// <exception cref="T:System.NullReferenceException">
        /// <paramref name="value" /> is null reference in the <see cref="T:System.Collections.IList" />.</exception>
        void IList.Insert(int index, object value)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="value">The <see cref="T:System.Object" /> to remove from the <see cref="T:System.Collections.IList" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. -or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        void IList.Remove(object value)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Not supported; calling this method throws an exception because the collection is read-only.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> is not a valid index in the <see cref="T:System.Collections.IList" />. </exception>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IList" /> is read-only. -or- The <see cref="T:System.Collections.IList" /> has a fixed size. </exception>
        void IList.RemoveAt(int index)
        {
            throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError);
        }

        /// <summary>
        /// Gets the cells default style.
        /// </summary>
        /// <value>The cells default style.</value>
        public static StyleInfo CellsDefault
        {
            get
            {
                if (cellsDefault == null)
                {
                    cellsDefault = new StyleInfo("CellsDefault");
                }
                return cellsDefault;
            }
        }

        /// <summary>
        /// Gets the column footer default style.
        /// </summary>
        /// <value>The column footer default style</value>
        internal static StyleInfo ColumnFooterDefault
        {
            get
            {
                if (columnFooterDefault == null)
                {
                    columnFooterDefault = new StyleInfo("ColumnFooterDefault");
                    columnFooterDefault.Foreground = new SolidColorBrush(Colors.Black);
                    columnFooterDefault.HorizontalAlignment = CellHorizontalAlignment.Center;
                    columnFooterDefault.VerticalAlignment = CellVerticalAlignment.Center;
                }
                return columnFooterDefault;
            }
        }

        /// <summary>
        /// Gets the column header default style.
        /// </summary>
        /// <value>The column header default style.</value>
        public static StyleInfo ColumnHeaderDefault
        {
            get
            {
                if (columnHeaderDefault == null)
                {
                    columnHeaderDefault = new StyleInfo("ColumnHeaderDefault");
                    columnHeaderDefault.Foreground = new SolidColorBrush(Colors.Black);
                    columnHeaderDefault.HorizontalAlignment = CellHorizontalAlignment.Center;
                    columnHeaderDefault.VerticalAlignment = CellVerticalAlignment.Center;
                }
                return columnHeaderDefault;
            }
        }

        /// <summary>
        /// Gets the default corner style.
        /// </summary>
        /// <value>The default corner style.</value>
        public static StyleInfo CornerDefault
        {
            get
            {
                if (cornerDefault == null)
                {
                    cornerDefault = new StyleInfo("CornerDefault");
                    cornerDefault.Foreground = new SolidColorBrush(Colors.Black);
                }
                return cornerDefault;
            }
        }

        /// <summary>
        /// Gets the number of StyleInfo objects in the collection.
        /// </summary>
        [DefaultValue(0)]
        public int Count
        {
            get { return  Styles.Length; }
        }

        internal static FontFamily DefaultFontFamily
        {
            get
            {
                if (defaultFontFamily == null)
                    defaultFontFamily = new FontFamily(NameConstans.DEFAULT_FONT_FAMILY);
                return defaultFontFamily;
            }
        }

        internal static string DefaultFontName
        {
            get
            {
                return DefaultFontFamily.Source;
            }
        }

        internal static double DefaultFontSize
        {
            get
            {
                if (defaultFontSize == 0.0)
                    defaultFontSize = 15d;
                return defaultFontSize;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection has a fixed size. 
        /// <para>This implementation always returns false.</para>
        /// </summary>
        [DefaultValue(false)]
        public bool IsFixedSize
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets a value that indicates whether the collection is read-only.
        /// <para>This implementation always returns true.</para>
        /// </summary>
        [DefaultValue(false)]
        public bool IsReadOnly
        {
            get { return  true; }
        }

        /// <summary>
        /// Gets a value that indicates whether access to the collection is synchronized.
        /// <para>This implementation always returns false.</para>
        /// </summary>
        [DefaultValue(false)]
        public bool IsSynchronized
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Cells.Data.StyleInfo" /> object at the specified index.
        /// </summary>
        /// <param name="index">The style index.</param>
        /// <value>The <see cref="T:Dt.Cells.Data.StyleInfo" /> object at the specified index.</value>
        public StyleInfo this[int index]
        {
            get
            {
                if ((0 > index) || (index >= this.Count))
                {
                    throw new IndexOutOfRangeException("index");
                }
                return Styles[index];
            }
            set { throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError); }
        }

        /// <summary>
        /// Gets the row header default style.
        /// </summary>
        /// <value>The row header default style.</value>
        public static StyleInfo RowHeaderDefault
        {
            get
            {
                if (rowHeaderDefault == null)
                {
                    rowHeaderDefault = new StyleInfo("RowHeaderDefault");
                    rowHeaderDefault.Foreground = new SolidColorBrush(Colors.Black);
                    rowHeaderDefault.HorizontalAlignment = CellHorizontalAlignment.Center;
                    rowHeaderDefault.VerticalAlignment = CellVerticalAlignment.Center;
                }
                return rowHeaderDefault;
            }
        }

        /// <summary>
        /// Represents the array of StyleInfo objects that contain the default styles.
        /// </summary>
        /// <value>The array of StyleInfo objects that contain the default styles.</value>
        public static StyleInfo[] Styles
        {
            get
            {
                if (styles == null)
                {
                    styles = new StyleInfo[] { CellsDefault, ColumnHeaderDefault, ColumnFooterDefault, RowHeaderDefault, CornerDefault };
                }
                return styles;
            }
        }

        /// <summary>
        /// Gets a NamedStyleCollection object that can be used to 
        /// synchronize access to the collection. 
        /// </summary>
        public DefaultStyleCollection SyncRoot
        {
            get { return  this; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        object ICollection.SyncRoot
        {
            get { return  this; }
        }

        /// <summary>
        /// Gets or sets the StyleInfo object at the specified index. 
        /// </summary>
        /// <param name="index">The style index.</param>
        object IList.this[int index]
        {
            get { return  this[index]; }
            set { throw new InvalidOperationException(ResourceStrings.StyleInfoChangeReadOnlyCollectionError); }
        }
    }
}

