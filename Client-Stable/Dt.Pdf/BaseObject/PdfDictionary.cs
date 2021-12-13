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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Pdf.BaseObject
{
    /// <summary>
    /// Pdf base type Dictionary.
    /// </summary>
    public class PdfDictionary : PdfObjectBase, IDictionary<PdfName, PdfObjectBase>, ICollection<KeyValuePair<PdfName, PdfObjectBase>>, IEnumerable<KeyValuePair<PdfName, PdfObjectBase>>, IEnumerable
    {
        private readonly Dictionary<PdfName, PdfObjectBase> dic;
        private const byte dicPrefix = 60;
        private const byte dicSuffix = 0x3e;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfDictionary" /> class.
        /// </summary>
        public PdfDictionary()
        {
            this.dic = new Dictionary<PdfName, PdfObjectBase>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Pdf.BaseObject.PdfDictionary" /> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PdfDictionary(PdfDictionary value)
        {
            this.dic = new Dictionary<PdfName, PdfObjectBase>();
            if (value == null)
            {
                throw new PdfArgumentNullException("value");
            }
            this.dic = new Dictionary<PdfName, PdfObjectBase>(value.dic);
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            this.dic.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public void Add(PdfName key, PdfObjectBase value)
        {
            this.dic.Add(key, value);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear()
        {
            this.dic.Clear();
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            foreach (KeyValuePair<PdfName, PdfObjectBase> pair in this.dic)
            {
                if (pair.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        public bool ContainsKey(PdfName key)
        {
            return this.dic.ContainsKey(key);
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
        public void CopyTo(KeyValuePair<PdfName, PdfObjectBase>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new PdfArgumentNullException("array");
            }
            if (array.Length < (this.dic.Count + arrayIndex))
            {
                throw new PdfArgumentOutOfRangeException("arrayIndex");
            }
            foreach (KeyValuePair<PdfName, PdfObjectBase> pair in this.dic)
            {
                array[arrayIndex++] = pair;
            }
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
        public IEnumerator<KeyValuePair<PdfName, PdfObjectBase>> GetEnumerator()
        {
            return this.dic.GetEnumerator();
        }

        /// <summary>
        /// Gets the or lazy create.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected T GetOrLazyCreate<T>(PdfName name) where T: PdfObjectBase, new()
        {
            if (!this.ContainsKey(name))
            {
                this.Add(name, Activator.CreateInstance<T>());
            }
            PdfObjectBase base2 = this[name];
            if (!(base2 is T))
            {
                throw new PdfObjectInternalException(PdfObjectInternalException.PdfObjectExceptionType.Cast);
            }
            return (T) base2;
        }

        /// <summary>
        /// Determines whether [is type of] [the specified type].
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// <c>true</c> if [is type of] [the specified type]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsTypeOf(PdfName type)
        {
            if (type == null)
            {
                throw new PdfArgumentNullException("type");
            }
            if (!this.ContainsKey(PdfName.Type))
            {
                return false;
            }
            PdfName name = this[PdfName.Type] as PdfName;
            if (name == null)
            {
                return false;
            }
            return name.Equals(type);
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public bool Remove(PdfName key)
        {
            return this.dic.Remove(key);
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
        public bool Remove(KeyValuePair<PdfName, PdfObjectBase> item)
        {
            return this.dic.Remove(item.Key);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.dic.GetEnumerator();
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
            psw.WriteByte(60).WriteByte(60);
            foreach (KeyValuePair<PdfName, PdfObjectBase> pair in this.dic)
            {
                pair.Key.ToPdf(writer);
                psw.WriteSpace();
                writer.WriteObject(pair.Value);
            }
            psw.WriteByte(0x3e).WriteByte(0x3e);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="key" /> is null.
        /// </exception>
        public bool TryGetValue(PdfName key, out PdfObjectBase value)
        {
            return this.dic.TryGetValue(key, out value);
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
            get { return  this.dic.Count; }
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
        /// Gets or sets the <see cref="T:Dt.Pdf.BaseObject.PdfObjectBase" /> with the specified key.
        /// </summary>
        /// <value></value>
        public PdfObjectBase this[PdfName key]
        {
            get { return  this.dic[key]; }
            set { this.dic[key] = value; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<PdfName> Keys
        {
            get { return  this.dic.Keys; }
        }

        /// <summary>
        /// Gets the raw value.
        /// </summary>
        /// <value>The raw value.</value>
        public Dictionary<PdfName, PdfObjectBase> RawValue
        {
            get { return  this.dic; }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<PdfObjectBase> Values
        {
            get { return  this.dic.Values; }
        }
    }
}

