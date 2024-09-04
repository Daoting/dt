#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#endregion

namespace Dt.Xls.Biff
{
    /// <summary>
    /// Represents the directory entry collection.
    /// </summary>
    internal class DirectoryEntryCollection : ICollection<DirectoryEntry>, IEnumerable<DirectoryEntry>, IEnumerable
    {
        /// <summary>
        /// Represents a list of directory entries.
        /// </summary>
        private List<DirectoryEntry> _directoryEntries = new List<DirectoryEntry>();

        /// <summary>
        /// Adds the specified directory entry.
        /// </summary>
        /// <param name="directoryEntry">The directory entry.</param>
        public void Add(DirectoryEntry directoryEntry)
        {
            if (directoryEntry.Name == "Root Entry")
            {
                this._directoryEntries.Insert(0, directoryEntry);
            }
            else
            {
                this._directoryEntries.Add(directoryEntry);
            }
        }

        /// <summary>
        /// Adds the <see cref="T:Dt.Xls.Biff.DirectoryEntry" /> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the new added directory entry.</returns>
        public DirectoryEntry Add(string name)
        {
            DirectoryEntry entry = new DirectoryEntry {
                Name = name
            };
            if (name == "Root Entry")
            {
                this._directoryEntries.Insert(0, entry);
                return entry;
            }
            this._directoryEntries.Add(entry);
            return entry;
        }

        /// <summary>
        /// Clear all <see cref="T:Dt.Xls.Biff.DirectoryEntry" />s in the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />.
        /// </summary>
        public void Clear()
        {
            this._directoryEntries.Clear();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned directory entry collection.</returns>
        public DirectoryEntryCollection Clone()
        {
            DirectoryEntryCollection entrys = new DirectoryEntryCollection {
                _directoryEntries = new List<DirectoryEntry>()
            };
            foreach (DirectoryEntry entry in this._directoryEntries)
            {
                entrys.Add(entry.Clone());
            }
            return entrys;
        }

        /// <summary>
        /// Determines whether the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />; otherwise, false.
        /// </returns>
        public bool Contains(DirectoryEntry item)
        {
            return this._directoryEntries.Contains(item);
        }

        /// <summary>
        /// Determines whether the specified directory entry exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        /// <see langword="true" /> if exists; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(string name)
        {
            return (this.IndexOf(name) >= 0);
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        public void CopyTo(DirectoryEntry[] array, int arrayIndex)
        {
            this._directoryEntries.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<DirectoryEntry> GetEnumerator()
        {
            return (IEnumerator<DirectoryEntry>) this._directoryEntries.GetEnumerator();
        }

        /// <summary>
        /// Finds the specified directory entry by its name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Returns the directory entry.</returns>
        public int IndexOf(string name)
        {
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                if (name.Trim() == this._directoryEntries[i].Name)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Reads the directory entries from the input stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="count">The count.</param>
        public void Read(BinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DirectoryEntry entry = new DirectoryEntry();
                entry.Read(reader);
                this._directoryEntries.Add(entry);
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" />.
        /// </returns>
        public bool Remove(DirectoryEntry item)
        {
            return this._directoryEntries.Remove(item);
        }

        /// <summary>
        /// Removes the specified directory entry by name.
        /// </summary>
        /// <param name="name">The name of removing directory entry.</param>
        /// <returns><b>true</b> if the directory entries are removed, otherwise <b>false</b>.</returns>
        public bool Remove(string name)
        {
            bool flag = false;
            int num = 0;
            while (num < this._directoryEntries.Count)
            {
                if (this._directoryEntries[num].Name == name)
                {
                    this._directoryEntries.RemoveAt(num);
                    flag = true;
                }
                else
                {
                    num++;
                }
            }
            return flag;
        }

        /// <summary>
        /// Removes the directory entry at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            this._directoryEntries.RemoveAt(index);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._directoryEntries.GetEnumerator();
        }

        /// <summary>
        /// Writes the directory entries to the output stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        public void Write(BinaryWriter writer, int startIndex, int count)
        {
            for (int i = startIndex; i < (startIndex + count); i++)
            {
                this._directoryEntries[i].Write(writer);
            }
        }

        /// <summary>
        /// Gets the count of directory entries.
        /// </summary>
        public int Count
        {
            get { return  this._directoryEntries.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:Dt.Xls.Biff.DirectoryEntryCollection" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets the <see cref="T:Dt.Xls.Biff.DirectoryEntry" /> at the specified index.
        /// </summary>
        /// <value>The directory entry.</value>
        public DirectoryEntry this[int index]
        {
            get { return  this._directoryEntries[index]; }
        }

        /// <summary>
        /// Gets the <see cref="T:Dt.Xls.Biff.DirectoryEntry" /> with the specified name.
        /// </summary>
        /// <value>The directory entry.</value>
        public DirectoryEntry this[string name]
        {
            get
            {
                int index = this.IndexOf(name);
                if (index != -1)
                {
                    return this[index];
                }
                return null;
            }
        }
    }
}

