#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Biff
{
    internal class BiffRecordReader
    {
        private int _activeSheetIndex = -1;
        private int _autoFilterDropDownArrowCount;
        private Dictionary<int, IExcelAutoFilter> _autoFilters = new Dictionary<int, IExcelAutoFilter>();
        private HashSet<short> _autoFilterSheets = new HashSet<short>();
        private List<uint> _boundSheetStartPosition = new List<uint>();
        private CalculationProperty _calcProperty = new CalculationProperty();
        private List<ExtendedFormat> _cellFormats = new List<ExtendedFormat>();
        private short _commentObjCount;
        private Queue<short> _commentObjectIDs = new Queue<short>();
        private List<ExcelConditionalFormat> _conditionaFormat12 = new List<ExcelConditionalFormat>();
        private List<ExcelConditionalFormat> _conditonalFormat = new List<ExcelConditionalFormat>();
        private ContinueRecordType _continueRecordType;
        private List<IFunction> _customFunctions = new List<IFunction>();
        private List<object> _customNameList = new List<object>();
        private List<IDifferentialFormatting> _dxfs = new List<IDifferentialFormatting>();
        private IExcelReader _excelReader;
        private List<IExternalWorkbookInfo> _externalWorkbookInfos = new List<IExternalWorkbookInfo>();
        private Dictionary<int, ExcelFont> _fonts;
        private ushort _formulaPendingCol;
        private ushort _formulaPendingRow;
        private short _formulaPendingSheet;
        private List<byte> _hiddenState = new List<byte>();
        private bool _isA1ReferenceStyle = true;
        private bool _isAutoFilter;
        private bool _isAutoFilter12;
        private bool _isFormulaPending;
        private bool _isInFeature11;
        private bool _isPanesFrozen;
        private bool _isPrintFitToPage;
        private bool _isStringPending;
        private LinkTable _linkTable = new LinkTable();
        private Dictionary<int, List<NamedCellRange>> _localDefinedNames = new Dictionary<int, List<NamedCellRange>>();
        private Dictionary<int, string> _numberFormats;
        private BiffRecord _previousRecord;
        private Dictionary<int, string> _printAreas = new Dictionary<int, string>();
        private ExcelPrintOptions _printOptions = new ExcelPrintOptions();
        private ExcelPrintPageMargin _printPageMargin = new ExcelPrintPageMargin();
        private ExcelPrintPageSetting _printPageSetting = new ExcelPrintPageSetting();
        private Dictionary<int, string> _printTitles = new Dictionary<int, string>();
        private ExcelConditionalFormat _priviousConditonalFormat;
        private bool _readAllSheet = true;
        private uint? _readContinueFRT12_cCriteria;
        private uint? _readContinueFRT12_cDateGroupings;
        private IExcelFilterColumn _readContinueFRT12_filterColumn;
        private bool _readXFExtension = true;
        private List<Tuple<int, int>> _selections = new List<Tuple<int, int>>();
        private List<string> _sheetNames = new List<string>();
        private Dictionary<int, List<ExcelConditionalFormat>> _sheetsConditionalFormats = new Dictionary<int, List<ExcelConditionalFormat>>();
        private List<IExcelTable> _sheetTables = new List<IExcelTable>();
        private List<ExcelSheetType> _sheetTypes = new List<ExcelSheetType>();
        private string[] _sst;
        private List<byte[]> _SSTBuffers = new List<byte[]>();
        private Dictionary<int, BiffRecord> _standColWidths = new Dictionary<int, BiffRecord>();
        private int _stringPendingCol;
        private int _stringPendingRow;
        private short _stringPendingSheet;
        private List<IExcelStyle> _styles = new List<IExcelStyle>();
        private List<ExcelSupBook> _supBookList = new List<ExcelSupBook>();
        private Dictionary<int, List<IExcelTable>> _tables = new Dictionary<int, List<IExcelTable>>();
        private List<IExcelTableStyle> _tableStyles = new List<IExcelTableStyle>();
        private Dictionary<string, string> _txoStringTable = new Dictionary<string, string>();
        private int _txoTextLength;
        private List<BiffRecord> _unsupportedBiffList = new List<BiffRecord>();
        private List<BiffRecord> _workbookBiffList = new List<BiffRecord>();
        private ExcelWorkbookPropery _workbookProperty = new ExcelWorkbookPropery();
        private List<List<BiffRecord>> _worksheetBiffList = new List<List<BiffRecord>>();
        private List<byte[]> _xfBuffers = new List<byte[]>();
        private List<XFRecrod> _xfs;
        private int externNameRecordIndex;
        private int externSheetCount;
        private FormulaProcess fp = new FormulaProcess();

        public BiffRecordReader(IExcelReader reader, IMeasureString measure)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this._excelReader = reader;
            this._xfs = new List<XFRecrod>();
            this._numberFormats = new Dictionary<int, string>();
            this._fonts = new Dictionary<int, ExcelFont>();
        }

        private void ClearCache()
        {
            this._sheetTypes.Clear();
            this._unsupportedBiffList.Clear();
            this._worksheetBiffList.Clear();
            this._workbookBiffList.Clear();
            this._sheetNames.Clear();
            this._printAreas.Clear();
            this._autoFilterSheets.Clear();
            this._printTitles.Clear();
            this._linkTable = new LinkTable();
            if (this._fonts != null)
            {
                this._fonts.Clear();
            }
            if (this._numberFormats != null)
            {
                this._numberFormats.Clear();
            }
            if (this._xfs != null)
            {
                this._xfs.Clear();
            }
            this._sst = null;
            this._boundSheetStartPosition.Clear();
            this._hiddenState.Clear();
            this._SSTBuffers.Clear();
            this._customNameList.Clear();
            this._supBookList.Clear();
            this._txoStringTable.Clear();
            this._localDefinedNames.Clear();
            this._commentObjectIDs.Clear();
            this._selections.Clear();
            this._dxfs.Clear();
            this._priviousConditonalFormat = null;
            this._conditonalFormat.Clear();
            this._conditionaFormat12.Clear();
            this._sheetsConditionalFormats.Clear();
            this._customFunctions.Clear();
            this._externalWorkbookInfos.Clear();
            this.externNameRecordIndex = 0;
            this.externSheetCount = 0;
            this._previousRecord = null;
            this._isInFeature11 = false;
            this._tables.Clear();
            this._standColWidths.Clear();
            this._readAllSheet = true;
            this._xfBuffers.Clear();
            this._readXFExtension = true;
            this._tableStyles.Clear();
            this._sheetTables.Clear();
        }

        private byte[] ConvertShareFormulaTokenToNormalFormulaToken(byte[] buffer)
        {
            if (buffer == null)
            {
                return null;
            }
            byte[] buffer2 = new byte[buffer.Length];
            Array.Copy(buffer, buffer2, buffer2.Length);
            if (buffer2[0] == 0x2c)
            {
                buffer2[0] = 0x24;
                return buffer2;
            }
            if (buffer2[0] == 0x4c)
            {
                buffer2[0] = 0x44;
                return buffer2;
            }
            if (buffer2[0] == 0x6c)
            {
                buffer2[0] = 100;
                return buffer2;
            }
            if (buffer2[0] == 0x2d)
            {
                buffer2[0] = 0x25;
                return buffer2;
            }
            if (buffer2[0] == 0x4d)
            {
                buffer2[0] = 0x45;
                return buffer2;
            }
            if (buffer2[0] == 0x6d)
            {
                buffer2[0] = 0x65;
            }
            return buffer2;
        }

        private short GetCommentObjId()
        {
            if (this._commentObjectIDs.Count == 0)
            {
                return -1;
            }
            return this._commentObjectIDs.Dequeue();
        }

        private string GetFormula(byte[] formulaBytes, IRange range = null)
        {
            int row = 0;
            int column = 0;
            if (range != null)
            {
                row = range.Row;
                column = range.Column;
            }
            using (BinaryReader reader = new BinaryReader((Stream) new MemoryStream(formulaBytes)))
            {
                FormulaProcess process = new FormulaProcess(reader, null, null, this._calcProperty.RefMode == ExcelReferenceStyle.A1);
                ExcelCalcError error = null;
                process.row = row;
                process.column = column;
                return process.ToString(this._linkTable, ref error, new int?(row), new int?(column), false);
            }
        }

        private int GetInt(byte b1, byte b2)
        {
            int num = b1 & 0xff;
            int num2 = b2 & 0xff;
            return ((num2 << 8) | num);
        }

        private int GetInt(byte b1, byte b2, byte b3, byte b4)
        {
            int @int = this.GetInt(b1, b2);
            return ((this.GetInt(b3, b4) << 0x10) | @int);
        }

        private IRange GetRange(SimpleBinaryReader reader)
        {
            int num = reader.ReadUInt16();
            int num2 = reader.ReadUInt16();
            int num3 = reader.ReadUInt16();
            int num4 = reader.ReadUInt16();
            ExcelCellRange range = new ExcelCellRange {
                Row = num,
                Column = num3,
                RowSpan = (num2 - num) + 1,
                ColumnSpan = (num4 - num3) + 1
            };
            if (range.RowSpan == 0x10000)
            {
                range.RowSpan = 0x100000;
            }
            if (range.ColumnSpan == 0x100)
            {
                range.ColumnSpan = 0x4000;
            }
            return range;
        }

        private List<IRange> GetRangeAndRangeList(SimpleBinaryReader reader)
        {
            List<IRange> list = new List<IRange>();
            this.GetRange(reader);
            int num = reader.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                list.Add(this.GetRange(reader));
            }
            return list;
        }

        private string GetSearchText(string formula)
        {
            if (!string.IsNullOrWhiteSpace(formula))
            {
                int index = formula.IndexOf("SEARCH(");
                if (index != -1)
                {
                    int num2 = formula.IndexOf(',', index + 7);
                    return formula.Substring(index + 8, (num2 - index) - 9);
                }
                index = formula.IndexOf('=');
                if (index != -1)
                {
                    return formula.Substring(index + 2, (formula.Length - index) - 3);
                }
            }
            return null;
        }

        private bool IsTxoRecord(BiffRecordNumber recordType)
        {
            if ((recordType != BiffRecordNumber.CONTINUE) && (recordType != BiffRecordNumber.TXO))
            {
                return (recordType == BiffRecordNumber.OBJ);
            }
            return true;
        }

        private void LogError(string message, ExcelWarningCode warningCode, Exception ex)
        {
            ExcelWarning excelWarning = new ExcelWarning(message, warningCode, -1, -1, -1, ex);
            this._excelReader.OnExcelLoadError(excelWarning);
        }

        private void LogError(string message, ExcelWarningCode warningCode, int sheet, int row, int column, Exception ex)
        {
            ExcelWarning excelWarning = new ExcelWarning(message, warningCode, sheet, row, column, ex);
            this._excelReader.OnExcelLoadError(excelWarning);
        }

        private void LogUnsupportedBiffRecord(int sheet, BiffRecord biff)
        {
            ExcelWarning excelWarning = new ExcelWarning(ResourceHelper.GetResourceString("UnsupportedRecords"), ExcelWarningCode.UnsupportedRecords, sheet, -1, -1);
            List<UnsupportedBiffRecord> list = new List<UnsupportedBiffRecord>();
            UnsupportedBiffRecord record = new UnsupportedBiffRecord {
                RecordType = (int) biff.RecordType,
                RecordValue = biff.DataBuffer
            };
            list.Add(record);
            excelWarning.UnsupportedBiffRecords = list;
            this._excelReader.OnExcelLoadError(excelWarning);
        }

        private void LogUnsupportedBiffRecords(int sheet, List<BiffRecord> biffs)
        {
            if (biffs != null)
            {
                ExcelWarning excelWarning = new ExcelWarning(ResourceHelper.GetResourceString("UnsupportedRecords"), ExcelWarningCode.UnsupportedRecords, sheet, -1, -1);
                List<UnsupportedBiffRecord> list = new List<UnsupportedBiffRecord>();
                foreach (BiffRecord record in biffs)
                {
                    UnsupportedBiffRecord record2 = new UnsupportedBiffRecord {
                        RecordType = (int) record.RecordType,
                        RecordValue = record.DataBuffer
                    };
                    list.Add(record2);
                }
                excelWarning.UnsupportedBiffRecords = list;
                this._excelReader.OnExcelLoadError(excelWarning);
            }
        }

        internal double NumFromRk(long rk, out bool isFloat)
        {
            double num = 0.0;
            isFloat = false;
            if ((rk & 2L) == 2L)
            {
                num = rk >> 2;
            }
            else
            {
                byte[] buffer = new byte[8];
                buffer[4] = (byte)(rk & 0xfcL);
                buffer[5] = (byte)((rk >> 8) & 0xffL);
                buffer[6] = (byte)((rk >> 0x10) & 0xffL);
                buffer[7] = (byte)((rk >> 0x18) & 0xffL);
                num = BitConverter.ToDouble(buffer, 0);
                isFloat = true;
            }
            if ((rk & 1L) == 1L)
            {
                num /= 100.0;
                isFloat = true;
            }
            return num;
        }

        private List<BiffRecord> PreProcessBiffRecords(List<BiffRecord> records)
        {
            List<BiffRecord> list = new List<BiffRecord>();
            if ((records == null) || (records.Count == 0))
            {
                return list;
            }
            bool flag = false;
            foreach (BiffRecord record in records)
            {
                if (record.RecordType == BiffRecordNumber.BOF)
                {
                    SimpleBinaryReader reader = new SimpleBinaryReader(record.DataBuffer);
                    reader.ReadUInt16();
                    if (reader.ReadUInt16() == 0x10)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if ((record.RecordType == BiffRecordNumber.EOF) && flag)
                {
                    flag = false;
                }
                if (!flag)
                {
                    list.Add(record);
                }
            }
            flag = false;
            List<BiffRecord> list2 = new List<BiffRecord>();
            foreach (BiffRecord record2 in list)
            {
                if (record2.RecordType == BiffRecordNumber.USERSVIEWBEGIN)
                {
                    flag = true;
                }
                if (record2.RecordType == BiffRecordNumber.USERSVIEWEND)
                {
                    flag = false;
                }
                if (!flag && (record2.RecordType != BiffRecordNumber.USERSVIEWEND))
                {
                    list2.Add(record2);
                }
            }
            return list2;
        }

        private List<IUnsupportRecord> PreProcessUnsupportedRecords(int sheetIndex, List<BiffRecord> records, List<BiffRecord> supportRecords)
        {
            if ((records == null) || (records.Count == 0))
            {
                return null;
            }
            List<IUnsupportRecord> list = new List<IUnsupportRecord>();
            List<BiffRecord> biffRecords = new List<BiffRecord>();
            bool flag = false;
            bool flag2 = false;
            for (int i = 0; i < records.Count; i++)
            {
                BiffRecord record = records[i];
                if (record.RecordType == BiffRecordNumber.MSODRAWING)
                {
                    flag = true;
                }
                if ((record.RecordType == BiffRecordNumber.BOF) && (biffRecords.Count > 0))
                {
                    flag = true;
                }
                if (((!flag && (i > 0)) && ((records[i - 1].RecordType == BiffRecordNumber.EOF) && this.IsTxoRecord(record.RecordType))) || (flag2 && this.IsTxoRecord(record.RecordType)))
                {
                    flag2 = true;
                }
                else
                {
                    flag2 = false;
                }
                if (flag && (record.RecordType == BiffRecordNumber.WINDOW2))
                {
                    flag = false;
                }
                if (flag || flag2)
                {
                    biffRecords.Add(record);
                }
                else
                {
                    supportRecords.Add(record);
                }
                if ((record.RecordType == BiffRecordNumber.EOF) && flag)
                {
                    flag = false;
                    List<IUnsupportRecord> list3 = this.ProcessUnsupportRecord(sheetIndex, biffRecords);
                    if ((list3 != null) && (list3.Count > 0))
                    {
                        list.AddRange((IEnumerable<IUnsupportRecord>) list3);
                    }
                    biffRecords.Clear();
                }
            }
            if (biffRecords.Count > 0)
            {
                List<IUnsupportRecord> list4 = this.ProcessUnsupportRecord(sheetIndex, biffRecords);
                if ((list4 != null) && (list4.Count > 0))
                {
                    list.AddRange((IEnumerable<IUnsupportRecord>) list4);
                }
                biffRecords.Clear();
            }
            return list;
        }

        private void ProcessBiffRecord(short sheet, BiffRecord biff)
        {
            if (((biff.RecordType == BiffRecordNumber.FEAT11) || (biff.RecordType == BiffRecordNumber.FEAT11)) || (biff.RecordType == BiffRecordNumber.LIST12))
            {
                this._isInFeature11 = true;
            }
            else if ((biff.RecordType == BiffRecordNumber.AUTOFILTER12) && this._isInFeature11)
            {
                this._isInFeature11 = true;
            }
            else
            {
                this._isInFeature11 = false;
            }
            BiffRecordNumber recordType = biff.RecordType;
            if (recordType <= BiffRecordNumber.WINDOW2)
            {
                if (recordType <= BiffRecordNumber.EXCEL9FILE)
                {
                    if (recordType <= BiffRecordNumber.SXDBEX)
                    {
                        switch (recordType)
                        {
                            case BiffRecordNumber.NOTAVAILABLE:
                            case ~BiffRecordNumber.NOTAVAILABLE:
                            case ((BiffRecordNumber) 1):
                            case ((BiffRecordNumber) 2):
                            case ((BiffRecordNumber) 3):
                            case ((BiffRecordNumber) 4):
                            case ((BiffRecordNumber) 5):
                            case ((BiffRecordNumber) 7):
                            case ((BiffRecordNumber) 8):
                            case BiffRecordNumber.BOF_BIFF2:
                            case ((BiffRecordNumber) 11):
                            case BiffRecordNumber.PASSWORD:
                            case BiffRecordNumber.WINDOWPROTECT:
                            case ((BiffRecordNumber) 30):
                            case ((BiffRecordNumber) 0x1f):
                            case ((BiffRecordNumber) 0x20):
                            case ((BiffRecordNumber) 0x21):
                            case ((BiffRecordNumber) 0x24):
                            case ((BiffRecordNumber) 0x25):
                            case ((BiffRecordNumber) 0x2c):
                            case ((BiffRecordNumber) 0x2d):
                            case ((BiffRecordNumber) 0x2e):
                            case BiffRecordNumber.FILEPASS:
                            case ((BiffRecordNumber) 0x30):
                            case (BiffRecordNumber.DATE1904 | BiffRecordNumber.DELTA):
                            case ((BiffRecordNumber) 0x33):
                            case ((BiffRecordNumber) 0x34):
                            case ((BiffRecordNumber) 0x35):
                            case (BiffRecordNumber.LEFTMARGIN | BiffRecordNumber.DELTA):
                            case (BiffRecordNumber.FONT | BiffRecordNumber.FORMULA2):
                            case (BiffRecordNumber.TOPMARGIN | BiffRecordNumber.DELTA):
                            case ((BiffRecordNumber) 0x39):
                            case (BiffRecordNumber.PRINTHEADERS | BiffRecordNumber.DELTA):
                            case (BiffRecordNumber.FONT | BiffRecordNumber.EOF):
                            case ((BiffRecordNumber) 0x3e):
                            case ((BiffRecordNumber) 0x3f):
                            case BiffRecordNumber.BACKUP:
                            case BiffRecordNumber.XF_OLD:
                            case ((BiffRecordNumber) 0x44):
                            case ((BiffRecordNumber) 0x45):
                            case ((BiffRecordNumber) 70):
                            case ((BiffRecordNumber) 0x47):
                            case ((BiffRecordNumber) 0x48):
                            case ((BiffRecordNumber) 0x49):
                            case ((BiffRecordNumber) 0x4a):
                            case ((BiffRecordNumber) 0x4b):
                            case (BiffRecordNumber.BACKUP | BiffRecordNumber.CALCCOUNT):
                            case BiffRecordNumber.PLS:
                            case (BiffRecordNumber.CODEPAGE | BiffRecordNumber.CALCCOUNT):
                            case ((BiffRecordNumber) 0x4f):
                            case BiffRecordNumber.DCON:
                            case BiffRecordNumber.DCONREF:
                            case BiffRecordNumber.DCONNAME:
                            case ((BiffRecordNumber) 0x53):
                            case ((BiffRecordNumber) 0x54):
                            case ((BiffRecordNumber) 0x56):
                            case ((BiffRecordNumber) 0x57):
                            case ((BiffRecordNumber) 0x58):
                            case BiffRecordNumber.XCT:
                            case BiffRecordNumber.CRN:
                            case BiffRecordNumber.FILESHARING:
                            case BiffRecordNumber.WRITEACCESS:
                            case BiffRecordNumber.UNCALCED:
                            case BiffRecordNumber.TEMPLATE:
                            case ((BiffRecordNumber) 0x61):
                            case ((BiffRecordNumber) 0x62):
                            case BiffRecordNumber.OBJPROTECT:
                            case ((BiffRecordNumber) 100):
                            case ((BiffRecordNumber) 0x65):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.FORMULA2):
                            case ((BiffRecordNumber) 0x67):
                            case ((BiffRecordNumber) 0x68):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.BOF_BIFF2):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.EOF):
                            case ((BiffRecordNumber) 0x6b):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.CALCCOUNT):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.CALCMODE):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.PRECISION):
                            case (BiffRecordNumber.OBJPROTECT | BiffRecordNumber.CALCCOUNT):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.DELTA):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.ITERATION):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.PROTECT):
                            case (BiffRecordNumber.OBJPROTECT | BiffRecordNumber.DELTA):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.HEADER):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.FOOTER):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.EXTERNCOUNT):
                            case (BiffRecordNumber.OBJPROTECT | BiffRecordNumber.HEADER):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.NAME):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.WINDOWPROTECT):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.VERTICALPAGEBREAKS):
                            case (BiffRecordNumber.OBJPROTECT | BiffRecordNumber.NAME):
                            case (BiffRecordNumber.TEMPLATE | BiffRecordNumber.NOTE):
                            case ((BiffRecordNumber) 0x7e):
                            case BiffRecordNumber.IMDATA:
                            case BiffRecordNumber.GRIDSET:
                            case BiffRecordNumber.BUNDLESHEET:
                            case BiffRecordNumber.WRITEPROT:
                            case BiffRecordNumber.ADDIN:
                            case BiffRecordNumber.EDG:
                            case BiffRecordNumber.PUB:
                            case ((BiffRecordNumber) 0x8a):
                            case ((BiffRecordNumber) 0x8b):
                            case ((BiffRecordNumber) 0x8e):
                            case ((BiffRecordNumber) 0x8f):
                            case BiffRecordNumber.SORT:
                            case BiffRecordNumber.SUB:
                            case BiffRecordNumber.PALETTE:
                            case ((BiffRecordNumber) 0x93):
                            case BiffRecordNumber.LHRECORD:
                            case BiffRecordNumber.LHNGRAPH:
                            case BiffRecordNumber.SOUND:
                            case ((BiffRecordNumber) 0x97):
                            case BiffRecordNumber.LPR:
                            case BiffRecordNumber.STANDARDWIDTH:
                            case ((BiffRecordNumber) 0x9a):
                            case BiffRecordNumber.FILTERMODE:
                            case BiffRecordNumber.FNGROUPCOUNT:
                            case ((BiffRecordNumber) 0x9f):
                            case ((BiffRecordNumber) 0xa2):
                            case ((BiffRecordNumber) 0xa3):
                            case ((BiffRecordNumber) 0xa4):
                            case ((BiffRecordNumber) 0xa5):
                            case (BiffRecordNumber.SCL | BiffRecordNumber.FORMULA2):
                            case (BiffRecordNumber.SETUP | BiffRecordNumber.FORMULA2):
                            case ((BiffRecordNumber) 0xa8):
                            case BiffRecordNumber.COORDLIST:
                            case (BiffRecordNumber.SCL | BiffRecordNumber.EOF):
                            case BiffRecordNumber.GCW:
                            case (BiffRecordNumber.SCL | BiffRecordNumber.CALCCOUNT):
                            case ((BiffRecordNumber) 0xad):
                            case BiffRecordNumber.SCENMAN:
                            case BiffRecordNumber.SCENARIO:
                            case BiffRecordNumber.SXVIEW:
                            case BiffRecordNumber.SXVD:
                            case BiffRecordNumber.SXVI:
                            case ((BiffRecordNumber) 0xb3):
                            case BiffRecordNumber.SXIVD:
                            case BiffRecordNumber.SXLI:
                            case BiffRecordNumber.SXPI:
                            case ((BiffRecordNumber) 0xb7):
                            case BiffRecordNumber.DOCROUTE:
                            case BiffRecordNumber.RECIPNAME:
                            case ((BiffRecordNumber) 0xba):
                            case ((BiffRecordNumber) 0xbb):
                            case ((BiffRecordNumber) 0xbf):
                            case (BiffRecordNumber.GUTS | BiffRecordNumber.BACKUP):
                            case BiffRecordNumber.MMS:
                            case BiffRecordNumber.ADDMENU:
                            case BiffRecordNumber.DELMENU:
                            case (BiffRecordNumber.VCENTER | BiffRecordNumber.BACKUP):
                            case BiffRecordNumber.SXDI:
                            case BiffRecordNumber.SXDB:
                            case ((BiffRecordNumber) 0xc7):
                            case (BiffRecordNumber.EDG | BiffRecordNumber.BACKUP):
                            case ((BiffRecordNumber) 0xc9):
                            case ((BiffRecordNumber) 0xca):
                            case ((BiffRecordNumber) 0xcb):
                            case (BiffRecordNumber.COUNTRY | BiffRecordNumber.BACKUP):
                            case BiffRecordNumber.SXSTRING:
                            case ((BiffRecordNumber) 0xce):
                            case ((BiffRecordNumber) 0xcf):
                            case BiffRecordNumber.SXTBL:
                            case BiffRecordNumber.SXTBRGIITM:
                            case BiffRecordNumber.SXTBPG:
                            case BiffRecordNumber.OBPROJ:
                            case ((BiffRecordNumber) 0xd4):
                            case BiffRecordNumber.SXIDSTM:
                            case BiffRecordNumber.DBCELL:
                            case ((BiffRecordNumber) 0xd8):
                            case ((BiffRecordNumber) 0xd9):
                            case ((BiffRecordNumber) 0xdb):
                            case BiffRecordNumber.PARAMQRY:
                            case BiffRecordNumber.SCENPROTECT:
                            case BiffRecordNumber.OLESIZE:
                            case BiffRecordNumber.UDDESC:
                            case BiffRecordNumber.INTERFACEHDR:
                            case BiffRecordNumber.INTERFACEEND:
                            case BiffRecordNumber.SXVS:
                            case ((BiffRecordNumber) 0xe4):
                            case ((BiffRecordNumber) 230):
                            case ((BiffRecordNumber) 0xe7):
                            case ((BiffRecordNumber) 0xe8):
                            case ((BiffRecordNumber) 0xe9):
                            case BiffRecordNumber.TABIDCONF:
                            case BiffRecordNumber.MSODRAWINGGROUP:
                            case BiffRecordNumber.MSODRAWING:
                            case BiffRecordNumber.MSODRAWINGSELECTION:
                            case ((BiffRecordNumber) 0xee):
                            case BiffRecordNumber.MERGE_DONTKNOW:
                            case BiffRecordNumber.SXRULE:
                            case BiffRecordNumber.SXEX:
                            case BiffRecordNumber.SXFILT:
                            case ((BiffRecordNumber) 0xf3):
                            case ((BiffRecordNumber) 0xf4):
                            case ((BiffRecordNumber) 0xf5):
                            case BiffRecordNumber.SXNAME:
                            case BiffRecordNumber.SXSELECT:
                            case BiffRecordNumber.SXPAIR:
                            case BiffRecordNumber.SXFMLA:
                            case ((BiffRecordNumber) 250):
                            case BiffRecordNumber.SXFORMAT:
                            case ((BiffRecordNumber) 0xfe):
                            case BiffRecordNumber.SXVDEX:
                            case ((BiffRecordNumber) 0x101):
                            case ((BiffRecordNumber) 0x102):
                            case BiffRecordNumber.SXFORMULA:
                            case BiffRecordNumber.SXDBEX:
                                return;

                            case BiffRecordNumber.FORMULA2:
                                this.ReadCellType(biff, sheet);
                                return;

                            case BiffRecordNumber.EOF:
                                this.ReadEOF(biff, sheet);
                                return;

                            case BiffRecordNumber.CALCCOUNT:
                                this.ReadCalcCount(biff, sheet);
                                return;

                            case BiffRecordNumber.CALCMODE:
                                this.ReadCalcMode(biff, sheet);
                                return;

                            case BiffRecordNumber.PRECISION:
                                this.ReadPrecision(biff, sheet);
                                return;

                            case BiffRecordNumber.REFMODE:
                                this.ReadRefMode(biff, sheet);
                                return;

                            case BiffRecordNumber.DELTA:
                                this.ReadDelta(biff, sheet);
                                return;

                            case BiffRecordNumber.ITERATION:
                                this.ReadIteration(biff, sheet);
                                return;

                            case BiffRecordNumber.PROTECT:
                                this.ReadProtect(biff, sheet);
                                return;

                            case BiffRecordNumber.HEADER:
                                this.ReadHeader(biff, sheet);
                                return;

                            case BiffRecordNumber.FOOTER:
                                this.ReadFooter(biff, sheet);
                                return;

                            case BiffRecordNumber.EXTERNCOUNT:
                                this.ReadExternCount(biff, sheet);
                                return;

                            case BiffRecordNumber.EXTERNSHEET:
                                if (this._excelReader is IExcelLosslessReader)
                                {
                                    UnsupportRecord unsupportRecord = new UnsupportRecord {
                                        FileType = ExcelFileType.XLS,
                                        Category = RecordCategory.Formula,
                                        Value = biff
                                    };
                                    (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, unsupportRecord);
                                    this.LogUnsupportedBiffRecord(sheet, biff);
                                }
                                this.ReadExternSheet(biff, sheet);
                                return;

                            case BiffRecordNumber.NAME:
                                this.ReadName(biff, sheet);
                                return;

                            case BiffRecordNumber.VERTICALPAGEBREAKS:
                                this.ReadVerticalPageBreaks(biff, sheet);
                                return;

                            case BiffRecordNumber.HORIZONTALPAGEBREAKS:
                                this.ReadHorizontalPageBreaks(biff, sheet);
                                return;

                            case BiffRecordNumber.NOTE:
                                this.ReadNote(biff, sheet);
                                return;

                            case BiffRecordNumber.SELECTION:
                                this.ReadSelection(biff, sheet);
                                return;

                            case BiffRecordNumber.DATE1904:
                                this.ReadDate1904(biff, sheet);
                                return;

                            case BiffRecordNumber.EXTERNNAME:
                                this.ReadExternName(biff, sheet);
                                return;

                            case BiffRecordNumber.LEFTMARGIN:
                                this.ReadLeftMargin(biff, sheet);
                                return;

                            case BiffRecordNumber.RIGHTMARGIN:
                                this.ReadRightMargin(biff, sheet);
                                return;

                            case BiffRecordNumber.TOPMARGIN:
                                this.ReadTopMargin(biff, sheet);
                                return;

                            case BiffRecordNumber.BOTTOMMARGIN:
                                this.ReadBottomMargin(biff, sheet);
                                return;

                            case BiffRecordNumber.PRINTHEADERS:
                                this.ReadPrintHeaders(biff, sheet);
                                return;

                            case BiffRecordNumber.PRINTGRIDLINES:
                                this.ReadPrintGridLines(biff, sheet);
                                return;

                            case BiffRecordNumber.FONT:
                                this.ReadFont(biff, sheet);
                                return;

                            case BiffRecordNumber.CONTINUE:
                                this.ReadContinue(biff, sheet);
                                return;

                            case BiffRecordNumber.WINDOW1:
                                this.ReadUnsupportRecord(biff, sheet);
                                this.ReadWindow1(biff, sheet);
                                return;

                            case BiffRecordNumber.PANE:
                                this.ReadPane(biff, sheet);
                                return;

                            case BiffRecordNumber.CODEPAGE:
                                this.ReadCodePage(biff, sheet);
                                return;

                            case BiffRecordNumber.DEFCOLWIDTH:
                                this.ReadDefaultColumnWidth(biff, sheet);
                                return;

                            case BiffRecordNumber.OBJ:
                                this.ReadOBJ(biff, sheet);
                                return;

                            case BiffRecordNumber.SAVERECALC:
                                this.ReadSaveReCalc(biff, sheet);
                                return;

                            case BiffRecordNumber.COLINFO:
                                this.ReadColumnInfo(biff, sheet);
                                return;

                            case BiffRecordNumber.GUTS:
                                this.ReadGuts(biff, sheet);
                                return;

                            case BiffRecordNumber.WSBOOL:
                                this.ReadWSBool(biff, sheet);
                                return;

                            case BiffRecordNumber.HCENTER:
                                this.ReadHCenter(biff, sheet);
                                return;

                            case BiffRecordNumber.VCENTER:
                                this.ReadVCenter(biff, sheet);
                                return;

                            case BiffRecordNumber.COUNTRY:
                                this.ReadCountry(biff, sheet);
                                return;

                            case BiffRecordNumber.HIDEOBJ:
                                this.ReadUnsupportRecord(biff, sheet);
                                return;

                            case BiffRecordNumber.AUTOFILTERINFO:
                                this.ReadAutoFilterInfo(biff, sheet);
                                return;

                            case BiffRecordNumber.AUTOFILTER:
                                this.ReadAutoFilter(biff, sheet);
                                return;

                            case BiffRecordNumber.SCL:
                                this.ReadSCL(biff, sheet);
                                return;

                            case BiffRecordNumber.SETUP:
                                this.ReadSetup(biff, sheet);
                                return;

                            case BiffRecordNumber.SHRFMLA:
                                this.ReadShareFormula(biff, sheet);
                                return;

                            case BiffRecordNumber.MULRK:
                                this.ReadMulCellType(biff, sheet);
                                return;

                            case BiffRecordNumber.MULBLANK:
                                this.ReadMulCellType(biff, sheet);
                                return;

                            case BiffRecordNumber.RSTRING:
                                this.ReadCellType(biff, sheet);
                                return;

                            case BiffRecordNumber.BOOKBOOL:
                                this.ReadBookBool(biff, sheet);
                                return;

                            case BiffRecordNumber.XF:
                                this.ReadXF(biff, sheet);
                                return;

                            case BiffRecordNumber.MERGECELLS:
                                this.ReadMergeCells(biff, sheet);
                                return;

                            case BiffRecordNumber.SST:
                                this.ReadSST(biff, sheet);
                                return;

                            case BiffRecordNumber.LABELSST:
                                this.ReadCellType(biff, sheet);
                                return;

                            case BiffRecordNumber.EXTSST:
                                this.ReadUnsupportRecord(biff, sheet);
                                return;
                        }
                        return;
                    }
                    switch (recordType)
                    {
                        case BiffRecordNumber.USESELFS:
                        case BiffRecordNumber.DSF:
                        case BiffRecordNumber.XL5MODIFY:
                        case BiffRecordNumber.TABID:
                        case BiffRecordNumber.FILESHARING2:
                        case ((BiffRecordNumber) 0x1a5):
                        case ((BiffRecordNumber) 0x1a6):
                        case ((BiffRecordNumber) 0x1a7):
                        case ((BiffRecordNumber) 0x1a8):
                        case BiffRecordNumber.USERBVIEW:
                        case BiffRecordNumber.USERSVIEWBEGIN:
                        case BiffRecordNumber.USERSVIEWEND:
                        case ((BiffRecordNumber) 0x1ac):
                        case BiffRecordNumber.QSI:
                        case BiffRecordNumber.PROT4REV:
                        case ((BiffRecordNumber) 0x1b3):
                        case ((BiffRecordNumber) 0x1b4):
                        case ((BiffRecordNumber) 0x1b5):
                        case BiffRecordNumber.REFRESHALL:
                        case ((BiffRecordNumber) 0x1b9):
                        case BiffRecordNumber.SXFDBTYPE:
                        case BiffRecordNumber.PROT4REVPASS:
                        case ((BiffRecordNumber) 0x1bd):
                        case ((BiffRecordNumber) 0x1bf):
                            return;

                        case BiffRecordNumber.SUPBOOK:
                            this.ReadSupBook(biff, sheet);
                            return;

                        case BiffRecordNumber.CONDFMT:
                            this.ReadCondFmt(biff, sheet);
                            return;

                        case BiffRecordNumber.CF:
                            this.ReadConditionalFormating(biff, sheet);
                            return;

                        case BiffRecordNumber.DVAL:
                            this.ReadDVAL(biff, sheet);
                            return;

                        case BiffRecordNumber.TXO:
                            this.ReadTXO(biff, sheet);
                            return;

                        case BiffRecordNumber.HLINK:
                            this.ReadCellType(biff, sheet);
                            return;

                        case BiffRecordNumber.CODENAME:
                            if (this._excelReader is IExcelLosslessReader)
                            {
                                UnsupportRecord record = new UnsupportRecord {
                                    FileType = ExcelFileType.XLS,
                                    Category = RecordCategory.VBA,
                                    Value = biff
                                };
                                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, record);
                            }
                            this.LogUnsupportedBiffRecord(sheet, biff);
                            return;

                        case BiffRecordNumber.DV:
                            this.ReadDV(biff, sheet);
                            return;

                        case BiffRecordNumber.EXCEL9FILE:
                            goto Label_0B4F;
                    }
                    return;
                }
                switch (recordType)
                {
                    case BiffRecordNumber.ARRAY:
                        this.ReadArrayFormula(biff, sheet);
                        return;

                    case (BiffRecordNumber.DIMENSIONS | BiffRecordNumber.DATE1904):
                    case BiffRecordNumber.EXTERNNAME2:
                    case ((BiffRecordNumber) 0x224):
                    case BiffRecordNumber.TABLE:
                    case ((BiffRecordNumber) 0x202):
                    case ((BiffRecordNumber) 0x206):
                    case BiffRecordNumber.BOF_BIFF3:
                    case ((BiffRecordNumber) 0x20a):
                    case BiffRecordNumber.INDEX:
                        return;

                    case BiffRecordNumber.DEFAULTROWHEIGHT:
                        this.ReadDefaultRowHeight(biff, sheet);
                        return;

                    case BiffRecordNumber.WINDOW2:
                        this.ReadWindow2(biff, sheet);
                        return;

                    case BiffRecordNumber.DIMENSIONS:
                        this.ReadDimensions(biff, sheet);
                        return;

                    case BiffRecordNumber.BLANK:
                        this.ReadCellType(biff, sheet);
                        return;

                    case BiffRecordNumber.NUMBER:
                        this.ReadCellType(biff, sheet);
                        return;

                    case BiffRecordNumber.LABEL:
                        this.ReadCellType(biff, sheet);
                        return;

                    case BiffRecordNumber.BOOLERR:
                        this.ReadCellType(biff, sheet);
                        return;

                    case BiffRecordNumber.STRING:
                        this.ReadString(biff, sheet);
                        return;

                    case BiffRecordNumber.ROW:
                        this.ReadRow(biff, sheet);
                        return;

                    case BiffRecordNumber.NAME2:
                        this.ReadName(biff, sheet);
                        return;
                }
                return;
            }
            if (recordType <= BiffRecordNumber.NUMBERFORMAT)
            {
                if (recordType > BiffRecordNumber.STYLE)
                {
                    switch (recordType)
                    {
                        case BiffRecordNumber.FORMULA:
                            this.ReadCellType(biff, sheet);
                            return;

                        case BiffRecordNumber.BOF_BIFF4:
                            return;

                        case BiffRecordNumber.NUMBERFORMAT:
                            this.ReadFormat(biff, sheet);
                            return;
                    }
                    return;
                }
                switch (recordType)
                {
                    case BiffRecordNumber.RK:
                        this.ReadCellType(biff, sheet);
                        return;

                    case BiffRecordNumber.STYLE:
                        this.ReadStyle(biff, sheet);
                        break;
                }
                return;
            }
            if (recordType <= BiffRecordNumber.CONTINUEFRT)
            {
                if (recordType != BiffRecordNumber.SHRFMLA2)
                {
                    if (recordType != BiffRecordNumber.BOF)
                    {
                        if (recordType != BiffRecordNumber.CONTINUEFRT)
                        {
                            return;
                        }
                        return;
                    }
                    this.ReadBOF(biff, sheet);
                    return;
                }
                this.ReadShareFormula(biff, sheet);
                return;
            }
            switch (recordType)
            {
                case BiffRecordNumber.SHEETEXT:
                    this.ReadSheetProperties(biff, sheet);
                    return;

                case BiffRecordNumber.BOOKEXT:
                case BiffRecordNumber.COMPAT12:
                case BiffRecordNumber.GUIDTypeLib:
                case BiffRecordNumber.COMPRESSPICTURE:
                    break;

                case ((BiffRecordNumber) 0x864):
                case ((BiffRecordNumber) 0x865):
                case ((BiffRecordNumber) 0x868):
                case (BiffRecordNumber.BOF | BiffRecordNumber.TEMPLATE):
                case ((BiffRecordNumber) 0x86a):
                case ((BiffRecordNumber) 0x86b):
                case ((BiffRecordNumber) 0x86c):
                case ((BiffRecordNumber) 0x86d):
                case ((BiffRecordNumber) 0x86e):
                case ((BiffRecordNumber) 0x86f):
                case ((BiffRecordNumber) 0x870):
                case ((BiffRecordNumber) 0x873):
                case BiffRecordNumber.CONTINUEFRT11:
                case ((BiffRecordNumber) 0x876):
                case ((BiffRecordNumber) 0x891):
                case ((BiffRecordNumber) 0x893):
                case ((BiffRecordNumber) 0x894):
                case ((BiffRecordNumber) 0x895):
                case ((BiffRecordNumber) 0x898):
                case (BiffRecordNumber.TABLESTYLEELEMENT | BiffRecordNumber.BOF_BIFF2):
                case ((BiffRecordNumber) 0x89a):
                    return;

                case BiffRecordNumber.HFPICTURE:
                    this.ReadUnsupportRecord(biff, sheet);
                    return;

                case BiffRecordNumber.FEATHEADR:
                    this.ReadFeatheadr(biff, sheet);
                    return;

                case BiffRecordNumber.FEATHEADR11:
                    this.ReadFeatheadr11(biff, sheet);
                    return;

                case BiffRecordNumber.FEAT11:
                    this.ReadFeature11(biff, sheet);
                    return;

                case BiffRecordNumber.DROPDOWNOBJIDS:
                    this.ReadDROPDOWNOBJIDS(biff, sheet);
                    return;

                case BiffRecordNumber.LIST12:
                    this.ReadList12(biff, sheet);
                    return;

                case BiffRecordNumber.FEAT12:
                    this.ReadFeature11(biff, sheet);
                    return;

                case BiffRecordNumber.CONDFMT12:
                    this.ReadCondFmt12(biff, sheet);
                    return;

                case BiffRecordNumber.CF12:
                    this.ReadConditionalFormating12(biff, sheet);
                    return;

                case BiffRecordNumber.CFEX:
                    this.ReadConditionalFormatingExtension(biff, sheet);
                    return;

                case BiffRecordNumber.XFCRC:
                    this.ReadXFCRC(biff, sheet);
                    return;

                case BiffRecordNumber.XFEXT:
                    this.ReadXFExt(biff, sheet);
                    return;

                case BiffRecordNumber.AUTOFILTER12:
                    this.ReadAutoFilter12(biff, sheet);
                    return;

                case BiffRecordNumber.CONTINUEFRT12:
                    this.ReadContinueFrt12(biff, sheet);
                    return;

                case BiffRecordNumber.DXF:
                    this.ReadDxf(biff, sheet);
                    return;

                case BiffRecordNumber.TABLESTYLES:
                    this.ReadTableStyles(biff, sheet);
                    return;

                case BiffRecordNumber.TABLESTYLE:
                    this.ReadTableStyle(biff, sheet);
                    return;

                case BiffRecordNumber.TABLESTYLEELEMENT:
                    this.ReadTableStyleElement(biff, sheet);
                    return;

                case BiffRecordNumber.STYLEEXT:
                    this.ReadStyleExt(biff, sheet);
                    return;

                case BiffRecordNumber.THEME:
                    this.ReadUnsupportRecord(biff, sheet);
                    this.ReadTheme(biff, sheet);
                    return;

                case BiffRecordNumber.HEADERFOOTER:
                    this.ReadHeaderFooter(biff, sheet);
                    return;

                case BiffRecordNumber.LISTDV:
                    this.ReadLISTDV(biff, sheet);
                    return;

                default:
                    return;
            }
        Label_0B4F:
            this.ReadUnsupportRecord(biff, sheet);
        }

        private void ProcessBiffs(int excelSheetIndex)
        {
            if (this._excelReader is IExcelLosslessReader)
            {
                IExcelLosslessReader reader = this._excelReader as IExcelLosslessReader;
                List<BiffRecord> biffs = new List<BiffRecord>();
                bool flag = false;
                foreach (BiffRecord record in this._workbookBiffList)
                {
                    if (flag && (record.RecordType == BiffRecordNumber.CONTINUE))
                    {
                        biffs.Add(record);
                    }
                    else if (flag)
                    {
                        flag = false;
                        UnsupportRecord unsupportRecord = new UnsupportRecord {
                            Category = RecordCategory.DrawingGroup,
                            FileType = ExcelFileType.XLS,
                            Value = new List<BiffRecord>((IEnumerable<BiffRecord>) biffs)
                        };
                        reader.AddUnsupportItem(-1, unsupportRecord);
                        this.LogUnsupportedBiffRecords(excelSheetIndex, biffs);
                        biffs.Clear();
                    }
                    if (record.RecordType == BiffRecordNumber.MSODRAWINGGROUP)
                    {
                        biffs.Add(record);
                        flag = true;
                    }
                }
            }
            foreach (BiffRecord record3 in this._workbookBiffList)
            {
                if ((this._continueRecordType != ContinueRecordType.Empty) && (record3.RecordType != BiffRecordNumber.CONTINUE))
                {
                    if (this._continueRecordType == ContinueRecordType.SST)
                    {
                        try
                        {
                            this.ProcessSST();
                        }
                        catch (Exception exception)
                        {
                            this.LogError(ResourceHelper.GetResourceString("sharedStringError"), ExcelWarningCode.General, -1, -1, -1, exception);
                        }
                    }
                    this._continueRecordType = ContinueRecordType.Empty;
                }
                try
                {
                    this.ProcessBiffRecord(-1, record3);
                    this._previousRecord = record3;
                }
                catch (Exception exception2)
                {
                    this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { record3.RecordType.ToString().ToLowerInvariant() }), ExcelWarningCode.General, -1, -1, -1, exception2);
                }
            }
            if (excelSheetIndex == -1)
            {
                for (int i = 0; i < this._worksheetBiffList.Count; i++)
                {
                    byte hiddenState = this._hiddenState[i];
                    if ((((ExcelSheetType) this._sheetTypes[i]) == ExcelSheetType.Worksheet) && (i < this._worksheetBiffList.Count))
                    {
                        foreach (BiffRecord record4 in this._worksheetBiffList[i])
                        {
                            if (record4.RecordType == BiffRecordNumber.WSBOOL)
                            {
                                SimpleBinaryReader reader2 = new SimpleBinaryReader(record4.DataBuffer);
                                if ((reader2.ReadInt16() & 0x10) == 0x10)
                                {
                                    this._sheetTypes[i] = ExcelSheetType.DialogSheet;
                                }
                                break;
                            }
                        }
                    }
                    this._excelReader.AddSheet(this._sheetNames[i], hiddenState, this._sheetTypes[i]);
                }
                for (int j = 0; j < this._worksheetBiffList.Count; j++)
                {
                    this.ResetWorksheetGlobalVariables();
                    this.ProcessSheet(j, this._sheetTypes[j], true);
                }
            }
            else if ((excelSheetIndex >= 0) && (excelSheetIndex < this._worksheetBiffList.Count))
            {
                byte num5 = this._hiddenState[excelSheetIndex];
                this._excelReader.AddSheet(this._sheetNames[excelSheetIndex], num5, this._sheetTypes[excelSheetIndex]);
                this.ResetWorksheetGlobalVariables();
                this.ProcessSheet(excelSheetIndex, this._sheetTypes[excelSheetIndex], false);
            }
            else
            {
                this.LogError(ResourceHelper.GetResourceString("sheetIndexOutOfRange"), ExcelWarningCode.CannotOpen, -1, -1, -1, null);
            }
        }

        private void ProcessSheet(int sheetIndex, ExcelSheetType sheetType, bool allSheet = true)
        {
            short sheet = (short) sheetIndex;
            int num2 = -1;
            int num3 = -1;
            int num4 = -1;
            int? nullable = null;
            List<BiffRecord> supportRecords = null;
            if (this._excelReader is IExcelLosslessReader)
            {
                supportRecords = new List<BiffRecord>();
                List<IUnsupportRecord> list2 = null;
                if (((sheetType == ExcelSheetType.ChartSheet) || (sheetType == ExcelSheetType.MacroSheet)) || ((sheetType == ExcelSheetType.DialogSheet) || (sheetType == ExcelSheetType.VBAModule)))
                {
                    list2 = this.ProcessUnsupportRecord(sheetIndex, this._worksheetBiffList[sheetIndex]);
                }
                else
                {
                    list2 = this.PreProcessUnsupportedRecords(sheetIndex, this._worksheetBiffList[sheetIndex], supportRecords);
                }
                if ((list2 != null) || (list2.Count > 0))
                {
                    IExcelLosslessReader reader = this._excelReader as IExcelLosslessReader;
                    foreach (IUnsupportRecord record in list2)
                    {
                        reader.AddUnsupportItem(sheetIndex, record);
                    }
                }
            }
            if (sheetType == ExcelSheetType.Worksheet)
            {
                if (supportRecords == null)
                {
                    supportRecords = this.PreProcessBiffRecords(this._worksheetBiffList[sheetIndex]);
                }
                if (supportRecords.Count != 0)
                {
                    for (int i = 0; i < supportRecords.Count; i++)
                    {
                        if (supportRecords[i].RecordType == BiffRecordNumber.STANDARDWIDTH)
                        {
                            this._standColWidths[sheetIndex] = supportRecords[i];
                        }
                        if ((supportRecords[i].RecordType == BiffRecordNumber.COLINFO) && !nullable.HasValue)
                        {
                            nullable = new int?(i);
                        }
                        if (supportRecords[i].RecordType == BiffRecordNumber.DIMENSIONS)
                        {
                            num2 = i;
                        }
                        if (supportRecords[i].RecordType == BiffRecordNumber.PANE)
                        {
                            num3 = i;
                        }
                        if (supportRecords[i].RecordType == BiffRecordNumber.WINDOW2)
                        {
                            num4 = i;
                        }
                    }
                    if (((num2 != -1) && (num3 != -1)) && ((num4 != -1) && (num3 > num4)))
                    {
                        if (nullable.HasValue && (nullable.Value < num2))
                        {
                            BiffRecord record2 = supportRecords[num3];
                            BiffRecord record3 = supportRecords[num4];
                            BiffRecord record4 = supportRecords[num2];
                            supportRecords.RemoveAt(num3);
                            supportRecords.RemoveAt(num4);
                            supportRecords.RemoveAt(num2);
                            supportRecords.Insert(nullable.Value, record2);
                            supportRecords.Insert(nullable.Value, record3);
                            supportRecords.Insert(nullable.Value, record4);
                        }
                        else
                        {
                            BiffRecord record5 = supportRecords[num3];
                            BiffRecord record6 = supportRecords[num4];
                            supportRecords.RemoveAt(num3);
                            supportRecords.RemoveAt(num4);
                            supportRecords.Insert(num2 + 1, record5);
                            supportRecords.Insert(num2 + 1, record6);
                        }
                    }
                    else if (((num2 != -1) && nullable.HasValue) && (nullable.Value < num2))
                    {
                        BiffRecord record7 = supportRecords[num2];
                        supportRecords.RemoveAt(num2);
                        supportRecords.Insert(nullable.Value, record7);
                    }
                    int num6 = -1;
                    num3 = -1;
                    for (int j = 0; j < supportRecords.Count; j++)
                    {
                        if (supportRecords[j].RecordType == BiffRecordNumber.PANE)
                        {
                            num3 = j;
                        }
                        if (supportRecords[j].RecordType == BiffRecordNumber.SCL)
                        {
                            num6 = j;
                        }
                    }
                    if (((num6 >= 0) && (num3 >= 0)) && (num3 < num6))
                    {
                        BiffRecord record8 = supportRecords[num6];
                        supportRecords.RemoveAt(num6);
                        supportRecords.Insert(num3, record8);
                    }
                    foreach (BiffRecord record9 in supportRecords)
                    {
                        try
                        {
                            if (allSheet)
                            {
                                this.ProcessBiffRecord(sheet, record9);
                            }
                            else
                            {
                                this.ProcessBiffRecord(0, record9);
                            }
                            this._previousRecord = record9;
                        }
                        catch (Exception exception)
                        {
                            this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { record9.RecordType.ToString().ToLowerInvariant() }), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
                        }
                    }
                }
            }
        }

        private void ProcessSST()
        {
            int num = 0;
            int length = 0;
            //hdt
            int num3 = 0;
            BinaryReader @this = null;
            string str = null;
            length = this._SSTBuffers[0].Length;
            if (length > 0)
            {
                @this = new BinaryReader((Stream) new MemoryStream(this._SSTBuffers[0]));
            }
            @this.ReadInt32();
            num = @this.ReadInt32();
            this._sst = new string[num];
            for (int i = 0; i < num; i++)
            {
                int num10;
                short num5 = 0;
                bool flag = false;
                byte num6 = 0;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                short num7 = 0;
                bool flag6 = false;
                int num8 = 0;
                bool flag7 = false;
                byte[] bytes = null;
                bool flag8 = false;
                byte[] buffer2 = null;
                byte[] buffer3 = null;
                int count = 0;
                try
                {
                    num5 = @this.ReadInt16();
                    flag = true;
                    num6 = @this.ReadByte();
                    flag2 = true;
                }
                catch
                {
                    @this.Close();
                    if (this._SSTBuffers[++num3] == null)
                    {
                        return;
                    }
                    @this = new BinaryReader((Stream) new MemoryStream(this._SSTBuffers[num3]));
                    if (!flag)
                    {
                        num5 = @this.ReadInt16();
                        flag = true;
                    }
                    if (!flag2)
                    {
                        num6 = @this.ReadByte();
                        flag2 = true;
                    }
                }
                finally
                {
                    flag3 = (num6 & 1) == 1;
                    flag4 = (num6 & 4) == 4;
                    flag5 = (num6 & 8) == 8;
                }
                try
                {
                    if (flag5)
                    {
                        num7 = @this.ReadInt16();
                        flag6 = true;
                    }
                    if (flag4)
                    {
                        num8 = @this.ReadInt32();
                        flag7 = true;
                    }
                }
                catch
                {
                    @this.Close();
                    if (this._SSTBuffers[++num3] == null)
                    {
                        return;
                    }
                    @this = new BinaryReader((Stream) new MemoryStream(this._SSTBuffers[num3]));
                    if (!flag6)
                    {
                        num7 = @this.ReadInt16();
                        flag6 = true;
                    }
                    if (!flag7)
                    {
                        num8 = @this.ReadInt32();
                        flag7 = true;
                    }
                }
                if (num5 == 0)
                {
                    this._sst[i] = null;
                    continue;
                }
                if (flag3)
                {
                    count = num5 * 2;
                }
                else
                {
                    count = num5;
                }
                try
                {
                    bytes = @this.ReadBytes(count);
                }
                catch
                {
                }
                int num11 = bytes.Length;
                if (!flag3)
                {
                    int num12 = Math.Min((int) num5, (int) bytes.Length);
                    byte[] buffer4 = new byte[num12 * 2];
                    num10 = 0;
                    while (num10 < num12)
                    {
                        buffer4[num10 * 2] = bytes[num10];
                        buffer4[(num10 * 2) + 1] = 0;
                        num10++;
                    }
                    bytes = buffer4;
                }
                str = Encoding.Unicode.GetString(bytes, 0, bytes.Length);
                int num13 = count - num11;
                int num14 = (num7 * 4) + num8;
                while ((num13 > 0) || (num14 > 0))
                {
                    if (num13 == 0)
                    {
                        try
                        {
                            buffer3 = @this.ReadBytes(num14);
                        }
                        catch
                        {
                        }
                        num14 -= buffer3.Length;
                        if (num14 == 0)
                        {
                            break;
                        }
                        if (this._SSTBuffers[++num3] == null)
                        {
                            return;
                        }
                        @this.Close();
                        @this = new BinaryReader((Stream) new MemoryStream(this._SSTBuffers[num3]));
                        continue;
                    }
                    if (this._SSTBuffers[++num3] == null)
                    {
                        return;
                    }
                    @this.Close();
                    @this = new BinaryReader((Stream) new MemoryStream(this._SSTBuffers[num3]));
                    flag8 = @this.ReadByte() == 1;
                    if (flag8 && !flag3)
                    {
                        num13 *= 2;
                    }
                    else if (!flag8 && flag3)
                    {
                        num13 /= 2;
                    }
                    buffer2 = @this.ReadBytes(num13);
                    if (flag8 && !flag3)
                    {
                        num11 += buffer2.Length / 2;
                    }
                    else if (!flag8 && flag3)
                    {
                        num11 += buffer2.Length * 2;
                    }
                    else
                    {
                        num11 += buffer2.Length;
                    }
                    if (buffer2 != null)
                    {
                        if (!flag8)
                        {
                            int num15 = buffer2.Length;
                            byte[] buffer5 = new byte[num15 * 2];
                            for (num10 = 0; num10 < num15; num10++)
                            {
                                buffer5[num10 * 2] = buffer2[num10];
                                buffer5[(num10 * 2) + 1] = 0;
                            }
                            buffer2 = buffer5;
                        }
                        string str2 = Encoding.Unicode.GetString(buffer2, 0, buffer2.Length);
                        str = str + str2;
                        buffer2 = null;
                    }
                    num13 = count - num11;
                }
                this._sst[i] = Convert.ToString(str, (IFormatProvider) CultureInfo.InvariantCulture);
            }
        }

        public void ProcessStream(Stream stream, int excelSheetIndex)
        {
            this.ClearCache();
            if (excelSheetIndex < -1)
            {
                this.LogError(ResourceHelper.GetResourceString("sheetIndexOutOfRange"), ExcelWarningCode.CannotOpen, -1, -1, -1, null);
            }
            else
            {
                if (excelSheetIndex >= 0)
                {
                    this._readAllSheet = false;
                }
                BiffRecord record = null;
                BinaryReader reader = new BinaryReader(stream);
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                int index = -1;
                try
                {
                    while (stream.Position < stream.Length)
                    {
                        record = new BiffRecord {
                            RecordType = (BiffRecordNumber) reader.ReadInt16(),
                            DataLength = reader.ReadInt16(),
                            DataBuffer = reader.ReadBytes(record.DataLength)
                        };
                        if (record.RecordType == BiffRecordNumber.BOF)
                        {
                            SimpleBinaryReader reader2 = new SimpleBinaryReader(record.DataBuffer);
                            if (reader2.ReadUInt16() != 0x600)
                            {
                                this.LogError(ResourceHelper.GetResourceString("biffEntryError"), ExcelWarningCode.CannotOpen, -1, -1, -1, null);
                                return;
                            }
                            ushort num3 = reader2.ReadUInt16();
                            if (this._boundSheetStartPosition.IndexOf((uint) stream.Position) != -1)
                            {
                                index = this._boundSheetStartPosition.IndexOf((uint) stream.Position);
                                switch (num3)
                                {
                                    case 0x20:
                                        this._sheetTypes.Add(ExcelSheetType.ChartSheet);
                                        break;

                                    case 0x40:
                                        this._sheetTypes.Add(ExcelSheetType.MacroSheet);
                                        break;

                                    case 0x100:
                                        this._sheetTypes.Add(ExcelSheetType.Workspace);
                                        break;

                                    case 6:
                                        goto Label_011D;

                                    case 0x10:
                                        goto Label_012B;
                                }
                            }
                        }
                        goto Label_0161;
                    Label_011D:
                        this._sheetTypes.Add(ExcelSheetType.VBAModule);
                        goto Label_0161;
                    Label_012B:
                        this._sheetTypes.Add(ExcelSheetType.Worksheet);
                    Label_0161:
                        if (record.RecordType == BiffRecordNumber.REFMODE)
                        {
                            if (BitConverter.ToInt16(record.DataBuffer, 0) == 1)
                            {
                                this._isA1ReferenceStyle = true;
                                this._calcProperty.RefMode = ExcelReferenceStyle.A1;
                            }
                            else
                            {
                                this._isA1ReferenceStyle = false;
                                this._calcProperty.RefMode = ExcelReferenceStyle.R1C1;
                            }
                        }
                        if (record.RecordType == BiffRecordNumber.PALETTE)
                        {
                            SimpleBinaryReader reader3 = new SimpleBinaryReader(record.DataBuffer);
                            short num5 = reader3.ReadInt16();
                            Dictionary<int, GcColor> palette = new Dictionary<int, GcColor>();
                            palette.Add(0, ColorExtension.FromArgb(0xff000000));
                            palette.Add(1, ColorExtension.FromArgb(uint.MaxValue));
                            palette.Add(2, ColorExtension.FromArgb(0xffff0000));
                            palette.Add(3, ColorExtension.FromArgb(0xff00ff00));
                            palette.Add(4, ColorExtension.FromArgb(0xff0000ff));
                            palette.Add(5, ColorExtension.FromArgb(0xffffff00));
                            palette.Add(6, ColorExtension.FromArgb(0xffff00ff));
                            palette.Add(7, ColorExtension.FromArgb(0xff00ffff));
                            int num6 = 8;
                            for (int i = 0; i < num5; i++)
                            {
                                palette.Add(num6, GcColor.FromArgb(0xff, reader3.ReadByte(), reader3.ReadByte(), reader3.ReadByte()));
                                reader3.ReadByte();
                                num6++;
                            }
                            this._excelReader.SetColorPalette(palette);
                        }
                        if (record.RecordType == BiffRecordNumber.SELECTION)
                        {
                            SimpleBinaryReader reader4 = new SimpleBinaryReader(record.DataBuffer);
                            byte num8 = reader4.ReadByte();
                            reader4.ReadBytes(8);
                            int num9 = reader4.ReadUInt16();
                            reader4.ReadInt16();
                            int num10 = reader4.ReadByte();
                            if (num8 == 3)
                            {
                                this._selections.Add(new Tuple<int, int>(num9, num10));
                            }
                        }
                        if (index == -1)
                        {
                            this._workbookBiffList.Add(record);
                            if (record.RecordType == BiffRecordNumber.BUNDLESHEET)
                            {
                                string str;
                                SimpleBinaryReader reader5 = new SimpleBinaryReader(record.DataBuffer);
                                uint num11 = reader5.ReadUInt32();
                                byte num12 = reader5.ReadByte();
                                reader5.ReadByte();
                                short charCount = reader5.ReadByte();
                                if (charCount < 1)
                                {
                                    charCount = 1;
                                }
                                if (charCount > 0x1f)
                                {
                                    charCount = 0x1f;
                                }
                                reader5.ReadUncompressedString(charCount, out str);
                                this._boundSheetStartPosition.Add(num11 + 20);
                                this._sheetNames.Add(str);
                                this._hiddenState.Add(num12);
                                this._linkTable.InternalSheetNames = this._sheetNames;
                                this._worksheetBiffList.Add(new List<BiffRecord>());
                            }
                        }
                        else
                        {
                            this._worksheetBiffList[index].Add(record);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("biffRecordFatalError"), ExcelWarningCode.CannotOpen, -1, -1, -1, exception);
                }
                this.ProcessBiffs(excelSheetIndex);
                try
                {
                    this._excelReader.SetDifferentialFormattingRecord(this._dxfs);
                }
                catch (Exception exception2)
                {
                    this.LogError(ResourceHelper.GetResourceString("biffReadDxfError"), ExcelWarningCode.General, -1, -1, -1, exception2);
                }
                foreach (KeyValuePair<int, IExcelAutoFilter> pair in this._autoFilters)
                {
                    try
                    {
                        if ((pair.Key >= 0) && (pair.Value != null))
                        {
                            this._excelReader.SetAutoFilter((short) pair.Key, pair.Value);
                        }
                    }
                    catch (Exception exception3)
                    {
                        this.LogError(string.Format(ResourceHelper.GetResourceString("biffReadAutoFilterError"), (object[]) new object[] { ((int) pair.Key) }), ExcelWarningCode.General, -1, -1, -1, exception3);
                    }
                }
                if (this._excelReader is IExcelTableReader)
                {
                    IExcelTableReader reader6 = this._excelReader as IExcelTableReader;
                    foreach (IExcelTableStyle style in this._tableStyles)
                    {
                        try
                        {
                            reader6.SetTableStyle(style);
                        }
                        catch (Exception exception4)
                        {
                            this.LogError(ResourceHelper.GetResourceString("biffReadTableStyleError"), ExcelWarningCode.General, -1, -1, -1, exception4);
                        }
                    }
                    if (this._tables != null)
                    {
                        foreach (KeyValuePair<int, List<IExcelTable>> pair2 in this._tables)
                        {
                            if (pair2.Value != null)
                            {
                                foreach (IExcelTable table in pair2.Value)
                                {
                                    try
                                    {
                                        reader6.SetTable(pair2.Key, table);
                                    }
                                    catch (Exception exception5)
                                    {
                                        this.LogError(string.Format(ResourceHelper.GetResourceString("biffReadTableError"), (object[]) new object[] { table.Name }), ExcelWarningCode.General, -1, -1, -1, exception5);
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (KeyValuePair<int, List<ExcelConditionalFormat>> pair3 in this._sheetsConditionalFormats)
                {
                    foreach (ExcelConditionalFormat format in pair3.Value)
                    {
                        this._excelReader.SetConditionalFormatting((short) pair3.Key, format);
                    }
                }
                this.ClearCache();
                this._excelReader.Finish();
            }
        }

        private List<IUnsupportRecord> ProcessUnsupportRecord(int sheetIndex, List<BiffRecord> biffRecords)
        {
            List<IUnsupportRecord> list = new List<IUnsupportRecord>();
            int num = 0;
            while (num < biffRecords.Count)
            {
                BiffRecord biff = biffRecords[num];
                if ((((biff.RecordType == BiffRecordNumber.SXVIEW) || (biff.RecordType == BiffRecordNumber.SXVI)) || ((biff.RecordType == BiffRecordNumber.SXDB) || (biff.RecordType == BiffRecordNumber.SXDI))) || ((((biff.RecordType == BiffRecordNumber.SXVS) || (biff.RecordType == BiffRecordNumber.SXFORMAT)) || ((biff.RecordType == BiffRecordNumber.SXFORMULA) || (biff.RecordType == BiffRecordNumber.SXLI))) || ((biff.RecordType == BiffRecordNumber.SXNAME) || (biff.RecordType == BiffRecordNumber.SXPI))))
                {
                    this.LogUnsupportedBiffRecord(sheetIndex, biff);
                    return null;
                }
                if ((biff.RecordType != BiffRecordNumber.TXO) && (biff.RecordType != BiffRecordNumber.FONTX))
                {
                    UnsupportRecord record2 = new UnsupportRecord {
                        Category = RecordCategory.Drawing,
                        FileType = ExcelFileType.XLS,
                        Value = biff
                    };
                    list.Add(record2);
                    this.LogUnsupportedBiffRecord(sheetIndex, biff);
                    num++;
                }
                else
                {
                    if (biff.RecordType == BiffRecordNumber.FONTX)
                    {
                        FontX tx = new FontX();
                        BinaryReader reader = new BinaryReader((Stream) new MemoryStream(biff.DataBuffer));
                        tx.Read(reader);
                        tx.Font = this._fonts[tx.ifnt];
                        UnsupportRecord record3 = new UnsupportRecord {
                            Category = RecordCategory.FontX,
                            FileType = ExcelFileType.XLS,
                            Value = tx
                        };
                        list.Add(record3);
                        this.LogUnsupportedBiffRecord(sheetIndex, biff);
                        num++;
                        continue;
                    }
                    List<BiffRecord> list2 = new List<BiffRecord> {
                        biffRecords[num]
                    };
                    int num2 = num + 1;
                    while ((num2 < biffRecords.Count) && (biffRecords[num2].RecordType == BiffRecordNumber.CONTINUE))
                    {
                        list2.Add(biffRecords[num2]);
                        num2++;
                    }
                    num = num2;
                    UnsupportRecord record7 = new UnsupportRecord {
                        Category = RecordCategory.Drawing,
                        FileType = ExcelFileType.XLS,
                        Value = list2[0]
                    };
                    list.Add(record7);
                    this.LogUnsupportedBiffRecord(sheetIndex, biff);
                    if (list2.Count > 1)
                    {
                        UnsupportRecord record4 = new UnsupportRecord {
                            Category = RecordCategory.Drawing,
                            FileType = ExcelFileType.XLS,
                            Value = list2[1]
                        };
                        list.Add(record4);
                        this.LogUnsupportedBiffRecord(sheetIndex, biff);
                    }
                    if (list2.Count > 2)
                    {
                        BiffRecord record5 = list2[2];
                        BinaryReader reader2 = new BinaryReader((Stream) new MemoryStream(record5.DataBuffer));
                        TXORuns runs = new TXORuns(record5.DataLength);
                        runs.Read(reader2);
                        if (runs.rgTXORuns != null)
                        {
                            foreach (Run run in runs.rgTXORuns)
                            {
                                run.Font = this._fonts[run.ifnt];
                            }
                        }
                        List<BiffRecord> list3 = null;
                        if (list2.Count > 3)
                        {
                            list3 = new List<BiffRecord>();
                            for (int i = 3; i < list2.Count; i++)
                            {
                                list3.Add(list2[i]);
                            }
                        }
                        runs.ContinueRecords = list3;
                        UnsupportRecord record6 = new UnsupportRecord {
                            Category = RecordCategory.TextRun,
                            FileType = ExcelFileType.XLS,
                            Value = runs
                        };
                        list.Add(record6);
                        this.LogUnsupportedBiffRecord(sheetIndex, record5);
                        this.LogUnsupportedBiffRecords(sheetIndex, runs.ContinueRecords);
                    }
                }
            }
            return list;
        }

        private void ReadArrayFormula(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int rowFirst = reader.ReadUInt16();
            int rowLast = reader.ReadUInt16();
            short columnFirst = reader.ReadByte();
            short columnLast = reader.ReadByte();
            short num5 = reader.ReadInt16();
            reader.ReadInt32();
            short count = reader.ReadInt16();
            byte[] buffer = reader.ReadBytes(count);
            byte[] buffer2 = null;
            if (((biff.DataLength - count) - 14) > 0)
            {
                buffer2 = reader.ReadBytes((biff.DataLength - count) - 14);
            }
            string str = null;
            ExcelFormula arrayFormula = new ExcelFormula();
            using (BinaryReader reader2 = new BinaryReader((Stream) new MemoryStream(buffer)))
            {
                BinaryReader @this = null;
                try
                {
                    if (buffer2 != null)
                    {
                        @this = new BinaryReader((Stream) new MemoryStream(buffer2));
                    }
                    ExcelCalcError error = null;
                    this.fp.formulaReader = reader2;
                    this.fp.formulaExtraReader = @this;
                    this.fp.sheet = this._formulaPendingSheet;
                    this.fp.row = this._formulaPendingRow;
                    this.fp.column = this._formulaPendingCol;
                    this.fp.isA1RefStyle = this._isA1ReferenceStyle;
                    this.fp.SheetsSelectionList = this._selections;
                    str = this.fp.ToString(this._linkTable, ref error, new int?(rowFirst), new int?(columnFirst), false);
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("readCellFormulaError"), ExcelWarningCode.FormulaError, -1, -1, -1, exception);
                }
                finally
                {
                    if (@this != null)
                    {
                        @this.Close();
                    }
                }
            }
            if (this._isA1ReferenceStyle)
            {
                arrayFormula.IsArrayFormula = true;
                arrayFormula.SetFormula(str);
                arrayFormula.FormulaTokenBits = buffer;
                arrayFormula.FormulaTokenExtraBits = buffer2;
            }
            else
            {
                arrayFormula.IsArrayFormula = true;
                arrayFormula.SetFormulaR1C1(str);
                arrayFormula.FormulaR1C1TokenBits = buffer;
                arrayFormula.FormulaR1C1TokenExtraBits = buffer2;
            }
            this._excelReader.SetArrayFormula(sheetIndex, rowFirst, rowLast, columnFirst, columnLast, arrayFormula);
        }

        private void ReadAutoFilter(BiffRecord biff, short sheetIndex)
        {
            try
            {
                this._isAutoFilter = true;
                this._isAutoFilter12 = false;
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                ushort num = reader.ReadUInt16();
                ExcelFilterColumn filterColumn = this._autoFilters[sheetIndex].FilterColumns[num] as ExcelFilterColumn;
                this.ReadAutoFilterColumn(reader, filterColumn);
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("readAutoFilterError"), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
            }
        }

        private void ReadAutoFilter12(BiffRecord biff, short sheetIndex)
        {
            try
            {
                this._isAutoFilter = false;
                this._isAutoFilter12 = true;
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                reader.ReadInt16();
                reader.ReadInt16();
                this.GetRange(reader);
                ushort num = reader.ReadUInt16();
                reader.ReadUInt32();
                uint num2 = reader.ReadUInt32();
                uint num3 = reader.ReadUInt32();
                uint num4 = reader.ReadUInt32();
                uint num5 = reader.ReadUInt32();
                ushort num6 = reader.ReadUInt16();
                reader.ReadUInt32();
                reader.ReadUInt32();
                reader.ReadBytes(0x10);
                ExcelFilterColumn column = null;
                if (!this._isInFeature11)
                {
                    column = this._autoFilters[sheetIndex].FilterColumns[num] as ExcelFilterColumn;
                }
                else
                {
                    if (this._sheetTables.Count <= 0)
                    {
                        return;
                    }
                    IExcelTable table = this._sheetTables[this._sheetTables.Count - 1];
                    if (table.AutoFilter == null)
                    {
                        return;
                    }
                    column = table.AutoFilter.FilterColumns[num] as ExcelFilterColumn;
                }
                ExcelDynamicFilter filter3 = new ExcelDynamicFilter {
                    Type = (ExcelDynamicFilterType) num3
                };
                column.DynamicFilter = filter3;
                switch (num2)
                {
                    case 1:
                    case 2:
                        break;

                    case 3:
                    {
                        ExcelIconFilter filter2 = new ExcelIconFilter {
                            IconSet = (ExcelIconSetType) reader.ReadInt32(),
                            IconId = reader.ReadUInt32()
                        };
                        column.IconFilter = filter2;
                        return;
                    }
                    case 0:
                        if (num4 > 0)
                        {
                            this._readContinueFRT12_cCriteria = new uint?(num4);
                            if (this._readContinueFRT12_filterColumn == null)
                            {
                                this._readContinueFRT12_filterColumn = column;
                            }
                        }
                        if (num5 > 0)
                        {
                            this._readContinueFRT12_cDateGroupings = new uint?(num5);
                            if (this._readContinueFRT12_filterColumn == null)
                            {
                                this._readContinueFRT12_filterColumn = column;
                            }
                        }
                        return;

                    default:
                        return;
                }
                ExcelColorFilter filter = new ExcelColorFilter();
                DifferentialFormatting dxfExt = this.ReadDifferentialFormattingRecord(reader);
                if (reader.Remaining > 0)
                {
                    this.ReadXFExtNoFRT(reader, dxfExt);
                }
                if (!this._dxfs.Contains(dxfExt))
                {
                    filter.DxfId = (uint) this._dxfs.Count;
                    this._dxfs.Add(dxfExt);
                }
                else
                {
                    filter.DxfId = (uint) this._dxfs.IndexOf(dxfExt);
                }
                if (num2 == 2)
                {
                    filter.CellColor = false;
                }
                column.ColorFilter = filter;
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("readAutoFilterError"), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
            }
        }

        private void ReadAutoFilterColumn(SimpleBinaryReader reader, ExcelFilterColumn filterColumn)
        {
            ExcelCustomFilter filter;
            int num3;
            ExcelCustomFilter filter2;
            int num4;
            short num = reader.ReadInt16();
            bool flag = (num & 3) == 0;
            bool flag2 = (num & 0x10) == 0x10;
            bool flag3 = (num & 0x20) == 0x20;
            bool flag4 = (num & 0x40) == 0x40;
            int num2 = (num & 0xff80) >> 7;
            if (flag2)
            {
                ExcelTop10 top = new ExcelTop10 {
                    Top = flag3,
                    Percent = flag4,
                    Value = num2
                };
                filterColumn.Top10 = top;
            }
            ExcelCustomFilters filters = new ExcelCustomFilters {
                And = flag
            };
            this.ReadDOper(reader, out filter, out num3);
            this.ReadDOper(reader, out filter2, out num4);
            if (filter != null)
            {
                if (num3 > 0)
                {
                    string str;
                    reader.ReadUncompressedString(num3, out str);
                    filter.Value = str;
                }
                filters.Filter1 = filter;
            }
            if (filter2 != null)
            {
                if (num4 > 0)
                {
                    string str2;
                    reader.ReadUncompressedString(num4, out str2);
                    filter2.Value = str2;
                }
                filters.Filter2 = filter2;
            }
            if ((filters.Filter1 != null) || (filters.Filter2 != null))
            {
                filterColumn.CustomFilters = filters;
            }
        }

        private void ReadAutoFilterInfo(BiffRecord biff, short sheetIndex)
        {
            this._autoFilterSheets.Add(sheetIndex);
            this._autoFilterDropDownArrowCount = BitConverter.ToInt16(biff.DataBuffer, 0);
            IExcelAutoFilter filter = this._autoFilters[sheetIndex];
            if (filter.FilterColumns == null)
            {
                filter.FilterColumns = new List<IExcelFilterColumn>();
            }
            for (int i = 0; i < this._autoFilterDropDownArrowCount; i++)
            {
                ExcelFilterColumn column = new ExcelFilterColumn {
                    AutoFilterColumnId = (uint) i
                };
                filter.FilterColumns.Add(column);
            }
        }

        private void ReadBOF(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadUInt16();
            if (((reader.ReadUInt16() != 5) && (sheetIndex != -1)) && (sheetIndex < this._sheetTypes.Count))
            {
                switch (this._sheetTypes[sheetIndex])
                {
                    case ExcelSheetType.Worksheet:
                        this._excelReader.Bof(sheetIndex);
                        this._txoStringTable.Clear();
                        this._isPanesFrozen = false;
                        return;

                    case ExcelSheetType.DialogSheet:
                    case ExcelSheetType.MacroSheet:
                    case ExcelSheetType.ChartSheet:
                    case ExcelSheetType.VBAModule:
                    case ExcelSheetType.Workspace:
                        return;
                }
            }
        }

        private void ReadBookBool(BiffRecord biff, short sheetIndex)
        {
            short num = new SimpleBinaryReader(biff.DataBuffer).ReadInt16();
            this._workbookProperty.SaveExternalLinks = num != 1;
        }

        private void ReadBottomMargin(BiffRecord biff, short sheetIndex)
        {
            double num = BitConverter.ToDouble(biff.DataBuffer, 0);
            this._printPageMargin.Bottom = num;
        }

        private void ReadCalcCount(BiffRecord biff, short sheetIndex)
        {
            int num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._calcProperty.MaxIterationCount = num;
        }

        private void ReadCalcMode(BiffRecord biff, short sheetIndex)
        {
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._calcProperty.CalculationMode = (ExcelCalculationMode) num;
        }

        private void ReadCellType(BiffRecord biff, short sheetIndex)
        {
            bool isFloat = false;
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int row = reader.ReadUInt16();
            int column = reader.ReadUInt16();
            int formatIndex = reader.ReadInt16();
            double num4 = 0.0;
            try
            {
                byte num5;
                ExcelCalcError illegalOrDeletedCellReference;
                byte[] buffer;
                string str6;
                ExcelFormula formula;
                byte num23;
                switch (biff.RecordType)
                {
                    case BiffRecordNumber.BLANK:
                        this._excelReader.SetCell(sheetIndex, row, column, null, CellType.Blank, formatIndex, null);
                        return;

                    case ((BiffRecordNumber) 0x202):
                        return;

                    case BiffRecordNumber.NUMBER:
                        num4 = reader.ReadDouble();
                        this._excelReader.SetCell(sheetIndex, row, column, (double) num4, CellType.Numeric, formatIndex, null);
                        return;

                    case BiffRecordNumber.LABEL:
                        string str;
                        if (reader.ReadCompressedString(2, out str))
                        {
                            this._excelReader.SetCell(sheetIndex, row, column, !string.IsNullOrEmpty(str) ? str : string.Empty, CellType.String, formatIndex, null);
                        }
                        return;

                    case BiffRecordNumber.BOOLERR:
                        num5 = reader.ReadByte();
                        if (!reader.ReadBoolean())
                        {
                            goto Label_0180;
                        }
                        illegalOrDeletedCellReference = null;
                        num23 = num5;
                        if (num23 > 15)
                        {
                            break;
                        }
                        switch (num23)
                        {
                            case 0:
                                goto Label_0120;

                            case 7:
                                goto Label_0129;

                            case 15:
                                goto Label_0132;
                        }
                        goto Label_0166;

                    case BiffRecordNumber.RK:
                        num4 = this.NumFromRk((long) reader.ReadInt32(), out isFloat);
                        this._excelReader.SetCell(sheetIndex, row, column, (double) num4, CellType.Numeric, formatIndex, null);
                        return;

                    case BiffRecordNumber.FORMULA:
                    case BiffRecordNumber.FORMULA2:
                    {
                        bool flag6 = false;
                        buffer = reader.ReadBytes(8);
                        short num18 = reader.ReadInt16();
                        reader.ReadInt32();
                        int count = reader.ReadInt16();
                        byte[] buffer2 = reader.ReadBytes(count);
                        byte[] buffer3 = null;
                        bool flag7 = (num18 & 8) == 8;
                        int num20 = 0x16 + count;
                        if ((biff.DataLength - num20) > 0)
                        {
                            buffer3 = reader.ReadBytes(biff.DataLength - num20);
                        }
                        str6 = null;
                        formula = new ExcelFormula();
                        if (this._isA1ReferenceStyle)
                        {
                            formula.FormulaTokenBits = buffer2;
                            formula.FormulaTokenExtraBits = buffer3;
                        }
                        else
                        {
                            formula.FormulaR1C1TokenBits = buffer2;
                            formula.FormulaR1C1TokenExtraBits = buffer3;
                        }
                        if (buffer2.Length > 0)
                        {
                            int firstRow = -1;
                            int firstCol = -1;
                            if (buffer2[0] == 1)
                            {
                                if (this._isA1ReferenceStyle)
                                {
                                    formula.FormulaTokenBits = null;
                                    formula.FormulaTokenExtraBits = null;
                                }
                                else
                                {
                                    formula.FormulaR1C1TokenBits = null;
                                    formula.FormulaR1C1TokenExtraBits = null;
                                }
                                SimpleBinaryReader reader3 = new SimpleBinaryReader(buffer2);
                                List<byte[]> list = null;
                                reader3.Seek(1, (SeekOrigin) SeekOrigin.Begin);
                                firstRow = reader3.ReadUInt16();
                                firstCol = reader3.ReadUInt16();
                                byte[] buffer4 = null;
                                byte[] extra = null;
                                List<byte[]> list2 = new List<byte[]>();
                                if ((this._linkTable.sharedFormulaList != null) && flag7)
                                {
                                    this._linkTable.sharedFormulaList.GetSharedFormula(sheetIndex, row, column, firstRow, firstCol, ref buffer4, ref extra);
                                    list2.Add(buffer4);
                                    list2.Add(extra);
                                }
                                else
                                {
                                    formula.IsArrayFormula = true;
                                }
                                list = list2;
                                firstRow = firstCol = -2;
                                this._formulaPendingSheet = sheetIndex;
                                this._formulaPendingRow = (ushort) row;
                                this._formulaPendingCol = (ushort) column;
                                this._isFormulaPending = true;
                                if (((list != null) && (list.Count != 0)) && (list[0] != null))
                                {
                                    buffer2 = list[0];
                                    buffer3 = list[1];
                                }
                            }
                            using (BinaryReader reader4 = new BinaryReader((Stream) new MemoryStream(buffer2)))
                            {
                                BinaryReader @this = null;
                                try
                                {
                                    if (buffer3 != null)
                                    {
                                        @this = new BinaryReader((Stream) new MemoryStream(buffer3));
                                    }
                                    ExcelCalcError error2 = null;
                                    this.fp.formulaReader = reader4;
                                    this.fp.formulaExtraReader = @this;
                                    this.fp.sheet = sheetIndex;
                                    this.fp.row = row;
                                    this.fp.column = column;
                                    this.fp.isA1RefStyle = this._isA1ReferenceStyle;
                                    try
                                    {
                                        str6 = this.fp.ToString(this._linkTable, ref error2, new int?(row), new int?(column), false);
                                    }
                                    catch (Exception exception2)
                                    {
                                        string message = "";
                                        ExcelWarningCode general = ExcelWarningCode.General;
                                        if (exception2 is ExcelException)
                                        {
                                            ExcelException exception3 = exception2 as ExcelException;
                                            if (exception3.Code == ExcelExceptionCode.FormulaError)
                                            {
                                                general = ExcelWarningCode.FormulaError;
                                                message = ResourceHelper.GetResourceString("readCellFormulaError");
                                            }
                                            else
                                            {
                                                message = ResourceHelper.GetResourceString("readCellError");
                                            }
                                        }
                                        else
                                        {
                                            message = ResourceHelper.GetResourceString("readCellError");
                                        }
                                        this.LogError(message, general, sheetIndex, row, column, exception2);
                                    }
                                }
                                finally
                                {
                                    if (@this != null)
                                    {
                                        @this.Close();
                                    }
                                }
                            }
                        }
                        if ((buffer[6] != 0xff) || (buffer[7] != 0xff))
                        {
                            goto Label_08C7;
                        }
                        if (buffer[0] == 1)
                        {
                            if (buffer[2] == 1)
                            {
                                flag6 = true;
                            }
                            if (this._isA1ReferenceStyle)
                            {
                                formula.SetFormula(str6);
                            }
                            else
                            {
                                formula.SetFormulaR1C1(str6);
                            }
                            this._excelReader.SetCell(sheetIndex, row, column, (bool) flag6, CellType.FormulaString, formatIndex, formula);
                            return;
                        }
                        if (buffer[0] != 2)
                        {
                            goto Label_083F;
                        }
                        ExcelCalcError argumentOrFunctionNotAvailable = ExcelCalcError.ArgumentOrFunctionNotAvailable;
                        switch (buffer[2])
                        {
                            case 0:
                                argumentOrFunctionNotAvailable = ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty;
                                break;

                            case 7:
                                argumentOrFunctionNotAvailable = ExcelCalcError.DivideByZero;
                                break;

                            case 15:
                                argumentOrFunctionNotAvailable = ExcelCalcError.WrongTypeOfOperand;
                                break;

                            case 0x24:
                                argumentOrFunctionNotAvailable = ExcelCalcError.ValueRangeOverflow;
                                break;

                            case 0x2a:
                                argumentOrFunctionNotAvailable = ExcelCalcError.ArgumentOrFunctionNotAvailable;
                                break;

                            case 0x17:
                                argumentOrFunctionNotAvailable = ExcelCalcError.IllegalOrDeletedCellReference;
                                break;

                            case 0x1d:
                                argumentOrFunctionNotAvailable = ExcelCalcError.WrongFunctionOrRangeName;
                                break;
                        }
                        if (this._isA1ReferenceStyle)
                        {
                            formula.SetFormula(str6);
                        }
                        else
                        {
                            formula.SetFormulaR1C1(str6);
                        }
                        this._excelReader.SetCell(sheetIndex, row, column, argumentOrFunctionNotAvailable, CellType.Error, formatIndex, formula);
                        return;
                    }
                    case BiffRecordNumber.LABELSST:
                    {
                        int index = reader.ReadInt32();
                        this._excelReader.SetCell(sheetIndex, row, column, (this._sst[index] != null) ? this._sst[index] : string.Empty, CellType.String, formatIndex, null);
                        return;
                    }
                    case BiffRecordNumber.HLINK:
                    {
                        SimpleBinaryReader reader2 = new SimpleBinaryReader(biff.DataBuffer);
                        ushort num7 = reader2.ReadUInt16();
                        ushort num8 = reader2.ReadUInt16();
                        ushort num9 = reader2.ReadUInt16();
                        ushort num10 = reader2.ReadUInt16();
                        try
                        {
                            reader2.ReadBytes(0x10);
                            reader2.ReadInt32();
                            uint num11 = reader2.ReadUInt32();
                            bool flag3 = (num11 & 1) == 1;
                            bool flag4 = (num11 & 0x10) == 0x10;
                            bool flag5 = (num11 & 0x80) == 0x80;
                            string description = null;
                            if (flag4)
                            {
                                XLUnicodeString str3 = null;
                                int num12 = reader2.ReadInt32();
                                str3 = new XLUnicodeString {
                                    cch = (short) num12,
                                    fHighByte = 1,
                                    rgb = reader2.ReadBytes((str3.cch * 2) - 2)
                                };
                                description = str3.Text;
                                reader2.ReadBytes(2);
                            }
                            if (flag5)
                            {
                                XLUnicodeString str4 = null;
                                int num13 = reader2.ReadInt32();
                                str4 = new XLUnicodeString {
                                    cch = (short) num13,
                                    fHighByte = 1,
                                    rgb = reader2.ReadBytes((str4.cch * 2) - 2)
                                };
                                reader2.ReadBytes(2);
                            }
                            if (flag3)
                            {
                                reader2.ReadBytes(0x10);
                                int num14 = reader2.ReadInt32();
                                string uri = Encoding.Unicode.GetString(reader2.ReadBytes(num14 - 2), 0, num14 - 2);
                                if (uri.IndexOf('\0') > 0)
                                {
                                    uri = uri.Substring(0, uri.IndexOf('\0') - 1);
                                }
                                reader2.ReadBytes(2);
                                if (ExcelHyperLink.IsValidUri(uri))
                                {
                                    ExcelHyperLink hyperLink = new ExcelHyperLink(description, uri);
                                    for (int i = num7; i <= num8; i++)
                                    {
                                        for (int j = num9; j <= num10; j++)
                                        {
                                            this._excelReader.SetCellHyperLink(sheetIndex, i, j, hyperLink);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            this.LogError(ResourceHelper.GetResourceString("readHyperLinkError"), ExcelWarningCode.General, sheetIndex, num7, num9, exception);
                        }
                        return;
                    }
                    default:
                        return;
                }
                switch (num23)
                {
                    case 0x17:
                        illegalOrDeletedCellReference = ExcelCalcError.IllegalOrDeletedCellReference;
                        break;

                    case 0x1d:
                        illegalOrDeletedCellReference = ExcelCalcError.WrongFunctionOrRangeName;
                        break;

                    case 0x2a:
                        illegalOrDeletedCellReference = ExcelCalcError.ArgumentOrFunctionNotAvailable;
                        break;

                    case 0x2b:
                        illegalOrDeletedCellReference = ExcelCalcError.FAILEDTOGETDATA;
                        break;

                    case 0x24:
                        illegalOrDeletedCellReference = ExcelCalcError.ValueRangeOverflow;
                        break;
                }
                goto Label_0166;
            Label_0120:
                illegalOrDeletedCellReference = ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty;
                goto Label_0166;
            Label_0129:
                illegalOrDeletedCellReference = ExcelCalcError.DivideByZero;
                goto Label_0166;
            Label_0132:
                illegalOrDeletedCellReference = ExcelCalcError.WrongTypeOfOperand;
            Label_0166:
                this._excelReader.SetCell(sheetIndex, row, column, illegalOrDeletedCellReference, CellType.Error, formatIndex, null);
                return;
            Label_0180:
                this._excelReader.SetCell(sheetIndex, row, column, (num5 == 1) ? ((bool) true) : ((bool) false), CellType.Boolean, formatIndex, null);
                return;
            Label_083F:
                if (buffer[0] == 0)
                {
                    this._isStringPending = true;
                    this._stringPendingSheet = sheetIndex;
                    this._stringPendingRow = row;
                    this._stringPendingCol = column;
                    if (this._isA1ReferenceStyle)
                    {
                        formula.SetFormula(str6);
                    }
                    else
                    {
                        formula.SetFormulaR1C1(str6);
                    }
                    this._excelReader.SetCell(sheetIndex, row, column, null, CellType.FormulaString, formatIndex, formula);
                }
                else
                {
                    if (this._isA1ReferenceStyle)
                    {
                        formula.SetFormula(str6);
                    }
                    else
                    {
                        formula.SetFormulaR1C1(str6);
                    }
                    this._excelReader.SetCell(sheetIndex, row, column, null, CellType.FormulaString, formatIndex, formula);
                }
                return;
            Label_08C7:
                num4 = BitConverter.ToDouble(buffer, 0);
                if (this._isA1ReferenceStyle)
                {
                    formula.SetFormula(str6);
                }
                else
                {
                    formula.SetFormulaR1C1(str6);
                }
                this._excelReader.SetCell(sheetIndex, row, column, (double) num4, CellType.FormulaString, formatIndex, formula);
            }
            catch (Exception exception4)
            {
                string resourceString = "";
                ExcelWarningCode warningCode = ExcelWarningCode.General;
                if (exception4 is ExcelException)
                {
                    ExcelException exception5 = exception4 as ExcelException;
                    if (exception5.Code == ExcelExceptionCode.FormulaError)
                    {
                        warningCode = ExcelWarningCode.FormulaError;
                        resourceString = ResourceHelper.GetResourceString("readCellFormulaError");
                    }
                    else
                    {
                        resourceString = ResourceHelper.GetResourceString("readCellError");
                    }
                }
                else
                {
                    resourceString = ResourceHelper.GetResourceString("readCellError");
                }
                this.LogError(resourceString, warningCode, sheetIndex, row, column, exception4);
            }
        }

        private ExcelConditionalFormatValueObject ReadCFVO(SimpleBinaryReader reader)
        {
            ExcelConditionalFormatValueObject obj2 = new ExcelConditionalFormatValueObject();
            byte num = reader.ReadByte();
            obj2.Type = (ExcelConditionalFormatValueObjectType) num;
            short count = reader.ReadInt16();
            if (count != 0)
            {
                string formula = this.GetFormula(reader.ReadBytes(count), null);
                obj2.Value = formula;
                obj2.IsFormula = true;
            }
            if (((obj2.Type != ExcelConditionalFormatValueObjectType.Min) && (obj2.Type != ExcelConditionalFormatValueObjectType.Max)) && (count == 0))
            {
                obj2.Value = ((double) reader.ReadDouble()).ToString();
            }
            return obj2;
        }

        private void ReadCodePage(BiffRecord biff, short sheetIndex)
        {
            BitConverter.ToInt16(biff.DataBuffer, 0);
        }

        private ExcelColor ReadColor(SimpleBinaryReader reader)
        {
            ExcelColor emptyColor = ExcelColor.EmptyColor;
            switch (reader.ReadInt32())
            {
                case 1:
                    emptyColor = new ExcelColor(ExcelColorType.Indexed, reader.ReadUInt32(), 0.0);
                    reader.ReadBytes(8);
                    return emptyColor;

                case 2:
                {
                    byte red = reader.ReadByte();
                    byte green = reader.ReadByte();
                    byte blue = reader.ReadByte();
                    byte alpha = reader.ReadByte();
                    return new ExcelColor(ExcelColorType.RGB, GcColor.FromArgb(alpha, red, green, blue).ToArgb(), reader.ReadDouble());
                }
                case 3:
                {
                    uint color = reader.ReadUInt32();
                    return new ExcelColor(ExcelColorType.Theme, color, reader.ReadDouble());
                }
            }
            return emptyColor;
        }

        private void ReadColumnInfo(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            short columnFirst = reader.ReadInt16();
            short columnLast = reader.ReadInt16();
            if (columnLast == 0x100)
            {
                columnLast = 0x4000;
            }
            ushort num3 = reader.ReadUInt16();
            short formatIndex = reader.ReadInt16();
            bool hidden = (reader.ReadByte() & 1) == 1;
            byte num5 = reader.ReadByte();
            byte outlineLevel = (byte)(num5 & 7);
            bool collapsed = (num5 & 0x10) == 0x10;
            this._excelReader.SetColumnInfo(sheetIndex, columnFirst, columnLast, formatIndex, ((double) num3) / 256.0, hidden, outlineLevel, collapsed);
        }

        private void ReadCondFmt(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            short num2 = (short)(reader.ReadInt16() >> 1);
            ExcelConditionalFormat format = new ExcelConditionalFormat {
                Identifier = num2
            };
            List<IRange> rangeAndRangeList = this.GetRangeAndRangeList(reader);
            format.Ranges = rangeAndRangeList;
            this._conditonalFormat.Add(format);
        }

        private void ReadCondFmt12(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            reader.ReadInt16();
            List<IRange> list = new List<IRange>();
            this.GetRange(reader);
            reader.ReadUInt16();
            short num2 = (short)(reader.ReadInt16() >> 1);
            this.GetRange(reader);
            ushort num3 = reader.ReadUInt16();
            new List<IRange>();
            for (int i = 0; i < num3; i++)
            {
                list.Add(this.GetRange(reader));
            }
            ExcelConditionalFormat format = new ExcelConditionalFormat {
                IsOffice2007ConditionalFormat = true,
                Identifier = num2,
                Ranges = list
            };
            this._conditionaFormat12.Add(format);
            this._priviousConditonalFormat = format;
        }

        private void ReadConditionalFormating(BiffRecord biff, short sheetIndex)
        {
            IRange range = null;
            try
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                byte num = reader.ReadByte();
                byte num2 = reader.ReadByte();
                short count = reader.ReadInt16();
                short num4 = reader.ReadInt16();
                DifferentialFormatting formatting = this.ReadDifferentialFormattingRecord(reader);
                this._dxfs.Add(formatting);
                range = this._conditonalFormat[this._conditonalFormat.Count - 1].Ranges[0];
                List<IRange> ranges = this._conditonalFormat[this._conditonalFormat.Count - 1].Ranges;
                string formula = null;
                string str2 = null;
                if (ranges.Count == 1)
                {
                    formula = this.GetFormula(reader.ReadBytes(count), range);
                    str2 = this.GetFormula(reader.ReadBytes(num4), range);
                }
                else
                {
                    int row = 0x7fffffff;
                    int column = 0x7fffffff;
                    foreach (IRange range2 in ranges)
                    {
                        if (range2.Row < row)
                        {
                            row = range2.Row;
                        }
                        if (range2.Column < column)
                        {
                            column = range2.Column;
                        }
                    }
                    ExcelCellRange range3 = new ExcelCellRange {
                        Row = row,
                        Column = column,
                        RowSpan = 1,
                        ColumnSpan = 1
                    };
                    formula = this.GetFormula(reader.ReadBytes(count), range3);
                    str2 = this.GetFormula(reader.ReadBytes(num4), range3);
                }
                if (num == 1)
                {
                    ExcelConditionalFormattingOperator noComparison = ExcelConditionalFormattingOperator.NoComparison;
                    noComparison = (ExcelConditionalFormattingOperator) num2;
                    ExcelHighlightingRule rule = new ExcelHighlightingRule {
                        DifferentialFormattingId = this._dxfs.Count - 1,
                        ComparisonOperator = noComparison
                    };
                    if (!string.IsNullOrWhiteSpace(formula))
                    {
                        rule.Formulas.Add(formula);
                    }
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        rule.Formulas.Add(str2);
                    }
                    this._conditonalFormat[this._conditonalFormat.Count - 1].ConditionalFormattingRules.Add(rule);
                }
                else
                {
                    ExcelGeneralRule rule2 = new ExcelGeneralRule(ExcelConditionalFormatType.Expression) {
                        DifferentialFormattingId = this._dxfs.Count - 1
                    };
                    if (!string.IsNullOrWhiteSpace(formula))
                    {
                        rule2.Formulas.Add(formula);
                    }
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        rule2.Formulas.Add(str2);
                    }
                    this._conditonalFormat[this._conditonalFormat.Count - 1].ConditionalFormattingRules.Add(rule2);
                }
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("readConditionalFormatError"), ExcelWarningCode.General, sheetIndex, (range != null) ? range.Row : -1, (range != null) ? range.Column : -1, exception);
            }
        }

        private void ReadConditionalFormating12(BiffRecord biff, short sheetIndex)
        {
            ExcelConditionalFormat format = this._priviousConditonalFormat;
            if (format != null)
            {
                try
                {
                    SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                    reader.ReadInt16();
                    reader.ReadInt16();
                    this.GetRange(reader);
                    byte num = reader.ReadByte();
                    byte num2 = reader.ReadByte();
                    short count = reader.ReadInt16();
                    short num4 = reader.ReadInt16();
                    DifferentialFormatting formatting = this.ReadDifferentialFormattingExtensionRecord(reader);
                    this._dxfs.Add(formatting);
                    string formula = this.GetFormula(reader.ReadBytes(count), null);
                    string str2 = this.GetFormula(reader.ReadBytes(num4), null);
                    short num5 = reader.ReadInt16();
                    if (num5 != 0)
                    {
                        this.GetFormula(reader.ReadBytes(num5), null);
                    }
                    bool flag = (reader.ReadByte() & 2) == 2;
                    short num7 = reader.ReadInt16();
                    short num8 = reader.ReadInt16();
                    byte num9 = reader.ReadByte();
                    reader.ReadBytes(num9);
                    if (num == 1)
                    {
                        ExcelConditionalFormattingOperator noComparison = ExcelConditionalFormattingOperator.NoComparison;
                        noComparison = (ExcelConditionalFormattingOperator) num2;
                        ExcelHighlightingRule rule = new ExcelHighlightingRule {
                            DifferentialFormattingId = this._dxfs.Count - 1,
                            ComparisonOperator = noComparison,
                            Priority = num7,
                            StopIfTrue = flag
                        };
                        if (!string.IsNullOrWhiteSpace(formula))
                        {
                            rule.Formulas.Add(formula);
                        }
                        if (!string.IsNullOrWhiteSpace(str2))
                        {
                            rule.Formulas.Add(str2);
                        }
                        format.ConditionalFormattingRules.Add(rule);
                    }
                    else if (num == 2)
                    {
                        ExcelGeneralRule rule2 = new ExcelGeneralRule((ExcelConditionalFormatType) num8) {
                            Priority = num7,
                            StopIfTrue = flag,
                            DifferentialFormattingId = this._dxfs.Count - 1
                        };
                        if (!string.IsNullOrWhiteSpace(formula))
                        {
                            rule2.Formulas.Add(formula);
                        }
                        if (!string.IsNullOrWhiteSpace(str2))
                        {
                            rule2.Formulas.Add(str2);
                        }
                        if ((rule2.Type == ExcelConditionalFormatType.ContainsText) && (formula != null))
                        {
                            string str3 = formula.ToUpperInvariant();
                            if (str3.StartsWith("ISERROR"))
                            {
                                rule2.Type = ExcelConditionalFormatType.NotContainsText;
                                rule2.Operator = (ExcelConditionalFormattingOperator)12;
                                string searchText = this.GetSearchText(formula);
                                if (!string.IsNullOrWhiteSpace(searchText))
                                {
                                    rule2.Text = searchText;
                                }
                            }
                            else if (str3.StartsWith("NOT(ISERROR"))
                            {
                                rule2.Type = ExcelConditionalFormatType.ContainsText;
                                rule2.Operator = (ExcelConditionalFormattingOperator)11;
                                string str5 = this.GetSearchText(formula);
                                if (!string.IsNullOrWhiteSpace(str5))
                                {
                                    rule2.Text = str5;
                                }
                            }
                            else if (str3.StartsWith("LEFT"))
                            {
                                rule2.Type = ExcelConditionalFormatType.BeginsWith;
                                rule2.Operator = (ExcelConditionalFormattingOperator)9;
                                string str6 = this.GetSearchText(formula);
                                if (!string.IsNullOrWhiteSpace(str6))
                                {
                                    rule2.Text = str6;
                                }
                            }
                            else if (str3.StartsWith("RIGHT"))
                            {
                                rule2.Type = ExcelConditionalFormatType.EndsWith;
                                rule2.Operator = (ExcelConditionalFormattingOperator)10;
                                string str7 = this.GetSearchText(formula);
                                if (!string.IsNullOrWhiteSpace(str7))
                                {
                                    rule2.Text = str7;
                                }
                            }
                        }
                        format.ConditionalFormattingRules.Add(rule2);
                    }
                    else if (num == 3)
                    {
                        ExcelColorScaleRule rule3 = new ExcelColorScaleRule {
                            Priority = num7,
                            StopIfTrue = flag
                        };
                        reader.ReadInt16();
                        reader.ReadByte();
                        byte num10 = reader.ReadByte();
                        byte num11 = reader.ReadByte();
                        reader.ReadByte();
                        List<ExcelConditionalFormatValueObject> list = new List<ExcelConditionalFormatValueObject>();
                        for (int i = 0; i < num10; i++)
                        {
                            ExcelConditionalFormatValueObject obj2 = this.ReadCFVO(reader);
                            double num13 = reader.ReadDouble();
                            if (obj2.Value == null)
                            {
                                obj2.Value = ((double) num13).ToString();
                            }
                            list.Add(obj2);
                        }
                        List<ExcelColor> list2 = new List<ExcelColor>();
                        for (int j = 0; j < num11; j++)
                        {
                            reader.ReadDouble();
                            list2.Add(this.ReadColor(reader));
                        }
                        if (list.Count == 2)
                        {
                            rule3.Minimum = list[0];
                            rule3.MinimumColor = list2[0];
                            rule3.Maximum = list[1];
                            rule3.MaximumColor = list2[1];
                        }
                        else if (list.Count == 3)
                        {
                            rule3.HasMiddleNode = true;
                            rule3.Minimum = list[0];
                            rule3.MinimumColor = list2[0];
                            rule3.Middle = list[1];
                            rule3.MiddleColor = list2[1];
                            rule3.Maximum = list[2];
                            rule3.MaximumColor = list2[2];
                        }
                        format.ConditionalFormattingRules.Add(rule3);
                    }
                    else
                    {
                        switch (num)
                        {
                            case 4:
                            {
                                reader.ReadInt16();
                                reader.ReadByte();
                                byte num15 = reader.ReadByte();
                                bool flag2 = (num15 & 1) == 1;
                                bool flag3 = (num15 & 2) != 2;
                                ExcelDataBarRule rule4 = new ExcelDataBarRule {
                                    RightToLeft = flag2,
                                    ShowValue = flag3,
                                    Priority = num7,
                                    StopIfTrue = flag,
                                    MinimumDataBarLength = reader.ReadByte(),
                                    MaximumDataBarLength = reader.ReadByte(),
                                    Color = this.ReadColor(reader),
                                    Minimum = this.ReadCFVO(reader),
                                    Maximum = this.ReadCFVO(reader)
                                };
                                if (rule4.Maximum.Type == ExcelConditionalFormatValueObjectType.Max)
                                {
                                    rule4.Maximum.Value = null;
                                }
                                if (rule4.Minimum.Type == ExcelConditionalFormatValueObjectType.Min)
                                {
                                    rule4.Minimum.Value = null;
                                }
                                format.ConditionalFormattingRules.Add(rule4);
                                return;
                            }
                            case 5:
                            {
                                ExcelGeneralRule rule5 = new ExcelGeneralRule((ExcelConditionalFormatType) num8) {
                                    Priority = num7,
                                    StopIfTrue = flag
                                };
                                reader.ReadInt16();
                                reader.ReadByte();
                                byte num16 = reader.ReadByte();
                                if ((num16 & 1) == 1)
                                {
                                    rule5.Bottom = true;
                                }
                                if ((num16 & 2) == 2)
                                {
                                    rule5.Percent = true;
                                }
                                rule5.Rank = new int?(reader.ReadUInt16());
                                rule5.DifferentialFormattingId = this._dxfs.Count - 1;
                                format.ConditionalFormattingRules.Add(rule5);
                                return;
                            }
                        }
                        if (num == 6)
                        {
                            ExcelIconSetsRule rule6 = new ExcelIconSetsRule {
                                Priority = num7
                            };
                            reader.ReadInt16();
                            reader.ReadByte();
                            byte num17 = reader.ReadByte();
                            rule6.IconSet = (ExcelIconSetType) reader.ReadByte();
                            byte num18 = reader.ReadByte();
                            rule6.IconOnly = (num18 & 1) == 1;
                            rule6.ReversedOrder = (num18 & 4) == 4;
                            for (int k = 0; k < num17; k++)
                            {
                                ExcelConditionalFormatValueObject obj3 = this.ReadCFVO(reader);
                                rule6.NotPassTheThresholdsWhenEquals.Add(reader.ReadByte() == 0);
                                reader.ReadBytes(4);
                                rule6.Thresholds.Add(obj3);
                            }
                            format.ConditionalFormattingRules.Add(rule6);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("readConditionalFormatError"), ExcelWarningCode.General, sheetIndex, format.Ranges[0].Row, format.Ranges[0].Column, exception);
                }
            }
        }

        private void ReadConditionalFormatingExtension(BiffRecord biff, short sheetIndex)
        {
            if (this._conditonalFormat != null)
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                reader.ReadInt16();
                reader.ReadInt16();
                this.GetRange(reader);
                int num = reader.ReadInt32();
                short num2 = reader.ReadInt16();
                ExcelConditionalFormat format = null;
                foreach (ExcelConditionalFormat format2 in this._conditonalFormat)
                {
                    if (format2.Identifier == num2)
                    {
                        format = format2;
                        break;
                    }
                }
                if (format != null)
                {
                    this._priviousConditonalFormat = format;
                    try
                    {
                        if (num == 0)
                        {
                            short num3 = reader.ReadInt16();
                            IExcelConditionalFormatRule rule = format.ConditionalFormattingRules[num3];
                            int num4 = reader.ReadByte();
                            int num5 = reader.ReadByte();
                            rule.Priority = reader.ReadUInt16();
                            byte num6 = reader.ReadByte();
                            rule.StopIfTrue = (num6 & 2) == 2;
                            if (reader.ReadByte() == 1)
                            {
                                int num7 = this._dxfs.Count;
                                this._dxfs.Add(this.ReadDifferentialFormattingExtensionRecord(reader));
                                if (rule is IExcelGeneralRule)
                                {
                                    (rule as IExcelGeneralRule).DifferentialFormattingId = num7;
                                    (rule as IExcelGeneralRule).Operator = new ExcelConditionalFormattingOperator?((ExcelConditionalFormattingOperator) num4);
                                    if (num5 > 0)
                                    {
                                        (rule as IExcelGeneralRule).Type = (ExcelConditionalFormatType) num5;
                                    }
                                }
                                else if (rule is IExcelHighlightingRule)
                                {
                                    (rule as IExcelHighlightingRule).DifferentialFormattingId = num7;
                                    (rule as IExcelHighlightingRule).ComparisonOperator = (ExcelConditionalFormattingOperator) num4;
                                    if (num5 > 0)
                                    {
                                        (rule as IExcelHighlightingRule).Type = (ExcelConditionalFormatType) num5;
                                    }
                                }
                            }
                            else if (rule is IExcelGeneralRule)
                            {
                                (rule as IExcelGeneralRule).Operator = new ExcelConditionalFormattingOperator?((ExcelConditionalFormattingOperator) num4);
                                if (num5 > 0)
                                {
                                    (rule as IExcelGeneralRule).Type = (ExcelConditionalFormatType) num5;
                                }
                            }
                            else if (rule is IExcelHighlightingRule)
                            {
                                (rule as IExcelHighlightingRule).ComparisonOperator = (ExcelConditionalFormattingOperator) num4;
                                if (num5 > 0)
                                {
                                    (rule as IExcelHighlightingRule).Type = (ExcelConditionalFormatType) num5;
                                }
                            }
                            SimpleBinaryReader reader2 = new SimpleBinaryReader(reader.ReadBytes(reader.ReadByte()));
                            if ((num5 == 5) && (rule is IExcelGeneralRule))
                            {
                                IExcelGeneralRule rule2 = rule as IExcelGeneralRule;
                                byte num8 = reader2.ReadByte();
                                if ((num8 & 1) == 0)
                                {
                                    rule2.Bottom = true;
                                }
                                if ((num8 & 2) == 2)
                                {
                                    rule2.Percent = true;
                                }
                                rule2.Rank = new int?(reader2.ReadUInt16());
                            }
                            else if (num5 == 8)
                            {
                                ExcelConditionalFormattingOperator noComparison = ExcelConditionalFormattingOperator.NoComparison;
                                ExcelConditionalFormatType containsText = ExcelConditionalFormatType.ContainsText;
                                switch (reader2.ReadUInt16())
                                {
                                    case 0:
                                        noComparison = ExcelConditionalFormattingOperator.ContainsText;
                                        containsText = ExcelConditionalFormatType.ContainsText;
                                        break;

                                    case 1:
                                        noComparison = ExcelConditionalFormattingOperator.NotContains;
                                        containsText = ExcelConditionalFormatType.NotContainsText;
                                        break;

                                    case 2:
                                        noComparison = ExcelConditionalFormattingOperator.BeginsWith;
                                        containsText = ExcelConditionalFormatType.BeginsWith;
                                        break;

                                    case 3:
                                        noComparison = ExcelConditionalFormattingOperator.EndsWith;
                                        containsText = ExcelConditionalFormatType.EndsWith;
                                        break;
                                }
                                if (rule is IExcelGeneralRule)
                                {
                                    IExcelGeneralRule rule3 = rule as IExcelGeneralRule;
                                    rule3.Operator = new ExcelConditionalFormattingOperator?(noComparison);
                                    rule3.Type = containsText;
                                    if (rule3.Formulas.Count == 1)
                                    {
                                        string searchText = this.GetSearchText(rule3.Formulas[0]);
                                        if (!string.IsNullOrWhiteSpace(searchText))
                                        {
                                            rule3.Text = searchText;
                                        }
                                    }
                                }
                                else if (rule is IExcelHighlightingRule)
                                {
                                    (rule as IExcelHighlightingRule).ComparisonOperator = noComparison;
                                }
                            }
                            else if (((num5 == 0x19) || (num5 == 0x1a)) && (rule is ExcelGeneralRule))
                            {
                                switch (reader2.ReadUInt16())
                                {
                                    case 1:
                                        (rule as ExcelGeneralRule).StdDev = 1;
                                        return;

                                    case 2:
                                        (rule as ExcelGeneralRule).StdDev = 2;
                                        return;

                                    case 3:
                                        (rule as ExcelGeneralRule).StdDev = 3;
                                        return;
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readConditionalFormatError"), ExcelWarningCode.General, sheetIndex, format.Ranges[0].Row, format.Ranges[0].Column, exception);
                    }
                }
            }
        }

        private void ReadContinue(BiffRecord biff, short sheetIndex)
        {
            switch (this._continueRecordType)
            {
                case ContinueRecordType.SST:
                    this._SSTBuffers.Add(biff.DataBuffer);
                    return;

                case ContinueRecordType.DBCELL:
                case ContinueRecordType.MSODRAWINGGROUP:
                case ContinueRecordType.MSODRAWING:
                    return;

                case ContinueRecordType.TXO1:
                {
                    string str;
                    new SimpleBinaryReader(biff.DataBuffer).ReadUncompressedString(this._txoTextLength, out str);
                    short commentObjId = -1;
                    if (this._commentObjCount <= 0)
                    {
                        break;
                    }
                    commentObjId = this.GetCommentObjId();
                    if (commentObjId != -2)
                    {
                        this._commentObjCount--;
                        string str2 = string.Format("{0}-{1}", (object[]) new object[] { ((short) sheetIndex).ToString(), ((short) commentObjId).ToString() });
                        this._txoStringTable[str2] = str;
                        break;
                    }
                    return;
                }
                case ContinueRecordType.TXO2:
                    this._continueRecordType = ContinueRecordType.Empty;
                    return;

                default:
                    return;
            }
            this._continueRecordType = ContinueRecordType.TXO2;
        }

        private void ReadContinueFrt12(BiffRecord biff, short sheetIndex)
        {
            IExcelFilterColumn column = this._readContinueFRT12_filterColumn;
            if (column != null)
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                reader.Seek(12, (SeekOrigin) SeekOrigin.Current);
                if (this._readContinueFRT12_cCriteria.HasValue)
                {
                    try
                    {
                        int num;
                        ExcelCustomFilter filter;
                        this.ReadDOper(reader, out filter, out num);
                        if (num > 0)
                        {
                            string str;
                            if (column.Filters == null)
                            {
                                column.Filters = new ExcelFilters();
                            }
                            if (column.Filters.Filter == null)
                            {
                                column.Filters.Filter = new List<string>();
                            }
                            reader.ReadUncompressedString(num, out str);
                            column.Filters.Filter.Add(str);
                        }
                        else if (filter != null)
                        {
                            if (column.CustomFilters != null)
                            {
                                if (column.CustomFilters.Filter1 == null)
                                {
                                    column.CustomFilters.Filter1 = filter;
                                }
                                else
                                {
                                    column.CustomFilters.Filter2 = filter;
                                }
                            }
                            else if ((column.DynamicFilter != null) && (column.DynamicFilter.Value == null))
                            {
                                column.DynamicFilter.Value = filter.Value;
                            }
                            else if ((column.DynamicFilter != null) && (column.DynamicFilter.MaxValue == null))
                            {
                                column.DynamicFilter.MaxValue = filter.Value;
                            }
                        }
                        this._readContinueFRT12_cCriteria = new uint?(this._readContinueFRT12_cCriteria.Value - 1);
                        if (this._readContinueFRT12_cCriteria.Value == 0)
                        {
                            this._readContinueFRT12_cCriteria = null;
                        }
                        if (this._readContinueFRT12_cDateGroupings.HasValue)
                        {
                            return;
                        }
                        if (!this._readContinueFRT12_cCriteria.HasValue)
                        {
                            this._readContinueFRT12_filterColumn = null;
                        }
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readAutoFilterError"), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
                    }
                }
                if (this._readContinueFRT12_cDateGroupings.HasValue)
                {
                    try
                    {
                        if (column.Filters == null)
                        {
                            column.Filters = new ExcelFilters();
                        }
                        if (column.Filters.DateGroupItem == null)
                        {
                            column.Filters.DateGroupItem = new List<IExcelDateGroupItem>();
                        }
                        ExcelDateGroupItem item = new ExcelDateGroupItem {
                            Year = reader.ReadUInt16(),
                            Month = reader.ReadUInt16(),
                            Day = (ushort) reader.ReadUInt32(),
                            Hour = reader.ReadUInt16(),
                            Minute = reader.ReadUInt16(),
                            Second = reader.ReadUInt16()
                        };
                        reader.Seek(6, (SeekOrigin) SeekOrigin.Current);
                        item.DateTimeGrouping = (ExcelDateTimeGrouping) reader.ReadInt32();
                        column.Filters.DateGroupItem.Add(item);
                        this._readContinueFRT12_cDateGroupings = new uint?(this._readContinueFRT12_cDateGroupings.Value - 1);
                        if (this._readContinueFRT12_cDateGroupings.Value == 0)
                        {
                            this._readContinueFRT12_cDateGroupings = null;
                        }
                        if (!this._readContinueFRT12_cCriteria.HasValue && !this._readContinueFRT12_cDateGroupings.HasValue)
                        {
                            this._readContinueFRT12_filterColumn = null;
                        }
                    }
                    catch (Exception exception2)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readAutoFilterError"), ExcelWarningCode.General, sheetIndex, -1, -1, exception2);
                    }
                }
            }
        }

        private void ReadCountry(BiffRecord biff, short sheetIndex)
        {
        }

        private void ReadDate1904(BiffRecord biff, short sheetIndex)
        {
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._workbookProperty.IsDate1904 = num == 1;
        }

        private void ReadDefaultColumnWidth(BiffRecord biff, short sheetIndex)
        {
            BiffRecord record;
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            if (this._standColWidths.TryGetValue(sheetIndex, out record) && (record != null))
            {
                this._excelReader.SetDefaultColumnWidth(sheetIndex, (double) num, new double?(((double) BitConverter.ToInt16(record.DataBuffer, 0)) / 256.0));
            }
            else
            {
                this._excelReader.SetDefaultColumnWidth(sheetIndex, (double) num, null);
            }
        }

        private void ReadDefaultRowHeight(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            double defaultHeight = ((double) reader.ReadInt16()) / 20.0;
            this._excelReader.SetDefaultRowHeight(sheetIndex, defaultHeight);
        }

        private void ReadDelta(BiffRecord biff, short sheetIndex)
        {
            double num = BitConverter.ToDouble(biff.DataBuffer, 0);
            this._calcProperty.MaximunChange = num;
        }

        private DifferentialFormatting ReadDifferentialFormattingExtensionRecord(SimpleBinaryReader reader)
        {
            DifferentialFormatting dxfExt = new DifferentialFormatting {
                IsDFXExten = true
            };
            int count = reader.ReadInt32();
            if (count <= 0)
            {
                reader.ReadInt16();
            }
            if (count != 0)
            {
                SimpleBinaryReader reader2 = new SimpleBinaryReader(reader.ReadBytes(count));
                dxfExt = this.ReadDifferentialFormattingRecord(reader2);
                dxfExt.IsDFXExten = true;
                if (reader2.Remaining != 0)
                {
                    this.ReadXFExtNoFRT(reader2, dxfExt);
                }
            }
            return dxfExt;
        }

        private DifferentialFormatting ReadDifferentialFormattingRecord(SimpleBinaryReader reader)
        {
            DifferentialFormatting formatting = new DifferentialFormatting {
                FormatId = -1
            };
            int num = reader.ReadInt32();
            short num2 = reader.ReadInt16();
            bool flag = (num & 1) == 0;
            bool flag2 = (num & 2) == 0;
            bool flag3 = (num & 4) == 0;
            bool flag4 = (num & 8) == 0;
            bool flag5 = (num & 0x10) == 0;
            bool flag6 = (num & 0x20) == 0;
            bool flag7 = (num & 0x40) == 0;
            bool flag8 = (num & 0x100) == 0;
            bool flag9 = (num & 0x200) == 0;
            bool flag10 = (num & 0x400) == 0;
            bool flag11 = (num & 0x800) == 0;
            bool flag12 = (num & 0x1000) == 0;
            bool flag13 = (num & 0x2000) == 0;
            bool flag14 = (num & 0x10000) == 0;
            bool flag15 = (num & 0x20000) == 0;
            bool flag16 = (num & 0x40000) == 0;
            bool flag17 = (num & 0x2000000) == 0x2000000;
            bool flag18 = (num & 0x4000000) == 0x4000000;
            bool flag19 = (num & 0x8000000) == 0x8000000;
            bool flag20 = (num & 0x10000000) == 0x10000000;
            bool flag21 = (num & 0x20000000) == 0x20000000;
            bool flag22 = (num & 0x40000000) == 0x40000000;
            bool flag23 = (num & 0x80000000L) == 0L;
            bool flag24 = (num2 & 1) == 1;
            if (flag17)
            {
                if (flag24)
                {
                    XLUnicodeString str = null;
                    reader.ReadInt16();
                    str = new XLUnicodeString {
                        cch = reader.ReadInt16(),
                        fHighByte = (byte)(reader.ReadByte() & 1),
                        rgb = reader.ReadBytes(str.cch)
                    };
                    formatting.NumberFormat = new ExcelNumberFormat(-1, str.Text);
                }
                else
                {
                    reader.ReadByte();
                    short id = reader.ReadByte();
                    if (this._numberFormats.ContainsKey(id))
                    {
                        formatting.NumberFormat = new ExcelNumberFormat(id, this._numberFormats[id]);
                    }
                    else
                    {
                        formatting.FormatId = id;
                    }
                }
            }
            else
            {
                formatting.FormatId = -1;
            }
            if (flag18)
            {
                ExcelFont font = this.ReadDXFFont(reader);
                formatting.Font = font;
            }
            if (flag19)
            {
                byte num4 = reader.ReadByte();
                byte num5 = (byte)(num4 & 7);
                byte num6 = (byte)((num4 & 0x70) >> 2);
                formatting.Alignment = new AlignmentBlock();
                if (flag)
                {
                    formatting.Alignment.HorizontalAlignment = (ExcelHorizontalAlignment) num5;
                }
                if (flag2)
                {
                    formatting.Alignment.VerticalAlignment = (ExcelVerticalAlignment) num6;
                }
                if (flag3)
                {
                    formatting.Alignment.IsTextWrapped = (num4 & 8) == 8;
                }
                if (flag5)
                {
                    formatting.Alignment.IsJustifyLastLine = (num4 & 0x80) == 0x80;
                }
                if (flag4)
                {
                    formatting.Alignment.TextRotation = reader.ReadByte();
                }
                reader.ReadByte();
                if (flag6)
                {
                    formatting.Alignment.IndentationLevel = (byte)(num4 & 15);
                }
                if (flag7)
                {
                    formatting.Alignment.IsShrinkToFit = (num4 & 0x10) == 0x10;
                }
                if (flag23)
                {
                    formatting.Alignment.TextDirection = (TextDirection)(byte)((num4 & 0xc0) >> 6);
                }
                reader.ReadBytes(5);
            }
            if (flag20)
            {
                ExcelBorder border = new ExcelBorder();
                short num7 = reader.ReadInt16();
                int num8 = reader.ReadInt32();
                reader.ReadBytes(2);
                if (flag10)
                {
                    border.Left.Color = new ExcelColor(ExcelColorType.Indexed, (uint)(num8 & 0x7f), 0.0);
                    border.Left.LineStyle = (ExcelBorderStyle)(num7 & 15);
                }
                if (flag11)
                {
                    border.Right.LineStyle = (ExcelBorderStyle)((num7 & 240) >> 4);
                    border.Right.Color = new ExcelColor(ExcelColorType.Indexed, (uint)((num8 & 0x3f80) >> 7), 0.0);
                }
                if (flag12)
                {
                    border.Top.LineStyle = (ExcelBorderStyle)((num7 & 0xf00) >> 8);
                    border.Top.Color = new ExcelColor(ExcelColorType.Indexed, (uint)((num8 & 0x7f0000) >> 0x10), 0.0);
                }
                if (flag13)
                {
                    border.Bottom.LineStyle = (ExcelBorderStyle)((num7 & 0xf000) >> 12);
                    border.Bottom.Color = new ExcelColor(ExcelColorType.Indexed, (uint)((num8 & 0x3f800000) >> 0x17), 0.0);
                }
                formatting.Border = border;
            }
            if (flag21)
            {
                int num9 = reader.ReadInt16() >> 10;
                FillPatternType none = FillPatternType.None;
                if (flag14)
                {
                    none = (FillPatternType) num9;
                }
                short num10 = reader.ReadInt16();
                ExcelColor color = null;
                ExcelColor color2 = null;
                if (flag15)
                {
                    color = new ExcelColor(ExcelColorType.Indexed, (uint)(num10 & 0x7f), 0.0);
                }
                if (flag16)
                {
                    color2 = new ExcelColor(ExcelColorType.Indexed, (uint)((num10 & 0x3f80) >> 7), 0.0);
                }
                formatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(none, color, color2);
            }
            if (flag22)
            {
                short num11 = reader.ReadInt16();
                if (flag9)
                {
                    formatting.IsHidden = (num11 & 2) == 2;
                }
                if (flag8)
                {
                    formatting.IsLocked = (num11 & 1) == 1;
                }
            }
            return formatting;
        }

        private void ReadDimensions(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int rowFirst = reader.ReadInt32();
            int rowLast = reader.ReadInt32();
            short columnFirst = reader.ReadInt16();
            short columnLast = reader.ReadInt16();
            this._excelReader.SetDimensions(sheetIndex, rowFirst, rowLast, columnFirst, columnLast);
        }

        private void ReadDOper(SimpleBinaryReader reader, out ExcelCustomFilter filter, out int cch)
        {
            byte num7;
            Exception exception;
            ExcelCustomFilter filter5;
            byte num = reader.ReadByte();
            switch (num)
            {
                case 2:
                {
                    bool flag;
                    byte num2 = reader.ReadByte();
                    double num3 = this.NumFromRk((long) reader.ReadInt32(), out flag);
                    reader.Seek(4, (SeekOrigin) SeekOrigin.Current);
                    ExcelCustomFilter filter2 = new ExcelCustomFilter {
                        Operator = (ExcelFilterOperator) num2,
                        ValueType = num,
                        Value = (double) num3
                    };
                    filter = filter2;
                    cch = -1;
                    return;
                }
                case 4:
                {
                    byte num4 = reader.ReadByte();
                    double num5 = reader.ReadDouble();
                    ExcelCustomFilter filter3 = new ExcelCustomFilter {
                        Operator = (ExcelFilterOperator) num4,
                        ValueType = num,
                        Value = (double) num5
                    };
                    filter = filter3;
                    cch = -1;
                    return;
                }
                case 6:
                {
                    byte num6 = reader.ReadByte();
                    if (this._isAutoFilter || this._isInFeature11)
                    {
                        reader.Seek(4, (SeekOrigin) SeekOrigin.Current);
                    }
                    cch = reader.ReadByte();
                    ExcelCustomFilter filter4 = new ExcelCustomFilter {
                        Operator = (ExcelFilterOperator) num6,
                        ValueType = num
                    };
                    filter = filter4;
                    reader.ReadByte();
                    reader.ReadBytes(2);
                    if (this._isAutoFilter12)
                    {
                        reader.Seek(4, (SeekOrigin) SeekOrigin.Current);
                    }
                    return;
                }
                case 8:
                {
                    num7 = reader.ReadByte();
                    bool flag2 = reader.ReadBoolean();
                    byte num8 = reader.ReadByte();
                    reader.Seek(6, (SeekOrigin) SeekOrigin.Current);
                    if (!flag2)
                    {
                        ExcelCustomFilter filter6 = new ExcelCustomFilter {
                            Operator = (ExcelFilterOperator) num7,
                            ValueType = num,
                            Value = (num8 != 0) ? ((bool) true) : ((bool) false)
                        };
                        filter = filter6;
                        goto Label_0239;
                    }
                    exception = null;
                    byte num10 = num8;
                    if (num10 > 15)
                    {
                        switch (num10)
                        {
                            case 0x17:
                                exception = (Exception) new NullReferenceException();
                                goto Label_01E2;

                            case 0x1d:
                                exception = (Exception) new MissingMemberException();
                                goto Label_01E2;

                            case 0x24:
                                exception = (Exception) new ArgumentOutOfRangeException();
                                goto Label_01E2;

                            case 0x2a:
                                exception = (Exception) new NotSupportedException();
                                goto Label_01E2;
                        }
                        break;
                    }
                    switch (num10)
                    {
                        case 0:
                            exception = (Exception) new ArgumentNullException();
                            goto Label_01E2;

                        case 7:
                            exception = (Exception) new DivideByZeroException();
                            goto Label_01E2;

                        case 15:
                            exception = (Exception) new ArgumentException();
                            goto Label_01E2;
                    }
                    break;
                }
                default:
                    reader.Seek(9, (SeekOrigin) SeekOrigin.Current);
                    filter = null;
                    cch = -1;
                    return;
            }
        Label_01E2:
            filter5 = new ExcelCustomFilter();
            filter5.Operator = (ExcelFilterOperator) num7;
            filter5.ValueType = num;
            filter5.Value = exception;
            filter = filter5;
        Label_0239:
            cch = -1;
        }

        private void ReadDROPDOWNOBJIDS(BiffRecord biff, short sheetIndex)
        {
        }

        private void ReadDV(BiffRecord biff, short sheetIndex)
        {
            this.ReadDV(biff.DataBuffer, biff.DataLength, sheetIndex);
        }

        private void ReadDV(byte[] data, int dataLen, short sheetIndex)
        {
            List<IRange> list = null;
            try
            {
                ExcelDataValidation dataValidation = new ExcelDataValidation();
                int pos = 0;
                int num = this.GetInt(data[pos], data[pos + 1], data[pos + 2], data[pos + 3]);
                pos += 4;
                dataValidation.Type = (ExcelDataValidationType)(num & 15);
                dataValidation.ErrorStyle = (ExcelDataValidationErrorStyle)((num & 0x70) >> 4);
                dataValidation.CompareOperator = (ExcelDataValidationOperator)((num & 0xf00000) >> 20);
                dataValidation.AllowBlank = (num & 0x100) == 0x100;
                dataValidation.ShowPromptBox = (num & 0x200) != 0x200;
                dataValidation.ShowInputMessage = (num & 0x40000) == 0x40000;
                dataValidation.ShowErrorBox = (num & 0x80000) == 0x80000;
                Func<string> func = delegate {
                    int charCount = this.GetInt(data[pos], data[pos + 1]);
                    pos += 2;
                    if (charCount > 0)
                    {
                        bool highByte = (data[pos] & 1) == 1;
                        pos++;
                        byte[] b = new byte[highByte ? (charCount * 2) : ((int) charCount)];
                        Array.Copy(data, pos, b, 0, b.Length);
                        pos += b.Length;
                        return SimpleBinaryReader.BytesToString(b, charCount, highByte).Trim(new char[1]);
                    }
                    return null;
                };
                dataValidation.PromptTitle = func();
                dataValidation.ErrorTitle = func();
                dataValidation.Prompt = func();
                dataValidation.Error = func();
                int num2 = this.GetInt(data[pos], data[pos + 1]);
                pos += 2;
                pos += 2;
                byte[] buffer = null;
                if (num2 > 0)
                {
                    buffer = new byte[num2];
                    Array.Copy(data, pos, buffer, 0, num2);
                    pos += num2;
                }
                int num3 = this.GetInt(data[pos], data[pos + 1]);
                pos += 2;
                pos += 2;
                byte[] buffer2 = null;
                if (num3 > 0)
                {
                    buffer2 = new byte[num3];
                    Array.Copy(data, pos, buffer2, 0, num3);
                    pos += num3;
                }
                list = new List<IRange>();
                int num4 = this.GetInt(data[pos], data[pos + 1]);
                pos += 2;
                for (int i = 0; i < num4; i++)
                {
                    int[] numArray = new int[4];
                    for (int j = 0; j < 4; j++)
                    {
                        numArray[j] = BitConverter.ToUInt16(data, pos);
                        pos += 2;
                    }
                    ExcelCellRange range = new ExcelCellRange {
                        Row = numArray[0],
                        RowSpan = (numArray[1] - numArray[0]) + 1,
                        Column = numArray[2],
                        ColumnSpan = (numArray[3] - numArray[2]) + 1
                    };
                    if (range.RowSpan == 0x10000)
                    {
                        range.RowSpan = 0x100000;
                    }
                    if (range.ColumnSpan == 0x100)
                    {
                        range.ColumnSpan = 0x4000;
                    }
                    list.Add(range);
                }
                dataValidation.Ranges = list;
                ExcelCalcError error = null;
                if ((buffer != null) && (buffer.Length > 0))
                {
                    using (BinaryReader reader = new BinaryReader((Stream) new MemoryStream(buffer)))
                    {
                        string formula = new FormulaProcess(reader, null, sheetIndex, dataValidation.Ranges[0].Row, dataValidation.Ranges[0].Column, this._calcProperty.RefMode == ExcelReferenceStyle.A1) { SheetsSelectionList = this._selections }.ToString(this._linkTable, ref error, null, null, false);
                        if (dataValidation.Type == ExcelDataValidationType.List)
                        {
                            dataValidation.FirstFormula = this.UpdateDVformulaIfNeed(formula);
                        }
                        else
                        {
                            dataValidation.FirstFormula = formula;
                        }
                    }
                }
                if ((buffer2 != null) && (buffer2.Length > 0))
                {
                    using (BinaryReader reader2 = new BinaryReader((Stream) new MemoryStream(buffer2)))
                    {
                        string str2 = new FormulaProcess(reader2, null, sheetIndex, dataValidation.Ranges[0].Row, dataValidation.Ranges[0].Column, this._calcProperty.RefMode == ExcelReferenceStyle.A1) { SheetsSelectionList = this._selections }.ToString(this._linkTable, ref error, new int?(dataValidation.Ranges[0].Row), new int?(dataValidation.Ranges[0].Column), false);
                        if (dataValidation.Type == ExcelDataValidationType.List)
                        {
                            dataValidation.SecondFormula = this.UpdateDVformulaIfNeed(str2);
                        }
                        else
                        {
                            dataValidation.SecondFormula = str2;
                        }
                    }
                }
                this._excelReader.SetValidationData(sheetIndex, dataValidation);
            }
            catch (Exception exception)
            {
                if ((list != null) && (list.Count > 0))
                {
                    this.LogError(ResourceHelper.GetResourceString("readDVError"), ExcelWarningCode.General, sheetIndex, list[0].Row, list[0].Column, exception);
                }
                else
                {
                    this.LogError(ResourceHelper.GetResourceString("readDVError"), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
                }
            }
        }

        private void ReadDVAL(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            reader.ReadInt32();
            reader.ReadInt32();
        }

        private void ReadDxf(BiffRecord biff, short sheetIndex)
        {
            BinaryReader reader = new BinaryReader((Stream) new MemoryStream(biff.DataBuffer));
            reader.ReadBytes(12);
            short num = reader.ReadInt16();
            DifferentialFormatting differentFormatting = new DifferentialFormatting();
            this.ReadXfProps(reader, differentFormatting);
            this._dxfs.Add(differentFormatting);
        }

        private ExcelFont ReadDXFFont(SimpleBinaryReader reader)
        {
            ExcelFont font = new ExcelFont();
            byte num = reader.ReadByte();
            if (num > 0)
            {
                XLUnicodeStringNoCch cch = new XLUnicodeStringNoCch {
                    cch = num
                };
                cch.Read(reader);
                if (!string.IsNullOrWhiteSpace(cch.Text))
                {
                    font.FontName = cch.DecodedText;
                }
                int count = 0x3f;
                if (cch.fHighByte == 1)
                {
                    count = 0x3f - (1 + (2 * num));
                }
                else
                {
                    count = 0x3e - num;
                }
                if (count > 0)
                {
                    reader.ReadBytes(count);
                }
            }
            else
            {
                reader.ReadBytes(0x3f);
            }
            int num3 = reader.ReadInt32();
            if (num3 == -1)
            {
                font.FontSize = 11.0;
            }
            if ((num3 >= 20) && (num3 <= 0x1fff))
            {
                font.FontSize = num3;
            }
            else
            {
                font.FontSize = 0.0;
            }
            int num4 = reader.ReadInt32();
            short num5 = reader.ReadInt16();
            short num6 = reader.ReadInt16();
            byte num7 = reader.ReadByte();
            reader.ReadBytes(3);
            int num8 = reader.ReadInt32();
            reader.ReadBytes(4);
            int num9 = reader.ReadInt32();
            bool flag = (num9 & 2) == 0;
            bool flag2 = (num9 & 8) == 0;
            bool flag3 = (num9 & 0x10) == 0;
            bool flag4 = (num9 & 0x80) == 0;
            bool flag5 = reader.ReadUInt32() == 0;
            bool flag6 = reader.ReadUInt32() == 0;
            reader.ReadBytes(0x12);
            if (flag && ((num4 & 2) == 2))
            {
                font.IsItalic = true;
            }
            if (flag && ((num5 & 700) == 700))
            {
                font.IsBold = true;
            }
            if (flag2 && ((num4 & 8) == 8))
            {
                font.IsOutlineStyle = true;
            }
            if (flag3 && ((num4 & 0x10) == 0x10))
            {
                font.IsShadowStyle = true;
            }
            if (flag4 && ((num4 & 0x80) == 0x80))
            {
                font.IsStrikeOut = true;
            }
            if (flag6)
            {
                font.UnderLineStyle = (UnderLineStyle) num7;
            }
            if (flag5)
            {
                font.VerticalAlignRun = (VerticalAlignRun)(byte)num6;
            }
            if (num8 != -1)
            {
                font.FontColor = new ExcelColor(ExcelColorType.Indexed, (uint) num8, 0.0);
                return font;
            }
            font.FontColor = null;
            return font;
        }

        private DifferentialFormatting ReadDxfn12List(SimpleBinaryReader reader)
        {
            DifferentialFormatting dxfExt = this.ReadDifferentialFormattingRecord(reader);
            if (reader.Remaining > 0)
            {
                this.ReadXFExtNoFRT(reader, dxfExt);
            }
            return dxfExt;
        }

        private void ReadEOF(BiffRecord biff, short sheetIndex)
        {
            this._excelReader.SetCalculationProperty(this._calcProperty);
            if (sheetIndex == -1)
            {
                foreach (IExcelStyle style in this._styles)
                {
                    try
                    {
                        this._excelReader.SetExcelStyle(style);
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, -1, -1, -1, exception);
                    }
                }
                try
                {
                    this._excelReader.SetExcelDefaultCellFormat(this._cellFormats[15]);
                }
                catch (Exception exception2)
                {
                    this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, -1, -1, -1, exception2);
                }
                for (int i = 0; i < this._cellFormats.Count; i++)
                {
                    try
                    {
                        this._excelReader.SetExcelCellFormat(this._cellFormats[i]);
                    }
                    catch (Exception exception3)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, -1, -1, -1, exception3);
                    }
                }
                this._excelReader.SetExcelWorkbookProperty(this._workbookProperty);
                this._linkTable.CustomNames = this._customNameList;
                this._linkTable.InternalSheetNames = this._sheetNames;
                FormulaProcess process = new FormulaProcess {
                    isDefinedNameFormula = true
                };
                ExcelCalcError error = null;
                int? rowBase = null;
                int? columnBase = null;
                List<IBuiltInName> builtInNames = new List<IBuiltInName>();
                for (int j = 0; j < this._customNameList.Count; j++)
                {
                    object[] objArray = this._customNameList[j] as object[];
                    string str = (string) (objArray[0] as string);
                    if (objArray[4] != null)
                    {
                        byte[] buffer = (byte[]) objArray[1];
                        byte[] buffer2 = (byte[]) objArray[5];
                        int sheet = (int) ((int) objArray[2]);
                        if (sheet != -1)
                        {
                            if ((sheet < this._selections.Count) && (sheet >= 0))
                            {
                                rowBase = new int?(this._selections[sheet].Item1);
                                columnBase = new int?(this._selections[sheet].Item2);
                            }
                            else
                            {
                                rowBase = new int?(this._selections[0].Item1);
                                columnBase = new int?(this._selections[0].Item2);
                            }
                        }
                        else if ((this._activeSheetIndex < this._selections.Count) && (this._activeSheetIndex >= 0))
                        {
                            rowBase = new int?(this._selections[this._activeSheetIndex].Item1);
                            columnBase = new int?(this._selections[this._activeSheetIndex].Item2);
                        }
                        else
                        {
                            rowBase = new int?(this._selections[0].Item1);
                            columnBase = new int?(this._selections[0].Item2);
                        }
                        string text = "";
                        if ((buffer != null) && (buffer.Length > 0))
                        {
                            try
                            {
                                using (BinaryReader reader = new BinaryReader((Stream) new MemoryStream(buffer)))
                                {
                                    process.formulaReader = reader;
                                    process.formulaExtraReader = null;
                                    if (buffer2 != null)
                                    {
                                        process.formulaExtraReader = new BinaryReader((Stream) new MemoryStream(buffer2));
                                    }
                                    process.name = str;
                                    process.isA1RefStyle = this._isA1ReferenceStyle;
                                    process.SheetsSelectionList = this._selections;
                                    process.activeSheet = this._activeSheetIndex;
                                    process.row = rowBase.HasValue ? rowBase.Value : 0;
                                    process.column = columnBase.HasValue ? columnBase.Value : 0;
                                    text = process.ToString(this._linkTable, ref error, rowBase, columnBase, true);
                                    if ((process.row != 0) || (process.column != 0))
                                    {
                                        text = Parser.Unparse(Parser.Parse(text, process.row, process.column, !this._isA1ReferenceStyle, this._linkTable), 0, 0, !this._isA1ReferenceStyle);
                                    }
                                    if (process.formulaExtraReader != null)
                                    {
                                        process.formulaExtraReader.Close();
                                    }
                                }
                            }
                            catch (Exception exception4)
                            {
                                if (exception4 is ExcelException)
                                {
                                    ExcelException exception5 = exception4 as ExcelException;
                                    if (exception5.Code == ExcelExceptionCode.FormulaError)
                                    {
                                        string resourceString = ResourceHelper.GetResourceString("defindedNameFomulaReadError");
                                        this.LogError(string.Format(resourceString, (object[]) new object[] { str }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception4);
                                    }
                                }
                                else
                                {
                                    string str4 = ResourceHelper.GetResourceString("definedNameGeneralError");
                                    this.LogError(string.Format(str4, (object[]) new object[] { str }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception4);
                                }
                            }
                        }
                        if (!((bool) objArray[4]))
                        {
                            NamedCellRange namedCellRange = new NamedCellRange(str, sheet);
                            if ((this._calcProperty != null) && !this._isA1ReferenceStyle)
                            {
                                namedCellRange.RefersToR1C1 = text;
                            }
                            else
                            {
                                namedCellRange.RefersTo = text;
                            }
                            namedCellRange.IsHidden = (bool) ((bool) objArray[3]);
                            namedCellRange.DefinitionBits = new Tuple<int, byte[]>(j, buffer);
                            namedCellRange.ExtraDefinitionBits = new Tuple<int, byte[]>(j, buffer2);
                            if (namedCellRange.Index == -1)
                            {
                                this._excelReader.SetNamedCellRange(namedCellRange);
                            }
                            else
                            {
                                if (!this._localDefinedNames.ContainsKey(namedCellRange.Index))
                                {
                                    this._localDefinedNames.Add(namedCellRange.Index, new List<NamedCellRange>());
                                }
                                this._localDefinedNames[namedCellRange.Index].Add(namedCellRange);
                            }
                        }
                        else
                        {
                            BuiltInName name = new BuiltInName(str) {
                                NameBits = new Tuple<int, byte[]>(j, buffer),
                                ExtraBits = new Tuple<int, byte[]>(j, buffer2)
                            };
                            builtInNames.Add(name);
                        }
                    }
                }
                this._excelReader.SetCustomOrFunctionNameList(this._customFunctions);
                this._excelReader.SetBuiltInNameList(builtInNames);
                foreach (IExternalWorkbookInfo info in this._externalWorkbookInfos)
                {
                    this._excelReader.SetExternalReferencedWorkbookInfo(info);
                }
            }
            else
            {
                List<NamedCellRange> list2;
                string str5 = null;
                if (this._printTitles.TryGetValue(sheetIndex, out str5) && !string.IsNullOrWhiteSpace(str5))
                {
                    this._excelReader.SetPrintTitles(sheetIndex, str5);
                }
                string str6 = null;
                if (this._printAreas.TryGetValue(sheetIndex, out str6) && !string.IsNullOrWhiteSpace(str6))
                {
                    this._excelReader.SetPrintArea(sheetIndex, str6);
                }
                if (this._localDefinedNames.TryGetValue(sheetIndex, out list2))
                {
                    foreach (NamedCellRange range2 in list2)
                    {
                        this._excelReader.SetNamedCellRange(range2);
                    }
                }
                this._excelReader.SetPrintPageMargin(sheetIndex, this._printPageMargin);
                this._excelReader.SetPrintOption(sheetIndex, this._printOptions);
                this._excelReader.SetPrintPageSetting(sheetIndex, this._printPageSetting);
                List<ExcelConditionalFormat> list3 = new List<ExcelConditionalFormat>();
                foreach (ExcelConditionalFormat format in this._conditonalFormat)
                {
                    list3.Add(format);
                }
                foreach (ExcelConditionalFormat format2 in this._conditionaFormat12)
                {
                    list3.Add(format2);
                }
                if (list3.Count > 0)
                {
                    this._sheetsConditionalFormats.Add(sheetIndex, list3);
                }
                this._tables[sheetIndex] = new List<IExcelTable>((IEnumerable<IExcelTable>) this._sheetTables);
                this._sheetTables.Clear();
            }
        }

        private void ReadExternCount(BiffRecord biff, short sheetIndex)
        {
            BitConverter.ToUInt16(biff.DataBuffer, 0);
        }

        private void ReadExternName(BiffRecord biff, short sheetIndex)
        {
            this.externNameRecordIndex++;
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            short num = reader.ReadInt16();
            short num2 = (short)(reader.ReadInt16() - 1);
            reader.ReadBytes(2);
            string outString = null;
            short charCount = reader.ReadByte();
            reader.ReadUncompressedString(charCount, out outString);
            this._linkTable.AddExternNames(outString, num2);
            if (this._externalWorkbookInfos.Count > 0)
            {
                ExternalWorkbookInfo info = this._externalWorkbookInfos[this._externalWorkbookInfos.Count - 1] as ExternalWorkbookInfo;
                if (!string.IsNullOrWhiteSpace(info.Name))
                {
                    NamedCellRange range = new NamedCellRange(outString, num2) {
                        DefinitionBits = new Tuple<int, byte[]>(this.externNameRecordIndex, biff.DataBuffer)
                    };
                    info.DefinedNames.Add(range);
                    info.ExternNameBits.Add(new Tuple<int, byte[]>(this.externNameRecordIndex, biff.DataBuffer));
                }
                else
                {
                    ExcelCustomFunction function = new ExcelCustomFunction {
                        Name = outString,
                        ExternNameBits = new Tuple<int, byte[]>(this.externNameRecordIndex, biff.DataBuffer)
                    };
                    this._customFunctions.Add(function);
                }
            }
        }

        private void ReadExternSheet(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                this.externSheetCount++;
                short externBookIndex = reader.ReadInt16();
                short beginSheetIndex = reader.ReadInt16();
                short endSheetIndex = reader.ReadInt16();
                ExternalWorkbookInfo info = this._externalWorkbookInfos[externBookIndex] as ExternalWorkbookInfo;
                info.ExternSheetBits.Add(new Tuple<int, short[]>(this.externSheetCount, new short[] { externBookIndex, beginSheetIndex, endSheetIndex }));
                this._linkTable.AddExternSheet(externBookIndex, beginSheetIndex, endSheetIndex);
            }
        }

        private void ReadFeatheadr(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadBytes(8);
            SharedFeatureType type = (SharedFeatureType) reader.ReadInt16();
            reader.ReadByte();
            if (reader.ReadUInt32() == uint.MaxValue)
            {
            }
        }

        public void ReadFeatheadr11(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadBytes(8);
            reader.ReadInt16();
            reader.ReadByte();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadInt16();
        }

        private void ReadFeature11(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadInt16();
            short num = reader.ReadInt16();
            reader.ReadBytes(8);
            reader.ReadInt16();
            ExcelTable table = new ExcelTable();
            this._sheetTables.Add(table);
            reader.ReadByte();
            reader.ReadInt32();
            short num2 = reader.ReadInt16();
            if (reader.ReadInt32() == 0)
            {
                int num3 = (biff.DataLength - (8 * num2)) - 0x1b;
            }
            reader.ReadInt16();
            List<IRange> list = new List<IRange>();
            for (int i = 0; i < num2; i++)
            {
                list.Add(this.GetRange(reader));
            }
            if (num2 > 0)
            {
                table.Range = list[0];
            }
            int num5 = reader.ReadInt32();
            int num6 = reader.ReadInt32();
            table.Id = num6;
            bool flag = reader.ReadInt32() == 1;
            table.ShowHeaderRow = flag;
            table.ShowTotalsRow = reader.ReadInt32() == 1;
            reader.ReadInt32();
            reader.ReadInt32();
            reader.ReadInt16();
            reader.ReadInt16();
            byte num7 = reader.ReadByte();
            if ((num7 & 2) == 2)
            {
                table.AutoFilter = new ExcelAutoFilter();
                table.AutoFilter.Range = table.Range;
            }
            table.ShowTotalsRow = (num7 & 0x40) == 0x40;
            byte num8 = reader.ReadByte();
            bool flag3 = (num8 & 4) == 4;
            bool flag4 = (num8 & 0x40) == 0x40;
            bool flag5 = (reader.ReadByte() & 0x10) == 0x10;
            reader.ReadByte();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadUInt32();
            reader.ReadInt32();
            reader.ReadBytes(0x10);
            XLUnicodeString str = new XLUnicodeString();
            str.Read(reader);
            table.Name = str.Text;
            int num10 = reader.ReadUInt16();
            if (flag4)
            {
                str = new XLUnicodeString();
                str.Read(reader);
                string text = str.Text;
            }
            if (flag5)
            {
                str = new XLUnicodeString();
                str.Read(reader);
                string text2 = str.Text;
            }
            for (int j = 0; j < num10; j++)
            {
                ExcelTableColumn column = new ExcelTableColumn {
                    Id = reader.ReadInt32()
                };
                reader.ReadUInt32();
                reader.ReadUInt32();
                column.TotalsRowFunction = (ExcelTableTotalsRowFunction) reader.ReadInt32();
                uint num12 = reader.ReadUInt32();
                reader.ReadUInt32();
                int num14 = reader.ReadInt32();
                bool flag6 = (num14 & 1) == 1;
                bool flag7 = (num14 & 4) == 4;
                bool flag8 = (num14 & 8) == 8;
                bool flag9 = (num14 & 0x80) == 0x80;
                bool flag10 = (num14 & 0x100) == 0x100;
                bool flag11 = (num14 & 0x200) == 0x200;
                bool flag12 = (num14 & 0x400) == 0x400;
                uint num15 = reader.ReadUInt32();
                reader.ReadUInt32();
                str = new XLUnicodeString();
                str.Read(reader);
                column.Name = str.Text;
                if (!flag3)
                {
                    str = new XLUnicodeString();
                    str.Read(reader);
                    string str2 = str.Text;
                    column.Name = str2;
                }
                if (num12 > 0)
                {
                    SimpleBinaryReader reader2 = new SimpleBinaryReader(reader.ReadBytes((int) num12));
                    this.ReadDxfn12List(reader2);
                }
                if (num15 > 0)
                {
                    SimpleBinaryReader reader3 = new SimpleBinaryReader(reader.ReadBytes((int) num15));
                    this.ReadDxfn12List(reader3);
                }
                if (flag6)
                {
                    int count = reader.ReadInt32();
                    reader.ReadInt16();
                    if (count > 0)
                    {
                        SimpleBinaryReader reader4 = new SimpleBinaryReader(reader.ReadBytes(count));
                        IExcelFilterColumn column2 = this.ReadTableAutoFilter(reader4, j);
                        if (table.AutoFilter != null)
                        {
                            table.AutoFilter.FilterColumns.Add(column2);
                        }
                    }
                }
                if (flag7)
                {
                    ushort num17 = reader.ReadUInt16();
                    for (int k = 0; k < num17; k++)
                    {
                        int num19 = reader.ReadInt32();
                        reader.ReadInt32();
                        new XLUnicodeString().Read(reader);
                    }
                }
                if (flag8)
                {
                    ushort num20 = reader.ReadUInt16();
                    new SimpleBinaryReader(reader.ReadBytes(num20)).ToString();
                }
                if (flag9)
                {
                    short num21 = reader.ReadInt16();
                    using (MemoryStream stream = new MemoryStream(reader.ReadBytes(num21)))
                    {
                        FormulaProcess process = new FormulaProcess {
                            formulaReader = new BinaryReader((Stream) stream),
                            isA1RefStyle = this._isA1ReferenceStyle
                        };
                        ExcelCalcError error = null;
                        int? rowBase = null;
                        int? columnBase = null;
                        string str4 = process.ToString(this._linkTable, ref error, rowBase, columnBase, false);
                        column.TotalsRowCustomFunction = str4;
                        column.TotalsRowFunctionIsArrayFormula = flag10;
                    }
                }
                if (flag12)
                {
                    XLUnicodeString str5 = new XLUnicodeString();
                    str5.Read(reader);
                    column.TotalsRowLabel = str5.Text;
                }
                switch (num5)
                {
                    case 1:
                        reader.ReadInt32();
                        reader.ReadUInt32();
                        reader.ReadInt32();
                        reader.ReadInt32();
                        break;

                    case 3:
                        reader.ReadUInt32();
                        break;
                }
                if (!flag3 && !flag)
                {
                    uint num22 = reader.ReadUInt32();
                    if (num22 > 0)
                    {
                        SimpleBinaryReader reader6 = new SimpleBinaryReader(reader.ReadBytes((int) num22));
                        this.ReadDxfn12List(reader6);
                        if (flag11)
                        {
                            XLUnicodeString str6 = new XLUnicodeString();
                            str6.Read(reader);
                            string text3 = str6.Text;
                        }
                    }
                }
                table.Columns.Add(column);
            }
        }

        private void ReadFont(BiffRecord biff, short sheetIndex)
        {
            FONTRecord record = new FONTRecord {
                RecordLength = (ushort)biff.DataBuffer.Length
            };
            record.Read(new SimpleBinaryReader(biff.DataBuffer));
            if (((record.ColorIndex > 0x3f) && (record.ColorIndex != 0x51)) && (record.ColorIndex != 0x7fff))
            {
                this.LogError(ResourceHelper.GetResourceString("biffReadFontError"), ExcelWarningCode.General, -1, -1, -1, null);
                record.ColorIndex = 8;
            }
            ExcelFont excelFont = ConverterFactory.GetExcelFont(record);
            int num = (this._fonts.Keys.Count >= 4) ? (this._fonts.Keys.Count + 1) : this._fonts.Keys.Count;
            this._fonts.Add(num, excelFont);
        }

        private void ReadFooter(BiffRecord biff, short sheetIndex)
        {
            if (biff.DataLength > 0)
            {
                string outString = null;
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                if (reader.ReadCompressedString(2, out outString) && !string.IsNullOrEmpty(outString))
                {
                    this._printPageSetting.Footer = outString;
                }
            }
        }

        private void ReadFormat(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int num = reader.ReadUInt16();
            FORMATRecord record = new FORMATRecord {
                RecordLength = (ushort)biff.DataBuffer.Length
            };
            record.Read(reader);
            if (!this._numberFormats.ContainsKey(num))
            {
                this._numberFormats[num] = record.FormatString;
            }
        }

        private ExcelColor ReadFullColorExt(SimpleBinaryReader reader)
        {
            short num = reader.ReadInt16();
            double tint = ((double) reader.ReadInt16()) / 32767.0;
            ExcelColor emptyColor = ExcelColor.EmptyColor;
            switch (num)
            {
                case 1:
                    emptyColor = new ExcelColor(ExcelColorType.Indexed, reader.ReadUInt32(), tint);
                    break;

                case 2:
                {
                    byte red = reader.ReadByte();
                    byte green = reader.ReadByte();
                    byte blue = reader.ReadByte();
                    byte alpha = reader.ReadByte();
                    emptyColor = new ExcelColor(ExcelColorType.RGB, GcColor.FromArgb(alpha, red, green, blue).ToArgb(), tint);
                    break;
                }
                case 3:
                    emptyColor = new ExcelColor(ExcelColorType.Theme, reader.ReadUInt32(), tint);
                    break;
            }
            reader.ReadBytes(8);
            return emptyColor;
        }

        private void ReadGuts(BiffRecord biff, short sheetIndex)
        {
            short rowGutter = BitConverter.ToInt16(biff.DataBuffer, 0);
            short columnGutter = BitConverter.ToInt16(biff.DataBuffer, 2);
            short rowMaxOutlineLevel = BitConverter.ToInt16(biff.DataBuffer, 4);
            short columnMaxOutlineLevel = BitConverter.ToInt16(biff.DataBuffer, 6);
            this._excelReader.SetRowColumnGutters(sheetIndex, rowGutter, columnGutter, rowMaxOutlineLevel, columnMaxOutlineLevel);
        }

        private void ReadHCenter(BiffRecord biff, short sheetIndex)
        {
            bool flag = BitConverter.ToInt16(biff.DataBuffer, 0) == 1;
            this._printOptions.HorizontalCentered = flag;
        }

        private void ReadHeader(BiffRecord biff, short sheetIndex)
        {
            if (biff.DataLength > 0)
            {
                string outString = null;
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                if (reader.ReadCompressedString(2, out outString) && !string.IsNullOrEmpty(outString))
                {
                    this._printPageSetting.Header = outString;
                }
            }
        }

        private void ReadHeaderFooter(BiffRecord biff, short sheetIndex)
        {
            using (MemoryStream stream = new MemoryStream(biff.DataBuffer))
            {
                BinaryReader reader = new BinaryReader((Stream) stream);
                reader.ReadInt16();
                reader.ReadInt16();
                reader.ReadBytes(8);
                reader.ReadBytes(0x10);
                short num = reader.ReadInt16();
                bool flag = (num & 1) == 1;
                bool flag2 = (num & 2) == 2;
                bool flag3 = (num & 4) == 4;
                bool flag4 = (num & 8) == 8;
                short num2 = reader.ReadInt16();
                short num3 = reader.ReadInt16();
                short num4 = reader.ReadInt16();
                short num5 = reader.ReadInt16();
                string text = null;
                string str2 = null;
                string str3 = null;
                string str4 = null;
                XLUnicodeString str5 = new XLUnicodeString();
                if (num2 > 0)
                {
                    str5.Read(reader);
                    text = str5.Text;
                }
                if (num3 > 0)
                {
                    str5.Read(reader);
                    str2 = str5.Text;
                }
                if (num4 > 0)
                {
                    str5.Read(reader);
                    str3 = str5.Text;
                }
                if (num5 > 0)
                {
                    str5.Read(reader);
                    str4 = str5.Text;
                }
                ExtendedHeadFooterSetting setting = new ExtendedHeadFooterSetting {
                    HeaderFooterDifferentOddEvenPages = flag,
                    HeaderFooterDifferentFirstPage = flag2,
                    HeaderFooterScalesWithDocument = flag3,
                    HeaderFooterAlignWithPageMargin = flag4,
                    HeaderEvenPage = (text == null) ? null : text.Trim(),
                    FooterEvenPage = (str2 == null) ? null : str2.Trim(),
                    HeaderFirstPage = (str3 == null) ? null : str3.Trim(),
                    FooterFirstPage = (str4 == null) ? null : str4.Trim()
                };
                this._printPageSetting.AdvancedHeadFooterSetting = setting;
            }
        }

        private void ReadHorizontalPageBreaks(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            List<int> list = new List<int>();
            for (int i = 0; i < num; i++)
            {
                list.Add(reader.ReadUInt16());
                reader.ReadInt32();
            }
            this._printPageSetting.RowBreakLines = list;
        }

        private void ReadIteration(BiffRecord biff, short sheetIndex)
        {
            switch (BitConverter.ToInt16(biff.DataBuffer, 0))
            {
                case 0:
                    this._calcProperty.IsIterateCalculate = false;
                    return;

                case 1:
                    this._calcProperty.IsIterateCalculate = true;
                    break;
            }
        }

        private void ReadLeftMargin(BiffRecord biff, short sheetIndex)
        {
            double num = BitConverter.ToDouble(biff.DataBuffer, 0);
            this._printPageMargin.Left = num;
        }

        private void ReadList12(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadBytes(12);
            ushort num = reader.ReadUInt16();
            reader.ReadUInt32();
            switch (num)
            {
                case 0:
                {
                    int count = reader.ReadInt32();
                    int num3 = reader.ReadInt32();
                    int num4 = reader.ReadInt32();
                    int num5 = reader.ReadInt32();
                    int num6 = reader.ReadInt32();
                    int num7 = reader.ReadInt32();
                    int num8 = reader.ReadInt32();
                    int num9 = reader.ReadInt32();
                    int num10 = reader.ReadInt32();
                    if (count > 0)
                    {
                        SimpleBinaryReader reader2 = new SimpleBinaryReader(reader.ReadBytes(count));
                        this.ReadDxfn12List(reader2);
                    }
                    if (num4 > 0)
                    {
                        SimpleBinaryReader reader3 = new SimpleBinaryReader(reader.ReadBytes(num4));
                        this.ReadDxfn12List(reader3);
                    }
                    if (num6 > 0)
                    {
                        SimpleBinaryReader reader4 = new SimpleBinaryReader(reader.ReadBytes(num6));
                        this.ReadDxfn12List(reader4);
                    }
                    if (num8 > 0)
                    {
                        SimpleBinaryReader reader5 = new SimpleBinaryReader(reader.ReadBytes(num8));
                        this.ReadDxfn12List(reader5);
                    }
                    if (num9 > 0)
                    {
                        SimpleBinaryReader reader6 = new SimpleBinaryReader(reader.ReadBytes(num9));
                        this.ReadDxfn12List(reader6);
                    }
                    if (num10 > 0)
                    {
                        SimpleBinaryReader reader7 = new SimpleBinaryReader(reader.ReadBytes(num10));
                        this.ReadDxfn12List(reader7);
                    }
                    if (num3 != -1)
                    {
                        XLUnicodeString str = new XLUnicodeString();
                        str.Read(reader);
                        string text = str.Text;
                    }
                    if (num5 != -1)
                    {
                        XLUnicodeString str2 = new XLUnicodeString();
                        str2.Read(reader);
                        string text2 = str2.Text;
                    }
                    if (num7 != -1)
                    {
                        XLUnicodeString str3 = new XLUnicodeString();
                        str3.Read(reader);
                        string text3 = str3.Text;
                        return;
                    }
                    break;
                }
                case 1:
                {
                    ushort num11 = reader.ReadUInt16();
                    bool flag = (num11 & 1) == 1;
                    bool flag2 = (num11 & 2) == 2;
                    bool flag3 = (num11 & 4) == 4;
                    bool flag4 = (num11 & 8) == 8;
                    XLUnicodeString str4 = new XLUnicodeString();
                    str4.Read(reader);
                    string str5 = str4.Text;
                    if (str5 == "\0")
                    {
                        str5 = null;
                    }
                    ExcelTableStyleInfo info = new ExcelTableStyleInfo {
                        Name = str5,
                        ShowFirstColumn = flag,
                        ShowLastColumn = flag2,
                        ShowRowStripes = flag3,
                        ShowColumnStripes = flag4
                    };
                    if (this._sheetTables.Count > 0)
                    {
                        this._sheetTables[this._sheetTables.Count - 1].TableStyleInfo = info;
                        return;
                    }
                    break;
                }
                case 2:
                {
                    XLUnicodeString str6 = new XLUnicodeString();
                    str6.Read(reader);
                    string text4 = str6.Text;
                    str6 = new XLUnicodeString();
                    str6.Read(reader);
                    string text5 = str6.Text;
                    break;
                }
            }
        }

        private void ReadLISTDV(BiffRecord biff, short sheetIndex)
        {
            int dataLen = biff.DataLength - 12;
            byte[] data = new byte[dataLen];
            Array.Copy(biff.DataBuffer, 12, data, 0, dataLen);
            this.ReadDV(data, dataLen, sheetIndex);
        }

        private void ReadMergeCells(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            ushort rowStart = 0;
            ushort columnStart = 0;
            for (int i = 0; i < num; i++)
            {
                try
                {
                    rowStart = reader.ReadUInt16();
                    ushort rowEnd = reader.ReadUInt16();
                    columnStart = reader.ReadUInt16();
                    ushort columnEnd = reader.ReadUInt16();
                    this._excelReader.SetMergeCells(sheetIndex, rowStart, rowEnd, columnStart, columnEnd);
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("readMergeCellError"), ExcelWarningCode.General, sheetIndex, rowStart, columnStart, exception);
                }
            }
        }

        private void ReadMulCellType(BiffRecord biff, short sheetIndex)
        {
            ushort num2;
            ushort num4;
            int startIndex = 0;
            ushort row = BitConverter.ToUInt16(biff.DataBuffer, startIndex);
            startIndex += 2;
            ushort num3 = BitConverter.ToUInt16(biff.DataBuffer, startIndex);
            startIndex += 2;
            switch (biff.RecordType)
            {
                case BiffRecordNumber.MULRK:
                {
                    uint num9 = 0;
                    rkrec[] rkrecArray = null;
                    num9 = (uint)((biff.DataLength - 6) / 6);
                    rkrecArray = new rkrec[num9];
                    for (int i = 0; i < num9; i++)
                    {
                        rkrecArray[i].ixfe = BitConverter.ToInt16(biff.DataBuffer, startIndex);
                        startIndex += 2;
                        rkrecArray[i].RK = BitConverter.ToInt32(biff.DataBuffer, startIndex);
                        startIndex += 4;
                    }
                    num4 = BitConverter.ToUInt16(biff.DataBuffer, startIndex);
                    startIndex += 2;
                    for (num2 = num3; num2 <= num4; num2++)
                    {
                        bool flag;
                        double num5 = this.NumFromRk((long) rkrecArray[num2 - num3].RK, out flag);
                        this._excelReader.SetCell(sheetIndex, row, num2, (double) num5, CellType.Numeric, rkrecArray[num2 - num3].ixfe, null);
                    }
                    return;
                }
                case BiffRecordNumber.MULBLANK:
                {
                    int num7 = 0;
                    int[] numArray = null;
                    num7 = (biff.DataLength - 6) / 2;
                    numArray = new int[num7];
                    for (int j = 0; j < num7; j++)
                    {
                        numArray[j] = BitConverter.ToInt16(biff.DataBuffer, startIndex);
                        startIndex += 2;
                    }
                    num4 = BitConverter.ToUInt16(biff.DataBuffer, startIndex);
                    startIndex += 2;
                    for (num2 = num3; num2 <= num4; num2++)
                    {
                        this._excelReader.SetCell(sheetIndex, row, num2, null, CellType.Unknown, numArray[num2 - num3], null);
                    }
                    return;
                }
            }
        }

        private void ReadName(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            bool flag = (num & 1) == 1;
            bool flag2 = (num & 2) == 2;
            bool flag3 = (num & 0x20) == 0x20;
            reader.ReadByte();
            byte num2 = reader.ReadByte();
            ushort count = reader.ReadUInt16();
            reader.ReadUInt16();
            ushort num4 = reader.ReadUInt16();
            reader.ReadUInt32();
            short charCount = num2;
            byte num6 = (byte)(reader.ReadByte() & 1);
            byte[] b = reader.ReadBytes((num6 == 1) ? (charCount * 2) : charCount);
            byte[] formulaBytes = reader.ReadBytes(count);
            byte[] buffer3 = null;
            if (reader.Remaining > 0)
            {
                buffer3 = reader.ReadBytes(reader.Remaining);
            }
            object[] objArray = new object[6];
            string name = null;
            if (!flag3)
            {
                name = SimpleBinaryReader.BytesToString(b, charCount, num6 == 1);
                if ((name.Length > 6) && name.StartsWith("_xlfn."))
                {
                    name = name.Substring(6).ToUpperInvariant();
                }
                objArray[0] = name;
                objArray[1] = formulaBytes;
                objArray[2] = num4 - 1;
                objArray[3] = (bool) flag;
                objArray[4] = (bool) flag2;
                objArray[5] = buffer3;
            }
            else
            {
                byte num7 = b[0];
                int num8 = num4 - 1;
                switch (num7)
                {
                    case 0:
                        name = "Consolidate_Area";
                        break;

                    case 1:
                        name = "Auto_Open";
                        break;

                    case 2:
                        name = "Auto_Close";
                        break;

                    case 3:
                        name = "Extract";
                        break;

                    case 4:
                        name = "Database";
                        break;

                    case 5:
                        name = "Criteria";
                        break;

                    case 6:
                        name = "_xlnm.Print_Area";
                        this._printAreas[num8] = this.GetFormula(formulaBytes, null);
                        break;

                    case 7:
                        name = "_xlnm.Print_Titles";
                        this._printTitles[num8] = this.GetFormula(formulaBytes, null);
                        break;

                    case 8:
                        name = "Recorder";
                        break;

                    case 9:
                        name = "Data_Form";
                        break;

                    case 10:
                        name = "Auto_Activate";
                        break;

                    case 11:
                        name = "Auto_Deactivate";
                        break;

                    case 12:
                        name = "Sheet_Title";
                        break;

                    case 13:
                    {
                        name = "_FilterDatabase";
                        int num9 = num4 - 1;
                        if ((formulaBytes != null) && (formulaBytes.Length > 0))
                        {
                            ExcelAutoFilter filter = new ExcelAutoFilter();
                            ExternalRangeExpression expression = Parser.Parse(this.GetFormula(formulaBytes, null), 0, 0, this._calcProperty.RefMode == ExcelReferenceStyle.R1C1, this._linkTable).Value as ExternalRangeExpression;
                            if (expression != null)
                            {
                                ExcelCellRange range = new ExcelCellRange {
                                    Row = expression.Row,
                                    Column = expression.Column,
                                    RowSpan = expression.RowCount,
                                    ColumnSpan = expression.ColumnCount
                                };
                                if ((range.Column == -1) && (range.ColumnSpan == 0))
                                {
                                    range.Column = 0;
                                    range.ColumnSpan = 0x100;
                                }
                                if ((range.Row == -1) && (range.RowSpan == 0))
                                {
                                    range.Row = 0;
                                    range.RowSpan = 0x10000;
                                }
                                filter.Range = range;
                                if (this._autoFilters == null)
                                {
                                    this._autoFilters = new Dictionary<int, IExcelAutoFilter>();
                                }
                                this._autoFilters[num9] = filter;
                            }
                        }
                        break;
                    }
                }
                objArray[0] = name;
                objArray[1] = formulaBytes;
                objArray[2] = num4 - 1;
                objArray[3] = (bool) flag;
                objArray[4] = (bool) flag2;
                objArray[5] = buffer3;
            }
            this._customNameList.Add(objArray);
            this._linkTable.AddDefinedNames(name, (objArray[2] != null) ? ((int) ((int) objArray[2])) : ((int) (-1)), null);
        }

        private void ReadNote(BiffRecord biff, short sheetIndex)
        {
            string str2;
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            short row = reader.ReadInt16();
            short column = reader.ReadInt16();
            short num3 = reader.ReadInt16();
            short num4 = reader.ReadInt16();
            reader.ReadBytes(biff.DataLength - 8);
            bool stickyNote = num3 == 2;
            string str = string.Format("{0}-{1}", (object[]) new object[] { ((short) sheetIndex).ToString(), ((short) num4).ToString() });
            if (this._txoStringTable.TryGetValue(str, out str2))
            {
                this._excelReader.SetCellNote(sheetIndex, row, column, stickyNote, str2);
            }
        }

        private void ReadOBJ(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadUInt16();
            reader.ReadUInt16();
            ushort num = reader.ReadUInt16();
            ushort num2 = reader.ReadUInt16();
            if (num == 0x19)
            {
                this._commentObjectIDs.Enqueue((short) num2);
                this._commentObjCount++;
            }
        }

        private void ReadPane(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort horizontalPosition = reader.ReadUInt16();
            ushort verticalPosition = reader.ReadUInt16();
            ushort topVisibleRow = reader.ReadUInt16();
            ushort leftmostVisibleColumn = reader.ReadUInt16();
            ushort paneIndex = reader.ReadUInt16();
            this._excelReader.SetPane(sheetIndex, horizontalPosition, verticalPosition, topVisibleRow, leftmostVisibleColumn, paneIndex, this._isPanesFrozen);
        }

        private void ReadPrecision(BiffRecord biff, short sheetIndex)
        {
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._calcProperty.IsFullPrecision = num != 0;
        }

        private void ReadPrintGridLines(BiffRecord biff, short sheetIndex)
        {
            bool flag = BitConverter.ToInt16(biff.DataBuffer, 0) == 1;
            this._printOptions.PrintGridLine = flag;
        }

        private void ReadPrintHeaders(BiffRecord biff, short sheetIndex)
        {
            bool flag = BitConverter.ToInt16(biff.DataBuffer, 0) == 1;
            this._printOptions.PrintRowColumnsHeaders = flag;
        }

        private void ReadProtect(BiffRecord biff, short sheetIndex)
        {
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._excelReader.SetProtect(sheetIndex, num == 1);
        }

        private void ReadRefMode(BiffRecord biff, short sheetIndex)
        {
            if (BitConverter.ToInt16(biff.DataBuffer, 0) == 1)
            {
                this._calcProperty.RefMode = ExcelReferenceStyle.A1;
                this._isA1ReferenceStyle = true;
            }
            else
            {
                this._calcProperty.RefMode = ExcelReferenceStyle.R1C1;
                this._isA1ReferenceStyle = false;
            }
        }

        private void ReadRightMargin(BiffRecord biff, short sheetIndex)
        {
            double num = BitConverter.ToDouble(biff.DataBuffer, 0);
            this._printPageMargin.Right = num;
        }

        private void ReadRow(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort row = reader.ReadUInt16();
            ushort firstDefinedColumn = reader.ReadUInt16();
            ushort lastDefinedColumn = reader.ReadUInt16();
            ushort num4 = (ushort)(reader.ReadUInt16() & 0x7fff);
            reader.ReadBytes(4);
            ushort num5 = reader.ReadUInt16();
            byte outlineLevel = (byte)(num5 & 7);
            bool collapsed = (num5 & 0x10) == 0x10;
            bool zeroHeight = (num5 & 0x20) == 0x20;
            bool unSynced = (num5 & 0x40) == 0x40;
            bool ghostDirty = (num5 & 0x80) == 0x80;
            ushort num7 = (ushort)(reader.ReadUInt16() & 0xfff);
            this._excelReader.SetRowInfo(sheetIndex, row, firstDefinedColumn, lastDefinedColumn, (short) num7, ((double) num4) / 20.0, outlineLevel, collapsed, zeroHeight, unSynced, ghostDirty);
        }

        private void ReadSaveReCalc(BiffRecord biff, short sheetIndex)
        {
            short num = BitConverter.ToInt16(biff.DataBuffer, 0);
            this._calcProperty.ReCalculationBeforeSave = num == 1;
        }

        private void ReadSCL(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            float num = Convert.ToSingle((short) reader.ReadInt16(), (IFormatProvider) CultureInfo.InvariantCulture);
            float num2 = Convert.ToSingle((short) reader.ReadInt16(), (IFormatProvider) CultureInfo.InvariantCulture);
            float zoom = num / num2;
            this._excelReader.SetZoom(sheetIndex, zoom);
        }

        private void ReadSelection(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            byte num = reader.ReadByte();
            ushort rowActive = reader.ReadUInt16();
            ushort columnActive = reader.ReadUInt16();
            ushort num4 = reader.ReadUInt16();
            ushort selectionCount = reader.ReadUInt16();
            List<int> rowFirst = new List<int>();
            List<int> rowLast = new List<int>();
            List<int> colFirst = new List<int>();
            List<int> colLast = new List<int>();
            for (int i = 0; i < selectionCount; i++)
            {
                int num7 = reader.ReadInt16();
                int num8 = reader.ReadInt16();
                int num9 = reader.ReadByte();
                int num10 = reader.ReadByte();
                if (((i != num4) || (num7 != num8)) || (num9 != num10))
                {
                    rowFirst.Add(num7);
                    rowLast.Add(num8);
                    colFirst.Add(num9);
                    colLast.Add(num10);
                }
            }
            this._excelReader.SetSelection(sheetIndex, (PaneType) num, rowActive, columnActive, selectionCount, rowFirst, rowLast, colFirst, colLast);
        }

        private void ReadSetup(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            short num = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            short num3 = reader.ReadInt16();
            short num4 = reader.ReadInt16();
            short num5 = reader.ReadInt16();
            short num6 = reader.ReadInt16();
            bool flag = (num6 & 1) == 1;
            bool flag2 = (num6 & 2) == 2;
            bool flag3 = (num6 & 4) == 4;
            bool flag4 = (num6 & 8) == 8;
            bool flag5 = (num6 & 0x10) == 0x10;
            bool flag6 = (num6 & 0x20) == 0x20;
            bool flag7 = (num6 & 0x40) == 0x40;
            bool flag8 = (num6 & 0x80) == 0x80;
            bool flag9 = (num6 & 0x200) == 0x200;
            reader.ReadInt16();
            reader.ReadInt16();
            double num7 = reader.ReadDouble();
            double num8 = reader.ReadDouble();
            ushort num9 = reader.ReadUInt16();
            if (flag3)
            {
                num9 = 1;
            }
            this._printPageSetting.CellErrorPrintStyle = (ExcelCellErrorPrintStyle)((num6 >> 10) & 3);
            if (flag8)
            {
                this._printPageSetting.UseCustomStartingPage = true;
                this._printPageSetting.FirstPageNumber = num3;
            }
            if (!flag3)
            {
                if (!flag7)
                {
                    this._printPageSetting.Orientation = flag2 ? ExcelPrintOrientation.Portrait : ExcelPrintOrientation.Landscape;
                }
                else
                {
                    this._printPageSetting.Orientation = ExcelPrintOrientation.Auto;
                }
                this._printPageSetting.ZoomFactor = ((float) num2) / 100f;
            }
            this._printPageSetting.ShowColor = !flag4;
            this._printPageSetting.PageOrder = flag ? ExcelPrintPageOrder.OverThenDown : ExcelPrintPageOrder.DownThenOver;
            if (num7 != 0.0)
            {
                this._printPageMargin.Header = num7;
            }
            if (num8 != 0.0)
            {
                this._printPageMargin.Footer = num8;
            }
            if (flag6)
            {
                this._printPageSetting.CommentsStyle = flag9 ? ExcelPrintNotesStyle.AtEnd : ExcelPrintNotesStyle.AsDisplayed;
            }
            else
            {
                this._printPageSetting.CommentsStyle = ExcelPrintNotesStyle.None;
            }
            this._printPageSetting.PaperSizeIndex = num;
            this._printPageSetting.Draft = flag5;
            this._printPageSetting.Copies = num9;
            this._printPageSetting.UseSmartPrint = this._isPrintFitToPage;
            this._printPageSetting.SmartPrintPagesWidth = num4;
            this._printPageSetting.SmartPrintPagesHeight = num5;
        }

        private void ReadShareFormula(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int rowFirst = reader.ReadUInt16();
            int rowLast = reader.ReadUInt16();
            short colFirst = reader.ReadByte();
            short colLast = reader.ReadByte();
            byte[] buffer = null;
            byte[] buffer2 = null;
            reader.ReadByte();
            reader.ReadByte();
            int count = reader.ReadInt16();
            int num6 = count + 10;
            buffer = reader.ReadBytes(count);
            if ((biff.DataLength - num6) > 0)
            {
                buffer2 = reader.ReadBytes(biff.DataLength - num6);
            }
            if (this._linkTable.sharedFormulaList == null)
            {
                this._linkTable.sharedFormulaList = new SharedFormulaList(this._calcProperty.RefMode);
            }
            string baseFormula = null;
            ExcelFormula cellFormula = new ExcelFormula();
            if (this._isFormulaPending)
            {
                using (BinaryReader reader2 = new BinaryReader((Stream) new MemoryStream(buffer)))
                {
                    BinaryReader rExtra = null;
                    try
                    {
                        if (buffer2 != null)
                        {
                            rExtra = new BinaryReader((Stream) new MemoryStream(buffer2));
                        }
                        ExcelCalcError error = null;
                        baseFormula = new FormulaProcess(reader2, rExtra, this._formulaPendingSheet, this._formulaPendingRow, this._formulaPendingCol, this._calcProperty.RefMode == ExcelReferenceStyle.A1) { SheetsSelectionList = this._selections }.ToString(this._linkTable, ref error, new int?(rowFirst), new int?(colFirst), false);
                        if ((this._calcProperty != null) && !this._isA1ReferenceStyle)
                        {
                            cellFormula.FormulaR1C1 = baseFormula;
                        }
                        else
                        {
                            cellFormula.Formula = baseFormula;
                        }
                        this._excelReader.SetCellFormula(this._formulaPendingSheet, this._formulaPendingRow, this._formulaPendingCol, cellFormula);
                    }
                    finally
                    {
                        if (rExtra != null)
                        {
                            rExtra.Close();
                        }
                    }
                }
                this._linkTable.sharedFormulaList.AddSharedFormula(sheetIndex, rowFirst, rowLast, colFirst, colLast, buffer, buffer2, baseFormula, this._linkTable);
                this._isFormulaPending = false;
            }
        }

        private void ReadSheetProperties(BiffRecord biff, short sheetIndex)
        {
            if (this._excelReader is IExcelReader2)
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                reader.ReadBytes(12);
                uint num = reader.ReadUInt32();
                int num3 = reader.ReadByte() & 0x7f;
                if (num3 != 0x7f)
                {
                    reader.ReadBytes(3);
                    if (num == 40)
                    {
                        reader.ReadBytes(4);
                        ExcelColor color = this.ReadColor(reader);
                        (this._excelReader as IExcelReader2).SetExcelSheetTabColor(sheetIndex, color);
                    }
                    else if ((num3 >= 8) && (num3 <= 0x3f))
                    {
                        (this._excelReader as IExcelReader2).SetExcelSheetTabColor(sheetIndex, new ExcelColor((ExcelPaletteColor) num3));
                    }
                }
            }
        }

        private void ReadSST(BiffRecord biff, short sheetIndex)
        {
            this._continueRecordType = ContinueRecordType.SST;
            this._SSTBuffers.Clear();
            this._SSTBuffers.Add(biff.DataBuffer);
        }

        private void ReadString(BiffRecord biff, short sheetIndex)
        {
            if (this._isStringPending)
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                XLUnicodeString str = new XLUnicodeString();
                str.Read(reader);
                this._excelReader.SetCellValue(this._stringPendingSheet, this._stringPendingRow, this._stringPendingCol, str.Text);
                this._isStringPending = false;
            }
        }

        private void ReadStyle(BiffRecord biff, short sheetIndex)
        {
            BinaryReader reader = new BinaryReader((Stream) new MemoryStream(biff.DataBuffer));
            ushort num = reader.ReadUInt16();
            int num2 = num & 0xfff;
            IExtendedFormat format = this._cellFormats[num2].Clone();
            if ((num & 0x8000) == 0x8000)
            {
                ExcelStyle style = new ExcelStyle {
                    Format = format,
                    BuiltInStyle = (BuiltInStyleIndex) reader.ReadByte(),
                    OutLineLevel = reader.ReadByte()
                };
                if ((style.BuiltInStyle != BuiltInStyleIndex.RowLevel) && (style.BuiltInStyle != BuiltInStyleIndex.ColumnLevel))
                {
                    style.Name = style.BuiltInStyle.Name();
                }
                else
                {
                    style.Name = string.Format("{0}_{1}", (object[]) new object[] { style.BuiltInStyle.Name(), ((int) (style.OutLineLevel + 1)) });
                }
                this._styles.Add(style);
            }
            else
            {
                ExcelStyle style2 = new ExcelStyle {
                    Format = format
                };
                XLUnicodeString str = new XLUnicodeString();
                str.Read(reader);
                style2.Name = str.Text.TrimEndIfNeeded();
                this._styles.Add(style2);
            }
        }

        private void ReadStyleExt(BiffRecord biff, short sheetIndex)
        {
            IExcelStyle style = this._styles[this._styles.Count - 1];
            BinaryReader reader = new BinaryReader((Stream) new MemoryStream(biff.DataBuffer));
            reader.ReadBytes(12);
            byte num = reader.ReadByte();
            bool flag = (num & 1) == 1;
            bool flag2 = (num & 4) == 4;
            byte num2 = reader.ReadByte();
            if (flag)
            {
                ExcelStyle style2 = new ExcelStyle {
                    Category = num2,
                    Format = style.Format,
                    IsCustomBuiltIn = flag2,
                    BuiltInStyle = (BuiltInStyleIndex) reader.ReadByte(),
                    OutLineLevel = reader.ReadByte()
                };
                int num3 = reader.ReadUInt16();
                style2.Name = (string) new string(Encoding.Unicode.GetChars(reader.ReadBytes(num3 * 2)));
                this.ReadXfProps(reader, style2);
                this._styles[this._styles.Count - 1] = style2.Copy();
            }
            else
            {
                reader.ReadBytes(2);
                CustomExcelStyle style3 = new CustomExcelStyle {
                    Name = style.Name,
                    Format = style.Format
                };
                int num4 = reader.ReadUInt16();
                style3.Name = (string) new string(Encoding.Unicode.GetChars(reader.ReadBytes(num4 * 2)));
                this.ReadXfProps(reader, style3);
                this._styles[this._styles.Count - 1] = style3;
            }
        }

        private void ReadSupBook(BiffRecord biff, short sheetIndex)
        {
            ExternalWorkbookInfo info = new ExternalWorkbookInfo {
                ExternalBookBits = biff.DataBuffer
            };
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            ushort num2 = reader.ReadUInt16();
            string text = null;
            string[] strArray = new string[num];
            ExcelSupBook book = null;
            if ((num2 > 1) && (num2 <= 0xff))
            {
                XLUnicodeStringNoCch cch = new XLUnicodeStringNoCch {
                    cch = num2
                };
                cch.Read(reader);
                text = cch.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    text = cch.DecodedText;
                }
                for (int i = 0; i < num; i++)
                {
                    XLUnicodeString str2 = new XLUnicodeString();
                    str2.Read(reader);
                    strArray[i] = str2.Text;
                }
            }
            switch (num2)
            {
                case 0x401:
                    book = new ExcelSupBook(true) {
                        SheetCount = num
                    };
                    info.SheetNames = this._sheetNames;
                    book.SheetNames = this._sheetNames;
                    break;

                case 0x3a01:
                    book = new ExcelSupBook(false) {
                        IsAddInReferencedSupBook = true,
                        SheetCount = num
                    };
                    break;

                default:
                    book = new ExcelSupBook {
                        SheetCount = num
                    };
                    if (text != null)
                    {
                        book.FileName = SimpleBinaryReader.DecodeText(text);
                        info.Name = book.FileName;
                    }
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        for (int j = 0; j < strArray.Length; j++)
                        {
                            book.SheetNames.Add(strArray[j]);
                        }
                        info.SheetNames = book.SheetNames;
                    }
                    if ((book.FileName != null) && (book.SheetNames.Count > 0))
                    {
                        this._linkTable.externalReferencedWookbookInfo.Add(book.FileName, book.SheetNames);
                    }
                    break;
            }
            this._supBookList.Add(book);
            this._linkTable.AddExternalBook(book);
            this._externalWorkbookInfos.Add(info);
        }

        private IExcelFilterColumn ReadTableAutoFilter(SimpleBinaryReader reader, int columnId)
        {
            reader.ReadUInt16();
            ExcelFilterColumn filterColumn = new ExcelFilterColumn();
            this.ReadAutoFilterColumn(reader, filterColumn);
            filterColumn.AutoFilterColumnId = (uint) columnId;
            return filterColumn;
        }

        private void ReadTableStyle(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadBytes(12);
            short num = reader.ReadInt16();
            bool flag = (num & 2) == 2;
            bool flag2 = (num & 4) == 4;
            reader.ReadInt32();
            string str = reader.ReadUnicodeString(reader.ReadUInt16());
            ExcelTableStyle style = new ExcelTableStyle {
                Name = str,
                IsPivotStyle = flag,
                IsTableStyle = flag2
            };
            this._tableStyles.Add(style);
        }

        private void ReadTableStyleElement(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadBytes(12);
            TableStyleElement element = new TableStyleElement {
                Type = (ExcelTableElementType) reader.ReadInt32(),
                Size = reader.ReadInt32(),
                DifferentFormattingIndex = reader.ReadInt32()
            };
            this._tableStyles[this._tableStyles.Count - 1].TableStyleElements.Add(element);
        }

        private void ReadTableStyles(BiffRecord biff, short sheetIndex)
        {
            if (this._excelReader is IExcelTableReader)
            {
                IExcelTableReader reader = this._excelReader as IExcelTableReader;
                SimpleBinaryReader reader2 = new SimpleBinaryReader(biff.DataBuffer);
                reader2.ReadBytes(12);
                reader2.ReadUInt32();
                int charCount = reader2.ReadUInt16();
                int num2 = reader2.ReadUInt16();
                string defaultTableStyleName = reader2.ReadUnicodeString(charCount);
                string defaultPivotTableStyleName = reader2.ReadUnicodeString(num2);
                reader.SetTableDefaultStyle(defaultTableStyleName, defaultPivotTableStyleName);
            }
        }

        private void ReadTheme(XElement node)
        {
            if (node != null)
            {
                string name = node.GetAttributeValueOrDefaultOfStringType("name", null);
                XElement element = node.TryGetChildElement("themeElements");
                if (element != null)
                {
                    ColorScheme colorScheme = XlsxReader.ReadColorScheme(element.TryGetChildElement("clrScheme"));
                    FontScheme fontScheme = XlsxReader.ReadFontScheme(element.TryGetChildElement("fontScheme"));
                    ExcelTheme theme = new ExcelTheme(name, colorScheme, fontScheme);
                    this._excelReader.SetTheme(theme);
                }
            }
        }

        private void ReadTheme(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadBytes(12);
            if (reader.ReadUInt32() == 0)
            {
                try
                {
                    MemoryStream stream = new MemoryStream(reader.ReadBytes(biff.DataLength - 0x10));
                    MemoryFolder mFolder = ZipHelper.ExtractZip((Stream) stream);
                    if (mFolder != null)
                    {
                        XFile file = new XFile(string.Empty, string.Empty);
                        file.LoadPackageRelationFiles(mFolder);
                        XFile fileByType = file.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
                        this.ReadTheme(fileByType, mFolder);
                    }
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("readThemeError"), ExcelWarningCode.General, -1, -1, -1, exception);
                }
            }
        }

        private void ReadTheme(XFile workbookFile, MemoryFolder mFolder)
        {
            XFile fileByType = workbookFile.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
            this.ReadTheme(XDocument.Load(mFolder.GetFile(fileByType.FileName)).Root);
        }

        private void ReadTopMargin(BiffRecord biff, short sheetIndex)
        {
            double num = BitConverter.ToDouble(biff.DataBuffer, 0);
            this._printPageMargin.Top = num;
        }

        private void ReadTXO(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadUInt16();
            reader.ReadUInt16();
            reader.Seek(6, (SeekOrigin) SeekOrigin.Current);
            ushort num2 = reader.ReadUInt16();
            if (num2 > 0)
            {
                this._txoTextLength = num2;
                this._continueRecordType = ContinueRecordType.TXO1;
            }
            else if (this._commentObjCount > 0)
            {
                this.GetCommentObjId();
                this._commentObjCount--;
            }
        }

        public void ReadUnsupportRecord(BiffRecord biff, short sheet)
        {
            if (this._excelReader is IExcelLosslessReader)
            {
                UnsupportRecord unsupportRecord = new UnsupportRecord {
                    FileType = ExcelFileType.XLS,
                    Category = RecordCategory.Drawing,
                    Value = biff
                };
                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, unsupportRecord);
            }
            this.LogUnsupportedBiffRecord(sheet, biff);
        }

        private void ReadVCenter(BiffRecord biff, short sheetIndex)
        {
            bool flag = BitConverter.ToInt16(biff.DataBuffer, 0) == 1;
            this._printOptions.VerticalCentered = flag;
        }

        private void ReadVerticalPageBreaks(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            ushort num = reader.ReadUInt16();
            List<int> list = new List<int>();
            for (int i = 0; i < num; i++)
            {
                list.Add(reader.ReadUInt16());
                reader.ReadInt32();
            }
            this._printPageSetting.ColumnBreakLines = list;
        }

        private void ReadWindow1(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            short num = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            short num3 = reader.ReadInt16();
            short num4 = reader.ReadInt16();
            ExcelRect rect = new ExcelRect((double) num, (double) num2, (double) num3, (double) num4);
            ushort num5 = reader.ReadUInt16();
            ushort selectedTabIndex = reader.ReadUInt16();
            ushort firstDisplayedTabIndex = reader.ReadUInt16();
            ushort selectedTabCount = reader.ReadUInt16();
            ushort tabRatio = reader.ReadUInt16();
            bool hidden = (num5 & 1) == 1;
            bool iconic = (num5 & 2) == 2;
            bool showHorizontalScrollbarAsNeeded = (num5 & 8) == 8;
            bool showVerticalScrollBarAsNeeded = (num5 & 0x10) == 0x10;
            bool showTabs = (num5 & 0x20) == 0x20;
            this._activeSheetIndex = selectedTabIndex;
            this._excelReader.SetWindow(rect, hidden, iconic);
            if (this._readAllSheet)
            {
                this._excelReader.SetTabs(showTabs, selectedTabIndex, firstDisplayedTabIndex, selectedTabCount, tabRatio);
            }
            else
            {
                this._excelReader.SetTabs(showTabs, 0, 0, 1, tabRatio);
            }
            this._excelReader.SetScroll(showHorizontalScrollbarAsNeeded, showVerticalScrollBarAsNeeded);
        }

        private void ReadWindow2(BiffRecord biff, short sheetIndex)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            int num = reader.ReadInt16();
            int topRow = reader.ReadUInt16();
            int leftColumn = reader.ReadUInt16();
            int num4 = reader.ReadInt32();
            bool showFormula = (num & 1) == 1;
            bool showGridLine = (num & 2) == 2;
            bool showRowColumnHeader = (num & 4) == 4;
            bool flag4 = (num & 8) == 8;
            bool showZeros = (num & 0x10) == 0x10;
            bool flag6 = (num & 0x20) == 0x20;
            bool rightToLeftColumns = (num & 0x40) == 0x40;
            if (!flag6)
            {
                this._excelReader.SetGridlineColor(sheetIndex, new ExcelColor(ExcelColorType.Indexed, (uint) num4, 0.0));
            }
            this._excelReader.SetTopLeft(sheetIndex, topRow, leftColumn);
            this._excelReader.SetDisplayElements(sheetIndex, showFormula, showZeros, showGridLine, showRowColumnHeader, rightToLeftColumns);
            this._isPanesFrozen = flag4;
        }

        private void ReadWSBool(BiffRecord biff, short sheetIndex)
        {
            short num = new SimpleBinaryReader(biff.DataBuffer).ReadInt16();
            this._isPrintFitToPage = (num & 0x100) == 0x100;
            bool summaryRowsBelowDetail = (num & 0x40) == 0x40;
            bool summaryColumnsRightToDetail = (num & 0x80) == 0x80;
            this._excelReader.SetOutlineDirection(sheetIndex, summaryColumnsRightToDetail, summaryRowsBelowDetail);
        }

        private void ReadXF(BiffRecord biff, short sheetIndex)
        {
            XFRecrod xf = new XFRecrod();
            xf.Read(new SimpleBinaryReader(biff.DataBuffer));
            this._xfBuffers.Add(biff.DataBuffer);
            this._xfs.Add(xf);
            ExcelFont font = new ExcelFont();
            if (this._fonts.ContainsKey(xf.FontIndex))
            {
                font = this._fonts[xf.FontIndex];
            }
            else
            {
                font = font.Default;
            }
            ExtendedFormat excelStyle = ConverterFactory.GetExcelStyle(xf, font);
            if (this._numberFormats.ContainsKey(xf.FormatIndex))
            {
                string code = this._numberFormats[xf.FormatIndex];
                excelStyle.NumberFormat = new ExcelNumberFormat(xf.FormatIndex, code);
            }
            else
            {
                excelStyle.NumberFormatIndex = xf.FormatIndex;
            }
            this._cellFormats.Add(excelStyle);
        }

        private void ReadXFCRC(BiffRecord biff, short sheet)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
            reader.ReadBytes(14);
            if (reader.ReadUInt16() != this._xfBuffers.Count)
            {
                this._readXFExtension = false;
            }
            MsoCrc32 crc = new MsoCrc32();
            uint crcValue = 0;
            foreach (byte[] buffer in this._xfBuffers)
            {
                crcValue = crc.CRC(crcValue, buffer);
            }
            if (crcValue != reader.ReadUInt32())
            {
                this._readXFExtension = false;
            }
        }

        private void ReadXFExt(BiffRecord biff, short sheet)
        {
            if (this._readXFExtension)
            {
                SimpleBinaryReader reader = new SimpleBinaryReader(biff.DataBuffer);
                reader.ReadBytes(12);
                reader.ReadBytes(2);
                int num = reader.ReadUInt16();
                reader.ReadBytes(2);
                ExtendedFormat format = this._cellFormats[num];
                int num2 = reader.ReadUInt16();
                for (int i = 0; i < num2; i++)
                {
                    byte num5;
                    short num4 = reader.ReadInt16();
                    reader.ReadUInt16();
                    switch (num4)
                    {
                        case 4:
                        {
                            format.PatternColor = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 5:
                        {
                            format.PatternBackgroundColor = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 6:
                        case 11:
                        case 12:
                        {
                            continue;
                        }
                        case 7:
                        {
                            format.Border.Top.Color = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 8:
                        {
                            format.Border.Bottom.Color = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 9:
                        {
                            format.Border.Left.Color = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 10:
                        {
                            format.Border.Right.Color = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 13:
                        {
                            format.Font.FontColor = this.ReadFullColorExt(reader);
                            continue;
                        }
                        case 14:
                        {
                            num5 = reader.ReadByte();
                            if (num5 != 1)
                            {
                                break;
                            }
                            format.Font.FontScheme = FontSchemeCategory.Major;
                            continue;
                        }
                        case 15:
                        {
                            format.Indent = (byte) reader.ReadUInt16();
                            continue;
                        }
                        default:
                        {
                            continue;
                        }
                    }
                    if (num5 == 2)
                    {
                        format.Font.FontScheme = FontSchemeCategory.Minor;
                    }
                }
            }
        }

        private void ReadXFExtNoFRT(SimpleBinaryReader reader, DifferentialFormatting dxfExt)
        {
            reader.ReadInt16();
            reader.ReadInt16();
            reader.ReadInt16();
            ushort num = reader.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                ushort num3 = reader.ReadUInt16();
                reader.ReadUInt16();
                switch (num3)
                {
                    case 4:
                        dxfExt.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(dxfExt.Fill.Item1, this.ReadFullColorExt(reader), dxfExt.Fill.Item3);
                        break;

                    case 5:
                        dxfExt.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(dxfExt.Fill.Item1, dxfExt.Fill.Item2, this.ReadFullColorExt(reader));
                        break;

                    case 7:
                        if (dxfExt.Border == null)
                        {
                            dxfExt.Border = new ExcelBorder();
                        }
                        dxfExt.Border.Top.Color = this.ReadFullColorExt(reader);
                        break;

                    case 8:
                        if (dxfExt.Border == null)
                        {
                            dxfExt.Border = new ExcelBorder();
                        }
                        dxfExt.Border.Bottom.Color = this.ReadFullColorExt(reader);
                        break;

                    case 9:
                        if (dxfExt.Border == null)
                        {
                            dxfExt.Border = new ExcelBorder();
                        }
                        dxfExt.Border.Left.Color = this.ReadFullColorExt(reader);
                        break;

                    case 10:
                        if (dxfExt.Border == null)
                        {
                            dxfExt.Border = new ExcelBorder();
                        }
                        dxfExt.Border.Right.Color = this.ReadFullColorExt(reader);
                        break;

                    case 13:
                        if (dxfExt.Font == null)
                        {
                            dxfExt.Font = new ExcelFont();
                        }
                        dxfExt.Font.FontColor = this.ReadFullColorExt(reader);
                        break;

                    case 14:
                        if (dxfExt.Font == null)
                        {
                            dxfExt.Font = new ExcelFont();
                        }
                        if (reader.Remaining < 2)
                        {
                            dxfExt.Font.FontScheme = FontSchemeCategory.None;
                        }
                        else
                        {
                            dxfExt.Font.FontScheme = (FontSchemeCategory) reader.ReadUInt16();
                        }
                        break;

                    case 15:
                        if (dxfExt.Alignment == null)
                        {
                            dxfExt.Alignment = new AlignmentBlock();
                        }
                        dxfExt.Alignment.IndentationLevel = (byte) reader.ReadUInt16();
                        break;
                }
            }
        }

        private ExcelBorderSide ReadXFPropBorder(BinaryReader reader)
        {
            return new ExcelBorderSide { Color = this.ReadXFPropColor(reader), LineStyle = (ExcelBorderStyle)(byte)reader.ReadUInt16() };
        }

        private ExcelColor ReadXFPropColor(BinaryReader reader)
        {
            switch (((reader.ReadByte() & 14) >> 1))
            {
                case 0:
                {
                    reader.ReadByte();
                    double num1 = ((double) reader.ReadInt16()) / 32767.0;
                    reader.ReadBytes(4);
                    return new ExcelColor(new GcColor()) { IsAutoColor = true };
                }
                case 1:
                {
                    uint color = reader.ReadByte();
                    double tint = ((double) reader.ReadInt16()) / 32767.0;
                    reader.ReadBytes(4);
                    return new ExcelColor(ExcelColorType.Indexed, color, tint);
                }
                case 2:
                {
                    reader.ReadByte();
                    double num5 = ((double) reader.ReadInt16()) / 32767.0;
                    byte red = reader.ReadByte();
                    byte green = reader.ReadByte();
                    byte blue = reader.ReadByte();
                    byte alpha = reader.ReadByte();
                    return new ExcelColor(ExcelColorType.RGB, GcColor.FromArgb(alpha, red, green, blue).ToArgb(), num5);
                }
                case 3:
                {
                    uint num10 = reader.ReadByte();
                    double num11 = ((double) reader.ReadInt16()) / 32767.0;
                    reader.ReadBytes(4);
                    return new ExcelColor(ExcelColorType.Theme, num10, num11);
                }
                case 4:
                    return null;
            }
            return null;
        }

        private void ReadXfProps(BinaryReader reader, IDifferentialFormatting differentFormatting)
        {
            reader.ReadBytes(2);
            int num = reader.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                ushort num3 = reader.ReadUInt16();
                reader.ReadUInt16();
                switch (num3)
                {
                    case 0:
                        if (differentFormatting.Fill == null)
                        {
                            differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.None, null, null);
                        }
                        differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>((FillPatternType) reader.ReadByte(), differentFormatting.Fill.Item2, differentFormatting.Fill.Item3);
                        break;

                    case 1:
                        if (differentFormatting.Fill == null)
                        {
                            differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.None, null, null);
                        }
                        differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(differentFormatting.Fill.Item1, this.ReadXFPropColor(reader), differentFormatting.Fill.Item3);
                        break;

                    case 2:
                        if (differentFormatting.Fill == null)
                        {
                            differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.None, null, null);
                        }
                        differentFormatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(differentFormatting.Fill.Item1, differentFormatting.Fill.Item2, this.ReadXFPropColor(reader));
                        break;

                    case 5:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.FontColor = this.ReadXFPropColor(reader);
                        break;

                    case 6:
                        if (differentFormatting.Border == null)
                        {
                            differentFormatting.Border = new ExcelBorder();
                        }
                        differentFormatting.Border.Top = this.ReadXFPropBorder(reader);
                        break;

                    case 7:
                        if (differentFormatting.Border == null)
                        {
                            differentFormatting.Border = new ExcelBorder();
                        }
                        differentFormatting.Border.Bottom = this.ReadXFPropBorder(reader);
                        break;

                    case 8:
                        if (differentFormatting.Border == null)
                        {
                            differentFormatting.Border = new ExcelBorder();
                        }
                        differentFormatting.Border.Left = this.ReadXFPropBorder(reader);
                        break;

                    case 9:
                        if (differentFormatting.Border == null)
                        {
                            differentFormatting.Border = new ExcelBorder();
                        }
                        differentFormatting.Border.Right = this.ReadXFPropBorder(reader);
                        break;

                    case 15:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.HorizontalAlignment = (ExcelHorizontalAlignment) reader.ReadByte();
                        break;

                    case 0x10:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.VerticalAlignment = (ExcelVerticalAlignment) reader.ReadByte();
                        break;

                    case 0x11:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.TextRotation = reader.ReadByte();
                        break;

                    case 0x12:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.IndentationLevel = (byte) reader.ReadUInt16();
                        break;

                    case 0x13:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.TextDirection = (TextDirection) reader.ReadByte();
                        break;

                    case 20:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.IsTextWrapped = reader.ReadByte() == 1;
                        break;

                    case 0x15:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.IsJustifyLastLine = reader.ReadByte() == 1;
                        break;

                    case 0x16:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Alignment.IsShrinkToFit = reader.ReadByte() == 1;
                        break;

                    case 0x18:
                        if (differentFormatting.Alignment == null)
                        {
                            differentFormatting.Alignment = new AlignmentBlock();
                        }
                        differentFormatting.Font.FontName = (string) new string(Encoding.Unicode.GetChars(reader.ReadBytes(reader.ReadUInt16() * 2)));
                        break;

                    case 0x19:
                        if (reader.ReadUInt16() == 700)
                        {
                            differentFormatting.Font.IsBold = true;
                        }
                        break;

                    case 0x1a:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.UnderLineStyle = (UnderLineStyle) reader.ReadByte();
                        break;

                    case 0x1b:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.VerticalAlignRun = (VerticalAlignRun) reader.ReadByte();
                        break;

                    case 0x1c:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.IsItalic = reader.ReadByte() == 1;
                        break;

                    case 0x1d:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.IsStrikeOut = reader.ReadByte() == 1;
                        break;

                    case 30:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.IsOutlineStyle = reader.ReadByte() == 1;
                        break;

                    case 0x1f:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.IsShadowStyle = reader.ReadByte() == 1;
                        break;

                    case 0x22:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.CharSetIndex = reader.ReadByte();
                        break;

                    case 0x23:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.FontFamily = (ExcelFontFamily) reader.ReadByte();
                        break;

                    case 0x24:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        differentFormatting.Font.FontSize = reader.ReadUInt32();
                        break;

                    case 0x25:
                        if (differentFormatting.Font == null)
                        {
                            differentFormatting.Font = new ExcelFont();
                        }
                        switch (reader.ReadByte())
                        {
                            case 1:
                            {
                                differentFormatting.Font.FontScheme = FontSchemeCategory.Major;
                                continue;
                            }
                            case 2:
                            {
                                differentFormatting.Font.FontScheme = FontSchemeCategory.Minor;
                                continue;
                            }
                        }
                        break;

                    case 0x29:
                    {
                        ushort num5 = reader.ReadUInt16();
                        differentFormatting.FormatId = num5;
                        break;
                    }
                    case 0x2b:
                        differentFormatting.IsLocked = (reader.ReadByte() & 1) == 1;
                        break;

                    case 0x2c:
                        differentFormatting.IsHidden = (reader.ReadByte() & 1) == 1;
                        break;
                }
            }
        }

        private void ReadXfProps(BinaryReader reader, IExcelStyle style)
        {
            reader.ReadBytes(2);
            int num = reader.ReadUInt16();
            for (int i = 0; i < num; i++)
            {
                byte num4;
                ushort num3 = reader.ReadUInt16();
                reader.ReadUInt16();
                switch (num3)
                {
                    case 0:
                    {
                        style.Format.FillPattern = (FillPatternType) reader.ReadByte();
                        continue;
                    }
                    case 1:
                    {
                        style.Format.PatternColor = this.ReadXFPropColor(reader);
                        continue;
                    }
                    case 2:
                    {
                        style.Format.PatternBackgroundColor = this.ReadXFPropColor(reader);
                        continue;
                    }
                    case 3:
                    case 4:
                    case 10:
                    case 11:
                    case 12:
                    case 13:
                    case 14:
                    case 0x17:
                    case 0x20:
                    case 0x21:
                    case 0x26:
                    case 0x27:
                    case 40:
                    case 0x2a:
                    {
                        continue;
                    }
                    case 5:
                    {
                        style.Format.Font.FontColor = this.ReadXFPropColor(reader);
                        continue;
                    }
                    case 6:
                    {
                        style.Format.Border.Top = this.ReadXFPropBorder(reader);
                        continue;
                    }
                    case 7:
                    {
                        style.Format.Border.Bottom = this.ReadXFPropBorder(reader);
                        continue;
                    }
                    case 8:
                    {
                        style.Format.Border.Left = this.ReadXFPropBorder(reader);
                        continue;
                    }
                    case 9:
                    {
                        style.Format.Border.Right = this.ReadXFPropBorder(reader);
                        continue;
                    }
                    case 15:
                    {
                        style.Format.HorizontalAlign = (ExcelHorizontalAlignment) reader.ReadByte();
                        continue;
                    }
                    case 0x10:
                    {
                        style.Format.VerticalAlign = (ExcelVerticalAlignment) reader.ReadByte();
                        continue;
                    }
                    case 0x11:
                    {
                        style.Format.Rotation = reader.ReadByte();
                        continue;
                    }
                    case 0x12:
                    {
                        style.Format.Indent = (byte) reader.ReadUInt16();
                        continue;
                    }
                    case 0x13:
                    {
                        style.Format.ReadingOrder = (TextDirection) reader.ReadByte();
                        continue;
                    }
                    case 20:
                    {
                        style.Format.IsWordWrap = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x15:
                    {
                        style.Format.IsJustfyLastLine = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x16:
                    {
                        style.Format.IsShrinkToFit = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x18:
                    {
                        style.Format.Font.FontName = (string) new string(Encoding.Unicode.GetChars(reader.ReadBytes(reader.ReadUInt16() * 2)));
                        continue;
                    }
                    case 0x19:
                    {
                        if (reader.ReadUInt16() == 700)
                        {
                            style.Format.Font.IsBold = true;
                        }
                        continue;
                    }
                    case 0x1a:
                    {
                        style.Format.Font.UnderLineStyle = (UnderLineStyle) reader.ReadByte();
                        continue;
                    }
                    case 0x1b:
                    {
                        style.Format.Font.VerticalAlignRun = (VerticalAlignRun) reader.ReadByte();
                        continue;
                    }
                    case 0x1c:
                    {
                        style.Format.Font.IsItalic = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x1d:
                    {
                        style.Format.Font.IsStrikeOut = reader.ReadByte() == 1;
                        continue;
                    }
                    case 30:
                    {
                        style.Format.Font.IsOutlineStyle = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x1f:
                    {
                        style.Format.Font.IsShadowStyle = reader.ReadByte() == 1;
                        continue;
                    }
                    case 0x22:
                    {
                        style.Format.Font.CharSetIndex = reader.ReadByte();
                        continue;
                    }
                    case 0x23:
                    {
                        style.Format.Font.FontFamily = (ExcelFontFamily) reader.ReadByte();
                        continue;
                    }
                    case 0x24:
                    {
                        style.Format.Font.FontSize = reader.ReadUInt32();
                        continue;
                    }
                    case 0x25:
                    {
                        num4 = reader.ReadByte();
                        if (num4 != 1)
                        {
                            break;
                        }
                        style.Format.Font.FontScheme = FontSchemeCategory.Major;
                        continue;
                    }
                    case 0x29:
                    {
                        ushort num5 = reader.ReadUInt16();
                        style.Format.NumberFormatIndex = num5;
                        continue;
                    }
                    case 0x2b:
                    {
                        style.Format.IsLocked = (reader.ReadByte() & 1) == 1;
                        continue;
                    }
                    case 0x2c:
                    {
                        style.Format.IsHidden = (reader.ReadByte() & 1) == 1;
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                if (num4 == 2)
                {
                    style.Format.Font.FontScheme = FontSchemeCategory.Minor;
                }
            }
        }

        private void ResetWorksheetGlobalVariables()
        {
            this._printPageMargin = new ExcelPrintPageMargin();
            this._printOptions = new ExcelPrintOptions();
            this._printPageSetting = new ExcelPrintPageSetting();
            this._conditonalFormat.Clear();
            this._conditionaFormat12.Clear();
            this._priviousConditonalFormat = null;
        }

        private string UpdateDVformulaIfNeed(string formula)
        {
            if ((!string.IsNullOrWhiteSpace(formula) && formula.StartsWith("\"")) && formula.EndsWith("\""))
            {
                return formula.Replace('\0', ',');
            }
            return formula;
        }

        public string Password { get; set; }

        internal enum EncryptionAlgorithm
        {
            None,
            RC4,
            AES128,
            AES192,
            AES256
        }

        internal enum EncryptionProviderType
        {
            Any,
            RC4,
            AES
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct rkrec
        {
            public int ixfe;
            public int RK;
        }

        /// <summary>
        /// The SharedFeatureType enumeration specifies the different types of shared features.
        /// </summary>
        internal enum SharedFeatureType
        {
            /// <summary>
            /// Specifies the smart tag type. A Shared Feature of this type is used to
            /// recognize certain types of entries (for example, proper names,
            /// dates/times, financial symbols) and flag them for action.
            /// </summary>
            ISFFACTOID = 4,
            /// <summary>
            /// Specifies the ignored formula errors type. A Shared Feature of this type is
            /// used to specify the formula errors to be ignored.
            /// </summary>
            ISFFEC2 = 3,
            /// <summary>
            /// Specifies the list type. A Shared Feature of this type is used to describe a
            /// table within a sheet
            /// </summary>
            ISFLIST = 5,
            /// <summary>
            /// Specifies the enhanced protection type. A Shared Feature of this type is 
            /// used to protect a shared workbook by restricting access to the areas of
            /// the workbook and to the available functionality.
            /// </summary>
            ISFPROTECTION = 2
        }
    }
}

