#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class used to open or save Excel document 
    /// by <see cref="T:Dt.Xls.IExcelReader" /> and <see cref="T:Dt.Xls.IExcelWriter" />.
    /// </summary>
    public class ExcelOperator
    {
        private ExcelFileHandler _fileHandler;

        /// <summary>
        /// Initialize a new read-only <see cref="T:Dt.Xls.ExcelOperator" /> with special <see cref="T:Dt.Xls.IExcelReader" />.
        /// </summary>
        /// <param name="reader">
        /// An <see cref="T:Dt.Xls.IExcelReader" /> indicate the reader.
        /// </param>
        /// <param name="measure">
        /// An <see cref="T:Dt.Xls.IMeasureString" /> indicate the utility instance for measuring string.
        /// The default value is <see langword="null" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="reader" /> is <see langword="null" />.</exception>
        public ExcelOperator(IExcelReader reader, IMeasureString measure = null) : this(reader, null, measure)
        {
        }

        /// <summary>
        /// Initialize a new write-only <see cref="T:Dt.Xls.ExcelOperator" /> with special <see cref="T:Dt.Xls.IExcelWriter" />.
        /// </summary>
        /// <param name="writer">
        /// An <see cref="T:Dt.Xls.IExcelWriter" /> indicate the writer.
        /// </param>
        /// <param name="measure">
        /// An <see cref="T:Dt.Xls.IMeasureString" /> indicate the utility instance for measuring string.
        /// The default value is <see langword="null" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="writer" /> is <see langword="null" />.</exception>
        public ExcelOperator(IExcelWriter writer, IMeasureString measure = null) : this(null, writer, measure)
        {
        }

        /// <summary>
        /// Initialize a new read-write <see cref="T:Dt.Xls.ExcelOperator" /> with special <see cref="T:Dt.Xls.IExcelReader" /> and <see cref="T:Dt.Xls.IExcelWriter" />.
        /// </summary>
        /// <param name="reader">An <see cref="T:Dt.Xls.IExcelReader" /> indicate the reader.</param>
        /// <param name="writer">An <see cref="T:Dt.Xls.IExcelWriter" /> indicate the writer.</param>
        /// <param name="measure">
        /// An <see cref="T:Dt.Xls.IMeasureString" /> indicate the utility instance for measuring string.
        /// The default value is <see langword="null" />.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// Both the <paramref name="reader" /> and <paramref name="writer" /> are <see langword="null" />.
        /// </exception>
        public ExcelOperator(IExcelReader reader, IExcelWriter writer, IMeasureString measure = null)
        {
            if (object.ReferenceEquals(reader, null) && object.ReferenceEquals(writer, null))
            {
                throw new ArgumentNullException(ResourceHelper.GetResourceString("readerAndWriterNullError"));
            }
            this._fileHandler = new ExcelFileHandler(reader, writer, null);
        }

        /// <summary>
        /// Open an Excel document from <see cref="T:System.IO.Stream" /> and load the worksheet which indicated by <paramref name="sheetIndex" /> with password.
        /// </summary>
        /// <param name="inStream">
        /// The input stream.
        /// </param>
        /// <param name="sheetIndex">
        /// The index of loading <see cref="T:Dt.Xls.ExcelWorksheet" />. If it's <b>-1</b>, means load all sheets.
        /// </param>
        /// <param name="password">
        /// The password to open the workbook
        /// The default password is <see langword="null" />.
        /// </param>
        public void Open(Stream inStream, int sheetIndex = -1, string password = null)
        {
            this._fileHandler.Open(inStream, sheetIndex, password);
        }

        /// <summary>
        /// Saves the Excel document data to a <see cref="T:System.IO.Stream" /> with <paramref name="workbookType" /> format
        /// and special password by <paramref name="password" />.
        /// </summary>
        /// <param name="outStream">
        /// The output stream.
        /// </param>
        /// <param name="password">
        /// The password of the workbook.
        /// The default password is <see langword="null" />.
        /// </param>
        /// <param name="workbookType">
        /// A <see cref="T:Dt.Xls.ExcelFileType" /> indicate the type of saved Excel file
        /// </param>
        public void Save(Stream outStream, ExcelFileType workbookType = 0, string password = null)
        {
            this._fileHandler.Save(outStream, workbookType, password);
        }

        /// <summary>
        /// Gets the value indicates whether can read or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> indicates can read, otherwise return <see langword="false" />.
        /// </value>
        public bool CanRead
        {
            get { return  this._fileHandler.CanRead; }
        }

        /// <summary>
        /// Gets the value indicates whether can write or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> indicates can write, otherwise return <see langword="false" />.
        /// </value>
        public bool CanWrite
        {
            get { return  this._fileHandler.CanWrite; }
        }

        /// <summary>
        /// Gets the exported excel version.
        /// </summary>
        /// <value>The exported excel version.</value>
        internal ExcelVersion ExportedExcelVersion
        {
            get { return  this._fileHandler.ExportedExcelVersion; }
        }

        /// <summary>
        /// Gets the imported excel version.
        /// </summary>
        /// <value>The imported excel version.</value>
        internal ExcelVersion ImportedExcelVersion
        {
            get { return  this._fileHandler.ImportedExcelVersion; }
        }
    }
}

