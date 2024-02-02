#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class BiffRecordWriter
    {
        private List<IExtendedFormat> _allformats = new List<IExtendedFormat>();
        private ICalculationProperty _calulationProperty;
        private List<IExtendedFormat> _cellFormats = new List<IExtendedFormat>();
        private List<IExcelCell> _cellsHasHyperlink = new List<IExcelCell>();
        private List<IDifferentialFormatting> _dxfs;
        private IExcelWriter _excelWriter;
        private List<ExtendedFormat> _extendedFormats = new List<ExtendedFormat>();
        private List<IExtendedFormat> _formats;
        private LinkTable _linkTable = new LinkTable();
        private readonly List<int> _mergedToSheetIndexList = new List<int>();
        private Dictionary<int, string> _numberFormats = new Dictionary<int, string>();
        private int _offSet;
        private short _sheetCount;
        private List<string> _sheetNames;
        private List<byte[]> _sstStringList;
        private List<IExtendedFormat> _styleFormats = new List<IExtendedFormat>();
        private Dictionary<int, short> _StyleIDCache = new Dictionary<int, short>();
        private Dictionary<string, int> _uniqueStringTable;
        private List<XFRecrod> _xfs = new List<XFRecrod>();
        private int nextTableId;
        private const int RANGEGROUP_GROUPSQUARELENGTH = 12;
        private const int RANGEGROUP_GROUPTITLEBARMARGIN = 5;
        private Dictionary<int, List<IUnsupportRecord>> unsupportRecords;

        public BiffRecordWriter(IExcelWriter writer, IMeasureString measure)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this._excelWriter = writer;
            this._calulationProperty = this._excelWriter.GetCalculationProperty();
            this.Reset();
        }

        private void AddFontX(List<IExcelFont> fonts)
        {
            IExcelLosslessWriter writer = this._excelWriter as IExcelLosslessWriter;
            int sheetCount = this._excelWriter.GetSheetCount();
            List<FontX> list = new List<FontX>();
            for (int i = 0; i < sheetCount; i++)
            {
                List<IUnsupportRecord> unsupportItems = writer.GetUnsupportItems(i);
                if (unsupportItems != null)
                {
                    foreach (IUnsupportRecord record in unsupportItems)
                    {
                        if ((record.Category == RecordCategory.FontX) && (record.Value is FontX))
                        {
                            list.Add(record.Value as FontX);
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                foreach (FontX tx in list)
                {
                    int index = fonts.IndexOf(tx.Font);
                    if (index >= 4)
                    {
                        index++;
                    }
                    else if (index == -1)
                    {
                        fonts.Add(tx.Font);
                        index = (fonts.Count > 4) ? fonts.Count : (fonts.Count - 1);
                    }
                    tx.ifnt = (ushort) index;
                }
            }
        }

        private void AddTXOFonts(List<IExcelFont> fonts)
        {
            IExcelLosslessWriter writer = this._excelWriter as IExcelLosslessWriter;
            int sheetCount = this._excelWriter.GetSheetCount();
            List<TXORuns> list = new List<TXORuns>();
            for (int i = 0; i < sheetCount; i++)
            {
                List<IUnsupportRecord> unsupportItems = writer.GetUnsupportItems(i);
                if (unsupportItems != null)
                {
                    foreach (IUnsupportRecord record in unsupportItems)
                    {
                        if ((record.Category == RecordCategory.TextRun) && (record.Value is TXORuns))
                        {
                            list.Add(record.Value as TXORuns);
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                foreach (TXORuns runs in list)
                {
                    if (runs.rgTXORuns != null)
                    {
                        foreach (Run run in runs.rgTXORuns)
                        {
                            int index = fonts.IndexOf(run.Font);
                            if (index >= 4)
                            {
                                index++;
                            }
                            else if (index == -1)
                            {
                                fonts.Add(run.Font);
                                index = (fonts.Count > 4) ? fonts.Count : (fonts.Count - 1);
                            }
                            run.ifnt = (ushort) index;
                        }
                    }
                }
            }
        }

        internal int AddUniqueString(string str)
        {
            int num = -1;
            if (!this._uniqueStringTable.TryGetValue(str, out num) || (num < 0))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    this.WriteBiffStr(new BinaryWriter((Stream) stream), str, false, false, false, false, 2);
                    this._sstStringList.Add(stream.ToArray());
                    this._uniqueStringTable[str] = num = this._sstStringList.Count - 1;
                }
            }
            return num;
        }

        private string AppendA1Letter(int coord)
        {
            coord = Math.Min(coord, 0x100);
            return IndexHelper.GetColumnIndexInA1Letter(coord);
        }

        internal bool BuildBiffStrComponents(string stringIn, StringConvert stringConvert, ref short charCount, ref byte grbit, ref byte[] biffStrBuffer)
        {
            bool flag = false;
            if (stringIn != null)
            {
                int index = stringIn.IndexOf('\0');
                if ((index != -1) && (index < stringIn.Length))
                {
                    stringIn = stringIn.Substring(0, index);
                }
            }
            if ((stringIn != null) && (0x7fff < stringIn.Length))
            {
                stringIn = stringIn.Substring(0, 0x7fff);
            }
            if (stringIn != null)
            {
                charCount = (short) stringIn.Length;
            }
            else
            {
                charCount = 0;
            }
            if (stringConvert == StringConvert.Unicode)
            {
                flag = true;
            }
            else if (stringConvert == StringConvert.Ascii)
            {
                flag = false;
            }
            else
            {
                for (int i = 0; i < charCount; i++)
                {
                    char ch = stringIn[i];
                    if ((ch < '\0') || (ch > '\x007f'))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            grbit = flag ? ((byte) 1) : ((byte) 0);
            if ((stringIn != null) && (charCount > 0))
            {
                string[] strArray = stringIn.Split(new char[] { '\r' });
                StringBuilder builder = new StringBuilder();
                foreach (string str in strArray)
                {
                    builder.Append(str);
                }
                stringIn = builder.ToString();
                if (flag)
                {
                    biffStrBuffer = Encoding.Unicode.GetBytes(stringIn);
                }
                else
                {
                    biffStrBuffer = EncodingHelper.GetASCIIBytes(stringIn);
                }
                if (flag)
                {
                    charCount = (short)(biffStrBuffer.Length / 2);
                }
                else
                {
                    charCount = (short)biffStrBuffer.Length;
                }
            }
            else
            {
                charCount = 0;
            }
            return true;
        }

        private void BuildSheetNames(MemoryStream[] sheetNameStreamList, StringBuilder[] sheetNameBuilders, List<bool> hiddenList, int sheetCount)
        {
            HashSet<string> set = new HashSet<string>();
            for (int i = 0; i < sheetCount; i++)
            {
                string sheetName = this._excelWriter.GetSheetName(i);
                if ((sheetName != null) && (sheetName.Length > 0))
                {
                    set.Add(sheetName);
                }
            }
            for (int j = 0; j < sheetCount; j++)
            {
                string str2 = null;
                StringBuilder builder = null;
                int num3 = -1;
                str2 = this._excelWriter.GetSheetName(j);
                bool flag = this._excelWriter.IsSheetHidden(j);
                hiddenList.Add(flag);
                if ((str2 == null) || (str2.Length == 0))
                {
                    int num4 = j;
                    while (true)
                    {
                        int num8 = num4 + 1;
                        str2 = "Sheet" + ((int) num8).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                        if (!set.Contains(str2))
                        {
                            set.Add(str2);
                            break;
                        }
                        num4++;
                    }
                    sheetNameBuilders[j] = new StringBuilder(str2);
                    continue;
                }
                builder = new StringBuilder(str2);
                while ((num3 = str2.IndexOfAny(@"?:/\*[]".ToCharArray(), num3 + 1)) != -1)
                {
                    builder[num3] = '_';
                }
                num3 = -1;
                while ((num3 = str2.IndexOfAny("'".ToCharArray(), num3 + 1)) != -1)
                {
                    if ((num3 == 0) || (num3 == (str2.Length - 1)))
                    {
                        builder[num3] = '_';
                    }
                }
                sheetNameBuilders[j] = builder;
            }
            for (int k = 1; k < sheetCount; k++)
            {
                if (sheetNameBuilders[k] == null)
                {
                    return;
                }
                string str3 = sheetNameBuilders[k].ToString();
                string str4 = str3;
                bool flag2 = true;
                int num6 = 0;
                while (flag2)
                {
                    flag2 = false;
                    for (int m = 0; m < k; m++)
                    {
                        if ((m != k) && str4.Equals(sheetNameBuilders[m].ToString()))
                        {
                            num6++;
                            str4 = string.Concat((object[]) new object[] { str3, " (", ((int) num6), ")" });
                            flag2 = true;
                            break;
                        }
                    }
                    if (!flag2 && (num6 > 0))
                    {
                        sheetNameBuilders[k].Append(" (").Append(num6).Append(")");
                    }
                }
            }
        }

        /// <summary>
        /// BuildStream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public void BuildStream(Stream stream)
        {
            try
            {
                this._sheetCount = (short) this._excelWriter.GetSheetCount();
                MemoryStream[] streams = new MemoryStream[this._sheetCount];
                BinaryWriter writer = new BinaryWriter(stream);
                this.WriteBeginWorkbookRecords(writer);
                this.WriteExcelFormatRecord(writer);
                this.WriteDxfs(writer);
                this.WriteStyleRecord(writer);
                this.WriterTableStyles(writer);
                this.WritePALETTE(writer);
                this.WriteWorkbookRecords(writer, streams, (int) writer.BaseStream.Length);
                this.Reset();
                this._excelWriter.Finish();
            }
            catch (Exception exception)
            {
                this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeFileError"), ExcelWarningCode.General, -1, -1, -1, exception));
            }
        }

        private string ConvertA1FormulaToR1C1Formula(string formula, int row, int column)
        {
            return Parser.Unparse(Parser.Parse(formula, row, column, false, this._linkTable), row, column, true);
        }

        private string ConvertDVFormulaBackIfNeeded(string formula)
        {
            if ((!string.IsNullOrWhiteSpace(formula) && formula.StartsWith("\"")) && formula.EndsWith("\""))
            {
                return formula.Replace(',', '\0');
            }
            return formula;
        }

        private string ConvertR1C1FormulaToA1Formula(string formula, int row, int column)
        {
            return Parser.Unparse(Parser.Parse(formula, row, column, true, this._linkTable), row, column, false);
        }

        public ConditionalFormatExtension CreateConditionalFormatExtension(IDifferentialFormatting dxf, IExcelConditionalFormatRule rule)
        {
            bool isDFXExten = dxf.IsDFXExten;
            IDifferentialFormatting dxfExt = dxf.Clone();
            if (dxfExt.Border != null)
            {
                if (dxfExt.Border.Left != null)
                {
                    this.UpdateDXFExtendedProperty("leftBorder", dxfExt.Border.Left.Color, dxfExt);
                }
                if (dxfExt.Border.Top != null)
                {
                    this.UpdateDXFExtendedProperty("topBorder", dxfExt.Border.Top.Color, dxfExt);
                }
                if (dxfExt.Border.Bottom != null)
                {
                    this.UpdateDXFExtendedProperty("bottomBorder", dxfExt.Border.Bottom.Color, dxfExt);
                }
                if (dxfExt.Border.Right != null)
                {
                    this.UpdateDXFExtendedProperty("rightBorder", dxfExt.Border.Right.Color, dxfExt);
                }
            }
            if (dxfExt.Font != null)
            {
                this.UpdateDXFExtendedProperty("fontColor", dxfExt.Font.FontColor, dxfExt);
                if (dxfExt.Font.FontScheme != FontSchemeCategory.None)
                {
                    dxfExt.IsDFXExten = true;
                    dxfExt.ExtendedPropertyList.Add(new Tuple<string, object>("fontSheme", dxfExt.Font.FontScheme));
                }
            }
            if (dxfExt.Fill != null)
            {
                this.UpdateDXFExtendedProperty("foreGround", dxfExt.Fill.Item2, dxfExt);
                this.UpdateDXFExtendedProperty("backGround", dxfExt.Fill.Item3, dxfExt);
            }
            if ((dxfExt.Alignment != null) && (dxf.Alignment.IndentationLevel > 7))
            {
                dxfExt.IsDFXExten = true;
                dxfExt.ExtendedPropertyList.Add(new Tuple<string, object>("indentationLevel", (byte) dxfExt.Alignment.IndentationLevel));
            }
            isDFXExten = dxfExt.IsDFXExten;
            int templementIndex = this.GetTemplementIndex(rule);
            if (!isDFXExten && ((templementIndex != -1) || !rule.StopIfTrue))
            {
                isDFXExten = true;
                dxfExt.IsDFXExten = true;
            }
            if (!isDFXExten)
            {
                return null;
            }
            ConditionalFormatExtension extension = new ConditionalFormatExtension();
            if (templementIndex != -1)
            {
                extension.TemplateIndex = (byte) templementIndex;
            }
            else
            {
                extension.TemplateIndex = (byte) rule.Type;
            }
            extension.stopIfTrue = rule.StopIfTrue;
            extension.Priority = (ushort) rule.Priority;
            extension.HasDXF = true;
            extension.DXF = dxfExt;
            return extension;
        }

        private CellType FigureCellType(object value)
        {
            if (value != null)
            {
                bool flag;
                double num;
                DateTime time;
                string str = value.ToString();
                if (bool.TryParse(str, out flag))
                {
                    return CellType.Boolean;
                }
                if (double.TryParse(str, out num))
                {
                    return CellType.Numeric;
                }
                if (DateTime.TryParse(str, out time))
                {
                    return CellType.Datetime;
                }
                if (value is ExcelCalcError)
                {
                    return CellType.Error;
                }
                if (value is string)
                {
                    return CellType.String;
                }
            }
            return CellType.Unknown;
        }

        private short FixWorksheetIndex(short sheetIndex)
        {
            if (((this._mergedToSheetIndexList != null) && (this._mergedToSheetIndexList.Count != 0)) && (sheetIndex >= 0))
            {
                for (int i = 0; i < this._mergedToSheetIndexList.Count; i++)
                {
                    if (sheetIndex >= this._mergedToSheetIndexList[i])
                    {
                        sheetIndex++;
                    }
                }
            }
            return sheetIndex;
        }

        private static SupBookBiff8Structure GetAddInReferencingSupBook()
        {
            return new SupBookBiff8Structure { ctab = 1, cch = 0x3a01 };
        }

        private byte[] GetAutoFilterCustomBuffer(IExcelFilterColumn item)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter((Stream) stream))
                {
                    writer.Write((short) ((short) item.AutoFilterColumnId));
                    writer.Write((short) 1);
                    byte[] buffer = null;
                    buffer = this.WriteDOper(writer, null);
                    byte[] buffer3 = null;
                    buffer3 = this.WriteDOper(writer, null);
                    if (buffer != null)
                    {
                        writer.Write(buffer);
                    }
                    if (buffer3 != null)
                    {
                        writer.Write(buffer3);
                    }
                }
                return stream.ToArray();
            }
        }

        private byte[] GetAutoFilterTop10Buffer(IExcelFilterColumn item, IExcelCustomFilters cf, IExcelTop10Filter top10)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter((Stream) stream))
                {
                    writer.Write((short) ((short) item.AutoFilterColumnId));
                    ushort num = 0;
                    num |= ((cf == null) || cf.And) ? ((ushort) 0) : ((ushort) 1);
                    if (top10 != null)
                    {
                        num |= 0x10;
                        num |= top10.Top ? ((ushort) 0x20) : ((ushort) 0);
                        num |= top10.Percent ? ((ushort) 0x40) : ((ushort) 0);
                        num |= (ushort)((((int) top10.Value) << 7) & 0xff80);
                    }
                    writer.Write(num);
                    byte[] buffer = null;
                    IExcelCustomFilter filter = null;
                    if (cf != null)
                    {
                        filter = cf.Filter1;
                    }
                    else if (top10 != null)
                    {
                        filter = new ExcelCustomFilter();
                        if (top10.Top)
                        {
                            filter.Operator = ExcelFilterOperator.GreaterThanOrEqual;
                        }
                        else
                        {
                            filter.Operator = ExcelFilterOperator.LessThanOrEqual;
                        }
                        if (!double.IsNaN(top10.FilterValue))
                        {
                            filter.Value = (double) top10.FilterValue;
                        }
                        filter.ValueType = 4;
                    }
                    buffer = this.WriteDOper(writer, (filter != null) ? filter : null);
                    byte[] buffer3 = null;
                    buffer3 = this.WriteDOper(writer, (cf != null) ? cf.Filter2 : null);
                    if (buffer != null)
                    {
                        writer.Write(buffer);
                    }
                    if (buffer3 != null)
                    {
                        writer.Write(buffer3);
                    }
                }
                return stream.ToArray();
            }
        }

        private IRange GetConditionalFormatRefBound(List<IRange> ranges)
        {
            if ((ranges == null) || (ranges.Count == 0))
            {
                return null;
            }
            int row = 0x7fffffff;
            int column = 0x7fffffff;
            int num3 = -2147483648;
            int num4 = -2147483648;
            foreach (IRange range in ranges)
            {
                if (range.Row < row)
                {
                    row = range.Row;
                }
                if (range.Column < column)
                {
                    column = range.Column;
                }
                if (((range.Row + range.RowSpan) - 1) > num3)
                {
                    num3 = (range.Row + range.RowSpan) - 1;
                }
                if (((range.Column + range.ColumnSpan) - 1) > num4)
                {
                    num4 = (range.Column + range.ColumnSpan) - 1;
                }
            }
            return new ExcelCellRange { Row = row, Column = column, RowSpan = (num3 - row) + 1, ColumnSpan = (num4 - column) + 1 };
        }

        private int GetDXFN12RecrodSize(IDifferentialFormatting dxfExt)
        {
            int num = 0;
            if ((((dxfExt.NumberFormat != null) || (dxfExt.FormatId >= 0)) || ((dxfExt.Font != null) || (dxfExt.Alignment != null))) || (((dxfExt.Border != null) || (dxfExt.Fill != null)) || (dxfExt.IsHidden || dxfExt.IsLocked)))
            {
                num += 6;
                if ((dxfExt.NumberFormat != null) || (dxfExt.FormatId >= 0))
                {
                    if ((dxfExt.NumberFormat != null) && (dxfExt.NumberFormat.NumberFormatCode != null))
                    {
                        XLUnicodeString str = new XLUnicodeString {
                            Text = dxfExt.NumberFormat.NumberFormatCode
                        };
                        short num2 = 0;
                        if (str.fHighByte == 0)
                        {
                            num2 = (short)(5 + str.cch);
                        }
                        else
                        {
                            num2 = (short)(5 + (2 * str.cch));
                        }
                        num += num2;
                    }
                    else
                    {
                        num += 2;
                    }
                }
                if (dxfExt.Font != null)
                {
                    num += 0x76;
                }
                if (dxfExt.Alignment != null)
                {
                    num += 8;
                }
                if (dxfExt.Border != null)
                {
                    num += 8;
                }
                if (dxfExt.Fill != null)
                {
                    num += 4;
                }
                if (dxfExt.IsHidden || dxfExt.IsLocked)
                {
                    num += 2;
                }
            }
            if ((dxfExt.ExtendedPropertyList != null) && (dxfExt.ExtendedPropertyList.Count != 0))
            {
                num += 8;
                foreach (Tuple<string, object> tuple in dxfExt.ExtendedPropertyList)
                {
                    num += 4;
                    if (tuple.Item2 is ExcelColor)
                    {
                        num += 0x10;
                    }
                    else if (tuple.Item2 is ushort)
                    {
                        num += 2;
                    }
                }
            }
            return num;
        }

        private uint GetDXFOptionFlag(IDifferentialFormatting dxf)
        {
            uint num = 0xfc3fffff;
            if (dxf.Alignment != null)
            {
                if (dxf.Alignment.HorizontalAlignment != ExcelHorizontalAlignment.General)
                {
                    num &= 0xfffffffe;
                }
                if (dxf.Alignment.VerticalAlignment != ExcelVerticalAlignment.Bottom)
                {
                    num &= 0xfffffffd;
                }
                if (dxf.Alignment.IsTextWrapped)
                {
                    num &= 0xfffffffb;
                }
                if (dxf.Alignment.TextRotation != 0)
                {
                    num &= 0xfffffff7;
                }
                if (dxf.Alignment.IsJustifyLastLine)
                {
                    num &= 0xffffffef;
                }
                if (dxf.Alignment.IndentationLevel != 0)
                {
                    num &= 0xffffffdf;
                }
                if (dxf.Alignment.IsShrinkToFit)
                {
                    num &= 0xffffffbf;
                }
                if (dxf.Alignment.TextDirection != TextDirection.AccordingToContext)
                {
                    num &= 0x7fffffff;
                }
            }
            if (dxf.IsLocked)
            {
                num &= 0xfffffeff;
            }
            if (dxf.IsHidden)
            {
                num &= 0xfffffdff;
            }
            if (dxf.Border != null)
            {
                ExcelBorderSide side = new ExcelBorderSide();
                if (dxf.Border.Left != side)
                {
                    num &= 0xfffffbff;
                }
                if (dxf.Border.Right != side)
                {
                    num &= 0xfffff7ff;
                }
                if (dxf.Border.Top != side)
                {
                    num &= 0xffffefff;
                }
                if (dxf.Border.Bottom != side)
                {
                    num &= 0xffffdfff;
                }
            }
            if (dxf.Fill != null)
            {
                if (((FillPatternType) dxf.Fill.Item1) != FillPatternType.None)
                {
                    num &= 0xfffeffff;
                }
                if ((dxf.Fill.Item2 != null) && (dxf.Fill.Item2 != ExcelColor.EmptyColor))
                {
                    num &= 0xfffdffff;
                }
                if ((dxf.Fill.Item3 != null) && (dxf.Fill.Item3 != ExcelColor.EmptyColor))
                {
                    num &= 0xfffbffff;
                }
            }
            if ((dxf.FormatId >= 0) || (dxf.NumberFormat != null))
            {
                num |= 0x2000000;
                num &= 0xfef7ffff;
            }
            else
            {
                num &= 0xfcffffff;
            }
            if (dxf.Font == null)
            {
                num &= 0xfbffffff;
            }
            if (dxf.Alignment == null)
            {
                num &= 0xf7ffffff;
            }
            if (dxf.Border == null)
            {
                num &= 0xefffffff;
            }
            if (dxf.Fill == null)
            {
                num &= 0xdfffffff;
            }
            if (!dxf.IsHidden && !dxf.IsLocked)
            {
                num &= 0xbfffffff;
            }
            return num;
        }

        private static List<ExternName> GetExternNameRecords(List<string> customOrFunctionNameList)
        {
            List<ExternName> list = new List<ExternName>();
            foreach (string str in customOrFunctionNameList)
            {
                ExternName name = new ExternName {
                    fBuiltIn = false,
                    fIcon = false,
                    fOle = false,
                    fOleLink = false,
                    fWantAdvise = false,
                    fWantPict = false,
                    cf = 0,
                    body = new ExternDocName()
                };
                name.body.ixals = 0;
                name.body.extName = new ShortXLUnicodeString();
                name.body.extName.Text = str;
                name.body.nameDefinition = new ExtNameParsedFormula();
                name.body.nameDefinition.cb = 0;
                list.Add(name);
            }
            return list;
        }

        private static SupBookBiff8Structure GetExternWorkBookSupBook(ExcelSupBook supBook)
        {
            SupBookBiff8Structure structure = new SupBookBiff8Structure {
                ctab = (ushort) supBook.SheetCount,
                virtPath = new XLUnicodeStringNoCch()
            };
            structure.virtPath.Text = supBook.FileName;
            structure.cch = structure.virtPath.cch;
            structure.rgst = new XLUnicodeString[supBook.SheetCount];
            for (int i = 0; i < supBook.SheetCount; i++)
            {
                structure.rgst[i] = new XLUnicodeString();
                structure.rgst[i].Text = supBook.SheetNames[i];
            }
            return structure;
        }

        private string GetFormulaForConditionalFormat(string formula, int sheet, bool useR1C1, LinkTable linkTable)
        {
            if (string.IsNullOrWhiteSpace(formula))
            {
                return formula;
            }
            string str = formula;
            ParsedToken token = Parser.Parse(formula, 0, 0, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
            if ((token != null) && (token.TokenType == TokenType.Reference))
            {
                return string.Format("'{0}'!{1}", (object[]) new object[] { this._excelWriter.GetSheetName(sheet), formula });
            }
            return str;
        }

        private List<Lbl> GetNameRecords(List<string> names, List<byte[]> nameDefinitions, List<byte[]> extras, List<short> tabs, List<bool> hiddenState)
        {
            List<Lbl> list = new List<Lbl>();
            for (int i = 0; i < names.Count; i++)
            {
                string str = names[i];
                if (str != null)
                {
                    Lbl lbl = new Lbl();
                    byte[] buffer = nameDefinitions[i];
                    byte[] buffer2 = extras[i];
                    if (str.StartsWith("_xlnm.Print_Area"))
                    {
                        short sheetIndex = tabs[i];
                        byte[] buffer3 = buffer;
                        lbl.cce = (ushort)buffer3.Length;
                        lbl.cch = 1;
                        lbl.chKey = 0;
                        lbl.fBuiltin = true;
                        lbl.fCalcExp = false;
                        lbl.fFunc = false;
                        lbl.fGrp = 0;
                        lbl.fHidden = false;
                        lbl.fOB = false;
                        lbl.fProc = false;
                        lbl.fPublished = false;
                        lbl.fWorkbookParam = false;
                        short num3 = this.FixWorksheetIndex(sheetIndex);
                        lbl.itab = (ushort)(num3 + 1);
                        lbl.Name = new XLUnicodeStringNoCch();
                        lbl.Name.cch = 1;
                        lbl.Name.fHighByte = 0;
                        lbl.Name.rgb = new byte[] { 6 };
                        lbl.rgce = buffer3;
                        lbl.extra = buffer2;
                        list.Add(lbl);
                    }
                    else if (str.StartsWith("_xlnm.Print_Titles"))
                    {
                        short num4 = tabs[i];
                        byte[] buffer4 = new byte[buffer.Length + 3];
                        MemoryStream stream = new MemoryStream(buffer4);
                        BinaryWriter @this = new BinaryWriter((Stream) stream);
                        @this.Write((byte) 0x29);
                        @this.Write((short) buffer.Length);
                        @this.Write(buffer);
                        if ((buffer2 != null) && (buffer2.Length > 0))
                        {
                            @this.Write(buffer2);
                        }
                        @this.Flush();
                        @this.Close();
                        byte[] buffer5 = buffer4;
                        lbl.cce = (ushort)buffer5.Length;
                        lbl.cch = 1;
                        lbl.chKey = 0;
                        lbl.fBuiltin = true;
                        lbl.fCalcExp = false;
                        lbl.fFunc = false;
                        lbl.fGrp = 0;
                        lbl.fHidden = false;
                        lbl.fOB = false;
                        lbl.fProc = false;
                        lbl.fPublished = false;
                        lbl.fWorkbookParam = false;
                        short num5 = this.FixWorksheetIndex(num4);
                        lbl.itab = (ushort)(num5 + 1);
                        lbl.Name = new XLUnicodeStringNoCch();
                        lbl.Name.cch = 1;
                        lbl.Name.fHighByte = 0;
                        lbl.Name.rgb = new byte[] { 7 };
                        lbl.rgce = buffer5;
                        lbl.extra = buffer2;
                        list.Add(lbl);
                    }
                    else if (((((str == "Consolidate_Area") || (str == "Auto_Open")) || ((str == "Auto_Close") || (str == "Extract"))) || (((str == "Database") || (str == "Criteria")) || ((str == "Recorder") || (str == "Data_Form")))) || (((str == "Auto_Activate") || (str == "Auto_Deactivate")) || (((str == "Sheet_Title") || (str == "_FilterDatabase")) || (str == "_xlnm._FilterDatabase"))))
                    {
                        int num6 = 0;
                        if (str == "Consolidate_Area")
                        {
                            num6 = 0;
                        }
                        else if (str == "Auto_Open")
                        {
                            num6 = 1;
                        }
                        else if (str == "Auto_Close")
                        {
                            num6 = 2;
                        }
                        else if (str == "Extract")
                        {
                            num6 = 3;
                        }
                        else if (str == "Database")
                        {
                            num6 = 4;
                        }
                        else if (str == "Criteria")
                        {
                            num6 = 5;
                        }
                        else if (str == "Recorder")
                        {
                            num6 = 8;
                        }
                        else if (str == "Data_Form")
                        {
                            num6 = 9;
                        }
                        else if (str == "Auto_Activate")
                        {
                            num6 = 10;
                        }
                        else if (str == "Auto_Deactivate")
                        {
                            num6 = 11;
                        }
                        else if (str == "Sheet_Title")
                        {
                            num6 = 12;
                        }
                        else if ((str == "_FilterDatabase") || (str == "_xlnm._FilterDatabase"))
                        {
                            num6 = 13;
                        }
                        short num7 = tabs[i];
                        byte[] buffer6 = new byte[buffer.Length + 3];
                        MemoryStream stream2 = new MemoryStream(buffer6);
                        BinaryWriter writer2 = new BinaryWriter((Stream) stream2);
                        writer2.Write((byte) 0x29);
                        writer2.Write((short) buffer.Length);
                        writer2.Write(buffer);
                        if ((buffer2 != null) && (buffer2.Length > 0))
                        {
                            writer2.Write(buffer2);
                        }
                        writer2.Flush();
                        writer2.Close();
                        byte[] buffer7 = buffer6;
                        lbl.cce = (ushort)buffer7.Length;
                        lbl.cch = 1;
                        lbl.chKey = 0;
                        lbl.fBuiltin = true;
                        lbl.fCalcExp = false;
                        lbl.fFunc = false;
                        lbl.fGrp = 0;
                        lbl.fHidden = false;
                        lbl.fOB = false;
                        lbl.fProc = false;
                        lbl.fPublished = false;
                        lbl.fWorkbookParam = false;
                        short num8 = this.FixWorksheetIndex(num7);
                        lbl.itab = (ushort)(num8 + 1);
                        lbl.Name = new XLUnicodeStringNoCch();
                        lbl.Name.cch = 1;
                        lbl.Name.fHighByte = 0;
                        lbl.Name.rgb = new byte[] { (byte) num6 };
                        lbl.rgce = buffer7;
                        lbl.extra = buffer2;
                        list.Add(lbl);
                    }
                    else if (str.ToUpperInvariant().StartsWith("_XLFN."))
                    {
                        lbl.fBuiltin = false;
                        lbl.fCalcExp = false;
                        lbl.fFunc = true;
                        lbl.fGrp = 0;
                        lbl.fHidden = true;
                        lbl.fOB = false;
                        lbl.fProc = true;
                        lbl.fPublished = false;
                        lbl.fWorkbookParam = false;
                        lbl.Name = new XLUnicodeStringNoCch();
                        lbl.Name.Text = str;
                        lbl.cch = (byte) lbl.Name.cch;
                        short num9 = this.FixWorksheetIndex(tabs[i]);
                        lbl.itab = (ushort)(num9 + 1);
                        lbl.rgce = buffer;
                        lbl.extra = buffer2;
                        lbl.cce = (ushort)buffer.Length;
                        list.Add(lbl);
                    }
                    else
                    {
                        lbl.fBuiltin = false;
                        lbl.fCalcExp = false;
                        lbl.fFunc = false;
                        lbl.fGrp = 0;
                        lbl.fHidden = hiddenState[i];
                        lbl.fOB = false;
                        lbl.fProc = false;
                        lbl.fPublished = false;
                        lbl.fWorkbookParam = false;
                        lbl.Name = new XLUnicodeStringNoCch();
                        lbl.Name.Text = str;
                        lbl.cch = (byte) lbl.Name.cch;
                        short num10 = this.FixWorksheetIndex(tabs[i]);
                        lbl.itab = (ushort)(num10 + 1);
                        lbl.rgce = buffer;
                        lbl.extra = buffer2;
                        lbl.cce = (ushort)buffer.Length;
                        list.Add(lbl);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the print title name records.
        /// </summary>
        /// <returns>Returns the name records.</returns>
        private List<Lbl> GetPrintTitleNameRecords()
        {
            return new List<Lbl>();
        }

        private static SupBookBiff8Structure GetSameSheetReferencingSupBook()
        {
            return new SupBookBiff8Structure { ctab = 0, cch = 1 };
        }

        private Tuple<int, int> GetSelectionTopLeftIndex(int sheet)
        {
            int top = 0;
            int left = 0;
            List<GcRect> selectionList = new List<GcRect>();
            PaneType topLeft = PaneType.TopLeft;
            GcPoint activeCell = new GcPoint();
            for (int i = 3; i >= 0; i--)
            {
                topLeft = (PaneType) i;
                selectionList.Clear();
                if (this._excelWriter.GetSelectionList((short) sheet, selectionList, ref activeCell, ref topLeft) && (selectionList.Count > 0))
                {
                    top = (int) selectionList[0].Top;
                    left = (int) selectionList[0].Left;
                    break;
                }
            }
            return new Tuple<int, int>(top, left);
        }

        private static SupBookBiff8Structure GetSelfReferencingSupBook(ushort sheetCount)
        {
            return new SupBookBiff8Structure { ctab = sheetCount, cch = 0x401 };
        }

        private string GetSheetNameForAutoFilter(int sheetIndex)
        {
            string sheetName = this._excelWriter.GetSheetName(sheetIndex);
            if (sheetName.Contains(" "))
            {
                return ("'" + sheetName + "'");
            }
            return sheetName;
        }

        private int GetTemplementIndex(IExcelConditionalFormatRule rule)
        {
            if (rule is IExcelGeneralRule)
            {
                IExcelGeneralRule rule2 = rule as IExcelGeneralRule;
                if (((rule2.Type == ExcelConditionalFormatType.ContainsText) || (rule2.Type == ExcelConditionalFormatType.NotContainsText)) || ((rule2.Type == ExcelConditionalFormatType.BeginsWith) || (rule2.Type == ExcelConditionalFormatType.EndsWith)))
                {
                    return 8;
                }
                if ((((rule2.Type == ExcelConditionalFormatType.Today) || (rule2.Type == ExcelConditionalFormatType.Tomorrow)) || ((rule2.Type == ExcelConditionalFormatType.Yesterday) || (rule2.Type == ExcelConditionalFormatType.Last7Days))) || ((((rule2.Type == ExcelConditionalFormatType.LastMonth) || (rule2.Type == ExcelConditionalFormatType.NextMonth)) || ((rule2.Type == ExcelConditionalFormatType.ThisWeek) || (rule2.Type == ExcelConditionalFormatType.NextWeek))) || ((rule2.Type == ExcelConditionalFormatType.LastWeek) || (rule2.Type == ExcelConditionalFormatType.ThisWeek))))
                {
                    return (int) rule2.Type;
                }
                if ((rule2.Type == ExcelConditionalFormatType.Top10) && ((rule2.Bottom.HasValue && rule2.Bottom.Value) || (rule2.Percent.HasValue && rule2.Percent.Value)))
                {
                    return 5;
                }
                if (rule2.Type == ExcelConditionalFormatType.AboveOrEqualToAverage)
                {
                    return 0x1d;
                }
                if (rule2.Type == ExcelConditionalFormatType.BelowOrEqualToAverage)
                {
                    return 30;
                }
                if ((rule2.Type == ExcelConditionalFormatType.AboveAverage) && rule2.StdDev.HasValue)
                {
                    return 0x19;
                }
                if ((rule2.Type == ExcelConditionalFormatType.BelowAverage) && rule2.StdDev.HasValue)
                {
                    return 0x1a;
                }
            }
            return -1;
        }

        private BiffRecord GetUnsupportRecord(int sheetIndex, BiffRecordNumber recordType)
        {
            if (this._excelWriter is IExcelLosslessWriter)
            {
                List<IUnsupportRecord> unsupportItems = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(sheetIndex);
                if (unsupportItems != null)
                {
                    foreach (IUnsupportRecord record in unsupportItems)
                    {
                        if (record.FileType == ExcelFileType.XLS)
                        {
                            if (record.Value is BiffRecord)
                            {
                                if ((record.Value as BiffRecord).RecordType == recordType)
                                {
                                    return (record.Value as BiffRecord);
                                }
                            }
                            else if (record.Value is List<BiffRecord>)
                            {
                                List<BiffRecord> list2 = record.Value as List<BiffRecord>;
                                foreach (BiffRecord record2 in list2)
                                {
                                    if (record2.RecordType == recordType)
                                    {
                                        return record2;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private IUnsupportRecord GetUnsupportRecord(int sheetIndex, RecordCategory category, bool removeFound = false)
        {
            List<IUnsupportRecord> unsupportRecords = this.GetUnsupportRecords(sheetIndex);
            IUnsupportRecord record = null;
            if (unsupportRecords != null)
            {
                for (int i = 0; i < unsupportRecords.Count; i++)
                {
                    IUnsupportRecord record2 = unsupportRecords[i];
                    if (record2.Category == category)
                    {
                        record = record2;
                        if (removeFound)
                        {
                            unsupportRecords.RemoveAt(i);
                        }
                    }
                }
            }
            return record;
        }

        private List<IUnsupportRecord> GetUnsupportRecords(int sheet)
        {
            if (this._excelWriter is IExcelLosslessWriter)
            {
                if ((this.unsupportRecords != null) && this.unsupportRecords.ContainsKey(sheet))
                {
                    return this.unsupportRecords[sheet];
                }
                if (this.unsupportRecords == null)
                {
                    this.unsupportRecords = new Dictionary<int, List<IUnsupportRecord>>();
                }
                List<IUnsupportRecord> unsupportItems = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(sheet);
                if (unsupportItems != null)
                {
                    List<IUnsupportRecord> list2 = new List<IUnsupportRecord>();
                    foreach (IUnsupportRecord record in unsupportItems)
                    {
                        if (record.FileType == ExcelFileType.XLS)
                        {
                            list2.Add(record);
                        }
                    }
                    this.unsupportRecords[sheet] = list2;
                    return list2;
                }
            }
            return null;
        }

        private int GetXfId(IExtendedFormat format)
        {
            for (int i = 0; i < this._allformats.Count; i++)
            {
                if (this._allformats[i].Equals(format))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the xti.
        /// </summary>
        /// <param name="iSupBook">The index of sup book.</param>
        /// <param name="itabFirst">The index of first tab.</param>
        /// <param name="itabLast">The index of last tab.</param>
        /// <returns>Returns the xti.</returns>
        private static XTI GetXti(ushort iSupBook, short itabFirst, short itabLast)
        {
            return new XTI { iSupBook = iSupBook, itabFirst = itabFirst, itabLast = itabLast };
        }

        private void InitLinkTable()
        {
            List<IExternalWorkbookInfo> externWorkbookInfo = this._excelWriter.GetExternWorkbookInfo();
            List<IFunction> customOrFunctionNameList = this._excelWriter.GetCustomOrFunctionNameList();
            new List<byte[]>();
            List<Tuple<int, short[]>> list3 = new List<Tuple<int, short[]>>();
            if (externWorkbookInfo != null)
            {
                foreach (ExternalWorkbookInfo info2 in externWorkbookInfo)
                {
                    if (info2 != null)
                    {
                        if (info2.ExternalBookBits != null)
                        {
                            ExcelSupBook book = this.ReadSupBook(info2.ExternalBookBits);
                            if (info2.ExternNameBits != null)
                            {
                                book.ExternNameBuffers = info2.ExternNameBits;
                            }
                            this._linkTable.SupBooks.Add(book);
                        }
                        if (info2.ExternSheetBits != null)
                        {
                            list3.AddRange((IEnumerable<Tuple<int, short[]>>) info2.ExternSheetBits);
                        }
                    }
                }
            }
            List<short[]> list4 = Enumerable.ToList<short[]>(Enumerable.Select<Tuple<int, short[]>, short[]>((IEnumerable<Tuple<int, short[]>>) Enumerable.OrderBy<Tuple<int, short[]>, int>((IEnumerable<Tuple<int, short[]>>) list3, delegate (Tuple<int, short[]> item) {
                return item.Item1;
            }), delegate (Tuple<int, short[]> item) {
                return item.Item2;
            }));
            if ((this._linkTable.SupBooks == null) || (this._linkTable.SupBooks.Count == 0))
            {
                ExcelSupBook book2 = new ExcelSupBook {
                    IsSelfReferenced = true,
                    SheetCount = this._excelWriter.GetSheetCount()
                };
                for (int i = 0; i < book2.SheetCount; i++)
                {
                    string sheetName = this._excelWriter.GetSheetName(i);
                    book2.SheetNames.Add(sheetName);
                }
                this._linkTable.SupBooks.Add(book2);
                ExcelSupBook book3 = new ExcelSupBook {
                    IsAddInReferencedSupBook = true
                };
                this._linkTable.SupBooks.Add(book3);
                ExcelSupBook book4 = new ExcelSupBook {
                    IsCurrentSheetSupBook = true
                };
                this._linkTable.SupBooks.Add(book4);
                if (externWorkbookInfo != null)
                {
                    foreach (IExternalWorkbookInfo info3 in externWorkbookInfo)
                    {
                        ExcelSupBook book5 = new ExcelSupBook {
                            FileName = info3.Name,
                            IsSelfReferenced = false,
                            SheetCount = info3.SheetNames.Count,
                            SheetNames = info3.SheetNames
                        };
                        this._linkTable.SupBooks.Add(book5);
                    }
                }
            }
            if (externWorkbookInfo != null)
            {
                foreach (IExternalWorkbookInfo info4 in externWorkbookInfo)
                {
                    if (!string.IsNullOrWhiteSpace(info4.Name) && (info4.DefinedNames != null))
                    {
                        this._linkTable.ExternalNamedCellRanges.Add(info4.Name, info4.DefinedNames);
                    }
                }
            }
            this._linkTable.InternalSheetNames.AddRange((IEnumerable<string>) this.SheetNames);
            List<IName> internalDefinedNames = this._excelWriter.GetInternalDefinedNames();
            if (internalDefinedNames == null)
            {
                internalDefinedNames = new List<IName>();
            }
            bool flag = false;
            foreach (IName name in internalDefinedNames)
            {
                if (((name.Name != null) && name.Name.StartsWith("_")) && ((name.Name == "_xlnm.Print_Area") || (name.Name == "_xlnm.Print_Titles")))
                {
                    flag = true;
                    break;
                }
            }
            bool flag2 = this._excelWriter.GetCalculationProperty().RefMode == ExcelReferenceStyle.R1C1;
            int sheetCount = this._excelWriter.GetSheetCount();
            if (!flag)
            {
                for (int j = 0; j < sheetCount; j++)
                {
                    string printArea = this._excelWriter.GetPrintArea(j);
                    if (!string.IsNullOrWhiteSpace(printArea))
                    {
                        NamedCellRange range = new NamedCellRange("_xlnm.Print_Area", j);
                        if (flag2)
                        {
                            range.RefersTo = this.ConvertR1C1FormulaToA1Formula(printArea, 0, 0);
                        }
                        else
                        {
                            range.RefersTo = printArea;
                        }
                        internalDefinedNames.Add(range);
                    }
                    string printTitle = this._excelWriter.GetPrintTitle(j);
                    if (!string.IsNullOrWhiteSpace(printTitle))
                    {
                        NamedCellRange range2 = new NamedCellRange("_xlnm.Print_Titles", j);
                        if (flag2)
                        {
                            range2.RefersTo = this.ConvertR1C1FormulaToA1Formula(printTitle, 0, 0);
                        }
                        else
                        {
                            range2.RefersTo = printTitle;
                        }
                        internalDefinedNames.Add(range2);
                    }
                }
            }
            List<IBuiltInName> builtInNameList = this._excelWriter.GetBuiltInNameList();
            List<Tuple<int, string, int, object>> list7 = new List<Tuple<int, string, int, object>>();
            int num4 = (internalDefinedNames.Count + ((builtInNameList != null) ? builtInNameList.Count : 0)) + 1;
            foreach (IName name2 in internalDefinedNames)
            {
                NamedCellRange range3 = name2 as NamedCellRange;
                if ((range3 != null) && (range3.DefinitionBits != null))
                {
                    list7.Add(new Tuple<int, string, int, object>(range3.DefinitionBits.Item1, range3.Name, range3.Index, name2));
                }
                else
                {
                    list7.Add(new Tuple<int, string, int, object>(num4++, name2.Name, name2.Index, name2));
                }
            }
            if (builtInNameList != null)
            {
                foreach (IBuiltInName name3 in builtInNameList)
                {
                    BuiltInName name4 = name3 as BuiltInName;
                    if ((name4 != null) && (name4.NameBits != null))
                    {
                        list7.Add(new Tuple<int, string, int, object>(name4.NameBits.Item1, name4.Name, -1, name3));
                    }
                    else
                    {
                        list7.Add(new Tuple<int, string, int, object>(num4++, name3.Name, -1, name3));
                    }
                }
            }
            foreach (Tuple<string, int, object> tuple in Enumerable.Select<Tuple<int, string, int, object>, Tuple<string, int, object>>((IEnumerable<Tuple<int, string, int, object>>) Enumerable.OrderBy<Tuple<int, string, int, object>, int>((IEnumerable<Tuple<int, string, int, object>>) list7, delegate (Tuple<int, string, int, object> item) {
                return item.Item1;
            }), delegate (Tuple<int, string, int, object> item) {
                return new Tuple<string, int, object>(item.Item2, item.Item3, item.Item4);
            }))
            {
                if ((tuple.Item1.ToUpperInvariant() != "_FILTERDATABASE") && (tuple.Item1.ToUpperInvariant() != "_XLNM._FILTERDATABASE"))
                {
                    this._linkTable.DefinedNames.Add(tuple);
                    this._linkTable.DefinedNamesHashSet.Add(tuple.Item1);
                }
            }
            this._linkTable.CustomOrFuctionNames = (customOrFunctionNameList != null) ? Enumerable.ToList<string>(Enumerable.Select<IFunction, string>((IEnumerable<IFunction>) customOrFunctionNameList, delegate (IFunction item) {
                return item.Name;
            })) : null;
            if (this._excelWriter is IExcelLosslessWriter)
            {
                IUnsupportRecord record = this.GetUnsupportRecord(-1, RecordCategory.Formula, true);
                if ((record != null) && (record.Value is BiffRecord))
                {
                    BiffRecord record2 = record.Value as BiffRecord;
                    SimpleBinaryReader reader = new SimpleBinaryReader(record2.DataBuffer);
                    ushort num5 = reader.ReadUInt16();
                    for (int k = 0; k < num5; k++)
                    {
                        short num7 = reader.ReadInt16();
                        short num8 = reader.ReadInt16();
                        short num9 = reader.ReadInt16();
                        ExcelExternSheet sheet = new ExcelExternSheet {
                            supBookIndex = num7,
                            beginSheetIndex = num8,
                            endSheetIndex = num9
                        };
                        this._linkTable.ExternalSheets.Add(sheet);
                    }
                }
            }
            if (list4 != null)
            {
                foreach (short[] numArray in list4)
                {
                    ExcelExternSheet sheet2 = new ExcelExternSheet {
                        supBookIndex = numArray[0],
                        beginSheetIndex = numArray[1],
                        endSheetIndex = numArray[2]
                    };
                    if (this._linkTable.ExternalSheets.IndexOf(sheet2) == -1)
                    {
                        this._linkTable.ExternalSheets.Add(sheet2);
                    }
                }
            }
            else if ((this._linkTable.CustomOrFuctionNames != null) && (this._linkTable.CustomOrFuctionNames.Count > 0))
            {
                ExcelExternSheet sheet4 = new ExcelExternSheet {
                    supBookIndex = 0,
                    beginSheetIndex = -2,
                    endSheetIndex = -2
                };
                if (this._linkTable.ExternalSheets.IndexOf(sheet4) == -1)
                {
                    this._linkTable.ExternalSheets.Add(sheet4);
                }
            }
        }

        private bool IsValidConditionalFormating(IExcelConditionalFormat conditionalFormat)
        {
            if (conditionalFormat.ConditionalFormattingRules.Count == 0)
            {
                return false;
            }
            foreach (IExcelConditionalFormatRule rule in conditionalFormat.ConditionalFormattingRules)
            {
                if (rule is ExcelGeneralRule)
                {
                    ExcelGeneralRule rule2 = rule as ExcelGeneralRule;
                    if (rule2.Formulas.Count == 0)
                    {
                        IRange range = conditionalFormat.Ranges[0];
                        string str = this.AppendA1Letter(range.Column) + ((int) (range.Row + 1));
                        string str2 = string.Empty;
                        if (range.RowSpan == 0x100000)
                        {
                            str2 = string.Format("${0}:${1}", (object[]) new object[] { this.AppendA1Letter(range.Column), this.AppendA1Letter((range.Column + range.ColumnSpan) - 1) });
                        }
                        else if (range.ColumnSpan == 0x4000)
                        {
                            str2 = string.Format("${0}:${1}", (object[]) new object[] { ((int) (range.Row + 1)), ((int) (range.Row + range.RowSpan)) });
                        }
                        else
                        {
                            str2 = string.Format("${0}${1}:${2}${3}", (object[]) new object[] { this.AppendA1Letter(range.Column), ((int) (range.Row + 1)), this.AppendA1Letter((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
                        }
                        if (rule2.Type == ExcelConditionalFormatType.Top10)
                        {
                            if (rule2.Bottom.HasValue && rule2.Bottom.Value)
                            {
                                if (rule2.Percent.HasValue && rule2.Percent.Value)
                                {
                                    rule2.Formulas.Add(string.Format("IF(INT(COUNT({0})*{1}%)>0,SMALL({0},INT(COUNT({0})*{1}%)),MIN({0}))>={2}", (object[]) new object[] { str2, ((int) rule2.Rank.Value), str }));
                                }
                                else
                                {
                                    rule2.Formulas.Add(string.Format("SMALL(({0}),MIN({1},COUNT({0})))>={2}", (object[]) new object[] { str2, ((int) rule2.Rank.Value), str }));
                                }
                            }
                            else if (rule2.Percent.HasValue && rule2.Percent.Value)
                            {
                                rule2.Formulas.Add(string.Format("IF(INT(COUNT({0})*{1}%)>0,LARGE({0},INT(COUNT({0})*{1}%)),MIN({0}))<={2}", (object[]) new object[] { str2, ((int) rule2.Rank.Value), str }));
                            }
                            else
                            {
                                rule2.Formulas.Add(string.Format("LARGE(({0}),MIN({1},COUNT({0})))<={2}", (object[]) new object[] { str2, ((int) rule2.Rank.Value), str }));
                            }
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.ContainsText)
                        {
                            rule2.Formulas.Add(string.Format("NOT(ISERROR(SEARCH(\"{0}\",{1})))", (object[]) new object[] { rule2.Text, str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.NotContainsText)
                        {
                            rule2.Formulas.Add(string.Format("ISERROR(SEARCH(\"{0}\",{1}))", (object[]) new object[] { rule2.Text, str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.BeginsWith)
                        {
                            rule2.Formulas.Add(string.Format("LEFT({0},{1}) = \"{2}\"", (object[]) new object[] { str, ((int) rule2.Text.Length), rule2.Text }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.EndsWith)
                        {
                            rule2.Formulas.Add(string.Format("RIGHT({0},{1}) = \"{2}\"", (object[]) new object[] { str, ((int) rule2.Text.Length), rule2.Text }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.DuplicateValues)
                        {
                            rule2.Formulas.Add(string.Format("AND(COUNTIF({0},{1})>1,NOT(ISBLANK({1})))", (object[]) new object[] { str2, str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.UniqueValues)
                        {
                            rule2.Formulas.Add(string.Format("AND(COUNTIF({0},{1})=1,NOT(ISBLANK({1})))", (object[]) new object[] { str2, str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.Today)
                        {
                            rule2.Formulas.Add(string.Format("FLOOR({0},1)= TODAY()", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.Tomorrow)
                        {
                            rule2.Formulas.Add(string.Format("FLOOR({0},1)= TODAY()+1", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.Yesterday)
                        {
                            rule2.Formulas.Add(string.Format("FLOOR({0},1)= TODAY()-1", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.Last7Days)
                        {
                            rule2.Formulas.Add(string.Format("AND(TODAY()-FLOOR({0},1)<=6,FLOOR({0},1)<=TODAY())", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.NextWeek)
                        {
                            rule2.Formulas.Add(string.Format("AND(ROUNDDOWN({0},0)-TODAY()>(7-WEEKDAY(TODAY())),ROUNDDOWN({0},0)-TODAY()<(15-WEEKDAY(TODAY())))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.LastWeek)
                        {
                            rule2.Formulas.Add(string.Format("AND(TODAY()-ROUNDDOWN({0},0)>=(WEEKDAY(TODAY())),TODAY()-ROUNDDOWN({0},0)<(WEEKDAY(TODAY())+7))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.ThisMonth)
                        {
                            rule2.Formulas.Add(string.Format("AND(MONTH({0})=MONTH(TODAY()),YEAR({0})=YEAR(TODAY()))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.ThisWeek)
                        {
                            rule2.Formulas.Add(string.Format("AND(TODAY()-ROUNDDOWN({0},0)<=WEEKDAY(TODAY())-1,ROUNDDOWN({0},0)-TODAY()<=7-WEEKDAY(TODAY()))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.LastMonth)
                        {
                            rule2.Formulas.Add(string.Format("AND(MONTH({0})=MONTH(EDATE(TODAY(),0-1)),YEAR({0})=YEAR(EDATE(TODAY(),0-1)))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (rule2.Type == ExcelConditionalFormatType.NextMonth)
                        {
                            rule2.Formulas.Add(string.Format("AND(MONTH({0})=MONTH(TODAY())+1,OR(YEAR({0})=YEAR(TODAY()),AND(MONTH({0})=12,YEAR({0})=YEAR(TODAY())+1)))", (object[]) new object[] { str }));
                            continue;
                        }
                        if (((rule2.Type == ExcelConditionalFormatType.AboveAverage) || (rule2.Type == ExcelConditionalFormatType.BelowAverage)) || ((rule2.Type == ExcelConditionalFormatType.AboveOrEqualToAverage) || (rule2.Type == ExcelConditionalFormatType.BelowOrEqualToAverage)))
                        {
                            if ((rule2.Type == ExcelConditionalFormatType.AboveAverage) && !rule2.StdDev.HasValue)
                            {
                                rule2.Formulas.Add(string.Format("{0}>AVERAGE({1})", (object[]) new object[] { str, str2 }));
                                continue;
                            }
                            if ((rule2.Type == ExcelConditionalFormatType.BelowAverage) && !rule2.StdDev.HasValue)
                            {
                                rule2.Formulas.Add(string.Format("{0}<AVERAGE({1})", (object[]) new object[] { str, str2 }));
                                continue;
                            }
                            if (rule2.AboveAverage.HasValue)
                            {
                                if (((rule2.EqualAverage.HasValue && !rule2.AboveAverage.Value) && (rule2.EqualAverage.HasValue && rule2.EqualAverage.Value)) && !rule2.StdDev.HasValue)
                                {
                                    rule2.Formulas.Add(string.Format("{0}<=AVERAGE({1})", (object[]) new object[] { str, str2 }));
                                    continue;
                                }
                                if (!rule2.AboveAverage.Value && !rule2.StdDev.HasValue)
                                {
                                    rule2.Formulas.Add(string.Format("{0}<AVERAGE({1})", (object[]) new object[] { str, str2 }));
                                    continue;
                                }
                            }
                            if ((rule2.EqualAverage.HasValue && rule2.EqualAverage.Value) && !rule2.StdDev.HasValue)
                            {
                                rule2.Formulas.Add(string.Format("{0}>=AVERAGE({1})", (object[]) new object[] { str, str2 }));
                                continue;
                            }
                            if (rule2.StdDev.HasValue)
                            {
                                if (rule2.AboveAverage.HasValue && !rule2.AboveAverage.Value)
                                {
                                    rule2.Formulas.Add(string.Format("({0}-AVERAGE({1}))<=STDEVP({1})*(-{2})", (object[]) new object[] { str, str2, ((int) rule2.StdDev.Value) }));
                                }
                                else
                                {
                                    rule2.Formulas.Add(string.Format("({0}-AVERAGE({1}))>=STDEVP({1})*({2})", (object[]) new object[] { str, str2, ((int) rule2.StdDev.Value) }));
                                }
                                continue;
                            }
                        }
                    }
                    if (rule2.Formulas.Count == 0)
                    {
                        return false;
                    }
                }
                else if (rule is ExcelIconSetsRule)
                {
                    ExcelIconSetsRule rule3 = rule as ExcelIconSetsRule;
                    if (((rule3.IconSet == ExcelIconSetType.Icon_3Stars) || (rule3.IconSet == ExcelIconSetType.Icon_3Triangles)) || (rule3.IconSet == ExcelIconSetType.Icon_5Boxes))
                    {
                        return false;
                    }
                    return true;
                }
            }
            return true;
        }

        private bool IsValidDynamicFilter(IExcelDynamicFilter dynamicFilter)
        {
            if (dynamicFilter == null)
            {
                return false;
            }
            if (((dynamicFilter.Type == ExcelDynamicFilterType.Null) && (dynamicFilter.Value == null)) && (dynamicFilter.MaxValue == null))
            {
                return false;
            }
            return true;
        }

        private bool NeedWriteBorder(IExcelBorder border)
        {
            if (border == null)
            {
                return false;
            }
            if ((!this.NeedWriteColorExt(border.Top.Color) && !this.NeedWriteColorExt(border.Bottom.Color)) && (!this.NeedWriteColorExt(border.Left.Color) && !this.NeedWriteColorExt(border.Right.Color)))
            {
                return false;
            }
            return true;
        }

        private bool NeedWriteColorExt(IExcelColor color)
        {
            if (color == null)
            {
                return false;
            }
            return ((color.ColorType != ExcelColorType.Indexed) || (Math.Abs((double) (color.Tint - 0.0)) > 0.01));
        }

        private void ProcessCells(BinaryWriter writer, short sheetIndex)
        {
            BiffRecord biff = new BiffRecord();
            int num = 0;
            int num2 = 0;
            this._excelWriter.GetDimensions(sheetIndex, ref num2, ref num);
            num = Math.Min(num, 0x100);
            num2 = Math.Min(num2, 0x10000);
            List<IExcelRow> list = new List<IExcelRow>();
            List<IExcelRow> nonEmptyRows = this._excelWriter.GetNonEmptyRows(sheetIndex);
            if (nonEmptyRows != null)
            {
                foreach (IExcelRow row in nonEmptyRows)
                {
                    if ((row != null) && (row.Index < 0x10000))
                    {
                        list.Add(row);
                    }
                }
            }
            double defaultColumnWidth = this._excelWriter.GetDefaultColumnWidth(sheetIndex);
            List<IExcelColumn> nonEmptyColumns = this._excelWriter.GetNonEmptyColumns(sheetIndex);
            if (nonEmptyColumns != null)
            {
                int num4 = 0;
                int num5 = 0;
                while (num5 < nonEmptyColumns.Count)
                {
                    IExcelColumn column = nonEmptyColumns[num5];
                    int index = column.Index;
                    if (index >= 0x100)
                    {
                        break;
                    }
                    int num7 = num5 + 1;
                    int num8 = index;
                    while (num7 < nonEmptyColumns.Count)
                    {
                        IExcelColumn column2 = nonEmptyColumns[num7];
                        if (((((column2.Index != (num8 + 1)) || (column2.Index >= 0x100)) || ((column2.FormatId != column.FormatId) || (column2.Visible != column.Visible))) || ((column2.OutLineLevel != column.OutLineLevel) || (column2.Collapsed != column.Collapsed))) || (((double.IsNaN(column2.Width) || double.IsNaN(column.Width)) || (Math.Ceiling((double) (column2.Width * 256.0)) != Math.Ceiling((double) (column.Width * 256.0)))) && (!double.IsNaN(column2.Width) || !double.IsNaN(column.Width))))
                        {
                            break;
                        }
                        num8 = column2.Index;
                        num5 = num7;
                        num7++;
                    }
                    num5++;
                    num4 = num8;
                    biff = new BiffRecord {
                        RecordType = BiffRecordNumber.COLINFO,
                        DataLength = 12
                    };
                    biff.Write(writer);
                    writer.Write((short) ((short) index));
                    writer.Write((short) ((short) num8));
                    double width = column.Width;
                    if (double.IsNaN(width))
                    {
                        width = this._excelWriter.GetDefaultColumnWidth(sheetIndex);
                    }
                    writer.Write((ushort) (width * 256.0));
                    writer.Write((column.FormatId >= 0) ? ((short) (column.FormatId + this._offSet)) : ((short) 15));
                    byte num10 = 0;
                    if (!column.Visible)
                    {
                        num10 |= 1;
                    }
                    if (!double.IsNaN(column.Width) && (Math.Abs((double) (column.Width - defaultColumnWidth)) > 0.0001))
                    {
                        num10 |= 2;
                    }
                    if (((column.FormatId != -1) && (column.FormatId < this._formats.Count)) && this._formats[column.FormatId].IsShrinkToFit)
                    {
                        num10 |= 4;
                    }
                    writer.Write(num10);
                    byte num11 = 0;
                    num11 |= column.OutLineLevel;
                    if (column.Collapsed)
                    {
                        num11 |= 0x10;
                    }
                    writer.Write(num11);
                    writer.Write((ushort) 0);
                }
                short num12 = (short) this._excelWriter.GetDefaultColumnWidth(sheetIndex);
                if ((num12 == 0) && (num4 < 0x100))
                {
                    biff = new BiffRecord {
                        RecordType = BiffRecordNumber.COLINFO,
                        DataLength = 12
                    };
                    biff.Write(writer);
                    writer.Write((short) (num4 + 1));
                    writer.Write((short) 0x100);
                    writer.Write(num12);
                    writer.Write((short) 15);
                    byte num13 = 1;
                    writer.Write(num13);
                    byte num14 = 0;
                    writer.Write(num14);
                    writer.Write((ushort) 0);
                }
            }
            biff.RecordType = BiffRecordNumber.DIMENSIONS;
            biff.DataLength = 14;
            this.WriteDIMENSIONS(writer, biff, sheetIndex, num2, num);
            bool customHeight = false;
            double defaultRowHeight = this._excelWriter.GetDefaultRowHeight(sheetIndex, ref customHeight);
            foreach (IExcelRow row2 in list)
            {
                double height = row2.Height;
                bool isDefaultRowheight = false;
                if (double.IsNaN(height) || (Math.Abs((double) (height - 0.0)) <= 1E-05))
                {
                    isDefaultRowheight = true;
                    height = defaultRowHeight;
                }
                else if ((height > 0.0) && (Math.Abs((double) (height - defaultRowHeight)) < 1E-05))
                {
                    isDefaultRowheight = true;
                }
                this.WriteRowRecord(writer, sheetIndex, row2, (ushort) Math.Ceiling((double) (height * 20.0)), isDefaultRowheight);
            }
            List<IExcelCell> cells = new List<IExcelCell>();
            this._excelWriter.GetCells(sheetIndex, cells);
            FormulaProcess process = new FormulaProcess();
            bool flag3 = this._calulationProperty.RefMode == ExcelReferenceStyle.A1;
            foreach (IExcelCell cell in cells)
            {
                try
                {
                    double num23;
                    DateTime time;
                    short num26;
                    double num35;
                    if ((cell == null) || ((cell.Row >= 0x10000) || (cell.Column >= 0x100)))
                    {
                        continue;
                    }
                    if (cell.Hyperlink != null)
                    {
                        this._cellsHasHyperlink.Add(cell);
                    }
                    short num17 = this.TryGetStyleID(cell.FormatId);
                    object description = cell.Value;
                    string formulaArray = cell.Formula;
                    if (!flag3)
                    {
                        formulaArray = cell.FormulaR1C1;
                    }
                    if (cell.IsArrayFormula)
                    {
                        formulaArray = cell.FormulaArray;
                        if (!flag3)
                        {
                            formulaArray = cell.FormulaArrayR1C1;
                        }
                    }
                    ushort num18 = (ushort) cell.Row;
                    short num19 = (short) cell.Column;
                    short errorVal = 0;
                    byte[] formulaTokenBits = null;
                    byte[] formulaTokenExtraBits = null;
                    bool flag4 = false;
                    byte[] buffer3 = null;
                    if (((description == null) && (cell.Hyperlink != null)) && !string.IsNullOrWhiteSpace(cell.Hyperlink.Description))
                    {
                        description = cell.Hyperlink.Description;
                        cell.CellType = CellType.String;
                    }
                    if (cell.CellType == CellType.Unknown)
                    {
                        if (!string.IsNullOrWhiteSpace(formulaArray))
                        {
                            cell.CellType = CellType.FormulaString;
                        }
                        else
                        {
                            cell.CellType = this.FigureCellType(cell.Value);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(formulaArray))
                    {
                        if (!cell.IsArrayFormula)
                        {
                            cell.CellType = CellType.FormulaString;
                        }
                        else
                        {
                            cell.CellType = CellType.Array;
                        }
                    }
                    if (((cell.CellType != CellType.Array) && (cell.CellType != CellType.FormulaString)) || string.IsNullOrWhiteSpace(formulaArray))
                    {
                        goto Label_112E;
                    }
                    if ((cell.CellFormula != null) && !cell.CellFormula.IsArrayFormula)
                    {
                        ExcelFormula cellFormula = cell.CellFormula as ExcelFormula;
                        if (cellFormula != null)
                        {
                            if (flag3)
                            {
                                formulaTokenBits = cellFormula.FormulaTokenBits;
                                formulaTokenExtraBits = cellFormula.FormulaTokenExtraBits;
                            }
                            else
                            {
                                formulaTokenBits = cellFormula.FormulaR1C1TokenBits;
                                formulaTokenExtraBits = cellFormula.FormulaR1C1TokenExtraBits;
                            }
                        }
                    }
                    if (formulaTokenBits == null)
                    {
                        int extraDataLength = 0;
                        process.sheet = sheetIndex;
                        process.row = num18;
                        process.column = num19;
                        process.isA1RefStyle = flag3;
                        if (!cell.IsArrayFormula)
                        {
                            try
                            {
                                formulaTokenBits = process.ToExcelParsedFormula(sheetIndex, formulaArray, this._linkTable, ref extraDataLength, false);
                            }
                            catch (ExcelException exception)
                            {
                                if (exception.Code == ExcelExceptionCode.ParseException)
                                {
                                    this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaParseError"), (object[]) new object[] { exception.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception));
                                }
                                else if (exception.Code == ExcelExceptionCode.FormulaError)
                                {
                                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellFormulaError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception));
                                }
                                else
                                {
                                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception));
                                }
                            }
                            catch (NotSupportedException exception2)
                            {
                                this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaNotSupported"), (object[]) new object[] { exception2.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, (Exception) exception2));
                            }
                            catch (Exception exception3)
                            {
                                this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception3));
                            }
                            if (extraDataLength > 0)
                            {
                                byte[] buffer4 = new byte[formulaTokenBits.Length - extraDataLength];
                                byte[] buffer5 = new byte[extraDataLength];
                                Array.Copy(formulaTokenBits, buffer4, buffer4.Length);
                                Array.Copy(formulaTokenBits, buffer4.Length, buffer5, 0, extraDataLength);
                                formulaTokenBits = buffer4;
                                formulaTokenExtraBits = buffer5;
                            }
                        }
                        if (cell.IsArrayFormula)
                        {
                            ExcelFormula formula2 = cell.CellFormula as ExcelFormula;
                            if (formula2 != null)
                            {
                                if (flag3)
                                {
                                    formulaTokenExtraBits = formula2.FormulaTokenBits;
                                    buffer3 = formula2.FormulaTokenExtraBits;
                                }
                                else
                                {
                                    formulaTokenExtraBits = formula2.FormulaR1C1TokenBits;
                                    buffer3 = formula2.FormulaR1C1TokenExtraBits;
                                }
                            }
                            if (formulaTokenExtraBits == null)
                            {
                                process._isArrayFormula = true;
                                try
                                {
                                    formulaTokenExtraBits = process.ToExcelParsedFormula(sheetIndex, formulaArray, this._linkTable, ref extraDataLength, false);
                                }
                                catch (ExcelException exception4)
                                {
                                    if (exception4.Code == ExcelExceptionCode.ParseException)
                                    {
                                        this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaParseError"), (object[]) new object[] { exception4.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception4));
                                    }
                                    else if (exception4.Code == ExcelExceptionCode.FormulaError)
                                    {
                                        this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellFormulaError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception4));
                                    }
                                    else
                                    {
                                        this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception4));
                                    }
                                }
                                catch (NotSupportedException exception5)
                                {
                                    this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaNotSupported"), (object[]) new object[] { exception5.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, (Exception) exception5));
                                }
                                catch (Exception exception6)
                                {
                                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception6));
                                }
                                process._isArrayFormula = false;
                                if (extraDataLength > 0)
                                {
                                    byte[] buffer6 = new byte[formulaTokenExtraBits.Length - extraDataLength];
                                    byte[] buffer7 = new byte[extraDataLength];
                                    Array.Copy(formulaTokenExtraBits, buffer6, buffer6.Length);
                                    Array.Copy(formulaTokenExtraBits, buffer6.Length, buffer7, 0, extraDataLength);
                                    formulaTokenExtraBits = buffer6;
                                    buffer3 = buffer7;
                                }
                            }
                            byte[] buffer8 = new byte[5];
                            buffer8[0] = 1;
                            if ((cell.CellFormula != null) && (cell.CellFormula.ArrayFormulaRange != null))
                            {
                                buffer8[1] = (byte)(cell.CellFormula.ArrayFormulaRange.Row & 0xff);
                                buffer8[2] = (byte)((cell.CellFormula.ArrayFormulaRange.Row & 0xff00) >> 8);
                                buffer8[3] = (byte)(cell.CellFormula.ArrayFormulaRange.Column & 0xff);
                                buffer8[4] = (byte)((cell.CellFormula.ArrayFormulaRange.Column & 0xff00) >> 8);
                            }
                            else
                            {
                                buffer8[1] = (byte)(cell.Row & 0xff);
                                buffer8[2] = (byte)((cell.Row & 0xff00) >> 8);
                                buffer8[3] = (byte)(cell.Column & 0xff);
                                buffer8[4] = (byte)((cell.Column & 0xff00) >> 8);
                            }
                            flag4 = true;
                            formulaTokenBits = buffer8;
                        }
                    }
                    if ((formulaTokenBits == null) || (formulaTokenBits.Length == 0))
                    {
                        formulaTokenBits = new byte[] { 0x1c, 0x1d };
                    }
                    biff.RecordType = BiffRecordNumber.FORMULA2;
                    int num22 = 0x16 + formulaTokenBits.Length;
                    if ((formulaTokenExtraBits != null) && !flag4)
                    {
                        num22 += formulaTokenExtraBits.Length;
                    }
                    biff.DataLength = (short) num22;
                    biff.Write(writer);
                    writer.Write(num18);
                    writer.Write(num19);
                    writer.Write(num17);
                    bool flag5 = false;
                    if (description == null)
                    {
                        goto Label_0DDD;
                    }
                    if (description is bool)
                    {
                        writer.Write((byte) 1);
                        writer.Write((byte) 0);
                        writer.Write(((bool) description) ? ((byte) 1) : ((byte) 0));
                        writer.Write((byte) 0);
                        writer.Write((byte) 0);
                        writer.Write((byte) 0);
                        writer.Write((ushort) 0xffff);
                        goto Label_0E15;
                    }
                    if (double.TryParse(description.ToString(), out num23))
                    {
                        writer.Write(num23);
                        goto Label_0E15;
                    }
                    if (!(description is DateTime))
                    {
                        goto Label_0C79;
                    }
                    double result = 0.0;
                    if (DateTime.TryParse(description.ToString(), out time))
                    {
                        try
                        {
                            result = time.ToOADate();
                            goto Label_0C6C;
                        }
                        catch (OverflowException)
                        {
                            double.TryParse(description.ToString(), out result);
                            goto Label_0C6C;
                        }
                    }
                    double.TryParse(description.ToString(), out result);
                Label_0C6C:
                    writer.Write(result);
                    goto Label_0E15;
                Label_0C79:
                    if (description is TimeSpan)
                    {
                        DateTime @this = new DateTime(0x76b, 12, 30);
                        TimeSpan span = (TimeSpan) description;
                        double num25 = 0.0;
                        if (span.Ticks >= 0L)
                        {
                            @this = @this.Add(span);
                            try
                            {
                                num25 = @this.ToOADate();
                            }
                            catch (OverflowException)
                            {
                                double.TryParse(description.ToString(), out num25);
                            }
                        }
                        writer.Write(num25);
                    }
                    else if (this._excelWriter.IsCalculationError(description, ref errorVal))
                    {
                        if (errorVal != 0xff)
                        {
                            writer.Write((byte) 2);
                            writer.Write((byte) 0);
                            writer.Write((byte) ((byte) errorVal));
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((ushort) 0xffff);
                        }
                        else
                        {
                            flag5 = true;
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write((ushort) 0xffff);
                        }
                    }
                    else if (description is string)
                    {
                        if (!string.IsNullOrEmpty((string) (description as string)))
                        {
                            flag5 = true;
                            writer.Write((byte) 0);
                            writer.Write((byte) 0);
                            writer.Write(0);
                            writer.Write((ushort) 0xffff);
                        }
                        else
                        {
                            writer.Write((byte) 3);
                            writer.Write((byte) 0);
                            writer.Write(0);
                            writer.Write((ushort) 0xffff);
                        }
                    }
                    goto Label_0E15;
                Label_0DDD:
                    flag5 = true;
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                    writer.Write((ushort) 0xffff);
                Label_0E15:
                    num26 = 0;
                    writer.Write(num26);
                    writer.Write((uint) 0);
                    short length = 0;
                    length = (short)formulaTokenBits.Length;
                    writer.Write(length);
                    writer.Write(formulaTokenBits, 0, formulaTokenBits.Length);
                    if ((flag4 && (formulaTokenExtraBits != null)) && (formulaTokenExtraBits.Length > 0))
                    {
                        if (((cell.CellFormula != null) && (cell.CellFormula.ArrayFormulaRange != null)) && ((cell.CellFormula.ArrayFormulaRange.Row == num18) && (cell.CellFormula.ArrayFormulaRange.Column == num19)))
                        {
                            MemoryStream stream = new MemoryStream();
                            biff.RecordType = BiffRecordNumber.ARRAY;
                            num22 = formulaTokenExtraBits.Length + 14;
                            if (buffer3 != null)
                            {
                                num22 = buffer3.Length + num22;
                            }
                            biff.DataLength = (short) num22;
                            biff.Write(writer);
                            writer.Write((short) ((short) cell.CellFormula.ArrayFormulaRange.Row));
                            writer.Write((short) ((cell.CellFormula.ArrayFormulaRange.Row + cell.CellFormula.ArrayFormulaRange.RowSpan) - 1));
                            writer.Write((byte) ((byte) cell.CellFormula.ArrayFormulaRange.Column));
                            writer.Write((byte) ((cell.CellFormula.ArrayFormulaRange.Column + cell.CellFormula.ArrayFormulaRange.ColumnSpan) - 1));
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write((short) formulaTokenExtraBits.Length);
                            writer.Write(formulaTokenExtraBits);
                            if (buffer3 != null)
                            {
                                writer.Write(buffer3);
                            }
                            stream.Close();
                        }
                    }
                    else if (formulaTokenExtraBits != null)
                    {
                        writer.Write(formulaTokenExtraBits);
                    }
                    if (flag5)
                    {
                        MemoryStream stream2 = new MemoryStream();
                        biff.RecordType = BiffRecordNumber.STRING;
                        this.WriteBiffStr(new BinaryWriter((Stream) stream2), (string) (description as string), false, false, false, false, 2);
                        byte[] buffer = stream2.GetBuffer();
                        int count = 0x201c;
                        if (buffer.Length < count)
                        {
                            biff.DataLength = (short) stream2.Length;
                            biff.Write(writer);
                            writer.Write(buffer, 0, (int) stream2.Length);
                            stream2.Close();
                        }
                        else
                        {
                            int num29 = count;
                            biff.DataLength = (short) count;
                            biff.Write(writer);
                            byte[] buffer11 = new byte[num29];
                            Array.Copy(buffer, buffer11, num29);
                            writer.Write(buffer11, 0, num29);
                            int num30 = buffer.Length - num29;
                            int num31 = 1;
                            num29 = count - 1;
                            for (int i = count; num30 > num29; i += num29)
                            {
                                new BiffRecord { RecordType = BiffRecordNumber.CONTINUE, DataLength = (short) count }.Write(writer);
                                buffer11 = new byte[count];
                                Array.Copy(buffer, i, buffer11, 1, num29);
                                writer.Write(buffer11, 0, count);
                                num31++;
                                num30 -= num29;
                            }
                            new BiffRecord { RecordType = BiffRecordNumber.CONTINUE, DataLength = (short)(num30 + 1) }.Write(writer);
                            buffer11 = new byte[num30 + 1];
                            Array.Copy(buffer, (num29 * num31) + 1, buffer11, 1, num30);
                            writer.Write(buffer11, 0, num30 + 1);
                            stream2.Close();
                        }
                    }
                    continue;
                Label_112E:
                    if ((description != null) && (cell.CellType == CellType.String))
                    {
                        biff.RecordType = BiffRecordNumber.LABELSST;
                        biff.DataLength = 10;
                        biff.Write(writer);
                        writer.Write(num18);
                        writer.Write(num19);
                        writer.Write(num17);
                        writer.Write(this.AddUniqueString((string) (description as string)));
                        continue;
                    }
                    if ((cell.CellType != CellType.Numeric) && (cell.CellType != CellType.Datetime))
                    {
                        goto Label_1329;
                    }
                    if (description is string)
                    {
                        if (cell.CellType == CellType.Datetime)
                        {
                            try
                            {
                                DateTime time3;
                                DateTime.TryParse((string) (description as string), out time3);
                                description = (double) time3.ToOADate();
                                goto Label_11FF;
                            }
                            catch (OverflowException)
                            {
                                goto Label_11FF;
                            }
                        }
                        if (cell.CellType == CellType.Numeric)
                        {
                            double num33 = 0.0;
                            double.TryParse((string) (description as string), out num33);
                            description = (double) num33;
                        }
                    }
                Label_11FF:
                    if (description is DateTime)
                    {
                        DateTime time4 = (DateTime) description;
                        try
                        {
                            description = (double) time4.ToOADate();
                            goto Label_12E6;
                        }
                        catch
                        {
                            description = time4.ToShortDateString();
                            biff.RecordType = BiffRecordNumber.LABELSST;
                            biff.DataLength = 10;
                            biff.Write(writer);
                            writer.Write(num18);
                            writer.Write(num19);
                            writer.Write(num17);
                            writer.Write(this.AddUniqueString((string) (description as string)));
                            continue;
                        }
                    }
                    if (description is TimeSpan)
                    {
                        DateTime time5 = new DateTime(0x76b, 12, 30);
                        TimeSpan span2 = (TimeSpan) description;
                        if (span2.Ticks >= 0L)
                        {
                            time5 = time5.Add(span2);
                            try
                            {
                                description = (double) time5.ToOADate();
                            }
                            catch (OverflowException)
                            {
                                double num34 = 0.0;
                                double.TryParse(description.ToString(), out num34);
                                description = (double) num34;
                            }
                        }
                    }
                Label_12E6:
                    num35 = 0.0;
                    if (description is double)
                    {
                        num35 = (double) ((double) description);
                    }
                    else
                    {
                        double.TryParse(description.ToString(), out num35);
                    }
                    this.WriteNumber(writer, biff, num35, num18, num19, num17);
                    continue;
                Label_1329:
                    if (cell.CellType == CellType.Boolean)
                    {
                        bool flag6;
                        bool.TryParse(cell.Value.ToString(), out flag6);
                        byte num36 = flag6 ? ((byte) 1) : ((byte) 0);
                        biff.RecordType = BiffRecordNumber.BOOLERR;
                        biff.DataLength = 8;
                        biff.Write(writer);
                        writer.Write(num18);
                        writer.Write(num19);
                        writer.Write(num17);
                        writer.Write(num36);
                        writer.Write((byte) 0);
                    }
                    else
                    {
                        if (cell.CellType == CellType.Error)
                        {
                            this._excelWriter.IsCalculationError(description, ref errorVal);
                            biff.RecordType = BiffRecordNumber.BOOLERR;
                            biff.DataLength = 8;
                            biff.Write(writer);
                            writer.Write(num18);
                            writer.Write(num19);
                            writer.Write(num17);
                            writer.Write((byte) ((byte) errorVal));
                            writer.Write((byte) 1);
                            continue;
                        }
                        if (description == null)
                        {
                            biff.RecordType = BiffRecordNumber.BLANK;
                            biff.DataLength = 6;
                            biff.Write(writer);
                            writer.Write(num18);
                            writer.Write(num19);
                            writer.Write(num17);
                        }
                        else
                        {
                            biff.RecordType = BiffRecordNumber.BLANK;
                            biff.DataLength = 6;
                            biff.Write(writer);
                            writer.Write(num18);
                            writer.Write(num19);
                            writer.Write(num17);
                        }
                    }
                }
                catch (ExcelException exception7)
                {
                    if (exception7.Code == ExcelExceptionCode.ParseException)
                    {
                        this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaParseError"), (object[]) new object[] { exception7.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception7));
                    }
                    else if (exception7.Code == ExcelExceptionCode.FormulaError)
                    {
                        this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellFormulaError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception7));
                    }
                    else
                    {
                        this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception7));
                    }
                }
                catch (NotSupportedException exception8)
                {
                    this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeFormulaNotSupported"), (object[]) new object[] { exception8.Message }), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, (Exception) exception8));
                }
                catch (Exception exception9)
                {
                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheetIndex, cell.Row, cell.Column, exception9));
                }
            }
        }

        private void ProcessConditionalFormat(BinaryWriter writer, short sheet)
        {
            List<ConditionalFormatExtension> list = new List<ConditionalFormatExtension>();
            List<IExcelConditionalFormat> conditionalFormat = this._excelWriter.GetConditionalFormat(sheet);
            if (conditionalFormat != null)
            {
                for (int i = 0; i < conditionalFormat.Count; i++)
                {
                    IExcelConditionalFormat format = conditionalFormat[i];
                    if ((format.Ranges != null) && (format.Ranges.Count != 0))
                    {
                        try
                        {
                            if (this.IsValidConditionalFormating(format))
                            {
                                this.WriteConditionalFormatingRecord(writer, sheet, format);
                                for (int j = 0; j < format.ConditionalFormattingRules.Count; j++)
                                {
                                    IExcelConditionalFormatRule rule = format.ConditionalFormattingRules[j];
                                    int comparisonOperator = 0;
                                    int differentialFormattingId = -1;
                                    if (rule is IExcelGeneralRule)
                                    {
                                        differentialFormattingId = (rule as IExcelGeneralRule).DifferentialFormattingId;
                                        if ((rule as IExcelGeneralRule).Operator.HasValue)
                                        {
                                            comparisonOperator = (int)(rule as IExcelGeneralRule).Operator.Value;
                                        }
                                    }
                                    else if (rule is IExcelHighlightingRule)
                                    {
                                        differentialFormattingId = (rule as IExcelHighlightingRule).DifferentialFormattingId;
                                        comparisonOperator = (int) (rule as IExcelHighlightingRule).ComparisonOperator;
                                    }
                                    if (differentialFormattingId >= 0)
                                    {
                                        if ((this.Dxfs != null) && (this.Dxfs.Count > differentialFormattingId))
                                        {
                                            ConditionalFormatExtension extension = this.CreateConditionalFormatExtension(this.Dxfs[differentialFormattingId], rule);
                                            if (extension != null)
                                            {
                                                extension.ComparisionOperator = (byte) comparisonOperator;
                                                extension.Identifier = format.Identifier;
                                                extension.RuleId = (short) j;
                                                list.Add(extension);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ConditionalFormatExtension extension2 = this.CreateConditionalFormatExtension(new DifferentialFormatting(), rule);
                                        if (extension2 != null)
                                        {
                                            extension2.ComparisionOperator = (byte) comparisonOperator;
                                            extension2.Identifier = format.Identifier;
                                            extension2.RuleId = (short) j;
                                            list.Add(extension2);
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeConditionalFormatError"), ExcelWarningCode.General, sheet, format.Ranges[0].Row, format.Ranges[0].Column, exception));
                        }
                    }
                }
            }
            foreach (ConditionalFormatExtension extension3 in list)
            {
                this.WriteConditionalFormatExtensionRecord(writer, sheet, extension3, conditionalFormat);
            }
        }

        private void ProcessHyperLinks(BinaryWriter writer, short sheet)
        {
            if ((this._cellsHasHyperlink != null) && (this._cellsHasHyperlink.Count > 0))
            {
                byte[] buffer = new byte[] { 0xd0, 0xc9, 0xea, 0x79, 0xf9, 0xba, 0xce, 0x11, 140, 130, 0, 170, 0, 0x4b, 0xa9, 11 };
                foreach (IExcelCell cell in this._cellsHasHyperlink)
                {
                    BiffRecord record = new BiffRecord {
                        RecordType = BiffRecordNumber.HLINK
                    };
                    int num = 0;
                    IExcelHyperLink hyperlink = cell.Hyperlink;
                    if (hyperlink != null)
                    {
                        switch (hyperlink.Type)
                        {
                            case HyperLinkType.URL:
                                goto Label_0090;
                        }
                    }
                    continue;
                Label_0090:
                    num = 60;
                    if (!string.IsNullOrWhiteSpace(hyperlink.Description))
                    {
                        num += hyperlink.Description.Length * 2;
                    }
                    if (!string.IsNullOrWhiteSpace(hyperlink.Address))
                    {
                        num += hyperlink.Address.Length * 2;
                    }
                    record.DataLength = (short) num;
                    record.Write(writer);
                    writer.Write((ushort) ((ushort) cell.Row));
                    writer.Write((ushort) ((ushort) cell.Row));
                    writer.Write((ushort) ((ushort) cell.Column));
                    writer.Write((ushort) ((ushort) cell.Column));
                    writer.Write(buffer);
                    writer.Write(2);
                    int num2 = 0;
                    if (!string.IsNullOrWhiteSpace(hyperlink.Description))
                    {
                        num2 |= 0x10;
                    }
                    if (!string.IsNullOrWhiteSpace(hyperlink.Address))
                    {
                        num2 |= 1;
                    }
                    writer.Write(num2);
                    if (!string.IsNullOrWhiteSpace(hyperlink.Description))
                    {
                        writer.Write((int) (hyperlink.Description.Length + 1));
                        writer.Write(Encoding.Unicode.GetBytes(hyperlink.Description));
                        writer.Write((short) 0);
                    }
                    else
                    {
                        writer.Write(2);
                        writer.Write((short) 0);
                    }
                    byte[] buffer2 = new byte[] { 0xe0, 0xc9, 0xea, 0x79, 0xf9, 0xba, 0xce, 0x11, 140, 130, 0, 170, 0, 0x4b, 0xa9, 11 };
                    writer.Write(buffer2);
                    if (!string.IsNullOrWhiteSpace(hyperlink.Address))
                    {
                        writer.Write((int) ((hyperlink.Address.Length * 2) + 2));
                        writer.Write(Encoding.Unicode.GetBytes(hyperlink.Address));
                        writer.Write((short) 0);
                    }
                    else
                    {
                        writer.Write(1);
                        writer.Write((short) 0);
                    }
                }
            }
        }

        private void ProcessMergedCells(BinaryWriter writer, short sheet)
        {
            List<IRange> mergedCells = this._excelWriter.GetMergedCells(sheet);
            int num = 0;
            if ((mergedCells != null) && (mergedCells.Count > 0))
            {
                List<IRange> list2 = Enumerable.ToList<IRange>(Enumerable.Take<IRange>((IEnumerable<IRange>) (from item in (IEnumerable<IRange>) mergedCells select item), 0x3e8));
                num += 0x3e8;
                while (true)
                {
                    new BiffRecord { RecordType = BiffRecordNumber.MERGECELLS, DataLength = (short)(2 + (8 * Enumerable.Count<IRange>((IEnumerable<IRange>) list2))) }.Write(writer);
                    writer.Write((short) ((short) Enumerable.Count<IRange>((IEnumerable<IRange>) list2)));
                    foreach (IRange range in list2)
                    {
                        writer.Write((ushort) ((ushort) range.Row));
                        writer.Write((ushort) ((ushort) Math.Min((range.Row + range.RowSpan) - 1, 0xffff)));
                        writer.Write((ushort) ((ushort) range.Column));
                        writer.Write((ushort) ((ushort) Math.Min((range.Column + range.ColumnSpan) - 1, 0xff)));
                    }
                    if (mergedCells.Count < num)
                    {
                        break;
                    }
                    list2 = Enumerable.ToList<IRange>(Enumerable.Take<IRange>((IEnumerable<IRange>) (from item in Enumerable.Skip<IRange>((IEnumerable<IRange>) mergedCells, num) select item), 0x3e8));
                    num += 0x3e8;
                }
            }
        }

        private void ProcessNames(ref MemoryStream nameStream)
        {
            nameStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter((Stream) nameStream);
            this.WriteNames(writer);
        }

        private void ProcessSelection(BinaryWriter writer, short sheet)
        {
            BiffRecord record = new BiffRecord();
            List<GcRect> selectionList = new List<GcRect>();
            GcPoint activeCell = new GcPoint();
            PaneType topLeft = PaneType.TopLeft;
            short num = 0;
            short num2 = 0;
            short num3 = 1;
            for (int i = 3; i >= 0; i--)
            {
                topLeft = (PaneType) i;
                selectionList.Clear();
                if (this._excelWriter.GetSelectionList(sheet, selectionList, ref activeCell, ref topLeft))
                {
                    activeCell.X = Math.Min(activeCell.X, 255.0);
                    activeCell.Y = Math.Min(activeCell.Y, 65535.0);
                    record.RecordType = BiffRecordNumber.SELECTION;
                    record.DataLength = (short)(9 + (6 * Math.Max(1, selectionList.Count)));
                    record.Write(writer);
                    writer.Write((byte) ((byte) topLeft));
                    num3 = (short) selectionList.Count;
                    if (num3 == 0)
                    {
                        if ((activeCell.X >= 0.0) && (activeCell.Y >= 0.0))
                        {
                            selectionList.Add(new GcRect(activeCell.X, activeCell.Y, 1.0, 1.0));
                            num3 = 1;
                            num = 0;
                        }
                    }
                    else
                    {
                        num2 = 0;
                        while (num2 < num3)
                        {
                            GcRect rect = selectionList[num2];
                            if ((rect.Left == -1.0) || (rect.Width == -1.0))
                            {
                                rect = new GcRect(0.0, rect.Top, 256.0, rect.Height);
                            }
                            if ((rect.Top == -1.0) || (rect.Height == -1.0))
                            {
                                rect = new GcRect(rect.Left, 0.0, rect.Width, 65536.0);
                            }
                            if (((activeCell.X >= rect.Left) && (activeCell.X <= ((rect.Left + rect.Width) - 1.0))) && ((activeCell.Y >= rect.Top) && (activeCell.Y <= ((rect.Top + rect.Height) - 1.0))))
                            {
                                num = num2;
                                break;
                            }
                            num2++;
                        }
                    }
                    writer.Write((ushort) ((ushort) activeCell.Y));
                    writer.Write((short) ((short) activeCell.X));
                    writer.Write(num);
                    writer.Write(num3);
                    for (num2 = 0; num2 < num3; num2++)
                    {
                        GcRect rect2 = selectionList[num2];
                        if ((rect2.Left == -1.0) || (rect2.Width == -1.0))
                        {
                            rect2 = new GcRect(-1.0, rect2.Top, -1.0, rect2.Height);
                        }
                        if ((rect2.Top == -1.0) || (rect2.Height == -1.0))
                        {
                            rect2 = new GcRect(rect2.Left, -1.0, rect2.Width, -1.0);
                        }
                        writer.Write((ushort) ((ushort) Math.Min(rect2.Top, 65535.0)));
                        double num5 = (rect2.Top + rect2.Height) - 1.0;
                        writer.Write((ushort) ((ushort) Math.Min(num5, 65535.0)));
                        writer.Write((byte) ((byte) Math.Min(rect2.Left, 255.0)));
                        double num6 = (rect2.Left + rect2.Width) - 1.0;
                        writer.Write((byte) ((byte) Math.Min(num6, 255.0)));
                    }
                }
            }
        }

        /// <summary>
        /// ProcessSheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        internal bool ProcessSheet(short sheet, MemoryStream stream)
        {
            BinaryWriter writer = new BinaryWriter((Stream) stream);
            BiffRecord biff = new BiffRecord();
            bool flag = false;
            if (this._excelWriter is IExcelLosslessWriter)
            {
                IExcelLosslessWriter writer2 = this._excelWriter as IExcelLosslessWriter;
                if (writer2.GetSheetType(sheet) != ExcelSheetType.Worksheet)
                {
                    List<IUnsupportRecord> unsupportItems = writer2.GetUnsupportItems(sheet);
                    if (unsupportItems.Count > 0)
                    {
                        flag = true;
                        foreach (IUnsupportRecord record2 in unsupportItems)
                        {
                            if ((record2.Category == RecordCategory.TextRun) && (record2.Value is TXORuns))
                            {
                                TXORuns runs = record2.Value as TXORuns;
                                new BiffRecord { RecordType = BiffRecordNumber.CONTINUE, DataLength = (short) runs.Size }.Write(writer);
                                runs.Write(writer);
                                if (runs.ContinueRecords == null)
                                {
                                    continue;
                                }
                                using (List<BiffRecord>.Enumerator enumerator2 = runs.ContinueRecords.GetEnumerator())
                                {
                                    while (enumerator2.MoveNext())
                                    {
                                        enumerator2.Current.Write(writer);
                                    }
                                    continue;
                                }
                            }
                            if (record2.Value is BiffRecord)
                            {
                                (record2.Value as BiffRecord).Write(writer);
                            }
                            else if (record2.Value is List<BiffRecord>)
                            {
                                List<BiffRecord> list3 = record2.Value as List<BiffRecord>;
                                using (List<BiffRecord>.Enumerator enumerator3 = list3.GetEnumerator())
                                {
                                    while (enumerator3.MoveNext())
                                    {
                                        enumerator3.Current.Write(writer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                biff.RecordType = BiffRecordNumber.BOF;
                this.WriteBOF(writer, biff, false);
                biff.RecordType = BiffRecordNumber.INDEX;
                biff.DataLength = 0x10;
                this.WriteRecord(writer, biff, sheet);
                this.WriteCalculationProperty(sheet, writer, biff);
                this.WritePrintPageSetup(sheet, writer, biff);
                this.WriteWorksheetPrintMargin(sheet, writer);
                biff.RecordType = BiffRecordNumber.SETUP;
                biff.DataLength = 0x22;
                this.WriteRecord(writer, biff, sheet);
                biff.RecordType = BiffRecordNumber.PROTECT;
                biff.DataLength = 2;
                this.WriteRecord(writer, biff, sheet);
                biff.RecordType = BiffRecordNumber.DEFCOLWIDTH;
                biff.DataLength = 2;
                this.WriteRecord(writer, biff, sheet);
                this.WriteAutoFilter(writer, sheet);
                this.ProcessCells(writer, sheet);
                if (this._excelWriter is IExcelLosslessWriter)
                {
                    List<IUnsupportRecord> list4 = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(sheet);
                    if ((list4 != null) && (list4.Count > 0))
                    {
                        foreach (IUnsupportRecord record7 in list4)
                        {
                            if ((record7.FileType == ExcelFileType.XLS) && (record7.Category != RecordCategory.VBA))
                            {
                                if ((record7.Category == RecordCategory.Drawing) && (record7.Value is BiffRecord))
                                {
                                    BiffRecord record8 = record7.Value as BiffRecord;
                                    if (record8.RecordType == BiffRecordNumber.BOF)
                                    {
                                        biff.RecordType = BiffRecordNumber.BOF;
                                        biff.DataLength = 0x10;
                                        biff.Write(writer);
                                        writer.Write((ushort) 0x600);
                                        writer.Write((ushort) 0x20);
                                        writer.Write((ushort) 0x1faa);
                                        writer.Write((ushort) 0x7cd);
                                        writer.Write((ushort) 0x80c9);
                                        writer.Write((short) 1);
                                        writer.Write(0x406);
                                    }
                                    else
                                    {
                                        record8.Write(writer);
                                    }
                                }
                                else
                                {
                                    if ((record7.Category == RecordCategory.TextRun) && (record7.Value is TXORuns))
                                    {
                                        TXORuns runs2 = record7.Value as TXORuns;
                                        new BiffRecord { RecordType = BiffRecordNumber.CONTINUE, DataLength = (short) runs2.Size }.Write(writer);
                                        runs2.Write(writer);
                                        if (runs2.ContinueRecords == null)
                                        {
                                            continue;
                                        }
                                        using (List<BiffRecord>.Enumerator enumerator5 = runs2.ContinueRecords.GetEnumerator())
                                        {
                                            while (enumerator5.MoveNext())
                                            {
                                                enumerator5.Current.Write(writer);
                                            }
                                            continue;
                                        }
                                    }
                                    if ((record7.Category == RecordCategory.FontX) && (record7.Value is FontX))
                                    {
                                        FontX tx = record7.Value as FontX;
                                        new BiffRecord { RecordType = BiffRecordNumber.FONTX, DataLength = 2 }.Write(writer);
                                        tx.Write(writer);
                                    }
                                    else
                                    {
                                        Debugger.Break();
                                    }
                                }
                            }
                        }
                    }
                }
                biff.RecordType = BiffRecordNumber.WINDOW2;
                biff.DataLength = 0x12;
                this.WriteRecord(writer, biff, sheet);
                biff.RecordType = BiffRecordNumber.PANE;
                biff.DataLength = 10;
                this.WriteRecord(writer, biff, sheet);
                biff.RecordType = BiffRecordNumber.SCL;
                biff.DataLength = 4;
                this.WriteRecord(writer, biff, sheet);
                this.ProcessSelection(writer, sheet);
                BiffRecord unsupportRecord = this.GetUnsupportRecord(sheet, BiffRecordNumber.CODENAME);
                if (unsupportRecord != null)
                {
                    unsupportRecord.Write(writer);
                }
                this.ProcessHyperLinks(writer, sheet);
                this._cellsHasHyperlink.Clear();
                this.ProcessMergedCells(writer, sheet);
                this.ProcessConditionalFormat(writer, sheet);
                this.WriteDV(writer, sheet);
                this.WriteSheetTables(writer, sheet);
                this.WriteSheetTabColor(writer, sheet);
                biff.RecordType = BiffRecordNumber.EOF;
                biff.DataLength = 0;
                this.WriteEOF(writer, biff);
            }
            return true;
        }

        private void ProcessSST(MemoryStream sstStream)
        {
            if ((this._sstStringList != null) && (this._sstStringList.Count > 0))
            {
                BiffRecord record = new BiffRecord();
                List<object> list = new List<object>();
                int num = 0x2020;
                int num2 = 8;
                byte[] buffer = null;
                bool flag = false;
                int length = 0;
                int num5 = this._sstStringList.Count;
                MemoryStream @this = new MemoryStream();
                BinaryWriter writer = new BinaryWriter((Stream) @this);
                record.RecordType = BiffRecordNumber.SST;
                writer.Write(num5);
                writer.Write(num5);
                bool flag2 = true;
                for (int i = 0; i < num5; i++)
                {
                    buffer = this._sstStringList[i];
                    int index = 0;
                    flag = false;
                    length = buffer.Length;
                    while (index < length)
                    {
                        if (((num2 + length) - index) > num)
                        {
                            bool flag3 = true;
                            int num7 = num - num2;
                            int count = length;
                            if (num7 > 3)
                            {
                                MemoryStream stream2 = new MemoryStream(buffer);
                                BinaryReader reader = new BinaryReader((Stream) stream2);
                                reader.ReadInt16();
                                flag = (reader.ReadByte() & 1) == 1;
                                reader.Close();
                                flag3 = true;
                                count = Math.Min(length - index, num7);
                                if (flag && ((flag2 && (((count - 3) % 2) == 1)) || (!flag2 && ((count % 2) == 1))))
                                {
                                    count--;
                                }
                                writer.Write(buffer, index, count);
                                index += count;
                                num2 += count;
                            }
                            else
                            {
                                flag3 = false;
                            }
                            record.DataLength = (short) num2;
                            list.Add(record);
                            list.Add(@this.ToArray());
                            @this.Close();
                            @this = new MemoryStream();
                            writer = new BinaryWriter((Stream) @this);
                            record = new BiffRecord {
                                RecordType = BiffRecordNumber.CONTINUE
                            };
                            if (flag3)
                            {
                                num2 = 1;
                                writer.Write(flag ? ((byte) 1) : ((byte) 0));
                            }
                            else
                            {
                                num2 = 0;
                            }
                        }
                        else
                        {
                            int num10 = length - index;
                            writer.Write(buffer, index, num10);
                            index += num10;
                            num2 += num10;
                        }
                        flag2 = false;
                    }
                    flag2 = true;
                }
                record.DataLength = (short) num2;
                num2 = 0;
                list.Add(record);
                list.Add(@this.ToArray());
                @this.Close();
                writer.Close();
                BinaryWriter writer2 = new BinaryWriter((Stream) sstStream);
                foreach (object obj2 in list)
                {
                    if (obj2 is BiffRecord)
                    {
                        (obj2 as BiffRecord).Write(writer2);
                    }
                    else if (obj2 is byte[])
                    {
                        writer2.Write(obj2 as byte[], 0, (obj2 as byte[]).Length);
                    }
                }
                byte[] buffer2 = new byte[0x400];
                new BiffRecord { RecordType = BiffRecordNumber.EXTSST, DataLength = 0x402 }.Write(writer2);
                writer2.Write((short) 1);
                writer2.Write(buffer2, 0, 0x400);
            }
        }

        private ExcelSupBook ReadSupBook(byte[] buffer)
        {
            SimpleBinaryReader reader = new SimpleBinaryReader(buffer);
            ushort num = reader.ReadUInt16();
            ushort charCount = reader.ReadUInt16();
            string outString = null;
            string[] strArray = new string[num];
            ExcelSupBook book = null;
            if ((charCount > 1) && (charCount <= 0xff))
            {
                reader.ReadUncompressedString(charCount, out outString);
                for (int i = 0; i < num; i++)
                {
                    string str2;
                    reader.ReadCompressedString(2, out str2);
                    strArray[i] = str2;
                }
            }
            switch (charCount)
            {
                case 0x401:
                    book = new ExcelSupBook(true) {
                        SheetCount = num,
                        SheetNames = this.SheetNames
                    };
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
                    if (outString != null)
                    {
                        book.FileName = SimpleBinaryReader.DecodeText(outString);
                    }
                    if ((strArray != null) && (strArray.Length > 0))
                    {
                        for (int j = 0; j < strArray.Length; j++)
                        {
                            book.SheetNames.Add(strArray[j]);
                        }
                    }
                    break;
            }
            book.Buffer = buffer;
            return book;
        }

        private void Reset()
        {
            this._sstStringList = new List<byte[]>();
            this._uniqueStringTable = new Dictionary<string, int>();
            this._linkTable = new LinkTable();
            this._mergedToSheetIndexList.Clear();
            this._formats = null;
            this._sheetCount = 0;
            this._dxfs = null;
        }

        private bool TryGetBuiltInStyleIndex(List<IExcelStyle> styles, string styleName, out int index)
        {
            index = 0;
            for (int i = 0; i < styles.Count; i++)
            {
                if (styles[i].Name == styleName)
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        private short TryGetStyleID(int id)
        {
            if (id < 0)
            {
                return 15;
            }
            return (short) (id + this._offSet);
        }

        private void UpdateDXFExtendedProperty(string key, IExcelColor color, IDifferentialFormatting dxfExt)
        {
            if (((color != null) && !color.IsIndexedColor) && ((color.ColorType != ExcelColorType.RGB) || (color.Value != 0)))
            {
                dxfExt.IsDFXExten = true;
                dxfExt.ExtendedPropertyList.Add(new Tuple<string, object>(key, color));
            }
        }

        private void WriteAutoFilter(BinaryWriter writer, short sheet)
        {
            IExcelAutoFilter autoFilter = this._excelWriter.GetAutoFilter(sheet);
            try
            {
                if ((autoFilter != null) && (autoFilter.Range != null))
                {
                    int num = Math.Min(autoFilter.Range.RowSpan, 0x10000);
                    int num2 = Math.Min(autoFilter.Range.ColumnSpan, 0x100);
                    Tuple<short, short> startRowColumnPair = new Tuple<short, short>((short) autoFilter.Range.Column, (short) autoFilter.Range.Row);
                    Tuple<short, short> endRowColumnPair = new Tuple<short, short>((short) ((autoFilter.Range.Column + num2) - 1), (short) ((autoFilter.Range.Row + num) - 1));
                    int columnSpan = autoFilter.Range.ColumnSpan;
                    if (columnSpan == 0x4000)
                    {
                        columnSpan = 0x100;
                    }
                    if (autoFilter.FilterColumns != null)
                    {
                        BiffRecord record2 = new BiffRecord {
                            RecordType = BiffRecordNumber.FILTERMODE,
                            DataLength = 0
                        };
                        record2.Write(writer);
                    }
                    BiffRecord record3 = new BiffRecord {
                        RecordType = BiffRecordNumber.AUTOFILTERINFO,
                        DataLength = 2
                    };
                    record3.Write(writer);
                    writer.Write((short) ((short) columnSpan));
                    if (autoFilter.FilterColumns != null)
                    {
                        foreach (IExcelFilterColumn column in autoFilter.FilterColumns)
                        {
                            if ((column.AutoFilterColumnId + autoFilter.Range.Column) < 0x100L)
                            {
                                this.WriteAutoFilterColumn(writer, column, startRowColumnPair, endRowColumnPair, false, 0);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeAutoFilterError"), ExcelWarningCode.General, sheet, -1, -1, exception));
            }
        }

        private void WriteAutoFilterColumn(BinaryWriter writer, IExcelFilterColumn filterColumn, Tuple<short, short> startRowColumnPair, Tuple<short, short> endRowColumnPair, bool isTableFilter = false, int tableIdentifier = 0)
        {
            BiffRecord record = null;
            byte[] buffer6;
            IExcelCustomFilters customFilters = filterColumn.CustomFilters;
            IExcelTop10Filter filter = filterColumn.Top10;
            List<IDifferentialFormatting> dxfs = this.Dxfs;
            if (!isTableFilter)
            {
                if ((customFilters != null) || (filter != null))
                {
                    record = new BiffRecord {
                        RecordType = BiffRecordNumber.AUTOFILTER
                    };
                    byte[] buffer = this.GetAutoFilterTop10Buffer(filterColumn, customFilters, filter);
                    record.DataLength = (short)buffer.Length;
                    record.Write(writer);
                    writer.Write(buffer);
                }
                else
                {
                    record = new BiffRecord {
                        RecordType = BiffRecordNumber.AUTOFILTER
                    };
                    byte[] autoFilterCustomBuffer = this.GetAutoFilterCustomBuffer(filterColumn);
                    record.DataLength = (short)autoFilterCustomBuffer.Length;
                    record.Write(writer);
                    writer.Write(autoFilterCustomBuffer);
                }
            }
            uint num = 0;
            uint num2 = 0;
            uint num3 = 0;
            if (((filterColumn.DynamicFilter != null) || (filterColumn.ColorFilter != null)) || (filterColumn.IconFilter != null))
            {
                byte[] buffer3;
                if ((filterColumn.DynamicFilter == null) && ((filterColumn.ColorFilter != null) || (filterColumn.IconFilter != null)))
                {
                    ExcelDynamicFilter filter2 = new ExcelDynamicFilter {
                        Type = ExcelDynamicFilterType.Null
                    };
                    filterColumn.DynamicFilter = filter2;
                }
                using (MemoryStream stream = new MemoryStream())
                {
                    using (BinaryWriter writer2 = new BinaryWriter((Stream) stream))
                    {
                        writer2.Write((short) 0x87e);
                        writer2.Write((short) 1);
                        writer2.Write(startRowColumnPair.Item2);
                        writer2.Write(endRowColumnPair.Item2);
                        writer2.Write(startRowColumnPair.Item1);
                        writer2.Write(endRowColumnPair.Item1);
                        writer2.Write((short) ((short) filterColumn.AutoFilterColumnId));
                        writer2.Write(0);
                        uint type = (uint) filterColumn.DynamicFilter.Type;
                        if (filterColumn.ColorFilter != null)
                        {
                            num = (uint)(filterColumn.ColorFilter.CellColor ? 1 : 2);
                        }
                        if (filterColumn.IconFilter != null)
                        {
                            num = 3;
                        }
                        if (num == 0)
                        {
                            if (filterColumn.CustomFilters != null)
                            {
                                if (filterColumn.CustomFilters.Filter1 != null)
                                {
                                    num2++;
                                }
                                if (filterColumn.CustomFilters.Filter2 != null)
                                {
                                    num2++;
                                }
                            }
                            if (filterColumn.Filters != null)
                            {
                                if (filterColumn.Filters.DateGroupItem != null)
                                {
                                    num3 = (uint) filterColumn.Filters.DateGroupItem.Count;
                                }
                                if (filterColumn.Filters.Filter != null)
                                {
                                    num2 += (uint)filterColumn.Filters.Filter.Count;
                                }
                            }
                            if ((this.IsValidDynamicFilter(filterColumn.DynamicFilter) && (filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.AboveAverage)) || (filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.BelowAverage))
                            {
                                num2++;
                            }
                            else if (this.IsValidDynamicFilter(filterColumn.DynamicFilter))
                            {
                                num2 += 2;
                            }
                        }
                        writer2.Write(num);
                        writer2.Write(type);
                        writer2.Write(num2);
                        if (isTableFilter)
                        {
                            writer2.Write((uint) uint.MaxValue);
                        }
                        else
                        {
                            writer2.Write(num3);
                        }
                        if (isTableFilter)
                        {
                            writer2.Write((short) 0);
                        }
                        else
                        {
                            writer2.Write((short) 8);
                        }
                        writer2.Write((short) 0);
                        if (isTableFilter)
                        {
                            writer2.Write((ushort) 0xffff);
                        }
                        else
                        {
                            writer2.Write((ushort) 0);
                        }
                        if (isTableFilter)
                        {
                            writer2.Write(tableIdentifier);
                        }
                        else
                        {
                            writer2.Write((uint) uint.MaxValue);
                        }
                        writer2.Write(0);
                        writer2.Write(0);
                        writer2.Write(0);
                        writer2.Write(0);
                        if (num > 0)
                        {
                            if (((filterColumn.ColorFilter != null) && (dxfs != null)) && (dxfs.Count > filterColumn.ColorFilter.DxfId))
                            {
                                IDifferentialFormatting dxf = dxfs[(int) filterColumn.ColorFilter.DxfId];
                                this.WriteDXFRecord(writer2, dxf);
                                this.WriteXfExtNoFRTRecord(writer2, dxf);
                            }
                            if (filterColumn.IconFilter != null)
                            {
                                writer2.Write((int) filterColumn.IconFilter.IconSet);
                                writer2.Write(filterColumn.IconFilter.IconId);
                            }
                        }
                    }
                    buffer3 = stream.ToArray();
                }
                record = new BiffRecord {
                    RecordType = BiffRecordNumber.AUTOFILTER12,
                    DataLength = (short)buffer3.Length
                };
                record.Write(writer);
                writer.Write(buffer3);
            }
            if (this.IsValidDynamicFilter(filterColumn.DynamicFilter) && ((filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.AboveAverage) || (filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.BelowAverage)))
            {
                byte[] buffer4;
                using (MemoryStream stream2 = new MemoryStream())
                {
                    using (BinaryWriter writer3 = new BinaryWriter((Stream) stream2))
                    {
                        writer3.Write((short) 0x87f);
                        writer3.Write((short) 1);
                        writer3.Write(startRowColumnPair.Item2);
                        writer3.Write(endRowColumnPair.Item2);
                        writer3.Write(startRowColumnPair.Item1);
                        writer3.Write(endRowColumnPair.Item1);
                        byte op = 0;
                        if (filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.AboveAverage)
                        {
                            op = 4;
                        }
                        else if (filterColumn.DynamicFilter.Type == ExcelDynamicFilterType.BelowAverage)
                        {
                            op = 1;
                        }
                        this.WriteDoperNumber(writer3, (double) ((double) filterColumn.DynamicFilter.Value), op);
                    }
                    buffer4 = stream2.ToArray();
                }
                record = new BiffRecord {
                    RecordType = BiffRecordNumber.CONTINUEFRT12,
                    DataLength = (short)buffer4.Length
                };
                record.Write(writer);
                writer.Write(buffer4);
                num2--;
            }
            else if (this.IsValidDynamicFilter(filterColumn.DynamicFilter))
            {
                byte[] buffer5;
                using (MemoryStream stream3 = new MemoryStream())
                {
                    using (BinaryWriter writer4 = new BinaryWriter((Stream) stream3))
                    {
                        writer4.Write((short) 0x87f);
                        writer4.Write((short) 1);
                        writer4.Write(startRowColumnPair.Item2);
                        writer4.Write(endRowColumnPair.Item2);
                        writer4.Write(startRowColumnPair.Item1);
                        writer4.Write(endRowColumnPair.Item1);
                        byte num6 = 4;
                        this.WriteDoperNumber(writer4, double.Parse(filterColumn.DynamicFilter.Value.ToString()), num6);
                    }
                    buffer5 = stream3.ToArray();
                }
                record = new BiffRecord {
                    RecordType = BiffRecordNumber.CONTINUEFRT12,
                    DataLength = (short)buffer5.Length
                };
                record.Write(writer);
                writer.Write(buffer5);
                num2--;
                buffer5 = null;
                using (MemoryStream stream4 = new MemoryStream())
                {
                    using (BinaryWriter writer5 = new BinaryWriter((Stream) stream4))
                    {
                        writer5.Write((short) 0x87f);
                        writer5.Write((short) 1);
                        writer5.Write(startRowColumnPair.Item2);
                        writer5.Write(endRowColumnPair.Item2);
                        writer5.Write(startRowColumnPair.Item1);
                        writer5.Write(endRowColumnPair.Item1);
                        byte num7 = 1;
                        this.WriteDoperNumber(writer5, double.Parse(filterColumn.DynamicFilter.MaxValue.ToString()), num7);
                    }
                    buffer5 = stream4.ToArray();
                }
                BiffRecord record7 = new BiffRecord {
                    RecordType = BiffRecordNumber.CONTINUEFRT12,
                    DataLength = (short)buffer5.Length
                };
                record7.Write(writer);
                writer.Write(buffer5);
                num2--;
            }
            bool flag = true;
            bool flag2 = true;
            if (((num2 <= 0) || (filterColumn.Filters == null)) || (filterColumn.Filters.Filter == null))
            {
                goto Label_0897;
            }
            using (List<string>.Enumerator enumerator = filterColumn.Filters.Filter.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    int num8;
                    if (!int.TryParse(enumerator.Current, out num8))
                    {
                        flag = false;
                        goto Label_0722;
                    }
                }
            }
        Label_0722:
            if (flag)
            {
                goto Label_0897;
            }
            using (List<string>.Enumerator enumerator2 = filterColumn.Filters.Filter.GetEnumerator())
            {
                while (enumerator2.MoveNext())
                {
                    double num9;
                    if (!double.TryParse(enumerator2.Current, out num9))
                    {
                        flag2 = false;
                        break;
                    }
                }
                goto Label_0897;
            }
        Label_0772:
            using (MemoryStream stream5 = new MemoryStream())
            {
                using (BinaryWriter writer6 = new BinaryWriter((Stream) stream5))
                {
                    writer6.Write((short) 0x87f);
                    writer6.Write((short) 1);
                    writer6.Write(startRowColumnPair.Item2);
                    writer6.Write(endRowColumnPair.Item2);
                    writer6.Write(startRowColumnPair.Item1);
                    writer6.Write(endRowColumnPair.Item1);
                    if (flag)
                    {
                        this.WriteDoperRKNumber(writer6, int.Parse(filterColumn.Filters.Filter[(int) (num2 - 1)]), 2);
                    }
                    else if (flag2)
                    {
                        this.WriteDoperNumber(writer6, double.Parse(filterColumn.Filters.Filter[(int) (num2 - 1)]), 2);
                    }
                    else
                    {
                        this.WriteDoperString(writer6, filterColumn.Filters.Filter[(int) (num2 - 1)]);
                    }
                }
                buffer6 = stream5.ToArray();
            }
            record = new BiffRecord {
                RecordType = BiffRecordNumber.CONTINUEFRT12,
                DataLength = (short)buffer6.Length
            };
            record.Write(writer);
            writer.Write(buffer6);
            num2--;
        Label_0897:
            if (num2 > 0)
            {
                goto Label_0772;
            }
            while (num3 > 0)
            {
                byte[] buffer7;
                using (MemoryStream stream6 = new MemoryStream())
                {
                    using (BinaryWriter writer7 = new BinaryWriter((Stream) stream6))
                    {
                        writer7.Write((short) 0x87f);
                        writer7.Write((short) 1);
                        writer7.Write(startRowColumnPair.Item2);
                        writer7.Write(endRowColumnPair.Item2);
                        writer7.Write(startRowColumnPair.Item1);
                        writer7.Write(endRowColumnPair.Item1);
                        IExcelDateGroupItem item = filterColumn.Filters.DateGroupItem[(int) (num3 - 1)];
                        writer7.Write((short) ((short) item.Year));
                        writer7.Write((short) ((short) item.Month));
                        writer7.Write((uint) item.Day);
                        writer7.Write((short) ((short) item.Hour));
                        writer7.Write((short) ((short) item.Minute));
                        writer7.Write((short) ((short) item.Second));
                        writer7.Write((short) 0);
                        writer7.Write(0);
                        writer7.Write((int) item.DateTimeGrouping);
                    }
                    buffer7 = stream6.ToArray();
                }
                BiffRecord record9 = new BiffRecord {
                    RecordType = BiffRecordNumber.CONTINUEFRT12,
                    DataLength = (short)buffer7.Length
                };
                record9.Write(writer);
                writer.Write(buffer7);
                num3--;
            }
        }

        /// <summary>
        /// WriteBeginWorkbookRecords
        /// </summary>
        /// <returns></returns>
        internal bool WriteBeginWorkbookRecords(BinaryWriter writer)
        {
            BiffRecord biff = new BiffRecord();
            this.WriteBOF(writer, biff, true);
            if (this.IsEncrypted)
            {
                biff.RecordType = BiffRecordNumber.FILEPASS;
                biff.DataLength = 0x36;
                this.WriteFilePass(writer, biff);
            }
            biff.RecordType = BiffRecordNumber.INTERFACEHDR;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write((short) 0x4b0);
            biff.RecordType = BiffRecordNumber.MMS;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write((short) 0);
            biff.RecordType = BiffRecordNumber.INTERFACEEND;
            biff.DataLength = 0;
            biff.Write(writer);
            biff.RecordType = BiffRecordNumber.CODEPAGE;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write((short) 0x4b0);
            BiffRecord unsupportRecord = this.GetUnsupportRecord(-1, BiffRecordNumber.EXCEL9FILE);
            if (unsupportRecord != null)
            {
                unsupportRecord.Write(writer);
            }
            BiffRecord record3 = this.GetUnsupportRecord(-1, BiffRecordNumber.CODENAME);
            if (record3 != null)
            {
                record3.Write(writer);
            }
            biff.RecordType = BiffRecordNumber.PROTECT;
            biff.DataLength = 2;
            this.WriteProtect(writer, biff, -1);
            biff.RecordType = BiffRecordNumber.WINDOW1;
            biff.DataLength = 0x12;
            this.WriteWINDOW1(writer, biff);
            BiffRecord record4 = this.GetUnsupportRecord(-1, BiffRecordNumber.HIDEOBJ);
            if (record4 != null)
            {
                record4.Write(writer);
            }
            IExcelWorkbookPropery workbookProperty = this._excelWriter.GetWorkbookProperty();
            biff.RecordType = BiffRecordNumber.DATE1904;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write(workbookProperty.IsDate1904 ? ((short) 1) : ((short) 0));
            biff.RecordType = BiffRecordNumber.PRECISION;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write(this._calulationProperty.IsFullPrecision ? ((short) 1) : ((short) 0));
            biff.RecordType = BiffRecordNumber.BOOKBOOL;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write(workbookProperty.SaveExternalLinks ? ((short) 0) : ((short) 1));
            return true;
        }

        internal bool WriteBiffStr(BinaryWriter writer, string stringIn, bool useHighByteParam, bool highByte, bool extString, bool richString, short byteSizeOfCharCount)
        {
            short charCount = 0;
            byte grbit = 0;
            byte[] biffStrBuffer = null;
            StringConvert stringConvert = useHighByteParam ? (highByte ? StringConvert.Unicode : StringConvert.Ascii) : StringConvert.None;
            this.BuildBiffStrComponents(stringIn, stringConvert, ref charCount, ref grbit, ref biffStrBuffer);
            grbit |= (byte)((extString ? 4 : 0) | (richString ? 8 : 0));
            if ((byteSizeOfCharCount == 1) && (charCount > 0))
            {
                writer.Write((byte) ((byte) charCount));
            }
            else
            {
                writer.Write((ushort) ((ushort) charCount));
            }
            writer.Write(grbit);
            if ((biffStrBuffer != null) && (biffStrBuffer.Length > 0))
            {
                writer.Write(biffStrBuffer);
            }
            return true;
        }

        internal bool WriteBiffStrForDV(BinaryWriter writer, string stringIn, bool useHighByteParam, bool highByte, bool extString, bool richString, short byteSizeOfCharCount)
        {
            short charCount = 0;
            byte grbit = 0;
            byte[] biffStrBuffer = null;
            StringConvert stringConvert = useHighByteParam ? (highByte ? StringConvert.Unicode : StringConvert.Ascii) : StringConvert.None;
            this.BuildBiffStrComponents(stringIn, stringConvert, ref charCount, ref grbit, ref biffStrBuffer);
            if (biffStrBuffer == null)
            {
                charCount = 1;
                biffStrBuffer = new byte[1];
            }
            grbit |= (byte)((extString ? 4 : 0) | (richString ? 8 : 0));
            if ((byteSizeOfCharCount == 1) && (charCount > 0))
            {
                writer.Write((byte) ((byte) charCount));
            }
            else
            {
                writer.Write((ushort) ((ushort) charCount));
            }
            writer.Write(grbit);
            if ((biffStrBuffer != null) && (biffStrBuffer.Length > 0))
            {
                writer.Write(biffStrBuffer);
            }
            return true;
        }

        internal bool WriteBOF(BinaryWriter writer, BiffRecord biff, bool workbook)
        {
            biff.RecordType = BiffRecordNumber.BOF;
            biff.DataLength = 0x10;
            biff.Write(writer);
            writer.Write((ushort) 0x600);
            if (workbook)
            {
                writer.Write((ushort) 5);
            }
            else
            {
                writer.Write((ushort) 0x10);
            }
            writer.Write((ushort) 0x1faa);
            writer.Write((ushort) 0x7cd);
            writer.Write((ushort) 0x80c9);
            writer.Write((short) 1);
            writer.Write(0x406);
            return true;
        }

        internal bool WriteBoundSheet(BinaryWriter writer, BiffRecord biff, int offset, MemoryStream sheetNameStream, bool hidden, ExcelSheetType sheetType)
        {
            short num = 0;
            switch (sheetType)
            {
                case ExcelSheetType.MacroSheet:
                    num = 0x100;
                    break;

                case ExcelSheetType.ChartSheet:
                    num = 0x200;
                    break;

                case ExcelSheetType.VBAModule:
                    num = 0x600;
                    break;
            }
            if (hidden)
            {
                num |= 1;
            }
            biff.RecordType = BiffRecordNumber.BUNDLESHEET;
            if ((sheetNameStream != null) && (sheetNameStream.Length > 0L))
            {
                biff.DataLength += (short) sheetNameStream.Length;
            }
            else
            {
                biff.DataLength += 2;
            }
            biff.Write(writer);
            writer.Write((uint) offset);
            writer.Write(num);
            if ((sheetNameStream != null) && (sheetNameStream.Length > 0L))
            {
                byte[] buffer = new byte[sheetNameStream.Length];
                for (int i = 0; i < sheetNameStream.Length; i++)
                {
                    buffer[i] = sheetNameStream.GetBuffer()[i];
                }
                writer.Write(buffer);
            }
            else
            {
                writer.Write((ushort) 0);
            }
            return true;
        }

        internal void WriteCalcCount(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            if (this._calulationProperty.MaxIterationCount < 1)
            {
                this._calulationProperty.MaxIterationCount = 1;
            }
            if (this._calulationProperty.MaxIterationCount > 0x7fff)
            {
                this._calulationProperty.MaxIterationCount = 0x7fff;
            }
            biff.Write(writer);
            writer.Write((short) ((short) this._calulationProperty.MaxIterationCount));
        }

        internal void WriteCalcMode(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            writer.Write((short) ((short) this._calulationProperty.CalculationMode));
        }

        private void WriteCalculationProperty(short sheet, BinaryWriter writer, BiffRecord biff)
        {
            biff.RecordType = BiffRecordNumber.CALCMODE;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.CALCCOUNT;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.REFMODE;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.ITERATION;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.DELTA;
            biff.DataLength = 8;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.SAVERECALC;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
        }

        private void WriteCFColor(BinaryWriter writer, IExcelColor color)
        {
            if (color.IsIndexedColor)
            {
                writer.Write(1);
                writer.Write(color.Value);
                writer.Write((double) 0.0);
            }
            else if (color.IsRGBColor)
            {
                GcColor color2 = ColorExtension.FromArgb(color.Value);
                writer.Write(2);
                writer.Write(color2.R);
                writer.Write(color2.G);
                writer.Write(color2.B);
                writer.Write(color2.A);
                writer.Write(color.Tint);
            }
            else if (color.IsThemeColor)
            {
                writer.Write(3);
                writer.Write(color.Value);
                writer.Write(color.Tint);
            }
        }

        private void WriteConditionalFormatExtensionRecord(BinaryWriter writer, short sheet, ConditionalFormatExtension cfex, List<IExcelConditionalFormat> conditionalFormats)
        {
            BiffRecord record = new BiffRecord {
                RecordType = BiffRecordNumber.CFEX
            };
            int num = 0x1a;
            num += 0x11;
            if (cfex.HasDXF && (cfex.DXF != null))
            {
                int num2 = this.GetDXFN12RecrodSize(cfex.DXF);
                num += 4;
                if (num2 > 0)
                {
                    num += num2;
                }
                else
                {
                    num += 2;
                }
            }
            IExcelConditionalFormat format = null;
            foreach (IExcelConditionalFormat format2 in conditionalFormats)
            {
                if ((format2.Identifier == cfex.Identifier) && (format2.ConditionalFormattingRules.Count > cfex.RuleId))
                {
                    format = format2;
                    break;
                }
            }
            if (((format != null) && (format.Ranges != null)) && (format.Ranges.Count != 0))
            {
                record.DataLength = (short) num;
                record.Write(writer);
                IRange range = format.Ranges[0];
                try
                {
                    writer.Write((short) 0x87b);
                    writer.Write((short) 1);
                    this.WriteRange(writer, range);
                    writer.Write(0);
                    writer.Write(cfex.Identifier);
                    writer.Write(cfex.RuleId);
                    writer.Write(cfex.ComparisionOperator);
                    writer.Write(cfex.TemplateIndex);
                    writer.Write((short) ((short) cfex.Priority));
                    byte num3 = 9;
                    if (cfex.stopIfTrue)
                    {
                        num3 |= 2;
                    }
                    else
                    {
                        num3 = 0;
                    }
                    writer.Write(num3);
                    if (!cfex.HasDXF)
                    {
                        writer.Write((byte) 0);
                    }
                    else
                    {
                        writer.Write((byte) 1);
                        this.WriteDXFN12Record(writer, cfex.DXF);
                    }
                    writer.Write((byte) 0x10);
                    IExcelConditionalFormatRule rule = format.ConditionalFormattingRules[cfex.RuleId];
                    if (cfex.TemplateIndex == 5)
                    {
                        IExcelGeneralRule rule2 = rule as IExcelGeneralRule;
                        if (rule2 != null)
                        {
                            int num4 = 1;
                            if (rule2.Bottom.HasValue && rule2.Bottom.Value)
                            {
                                num4 = 0;
                            }
                            if (rule2.Percent.HasValue && rule2.Percent.Value)
                            {
                                num4 |= 2;
                            }
                            writer.Write((byte) ((byte) num4));
                            if (rule2.Rank.HasValue)
                            {
                                writer.Write((ushort) ((ushort) rule2.Rank.Value));
                            }
                            else
                            {
                                writer.Write((ushort) 0);
                            }
                            writer.Write(new byte[13]);
                        }
                        else
                        {
                            writer.Write(new byte[0x10]);
                        }
                    }
                    else if (cfex.TemplateIndex == 8)
                    {
                        ExcelConditionalFormattingOperator noComparison = ExcelConditionalFormattingOperator.NoComparison;
                        if (rule is IExcelGeneralRule)
                        {
                            if ((rule as IExcelGeneralRule).Operator.HasValue)
                            {
                                noComparison = (rule as IExcelGeneralRule).Operator.Value;
                            }
                        }
                        else if (rule is IExcelHighlightingRule)
                        {
                            noComparison = (rule as IExcelHighlightingRule).ComparisonOperator;
                        }
                        short num5 = 0;
                        switch (noComparison)
                        {
                            case ExcelConditionalFormattingOperator.ContainsText:
                                num5 = 0;
                                break;

                            case ExcelConditionalFormattingOperator.NotContains:
                                num5 = 1;
                                break;

                            case ExcelConditionalFormattingOperator.BeginsWith:
                                num5 = 2;
                                break;

                            case ExcelConditionalFormattingOperator.EndsWith:
                                num5 = 3;
                                break;
                        }
                        writer.Write(num5);
                        writer.Write(new byte[14]);
                    }
                    else if ((cfex.TemplateIndex == 0x19) || (cfex.TemplateIndex == 0x1a))
                    {
                        IExcelGeneralRule rule3 = rule as IExcelGeneralRule;
                        if (rule3 != null)
                        {
                            int num6 = 0;
                            if (rule3.StdDev.HasValue)
                            {
                                num6 = rule3.StdDev.Value;
                            }
                            writer.Write((short) ((short) num6));
                            writer.Write(new byte[14]);
                        }
                        else
                        {
                            writer.Write(new byte[0x10]);
                        }
                    }
                    else if ((cfex.TemplateIndex >= 15) && (cfex.TemplateIndex <= 0x18))
                    {
                        writer.Write((short) (cfex.TemplateIndex - 15));
                        writer.Write(new byte[14]);
                    }
                    else
                    {
                        writer.Write(new byte[0x10]);
                    }
                }
                catch (Exception exception)
                {
                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeConditionalFormatError"), ExcelWarningCode.General, sheet, format.Ranges[0].Row, format.Ranges[0].Column, exception));
                }
            }
        }

        private void WriteConditionalFormating(BinaryWriter writer, short sheet, IExcelConditionalFormat conditionalFormat)
        {
            if (((conditionalFormat != null) && (conditionalFormat.Ranges != null)) && (conditionalFormat.Ranges.Count != 0))
            {
                BiffRecord record = new BiffRecord {
                    RecordType = BiffRecordNumber.CONDFMT
                };
                IRange conditionalFormatRefBound = null;
                if (conditionalFormat.Ranges.Count == 1)
                {
                    conditionalFormatRefBound = conditionalFormat.Ranges[0];
                }
                else
                {
                    conditionalFormatRefBound = this.GetConditionalFormatRefBound(conditionalFormat.Ranges);
                }
                if (conditionalFormatRefBound != null)
                {
                    record.DataLength = (short)(14 + (conditionalFormat.Ranges.Count * 8));
                    record.Write(writer);
                    writer.Write((short) ((short) conditionalFormat.ConditionalFormattingRules.Count));
                    short num = (short)(conditionalFormat.Identifier << 1);
                    num |= 1;
                    writer.Write(num);
                    this.WriteRange(writer, conditionalFormatRefBound);
                    writer.Write((short) ((short) conditionalFormat.Ranges.Count));
                    for (int i = 0; i < conditionalFormat.Ranges.Count; i++)
                    {
                        this.WriteRange(writer, conditionalFormat.Ranges[i]);
                    }
                    List<IDifferentialFormatting> dxfs = this.Dxfs;
                    if (dxfs == null)
                    {
                        dxfs = new List<IDifferentialFormatting>();
                    }
                    foreach (IExcelConditionalFormatRule rule in conditionalFormat.ConditionalFormattingRules)
                    {
                        BiffRecord record2 = new BiffRecord {
                            RecordType = BiffRecordNumber.CF
                        };
                        short num3 = 12;
                        short length = 0;
                        short num5 = 0;
                        byte[] buffer = null;
                        byte[] buffer2 = null;
                        byte num6 = 0;
                        byte comparisonOperator = 0;
                        int differentialFormattingId = 0;
                        List<string> formulas = null;
                        if (rule is IExcelGeneralRule)
                        {
                            IExcelGeneralRule rule2 = rule as IExcelGeneralRule;
                            if (rule2.DifferentialFormattingId == -1)
                            {
                                differentialFormattingId = dxfs.Count;
                                dxfs.Add(new DifferentialFormatting());
                            }
                            else
                            {
                                differentialFormattingId = rule2.DifferentialFormattingId;
                            }
                            num6 = 2;
                            formulas = rule2.Formulas;
                        }
                        else if (rule is IExcelHighlightingRule)
                        {
                            IExcelHighlightingRule rule3 = rule as IExcelHighlightingRule;
                            differentialFormattingId = rule3.DifferentialFormattingId;
                            num6 = 1;
                            comparisonOperator = (byte) rule3.ComparisonOperator;
                            formulas = rule3.Formulas;
                        }
                        IDifferentialFormatting dxf = new DifferentialFormatting();
                        if (differentialFormattingId != -1)
                        {
                            dxf = dxfs[differentialFormattingId];
                        }
                        if ((dxf.NumberFormat != null) || (dxf.FormatId >= 0))
                        {
                            if ((dxf.NumberFormat != null) && (dxf.NumberFormat.NumberFormatCode != null))
                            {
                                XLUnicodeString str = new XLUnicodeString {
                                    Text = dxf.NumberFormat.NumberFormatCode
                                };
                                short num9 = 0;
                                if (str.fHighByte == 0)
                                {
                                    num9 = (short)(5 + str.cch);
                                }
                                else
                                {
                                    num9 = (short)(5 + (2 * str.cch));
                                }
                                num3 += num9;
                            }
                            else
                            {
                                num3 += 2;
                            }
                        }
                        if (dxf.Font != null)
                        {
                            num3 += 0x76;
                        }
                        if (dxf.Alignment != null)
                        {
                            num3 += 8;
                        }
                        if (dxf.Border != null)
                        {
                            num3 += 8;
                        }
                        if (dxf.Fill != null)
                        {
                            num3 += 4;
                        }
                        if (dxf.IsHidden || dxf.IsLocked)
                        {
                            num3 += 2;
                        }
                        int extraDataLength = 0;
                        FormulaProcess process = new FormulaProcess(sheet, 0, 0, null);
                        int row = conditionalFormat.Ranges[0].Row;
                        int column = conditionalFormat.Ranges[0].Column;
                        foreach (IRange range2 in conditionalFormat.Ranges)
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
                        process.row = row;
                        process.column = column;
                        process._isConditionalFormatFormula = true;
                        process.isA1RefStyle = this._calulationProperty.RefMode == ExcelReferenceStyle.A1;
                        if ((formulas != null) && (formulas.Count == 1))
                        {
                            buffer = process.ToExcelParsedFormula(sheet, formulas[0], this._linkTable, ref extraDataLength, false);
                            length = (short)buffer.Length;
                        }
                        else if ((formulas != null) && (formulas.Count == 2))
                        {
                            buffer = process.ToExcelParsedFormula(sheet, formulas[0], this._linkTable, ref extraDataLength, false);
                            length = (short)buffer.Length;
                            buffer2 = process.ToExcelParsedFormula(sheet, formulas[1], this._linkTable, ref extraDataLength, false);
                            num5 = (short)buffer2.Length;
                        }
                        num3 += length;
                        num3 += num5;
                        record2.DataLength = num3;
                        record2.Write(writer);
                        writer.Write(num6);
                        writer.Write(comparisonOperator);
                        writer.Write(length);
                        writer.Write(num5);
                        this.WriteDXFRecord(writer, dxf);
                        if (buffer != null)
                        {
                            writer.Write(buffer);
                        }
                        if (buffer2 != null)
                        {
                            writer.Write(buffer2);
                        }
                    }
                }
            }
        }

        private void WriteConditionalFormating12(BinaryWriter writer, short sheet, IExcelConditionalFormat conditionalFormat)
        {
            if (((conditionalFormat != null) && (conditionalFormat.Ranges != null)) && (conditionalFormat.Ranges.Count != 0))
            {
                BiffRecord record = new BiffRecord {
                    RecordType = BiffRecordNumber.CONDFMT12
                };
                IRange conditionalFormatRefBound = null;
                if (conditionalFormat.Ranges.Count == 1)
                {
                    conditionalFormatRefBound = conditionalFormat.Ranges[0];
                }
                else
                {
                    conditionalFormatRefBound = this.GetConditionalFormatRefBound(conditionalFormat.Ranges);
                }
                if (conditionalFormatRefBound != null)
                {
                    record.DataLength = (short)(0x1a + (conditionalFormat.Ranges.Count * 8));
                    record.Write(writer);
                    writer.Write((short) 0x879);
                    writer.Write((short) 1);
                    this.WriteRange(writer, conditionalFormatRefBound);
                    writer.Write((short) ((short) conditionalFormat.ConditionalFormattingRules.Count));
                    writer.Write((short) (conditionalFormat.Identifier << 1));
                    this.WriteRange(writer, conditionalFormatRefBound);
                    writer.Write((short) ((short) conditionalFormat.Ranges.Count));
                    for (int i = 0; i < conditionalFormat.Ranges.Count; i++)
                    {
                        this.WriteRange(writer, conditionalFormat.Ranges[i]);
                    }
                    foreach (IExcelConditionalFormatRule rule in conditionalFormat.ConditionalFormattingRules)
                    {
                        BiffRecord record2 = new BiffRecord {
                            RecordType = BiffRecordNumber.CF12
                        };
                        short num2 = 40;
                        short length = 0;
                        short num4 = 0;
                        short num5 = 0;
                        byte[] buffer = null;
                        byte[] buffer2 = null;
                        byte[] buffer3 = null;
                        FormulaProcess process = new FormulaProcess(sheet, 0, 0, null);
                        int row = conditionalFormat.Ranges[0].Row;
                        int column = conditionalFormat.Ranges[0].Column;
                        foreach (IRange range2 in conditionalFormat.Ranges)
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
                        process.row = row;
                        process.column = column;
                        process._isConditionalFormatFormula = true;
                        process.isA1RefStyle = this._calulationProperty.RefMode == ExcelReferenceStyle.A1;
                        int extraDataLength = 0;
                        if (rule is IExcelHighlightingRule)
                        {
                            num2 += 6;
                            IExcelHighlightingRule rule2 = rule as IExcelHighlightingRule;
                            if (rule2.Formulas.Count > 0)
                            {
                                buffer = process.ToExcelParsedFormula(sheet, rule2.Formulas[0], this._linkTable, ref extraDataLength, false);
                                if (buffer != null)
                                {
                                    length = (short)buffer.Length;
                                }
                                if (rule2.Formulas.Count > 1)
                                {
                                    buffer3 = process.ToExcelParsedFormula(sheet, rule2.Formulas[1], this._linkTable, ref extraDataLength, false);
                                    if (buffer3 != null)
                                    {
                                        num5 = (short)buffer3.Length;
                                    }
                                }
                            }
                            IDifferentialFormatting dxfExt = this.Dxfs[rule2.DifferentialFormattingId];
                            num2 += (short) this.GetDXFN12RecrodSize(dxfExt);
                            byte num9 = 1;
                            if (rule2.StopIfTrue)
                            {
                                num9 |= 2;
                            }
                            num2 += (short)(length + num5);
                            record2.DataLength = num2;
                            record2.Write(writer);
                            writer.Write((short) 0x87a);
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte) 1);
                            writer.Write((byte) ((byte) rule2.ComparisonOperator));
                            writer.Write(length);
                            writer.Write(num5);
                            this.WriteDXFN12Record(writer, dxfExt);
                            if (length > 0)
                            {
                                writer.Write(buffer);
                            }
                            if (num5 > 0)
                            {
                                writer.Write(buffer3);
                            }
                            writer.Write((short) 0);
                            writer.Write(num9);
                            writer.Write((short) ((short) rule2.Priority));
                            writer.Write((short) ((short) rule2.Type));
                            writer.Write((byte) 0x10);
                            writer.Write(new byte[0x10]);
                        }
                        else if (rule is IExcelGeneralRule)
                        {
                            num2 += 6;
                            IExcelGeneralRule rule3 = rule as IExcelGeneralRule;
                            if (rule3.Formulas.Count > 0)
                            {
                                buffer = process.ToExcelParsedFormula(sheet, rule3.Formulas[0], this._linkTable, ref extraDataLength, false);
                                if (buffer != null)
                                {
                                    length = (short)buffer.Length;
                                }
                                if (rule3.Formulas.Count > 1)
                                {
                                    buffer3 = process.ToExcelParsedFormula(sheet, rule3.Formulas[1], this._linkTable, ref extraDataLength, false);
                                    if (buffer3 != null)
                                    {
                                        num5 = (short)buffer3.Length;
                                    }
                                }
                            }
                            IDifferentialFormatting formatting2 = this.Dxfs[rule3.DifferentialFormattingId];
                            num2 += (short) this.GetDXFN12RecrodSize(formatting2);
                            byte num10 = 1;
                            if (rule3.StopIfTrue)
                            {
                                num10 |= 2;
                            }
                            num2 += (short)(length + num5);
                            record2.DataLength = num2;
                            record2.Write(writer);
                            writer.Write((short) 0x87a);
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte) 2);
                            writer.Write((byte) 0);
                            writer.Write(length);
                            writer.Write(num5);
                            this.WriteDXFN12Record(writer, formatting2);
                            if (length > 0)
                            {
                                writer.Write(buffer);
                            }
                            if (num5 > 0)
                            {
                                writer.Write(buffer3);
                            }
                            writer.Write((short) 0);
                            writer.Write(num10);
                            writer.Write((short) ((short) rule3.Priority));
                            writer.Write((short) ((short) rule3.Type));
                            writer.Write((byte) 0x10);
                            writer.Write(new byte[0x10]);
                        }
                        else if (rule is IExcelColorScaleRule)
                        {
                            IExcelColorScaleRule rule4 = rule as IExcelColorScaleRule;
                            int num11 = rule4.HasMiddleNode ? 3 : 2;
                            new List<byte[]>();
                            num2 += 14;
                            num2 += 3;
                            if (rule4.Minimum.IsFormula || (rule4.Minimum.Type == ExcelConditionalFormatValueObjectType.Formula))
                            {
                                string formula = this.GetFormulaForConditionalFormat(rule4.Minimum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer = process.ToExcelParsedFormula(sheet, formula, this._linkTable, ref extraDataLength, false);
                            }
                            else if ((rule4.Minimum.Type != ExcelConditionalFormatValueObjectType.Min) && ((buffer == null) || (buffer.Length == 0)))
                            {
                                num2 += 8;
                            }
                            if (rule4.HasMiddleNode)
                            {
                                num2 += 3;
                                if (rule4.Middle.IsFormula || (rule4.Middle.Type == ExcelConditionalFormatValueObjectType.Formula))
                                {
                                    string str2 = this.GetFormulaForConditionalFormat(rule4.Middle.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                    buffer2 = process.ToExcelParsedFormula(sheet, str2, this._linkTable, ref extraDataLength, false);
                                }
                                else if ((rule4.Middle.Type != ExcelConditionalFormatValueObjectType.Min) && ((buffer2 == null) || (buffer2.Length == 0)))
                                {
                                    num2 += 8;
                                }
                            }
                            num2 += 3;
                            if (rule4.Maximum.IsFormula || (rule4.Maximum.Type == ExcelConditionalFormatValueObjectType.Formula))
                            {
                                string str3 = this.GetFormulaForConditionalFormat(rule4.Maximum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer3 = process.ToExcelParsedFormula(sheet, str3, this._linkTable, ref extraDataLength, false);
                            }
                            else if ((rule4.Maximum.Type != ExcelConditionalFormatValueObjectType.Max) && ((buffer3 == null) || (buffer3.Length == 0)))
                            {
                                num2 += 8;
                            }
                            num2 += (short)(num11 * 8);
                            num2 += (short)(num11 * 0x18);
                            if (buffer != null)
                            {
                                length = (short)buffer.Length;
                            }
                            if (buffer2 != null)
                            {
                                num4 = (short)buffer2.Length;
                            }
                            if (buffer3 != null)
                            {
                                num5 = (short)buffer3.Length;
                            }
                            num2 += (short)((length + num4) + num5);
                            record2.DataLength = num2;
                            record2.Write(writer);
                            writer.Write((short) 0x87a);
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte) 3);
                            writer.Write((byte) 0);
                            writer.Write((ushort) 0);
                            writer.Write((ushort) 0);
                            this.WriteEmptyDXFN12Record(writer);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            writer.Write((ushort) ((ushort) rule4.Priority));
                            writer.Write((short) ((short) rule4.Type));
                            writer.Write((byte) 0x10);
                            writer.Write(new byte[0x10]);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) ((byte) num11));
                            writer.Write((byte) ((byte) num11));
                            writer.Write((byte) 3);
                            writer.Write((byte) ((byte) rule4.Minimum.Type));
                            writer.Write(length);
                            if ((buffer != null) && (buffer.Length > 0))
                            {
                                writer.Write(buffer);
                            }
                            if ((rule4.Minimum.Type != ExcelConditionalFormatValueObjectType.Min) && (length == 0))
                            {
                                double result = 0.0;
                                if (((rule4.Minimum != null) && (rule4.Minimum.Value != null)) && double.TryParse(rule4.Minimum.Value, out result))
                                {
                                    writer.Write(result);
                                }
                                else
                                {
                                    writer.Write((double) 0.0);
                                }
                            }
                            writer.Write((double) 0.0);
                            if (rule4.HasMiddleNode)
                            {
                                writer.Write((byte) ((byte) rule4.Middle.Type));
                                writer.Write(num4);
                                if (num4 > 0)
                                {
                                    writer.Write(buffer2);
                                }
                                else
                                {
                                    double num13 = 0.0;
                                    if (((rule4.Middle != null) && (rule4.Middle.Value != null)) && double.TryParse(rule4.Middle.Value, out num13))
                                    {
                                        writer.Write(num13);
                                    }
                                    else
                                    {
                                        writer.Write((double) 0.0);
                                    }
                                }
                                writer.Write((double) 0.5);
                            }
                            writer.Write((byte) ((byte) rule4.Maximum.Type));
                            writer.Write(num5);
                            if ((buffer3 != null) && (buffer3.Length > 0))
                            {
                                writer.Write(buffer3);
                            }
                            if ((rule4.Maximum.Type != ExcelConditionalFormatValueObjectType.Max) && (num5 == 0))
                            {
                                double num14 = 0.0;
                                if (((rule4.Maximum != null) && (rule4.Maximum.Value != null)) && double.TryParse(rule4.Maximum.Value, out num14))
                                {
                                    writer.Write(num14);
                                }
                                else
                                {
                                    writer.Write((double) 0.0);
                                }
                            }
                            writer.Write((double) 1.0);
                            writer.Write((double) 0.0);
                            this.WriteCFColor(writer, rule4.MinimumColor);
                            if (rule4.HasMiddleNode)
                            {
                                writer.Write((double) 0.5);
                                this.WriteCFColor(writer, rule4.MiddleColor);
                            }
                            writer.Write((double) 1.0);
                            this.WriteCFColor(writer, rule4.MaximumColor);
                        }
                        else if (rule is IExcelDataBarRule)
                        {
                            IExcelDataBarRule rule5 = rule as IExcelDataBarRule;
                            num2 += 30;
                            num2 += 3;
                            double num15 = 0.0;
                            if (rule5.Minimum.Type == ExcelConditionalFormatValueObjectType.AutoMin)
                            {
                                rule5.Minimum.Type = ExcelConditionalFormatValueObjectType.Min;
                            }
                            if (rule5.Maximum.Type == ExcelConditionalFormatValueObjectType.AutoMax)
                            {
                                rule5.Maximum.Type = ExcelConditionalFormatValueObjectType.Max;
                            }
                            if (rule5.Minimum.IsFormula || (rule5.Minimum.Type == ExcelConditionalFormatValueObjectType.Formula))
                            {
                                string str4 = this.GetFormulaForConditionalFormat(rule5.Minimum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer = process.ToExcelParsedFormula(sheet, str4, this._linkTable, ref extraDataLength, false);
                            }
                            else if (!string.IsNullOrEmpty(rule5.Minimum.Value) && !double.TryParse(rule5.Minimum.Value, out num15))
                            {
                                string str5 = this.GetFormulaForConditionalFormat(rule5.Minimum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer = process.ToExcelParsedFormula(sheet, str5, this._linkTable, ref extraDataLength, false);
                            }
                            else if (rule5.Minimum.Type != ExcelConditionalFormatValueObjectType.Min)
                            {
                                num2 += 8;
                            }
                            num2 += 3;
                            if (rule5.Maximum.IsFormula || (rule5.Maximum.Type == ExcelConditionalFormatValueObjectType.Formula))
                            {
                                string str6 = this.GetFormulaForConditionalFormat(rule5.Maximum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer3 = process.ToExcelParsedFormula(sheet, str6, this._linkTable, ref extraDataLength, false);
                            }
                            else if (!string.IsNullOrEmpty(rule5.Maximum.Value) && !double.TryParse(rule5.Maximum.Value, out num15))
                            {
                                string str7 = this.GetFormulaForConditionalFormat(rule5.Maximum.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                buffer3 = process.ToExcelParsedFormula(sheet, str7, this._linkTable, ref extraDataLength, false);
                            }
                            else if (rule5.Maximum.Type != ExcelConditionalFormatValueObjectType.Max)
                            {
                                num2 += 8;
                            }
                            if (buffer != null)
                            {
                                length = (short)buffer.Length;
                            }
                            if (buffer3 != null)
                            {
                                num5 = (short)buffer3.Length;
                            }
                            num2 += (short)(length + num5);
                            record2.DataLength = num2;
                            record2.Write(writer);
                            writer.Write((short) 0x87a);
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte) 4);
                            writer.Write((byte) 0);
                            writer.Write((ushort) 0);
                            writer.Write((ushort) 0);
                            this.WriteEmptyDXFN12Record(writer);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            writer.Write((ushort) ((ushort) rule5.Priority));
                            writer.Write((ushort) ((ushort) rule5.Type));
                            writer.Write((byte) 0x10);
                            writer.Write(new byte[0x10]);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            byte num16 = 0;
                            if (rule5.RightToLeft)
                            {
                                num16 |= 1;
                            }
                            if (!rule5.ShowValue)
                            {
                                num16 |= 2;
                            }
                            writer.Write(num16);
                            if ((rule5.MinimumDataBarLength == 0) && (rule5.MaximumDataBarLength == 0))
                            {
                                rule5.MinimumDataBarLength = 10;
                                rule5.MaximumDataBarLength = 90;
                            }
                            writer.Write(rule5.MinimumDataBarLength);
                            writer.Write(rule5.MaximumDataBarLength);
                            this.WriteCFColor(writer, rule5.Color);
                            writer.Write((byte) ((byte) rule5.Minimum.Type));
                            writer.Write(length);
                            if ((buffer != null) && (buffer.Length > 0))
                            {
                                writer.Write(buffer);
                            }
                            if ((rule5.Minimum.Type != ExcelConditionalFormatValueObjectType.Min) && (length == 0))
                            {
                                writer.Write(double.Parse(rule5.Minimum.Value));
                            }
                            writer.Write((byte) ((byte) rule5.Maximum.Type));
                            writer.Write(num5);
                            if ((buffer3 != null) && (buffer3.Length > 0))
                            {
                                writer.Write(buffer3);
                            }
                            if ((rule5.Maximum.Type != ExcelConditionalFormatValueObjectType.Max) && (num5 == 0))
                            {
                                writer.Write(double.Parse(rule5.Maximum.Value));
                            }
                        }
                        else if (rule is ExcelIconSetsRule)
                        {
                            ExcelIconSetsRule rule6 = rule as ExcelIconSetsRule;
                            num2 += 14;
                            num2 += (short)(rule6.Thresholds.Count * 5);
                            List<byte[]> list = new List<byte[]>();
                            foreach (IExcelConditionalFormatValueObject obj2 in rule6.Thresholds)
                            {
                                byte[] buffer4 = null;
                                num2 += 3;
                                double num17 = 0.0;
                                if (obj2.IsFormula || (obj2.Type == ExcelConditionalFormatValueObjectType.Formula))
                                {
                                    string str8 = this.GetFormulaForConditionalFormat(obj2.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                    buffer4 = process.ToExcelParsedFormula(sheet, str8, this._linkTable, ref extraDataLength, false);
                                }
                                else if ((obj2.Type == ExcelConditionalFormatValueObjectType.Num) && !double.TryParse(obj2.Value, out num17))
                                {
                                    obj2.Type = ExcelConditionalFormatValueObjectType.Formula;
                                    string str9 = this.GetFormulaForConditionalFormat(obj2.Value, sheet, this._calulationProperty.RefMode != ExcelReferenceStyle.A1, this._linkTable);
                                    buffer4 = process.ToExcelParsedFormula(sheet, str9, this._linkTable, ref extraDataLength, false);
                                }
                                else if ((buffer4 == null) || ((buffer4 != null) && (buffer4.Length == 0)))
                                {
                                    num2 += 8;
                                }
                                if ((buffer4 != null) && (buffer4.Length > 0))
                                {
                                    num2 += (short)buffer4.Length;
                                }
                                list.Add(buffer4);
                            }
                            record2.DataLength = num2;
                            record2.Write(writer);
                            writer.Write((short) 0x87a);
                            writer.Write((short) 0);
                            writer.Write(0);
                            writer.Write(0);
                            writer.Write((byte) 6);
                            writer.Write((byte) 0);
                            writer.Write((ushort) 0);
                            writer.Write((ushort) 0);
                            this.WriteEmptyDXFN12Record(writer);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            writer.Write((ushort) ((ushort) rule6.Priority));
                            writer.Write((short) ((short) rule6.Type));
                            writer.Write((byte) 0x10);
                            writer.Write(new byte[0x10]);
                            writer.Write((short) 0);
                            writer.Write((byte) 0);
                            writer.Write((byte) ((byte) rule6.Thresholds.Count));
                            if (rule6.IconSet != ExcelIconSetType.Icon_NIL)
                            {
                                writer.Write((byte) ((byte) rule6.IconSet));
                            }
                            byte num18 = 0;
                            if (rule6.IconOnly)
                            {
                                num18 |= 1;
                            }
                            if (rule6.ReversedOrder)
                            {
                                num18 |= 4;
                            }
                            writer.Write(num18);
                            for (int j = 0; j < rule6.Thresholds.Count; j++)
                            {
                                IExcelConditionalFormatValueObject obj3 = rule6.Thresholds[j];
                                writer.Write((byte) ((byte) obj3.Type));
                                if ((list[j] != null) && (list[j].Length > 0))
                                {
                                    writer.Write((short) list[j].Length);
                                    writer.Write(list[j]);
                                }
                                else
                                {
                                    double num20 = 0.0;
                                    if (double.TryParse(obj3.Value, out num20))
                                    {
                                        writer.Write((short) 0);
                                        if ((obj3.Type != ExcelConditionalFormatValueObjectType.Min) && (obj3.Type != ExcelConditionalFormatValueObjectType.Max))
                                        {
                                            writer.Write(double.Parse(obj3.Value));
                                        }
                                    }
                                }
                                if (rule6.NotPassTheThresholdsWhenEquals[j])
                                {
                                    writer.Write((byte) 0);
                                }
                                else
                                {
                                    writer.Write((byte) 1);
                                }
                                writer.Write(0);
                            }
                        }
                    }
                }
            }
        }

        private void WriteConditionalFormatingRecord(BinaryWriter writer, short sheet, IExcelConditionalFormat conditionalFormat)
        {
            if (conditionalFormat.IsOffice2007ConditionalFormat)
            {
                this.WriteConditionalFormating12(writer, sheet, conditionalFormat);
            }
            else
            {
                this.WriteConditionalFormating(writer, sheet, conditionalFormat);
            }
        }

        internal void WriteDefaultColumnWidth(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            double defaultColumnWidth = this._excelWriter.GetDefaultColumnWidth(sheetIndex);
            biff.Write(writer);
            writer.Write((ushort) ((ushort) defaultColumnWidth));
            biff.RecordType = BiffRecordNumber.STANDARDWIDTH;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write((ushort) (defaultColumnWidth * 256.0));
        }

        internal void WriteDefaultRowHeight(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            bool customHeight = false;
            double defaultRowHeight = this._excelWriter.GetDefaultRowHeight(sheetIndex, ref customHeight);
            biff.Write(writer);
            short num2 = 0;
            if (customHeight)
            {
                num2 = 1;
            }
            writer.Write(num2);
            writer.Write((ushort) ((ushort) Math.Ceiling((double) (defaultRowHeight * 20.0))));
        }

        internal void WriteDelta(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            double maximunChange = this._calulationProperty.MaximunChange;
            if (maximunChange < 0.0)
            {
                maximunChange = 0.0;
            }
            biff.Write(writer);
            writer.Write(maximunChange);
        }

        private bool WriteDIMENSIONS(BinaryWriter writer, BiffRecord biff, short sheet, int rowCount, int columnCount)
        {
            if (writer == null)
            {
                return false;
            }
            int num = columnCount;
            int num2 = rowCount;
            if (num > 0x100)
            {
                num = 0x100;
            }
            if (num2 > 0x10000)
            {
                num2 = 0x10000;
            }
            biff.Write(writer);
            writer.Write(0);
            writer.Write((uint) num2);
            writer.Write((ushort) 0);
            writer.Write((ushort) ((ushort) num));
            writer.Write((short) 0);
            return true;
        }

        private byte[] WriteDOper(BinaryWriter bw, IExcelCustomFilter cf)
        {
            bool flag;
            byte num5;
            if (cf == null)
            {
                bw.Write((byte) 0);
                bw.Write((byte) 0);
                bw.Write(0);
                bw.Write(0);
                return null;
            }
            byte[] buffer = null;
            byte valueType = cf.ValueType;
            if ((valueType == 0) && (cf.Value != null))
            {
                int index = Array.IndexOf<Type>(TypeExtension.KnownTypes, cf.Value.GetType());
                if (index >= 0)
                {
                    switch (TypeExtension.KnownCodes[index])
                    {
                        case Utils.TypeCode.Single:
                        case Utils.TypeCode.Double:
                            valueType = 4;
                            goto Label_00AA;

                        case Utils.TypeCode.Boolean:
                            valueType = 8;
                            goto Label_00AA;

                        case Utils.TypeCode.String:
                            double num3;
                            if (double.TryParse(cf.Value.ToString(), out num3))
                            {
                                valueType = 4;
                            }
                            else
                            {
                                valueType = 6;
                            }
                            goto Label_00AA;
                    }
                    if (cf.Value is Exception)
                    {
                        valueType = 8;
                    }
                }
            }
        Label_00AA:
            bw.Write(valueType);
            bw.Write((byte) ((byte) cf.Operator));
            switch (valueType)
            {
                case 2:
                {
                    byte[] bytes = BitConverter.GetBytes((double) ((double) cf.Value));
                    int num4 = 0;
                    num4 |= bytes[4] & 0xfc;
                    num4 |= bytes[5] << 8;
                    num4 |= bytes[6] << 0x10;
                    num4 |= bytes[7] << 0x18;
                    bw.Write(num4);
                    bw.Write(0);
                    return buffer;
                }
                case 4:
                    bw.Write(double.Parse(cf.Value.ToString()));
                    return buffer;

                case 6:
                {
                    string str = (string) ((string) cf.Value);
                    XLUnicodeStringNoCch cch = new XLUnicodeStringNoCch {
                        Text = str
                    };
                    buffer = new byte[cch.rgb.Length + 1];
                    buffer[0] = 1;
                    Array.Copy(cch.rgb, 0, buffer, 1, cch.rgb.Length);
                    bw.Write(0);
                    bw.Write((byte) ((byte) cch.cch));
                    bw.Write((byte) 0);
                    bw.Write((short) 0);
                    return buffer;
                }
                case 8:
                {
                    flag = cf.Value is Exception;
                    if (!flag)
                    {
                        num5 = ((bool) cf.Value) ? ((byte) 1) : ((byte) 0);
                        break;
                    }
                    Exception exception = cf.Value as Exception;
                    if (!(exception is ArgumentNullException))
                    {
                        if (exception is ArgumentOutOfRangeException)
                        {
                            num5 = 0x24;
                        }
                        else if (exception is ArgumentException)
                        {
                            num5 = 15;
                        }
                        else if (exception is DivideByZeroException)
                        {
                            num5 = 7;
                        }
                        else if (exception is NullReferenceException)
                        {
                            num5 = 0x17;
                        }
                        else if (exception is MissingMemberException)
                        {
                            num5 = 0x1d;
                        }
                        else if (exception is NotSupportedException)
                        {
                            num5 = 0x2a;
                        }
                        else
                        {
                            num5 = 0xff;
                        }
                        break;
                    }
                    num5 = 1;
                    break;
                }
                default:
                    bw.Write(0);
                    bw.Write(0);
                    return buffer;
            }
            bw.Write(flag);
            bw.Write(num5);
            bw.Write((short) 0);
            bw.Write(0);
            return buffer;
        }

        private byte[] WriteDoperNumber(BinaryWriter bw, double value, byte op)
        {
            bw.Write((byte) 4);
            bw.Write(op);
            bw.Write(value);
            return null;
        }

        private byte[] WriteDoperRKNumber(BinaryWriter bw, int value, byte op)
        {
            bw.Write((byte) 4);
            bw.Write(op);
            value = value << 2;
            value |= 2;
            bw.Write(value);
            bw.Write(0);
            return null;
        }

        private byte[] WriteDoperString(BinaryWriter bw, string value)
        {
            byte[] buffer = null;
            bw.Write((byte) 6);
            bw.Write((byte) 2);
            string stringIn = value;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter((Stream) stream))
                {
                    this.WriteBiffStr(writer, stringIn, false, false, false, false, 2);
                }
                buffer = stream.ToArray();
            }
            bw.Write((byte) buffer.Length);
            bw.Write((byte) 0);
            bw.Write((short) 0);
            bw.Write(0);
            return buffer;
        }

        private void WriteDV(BinaryWriter writer, short sheet)
        {
            List<IExcelDataValidation> validationData = this._excelWriter.GetValidationData(sheet);
            if ((validationData != null) && (validationData.Count > 0))
            {
                BiffRecord record = new BiffRecord {
                    RecordType = BiffRecordNumber.DVAL,
                    DataLength = 0x12
                };
                record.Write(writer);
                writer.Write((byte) 4);
                writer.Write((byte) 0);
                writer.Write(0);
                writer.Write(0);
                writer.Write(-1);
                writer.Write(validationData.Count);
                record.RecordType = BiffRecordNumber.DV;
                using (List<IExcelDataValidation>.Enumerator enumerator = validationData.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        IExcelDataValidation item = enumerator.Current;
                        if ((item.Ranges != null) && (item.Ranges.Count != 0))
                        {
                            try
                            {
                                byte[] buffer;
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    Action<string> action2 = null;
                                    using (BinaryWriter bw = new BinaryWriter((Stream) stream))
                                    {
                                        int num = (int) (item.Type | ((ExcelDataValidationType) (((int) item.ErrorStyle) << 4)));
                                        num |= item.AllowBlank ? 0x100 : 0;
                                        num |= ((item.Type == ExcelDataValidationType.List) && !item.ShowPromptBox) ? 0x200 : 0;
                                        num |= item.ShowInputMessage ? 0x40000 : 0;
                                        num |= item.ShowErrorBox ? 0x80000 : 0;
                                        num |= ((int) item.CompareOperator) << 20;
                                        if (((item.Type == ExcelDataValidationType.List) && item.FirstFormula.StartsWith("\"")) && item.FirstFormula.EndsWith("\""))
                                        {
                                            num |= 0x80;
                                        }
                                        bw.Write(num);
                                        this.WriteBiffStrForDV(bw, item.PromptTitle, false, false, false, false, 2);
                                        this.WriteBiffStrForDV(bw, item.ErrorTitle, false, false, false, false, 2);
                                        this.WriteBiffStrForDV(bw, item.Prompt, false, false, false, false, 2);
                                        this.WriteBiffStrForDV(bw, item.Error, false, false, false, false, 2);
                                        if (action2 == null)
                                        {
                                            action2 = delegate (string f) {
                                                byte[] bufferTemp;
                                                if ((item.Type == ExcelDataValidationType.None) || string.IsNullOrEmpty(item.FirstFormula))
                                                {
                                                    bufferTemp = new byte[0];
                                                    bw.Write((short) 0);
                                                    bw.Write((short) 0);
                                                }
                                                else
                                                {
                                                    int extraDataLength = 0;
                                                    FormulaProcess process = new FormulaProcess(sheet, 0, 0, null);
                                                    int row = item.Ranges[0].Row;
                                                    int column = item.Ranges[0].Column;
                                                    process.row = row;
                                                    process.column = column;
                                                    process.isA1RefStyle = this._excelWriter.GetCalculationProperty().RefMode == ExcelReferenceStyle.A1;
                                                    process._isConditionalFormatFormula = true;
                                                    process._isDataValidationFormula = true;
                                                    bufferTemp = process.ToExcelParsedFormula(sheet, f, this._linkTable, ref extraDataLength, false);
                                                    process._isConditionalFormatFormula = false;
                                                    process._isDataValidationFormula = true;
                                                    bw.Write((short)bufferTemp.Length);
                                                    bw.Write((short) 0);
                                                    bw.Write(bufferTemp);
                                                }
                                            };
                                        }
                                        Action<string> action = action2;
                                        string firstFormula = item.FirstFormula;
                                        string secondFormula = item.SecondFormula;
                                        if (item.Type == ExcelDataValidationType.List)
                                        {
                                            firstFormula = this.ConvertDVFormulaBackIfNeeded(firstFormula);
                                        }
                                        action(firstFormula);
                                        action(secondFormula);
                                        bw.Write((short) ((short) item.Ranges.Count));
                                        foreach (IRange range in item.Ranges)
                                        {
                                            this.WriteRange(bw, range);
                                        }
                                    }
                                    buffer = stream.ToArray();
                                }
                                record.DataLength = (short)buffer.Length;
                                record.Write(writer);
                                writer.Write(buffer);
                                continue;
                            }
                            catch (Exception exception)
                            {
                                this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeDvError"), ExcelWarningCode.General, sheet, item.Ranges[0].Row, item.Ranges[0].Column, exception));
                                continue;
                            }
                        }
                    }
                }
            }
        }

        private void WriteDxf(BinaryWriter writer, IDifferentialFormatting dxf)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer2 = new BinaryWriter((Stream) stream);
            int num = this.WriteXfProps(writer2, dxf);
            writer2.Flush();
            writer2.Dispose();
            byte[] buffer = stream.ToArray();
            new BiffRecord { RecordType = BiffRecordNumber.DXF, DataLength = (short)(0x12 + buffer.Length) }.Write(writer);
            writer.Write((short) 0x88d);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 3);
            writer.Write((short) 0);
            writer.Write((short) ((short) num));
            writer.Write(buffer);
        }

        private void WriteDXFN12Record(BinaryWriter writer, IDifferentialFormatting dxfExt)
        {
            int num = this.GetDXFN12RecrodSize(dxfExt);
            writer.Write((uint) num);
            if (num == 0)
            {
                writer.Write((short) 0);
            }
            else
            {
                this.WriteDXFRecord(writer, dxfExt);
                if ((dxfExt.ExtendedPropertyList != null) && (dxfExt.ExtendedPropertyList.Count > 0))
                {
                    writer.Write((short) 0);
                    writer.Write((ushort) 0xffff);
                    writer.Write((short) 0);
                    writer.Write((short) ((short) dxfExt.ExtendedPropertyList.Count));
                    foreach (Tuple<string, object> tuple in dxfExt.ExtendedPropertyList)
                    {
                        if (tuple.Item1 == "foreGround")
                        {
                            writer.Write((short) 4);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "backGround")
                        {
                            writer.Write((short) 5);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "topBorder")
                        {
                            writer.Write((short) 7);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "bottomBorder")
                        {
                            writer.Write((short) 8);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "leftBorder")
                        {
                            writer.Write((short) 9);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "rightBorder")
                        {
                            writer.Write((short) 10);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if (tuple.Item1 == "fontColor")
                        {
                            writer.Write((short) 13);
                            this.WriteFullColor(writer, (ExcelColor) tuple.Item2);
                        }
                        else if ((tuple.Item1 == "fontSheme") || (tuple.Item1 == "indentationLevel"))
                        {
                            writer.Write((short) 14);
                            writer.Write((short) 2);
                            writer.Write((ushort) ((ushort) tuple.Item2));
                        }
                        else
                        {
                            writer.Write((short) 1);
                            writer.Write((short) 0);
                        }
                    }
                }
            }
        }

        private void WriteDXFRecord(BinaryWriter writer, IDifferentialFormatting dxf)
        {
            writer.Write(this.GetDXFOptionFlag(dxf));
            int num = 0;
            if (((dxf.NumberFormat != null) || (dxf.FormatId >= 0)) && ((dxf.NumberFormat != null) && (dxf.NumberFormat.NumberFormatCode != null)))
            {
                num |= 1;
            }
            num |= 2;
            num |= 0x8000;
            writer.Write((short) ((short) num));
            if ((dxf.FormatId >= 0) || (dxf.NumberFormat != null))
            {
                if ((dxf.NumberFormat != null) && (dxf.NumberFormat.NumberFormatCode != null))
                {
                    XLUnicodeString str = new XLUnicodeString {
                        Text = dxf.NumberFormat.NumberFormatCode
                    };
                    short num2 = 0;
                    if (str.fHighByte == 0)
                    {
                        num2 = (short)(5 + str.cch);
                    }
                    else
                    {
                        num2 = (short)(5 + (2 * str.cch));
                    }
                    writer.Write(num2);
                    str.Write(writer);
                }
                else
                {
                    writer.Write((byte) 0);
                    writer.Write((dxf.FormatId >= 0) ? ((byte) ((byte) dxf.FormatId)) : ((byte) 0));
                }
            }
            if (dxf.Font != null)
            {
                IExcelFont font = dxf.Font;
                if (!string.IsNullOrWhiteSpace(font.FontName))
                {
                    XLUnicodeStringNoCch cch = new XLUnicodeStringNoCch {
                        Text = font.FontName
                    };
                    writer.Write((byte) ((byte) cch.cch));
                    cch.Write(writer);
                    int num3 = 0x3e - (cch.cch * 2);
                    if (num3 > 0)
                    {
                        writer.Write(new byte[num3]);
                    }
                }
                else
                {
                    writer.Write(new byte[0x40]);
                }
                writer.Write(-1);
                int num4 = 0;
                int num5 = 0;
                if (font.IsItalic)
                {
                    num4 |= 2;
                    num5 = (int)(num5 & 0xfffffffdL);
                }
                if (font.IsOutlineStyle)
                {
                    num4 |= 8;
                    num5 = (int)(num5 & 0xfffffff7L);
                }
                if (font.IsShadowStyle)
                {
                    num4 |= 0x10;
                    num5 = (int)(num5 & 0xffffffefL);
                }
                if (font.IsStrikeOut)
                {
                    num4 |= 0x80;
                    num5 = (int)(num5 & 0xffffff7fL);
                }
                writer.Write(num4);
                short num6 = 400;
                if (font.IsBold)
                {
                    num6 = 700;
                    num5 = (int)(num5 & 0xfffffffdL);
                }
                writer.Write(num6);
                int num7 = 1;
                int num8 = 1;
                if (font.VerticalAlignRun != VerticalAlignRun.BaseLine)
                {
                    writer.Write((short) font.VerticalAlignRun);
                    num7 = 0;
                }
                else
                {
                    writer.Write((short) 0);
                }
                if (font.UnderLineStyle != UnderLineStyle.None)
                {
                    writer.Write((byte) font.UnderLineStyle);
                    num8 = 0;
                }
                else
                {
                    writer.Write((byte) 0);
                }
                writer.Write(new byte[3]);
                if (font.FontColor == null)
                {
                    writer.Write(-1);
                }
                else if (font.FontColor == null)
                {
                    writer.Write(0x40);
                }
                else
                {
                    writer.Write((int) this._excelWriter.GetPaletteColor(font.FontColor));
                }
                writer.Write(0);
                writer.Write(num5);
                writer.Write(num7);
                writer.Write(num8);
                writer.Write(1);
                writer.Write(1);
                writer.Write(new byte[8]);
                writer.Write((short) 1);
            }
            if (dxf.Alignment != null)
            {
                byte num9 = 0;
                num9 = (byte) (num9 | (byte)dxf.Alignment.HorizontalAlignment);
                num9 |= (byte)(((byte) dxf.Alignment.VerticalAlignment) << 4);
                if (dxf.Alignment.IsTextWrapped)
                {
                    num9 |= 8;
                }
                if (dxf.Alignment.IsJustifyLastLine)
                {
                    num9 |= 0x80;
                }
                writer.Write(num9);
                writer.Write(dxf.Alignment.TextRotation);
                byte num10 = 0;
                num10 |= dxf.Alignment.IndentationLevel;
                if (dxf.Alignment.IsShrinkToFit)
                {
                    num10 |= 0x10;
                }
                if (dxf.Alignment.TextDirection != TextDirection.AccordingToContext)
                {
                    num10 |= (byte)(((byte) dxf.Alignment.TextDirection) << 6);
                }
                writer.Write(num10);
                writer.Write((byte) 0);
                writer.Write((short) 0);
                writer.Write((short) 0);
            }
            if (dxf.Border != null)
            {
                byte num11 = 0;
                num11 = (byte) (num11 | (byte)dxf.Border.Left.LineStyle);
                num11 |= (byte)(((byte)dxf.Border.Right.LineStyle) << 4);
                writer.Write(num11);
                num11 = (byte) (num11 | (byte)dxf.Border.Top.LineStyle);
                num11 |= (byte)(((byte)dxf.Border.Bottom.LineStyle) << 4);
                writer.Write(num11);
                ushort num12 = 0;
                num12 |= (ushort) this._excelWriter.GetPaletteColor(dxf.Border.Left.Color);
                num12 |= (ushort)(((int) this._excelWriter.GetPaletteColor(dxf.Border.Right.Color)) << 7);
                writer.Write(num12);
                num12 = 0;
                num12 |= (ushort) this._excelWriter.GetPaletteColor(dxf.Border.Top.Color);
                num12 |= (ushort)(((int) this._excelWriter.GetPaletteColor(dxf.Border.Bottom.Color)) << 7);
                writer.Write(num12);
                writer.Write((short) 0);
            }
            if (dxf.Fill != null)
            {
                short num13 = (short) dxf.Fill.Item1;
                writer.Write((short) (num13 << 10));
                if (!dxf.IsDFXExten)
                {
                    ushort num14 = 0;
                    if (dxf.Fill.Item2 != null)
                    {
                        num14 |= (ushort) this._excelWriter.GetPaletteColor(dxf.Fill.Item2);
                    }
                    else
                    {
                        num14 |= 0x40;
                    }
                    if (dxf.Fill.Item3 != null)
                    {
                        num14 |= (ushort)(((int) this._excelWriter.GetPaletteColor(dxf.Fill.Item3)) << 7);
                    }
                    else
                    {
                        num14 |= 0x2080;
                    }
                    writer.Write(num14);
                }
                else
                {
                    int paletteColor = 0x40;
                    int num16 = 0x41;
                    if (dxf.Fill.Item2 != null)
                    {
                        paletteColor = (int) this._excelWriter.GetPaletteColor(dxf.Fill.Item2);
                    }
                    if (dxf.Fill.Item3 != null)
                    {
                        num16 = (int) this._excelWriter.GetPaletteColor(dxf.Fill.Item3);
                    }
                    ushort num17 = (ushort)(paletteColor & 0x7f);
                    num17 |= (ushort)((num16 & 0x7f) << 7);
                    writer.Write(num17);
                }
            }
            if (dxf.IsHidden || dxf.IsLocked)
            {
                short num18 = 0;
                if (dxf.IsLocked)
                {
                    num18 |= 1;
                }
                if (dxf.IsHidden)
                {
                    num18 |= 2;
                }
                writer.Write(num18);
            }
        }

        private void WriteDxfs(BinaryWriter writer)
        {
            if ((this._excelWriter is IExcelTableWriter) && (this.Dxfs != null))
            {
                foreach (IDifferentialFormatting formatting in this.Dxfs)
                {
                    this.WriteDxf(writer, formatting);
                }
            }
        }

        private void WriteEmptyDXFN12Record(BinaryWriter writer)
        {
            writer.Write(0);
            writer.Write((short) 0);
        }

        private void WriteEndWorkbookRecords(BinaryWriter writer)
        {
            BiffRecord biff = new BiffRecord {
                RecordType = BiffRecordNumber.EOF,
                DataLength = 0
            };
            this.WriteEOF(writer, biff);
        }

        /// <summary>
        /// WriteRecord
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="biff"></param>
        /// <returns></returns>
        internal bool WriteEOF(BinaryWriter writer, BiffRecord biff)
        {
            biff.Write(writer);
            return true;
        }

        private void WriteExcelFormatRecord(BinaryWriter writer)
        {
            this._formats = this._excelWriter.GetExcelCellFormats();
            List<IExcelStyle> excelStyles = this._excelWriter.GetExcelStyles();
            if ((excelStyles == null) || (excelStyles.Count == 0))
            {
                IExtendedFormat format = this._excelWriter.GetExcelDefaultCellFormat();
                if (format != null)
                {
                    ExcelStyle style = new ExcelStyle {
                        BuiltInStyle = BuiltInStyleIndex.Normal,
                        Name = "Normal",
                        Format = format
                    };
                    excelStyles = new List<IExcelStyle> {
                        style
                    };
                }
                else
                {
                    excelStyles = new List<IExcelStyle> {
                        BuiltInExcelStyles.GetNormalStyle()
                    };
                }
            }
            IExcelStyle style2 = null;
            int index = -1;
            string styleName = "Normal";
            if (excelStyles != null)
            {
                foreach (IExcelStyle style3 in excelStyles)
                {
                    if (style3.IsBuiltInStyle)
                    {
                        ExcelStyle style4 = style3 as ExcelStyle;
                        if (style4.BuiltInStyle == BuiltInStyleIndex.Normal)
                        {
                            styleName = style4.Name;
                        }
                    }
                }
            }
            if (this.TryGetBuiltInStyleIndex(excelStyles, styleName, out index))
            {
                style2 = excelStyles[index];
            }
            List<IExcelFont> fonts = new List<IExcelFont>();
            this._allformats.AddRange((IEnumerable<IExtendedFormat>) this._formats);
            if (this._allformats == null)
            {
                this._allformats = new List<IExtendedFormat>();
            }
            if (this._allformats.Count == 0)
            {
                this._allformats.Insert(0, style2.Format);
                this._offSet++;
            }
            if (!this._allformats[0].Equals(style2.Format))
            {
                this._allformats.Insert(0, style2.Format);
                this._offSet++;
            }
            string str2 = "RowLevel_";
            string str3 = "ColLevel_";
            for (int i = 0; i < 7; i++)
            {
                int num8 = i + 1;
                string str4 = str2 + ((int) num8).ToString();
                int num9 = i + 1;
                string str5 = str3 + ((int) num9).ToString();
                int num3 = -1;
                if (this.TryGetBuiltInStyleIndex(excelStyles, str4, out num3))
                {
                    if (!this._allformats[(2 * i) + 1].Equals(excelStyles[num3].Format))
                    {
                        this._allformats.Insert((2 * i) + 1, excelStyles[num3].Format);
                        excelStyles.RemoveAt(num3);
                        this._offSet++;
                    }
                }
                else if (this._allformats.Count > ((2 * i) + 1))
                {
                    this._allformats.Insert((2 * i) + 1, style2.Format);
                    this._offSet++;
                }
                else
                {
                    this._allformats.Add(style2.Format);
                    this._offSet++;
                }
                if (this.TryGetBuiltInStyleIndex(excelStyles, str5, out num3))
                {
                    if (!this._allformats[(2 * i) + 2].Equals(excelStyles[num3].Format))
                    {
                        this._allformats.Insert((2 * i) + 2, excelStyles[num3].Format);
                        excelStyles.RemoveAt(num3);
                        this._offSet++;
                    }
                }
                else if (this._allformats.Count > ((2 * i) + 2))
                {
                    this._allformats.Insert((2 * i) + 2, style2.Format);
                    this._offSet++;
                }
                else
                {
                    this._allformats.Add(style2.Format);
                    this._offSet++;
                }
            }
            IExtendedFormat excelDefaultCellFormat = this._excelWriter.GetExcelDefaultCellFormat();
            if (this._allformats.Count == 15)
            {
                this._allformats.Add(excelDefaultCellFormat);
                this._offSet++;
            }
            else if (!this._allformats[15].Equals(excelDefaultCellFormat))
            {
                this._allformats.Insert(15, excelDefaultCellFormat);
                this._offSet++;
            }
            for (int j = 0; j < excelStyles.Count; j++)
            {
                if (j != index)
                {
                    this._allformats.Add(excelStyles[j].Format);
                }
            }
            for (int k = 0; k < this._allformats.Count; k++)
            {
                IExtendedFormat format3 = this._allformats[k];
                int num6 = 0;
                int numberFormatId = 0;
                if (format3.Font != null)
                {
                    num6 = fonts.IndexOf(format3.Font);
                    if (num6 >= 4)
                    {
                        num6++;
                    }
                    else if (num6 == -1)
                    {
                        fonts.Add(format3.Font);
                        num6 = (fonts.Count > 4) ? fonts.Count : (fonts.Count - 1);
                    }
                }
                if (format3.NumberFormat != null)
                {
                    numberFormatId = format3.NumberFormat.NumberFormatId;
                    if (!this._numberFormats.ContainsKey(numberFormatId))
                    {
                        this._numberFormats.Add(numberFormatId, format3.NumberFormat.NumberFormatCode);
                    }
                }
                else if (format3.NumberFormatIndex > 0)
                {
                    numberFormatId = format3.NumberFormatIndex;
                }
                XFRecrod recrod = ConverterFactory.GetXFBiffRecord(format3, (ushort) num6, this._excelWriter);
                if (format3.IsStyleFormat)
                {
                    recrod.IsStyleXF = true;
                    recrod.ParentXFIndex = 0xfff;
                }
                else
                {
                    recrod.IsStyleXF = false;
                    if (recrod.ParentXFIndex != 0)
                    {
                        recrod.ParentXFIndex += (ushort)this._offSet;
                    }
                }
                this._xfs.Add(recrod);
            }
            if (this._excelWriter is IExcelLosslessWriter)
            {
                this.AddTXOFonts(fonts);
                this.AddFontX(fonts);
            }
            this.WriteFONTRecord(writer, fonts);
            this.WriteFORMATRecord(writer, this._numberFormats);
            this.WriteXFRecord(writer, this._xfs, this._allformats);
        }

        private void WriteFeather(BinaryWriter writer)
        {
            new BiffRecord { RecordType = BiffRecordNumber.FEATHEADR, DataLength = 0x17 }.Write(writer);
            writer.Write((short) 0x867);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 2);
            writer.Write((byte) 1);
            writer.Write(-1);
            writer.Write((short) 0x4403);
            writer.Write((short) 0);
        }

        private void WriteFeather11(BinaryWriter writer, int nextTableEntry)
        {
            new BiffRecord { RecordType = BiffRecordNumber.FEATHEADR11, DataLength = 0x1d }.Write(writer);
            writer.Write((short) 0x871);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 5);
            writer.Write((byte) 1);
            writer.Write(-1);
            writer.Write(-1);
            writer.Write(nextTableEntry);
            writer.Write((short) 0);
        }

        private void WriteFilePass(BinaryWriter writer, BiffRecord biff)
        {
            biff.Write(writer);
            writer.Write((ushort) 1);
            writer.Write((ushort) 1);
            writer.Write((ushort) 1);
            writer.Write(new byte[0x30]);
        }

        private void WriteFONTRecord(BinaryWriter writer, List<IExcelFont> fonts)
        {
            while (fonts.Count < 5)
            {
                fonts.Add(new ExcelFont().Default);
            }
            using (List<IExcelFont>.Enumerator enumerator = fonts.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ConverterFactory.GetFontBiffRecord(enumerator.Current, this._excelWriter).Write(writer);
                }
            }
        }

        internal void WriteFooter(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            string footer = this._excelWriter.GetPrintPageSetting(sheetIndex).Footer;
            if (string.IsNullOrWhiteSpace(footer))
            {
                footer = this._excelWriter.GetPrintPageSetting(sheetIndex).AdvancedHeadFooterSetting.FooterOddPage;
            }
            if ((footer != null) && (footer.Length > 0))
            {
                MemoryStream @this = new MemoryStream();
                BinaryWriter writer2 = new BinaryWriter((Stream) @this);
                this.WriteBiffStr(writer2, footer, false, false, false, false, 2);
                biff.DataLength = (short) @this.Length;
                biff.Write(writer);
                writer.Write(@this.GetBuffer(), 0, (int) @this.Length);
            }
            else
            {
                biff.DataLength = 0;
                biff.Write(writer);
            }
        }

        private void WriteFORMATRecord(BinaryWriter writer, Dictionary<int, string> formats)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            List<int> list = new List<int>((IEnumerable<int>) formats.Keys);
            list.Sort();
            foreach (int num in list)
            {
                if (dictionary.ContainsKey(num))
                {
                    dictionary[num] = formats[num];
                }
                else
                {
                    dictionary.Add(num, formats[num]);
                }
            }
            foreach (KeyValuePair<int, string> pair in dictionary)
            {
                FORMATRecord record2 = new FORMATRecord {
                    FormatIndex = (ushort) pair.Key,
                    FormatString = pair.Value
                };
                record2.Write(writer);
            }
        }

        private void WriteFullColor(BinaryWriter writer, IExcelColor color)
        {
            writer.Write((short) 20);
            if (color.IsIndexedColor)
            {
                writer.Write((short) 1);
                writer.Write((short) (color.Tint * 32767.0));
                writer.Write(color.Value);
            }
            else if (color.IsRGBColor)
            {
                writer.Write((short) 2);
                writer.Write((short) (color.Tint * 32767.0));
                GcColor color2 = ColorExtension.FromArgb(color.Value);
                writer.Write(color2.R);
                writer.Write(color2.G);
                writer.Write(color2.B);
                writer.Write(color2.A);
            }
            else if (color.IsThemeColor)
            {
                writer.Write((short) 3);
                writer.Write((short) (color.Tint * 32767.0));
                writer.Write(color.Value);
            }
            writer.Write(0);
            writer.Write(0);
        }

        private void WriteFullColorExt(BinaryWriter bw, IExcelColor color)
        {
            if (color == null)
            {
                bw.Write((short) 1);
                bw.Write((short) 0);
                bw.Write(0);
                bw.Write((double) 0.0);
            }
            else if (color.ColorType == ExcelColorType.Indexed)
            {
                bw.Write((short) 1);
                bw.Write((short) (color.Tint * 32767.0));
                bw.Write(color.Value);
                bw.Write((double) 0.0);
            }
            else if (color.ColorType == ExcelColorType.RGB)
            {
                bw.Write((short) 2);
                bw.Write((short) (color.Tint * 32767.0));
                GcColor color2 = ColorExtension.FromArgb(color.Value);
                bw.Write(color2.R);
                bw.Write(color2.G);
                bw.Write(color2.B);
                bw.Write(color2.A);
                bw.Write((double) 0.0);
            }
            else
            {
                bw.Write((short) 3);
                bw.Write((short) (color.Tint * 32767.0));
                bw.Write(color.Value);
                bw.Write((double) 0.0);
            }
        }

        internal void WriteGuts(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            int rowMaxOutLineLevel = 0;
            int columnMaxOutLineLevel = 0;
            this._excelWriter.GetGutters(sheetIndex, ref rowMaxOutLineLevel, ref columnMaxOutLineLevel);
            int num3 = ((rowMaxOutLineLevel > 0) ? 5 : 0) + (12 * rowMaxOutLineLevel);
            int num4 = ((columnMaxOutLineLevel > 0) ? 5 : 0) + (12 * columnMaxOutLineLevel);
            biff.Write(writer);
            writer.Write((short) ((short) num3));
            writer.Write((short) ((short) num4));
            writer.Write((short) ((short) rowMaxOutLineLevel));
            writer.Write((short) ((short) columnMaxOutLineLevel));
        }

        internal void WriteHCenter(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            short num = this._excelWriter.GetPrintOptions(sheetIndex).HorizontalCentered ? ((short) 1) : ((short) 0);
            biff.Write(writer);
            writer.Write(num);
        }

        internal void WriteHeader(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            string header = this._excelWriter.GetPrintPageSetting(sheetIndex).Header;
            if (string.IsNullOrWhiteSpace(header))
            {
                header = this._excelWriter.GetPrintPageSetting(sheetIndex).AdvancedHeadFooterSetting.HeaderOddPage;
            }
            if ((header != null) && (header.Length > 0))
            {
                MemoryStream @this = new MemoryStream();
                BinaryWriter writer2 = new BinaryWriter((Stream) @this);
                this.WriteBiffStr(writer2, header, false, false, false, false, 2);
                biff.DataLength = (short) @this.Length;
                biff.Write(writer);
                writer.Write(@this.GetBuffer(), 0, (int) @this.Length);
            }
            else
            {
                biff.DataLength = 0;
                biff.Write(writer);
            }
        }

        private void WriteHeaderFooter(BinaryWriter writer, IExtendedHeadFooterSetting extendedHeadFooterSetting)
        {
            BiffRecord record = new BiffRecord {
                RecordType = BiffRecordNumber.HEADERFOOTER
            };
            int num = 0x26;
            List<int> list = new List<int>();
            List<XLUnicodeString> list2 = new List<XLUnicodeString>();
            if (!string.IsNullOrWhiteSpace(extendedHeadFooterSetting.HeaderEvenPage))
            {
                XLUnicodeString str = new XLUnicodeString {
                    Text = extendedHeadFooterSetting.HeaderEvenPage
                };
                if (str.fHighByte == 1)
                {
                    list.Add(str.cch * 2);
                }
                else
                {
                    list.Add(str.cch);
                }
                list2.Add(str);
            }
            else
            {
                list.Add(0);
            }
            if (!string.IsNullOrWhiteSpace(extendedHeadFooterSetting.FooterEvenPage))
            {
                XLUnicodeString str2 = new XLUnicodeString {
                    Text = extendedHeadFooterSetting.FooterEvenPage
                };
                if (str2.fHighByte == 1)
                {
                    list.Add(str2.cch * 2);
                }
                else
                {
                    list.Add(str2.cch);
                }
                list2.Add(str2);
            }
            else
            {
                list.Add(0);
            }
            if (!string.IsNullOrWhiteSpace(extendedHeadFooterSetting.HeaderFirstPage))
            {
                XLUnicodeString str3 = new XLUnicodeString {
                    Text = extendedHeadFooterSetting.HeaderFirstPage
                };
                if (str3.fHighByte == 1)
                {
                    list.Add(str3.cch * 2);
                }
                else
                {
                    list.Add(str3.cch);
                }
                list2.Add(str3);
            }
            else
            {
                list.Add(0);
            }
            if (!string.IsNullOrWhiteSpace(extendedHeadFooterSetting.FooterFirstPage))
            {
                XLUnicodeString str4 = new XLUnicodeString {
                    Text = extendedHeadFooterSetting.FooterFirstPage
                };
                if (str4.fHighByte == 1)
                {
                    list.Add(str4.cch * 2);
                }
                else
                {
                    list.Add(str4.cch);
                }
                list2.Add(str4);
            }
            else
            {
                list.Add(0);
            }
            int num2 = 0x3330;
            if (extendedHeadFooterSetting.HeaderFooterDifferentOddEvenPages)
            {
                num2 |= 1;
            }
            if (extendedHeadFooterSetting.HeaderFooterDifferentFirstPage)
            {
                num2 |= 2;
            }
            if (extendedHeadFooterSetting.HeaderFooterScalesWithDocument)
            {
                num2 |= 4;
            }
            if (extendedHeadFooterSetting.HeaderFooterAlignWithPageMargin)
            {
                num2 |= 8;
            }
            foreach (int num3 in list)
            {
                num += (num3 > 0) ? (num3 + 3) : num3;
            }
            record.DataLength = (short) num;
            record.Write(writer);
            writer.Write((short) 0x89c);
            writer.Write(new byte[0x1a]);
            writer.Write((short) ((short) num2));
            foreach (int num4 in list)
            {
                writer.Write((short) ((short) num4));
            }
            foreach (XLUnicodeString str5 in list2)
            {
                writer.Write(str5.cch);
                writer.Write(str5.fHighByte);
                writer.Write(str5.rgb);
            }
        }

        internal void WriteHorizontalPageBreaks(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            List<int> rowBreakLines = this._excelWriter.GetPrintPageSetting(sheetIndex).RowBreakLines;
            if ((rowBreakLines != null) && (rowBreakLines.Count > 0))
            {
                biff.DataLength = (short)(2 + (rowBreakLines.Count * 6));
                biff.Write(writer);
                writer.Write((ushort) ((ushort) rowBreakLines.Count));
                foreach (int num in rowBreakLines)
                {
                    writer.Write((ushort) ((ushort) num));
                    writer.Write((short) 0);
                    writer.Write((ushort) 0xff);
                }
            }
        }

        internal void WriteIndex(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
        }

        internal void WriteIteration(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            bool isIterateCalculate = this._calulationProperty.IsIterateCalculate;
            biff.Write(writer);
            if (isIterateCalculate)
            {
                writer.Write((short) 1);
            }
            else
            {
                writer.Write((short) 0);
            }
        }

        private void WriteNames(BinaryWriter writer)
        {
            List<Tuple<int, int>> list = new List<Tuple<int, int>>();
            List<string> sheetNames = this.SheetNames;
            int sheetCount = this._excelWriter.GetSheetCount();
            for (int i = 0; i < sheetCount; i++)
            {
                list.Add(this.GetSelectionTopLeftIndex(i));
            }
            bool showTabs = true;
            int selectedTabIndex = 0;
            int firstDisplayedTabIndex = 0;
            int selectedTabCount = 0;
            int tabRatio = 600;
            this._excelWriter.GetTabs(ref showTabs, ref selectedTabIndex, ref firstDisplayedTabIndex, ref selectedTabCount, ref tabRatio);
            FormulaProcess process = new FormulaProcess(-1, -1, -1, null) {
                SheetsSelectionList = list,
                sheetNames = sheetNames,
                isA1RefStyle = this._excelWriter.GetCalculationProperty().RefMode == ExcelReferenceStyle.A1,
                HasExternNames = false,
                activeSheet = selectedTabIndex
            };
            int extraDataLength = 0;
            List<Tuple<byte[], byte[], string, short, bool>> list3 = new List<Tuple<byte[], byte[], string, short, bool>>();
            HashSet<string> set = new HashSet<string>(new string[] { "AVERAGEIF", "AVERAGEIFS", "CUBEKPIMEMBER", "CUBEMEMBER", "CUBEMEMBERPROPERTY", "CUBERANKEDMEMBER", "CUBESET", "CUBESETCOUNT", "CUBEVALUE", "COUNTIFS", "IFERROR", "SUMIFS" });
            for (int j = 0; j < this._linkTable.DefinedNames.Count; j++)
            {
                Tuple<string, int, object> tuple = this._linkTable.DefinedNames[j];
                byte[] buffer = null;
                byte[] buffer2 = null;
                if (tuple.Item3 != null)
                {
                    if (tuple.Item3 is IName)
                    {
                        IName name = tuple.Item3 as IName;
                        try
                        {
                            if ((name.Name.ToUpperInvariant() != "_XLNM._FILTERDATABASE") && (name.Name.ToUpperInvariant() != "_FILTERDATABASE"))
                            {
                                NamedCellRange range = name as NamedCellRange;
                                if ((range != null) && (range.DefinitionBits != null))
                                {
                                    buffer = range.DefinitionBits.Item2;
                                    buffer2 = range.ExtraDefinitionBits.Item2;
                                }
                                else
                                {
                                    string text = process.isA1RefStyle ? name.RefersTo : name.RefersToR1C1;
                                    int num9 = (name.Index == -1) ? process.activeSheet : name.Index;
                                    Tuple<int, int> tuple2 = list[num9];
                                    text = Parser.Unparse(Parser.Parse(text, 0, 0, !process.isA1RefStyle, this._linkTable), tuple2.Item1, tuple2.Item2, !process.isA1RefStyle);
                                    process.row = tuple2.Item1;
                                    process.column = tuple2.Item2;
                                    buffer = process.ToExcelParsedFormula((short) name.Index, text, this._linkTable, ref extraDataLength, true);
                                    if (extraDataLength > 0)
                                    {
                                        byte[] buffer3 = new byte[buffer.Length - extraDataLength];
                                        byte[] buffer4 = new byte[extraDataLength];
                                        Array.Copy(buffer, buffer3, buffer3.Length);
                                        Array.Copy(buffer, buffer3.Length, buffer4, 0, extraDataLength);
                                        buffer = buffer3;
                                        buffer2 = buffer4;
                                    }
                                }
                                list3.Add(new Tuple<byte[], byte[], string, short, bool>(buffer, buffer2, name.Name, (short) name.Index, name.IsHidden));
                            }
                        }
                        catch (NotSupportedException exception)
                        {
                            this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(string.Format(ResourceHelper.GetResourceString("writeFormulaNotSupported"), (object[]) new object[] { exception.Message }), (object[]) new object[] { name.Name }), ExcelWarningCode.FormulaNotSupportError, -1, -1, -1, (Exception) exception));
                        }
                        catch (Exception exception2)
                        {
                            this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeNameError"), (object[]) new object[] { name.Name }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception2));
                        }
                    }
                    if (tuple.Item3 is IBuiltInName)
                    {
                        IBuiltInName name2 = tuple.Item3 as IBuiltInName;
                        BuiltInName name3 = name2 as BuiltInName;
                        try
                        {
                            buffer = null;
                            buffer2 = null;
                            if ((name3 != null) && (name3.NameBits != null))
                            {
                                buffer = name3.NameBits.Item2;
                                buffer2 = name3.ExtraBits.Item2;
                            }
                            else
                            {
                                buffer = process.ToExcelParsedFormula(-1, name2.Name, this._linkTable, ref extraDataLength, true);
                            }
                            if (set.Contains(name2.Name.ToUpperInvariant()))
                            {
                                list3.Add(new Tuple<byte[], byte[], string, short, bool>(buffer, buffer2, "_xlfn." + name2.Name.ToUpperInvariant(), -1, true));
                            }
                            else
                            {
                                list3.Add(new Tuple<byte[], byte[], string, short, bool>(buffer, buffer2, name2.Name, -1, true));
                            }
                        }
                        catch (Exception exception3)
                        {
                            this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeNameError"), (object[]) new object[] { name2.Name }), ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception3));
                        }
                    }
                }
                else
                {
                    try
                    {
                        string formula = "";
                        if (tuple.Item3 != null)
                        {
                            formula = tuple.Item3.ToString();
                        }
                        buffer = process.ToExcelParsedFormula(-1, formula, this._linkTable, ref extraDataLength, true);
                        list3.Add(new Tuple<byte[], byte[], string, short, bool>(buffer, null, tuple.Item1, -1, true));
                    }
                    catch (ExcelException exception4)
                    {
                        this._excelWriter.OnExcelSaveError(new ExcelWarning(exception4.Message, ExcelWarningCode.DefinedOrCustomNameError, -1, -1, -1, exception4));
                    }
                }
            }
            int num10 = this._excelWriter.GetSheetCount();
            for (short k = 0; k < num10; k++)
            {
                IExcelAutoFilter autoFilter = this._excelWriter.GetAutoFilter(k);
                if ((autoFilter != null) && (autoFilter.Range != null))
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (new BinaryWriter((Stream) stream))
                        {
                            string str3;
                            int num12 = 0;
                            int columnSpan = autoFilter.Range.ColumnSpan;
                            int rowSpan = autoFilter.Range.RowSpan;
                            if (autoFilter.Range.ColumnSpan == 0x4000)
                            {
                                columnSpan = 0x100;
                            }
                            if (autoFilter.Range.RowSpan == 0x100000)
                            {
                                rowSpan = 0x10000;
                            }
                            if (this._calulationProperty.RefMode == ExcelReferenceStyle.A1)
                            {
                                str3 = string.Format("{0}!${1}${2}:${3}${4}", (object[]) new object[] { this.GetSheetNameForAutoFilter(k), this.AppendA1Letter(autoFilter.Range.Column), ((int) (autoFilter.Range.Row + 1)), this.AppendA1Letter((autoFilter.Range.Column + columnSpan) - 1), ((int) (autoFilter.Range.Row + rowSpan)) });
                            }
                            else
                            {
                                str3 = string.Format("{0}!R{1}C{2}:R{3}C{4}", (object[]) new object[] { this.GetSheetNameForAutoFilter(k), ((int) (autoFilter.Range.Row + 1)), ((int) (autoFilter.Range.Column + 1)), ((int) (autoFilter.Range.Row + rowSpan)), ((int) (autoFilter.Range.Column + columnSpan)) });
                            }
                            try
                            {
                                process.ToExcelParsedFormula(k, str3, this._linkTable, ref num12, true);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            list3 = Enumerable.ToList<Tuple<byte[], byte[], string, short, bool>>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3);
            List<string> names = Enumerable.ToList<string>(Enumerable.Select<Tuple<byte[], byte[], string, short, bool>, string>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3, delegate (Tuple<byte[], byte[], string, short, bool> item) {
                return item.Item3;
            }));
            List<byte[]> nameDefinitions = Enumerable.ToList<byte[]>(Enumerable.Select<Tuple<byte[], byte[], string, short, bool>, byte[]>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3, delegate (Tuple<byte[], byte[], string, short, bool> item) {
                return item.Item1;
            }));
            List<byte[]> extras = Enumerable.ToList<byte[]>(Enumerable.Select<Tuple<byte[], byte[], string, short, bool>, byte[]>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3, delegate (Tuple<byte[], byte[], string, short, bool> item) {
                return item.Item2;
            }));
            List<short> tabs = Enumerable.ToList<short>(Enumerable.Select<Tuple<byte[], byte[], string, short, bool>, short>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3, delegate (Tuple<byte[], byte[], string, short, bool> item) {
                return item.Item4;
            }));
            List<bool> hiddenState = Enumerable.ToList<bool>(Enumerable.Select<Tuple<byte[], byte[], string, short, bool>, bool>((IEnumerable<Tuple<byte[], byte[], string, short, bool>>) list3, delegate (Tuple<byte[], byte[], string, short, bool> item) {
                return item.Item5;
            }));
            List<Lbl> list9 = this.GetNameRecords(names, nameDefinitions, extras, tabs, hiddenState);
            ExternSheet sheet = new ExternSheet();
            new List<Tuple<int, byte[]>>();
            List<string> list10 = new List<string>();
            for (int m = 0; m < this._linkTable.SupBooks.Count; m++)
            {
                ExcelSupBook supBook = this._linkTable.SupBooks[m];
                try
                {
                    if (supBook.Buffer != null)
                    {
                        writer.Write((short) 430);
                        writer.Write((short) supBook.Buffer.Length);
                        writer.Write(supBook.Buffer);
                        if ((supBook.ExternNameBuffers != null) && (supBook.ExternNameBuffers.Count > 0))
                        {
                            foreach (byte[] buffer5 in Enumerable.Select<Tuple<int, byte[]>, byte[]>((IEnumerable<Tuple<int, byte[]>>) Enumerable.OrderBy<Tuple<int, byte[]>, int>((IEnumerable<Tuple<int, byte[]>>) supBook.ExternNameBuffers, delegate (Tuple<int, byte[]> record) {
                                return record.Item1;
                            }), delegate (Tuple<int, byte[]> record) {
                                return record.Item2;
                            }))
                            {
                                writer.Write((short) 0x23);
                                writer.Write((short) buffer5.Length);
                                writer.Write(buffer5);
                            }
                        }
                        if (supBook.IsAddInReferencedSupBook)
                        {
                            List<IFunction> customOrFunctionNameList = this._excelWriter.GetCustomOrFunctionNameList();
                            List<Tuple<int, byte[]>> list12 = new List<Tuple<int, byte[]>>();
                            if (customOrFunctionNameList != null)
                            {
                                foreach (ExcelCustomFunction function2 in customOrFunctionNameList)
                                {
                                    if ((function2 != null) && (function2.ExternNameBits != null))
                                    {
                                        list12.Add(function2.ExternNameBits);
                                    }
                                    else
                                    {
                                        list10.Add(function2.Name);
                                    }
                                }
                            }
                            foreach (byte[] buffer6 in Enumerable.Select<Tuple<int, byte[]>, byte[]>((IEnumerable<Tuple<int, byte[]>>) Enumerable.OrderBy<Tuple<int, byte[]>, int>((IEnumerable<Tuple<int, byte[]>>) list12, delegate (Tuple<int, byte[]> record) {
                                return record.Item1;
                            }), delegate (Tuple<int, byte[]> record) {
                                return record.Item2;
                            }))
                            {
                                writer.Write((short) 0x23);
                                writer.Write((short) buffer6.Length);
                                writer.Write(buffer6);
                            }
                        }
                    }
                    else if (supBook.IsSelfReferenced)
                    {
                        GetSelfReferencingSupBook((ushort) supBook.SheetCount).Write(writer);
                    }
                    else if (supBook.IsAddInReferencedSupBook)
                    {
                        GetAddInReferencingSupBook().Write(writer);
                        if ((this._linkTable.CustomOrFuctionNames != null) && (this._linkTable.CustomOrFuctionNames.Count > 0))
                        {
                            List<ExternName> list13 = new List<ExternName>();
                            list13.AddRange((IEnumerable<ExternName>) GetExternNameRecords(this._linkTable.CustomOrFuctionNames));
                            for (int num16 = 0; num16 < list13.Count; num16++)
                            {
                                list13[num16].Write(writer);
                            }
                        }
                    }
                    else if (supBook.IsCurrentSheetSupBook)
                    {
                        new BiffRecord { RecordType = BiffRecordNumber.SUPBOOK, DataLength = 6 }.Write(writer);
                        writer.Write((short) 0);
                        writer.Write((short) 1);
                        writer.Write((short) 0);
                    }
                    else
                    {
                        string fileName = supBook.FileName;
                        if (this._linkTable.ExternalNamedCellRanges.ContainsKey(fileName))
                        {
                            List<IName> list14 = this._linkTable.ExternalNamedCellRanges[fileName];
                            List<string> list15 = new List<string>();
                            foreach (IName name4 in list14)
                            {
                                list15.Add(name4.Name);
                            }
                            List<ExternName> list16 = new List<ExternName>();
                            list16.AddRange((IEnumerable<ExternName>) GetExternNameRecords(list15));
                            GetExternWorkBookSupBook(supBook).Write(writer);
                            for (int num17 = 0; num17 < list16.Count; num17++)
                            {
                                list16[num17].Write(writer);
                            }
                        }
                        else
                        {
                            GetExternWorkBookSupBook(supBook).Write(writer);
                        }
                    }
                }
                catch (Exception exception5)
                {
                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeExternSheetError"), ExcelWarningCode.ExcelExternSheetError, -1, -1, -1, exception5));
                }
            }
            foreach (ExcelExternSheet sheet2 in this._linkTable.ExternalSheets)
            {
                sheet.AddXti(GetXti((ushort) sheet2.supBookIndex, (short) sheet2.beginSheetIndex, (short) sheet2.endSheetIndex));
            }
            if (sheet.rgXTI != null)
            {
                sheet.Write(writer);
            }
            for (int n = 0; (list9 != null) && (n < list9.Count); n++)
            {
                list9[n].Write(writer);
            }
            for (short num19 = 0; num19 < num10; num19++)
            {
                IExcelAutoFilter filter2 = this._excelWriter.GetAutoFilter(num19);
                try
                {
                    if ((filter2 != null) && (filter2.Range != null))
                    {
                        byte[] buffer7;
                        string str5 = null;
                        int num20 = filter2.Range.ColumnSpan;
                        int num21 = filter2.Range.RowSpan;
                        if (filter2.Range.ColumnSpan == 0x4000)
                        {
                            num20 = 0x100;
                        }
                        if (filter2.Range.RowSpan == 0x100000)
                        {
                            num21 = 0x10000;
                        }
                        if (this._calulationProperty.RefMode == ExcelReferenceStyle.A1)
                        {
                            str5 = string.Format("{0}!${1}${2}:${3}${4}", (object[]) new object[] { this.GetSheetNameForAutoFilter(num19), this.AppendA1Letter(filter2.Range.Column), ((int) (filter2.Range.Row + 1)), this.AppendA1Letter((filter2.Range.Column + num20) - 1), ((int) (filter2.Range.Row + num21)) });
                        }
                        else
                        {
                            str5 = string.Format("{0}!R{1}C{2}:R{3}C{4}", (object[]) new object[] { this.GetSheetNameForAutoFilter(num19), ((int) (filter2.Range.Row + 1)), ((int) (filter2.Range.Column + 1)), ((int) (filter2.Range.Row + num21)), ((int) (filter2.Range.Column + num20)) });
                        }
                        using (MemoryStream stream2 = new MemoryStream())
                        {
                            using (BinaryWriter writer3 = new BinaryWriter((Stream) stream2))
                            {
                                int num22 = 0;
                                byte[] buffer8 = process.ToExcelParsedFormula(num19, str5, this._linkTable, ref num22, true);
                                ushort num23 = 0;
                                num23 |= 1;
                                num23 |= 0x20;
                                writer3.Write(num23);
                                writer3.Write((byte) 0);
                                writer3.Write((byte) 1);
                                writer3.Write((ushort) buffer8.Length);
                                writer3.Write((short) 0);
                                writer3.Write((short) (num19 + 1));
                                writer3.Write(0);
                                writer3.Write((byte) 0);
                                writer3.Write((byte) 13);
                                writer3.Write(buffer8);
                            }
                            buffer7 = stream2.ToArray();
                        }
                        BiffRecord record2 = new BiffRecord {
                            RecordType = BiffRecordNumber.NAME
                        };
                        record2.DataLength = (short)buffer7.Length;
                        record2.Write(writer);
                        writer.Write(buffer7);
                    }
                }
                catch (Exception exception6)
                {
                    this._excelWriter.OnExcelSaveError(new ExcelWarning(ResourceHelper.GetResourceString("writeAutoFilterError"), ExcelWarningCode.General, num19, -1, -1, exception6));
                }
            }
        }

        private void WriteNumber(BinaryWriter writer, BiffRecord biff, double value, ushort row, short column, short ixf)
        {
            writer.Write(0xe0203);
            writer.Write(row);
            writer.Write(column);
            writer.Write(ixf);
            writer.Write(value);
        }

        internal void WritePALETTE(BinaryWriter writer)
        {
            Dictionary<int, GcColor> colorPalette = this._excelWriter.GetColorPalette();
            if (colorPalette != null)
            {
                new BiffRecord { RecordType = BiffRecordNumber.PALETTE, DataLength = 0xe2 }.Write(writer);
                writer.Write((ushort) 0x38);
                for (int i = 8; i < 0x40; i++)
                {
                    GcColor color = colorPalette[i];
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                    writer.Write(color.A);
                }
            }
        }

        internal void WritePane(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            int frozenColumnCount = 0;
            int frozenRowCount = 0;
            int frozenTrailingColumnCount = 0;
            int frozenTrailingRowCount = 0;
            int leftmostVisibleColumn = 0;
            int topVisibleRow = 0;
            int paneIndex = 3;
            this._excelWriter.GetFrozen(sheetIndex, ref frozenRowCount, ref frozenColumnCount, ref frozenTrailingRowCount, ref frozenTrailingColumnCount);
            if ((frozenColumnCount > 0) || (frozenRowCount > 0))
            {
                if (frozenColumnCount > 0)
                {
                    leftmostVisibleColumn = frozenColumnCount;
                }
                else
                {
                    leftmostVisibleColumn = 0x40;
                }
                if (frozenRowCount > 0)
                {
                    topVisibleRow = frozenRowCount;
                }
                else
                {
                    topVisibleRow = 0x40;
                }
                if ((frozenColumnCount > 0) && (frozenRowCount == 0))
                {
                    paneIndex = 1;
                }
                else if ((frozenColumnCount == 0) && (frozenRowCount > 0))
                {
                    paneIndex = 2;
                }
                else if ((frozenColumnCount > 0) && (frozenRowCount > 0))
                {
                    paneIndex = 0;
                }
                biff.Write(writer);
                writer.Write((ushort) ((ushort) frozenColumnCount));
                writer.Write((ushort) ((ushort) frozenRowCount));
                writer.Write((ushort) ((ushort) topVisibleRow));
                writer.Write((ushort) ((ushort) leftmostVisibleColumn));
                writer.Write((ushort) ((ushort) paneIndex));
            }
            else
            {
                this._excelWriter.GetPane(sheetIndex, ref frozenColumnCount, ref frozenRowCount, ref topVisibleRow, ref leftmostVisibleColumn, ref paneIndex);
                if ((frozenColumnCount > 0) || (frozenRowCount > 0))
                {
                    if (frozenColumnCount == 0)
                    {
                        leftmostVisibleColumn = 0x40;
                    }
                    if (frozenRowCount == 0)
                    {
                        topVisibleRow = 0x40;
                    }
                    biff.Write(writer);
                    writer.Write((ushort) ((ushort) frozenColumnCount));
                    writer.Write((ushort) ((ushort) frozenRowCount));
                    writer.Write((ushort) ((ushort) topVisibleRow));
                    writer.Write((ushort) ((ushort) leftmostVisibleColumn));
                    writer.Write((ushort) ((ushort) paneIndex));
                }
            }
        }

        internal void WritePrintGridLines(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            writer.Write(this._excelWriter.GetPrintOptions(sheetIndex).PrintGridLine ? ((short) 1) : ((short) 0));
        }

        internal void WritePrintHeaders(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            writer.Write(this._excelWriter.GetPrintOptions(sheetIndex).PrintRowColumnsHeaders ? ((short) 1) : ((short) 0));
        }

        private void WritePrintPageSetup(short sheet, BinaryWriter writer, BiffRecord biff)
        {
            biff.RecordType = BiffRecordNumber.PRINTHEADERS;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.PRINTGRIDLINES;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.GRIDSET;
            biff.DataLength = 2;
            biff.Write(writer);
            writer.Write((short) 1);
            biff.RecordType = BiffRecordNumber.GUTS;
            biff.DataLength = 8;
            biff.Write(writer);
            writer.Write((double) 0.0);
            biff.RecordType = BiffRecordNumber.DEFAULTROWHEIGHT;
            biff.DataLength = 4;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.WSBOOL;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.HEADER;
            biff.DataLength = 0;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.FOOTER;
            biff.DataLength = 0;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.HCENTER;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.VCENTER;
            biff.DataLength = 2;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.HORIZONTALPAGEBREAKS;
            biff.DataLength = 0;
            this.WriteRecord(writer, biff, sheet);
            biff.RecordType = BiffRecordNumber.VERTICALPAGEBREAKS;
            biff.DataLength = 0;
            this.WriteRecord(writer, biff, sheet);
        }

        internal void WriteProtect(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            bool isProtect = false;
            this._excelWriter.GetProtect(sheetIndex, ref isProtect);
            biff.Write(writer);
            writer.Write(isProtect ? ((short) 1) : ((short) 0));
        }

        private void WriteRange(BinaryWriter writer, IRange range)
        {
            writer.Write((ushort) ((ushort) range.Row));
            if (range.RowSpan == 0x100000)
            {
                writer.Write((ushort) 0xffff);
            }
            else
            {
                writer.Write((ushort) ((range.Row + range.RowSpan) - 1));
            }
            writer.Write((ushort) ((ushort) range.Column));
            if (range.ColumnSpan == 0x4000)
            {
                writer.Write((ushort) 0xff);
            }
            else
            {
                writer.Write((ushort) ((range.Column + range.ColumnSpan) - 1));
            }
        }

        private void WriteRecord(BinaryWriter writer, BiffRecord biff, short sheet)
        {
            BiffRecordNumber recordType = biff.RecordType;
            if (recordType <= BiffRecordNumber.SAVERECALC)
            {
                switch (recordType)
                {
                    case BiffRecordNumber.PANE:
                        this.WritePane(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.DEFCOLWIDTH:
                        this.WriteDefaultColumnWidth(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.SAVERECALC:
                        this.WriteSaveReCalc(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.CALCCOUNT:
                        this.WriteCalcCount(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.CALCMODE:
                        this.WriteCalcMode(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.PRECISION:
                    case BiffRecordNumber.PASSWORD:
                    case BiffRecordNumber.EXTERNCOUNT:
                    case BiffRecordNumber.EXTERNSHEET:
                    case BiffRecordNumber.NAME:
                    case BiffRecordNumber.WINDOWPROTECT:
                        return;

                    case BiffRecordNumber.REFMODE:
                        this.WriteRefMode(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.DELTA:
                        this.WriteDelta(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.ITERATION:
                        this.WriteIteration(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.PROTECT:
                        this.WriteProtect(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.HEADER:
                        this.WriteHeader(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.FOOTER:
                        this.WriteFooter(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.VERTICALPAGEBREAKS:
                        this.WriteVerticalPageBreaks(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.HORIZONTALPAGEBREAKS:
                        this.WriteHorizontalPageBreaks(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.PRINTHEADERS:
                        this.WritePrintHeaders(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.PRINTGRIDLINES:
                        this.WritePrintGridLines(writer, biff, sheet);
                        return;
                }
            }
            else if (recordType <= BiffRecordNumber.SETUP)
            {
                switch (recordType)
                {
                    case BiffRecordNumber.GUTS:
                        this.WriteGuts(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.WSBOOL:
                        this.WriteWSBool(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.GRIDSET:
                        return;

                    case BiffRecordNumber.HCENTER:
                        this.WriteHCenter(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.VCENTER:
                        this.WriteVCenter(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.SCL:
                        this.WriteSCL(writer, biff, sheet);
                        return;

                    case BiffRecordNumber.SETUP:
                        this.WriteSETUP(writer, biff, sheet);
                        return;
                }
            }
            else
            {
                if (recordType != BiffRecordNumber.INDEX)
                {
                    if (recordType != BiffRecordNumber.DEFAULTROWHEIGHT)
                    {
                        if (recordType == BiffRecordNumber.WINDOW2)
                        {
                            this.WriteWINDOW2(writer, biff, sheet);
                        }
                        return;
                    }
                }
                else
                {
                    this.WriteIndex(writer, biff, sheet);
                    return;
                }
                this.WriteDefaultRowHeight(writer, biff, sheet);
            }
        }

        internal void WriteRefMode(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            short num = 0;
            if (this._calulationProperty.RefMode == ExcelReferenceStyle.A1)
            {
                num = 1;
            }
            biff.Write(writer);
            writer.Write(num);
        }

        private void WriteRowRecord(BinaryWriter writer, short sheetIndex, IExcelRow row, ushort height, bool isDefaultRowheight)
        {
            writer.Write(0x100208);
            writer.Write((ushort) ((ushort) row.Index));
            writer.Write(0);
            ushort num = row.CustomHeight ? ((ushort) (height & 0x7fff)) : ((ushort) (height | 0x8000));
            writer.Write(num);
            writer.Write(0);
            ushort num2 = 0;
            num2 |= row.OutLineLevel;
            num2 |= (ushort)(row.Collapsed ? 0x10 : 0);
            num2 |= (ushort)(!row.Visible ? 0x20 : 0);
            if (row.CustomHeight)
            {
                num2 |= 0x40;
            }
            if (row.FormatId >= 0)
            {
                num2 |= 0x80;
            }
            num2 |= 0x100;
            writer.Write(num2);
            writer.Write((row.FormatId >= 0) ? ((short) (row.FormatId + this._offSet)) : ((short) 15));
        }

        private void WriterTableStyles(BinaryWriter writer)
        {
            if (this._excelWriter is IExcelTableWriter)
            {
                IExcelTableWriter writer2 = this._excelWriter as IExcelTableWriter;
                bool flag = false;
                int sheetCount = this._excelWriter.GetSheetCount();
                for (int i = 0; i < sheetCount; i++)
                {
                    List<IExcelTable> sheetTables = writer2.GetSheetTables(i);
                    if ((sheetTables != null) && (sheetTables.Count > 0))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    string defaultTableStyleName = writer2.GetDefaultTableStyleName();
                    string defaultPivotTableStyleName = writer2.GetDefaultPivotTableStyleName();
                    if (string.IsNullOrWhiteSpace(defaultTableStyleName))
                    {
                        defaultTableStyleName = "TableStyleMedium2";
                    }
                    if (string.IsNullOrWhiteSpace(defaultPivotTableStyleName))
                    {
                        defaultPivotTableStyleName = "PivotStyleMedium9";
                    }
                    List<IExcelTableStyle> tableStyles = writer2.GetTableStyles();
                    byte[] bytes = Encoding.Unicode.GetBytes(defaultTableStyleName);
                    byte[] buffer = Encoding.Unicode.GetBytes(defaultPivotTableStyleName);
                    new BiffRecord { RecordType = BiffRecordNumber.TABLESTYLES, DataLength = (short)((20 + buffer.Length) + bytes.Length) }.Write(writer);
                    writer.Write((short) 0x88e);
                    writer.Write((short) 0);
                    writer.Write((double) 0.0);
                    if (tableStyles != null)
                    {
                        writer.Write((int) (tableStyles.Count + 0x90));
                    }
                    else
                    {
                        writer.Write(0);
                    }
                    writer.Write((short) (bytes.Length / 2));
                    writer.Write((short) (buffer.Length / 2));
                    writer.Write(bytes);
                    writer.Write(buffer);
                    if (tableStyles != null)
                    {
                        foreach (IExcelTableStyle style in tableStyles)
                        {
                            this.WriteTableStyle(writer, style);
                        }
                    }
                }
            }
        }

        internal void WriteSaveReCalc(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            writer.Write(this._calulationProperty.ReCalculationBeforeSave ? ((short) 1) : ((short) 0));
        }

        internal void WriteSCL(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
        }

        internal void WriteSETUP(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            short num = 0;
            short num2 = 0;
            ushort num3 = 0;
            IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheetIndex);
            IExcelPrintPageMargin printPageMargin = this._excelWriter.GetPrintPageMargin(sheetIndex);
            num3 = (ushort)((((((((((printPageSetting.PageOrder == ExcelPrintPageOrder.OverThenDown) ? 1 : 0) | ((printPageSetting.Orientation == ExcelPrintOrientation.Portrait) ? 2 : 0)) | ((Math.Abs((double) (printPageSetting.ZoomFactor - 1.0)) > 0.01) ? 0 : 4)) | (printPageSetting.ShowColor ? 0 : 8)) | (printPageSetting.Draft ? 0x10 : 0)) | ((printPageSetting.CommentsStyle != ExcelPrintNotesStyle.None) ? 0x20 : 0)) | ((printPageSetting.Orientation == ExcelPrintOrientation.Auto) ? 0x40 : 0)) | (printPageSetting.UseCustomStartingPage ? 0x80 : 0)) | ((printPageSetting.CommentsStyle == ExcelPrintNotesStyle.AtEnd) ? 0x200 : 0));
            ushort num4 = (ushort)(((int) printPageSetting.CellErrorPrintStyle) << 10);
            num3 |= num4;
            if (printPageSetting.Orientation != ExcelPrintOrientation.Auto)
            {
                num3 = (ushort)(num3 & 0xffbb);
                if (printPageSetting.Orientation == ExcelPrintOrientation.Portrait)
                {
                    num3 |= 2;
                }
                else
                {
                    num3 = (ushort)(num3 & 0xfffd);
                }
            }
            biff.Write(writer);
            writer.Write((short) ((short) printPageSetting.PaperSizeIndex));
            writer.Write((short) (printPageSetting.ZoomFactor * 100f));
            writer.Write(printPageSetting.FirstPageNumber);
            writer.Write((short) ((short) printPageSetting.SmartPrintPagesWidth));
            writer.Write((short) ((short) printPageSetting.SmartPrintPagesHeight));
            writer.Write(num3);
            writer.Write(num);
            writer.Write(num2);
            writer.Write(printPageMargin.Header);
            writer.Write(printPageMargin.Footer);
            writer.Write(printPageSetting.Copies);
            if (printPageSetting.AdvancedHeadFooterSetting != null)
            {
                this.WriteHeaderFooter(writer, printPageSetting.AdvancedHeadFooterSetting);
            }
        }

        private void WriteSheetTabColor(BinaryWriter writer, short sheet)
        {
            if (this._excelWriter is IExcelWriter2)
            {
                IExcelColor sheetTabColor = (this._excelWriter as IExcelWriter2).GetSheetTabColor(sheet);
                if (sheetTabColor != null)
                {
                    new BiffRecord { RecordType = BiffRecordNumber.SHEETEXT, DataLength = 40 }.Write(writer);
                    writer.Write(0x862);
                    writer.Write((double) 0.0);
                    writer.Write(40);
                    int paletteColor = (int) this._excelWriter.GetPaletteColor(sheetTabColor);
                    if ((paletteColor < 8) || (paletteColor > 0x3f))
                    {
                        paletteColor = 0x7f;
                    }
                    writer.Write(paletteColor);
                    paletteColor = 8 | paletteColor;
                    writer.Write(paletteColor);
                    this.WriteCFColor(writer, sheetTabColor);
                }
            }
        }

        private void WriteSheetTables(BinaryWriter writer, short sheet)
        {
            if (this._excelWriter is IExcelTableWriter)
            {
                List<IExcelTable> sheetTables = (this._excelWriter as IExcelTableWriter).GetSheetTables(sheet);
                if ((sheetTables != null) && (sheetTables.Count > 0))
                {
                    this.nextTableId += sheetTables.Count;
                    this.WriteFeather(writer);
                    this.WriteFeather11(writer, this.nextTableId);
                    foreach (IExcelTable table in sheetTables)
                    {
                        this.WriteTable(writer, sheet, table);
                    }
                }
            }
        }

        private void WriteStyleExtensionRecord(BinaryWriter writer, IExcelStyle style)
        {
            if (!string.IsNullOrWhiteSpace(style.Name))
            {
                BiffRecord record = new BiffRecord {
                    RecordType = BiffRecordNumber.STYLEEXT
                };
                ExcelStyle style2 = style as ExcelStyle;
                byte[] bytes = Encoding.Unicode.GetBytes(style.Name);
                MemoryStream stream = new MemoryStream();
                BinaryWriter writer2 = new BinaryWriter((Stream) stream);
                int num = this.WriteXfProps(writer2, style);
                writer2.Flush();
                writer2.Dispose();
                byte[] buffer = stream.ToArray();
                record.DataLength = (short)((0x16 + bytes.Length) + ((buffer.Length > 0) ? buffer.Length : 0));
                record.Write(writer);
                writer.Write((short) 0x892);
                writer.Write((short) 0);
                writer.Write((double) 0.0);
                int num2 = 0;
                if (style.IsBuiltInStyle)
                {
                    num2 |= 1;
                }
                if ((style2 != null) && style2.IsCustomBuiltIn)
                {
                    num2 |= 4;
                }
                writer.Write((byte) ((byte) num2));
                if (style2 != null)
                {
                    writer.Write((byte) ((byte) style2.GetBuiltInStyleCategory()));
                }
                else
                {
                    writer.Write((byte) 0);
                }
                if ((style2 != null) && style2.IsBuiltInStyle)
                {
                    writer.Write((byte) ((byte) style2.BuiltInStyle));
                    writer.Write(style2.OutLineLevel);
                }
                else
                {
                    writer.Write((ushort) 0xffff);
                }
                writer.Write((ushort) (bytes.Length / 2));
                writer.Write(bytes);
                writer.Write((short) 0);
                writer.Write((ushort) ((ushort) num));
                if (buffer.Length > 0)
                {
                    writer.Write(buffer);
                }
            }
        }

        private void WriteStyleRecord(BinaryWriter writer)
        {
            List<IExcelStyle> excelStyles = this._excelWriter.GetExcelStyles();
            if ((excelStyles == null) || (excelStyles.Count == 0))
            {
                IExtendedFormat excelDefaultCellFormat = this._excelWriter.GetExcelDefaultCellFormat();
                if (excelDefaultCellFormat != null)
                {
                    ExcelStyle style = new ExcelStyle {
                        BuiltInStyle = BuiltInStyleIndex.Normal,
                        Name = "Normal",
                        Format = excelDefaultCellFormat
                    };
                    excelStyles = new List<IExcelStyle> {
                        style
                    };
                }
                else
                {
                    excelStyles = new List<IExcelStyle> {
                        BuiltInExcelStyles.GetNormalStyle()
                    };
                }
            }
            if ((excelStyles != null) && (excelStyles.Count > 0))
            {
                foreach (IExcelStyle style2 in excelStyles)
                {
                    ExcelStyle style3 = style2 as ExcelStyle;
                    if (style3 != null)
                    {
                        BiffRecord record = new BiffRecord {
                            RecordType = BiffRecordNumber.STYLE
                        };
                        int xfId = this.GetXfId(style3.Format);
                        int builtInStyle = (int) style3.BuiltInStyle;
                        if (((builtInStyle > 0) && (builtInStyle <= 9)) || (style3.Name == "Normal"))
                        {
                            record.DataLength = 4;
                            record.Write(writer);
                            ushort num3 = (ushort) xfId;
                            num3 |= 0x8000;
                            writer.Write(num3);
                            writer.Write((byte) ((byte) style3.BuiltInStyle));
                            writer.Write(style3.OutLineLevel);
                            this.WriteStyleExtensionRecord(writer, style3);
                        }
                        else
                        {
                            XLUnicodeString str = new XLUnicodeString {
                                fHighByte = 1,
                                Text = style3.Name
                            };
                            record.DataLength = (short)(2 + str.DataLength);
                            record.Write(writer);
                            ushort num4 = (ushort) xfId;
                            num4 &= 0xfff;
                            writer.Write(num4);
                            str.Write(writer);
                            this.WriteStyleExtensionRecord(writer, style3);
                        }
                    }
                    else
                    {
                        BiffRecord record2 = new BiffRecord {
                            RecordType = BiffRecordNumber.STYLE
                        };
                        int num5 = this.GetXfId(style2.Format);
                        XLUnicodeString str2 = new XLUnicodeString {
                            fHighByte = 1,
                            Text = style2.Name
                        };
                        record2.DataLength = (short)(2 + str2.DataLength);
                        record2.Write(writer);
                        ushort num6 = (ushort) num5;
                        num6 &= 0xfff;
                        writer.Write(num6);
                        str2.Write(writer);
                        this.WriteStyleExtensionRecord(writer, style2);
                    }
                }
            }
        }

        private void WriteTable(BinaryWriter writer, short sheet, IExcelTable table)
        {
            bool flag = false;
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer2 = new BinaryWriter((Stream) stream);
            writer2.Write(0);
            writer2.Write(table.Id);
            if (table.ShowHeaderRow)
            {
                writer2.Write(1);
            }
            else
            {
                writer2.Write(0);
            }
            if (table.ShowTotalsRow)
            {
                writer2.Write(1);
            }
            else
            {
                writer2.Write(0);
            }
            writer2.Write(0);
            writer2.Write(0x40);
            writer2.Write((short) 0x3266);
            writer2.Write((short) 0);
            int num = 0;
            if (table.AutoFilter != null)
            {
                num |= 6;
            }
            if (table.ShowTotalsRow)
            {
                num |= 0x40;
            }
            writer2.Write((byte) ((byte) num));
            num = 0;
            if (table.AutoFilter != null)
            {
                num |= 8;
            }
            writer2.Write((byte) ((byte) num));
            writer2.Write((byte) 0xde);
            writer2.Write((byte) 0);
            writer2.Write((uint) 0);
            writer2.Write((uint) 0);
            writer2.Write((uint) 0);
            writer2.Write((uint) 0);
            writer2.Write(0);
            writer2.Write(0);
            writer2.Write(0);
            writer2.Write(0);
            new XLUnicodeString { Text = table.Name }.Write(writer2);
            List<IExcelTableColumn> columns = table.Columns;
            if (table.Columns != null)
            {
                writer2.Write((short) ((short) table.Columns.Count));
            }
            else
            {
                writer2.Write((short) 0);
            }
            XLUnicodeString str = new XLUnicodeString {
                Text = ((int) table.Id).ToString()
            };
            str.Write(writer2);
            List<IExcelFilterColumn> list2 = new List<IExcelFilterColumn>();
            if (columns != null)
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    IExcelTableColumn column = columns[i];
                    writer2.Write(column.Id);
                    writer2.Write((uint) 0);
                    writer2.Write((uint) 0);
                    writer2.Write((int) column.TotalsRowFunction);
                    writer2.Write((uint) 0);
                    writer2.Write((uint) uint.MaxValue);
                    int num3 = 0;
                    if (table.AutoFilter != null)
                    {
                        num3 |= 1;
                    }
                    if (!string.IsNullOrWhiteSpace(column.CalculatedColumnFormula))
                    {
                        num3 |= 8;
                    }
                    if (column.TotalsRowFunction == ExcelTableTotalsRowFunction.Custom)
                    {
                        flag = true;
                        num3 |= 0x80;
                    }
                    if (column.TotalsRowFunctionIsArrayFormula)
                    {
                        num3 |= 0x100;
                    }
                    if (!string.IsNullOrWhiteSpace(column.TotalsRowLabel))
                    {
                        flag = true;
                        num3 |= 0x400;
                    }
                    writer2.Write(num3);
                    writer2.Write((uint) 0);
                    writer2.Write((uint) uint.MaxValue);
                    str = new XLUnicodeString {
                        Text = "0"
                    };
                    str.Write(writer2);
                    string name = column.Name;
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        name = "Column" + ((int) column.Id).ToString();
                    }
                    str = new XLUnicodeString {
                        Text = name
                    };
                    str.Write(writer2);
                    if (table.AutoFilter != null)
                    {
                        IExcelFilterColumn item = null;
                        foreach (IExcelFilterColumn column3 in table.AutoFilter.FilterColumns)
                        {
                            if (column3.AutoFilterColumnId == i)
                            {
                                item = column3;
                                break;
                            }
                        }
                        if (item == null)
                        {
                            writer2.Write(0);
                            writer2.Write((short) (-1));
                        }
                        else
                        {
                            IExcelCustomFilters customFilters = item.CustomFilters;
                            IExcelTop10Filter filter = item.Top10;
                            if ((customFilters != null) || (filter != null))
                            {
                                byte[] buffer = this.GetAutoFilterTop10Buffer(item, customFilters, filter);
                                writer2.Write((int) buffer.Length);
                                writer2.Write((short) (-1));
                                writer2.Write(buffer);
                            }
                            else
                            {
                                byte[] autoFilterCustomBuffer = this.GetAutoFilterCustomBuffer(item);
                                writer2.Write((int) autoFilterCustomBuffer.Length);
                                writer2.Write((short) (-1));
                                writer2.Write(autoFilterCustomBuffer);
                            }
                            if ((item.DynamicFilter != null) || (item.Filters != null))
                            {
                                list2.Add(item);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(column.TotalsRowCustomFunction))
                    {
                        FormulaProcess process = new FormulaProcess {
                            isA1RefStyle = this._excelWriter.GetCalculationProperty().RefMode == ExcelReferenceStyle.A1
                        };
                        int extraDataLength = 0;
                        try
                        {
                            byte[] buffer3 = process.ToExcelParsedFormula(sheet, column.TotalsRowCustomFunction, this._linkTable, ref extraDataLength, false);
                            writer2.Write((ushort) buffer3.Length);
                            writer2.Write(buffer3);
                        }
                        catch (ExcelException exception)
                        {
                            if (exception.Code == ExcelExceptionCode.ParseException)
                            {
                                this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeTableFormulaParseError"), (object[]) new object[] { exception.Message }), ExcelWarningCode.FormulaError, sheet, -1, -1, exception));
                            }
                            else
                            {
                                this._excelWriter.OnExcelSaveError(new ExcelWarning(string.Format(ResourceHelper.GetResourceString("writeTableError"), (object[]) new object[] { table.Name }), ExcelWarningCode.FormulaError, sheet, -1, -1, exception));
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(column.TotalsRowLabel))
                    {
                        new XLUnicodeString { Text = column.TotalsRowLabel }.Write(writer2);
                    }
                    if (!table.ShowHeaderRow)
                    {
                        writer2.Write(0);
                    }
                }
            }
            writer2.Flush();
            writer2.Close();
            byte[] buffer4 = stream.ToArray();
            MemoryStream stream2 = new MemoryStream();
            BinaryWriter writer3 = new BinaryWriter((Stream) stream2);
            if (!flag)
            {
                writer3.Write((short) 0x872);
            }
            else
            {
                writer3.Write((short) 0x878);
            }
            writer3.Write((short) 1);
            this.WriteRange(writer3, table.Range);
            writer3.Write((short) 5);
            writer3.Write((byte) 0);
            writer3.Write(0);
            writer3.Write((short) 1);
            writer3.Write(0);
            writer3.Write((short) 0);
            this.WriteRange(writer3, table.Range);
            writer3.Write(buffer4);
            writer3.Flush();
            writer3.Close();
            byte[] buffer5 = stream2.ToArray();
            BiffRecord record = new BiffRecord();
            if (!flag)
            {
                record.RecordType = BiffRecordNumber.FEAT11;
            }
            else
            {
                record.RecordType = BiffRecordNumber.FEAT12;
            }
            record.DataLength = (short)buffer5.Length;
            record.Write(writer);
            writer.Write(buffer5);
            new BiffRecord { RecordType = BiffRecordNumber.LIST12, DataLength = 0x36 }.Write(writer);
            writer.Write((short) 0x877);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 0);
            writer.Write((uint) table.Id);
            writer.Write(0);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(-1);
            writer.Write(0);
            writer.Write(0);
            writer.Write(0);
            if (list2.Count > 0)
            {
                IExcelAutoFilter autoFilter = table.AutoFilter;
                int num5 = Math.Min(autoFilter.Range.RowSpan, 0x10000);
                int num6 = Math.Min(autoFilter.Range.ColumnSpan, 0x100);
                Tuple<short, short> startRowColumnPair = new Tuple<short, short>((short) autoFilter.Range.Column, (short) autoFilter.Range.Row);
                Tuple<short, short> endRowColumnPair = new Tuple<short, short>((short) ((autoFilter.Range.Column + num6) - 1), (short) ((autoFilter.Range.Row + num5) - 1));
                foreach (IExcelFilterColumn column4 in list2)
                {
                    this.WriteAutoFilterColumn(writer, column4, startRowColumnPair, endRowColumnPair, true, table.Id);
                }
            }
            IExcelTableStyleInfo tableStyleInfo = table.TableStyleInfo;
            str = new XLUnicodeString {
                Text = tableStyleInfo.Name
            };
            new BiffRecord { RecordType = BiffRecordNumber.LIST12, DataLength = (short)(20 + str.DataLength) }.Write(writer);
            writer.Write((short) 0x877);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 1);
            writer.Write((uint) table.Id);
            int num7 = 0;
            if (tableStyleInfo.ShowFirstColumn)
            {
                num7 |= 1;
            }
            if (tableStyleInfo.ShowLastColumn)
            {
                num7 |= 2;
            }
            if (tableStyleInfo.ShowRowStripes)
            {
                num7 |= 4;
            }
            if (tableStyleInfo.ShowColumnStripes)
            {
                num7 |= 8;
            }
            writer.Write((ushort) ((ushort) num7));
            str.Write(writer);
            str = new XLUnicodeString {
                Text = table.Name
            };
            XLUnicodeString str3 = new XLUnicodeString {
                Text = ""
            };
            new BiffRecord { RecordType = BiffRecordNumber.LIST12, DataLength = (short)((0x12 + str.DataLength) + str3.DataLength) }.Write(writer);
            writer.Write((short) 0x877);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 2);
            writer.Write((uint) table.Id);
            str.Write(writer);
            str3.Write(writer);
        }

        private void WriteTableStyle(BinaryWriter writer, IExcelTableStyle tableStyle)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(tableStyle.Name);
            new BiffRecord { RecordType = BiffRecordNumber.TABLESTYLE, DataLength = (short)(20 + bytes.Length) }.Write(writer);
            writer.Write((short) 0x88f);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            int num = 0;
            if (tableStyle.IsPivotStyle)
            {
                num |= 2;
            }
            if (tableStyle.IsTableStyle)
            {
                num |= 4;
            }
            writer.Write((short) ((short) num));
            List<IExcelTableStyleElement> tableStyleElements = tableStyle.TableStyleElements;
            if (tableStyleElements != null)
            {
                writer.Write(tableStyleElements.Count);
            }
            else
            {
                writer.Write(0);
            }
            writer.Write((short) (bytes.Length / 2));
            writer.Write(bytes);
            if (tableStyleElements != null)
            {
                foreach (IExcelTableStyleElement element in tableStyleElements)
                {
                    this.WritetTableStyleElement(writer, element);
                }
            }
        }

        private void WritetTableStyleElement(BinaryWriter writer, IExcelTableStyleElement styleElement)
        {
            new BiffRecord { RecordType = BiffRecordNumber.TABLESTYLEELEMENT, DataLength = 0x18 }.Write(writer);
            writer.Write((short) 0x890);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((int) styleElement.Type);
            writer.Write((styleElement.Size >= 0) ? styleElement.Size : 0);
            writer.Write((styleElement.DifferentFormattingIndex >= 0) ? styleElement.DifferentFormattingIndex : 0);
        }

        private void WriteUnsupportRecord(BinaryWriter writer, int sheetIndex, BiffRecordNumber recordType)
        {
            BiffRecord unsupportRecord = this.GetUnsupportRecord(sheetIndex, recordType);
            if (unsupportRecord != null)
            {
                unsupportRecord.Write(writer);
            }
        }

        internal void WriteVCenter(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            short num = this._excelWriter.GetPrintOptions(sheetIndex).VerticalCentered ? ((short) 1) : ((short) 0);
            biff.Write(writer);
            writer.Write(num);
        }

        internal void WriteVerticalPageBreaks(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            List<int> columnBreakLines = this._excelWriter.GetPrintPageSetting(sheetIndex).ColumnBreakLines;
            if ((columnBreakLines != null) && (columnBreakLines.Count > 0))
            {
                biff.DataLength = (short)(2 + (columnBreakLines.Count * 6));
                biff.Write(writer);
                writer.Write((ushort) ((ushort) columnBreakLines.Count));
                foreach (int num in columnBreakLines)
                {
                    writer.Write((ushort) ((ushort) num));
                    writer.Write((short) 0);
                    writer.Write((ushort) 0xffff);
                }
            }
        }

        internal bool WriteWINDOW1(BinaryWriter writer, BiffRecord biff)
        {
            IExcelRect rect = new ExcelRect(120.0, 15.0, 17102.0, 10110.0);
            bool hidden = false;
            bool iconic = false;
            bool horizontalScroll = true;
            bool verticalScroll = true;
            bool showTabs = true;
            int selectedTabIndex = 0;
            int firstDisplayedTabIndex = 0;
            int selectedTabCount = 0;
            int tabRatio = 600;
            int num5 = 0;
            IExcelRect window = this._excelWriter.GetWindow(ref hidden, ref iconic);
            if (window != null)
            {
                rect = window;
            }
            this._excelWriter.GetTabs(ref showTabs, ref selectedTabIndex, ref firstDisplayedTabIndex, ref selectedTabCount, ref tabRatio);
            this._excelWriter.GetScroll(ref horizontalScroll, ref verticalScroll);
            biff.Write(writer);
            BiffRecord unsupportRecord = this.GetUnsupportRecord(-1, BiffRecordNumber.WINDOW1);
            if (unsupportRecord != null)
            {
                byte[] buffer = new byte[8];
                Array.Copy(unsupportRecord.DataBuffer, buffer, 8);
                writer.Write(buffer);
            }
            else
            {
                writer.Write((short) ((short) rect.Left));
                writer.Write((short) ((short) rect.Top));
                writer.Write((short) ((short) rect.Width));
                writer.Write((short) ((short) rect.Height));
            }
            num5 = ((((hidden ? 1 : 0) | (iconic ? 2 : 0)) | (horizontalScroll ? 8 : 0)) | (verticalScroll ? 0x10 : 0)) | (showTabs ? 0x20 : 0);
            writer.Write((short) ((short) num5));
            writer.Write((short) ((short) selectedTabIndex));
            writer.Write((short) ((short) firstDisplayedTabIndex));
            writer.Write((short) ((short) selectedTabCount));
            writer.Write((short) ((short) tabRatio));
            return true;
        }

        internal void WriteWINDOW2(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            biff.Write(writer);
            bool showFormula = false;
            bool showZeros = true;
            bool showGridLine = true;
            bool showRowColumnHeader = true;
            bool rightToLeftColumns = false;
            this._excelWriter.GetDisplayElements(sheetIndex, ref showFormula, ref showZeros, ref showGridLine, ref showRowColumnHeader, ref rightToLeftColumns);
            IUnsupportRecord record = this.GetUnsupportRecord(sheetIndex, RecordCategory.Drawing, false);
            int num = 0;
            if (showFormula)
            {
                num |= 1;
            }
            if (showGridLine)
            {
                num |= 2;
            }
            if (showRowColumnHeader || (record != null))
            {
                num |= 4;
            }
            if (showZeros)
            {
                num |= 0x10;
            }
            if (record != null)
            {
                num |= 0x20;
            }
            if (rightToLeftColumns)
            {
                num |= 0x40;
            }
            if (record != null)
            {
                num |= 0x80;
            }
            int rowMaxOutLineLevel = -1;
            int columnMaxOutLineLevel = -1;
            this._excelWriter.GetGutters(sheetIndex, ref rowMaxOutLineLevel, ref columnMaxOutLineLevel);
            if ((rowMaxOutLineLevel > 0) || (columnMaxOutLineLevel > 0))
            {
                num |= 0x80;
            }
            int frozenColumnCount = 0;
            int frozenRowCount = 0;
            int frozenTrailingColumnCount = 0;
            int frozenTrailingRowCount = 0;
            bool flag6 = false;
            this._excelWriter.GetFrozen(sheetIndex, ref frozenRowCount, ref frozenColumnCount, ref frozenTrailingRowCount, ref frozenTrailingColumnCount);
            if ((frozenColumnCount > 0) || (frozenRowCount > 0))
            {
                num |= 8;
                flag6 = true;
            }
            bool showTabs = true;
            int selectedTabIndex = 0;
            int firstDisplayedTabIndex = 0;
            int selectedTabCount = 0;
            int tabRatio = 600;
            this._excelWriter.GetTabs(ref showTabs, ref selectedTabIndex, ref firstDisplayedTabIndex, ref selectedTabCount, ref tabRatio);
            if (sheetIndex == selectedTabIndex)
            {
                num |= 0x200;
            }
            if (record != null)
            {
                num |= 0x400;
            }
            writer.Write((short) ((short) num));
            int topRow = 0;
            int leftColumn = 0;
            if (!flag6)
            {
                this._excelWriter.GetTopLeft(sheetIndex, ref topRow, ref leftColumn);
                if (topRow >= 0x10000)
                {
                    topRow = 0xffff;
                }
                if (topRow < 0)
                {
                    topRow = 0;
                }
                if (leftColumn >= 0x100)
                {
                    leftColumn = 0xff;
                }
                if (leftColumn < 0)
                {
                    leftColumn = 0;
                }
            }
            writer.Write((short) ((short) topRow));
            writer.Write((short) ((short) leftColumn));
            IExcelColor gridlineColor = this._excelWriter.GetGridlineColor(sheetIndex);
            if (gridlineColor != null)
            {
                if (gridlineColor.IsIndexedColor)
                {
                    writer.Write((short) ((short) gridlineColor.Value));
                }
                else
                {
                    writer.Write((short) ((short) this._excelWriter.GetPaletteColor(gridlineColor)));
                }
            }
            else
            {
                writer.Write((short) 0x40);
            }
            writer.Write((short) 0);
            writer.Write((short) 0);
            writer.Write((short) 0);
            if (record == null)
            {
                writer.Write(0);
            }
            else
            {
                writer.Write(0);
            }
        }

        /// <summary>
        /// WriteWorkbookRecords
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="streams"></param>
        /// <param name="streamLen"></param>
        /// <returns></returns>
        internal bool WriteWorkbookRecords(BinaryWriter writer, MemoryStream[] streams, int streamLen)
        {
            short num2;
            IUnsupportRecord record2;
            BiffRecord biff = new BiffRecord();
            int sheetCount = this._excelWriter.GetSheetCount();
            List<bool> hiddenList = new List<bool>();
            MemoryStream nameStream = null;
            int num4 = (streamLen + (sheetCount * ((BiffRecord.Length + 4) + 2))) + BiffRecord.Length;
            MemoryStream[] sheetNameStreamList = new MemoryStream[sheetCount];
            StringBuilder[] sheetNameBuilders = new StringBuilder[sheetCount];
            ExcelSheetType[] typeArray = new ExcelSheetType[sheetCount];
            IExcelLosslessWriter writer2 = null;
            if (this._excelWriter is IExcelLosslessWriter)
            {
                writer2 = this._excelWriter as IExcelLosslessWriter;
            }
            this.BuildSheetNames(sheetNameStreamList, sheetNameBuilders, hiddenList, sheetCount);
            for (num2 = 0; num2 < sheetCount; num2++)
            {
                sheetNameStreamList[num2] = new MemoryStream();
                BinaryWriter writer3 = new BinaryWriter((Stream) sheetNameStreamList[num2]);
                this.WriteBiffStr(writer3, sheetNameBuilders[num2].ToString(), false, false, false, false, 1);
                num4 += (int) sheetNameStreamList[num2].Length;
                if (writer2 != null)
                {
                    typeArray[num2] = writer2.GetSheetType(num2);
                }
            }
            this.InitLinkTable();
            for (short i = 0; i < this._sheetCount; i++)
            {
                streams[i] = new MemoryStream();
                this.ProcessSheet(i, streams[i]);
            }
            this.ProcessNames(ref nameStream);
            if ((nameStream != null) && (nameStream.Length > 0L))
            {
                num4 += (int) nameStream.Length;
            }
            MemoryStream @this = new MemoryStream();
            BinaryWriter writer4 = new BinaryWriter((Stream) @this);
            this.WriteUnsupportRecord(writer4, -1, BiffRecordNumber.HFPICTURE);
        Label_013E:
            record2 = this.GetUnsupportRecord(-1, RecordCategory.DrawingGroup, true);
            if (record2 != null)
            {
                List<BiffRecord> list2 = record2.Value as List<BiffRecord>;
                if (list2 == null)
                {
                    goto Label_013E;
                }
                using (List<BiffRecord>.Enumerator enumerator = list2.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Write(writer4);
                    }
                    goto Label_013E;
                }
            }
            if ((@this != null) && (@this.Length > 0L))
            {
                num4 += (int) @this.Length;
            }
            MemoryStream sstStream = new MemoryStream();
            this.ProcessSST(sstStream);
            if ((sstStream != null) && (sstStream.Length > 0L))
            {
                num4 += (int) sstStream.Length;
            }
            MemoryStream stream4 = new MemoryStream();
            BinaryWriter writer5 = new BinaryWriter((Stream) stream4);
            this.WriteUnsupportRecord(writer5, -1, BiffRecordNumber.BOOKEXT);
            this.WriteUnsupportRecord(writer5, -1, BiffRecordNumber.THEME);
            this.WriteUnsupportRecord(writer5, -1, BiffRecordNumber.COMPRESSPICTURE);
            this.WriteUnsupportRecord(writer5, -1, BiffRecordNumber.COMPAT12);
            this.WriteUnsupportRecord(writer5, -1, BiffRecordNumber.GUIDTypeLib);
            if ((stream4 != null) && (stream4.Length > 0L))
            {
                num4 += (int) stream4.Length;
            }
            bool flag = false;
            for (num2 = 0; num2 < sheetCount; num2++)
            {
                if (!hiddenList[num2])
                {
                    flag = true;
                    break;
                }
            }
            if ((sheetCount > 0) && !flag)
            {
                hiddenList[0] = false;
            }
            int num6 = this._sheetCount + this._mergedToSheetIndexList.Count;
            for (num2 = 0; num2 < num6; num2++)
            {
                int offset = num4;
                for (short j = 0; j <= (num2 - 1); j++)
                {
                    offset += (int) streams[j].Length;
                }
                if (this._mergedToSheetIndexList.IndexOf(num2) == -1)
                {
                    biff.RecordType = BiffRecordNumber.BUNDLESHEET;
                    biff.DataLength = 6;
                    if (writer2 != null)
                    {
                        this.WriteBoundSheet(writer, biff, offset, sheetNameStreamList[num2], hiddenList[num2], typeArray[num2]);
                    }
                    else
                    {
                        this.WriteBoundSheet(writer, biff, offset, sheetNameStreamList[num2], hiddenList[num2], ExcelSheetType.Worksheet);
                    }
                }
            }
            if ((nameStream != null) && (nameStream.Length > 0L))
            {
                writer.Write(nameStream.GetBuffer(), 0, (int) nameStream.Length);
                nameStream.Close();
            }
            if ((@this != null) && (@this.Length > 0L))
            {
                writer.Write(@this.ToArray(), 0, (int) @this.Length);
                @this.Close();
            }
            if ((sstStream != null) && (sstStream.Length > 0L))
            {
                writer.Write(sstStream.ToArray(), 0, (int) sstStream.Length);
                sstStream.Close();
            }
            if ((stream4 != null) && (stream4.Length > 0L))
            {
                writer.Write(stream4.ToArray(), 0, (int) stream4.Length);
                stream4.Close();
            }
            this.WriteEndWorkbookRecords(writer);
            for (num2 = 0; num2 < streams.Length; num2++)
            {
                byte[] buffer = new byte[streams[num2].Length];
                streams[num2].Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                streams[num2].Read(buffer, 0, (int) streams[num2].Length);
                writer.Write(buffer);
                streams[num2].Close();
            }
            return true;
        }

        private void WriteWorksheetPrintMargin(short sheet, BinaryWriter writer)
        {
            BiffRecord record = new BiffRecord();
            IExcelPrintPageMargin printPageMargin = this._excelWriter.GetPrintPageMargin(sheet);
            record.RecordType = BiffRecordNumber.LEFTMARGIN;
            record.DataLength = 8;
            record.Write(writer);
            writer.Write(printPageMargin.Left);
            record.RecordType = BiffRecordNumber.RIGHTMARGIN;
            record.DataLength = 8;
            record.Write(writer);
            writer.Write(printPageMargin.Right);
            record.RecordType = BiffRecordNumber.TOPMARGIN;
            record.DataLength = 8;
            record.Write(writer);
            writer.Write(printPageMargin.Top);
            record.RecordType = BiffRecordNumber.BOTTOMMARGIN;
            record.DataLength = 8;
            record.Write(writer);
            writer.Write(printPageMargin.Bottom);
        }

        internal void WriteWSBool(BinaryWriter writer, BiffRecord biff, short sheetIndex)
        {
            IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheetIndex);
            int num = 0;
            if (printPageSetting.UseSmartPrint)
            {
                num = 0x5c1;
            }
            else
            {
                num = 0x4c1;
            }
            bool summaryRowsBelowDetail = true;
            bool summaryColumnsRightToDetail = true;
            this._excelWriter.GetOutlineDirection(sheetIndex, ref summaryColumnsRightToDetail, ref summaryRowsBelowDetail);
            if (!summaryColumnsRightToDetail)
            {
                num &= 0xff7f;
            }
            if (!summaryRowsBelowDetail)
            {
                num &= 0xffbf;
            }
            biff.Write(writer);
            writer.Write((short) ((short) num));
        }

        private void WriteXfExtNoFRTRecord(BinaryWriter writer, IDifferentialFormatting format)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter((Stream) stream);
            int num = 0;
            if ((format.Font != null) && this.NeedWriteColorExt(format.Font.FontColor))
            {
                num++;
                bw.Write((short) 13);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Font.FontColor);
            }
            if ((format.Fill != null) && this.NeedWriteColorExt(format.Fill.Item2))
            {
                num++;
                bw.Write((short) 4);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Fill.Item2);
            }
            if ((format.Fill != null) && this.NeedWriteColorExt(format.Fill.Item3))
            {
                num++;
                bw.Write((short) 5);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Fill.Item3);
            }
            bw.Flush();
            bw.Close();
            byte[] buffer = stream.ToArray();
            writer.Write((short) 0);
            writer.Write((ushort) 0xffff);
            writer.Write((short) 0);
            writer.Write((short) ((short) num));
            writer.Write(buffer);
        }

        private void WriteXFExtRecrod(BinaryWriter writer, IExtendedFormat format, int xfIndex)
        {
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter((Stream) stream);
            int num = 0;
            if (this.NeedWriteColorExt(format.Font.FontColor))
            {
                num++;
                bw.Write((short) 13);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Font.FontColor);
            }
            num++;
            bw.Write((short) 14);
            bw.Write((ushort) 5);
            if (format.Font.FontScheme == FontSchemeCategory.Major)
            {
                bw.Write((byte) 1);
            }
            else if (format.Font.FontScheme == FontSchemeCategory.Minor)
            {
                bw.Write((byte) 2);
            }
            else
            {
                bw.Write((byte) 0);
            }
            if (this.NeedWriteColorExt(format.PatternColor))
            {
                num++;
                bw.Write((short) 4);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.PatternColor);
            }
            if (this.NeedWriteColorExt(format.PatternBackgroundColor))
            {
                num++;
                bw.Write((short) 5);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.PatternBackgroundColor);
            }
            if (this.NeedWriteBorder(format.Border))
            {
                num += 4;
                bw.Write((short) 7);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Border.Top.Color);
                bw.Write((short) 8);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Border.Bottom.Color);
                bw.Write((short) 9);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Border.Left.Color);
                bw.Write((short) 10);
                bw.Write((ushort) 20);
                this.WriteFullColorExt(bw, format.Border.Right.Color);
            }
            bw.Flush();
            bw.Dispose();
            byte[] buffer = stream.ToArray();
            if (buffer.Length > 0)
            {
                writer.Write((short) 0x87d);
                writer.Write((short) (20 + buffer.Length));
                writer.Write((short) 0x87d);
                writer.Write((short) 0);
                writer.Write((double) 0.0);
                writer.Write((short) 0);
                writer.Write((ushort) ((ushort) xfIndex));
                writer.Write((short) 0);
                writer.Write((short) ((short) num));
                writer.Write(buffer);
            }
        }

        private void WriteXFPropBorder(BinaryWriter writer, IExcelBorderSide borderSide)
        {
            this.WriteXfPropColor(writer, borderSide.Color);
            writer.Write((ushort) borderSide.LineStyle);
        }

        private void WriteXfPropColor(BinaryWriter writer, IExcelColor color)
        {
            if (color == null)
            {
                color = new ExcelColor(ExcelColorType.Indexed, 0x40, 0.0);
            }
            int num = 1;
            if (color == null)
            {
                num |= 8;
            }
            if (color.ColorType == ExcelColorType.Indexed)
            {
                num |= 2;
            }
            else if (color.ColorType == ExcelColorType.RGB)
            {
                num |= 4;
            }
            else if (color.ColorType == ExcelColorType.Theme)
            {
                num |= 6;
            }
            writer.Write((byte) ((byte) num));
            if ((color == null) || color.IsAutoColor)
            {
                writer.Write(0);
                writer.Write((short) 0);
                writer.Write((byte) 0);
            }
            else if (color.ColorType == ExcelColorType.Indexed)
            {
                writer.Write((byte) ((byte) color.Value));
                writer.Write((short) (color.Tint * 32767.0));
                GcColor color2 = ColorExtension.FromArgb(color.Value);
                writer.Write(color2.R);
                writer.Write(color2.G);
                writer.Write(color2.B);
                writer.Write(color2.A);
            }
            else if (color.ColorType == ExcelColorType.RGB)
            {
                writer.Write((byte) 0xff);
                writer.Write((short) (color.Tint * 32767.0));
                GcColor color3 = ColorExtension.FromArgb(color.Value);
                writer.Write(color3.R);
                writer.Write(color3.G);
                writer.Write(color3.B);
                writer.Write(color3.A);
            }
            else if (color.ColorType == ExcelColorType.Theme)
            {
                writer.Write((byte) ((byte) color.Value));
                writer.Write((short) (color.Tint * 32767.0));
                writer.Write((byte) 0);
                writer.Write((byte) 0);
                writer.Write((byte) 0);
                writer.Write((byte) 0xff);
            }
        }

        private int WriteXfProps(BinaryWriter writer, IDifferentialFormatting differentFormatting)
        {
            int num = 0;
            if ((differentFormatting.Fill != null) && this.NeedWriteColorExt(differentFormatting.Fill.Item2))
            {
                num++;
                writer.Write((ushort) 1);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, differentFormatting.Fill.Item2);
            }
            if ((differentFormatting.Fill != null) && this.NeedWriteColorExt(differentFormatting.Fill.Item3))
            {
                num++;
                writer.Write((ushort) 2);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, differentFormatting.Fill.Item3);
            }
            if ((differentFormatting.Font != null) && this.NeedWriteColorExt(differentFormatting.Font.FontColor))
            {
                num++;
                writer.Write((ushort) 5);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, differentFormatting.Font.FontColor);
            }
            if (num > 0)
            {
                num++;
                writer.Write((ushort) 0x25);
                writer.Write((ushort) 5);
                if ((differentFormatting.Font != null) && (differentFormatting.Font.FontScheme == FontSchemeCategory.Major))
                {
                    writer.Write((byte) 1);
                }
                else if ((differentFormatting.Font != null) && (differentFormatting.Font.FontScheme == FontSchemeCategory.Minor))
                {
                    writer.Write((byte) 2);
                }
                else
                {
                    writer.Write((byte) 0);
                }
            }
            if ((differentFormatting.Border != null) && this.NeedWriteBorder(differentFormatting.Border))
            {
                num += 4;
                writer.Write((ushort) 6);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, differentFormatting.Border.Top);
                writer.Write((ushort) 7);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, differentFormatting.Border.Bottom);
                writer.Write((ushort) 8);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, differentFormatting.Border.Left);
                writer.Write((ushort) 9);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, differentFormatting.Border.Right);
            }
            return num;
        }

        private int WriteXfProps(BinaryWriter writer, IExcelStyle style)
        {
            int num = 0;
            if (this.NeedWriteColorExt(style.Format.PatternColor))
            {
                num++;
                writer.Write((ushort) 1);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, style.Format.PatternColor);
            }
            if (this.NeedWriteColorExt(style.Format.PatternBackgroundColor))
            {
                num++;
                writer.Write((ushort) 2);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, style.Format.PatternBackgroundColor);
            }
            if (this.NeedWriteColorExt(style.Format.Font.FontColor))
            {
                num++;
                writer.Write((ushort) 5);
                writer.Write((ushort) 12);
                this.WriteXfPropColor(writer, style.Format.Font.FontColor);
            }
            if (num > 0)
            {
                num++;
                writer.Write((ushort) 0x25);
                writer.Write((ushort) 5);
                if (style.Format.Font.FontScheme == FontSchemeCategory.Major)
                {
                    writer.Write((byte) 1);
                }
                else if (style.Format.Font.FontScheme == FontSchemeCategory.Minor)
                {
                    writer.Write((byte) 2);
                }
                else
                {
                    writer.Write((byte) 0);
                }
            }
            if (this.NeedWriteBorder(style.Format.Border))
            {
                num += 4;
                writer.Write((ushort) 6);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, style.Format.Border.Top);
                writer.Write((ushort) 7);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, style.Format.Border.Bottom);
                writer.Write((ushort) 8);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, style.Format.Border.Left);
                writer.Write((ushort) 9);
                writer.Write((ushort) 14);
                this.WriteXFPropBorder(writer, style.Format.Border.Right);
            }
            return num;
        }

        private void WriteXFRecord(BinaryWriter writer, List<XFRecrod> formatRecords, List<IExtendedFormat> formats)
        {
            new List<BiffRecord>();
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < formatRecords.Count; i++)
            {
                formatRecords[i].Write(writer);
                byte[] buffer = new byte[20];
                formatRecords[i].WriteToBuffer(buffer);
                list.Add(buffer);
            }
            MsoCrc32 crc = new MsoCrc32();
            uint crcValue = 0;
            foreach (byte[] buffer2 in list)
            {
                crcValue = crc.CRC(crcValue, buffer2);
            }
            writer.Write((short) 0x87c);
            writer.Write((short) 20);
            writer.Write((short) 0x87c);
            writer.Write((short) 0);
            writer.Write((double) 0.0);
            writer.Write((short) 0);
            writer.Write((ushort) ((ushort) formats.Count));
            writer.Write((int) crcValue);
            for (int j = 0; j < formats.Count; j++)
            {
                this.WriteXFExtRecrod(writer, formats[j], j);
            }
        }

        private List<IDifferentialFormatting> Dxfs
        {
            get
            {
                if (this._dxfs == null)
                {
                    this._dxfs = this._excelWriter.GetDifferentialFormattingRecords();
                }
                return this._dxfs;
            }
        }

        public bool IsEncrypted { get; set; }

        public string Password { get; set; }

        private List<string> SheetNames
        {
            get
            {
                if (this._sheetNames == null)
                {
                    this._sheetNames = new List<string>();
                    int sheetCount = this._excelWriter.GetSheetCount();
                    for (int i = 0; i < sheetCount; i++)
                    {
                        this._sheetNames.Add(this._excelWriter.GetSheetName(i));
                    }
                }
                return this._sheetNames;
            }
        }

        internal enum StringConvert
        {
            None,
            Unicode,
            Ascii
        }
    }
}

