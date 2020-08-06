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
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    internal sealed class CsvFileConnection : ConnectionBase
    {
        List<List<string>> cachedFileText;
        List<string> cachedHeaders;
        CsvFileSource fileSource;
        bool hasColumnHeader;
        bool hasRowHeader;

        public override bool CanOpen()
        {
            return (base.DataSource is CsvFileSource);
        }

        void CreateDataFields()
        {
            if ((this.cachedFileText != null) || (this.cachedFileText.Count > 0))
            {
                int maxLength = CsvImp.GetMaxLength(this.cachedFileText);
                if (this.hasColumnHeader)
                {
                    this.cachedHeaders = this.cachedFileText[0];
                }
                else
                {
                    this.cachedHeaders = new List<string>();
                    int num2 = 0;
                    if (!this.hasRowHeader)
                    {
                        num2 = 1;
                    }
                    for (int i = num2; i <= maxLength; i++)
                    {
                        this.cachedHeaders.Add("Column" + ((int) i).ToString());
                    }
                }
            }
        }

        protected override object GetRecord(int recordIndex)
        {
            if (!base.IsOpen || (this.cachedFileText == null))
            {
                return null;
            }
            if (!this.hasColumnHeader)
            {
                return this.cachedFileText[recordIndex];
            }
            return this.cachedFileText[recordIndex + 1];
        }

        public override int GetRecordCount()
        {
            if (!base.IsOpen || (this.cachedFileText == null))
            {
                return 0;
            }
            if (!this.hasColumnHeader)
            {
                return this.cachedFileText.Count;
            }
            return (this.cachedFileText.Count - 1);
        }

        protected override object GetRecordValue(object record, string field)
        {
            if (base.IsOpen)
            {
                int index = -1;
                if (this.cachedHeaders != null)
                {
                    index = this.cachedHeaders.IndexOf(field);
                }
                if (index > -1)
                {
                    List<string> list = record as List<string>;
                    if ((list != null) && (index < list.Count))
                    {
                        return list[index];
                    }
                }
            }
            return null;
        }

        public override void Open()
        {
            object dataSource = base.DataSource;
            this.fileSource = dataSource as CsvFileSource;
            if ((this.fileSource != null) && (this.fileSource.FileStream != null))
            {
                try
                {
                    Encoding encoding = this.fileSource.Encoding;
                    if (encoding == null)
                    {
                        encoding = Encoding.UTF8;
                    }
                    string data = new StreamReader(this.fileSource.FileStream, encoding, true).ReadToEnd();
                    this.cachedFileText = CsvImp.ParseText(data, this.fileSource.RowDelimiter, this.fileSource.ColumnDelimiter, this.fileSource.CellDelimiter);
                    this.hasRowHeader = (this.fileSource.OpenFlags & TextFileOpenFlags.IncludeRowHeader) == TextFileOpenFlags.IncludeRowHeader;
                    this.hasColumnHeader = (this.fileSource.OpenFlags & TextFileOpenFlags.IncludeColumnHeader) == TextFileOpenFlags.IncludeColumnHeader;
                    base.Open();
                    if ((this.cachedFileText != null) && (this.cachedFileText.Count > 0))
                    {
                        this.CreateDataFields();
                    }
                }
                catch
                {
                }
            }
        }

        public void ReOpen()
        {
            object dataSource = base.DataSource;
            this.fileSource = dataSource as CsvFileSource;
            if (((this.fileSource != null) && (this.fileSource.FileStream != null)) && (this.fileSource.FileStream.CanRead && this.fileSource.FileStream.CanSeek))
            {
                if (this.fileSource.FileStream.CanSeek)
                {
                    this.fileSource.FileStream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                }
                this.Open();
            }
        }

        public override string[] DataFields
        {
            get
            {
                if ((base.IsOpen && (this.cachedFileText != null)) && (this.cachedHeaders != null))
                {
                    string[] strArray = this.cachedHeaders.ToArray();
                    if (!this.hasRowHeader)
                    {
                        return strArray;
                    }
                    if (strArray.Length > 0)
                    {
                        List<string> list = Enumerable.ToList<string>(strArray);
                        list.RemoveAt(0);
                        return list.ToArray();
                    }
                }
                return base.DataFields;
            }
        }
    }
}

