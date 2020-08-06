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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a connection base for the data source.
    /// </summary>
    public abstract class ConnectionBase
    {
        bool allowInsert;
        SparseArray<RecordState> cachedRecordStates = new SparseArray<RecordState>();
        SparseArray<Dictionary<string, object>> cachedRecordValues = new SparseArray<Dictionary<string, object>>();
        object dataSource;
        internal static List<Type> externalConnectionTypes;
        bool isOpened;

        /// <summary>
        /// Creates a connection base.
        /// </summary>
        protected ConnectionBase()
        {
            this.Init();
        }

        /// <summary>
        /// Gets the type of the external connection.
        /// </summary>
        /// <param name="type">The type of the external connection.</param>
        public static void AddExternalConnectionType(Type type)
        {
            if (externalConnectionTypes == null)
            {
                externalConnectionTypes = new List<Type>();
            }
            externalConnectionTypes.Add(type);
        }

        /// <summary>
        /// Cancels the connection.
        /// </summary>
        public virtual void Cancel()
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        /// <summary>
        /// Gets whether the data source can be opened.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the data source can be opened; otherwise , <c>false</c>
        /// </returns>
        public abstract bool CanOpen();
        /// <summary>
        /// Clears cache data of the connection.
        /// </summary>
        public void ClearCachedData()
        {
            this.cachedRecordStates = new SparseArray<RecordState>();
            this.cachedRecordValues = new SparseArray<Dictionary<string, object>>();
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public virtual void Close()
        {
            if (this.isOpened)
            {
                this.isOpened = false;
                this.Init();
            }
        }

        /// <summary>
        /// Commits the connection change.
        /// </summary>
        public virtual void Commit()
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        /// <summary>
        /// Gets the type of the specified column field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The type of the column field.</returns>
        public virtual Type GetColumnDataType(string field)
        {
            return typeof(object);
        }

        internal List<int> GetDirtyRecordIndex()
        {
            return this.cachedRecordValues.GetNonEmptyIndexes();
        }

        /// <summary>
        /// Gets the record object of the data source.
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        /// <returns>The record object instance.</returns>
        protected abstract object GetRecord(int recordIndex);
        /// <summary>
        /// Gets the number of records.
        /// </summary>
        /// <returns>The record count.</returns>
        public abstract int GetRecordCount();
        /// <summary>
        /// Gets the record value from the specified field.
        /// </summary>
        /// <param name="record">The record object.</param>
        /// <param name="field">The field name.</param>
        /// <returns>The value of the record from the specified field.</returns>
        protected abstract object GetRecordValue(object record, string field);
        /// <summary>
        /// Gets the object of the specified record.
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        /// <param name="field">The field name.</param>
        /// <returns>The object of the specified record.</returns>
        public object GetValue(int recordIndex, string field)
        {
            if (((RecordState) this.cachedRecordStates[recordIndex]) == RecordState.Uncommitted)
            {
                Dictionary<string, object> dictionary = this.cachedRecordValues[recordIndex];
                if (dictionary == null)
                {
                    dictionary = new Dictionary<string, object>();
                    this.cachedRecordValues[recordIndex] = dictionary;
                }
                object recordValue = null;
                if (!dictionary.TryGetValue(field, out recordValue))
                {
                    object obj3 = this.GetRecord(recordIndex);
                    if (obj3 != null)
                    {
                        recordValue = this.GetRecordValue(obj3, field);
                    }
                    dictionary.Add(field, recordValue);
                }
                return recordValue;
            }
            object record = this.GetRecord(recordIndex);
            if (record != null)
            {
                return this.GetRecordValue(record, field);
            }
            return null;
        }

        void Init()
        {
            this.dataSource = null;
            this.ClearCachedData();
            this.allowInsert = true;
            this.isOpened = false;
        }

        /// <summary>
        /// Inserts a record in the specified location.
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        /// <param name="record">The record to be inserted.</param>
        public virtual void Insert(int recordIndex, object record)
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        /// <summary>
        /// Opens the data source.
        /// </summary>
        public virtual void Open()
        {
            if ((this.DataSource != null) && !this.isOpened)
            {
                this.isOpened = true;
            }
        }

        /// <summary>
        /// Removes the specified record.
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        public virtual void Remove(int recordIndex)
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        /// <summary>
        /// Sets the record value in the specified field.
        /// </summary>
        /// <param name="record">The record object.</param>
        /// <param name="field">The field name.</param>
        /// <param name="value">The value to set.</param>
        protected virtual void SetRecordValue(object record, string field, object value)
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        /// <summary>
        /// Puts a value of the specified type in the field.
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        /// <param name="field">The field name.</param>
        /// <param name="value">Value that is to be set.</param>
        public void SetValue(int recordIndex, string field, object value)
        {
            if (((RecordState) this.cachedRecordStates[recordIndex]) == RecordState.Committed)
            {
                this.cachedRecordStates[recordIndex] = RecordState.Uncommitted;
            }
            Dictionary<string, object> dictionary = this.cachedRecordValues[recordIndex];
            if (dictionary == null)
            {
                dictionary = new Dictionary<string, object>();
                this.cachedRecordValues[recordIndex] = dictionary;
            }
            dictionary[field] = value;
        }

        /// <summary>
        /// Updates the specified record. 
        /// </summary>
        /// <param name="recordIndex">The record index.</param>
        /// <param name="record">The record to be updated.</param>
        public virtual void Update(int recordIndex, object record)
        {
            throw new NotSupportedException(ResourceStrings.DataBindingNotSupport);
        }

        internal virtual void UpdateCollectionView()
        {
        }

        /// <summary>
        /// Gets whether the connection allows insert actions.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connection allows insert; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AllowInsert
        {
            get { return  this.allowInsert; }
        }

        /// <summary>
        /// Gets the data fields from the connection.
        /// </summary>
        public virtual string[] DataFields
        {
            get { return  new string[0]; }
        }

        /// <summary>
        /// Get or sets data source
        /// </summary>
        public object DataSource
        {
            get { return  this.dataSource; }
            set
            {
                if (this.IsOpen)
                {
                    this.Close();
                }
                this.dataSource = value;
            }
        }

        /// <summary>
        /// Gets whether the connection is open.
        /// </summary>
        /// <value>
        /// <c>true</c> if the connection is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen
        {
            get { return  this.isOpened; }
        }

        internal enum RecordState
        {
            Committed,
            Uncommitted
        }
    }
}

