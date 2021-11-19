#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Biff;
using Dt.Xls.OOXml;
using System;
using System.IO;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    internal class ExcelFileHandler
    {
        private IDocumentProperties _documentInfo;
        private IExcelReader _excelReader;
        private IExcelWriter _excelWriter;
        private ExcelVersion _exportedExcelVersion;
        private ExcelVersion _importedExcelVersion;
        private IMeasureString _measure;

        public ExcelFileHandler(IExcelReader reader, IExcelWriter writer, IMeasureString measure = null)
        {
            this._excelReader = reader;
            this._excelWriter = writer;
            this._measure = measure;
            this._documentInfo = new DocumentProperties();
        }

        private static void InitializeXlsFile(CompoundFile xlsFile)
        {
            if (xlsFile == null)
            {
                throw new ArgumentNullException();
            }
            xlsFile.AddStream(XlsDirectoryEntryNames.SummaryInformationStream, new byte[0x40]);
            xlsFile.AddStream(XlsDirectoryEntryNames.DocumentSummaryInfomationStream, new byte[0x40]);
        }

        private bool IsEncryptedWorkbookStream(Stream stream)
        {
            bool flag = false;
            if ((stream != null) && (stream.Length != 0L))
            {
                BiffRecord record = new BiffRecord();
                BiffRecord record2 = new BiffRecord();
                stream.Seek(20L, (SeekOrigin) SeekOrigin.Begin);
                BinaryReader reader = new BinaryReader(stream);
                record.Read(reader);
                stream.Seek(0x18L, (SeekOrigin) SeekOrigin.Begin);
                record2.Read(reader);
                if ((record.RecordType == BiffRecordNumber.FILEPASS) || (record2.RecordType == BiffRecordNumber.FILEPASS))
                {
                    flag = true;
                }
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            }
            return flag;
        }

        /// <summary>
        /// LoadFromCompoundStorageFile
        /// </summary>
        /// <param name="inStream"></param>
        /// <param name="excelSheetIndex"></param>
        /// <param name="password"></param>
        private void LoadFromCompoundStorage(Stream inStream, int excelSheetIndex, string password)
        {
            if (inStream != null)
            {
                try
                {
                    CompoundFile file = new CompoundFile(true);
                    if (!file.Read(inStream))
                    {
                        this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("biffEntryError"), ExcelWarningCode.CannotOpen));
                    }
                    else
                    {
                        file.RemoveStorage(XlsDirectoryEntryNames.XmlSignaturesStorage, "Root Entry");
                        if (this._documentInfo != null)
                        {
                            if (file.HasStream(XlsDirectoryEntryNames.SummaryInformationStream, "Root Entry"))
                            {
                                this._documentInfo.SummaryInformation = SummaryInformation.Read(file.GetStream(XlsDirectoryEntryNames.SummaryInformationStream));
                            }
                            if (file.HasStream(XlsDirectoryEntryNames.DocumentSummaryInfomationStream, "Root Entry"))
                            {
                                this._documentInfo.DocumentSummaryInfomation = DocumentSummaryInfomation.Read(file.GetStream(XlsDirectoryEntryNames.DocumentSummaryInfomationStream));
                            }
                        }
                        if (!file.HasStream(XlsDirectoryEntryNames.WorkbookStream, "Root Entry"))
                        {
                            this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("biffEntryError"), ExcelWarningCode.CannotOpen));
                            this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("biffEntryError"), ExcelWarningCode.CannotOpen));
                        }
                        else
                        {
                            Stream stream = (Stream) new MemoryStream(file.GetStream(XlsDirectoryEntryNames.WorkbookStream));
                            bool flag = this.IsEncryptedWorkbookStream(stream);
                            if (!flag && !string.IsNullOrWhiteSpace(password))
                            {
                                this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("invalidPassword"), ExcelWarningCode.IncorrectPassword));
                            }
                            else if (flag && (password == null))
                            {
                                this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("invalidPassword"), ExcelWarningCode.IncorrectPassword));
                            }
                            else if ((flag && (password != null)) && (password.Length > 0))
                            {
                                this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("winRTNotSupportPassword"), ExcelWarningCode.CannotOpen));
                            }
                            else if (flag && (password.Length == 0))
                            {
                                this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("invalidPassword"), ExcelWarningCode.IncorrectPassword));
                            }
                            else
                            {
                                new BiffRecordReader(this._excelReader, this._measure) { Password = password }.ProcessStream(stream, excelSheetIndex);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("biffGeneralError"), ExcelWarningCode.General, -1, -1, -1, exception));
                }
            }
        }

        private void LoadOffice12File(Stream inStream, int excelSheetIndex)
        {
            this._importedExcelVersion = ExcelVersion.Excel2007;
            new XlsxReader(this._excelReader, this._documentInfo).Load(inStream, excelSheetIndex);
        }

        public void Open(Stream inStream, int excelSheetIndex, string password)
        {
            try
            {
                if (CompoundFile.IsLegal(inStream))
                {
                    bool flag2 = false;
                    if ((password != null) && (password.Length > 0))
                    {
                        this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("winRTNotSupportPassword"), ExcelWarningCode.CannotOpen));
                    }
                    else if (!flag2)
                    {
                        try
                        {
                            this.LoadFromCompoundStorage(inStream, excelSheetIndex, password);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        this.LoadOffice12File(inStream, excelSheetIndex);
                    }
                }
                else
                {
                    this.LoadOffice12File(inStream, excelSheetIndex);
                }
            }
            catch (Exception exception)
            {
                this._excelReader.OnExcelLoadError(new ExcelWarning(ResourceHelper.GetResourceString("openFileError"), ExcelWarningCode.CannotOpen, -1, -1, -1, exception));
            }
        }

        /// <summary>
        /// Saves the specified out stream.
        /// </summary>
        /// <param name="outStream">The out stream.</param>
        /// <param name="workbookType">Type of the workbook.</param>
        /// <param name="password">The password.</param>
        public void Save(Stream outStream, ExcelFileType workbookType, string password)
        {
            if (workbookType == ExcelFileType.XLS)
            {
                this.SaveToCompoundStorage(outStream, password);
            }
            else
            {
                this.SaveToOffice12File(outStream, password, workbookType);
            }
        }

        private void SaveToCompoundStorage(Stream fileStream, string password)
        {
            if (fileStream != null)
            {
                BiffRecordWriter writer = null;
                MemoryStream stream = new MemoryStream();
                writer = new BiffRecordWriter(this._excelWriter, this._measure);
                if (((password != null) && (password.Length > 0)) && (password.Length < 0x100))
                {
                    writer.IsEncrypted = true;
                }
                writer.BuildStream((Stream) stream);
                CompoundFile xlsFile = new CompoundFile(false);
                InitializeXlsFile(xlsFile);
                xlsFile.AddStream(XlsDirectoryEntryNames.WorkbookStream, stream.ToArray());
                xlsFile.Write(fileStream);
            }
        }

        private void SaveToOffice12File(Stream stream, string password, ExcelFileType workbookType)
        {
            if (stream != null)
            {
                bool flag = false;
                if (((password != null) && (password.Length > 0)) && (password.Length < 0x100))
                {
                    flag = true;
                }
                this._exportedExcelVersion = ExcelVersion.Excel2007;
                XlsxWriter writer = new XlsxWriter(this._excelWriter, this._documentInfo, workbookType);
                if (flag)
                {
                    stream = (Stream) new MemoryStream();
                }
                writer.Save(stream);
            }
        }

        public bool CanRead
        {
            get { return  !object.ReferenceEquals(this._excelReader, null); }
        }

        public bool CanWrite
        {
            get { return  !object.ReferenceEquals(this._excelWriter, null); }
        }

        /// <summary>
        /// Gets or sets the exported excel version.
        /// </summary>
        /// <value>The exported excel version.</value>
        public ExcelVersion ExportedExcelVersion
        {
            get { return  this._exportedExcelVersion; }
            set { this._exportedExcelVersion = value; }
        }

        /// <summary>
        /// Indicate type of current opening file.
        /// </summary>
        /// <value>The imported excel version.</value>
        public ExcelVersion ImportedExcelVersion
        {
            get { return  this._importedExcelVersion; }
            set { this._importedExcelVersion = value; }
        }
    }
}

