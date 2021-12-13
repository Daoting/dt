#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using Dt.Pdf.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Array
    /// </summary>
    public class PdfArray : PdfObjectBase, ICollection<PdfObjectBase>, IEnumerable<PdfObjectBase>, IEnumerable
    {
        private readonly List<PdfObjectBase> list;
        private const byte prefix = 0x5b;
        private const byte suffix = 0x5d;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfArray" /> class.
        /// </summary>
        public PdfArray()
        {
            this.list = new List<PdfObjectBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfArray" /> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public PdfArray(PdfArray array)
        {
            this.list = new List<PdfObjectBase>();
            if (array == null)
            {
                throw new PdfArgumentNullException("array");
            }
            this.list.AddRange(array.list);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfArray" /> class.
        /// </summary>
        /// <param name="array">The array.</param>
        public PdfArray(IEnumerable<PdfObjectBase> array)
        {
            this.list = new List<PdfObjectBase>();
            this.list.AddRange(array);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(PdfObjectBase item)
        {
            this.list.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(PdfObjectBase item)
        {
            return this.list.Contains(item);
        }

        /// <summary>
        /// Converts the specified array to PdfArray.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        public static PdfArray Convert(Array array)
        {
            if (array == null)
            {
                throw new PdfArgumentNullException("array");
            }
            PdfArray array2 = new PdfArray();
            foreach (object obj2 in array)
            {
                if (obj2 is string)
                {
                    array2.Add(new PdfString((string) ((string) obj2)));
                }
                else if (!(obj2 is int))
                {
                    if (obj2 is float)
                    {
                        array2.Add(new PdfNumber((double) ((float) obj2)));
                    }
                    else
                    {
                        if (!(obj2 is double))
                        {
                            throw new PdfObjectInternalException(PdfObjectInternalException.PdfObjectExceptionType.Cast);
                        }
                        array2.Add(new PdfNumber((double) ((double) obj2)));
                    }
                }
                else
                {
                    array2.Add(new PdfNumber((double) ((int) obj2)));
                }
            }
            return array2;
        }

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="array" /> is multidimensional.
        /// -or-
        /// <paramref name="arrayIndex" /> is equal to or greater than the length of <paramref name="array" />.
        /// -or-
        /// The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.
        /// </exception>
        public void CopyTo(PdfObjectBase[] array, int arrayIndex)
        {
            this.list.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Get bytes of object
        /// </summary>
        /// <returns></returns>
        public override byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<PdfObjectBase> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, PdfObjectBase item)
        {
            this.list.Insert(index, item);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public bool Remove(PdfObjectBase item)
        {
            return this.list.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        /// <summary>
        /// Read data from Pdf reader
        /// </summary>
        /// <param name="reader">Pdf Reader</param>
        public override void ToObject(IPdfReader reader)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Write Pdf object to Pdf writer
        /// </summary>
        /// <param name="writer">Pdf Writer</param>
        public override void ToPdf(PdfWriter writer)
        {
            PdfStreamWriter psw = writer.Psw;
            psw.WriteByte(0x5b);
            this.WriteContent(writer);
            psw.WriteByte(0x5d);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
        /// </returns>
        public override string ToString()
        {
            if (this.list == null)
            {
                return base.ToString();
            }
            return this.list.ToString();
        }

        /// <summary>
        /// Writes the content.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void WriteContent(PdfWriter writer)
        {
            bool flag = false;
            foreach (PdfObjectBase base2 in this.list)
            {
                if (flag)
                {
                    writer.Psw.WriteSpace();
                }
                else
                {
                    flag = true;
                }
                writer.WriteObject(base2);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count
        {
            get { return  this.list.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return  false; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Pdf.BaseObject.PdfObjectBase" /> at the specified index.
        /// </summary>
        /// <value></value>
        public PdfObjectBase this[int index]
        {
            get { return  this.list[index]; }
            set { this.list[index] = value; }
        }

        /// <summary>
        /// Gets the last.
        /// </summary>
        /// <value>The last.</value>
        public PdfObjectBase Last
        {
            get
            {
                if (this.Count > 0)
                {
                    return this.list[this.Count - 1];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the raw value.
        /// </summary>
        /// <value>The raw value.</value>
        public List<PdfObjectBase> RawValue
        {
            get { return  this.list; }
        }
    }
}

