#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a data source class for Csv data binding.
    /// </summary>
    public class CsvFileSource
    {
        string columnDelimiter;
        System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// Constructs a data source for Csv file binding.
        /// </summary>
        /// <param name="CsvfileStream">The Csv file stream.</param>
        public CsvFileSource(Stream CsvfileStream)
        {
            this.ColumnDelimiter = ",";
            this.FileStream = CsvfileStream;
        }

        /// <summary>
        /// Gets or sets the cell delimiter.
        /// </summary>
        public string CellDelimiter { get; set; }

        /// <summary>
        /// Gets or sets the column delimiter.
        /// </summary>
        public string ColumnDelimiter
        {
            get { return  this.columnDelimiter; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.columnDelimiter = value;
                }
                else
                {
                    this.columnDelimiter = ",";
                }
            }
        }

        /// <summary>
        /// Gets or sets the encoding for the Csv file.
        /// </summary>
        public System.Text.Encoding Encoding
        {
            get { return  this.encoding; }
            set { this.encoding = value; }
        }

        /// <summary>
        /// Gets or sets Csv file stream.
        /// </summary>
        public Stream FileStream { get; set; }

        /// <summary>
        /// Gets or sets open flags of the Csv source.
        /// </summary>
        public TextFileOpenFlags OpenFlags { get; set; }

        /// <summary>
        /// Gets or sets the row delimiter.
        /// </summary>
        public string RowDelimiter { get; set; }
    }
}

