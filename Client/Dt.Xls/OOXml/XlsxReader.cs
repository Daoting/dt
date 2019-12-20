#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using Dt.Xls.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.OOXml
{
    internal class XlsxReader
    {
        private CalculationProperty _calcProperty;
        private static Regex _cellRefRegex = new Regex(@"(\$?)([A-Z]+)(\$?)(\d+)");
        private static Regex _cellRefRegex2 = new Regex(@"(\$?)(\d+):(\$?)(\d+)");
        private static Regex _cellRefRegex3 = new Regex(@"(\$?)([A-Z]+):(\$?)([A-Z]+)");
        private int? _columnCounter;
        private double _defaultRowHeight = 15.0;
        private static List<NamedCellRange> _definedNames = new List<NamedCellRange>();
        private IDocumentProperties _documentProperties;
        private List<IDifferentialFormatting> _dxfRecords = new List<IDifferentialFormatting>();
        private Dictionary<string, Tuple<IExcelConditionalFormat, IExcel2010DataBarRule>> _excel2010ExtensionDataBarRules = new Dictionary<string, Tuple<IExcelConditionalFormat, IExcel2010DataBarRule>>();
        private ExcelPrintPageSetting _excelPrintPageSetting;
        private IExcelReader _excelReader;
        private Dictionary<string, List<IName>> _externalCellRanges = new Dictionary<string, List<IName>>();
        private string _externalFileName;
        private Dictionary<int, Tuple<string, string>> _externalRefs = new Dictionary<int, Tuple<string, string>>();
        private List<string> _externalWorkbookSheetsNames = new List<string>();
        private Dictionary<string, string> _hyperLinksRelations = new Dictionary<string, string>();
        private LinkTable _linkTable = new LinkTable();
        private static Dictionary<int, IExcelNumberFormat> _numberFormats = new Dictionary<int, IExcelNumberFormat>();
        private List<PivotCacheInfo> _pivotCacheInfos = new List<PivotCacheInfo>();
        private static Regex _refRegexInFormula = new Regex(@"\[(\d+)\]");
        private int? _rowCounter;
        private Dictionary<int, Dt.Xls.OOXml.SharedFormula> _sharedFormula = new Dictionary<int, Dt.Xls.OOXml.SharedFormula>();
        private Dictionary<int, Tuple<int, int>> _sheetActiveCells = new Dictionary<int, Tuple<int, int>>();
        private List<SheetInfo> _sheetIDs = new List<SheetInfo>();
        private static Regex _specicalPattern = new Regex("_x00[1-9][0-9A-Fa-f]_");
        private static List<ExcelBorder> _styleBorders = new List<ExcelBorder>();
        private List<ExtendedFormat> _styleCellStyleXfs = new List<ExtendedFormat>();
        private List<ExtendedFormat> _styleCellXfs = new List<ExtendedFormat>();
        private static List<Tuple<FillPatternType, ExcelColor, ExcelColor>> _styleFills = new List<Tuple<FillPatternType, ExcelColor, ExcelColor>>();
        private static List<ExcelFont> _styleFonts = new List<ExcelFont>();
        private List<IExcelTableStyle> _tableStyles = new List<IExcelTableStyle>();
        private Dictionary<int, ExcelColor> _themedbgFillStyles = new Dictionary<int, ExcelColor>();
        private Dictionary<int, ExcelColor> _themedFillStyles = new Dictionary<int, ExcelColor>();
        private Dictionary<int, ExcelBorderSide> _themeLineStyles = new Dictionary<int, ExcelBorderSide>();
        private XFile _workbookFile;
        private Dictionary<string, XlsxReadHandler> _xlsxHandlerMap = new Dictionary<string, XlsxReadHandler>();
        private int identifier;
        private List<string> SST = new List<string>();

        public XlsxReader(IExcelReader reader, IDocumentProperties docProp = null)
        {
            this._excelReader = reader;
            this._documentProperties = docProp;
            this._rowCounter = null;
            this._columnCounter = null;
            this.CreateMap();
            ParsingContext.LinkTable = this._linkTable;
            ParsingContext.ParsingErrors.Clear();
        }

        private void ClearCache()
        {
            this._workbookFile = null;
            this._sheetIDs.Clear();
            this._pivotCacheInfos.Clear();
            _styleFills.Clear();
            _numberFormats.Clear();
            _styleFills.Clear();
            _styleFonts.Clear();
            _styleBorders.Clear();
            this._sheetActiveCells.Clear();
            this._styleCellXfs.Clear();
            this._externalRefs.Clear();
            this._styleCellStyleXfs.Clear();
            this._dxfRecords.Clear();
            this._externalWorkbookSheetsNames.Clear();
            this._externalCellRanges.Clear();
            this._sharedFormula.Clear();
            _numberFormats.Clear();
            this._hyperLinksRelations.Clear();
            _definedNames.Clear();
            this.SST.Clear();
            this._themedFillStyles.Clear();
            this._themedbgFillStyles.Clear();
            this._themeLineStyles.Clear();
            this._tableStyles.Clear();
            this._excel2010ExtensionDataBarRules.Clear();
            ParsingContext.ParsingErrors.Clear();
        }

        private string ConvertA1FormulaToR1C1Formula(string formula, int row, int column)
        {
            try
            {
                return Parser.Unparse(Parser.Parse(formula, row, column, false, this._linkTable), row, column, true);
            }
            catch (Exception exception)
            {
                this.LogError(string.Format(ResourceHelper.GetResourceString("convertA1ToR1C1FormulaError"), (object[]) new object[] { formula }), ExcelWarningCode.FormulaError, exception);
            }
            return formula;
        }

        private void CreateMap()
        {
            this._xlsxHandlerMap.Add("workbook", new XlsxReadHandler(this.ReadContent));
            this._xlsxHandlerMap.Add("bookViews", new XlsxReadHandler(this.ReadBookViews));
            this._xlsxHandlerMap.Add("sheets", new XlsxReadHandler(this.ReadSheets));
            this._xlsxHandlerMap.Add("pivotCaches", new XlsxReadHandler(this.ReadPivotChaches));
            this._xlsxHandlerMap.Add("definedNames", new XlsxReadHandler(this.ReadDefinedNames));
            this._xlsxHandlerMap.Add("calcPr", new XlsxReadHandler(this.ReadCalcProperties));
            this._xlsxHandlerMap.Add("workbookPr", new XlsxReadHandler(this.ReadWorkbookProperties));
            this._xlsxHandlerMap.Add("externalReferences", new XlsxReadHandler(this.ReadExternalReferences));
            this._xlsxHandlerMap.Add("externalBook", new XlsxReadHandler(this.ReadExternalWorkbookInfo));
            this._xlsxHandlerMap.Add("worksheet", new XlsxReadHandler(this.ReadContent));
            this._xlsxHandlerMap.Add("sheetPr", new XlsxReadHandler(this.ReadSheetPr));
            this._xlsxHandlerMap.Add("dimension", new XlsxReadHandler(this.ReadDimension));
            this._xlsxHandlerMap.Add("sheetViews", new XlsxReadHandler(this.ReadSheetViews));
            this._xlsxHandlerMap.Add("sheetFormatPr", new XlsxReadHandler(this.ReadSheetFormatProperties));
            this._xlsxHandlerMap.Add("cols", new XlsxReadHandler(this.ReadContent));
            this._xlsxHandlerMap.Add("col", new XlsxReadHandler(this.ReadColumnInfo));
            this._xlsxHandlerMap.Add("sheetData", new XlsxReadHandler(this.ReadContent));
            this._xlsxHandlerMap.Add("row", new XlsxReadHandler(this.ReadRow));
            this._xlsxHandlerMap.Add("dataValidations", new XlsxReadHandler(this.ReadDataValidations));
            this._xlsxHandlerMap.Add("pageMargins", new XlsxReadHandler(this.ReadPageMargins));
            this._xlsxHandlerMap.Add("printOptions", new XlsxReadHandler(this.ReadPrintOptions));
            this._xlsxHandlerMap.Add("pageSetup", new XlsxReadHandler(this.ReadPageSetup));
            this._xlsxHandlerMap.Add("pageSetUpPr", new XlsxReadHandler(this.ReadPageSetupAdditionalProperty));
            this._xlsxHandlerMap.Add("headerFooter", new XlsxReadHandler(this.ReadHeaderFooter));
            this._xlsxHandlerMap.Add("hyperlinks", new XlsxReadHandler(this.ReadHyperLink));
            this._xlsxHandlerMap.Add("colBreaks", new XlsxReadHandler(this.ReadColumnBreaks));
            this._xlsxHandlerMap.Add("rowBreaks", new XlsxReadHandler(this.ReadRowBreaks));
            this._xlsxHandlerMap.Add("mergeCells", new XlsxReadHandler(this.ReadMergeCells));
            this._xlsxHandlerMap.Add("conditionalFormatting", new XlsxReadHandler(this.ReadConditionalFormating));
            this._xlsxHandlerMap.Add("autoFilter", new XlsxReadHandler(this.ReadAutoFilter));
            this._xlsxHandlerMap.Add("sheetProtection", new XlsxReadHandler(this.ReadSheetProtection));
            this._xlsxHandlerMap.Add("extLst", new XlsxReadHandler(this.ReadExtensionList));
            this._xlsxHandlerMap.Add("sst", new XlsxReadHandler(this.ReadContent));
            this._xlsxHandlerMap.Add("si", new XlsxReadHandler(this.ReadSharedStringItem));
            this._xlsxHandlerMap.Add("fonts", new XlsxReadHandler(this.ReadFonts));
            this._xlsxHandlerMap.Add("borders", new XlsxReadHandler(this.ReadBorders));
            this._xlsxHandlerMap.Add("fills", new XlsxReadHandler(this.ReadFills));
            this._xlsxHandlerMap.Add("cellXfs", new XlsxReadHandler(this.ReadCellXFs));
            this._xlsxHandlerMap.Add("cellStyleXfs", new XlsxReadHandler(this.ReadCellStyleXfs));
            this._xlsxHandlerMap.Add("cellStyles", new XlsxReadHandler(this.ReadCellStyles));
            this._xlsxHandlerMap.Add("numFmts", new XlsxReadHandler(this.ReadNumberFormats));
            this._xlsxHandlerMap.Add("dxfs", new XlsxReadHandler(this.ReadDifferentialFormattingRecords));
            this._xlsxHandlerMap.Add("colors", new XlsxReadHandler(this.ReadColors));
            this._xlsxHandlerMap.Add("commentList", new XlsxReadHandler(this.ReadComment));
            this._xlsxHandlerMap.Add("table", new XlsxReadHandler(this.ReadSheetTable));
            this._xlsxHandlerMap.Add("tableStyles", new XlsxReadHandler(this.ReadTableStyles));
            this._xlsxHandlerMap.Add("AlternateContent", new XlsxReadHandler(this.ReadAlternateContent));
        }

        private double EMU2Pixles(int emu)
        {
            return (((double) (emu * 0x60)) / 914400.0);
        }

        private static string EncodeEvaluator(Match match)
        {
            int num = 0;
            int.TryParse(match.Groups[2].Value, (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
            if (match.Groups[1].Value == "_x005F")
            {
                return string.Format("_x00{0}_", (object[]) new object[] { match.Groups[2].Value });
            }
            char ch = (char) num;
            return ((char) ch).ToString();
        }

        internal static ExcelBorderSide GetBorder(XElement child, string border)
        {
            XElement element = child.TryGetChildElement(border);
            ExcelBorderSide side = new ExcelBorderSide();
            if (element != null)
            {
                ExcelBorderStyle style;
                Enum.TryParse<ExcelBorderStyle>(element.GetAttributeValueOrDefaultOfStringType("style", null), true, out style);
                side.LineStyle = style;
                XElement node = element.TryGetChildElement("color");
                if (node != null)
                {
                    ExcelColor color = TryReadColor(node);
                    side.Color = color;
                }
            }
            return side;
        }

        internal static ExcelBorderSide GetBorderIfExist(XElement child, string border)
        {
            if (child.TryGetChildElement(border) != null)
            {
                return GetBorder(child, border);
            }
            return null;
        }

        private ExcelConditionalFormatValueObjectType GetConditionalFormatValueObjectType(XElement element)
        {
            ExcelConditionalFormatValueObjectType type;
            Enum.TryParse<ExcelConditionalFormatValueObjectType>(element.GetAttributeValueOrDefaultOfStringType("type", null), true, out type);
            return type;
        }

        private ExternalCellRange GetExternalCellRange(string sqref)
        {
            ExternalRangeExpression expression = Parser.Parse(sqref, 0, 0, false, this._linkTable).Value as ExternalRangeExpression;
            if ((expression == null) || (expression.Source == null))
            {
                return null;
            }
            ExternalCellRange range = new ExternalCellRange();
            string str = expression.Source.ToString();
            int index = str.IndexOf('[');
            int num2 = str.IndexOf(']');
            if (((this._externalRefs.Count != 0) && (num2 != -1)) && ((num2 != (str.Length - 1)) && (index < num2)))
            {
                string s = str.Substring(index + 1, (num2 - index) - 1);
                int result = 0;
                if (int.TryParse(s, out result) && this._externalRefs.ContainsKey(result))
                {
                    range.WorkbookName = this._externalRefs[result].Item2;
                    range.WorksheetName = str.Substring(num2 + 1);
                }
                else
                {
                    range.WorksheetName = str;
                }
            }
            else
            {
                range.WorksheetName = str;
            }
            range.Row = expression.Row;
            range.Column = expression.Column;
            range.RowSpan = expression.RowCount;
            range.ColumnSpan = expression.ColumnCount;
            return range;
        }

        private IRange GetRange(string sqref)
        {
            ExcelCellRange range = new ExcelCellRange();
            string[] strArray = sqref.Split(new char[] { ':' });
            if (strArray.Length == 1)
            {
                int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray[0]);
                int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                range.Row = rowIndexInNumber;
                range.Column = columnIndexInNumber;
                range.RowSpan = 1;
                range.ColumnSpan = 1;
                return range;
            }
            if (strArray.Length == 2)
            {
                int num3 = IndexHelper.GetRowIndexInNumber(strArray[0]);
                int num4 = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                int num5 = IndexHelper.GetRowIndexInNumber(strArray[1]);
                int num6 = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                range.Row = num3;
                range.Column = num4;
                range.RowSpan = (num5 - num3) + 1;
                range.ColumnSpan = (num6 - num4) + 1;
            }
            return range;
        }

        private List<IRange> GetRanges(string sqref)
        {
            List<IRange> list = new List<IRange>();
            if (sqref != null)
            {
                foreach (string str in sqref.Split(new char[] { ' ' }))
                {
                    list.Add(this.GetRange(str));
                }
            }
            return list;
        }

        private string GetSharedFormula(Dt.Xls.OOXml.SharedFormula sharedFormula, int columnOffset, int rowOffset)
        {
            if (sharedFormula.ParsedSharedFormulaStructs == null)
            {
                ParseSharedFormulaStruct(sharedFormula, columnOffset, rowOffset, 0);
            }
            if (sharedFormula.ParsedSharedFormulaStructs == null)
            {
                return string.Empty;
            }
            string str = "";
            string baseFormula = sharedFormula.BaseFormula;
            if ((sharedFormula.ParsedSharedFormulaStructs == null) || (sharedFormula.ParsedSharedFormulaStructs.Count == 0))
            {
                return baseFormula;
            }
            foreach (ParsedSharedFormulaStruct struct2 in sharedFormula.ParsedSharedFormulaStructs)
            {
                if (struct2.SharedFormulaType == 0)
                {
                    int coord = struct2.Column1 + columnOffset;
                    if (coord >= 0x4000)
                    {
                        coord = struct2.Column1;
                    }
                    int num2 = struct2.Row1 + rowOffset;
                    if (num2 >= 0x100000)
                    {
                        num2 = struct2.Row1;
                    }
                    str = str + string.Format(struct2.formatString, (object[]) new object[] { IndexHelper.GetColumnIndexInA1Letter(coord), ((int) num2).ToString() });
                }
                else if (struct2.SharedFormulaType == 1)
                {
                    int num3 = struct2.Column1 + columnOffset;
                    if (num3 >= 0x4000)
                    {
                        num3 = struct2.Column1;
                    }
                    int num4 = struct2.Column2 + columnOffset;
                    if (num4 >= 0x4000)
                    {
                        num4 = struct2.Column2;
                    }
                    str = str + string.Format(struct2.formatString, (object[]) new object[] { IndexHelper.GetColumnIndexInA1Letter(num3), IndexHelper.GetColumnIndexInA1Letter(num4) });
                }
                else if (struct2.SharedFormulaType == 2)
                {
                    int num5 = struct2.Row1 + rowOffset;
                    if (num5 >= 0x100000)
                    {
                        num5 = struct2.Row1;
                    }
                    int num6 = struct2.Row2 + rowOffset;
                    if (num6 >= 0x100000)
                    {
                        num6 = struct2.Row2;
                    }
                    str = str + string.Format(struct2.formatString, (object[]) new object[] { ((int) num5).ToString(), ((int) num6).ToString() });
                }
            }
            return str;
        }

        private bool IsConditionalFormatElement(XElement node)
        {
            if (node.GetNamespaceOfPrefix("x14") == null)
            {
                return false;
            }
            string namespaceName = node.GetNamespaceOfPrefix("x14").NamespaceName;
            string str2 = node.GetAttributeValueOrDefaultOfStringType("uri", null);
            return ((namespaceName == "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main") && (str2 == "{78C0D931-6437-407d-A8EE-F0AAD7539E65}"));
        }

        private bool IsSparklineElement(XElement node)
        {
            if (node.GetNamespaceOfPrefix("x14") == null)
            {
                return false;
            }
            string namespaceName = node.GetNamespaceOfPrefix("x14").NamespaceName;
            string str2 = node.GetAttributeValueOrDefaultOfStringType("uri", null);
            return ((namespaceName == "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main") && (str2 == "{05C60535-1F16-4fd2-B633-F4F36F0B64E0}"));
        }

        internal void Load(Stream inStream, int excelSheetIndex)
        {
            this.ClearCache();
            if (excelSheetIndex < -1)
            {
                this.LogError(ResourceHelper.GetResourceString("sheetIndexOutOfRange"), ExcelWarningCode.CannotOpen, null);
            }
            else if (inStream != null)
            {
                using (MemoryFolder folder = ZipHelper.ExtractZip(inStream))
                {
                    if (folder != null)
                    {
                        XFile file = new XFile(string.Empty, string.Empty);
                        file.LoadPackageRelationFiles(folder);
                        if (this._documentProperties != null)
                        {
                            XFile fileByType = file.GetFileByType("http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties");
                            if (fileByType != null)
                            {
                                Stream stream = folder.GetFile(fileByType.FileName);
                                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                                XElement element = XDocument.Load(stream).Root;
                                if (element != null)
                                {
                                    XElement element2 = element.Element(XNames.creator);
                                    if (element2 != null)
                                    {
                                        if (this._documentProperties.SummaryInformation == null)
                                        {
                                            this._documentProperties.SummaryInformation = SummaryInformation.Read(null);
                                        }
                                        this._documentProperties.SummaryInformation.SetProperty<string>(0, element2.Value);
                                    }
                                }
                            }
                        }
                        this._workbookFile = file.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
                        if (this._workbookFile == null)
                        {
                            return;
                        }
                        this.LoadWorkbook(this._workbookFile, excelSheetIndex, folder);
                    }
                }
                this.ClearCache();
                this._excelReader.Finish();
                ParsingContext.LinkTable = null;
            }
        }

        /// <summary>
        /// hdt 唐忠宝修改，只是引入了 task 名称空间
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="excelSheetIndex"></param>
        internal void Load(string fileName, int excelSheetIndex)
        {
            // hdt
            if (string.IsNullOrEmpty(fileName))
                return;

            using (Stream stream = File.OpenRead(fileName))
            {
                this.Load(stream, excelSheetIndex);
            }
        }

        public void LoadWorkbook(XFile workbookFile, int excelSheetIndex, MemoryFolder mFolder)
        {
            if (workbookFile != null)
            {
                this.ReadStringTables(workbookFile, mFolder);
                this.ReadTheme(workbookFile, mFolder);
                this.ReadStyles(workbookFile, mFolder);
                this.ReadVBA(excelSheetIndex, workbookFile, mFolder);
                this.ReadWorkbook(workbookFile, mFolder, excelSheetIndex);
                this.ReadPivotCached(workbookFile, mFolder, excelSheetIndex);
                this.ReadExternalRefernce(workbookFile, mFolder);
                this.ReadWorksheets(workbookFile, mFolder, excelSheetIndex);
                this.SetLocalDefinedName();
            }
        }

        private void LogChartErrors(string chart)
        {
            foreach (string str in ParsingContext.ParsingErrors)
            {
                this.LogError(string.Format(ResourceHelper.GetResourceString("convertChartFormulaA1ToR1C1Error"), (object[]) new object[] { chart, str }), ExcelWarningCode.FormulaError, null);
            }
            ParsingContext.ParsingErrors.Clear();
        }

        private void LogError(string message, ExcelWarningCode warningCode, Exception ex)
        {
            this.LogError(message, warningCode, -1, -1, -1, ex);
        }

        private void LogError(string message, ExcelWarningCode warningCode, int sheet, int row, int column, Exception ex)
        {
            ExcelWarning excelWarning = new ExcelWarning(message, warningCode, sheet, row, column, ex);
            this._excelReader.OnExcelLoadError(excelWarning);
        }

        private void LogUnsupportedXmlContents(int sheet, string message)
        {
            ExcelWarning excelWarning = new ExcelWarning(ResourceHelper.GetResourceString("UnsupportedRecords"), ExcelWarningCode.UnsupportedRecords, sheet, -1, -1) {
                UnsupportedOpenXmlRecords = message
            };
            this._excelReader.OnExcelLoadError(excelWarning);
        }

        private void LogUnsupportedXmlContents(int sheet, MemoryFolder mFolder, XFile file)
        {
            if (file.FileName.ToUpper().EndsWith(".XML") || file.FileName.ToUpper().EndsWith(".RELS"))
            {
                Stream stream = mFolder.GetFile(file.FileName);
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                string message = XElement.Load(stream).ToString();
                this.LogUnsupportedXmlContents(sheet, message);
            }
        }

        private static bool ParseSharedFormulaStruct(Dt.Xls.OOXml.SharedFormula sharedFormula, int columnOffset, int rowOffset, int index)
        {
            if (index >= sharedFormula.BaseFormula.Length)
            {
                return true;
            }
            Match match = _cellRefRegex.Match(sharedFormula.BaseFormula.Substring(index));
            bool flag = false;
            if (match.Success)
            {
                flag = true;
                if (sharedFormula.ParsedSharedFormulaStructs == null)
                {
                    sharedFormula.ParsedSharedFormulaStructs = new List<ParsedSharedFormulaStruct>();
                }
                int num = 0;
                StringBuilder builder = new StringBuilder();
                builder.Clear();
                string s = match.Groups[2].Value;
                string str2 = match.Groups[4].Value;
                ParsedSharedFormulaStruct struct2 = new ParsedSharedFormulaStruct {
                    SharedFormulaType = 0,
                    Index = match.Index,
                    Length = match.Value.Length,
                    Column1 = IndexHelper.GetColumnIndexInNumber(s),
                    Row1 = int.Parse(str2),
                    firstSign = match.Groups[1].Value,
                    secondSign = match.Groups[3].Value
                };
                builder.Append(sharedFormula.BaseFormula.Substring(num + index, match.Index - num));
                string str3 = match.Value;
                if ((struct2.Index > 0) && (sharedFormula.BaseFormula[(struct2.Index + index) - 1] == '_'))
                {
                    builder.Append(str3);
                }
                else if ((struct2.firstSign == struct2.secondSign) && (struct2.firstSign == ""))
                {
                    str3 = str3.Replace(s, "{0}").Replace(str2, "{1}");
                    builder.Append(str3);
                }
                else if ((struct2.firstSign == "$") && (struct2.secondSign == ""))
                {
                    str3 = str3.Replace(str2, "{1}");
                    builder.Append(str3);
                }
                else if ((struct2.secondSign == "$") && (struct2.firstSign == ""))
                {
                    str3 = str3.Replace(s, "{0}");
                    builder.Append(str3);
                }
                else
                {
                    builder.Append(str3);
                }
                struct2.formatString = builder.ToString();
                sharedFormula.ParsedSharedFormulaStructs.Add(struct2);
                num = (index + match.Index) + match.Length;
                if ((num != sharedFormula.BaseFormula.Length) && !ParseSharedFormulaStruct(sharedFormula, columnOffset, rowOffset, num))
                {
                    struct2 = new ParsedSharedFormulaStruct {
                        formatString = sharedFormula.BaseFormula.Substring(num)
                    };
                    sharedFormula.ParsedSharedFormulaStructs.Add(struct2);
                }
            }
            else if (columnOffset == 0)
            {
                match = _cellRefRegex2.Match(sharedFormula.BaseFormula.Substring(index));
                if (match.Success)
                {
                    flag = true;
                    if (sharedFormula.ParsedSharedFormulaStructs == null)
                    {
                        sharedFormula.ParsedSharedFormulaStructs = new List<ParsedSharedFormulaStruct>();
                    }
                    int num2 = 0;
                    StringBuilder builder2 = new StringBuilder();
                    builder2.Clear();
                    string str4 = match.Groups[2].Value;
                    string str5 = match.Groups[4].Value;
                    ParsedSharedFormulaStruct struct3 = new ParsedSharedFormulaStruct {
                        SharedFormulaType = 2,
                        Index = match.Index,
                        Length = match.Value.Length,
                        Row1 = int.Parse(str4),
                        Row2 = int.Parse(str5),
                        firstSign = match.Groups[1].Value,
                        secondSign = match.Groups[3].Value
                    };
                    builder2.Append(sharedFormula.BaseFormula.Substring(num2 + index, match.Index - num2));
                    string str6 = sharedFormula.BaseFormula.Substring(match.Index + index, match.Length);
                    if ((struct3.Index > 0) && (sharedFormula.BaseFormula[(struct3.Index + index) - 1] == '_'))
                    {
                        builder2.Append(str6);
                    }
                    else if ((struct3.firstSign == struct3.secondSign) && (struct3.firstSign == ""))
                    {
                        builder2.Append("{0}:{1}");
                    }
                    else
                    {
                        builder2.Append(str6);
                    }
                    struct3.formatString = builder2.ToString();
                    sharedFormula.ParsedSharedFormulaStructs.Add(struct3);
                    num2 = (index + match.Index) + match.Value.Length;
                    if ((num2 != sharedFormula.BaseFormula.Length) && !ParseSharedFormulaStruct(sharedFormula, columnOffset, rowOffset, num2))
                    {
                        struct3 = new ParsedSharedFormulaStruct {
                            formatString = sharedFormula.BaseFormula.Substring(num2)
                        };
                        sharedFormula.ParsedSharedFormulaStructs.Add(struct3);
                    }
                }
            }
            else if (rowOffset == 0)
            {
                match = _cellRefRegex3.Match(sharedFormula.BaseFormula.Substring(index));
                if (match.Success)
                {
                    flag = true;
                    if (sharedFormula.ParsedSharedFormulaStructs == null)
                    {
                        sharedFormula.ParsedSharedFormulaStructs = new List<ParsedSharedFormulaStruct>();
                    }
                    int num3 = 0;
                    StringBuilder builder3 = new StringBuilder();
                    builder3.Clear();
                    string str7 = match.Groups[2].Value;
                    string str8 = match.Groups[4].Value;
                    ParsedSharedFormulaStruct struct4 = new ParsedSharedFormulaStruct {
                        SharedFormulaType = 1,
                        Index = match.Index,
                        Length = match.Value.Length,
                        Column1 = IndexHelper.GetColumnIndexInNumber(str7),
                        Column2 = IndexHelper.GetColumnIndexInNumber(str8),
                        firstSign = match.Groups[1].Value,
                        secondSign = match.Groups[3].Value
                    };
                    builder3.Append(sharedFormula.BaseFormula.Substring(num3 + index, match.Index - num3));
                    string str9 = sharedFormula.BaseFormula.Substring(match.Index + index, match.Length);
                    if ((struct4.Index > 0) && (sharedFormula.BaseFormula[(struct4.Index + index) - 1] == '_'))
                    {
                        builder3.Append(str9);
                    }
                    else if ((struct4.firstSign == struct4.secondSign) && (struct4.firstSign == ""))
                    {
                        builder3.Append("{0}:{1}");
                    }
                    else
                    {
                        builder3.Append(str9);
                    }
                    struct4.formatString = builder3.ToString();
                    sharedFormula.ParsedSharedFormulaStructs.Add(struct4);
                    num3 = (index + match.Index) + match.Value.Length;
                    if ((num3 != sharedFormula.BaseFormula.Length) && !ParseSharedFormulaStruct(sharedFormula, columnOffset, rowOffset, num3))
                    {
                        struct4 = new ParsedSharedFormulaStruct {
                            formatString = sharedFormula.BaseFormula.Substring(num3)
                        };
                        sharedFormula.ParsedSharedFormulaStructs.Add(struct4);
                    }
                }
            }
            if (sharedFormula.ParsedSharedFormulaStructs == null)
            {
                sharedFormula.ParsedSharedFormulaStructs = new List<ParsedSharedFormulaStruct>();
            }
            return flag;
        }

        private void ReadAlternateContent(XElement node, short sheet)
        {
            if (((node != null) && node.HasElements) && (this._excelReader is IExcelLosslessReader))
            {
                UnsupportRecord unsupportRecord = new UnsupportRecord {
                    Category = RecordCategory.AlternateContent,
                    FileType = ExcelFileType.XLSX,
                    Value = node.ToString()
                };
                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, unsupportRecord);
                this.LogUnsupportedXmlContents(sheet, node.ToString());
            }
        }

        private IExcelAutoFilter ReadAutoFilter(XElement node)
        {
            if ((node == null) || !node.HasAttributes)
            {
                return null;
            }
            ExcelAutoFilter filter = new ExcelAutoFilter();
            string str = node.GetAttributeValueOrDefaultOfStringType("ref", null);
            if (str.ToUpperInvariant() == "#REF!")
            {
                return null;
            }
            string[] strArray = str.Split(new char[] { ':' });
            Tuple<string, int> tuple = this.SpitRowColumn(strArray[0]);
            ExcelCellRange range = new ExcelCellRange {
                Row = tuple.Item2 - 1,
                Column = IndexHelper.GetColumnIndexInNumber(tuple.Item1),
                RowSpan = 1,
                ColumnSpan = 1
            };
            if (strArray.Length == 2)
            {
                tuple = this.SpitRowColumn(strArray[1]);
                range.RowSpan = tuple.Item2 - range.Row;
                range.ColumnSpan = (IndexHelper.GetColumnIndexInNumber(tuple.Item1) - range.Column) + 1;
            }
            filter.Range = range;
            if (node.TryGetChildElement("filterColumn") != null)
            {
                filter.FilterColumns = new List<IExcelFilterColumn>();
                foreach (XElement element2 in node.Elements())
                {
                    ExcelFilterColumn column = new ExcelFilterColumn();
                    string str2 = element2.GetAttributeValueOrDefaultOfStringType("colId", null);
                    if (!string.IsNullOrWhiteSpace(str2))
                    {
                        column.AutoFilterColumnId = (ushort) uint.Parse(str2);
                    }
                    XElement element = element2.TryGetChildElement("colorFilter");
                    if (element != null)
                    {
                        column.ColorFilter = new ExcelColorFilter();
                        column.ColorFilter.CellColor = element.GetAttributeValueOrDefaultOfBooleanType("cellColor", true);
                        column.ColorFilter.DxfId = element.GetAttributeValueOrDefaultOfUInt32Type("dxfId", 0, false);
                    }
                    XElement element4 = element2.TryGetChildElement("filters");
                    if (element4 != null)
                    {
                        ExcelCalendarType type;
                        column.Filters = new ExcelFilters();
                        column.Filters.Filter = new List<string>();
                        column.Filters.DateGroupItem = new List<IExcelDateGroupItem>();
                        column.Filters.Blank = element4.GetAttributeValueOrDefaultOfStringType("blank", null) == "1";
                        if (Enum.TryParse<ExcelCalendarType>(element4.GetAttributeValueOrDefaultOfStringType("calendarType", null), true, out type))
                        {
                            column.Filters.CalendarType = type;
                        }
                        foreach (XElement element5 in element4.Elements())
                        {
                            if (element5.Name.LocalName == "filter")
                            {
                                column.Filters.Filter.Add(element5.GetAttributeValueOrDefaultOfStringType("val", null));
                            }
                            else if (element5.Name.LocalName == "dateGroupItem")
                            {
                                ExcelDateTimeGrouping grouping;
                                ExcelDateGroupItem item = new ExcelDateGroupItem();
                                if (Enum.TryParse<ExcelDateTimeGrouping>(element5.GetAttributeValueOrDefaultOfStringType("dateTimeGrouping", null), true, out grouping))
                                {
                                    item.DateTimeGrouping = grouping;
                                }
                                item.Day = element5.GetAttributeValueOrDefaultOfUInt16Type("day", 1);
                                item.Hour = element5.GetAttributeValueOrDefaultOfUInt16Type("hour", 0);
                                item.Minute = element5.GetAttributeValueOrDefaultOfUInt16Type("minute", 0);
                                item.Month = element5.GetAttributeValueOrDefaultOfUInt16Type("month", 1);
                                item.Second = element5.GetAttributeValueOrDefaultOfUInt16Type("second", 0);
                                item.Year = element5.GetAttributeValueOrDefaultOfUInt16Type("year", 0);
                                column.Filters.DateGroupItem.Add(item);
                            }
                        }
                    }
                    XElement element6 = element2.TryGetChildElement("customFilters");
                    if (element6 != null)
                    {
                        column.CustomFilters = new ExcelCustomFilters();
                        string str3 = element6.GetAttributeValueOrDefaultOfStringType("and", null);
                        if (!string.IsNullOrWhiteSpace(str3))
                        {
                            column.CustomFilters.And = str3 == "1";
                        }
                        List<XElement> list = new List<XElement>(element6.Elements());
                        if (list.Count > 0)
                        {
                            ExcelFilterOperator @operator;
                            column.CustomFilters.Filter1 = new ExcelCustomFilter();
                            Enum.TryParse<ExcelFilterOperator>(list[0].GetAttributeValueOrDefaultOfStringType("operator", null), true, out @operator);
                            column.CustomFilters.Filter1.Operator = @operator;
                            column.CustomFilters.Filter1.Value = list[0].GetAttributeValueOrDefaultOfStringType("val", null);
                        }
                        if (list.Count > 1)
                        {
                            ExcelFilterOperator operator2;
                            column.CustomFilters.Filter2 = new ExcelCustomFilter();
                            Enum.TryParse<ExcelFilterOperator>(list[1].GetAttributeValueOrDefaultOfStringType("operator", null), true, out operator2);
                            column.CustomFilters.Filter2.Operator = operator2;
                            column.CustomFilters.Filter2.Value = list[1].GetAttributeValueOrDefaultOfStringType("val", null);
                        }
                    }
                    XElement element7 = element2.TryGetChildElement("dynamicFilter");
                    if (element7 != null)
                    {
                        ExcelDynamicFilterType type2;
                        column.DynamicFilter = new ExcelDynamicFilter();
                        string str4 = element7.GetAttributeValueOrDefaultOfStringType("type", null);
                        element7.GetAttributeValueOrDefaultOfStringType("maxValIso", null);
                        element7.GetAttributeValueOrDefaultOfStringType("valIso", null);
                        string str5 = element7.GetAttributeValueOrDefaultOfStringType("val", null);
                        string str6 = element7.GetAttributeValueOrDefaultOfStringType("maxVal", null);
                        if (Enum.TryParse<ExcelDynamicFilterType>(str4, true, out type2))
                        {
                            column.DynamicFilter.Type = type2;
                        }
                        if (!string.IsNullOrWhiteSpace(str5))
                        {
                            column.DynamicFilter.Value = str5;
                        }
                        if (!string.IsNullOrWhiteSpace(str6))
                        {
                            column.DynamicFilter.MaxValue = str6;
                        }
                    }
                    XElement element8 = element2.TryGetChildElement("iconFilter");
                    if (element8 != null)
                    {
                        ExcelIconSetType type3;
                        column.IconFilter = new ExcelIconFilter();
                        string str7 = element8.GetAttributeValueOrDefaultOfStringType("iconId", null);
                        if (!string.IsNullOrWhiteSpace(str7))
                        {
                            column.IconFilter.IconId = uint.Parse(str7);
                        }
                        else
                        {
                            column.IconFilter.NoIcon = true;
                        }
                        string str8 = element8.GetAttributeValueOrDefaultOfStringType("iconSet", null);
                        if (!string.IsNullOrWhiteSpace(str8) && Enum.TryParse<ExcelIconSetType>("Icon_" + str8, true, out type3))
                        {
                            column.IconFilter.IconSet = type3;
                        }
                    }
                    XElement element9 = element2.TryGetChildElement("top10");
                    if (element9 != null)
                    {
                        column.Top10 = new ExcelTop10();
                        column.Top10.FilterValue = element9.GetAttributeValueOrDefaultOfDoubleType("filterVal", double.NaN);
                        column.Top10.Value = element9.GetAttributeValueOrDefaultOfDoubleType("val", double.NaN);
                        column.Top10.Percent = element9.GetAttributeValueOrDefaultOfBooleanType("percent", false);
                        column.Top10.Top = element9.GetAttributeValueOrDefaultOfBooleanType("top", true);
                    }
                    filter.FilterColumns.Add(column);
                }
            }
            return filter;
        }

        private void ReadAutoFilter(XElement node, short sheet)
        {
            try
            {
                IExcelAutoFilter autoFilter = this.ReadAutoFilter(node);
                if (autoFilter != null)
                {
                    this._excelReader.SetAutoFilter(sheet, autoFilter);
                }
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("readAutoFilterError"), ExcelWarningCode.General, sheet, -1, -1, exception);
            }
        }

        private void ReadBackgroundFillStyleList(XElement node, short sheet)
        {
            this.ReadSolidFill(node, sheet, this._themedbgFillStyles);
        }

        private void ReadBookViews(XElement node, short sheet)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "workbookView")
                {
                    bool hidden = false;
                    switch (element.GetAttributeValueOrDefaultOfStringType("visibility", "visible"))
                    {
                        case "hidden":
                        case "veryHidden":
                            hidden = true;
                            break;
                    }
                    bool iconic = element.GetAttributeValueOrDefaultOfBooleanType("minimized", false);
                    bool showHorizontalScrollbarAsNeeded = element.GetAttributeValueOrDefaultOfBooleanType("showHorizontalScroll", true);
                    bool showVerticalScrollBarAsNeeded = element.GetAttributeValueOrDefaultOfBooleanType("showVerticalScroll", true);
                    bool showTabs = element.GetAttributeValueOrDefaultOfBooleanType("showSheetTabs", true);
                    int firstDisplayedTabIndex = element.GetAttributeValueOrDefaultOfInt32Type("firstSheet", 0);
                    int selectedTabIndex = element.GetAttributeValueOrDefaultOfInt32Type("activeTab", 0);
                    int tabRatio = element.GetAttributeValueOrDefaultOfInt32Type("tabRatio", 600);
                    int num4 = element.GetAttributeValueOrDefaultOfInt32Type("yWindow", 0);
                    int num5 = element.GetAttributeValueOrDefaultOfInt32Type("xWindow", 0);
                    int num6 = element.GetAttributeValueOrDefaultOfInt32Type("windowHeight", 0);
                    int num7 = element.GetAttributeValueOrDefaultOfInt32Type("windowWidth", 0);
                    ExcelRect rect = new ExcelRect((double) num5, (double) num4, (double) num7, (double) num6);
                    this._excelReader.SetWindow(rect, hidden, iconic);
                    this._excelReader.SetScroll(showHorizontalScrollbarAsNeeded, showVerticalScrollBarAsNeeded);
                    if (sheet == -1)
                    {
                        this._excelReader.SetTabs(showTabs, selectedTabIndex, firstDisplayedTabIndex, 1, tabRatio);
                    }
                    else
                    {
                        this._excelReader.SetTabs(showTabs, 0, 0, 1, tabRatio);
                    }
                }
            }
        }

        private void ReadBorders(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.HasElements)
                    {
                        ExcelBorder border = new ExcelBorder {
                            Left = GetBorder(element, "left"),
                            Right = GetBorder(element, "right"),
                            Top = GetBorder(element, "top"),
                            Bottom = GetBorder(element, "bottom")
                        };
                        _styleBorders.Add(border);
                    }
                }
            }
        }

        private void ReadCalcProperties(XElement node, short sheet)
        {
            this._calcProperty = new CalculationProperty();
            if ((node != null) && node.HasAttributes)
            {
                ExcelCalculationMode automatic = ExcelCalculationMode.Automatic;
                string str = node.GetAttributeValueOrDefaultOfStringType("calcMode", null);
                if (!string.IsNullOrWhiteSpace(str))
                {
                    if (str == "autoNoTable")
                    {
                        str = "AutomaticExceptTables";
                    }
                    Enum.TryParse<ExcelCalculationMode>(str, true, out automatic);
                }
                this._calcProperty.CalculationMode = automatic;
                this._calcProperty.ReCalculationBeforeSave = node.GetAttributeValueOrDefaultOfBooleanType("calcOnSave", true);
                this._calcProperty.MaxIterationCount = node.GetAttributeValueOrDefaultOfInt32Type("iterateCount", 100);
                this._calcProperty.IsIterateCalculate = node.GetAttributeValueOrDefaultOfBooleanType("iterate", false);
                this._calcProperty.IsFullPrecision = node.GetAttributeValueOrDefaultOfBooleanType("fullPrecision", true);
                this._calcProperty.MaximunChange = node.GetAttributeValueOrDefaultOfDoubleType("iterateDelta", 0.001);
                ExcelReferenceStyle result = ExcelReferenceStyle.A1;
                Enum.TryParse<ExcelReferenceStyle>(node.GetAttributeValueOrDefaultOfStringType("refMode", null), true, out result);
                this._calcProperty.RefMode = result;
                this._excelReader.SetCalculationProperty(this._calcProperty);
                ParsingContext.ReferenceStyle = this._calcProperty.RefMode;
            }
        }

        /// <summary>
        /// hdt 唐忠宝修改 ，把按参数传递的ref关键字改成编译前的 out 关键字。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="row"></param>
        /// <param name="sheet"></param>
        private void ReadCell(XElement node, int row, short sheet)
        {
            string str = node.GetAttributeValueOrDefaultOfStringType(XNameHelper.rName, null);
            int column = 0;
            if (!string.IsNullOrWhiteSpace(str))
            {
                column = IndexHelper.GetColumnIndexInNumber(str);
                this._columnCounter = new int?(column);
            }
            else if (this._columnCounter.HasValue)
            {
                this._columnCounter = new int?(this._columnCounter.Value + 1);
                column = this._columnCounter.Value;
            }
            else
            {
                this._columnCounter = 0;
                column = 0;
            }
            int num2 = row;
            try
            {
                CellType unknown = CellType.Unknown;
                object obj2 = null;
                string str2 = null;
                string str3 = null;
                string str4 = node.GetAttributeValueOrDefaultOfStringType(XNameHelper.tName, "n");
                int num3 = node.GetAttributeValueOrDefaultOfInt32Type(XNameHelper.sName, 0);
                ExcelFormula arrayFormula = null;
                foreach (XElement element in node.Elements())
                {
                    ExcelCalcError error;
                    int num10;
                    Dt.Xls.OOXml.SharedFormula formula4;
                    switch (element.Name.LocalName)
                    {
                        case "v":
                            switch (str4)
                            {
                                case "b":
                                {
                                    bool flag;
                                    unknown = CellType.Boolean;
                                    if (!bool.TryParse(element.Value, out flag))
                                    {
                                        goto Label_01E3;
                                    }
                                    obj2 = (bool) flag;
                                    continue;
                                }
                                case "d":
                                {
                                    unknown = CellType.Datetime;
                                    DateTime minValue = DateTime.MinValue;
                                    DateTime.TryParse(element.Value,(IFormatProvider) CultureInfo.CurrentCulture,((DateTimeStyles) DateTimeStyles.AssumeLocal),out minValue);
                                    obj2 = minValue;
                                    continue;
                                }
                                case "e":
                                    unknown = CellType.Error;
                                    error = null;
                                    switch (element.Value)
                                    {
                                        case "#DIV/0!":
                                            goto Label_0324;

                                        case "#N/A":
                                            goto Label_032D;

                                        case "#NAME?":
                                            goto Label_0336;

                                        case "#NULL!":
                                            goto Label_033F;

                                        case "#NUM!":
                                            goto Label_0348;

                                        case "#REF!":
                                            goto Label_0351;

                                        case "#VALUE!":
                                            goto Label_035A;
                                    }
                                    goto Label_0361;

                                case "inlineStr":
                                {
                                    obj2 = "";
                                    unknown = CellType.String;
                                    continue;
                                }
                                case "n":
                                {
                                    double num4;
                                    unknown = CellType.Numeric;
                                    double.TryParse(element.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4);
                                    obj2 = (double) num4;
                                    continue;
                                }
                                case "s":
                                {
                                    int num5;
                                    unknown = CellType.String;
                                    if ((!int.TryParse(element.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num5) || (num5 < 0)) || (num5 >= this.SST.Count))
                                    {
                                        goto Label_03EF;
                                    }
                                    obj2 = this.SST[num5];
                                    continue;
                                }
                                case "str":
                                {
                                    unknown = CellType.FormulaString;
                                    obj2 = element.Value;
                                    continue;
                                }
                            }
                            goto Label_040D;

                        case "f":
                            arrayFormula = new ExcelFormula();
                            if (!element.IsEmpty)
                            {
                                str2 = element.Value;
                                if (this._externalRefs.Count > 0)
                                {
                                    str2 = this.UpdateFormulaIfContainsExternalReference(str2);
                                }
                                if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                                {
                                    str3 = this.ConvertA1FormulaToR1C1Formula(str2, row, column);
                                }
                            }
                            if (element.GetAttributeValueOrDefaultOfStringType(XNameHelper.tName, null) == "array")
                            {
                                unknown = CellType.Array;
                                string[] strArray = element.GetAttributeValueOrDefaultOfStringType(XNameHelper.refName, null).Split(new char[] { ':' });
                                int rowFirst = 0;
                                int rowLast = 0;
                                int columnIndexInNumber = 0;
                                int num9 = 0;
                                rowFirst = IndexHelper.GetRowIndexInNumber(strArray[0]);
                                columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                                if (strArray.Length == 1)
                                {
                                    rowLast = rowFirst;
                                    num9 = columnIndexInNumber;
                                }
                                else
                                {
                                    rowLast = IndexHelper.GetRowIndexInNumber(strArray[1]);
                                    num9 = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                                }
                                arrayFormula.IsArrayFormula = true;
                                arrayFormula.SetFormula(str2, str3);
                                this._excelReader.SetArrayFormula(sheet, rowFirst, rowLast, (short) columnIndexInNumber, (short) num9, arrayFormula);
                            }
                            if (element.GetAttributeValueOrDefaultOfStringType(XNameHelper.tName, null) == "shared")
                            {
                                unknown = CellType.FormulaString;
                                num10 = element.GetAttributeValueOrDefaultOfInt32Type(XNameHelper.siName, -2147483648);
                                if (num10 != -2147483648)
                                {
                                    if (element.IsEmpty)
                                    {
                                        goto Label_068B;
                                    }
                                    Dt.Xls.OOXml.SharedFormula formula2 = new Dt.Xls.OOXml.SharedFormula {
                                        BaseFormula = element.Value
                                    };
                                    string str6 = element.GetAttributeValueOrDefaultOfStringType(XNameHelper.refName, null);
                                    if (!string.IsNullOrEmpty(str6))
                                    {
                                        string[] strArray2 = str6.Split(new char[] { ':' });
                                        if (strArray2.Length == 2)
                                        {
                                            int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray2[1]);
                                            int num12 = IndexHelper.GetColumnIndexInNumber(strArray2[1]);
                                            formula2.BaseRow = num2;
                                            formula2.BaseColumn = column;
                                            formula2.IsRowShared = rowIndexInNumber == formula2.BaseRow;
                                            if (formula2.IsRowShared)
                                            {
                                                formula2.Count = (num12 - formula2.BaseColumn) + 1;
                                            }
                                            else
                                            {
                                                formula2.Count = (rowIndexInNumber - formula2.BaseRow) + 1;
                                            }
                                        }
                                        else
                                        {
                                            formula2.BaseRow = IndexHelper.GetRowIndexInNumber(strArray2[0]);
                                            formula2.BaseColumn = IndexHelper.GetColumnIndexInNumber(strArray2[0]);
                                            formula2.IsRowShared = true;
                                            formula2.Count = 1;
                                        }
                                    }
                                    if (this._sharedFormula.ContainsKey(num10))
                                    {
                                        throw new ExcelException(ResourceHelper.GetResourceString("sharedFormulaError"), ExcelExceptionCode.IncorrectFile);
                                    }
                                    this._sharedFormula.Add(num10, formula2);
                                }
                            }
                            break;
                    }
                    continue;
                Label_01E3:
                    if ((element.Value != null) && (element.Value == "0"))
                    {
                        obj2 = false;
                    }
                    else if ((element.Value != null) && (element.Value == "1"))
                    {
                        obj2 = true;
                    }
                    else
                    {
                        obj2 = false;
                    }
                    continue;
                Label_0324:
                    error = ExcelCalcError.DivideByZero;
                    goto Label_0361;
                Label_032D:
                    error = ExcelCalcError.ArgumentOrFunctionNotAvailable;
                    goto Label_0361;
                Label_0336:
                    error = ExcelCalcError.WrongFunctionOrRangeName;
                    goto Label_0361;
                Label_033F:
                    error = ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty;
                    goto Label_0361;
                Label_0348:
                    error = ExcelCalcError.ValueRangeOverflow;
                    goto Label_0361;
                Label_0351:
                    error = ExcelCalcError.IllegalOrDeletedCellReference;
                    goto Label_0361;
                Label_035A:
                    error = ExcelCalcError.WrongTypeOfOperand;
                Label_0361:
                    obj2 = error;
                    continue;
                Label_03EF:
                    obj2 = element.Value;
                    continue;
                Label_040D:
                    unknown = CellType.Unknown;
                    obj2 = element.Value;
                    continue;
                Label_068B:
                    if (this._sharedFormula.TryGetValue(num10, out formula4))
                    {
                        int columnOffset = column - formula4.BaseColumn;
                        int rowOffset = num2 - formula4.BaseRow;
                        str2 = this.GetSharedFormula(formula4, columnOffset, rowOffset);
                    }
                }
                if (!string.IsNullOrWhiteSpace(str2))
                {
                    if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                    {
                        str3 = this.ConvertA1FormulaToR1C1Formula(str2, row, column);
                    }
                    arrayFormula.SetFormula(str2, str3);
                    this._excelReader.SetCell(sheet, num2, column, obj2, unknown, (short) (num3 + this._styleCellStyleXfs.Count), arrayFormula);
                }
                else
                {
                    this._excelReader.SetCell(sheet, num2, column, obj2, unknown, (short) (num3 + this._styleCellStyleXfs.Count), null);
                }
            }
            catch (Exception exception)
            {
                if (exception is ExcelException)
                {
                    ExcelException exception2 = exception as ExcelException;
                    if (exception2.Code == ExcelExceptionCode.ParseException)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readCellFormulaError"), ExcelWarningCode.General, sheet, num2, column, exception);
                    }
                    else
                    {
                        this.LogError(ResourceHelper.GetResourceString("readCellError"), ExcelWarningCode.General, sheet, num2, column, exception);
                    }
                }
                else
                {
                    this.LogError(ResourceHelper.GetResourceString("readCellError"), ExcelWarningCode.General, sheet, num2, column, exception);
                }
            }
        }

        /// <summary>
        /// hdt 唐忠宝 改ref为out
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="rowIndex"></param>
        private void ReadCell(XmlReader reader, short sheetIndex, int rowIndex)
        {
            int column = -1;
            string str = "n";
            short num2 = 0;
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.LocalName == "r")
                    {
                        column = IndexHelper.GetColumnIndexInNumber(reader.Value);
                    }
                    else if (reader.LocalName == "t")
                    {
                        str = reader.Value;
                    }
                    else if (reader.LocalName == "s")
                    {
                        num2 = short.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
                    }
                }
                while (reader.MoveToNextAttribute());
                reader.MoveToElement();
            }
            if (column == -1)
            {
                if (this._columnCounter.HasValue)
                {
                    column = this._columnCounter.Value + 1;
                }
                else
                {
                    column = 0;
                }
            }
            this._columnCounter = new int?(column);
            try
            {
                CellType numeric = CellType.Numeric;
                object obj2 = null;
                string str2 = null;
                string str3 = null;
                ExcelFormula arrayFormula = null;
                if (!reader.IsEmptyElement)
                {
                    int depth = reader.Depth;
                    while (reader.Read() && (reader.Depth > depth))
                    {
                        ExcelCalcError error;
                        if ((reader.Depth != (depth + 1)) || (reader.NodeType != ((XmlNodeType) ((int) XmlNodeType.Element))))
                        {
                            continue;
                        }
                        if ((reader.LocalName != "v") || reader.IsEmptyElement)
                        {
                            goto Label_03EC;
                        }
                        reader.Read();
                        string str4 = reader.Value;
                        switch (str)
                        {
                            case "b":
                            {
                                bool flag;
                                numeric = CellType.Boolean;
                                if (!bool.TryParse(str4, out flag))
                                {
                                    break;
                                }
                                obj2 = (bool) flag;
                                continue;
                            }
                            case "d":
                            {
                                numeric = CellType.Datetime;
                                DateTime minValue = DateTime.MinValue;
                                DateTime.TryParse(str4, (IFormatProvider) CultureInfo.InvariantCulture, ((DateTimeStyles) DateTimeStyles.AssumeLocal) | ((DateTimeStyles) DateTimeStyles.AdjustToUniversal), out minValue);
                                obj2 = minValue;
                                continue;
                            }
                            case "e":
                                numeric = CellType.Error;
                                error = null;
                                switch (str4)
                                {
                                    case "#DIV/0!":
                                        goto Label_030C;

                                    case "#N/A":
                                        goto Label_0315;

                                    case "#NAME?":
                                        goto Label_031E;

                                    case "#NULL!":
                                        goto Label_0327;

                                    case "#NUM!":
                                        goto Label_0330;

                                    case "#REF!":
                                        goto Label_0339;

                                    case "#VALUE!":
                                        goto Label_0342;
                                }
                                goto Label_0349;

                            case "inlineStr":
                            {
                                obj2 = "";
                                numeric = CellType.String;
                                continue;
                            }
                            case "n":
                            {
                                double num4;
                                numeric = CellType.Numeric;
                                double.TryParse(str4, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4);
                                obj2 = (double) num4;
                                continue;
                            }
                            case "s":
                            {
                                int num5;
                                numeric = CellType.String;
                                if ((!int.TryParse(str4, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num5) || (num5 < 0)) || (num5 >= this.SST.Count))
                                {
                                    goto Label_03CD;
                                }
                                obj2 = this.SST[num5];
                                continue;
                            }
                            case "str":
                            {
                                numeric = CellType.FormulaString;
                                obj2 = str4;
                                continue;
                            }
                            default:
                                goto Label_03E1;
                        }
                        if ((str4 != null) && (str4 == "0"))
                        {
                            obj2 = false;
                        }
                        else if ((str4 != null) && (str4 == "1"))
                        {
                            obj2 = true;
                        }
                        else
                        {
                            obj2 = false;
                        }
                        continue;
                    Label_030C:
                        error = ExcelCalcError.DivideByZero;
                        goto Label_0349;
                    Label_0315:
                        error = ExcelCalcError.ArgumentOrFunctionNotAvailable;
                        goto Label_0349;
                    Label_031E:
                        error = ExcelCalcError.WrongFunctionOrRangeName;
                        goto Label_0349;
                    Label_0327:
                        error = ExcelCalcError.InterSectionOfTwoCellRangesIsEmpty;
                        goto Label_0349;
                    Label_0330:
                        error = ExcelCalcError.ValueRangeOverflow;
                        goto Label_0349;
                    Label_0339:
                        error = ExcelCalcError.IllegalOrDeletedCellReference;
                        goto Label_0349;
                    Label_0342:
                        error = ExcelCalcError.WrongTypeOfOperand;
                    Label_0349:
                        obj2 = error;
                        continue;
                    Label_03CD:
                        obj2 = str4;
                        continue;
                    Label_03E1:
                        numeric = CellType.Unknown;
                        obj2 = str4;
                        continue;
                    Label_03EC:
                        if (reader.LocalName == "f")
                        {
                            Dt.Xls.OOXml.SharedFormula formula4;
                            str = null;
                            string str5 = null;
                            string str6 = null;
                            if (reader.MoveToFirstAttribute())
                            {
                                do
                                {
                                    if (reader.LocalName == "t")
                                    {
                                        str = reader.Value;
                                    }
                                    else if (reader.LocalName == "ref")
                                    {
                                        str5 = reader.Value;
                                    }
                                    else if (reader.LocalName == "si")
                                    {
                                        str6 = reader.Value;
                                    }
                                }
                                while (reader.MoveToNextAttribute());
                                reader.MoveToElement();
                            }
                            arrayFormula = new ExcelFormula();
                            if (!reader.IsEmptyElement)
                            {
                                reader.Read();
                                str2 = reader.Value;
                                if (this._externalRefs.Count > 0)
                                {
                                    str2 = this.UpdateFormulaIfContainsExternalReference(str2);
                                }
                                if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                                {
                                    str3 = this.ConvertA1FormulaToR1C1Formula(str2, rowIndex, column);
                                }
                            }
                            if (str == "array")
                            {
                                numeric = CellType.Array;
                                string[] strArray = str5.Split(new char[] { ':' });
                                int rowFirst = 0;
                                int rowLast = 0;
                                int columnIndexInNumber = 0;
                                int num9 = 0;
                                rowFirst = IndexHelper.GetRowIndexInNumber(strArray[0]);
                                columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                                if (strArray.Length == 1)
                                {
                                    rowLast = rowFirst;
                                    num9 = columnIndexInNumber;
                                }
                                else
                                {
                                    rowLast = IndexHelper.GetRowIndexInNumber(strArray[1]);
                                    num9 = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                                }
                                arrayFormula.IsArrayFormula = true;
                                arrayFormula.SetFormula(str2, str3);
                                this._excelReader.SetArrayFormula(sheetIndex, rowFirst, rowLast, (short) columnIndexInNumber, (short) num9, arrayFormula);
                                str2 = null;
                                continue;
                            }
                            if (str != "shared")
                            {
                                continue;
                            }
                            numeric = CellType.FormulaString;
                            if (str6 == null)
                            {
                                continue;
                            }
                            int num10 = int.Parse(str6, (IFormatProvider) CultureInfo.InvariantCulture);
                            if (!reader.IsEmptyElement)
                            {
                                Dt.Xls.OOXml.SharedFormula formula2 = new Dt.Xls.OOXml.SharedFormula {
                                    BaseFormula = reader.Value
                                };
                                if (!string.IsNullOrEmpty(str5))
                                {
                                    string[] strArray2 = str5.Split(new char[] { ':' });
                                    if (strArray2.Length == 2)
                                    {
                                        int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray2[1]);
                                        int num12 = IndexHelper.GetColumnIndexInNumber(strArray2[1]);
                                        formula2.BaseRow = rowIndex;
                                        formula2.BaseColumn = column;
                                        formula2.IsRowShared = rowIndexInNumber == formula2.BaseRow;
                                        if (formula2.IsRowShared)
                                        {
                                            formula2.Count = (num12 - formula2.BaseColumn) + 1;
                                        }
                                        else
                                        {
                                            formula2.Count = (rowIndexInNumber - formula2.BaseRow) + 1;
                                        }
                                    }
                                    else
                                    {
                                        formula2.BaseRow = IndexHelper.GetRowIndexInNumber(strArray2[0]);
                                        formula2.BaseColumn = IndexHelper.GetColumnIndexInNumber(strArray2[0]);
                                        formula2.IsRowShared = true;
                                        formula2.Count = 1;
                                    }
                                }
                                if (this._sharedFormula.ContainsKey(num10))
                                {
                                    throw new ExcelException(ResourceHelper.GetResourceString("sharedFormulaError"), ExcelExceptionCode.IncorrectFile);
                                }
                                this._sharedFormula.Add(num10, formula2);
                                continue;
                            }
                            if (this._sharedFormula.TryGetValue(num10, out formula4))
                            {
                                int columnOffset = column - formula4.BaseColumn;
                                int rowOffset = rowIndex - formula4.BaseRow;
                                str2 = this.GetSharedFormula(formula4, columnOffset, rowOffset);
                            }
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(str2))
                {
                    if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                    {
                        str3 = this.ConvertA1FormulaToR1C1Formula(str2, rowIndex, column);
                    }
                    arrayFormula.SetFormula(str2, str3);
                    this._excelReader.SetCell(sheetIndex, rowIndex, column, obj2, numeric, (short) (num2 + this._styleCellStyleXfs.Count), arrayFormula);
                }
                else
                {
                    this._excelReader.SetCell(sheetIndex, rowIndex, column, obj2, numeric, (short) (num2 + this._styleCellStyleXfs.Count), null);
                }
            }
            catch (Exception exception)
            {
                if (exception is ExcelException)
                {
                    ExcelException exception2 = exception as ExcelException;
                    if (exception2.Code == ExcelExceptionCode.ParseException)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readCellFormulaError"), ExcelWarningCode.General, sheetIndex, rowIndex, column, exception);
                    }
                    else
                    {
                        this.LogError(ResourceHelper.GetResourceString("readCellError"), ExcelWarningCode.General, sheetIndex, rowIndex, column, exception);
                    }
                }
                else
                {
                    this.LogError(ResourceHelper.GetResourceString("readCellError"), ExcelWarningCode.General, sheetIndex, rowIndex, column, exception);
                }
            }
        }

        private void ReadCellStyles(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    string str2;
                    string str = element.GetAttributeValueOrDefaultOfStringType("name", null);
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("xfId", 0);
                    ExtendedFormat format = this._styleCellStyleXfs[num];
                    if (element.TryGetAttributeValue("builtinId", out str2))
                    {
                        ExcelStyle style = new ExcelStyle {
                            Name = str,
                            Format = format.Clone()
                        };
                        bool flag = element.GetAttributeValueOrDefaultOfBooleanType("customBuiltin", false);
                        style.IsCustomBuiltIn = flag;
                        style.BuiltInStyle = (BuiltInStyleIndex) byte.Parse(str2);
                        if ((style.BuiltInStyle == BuiltInStyleIndex.ColumnLevel) || (style.BuiltInStyle == BuiltInStyleIndex.RowLevel))
                        {
                            style.OutLineLevel = element.GetAttributeValueOrDefaultOfByteType("iLevel", 0);
                        }
                        try
                        {
                            this._excelReader.SetExcelStyle(style);
                        }
                        catch (Exception exception)
                        {
                            this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, exception);
                        }
                    }
                    else
                    {
                        CustomExcelStyle style2 = new CustomExcelStyle {
                            Name = str,
                            Format = format.Clone()
                        };
                        try
                        {
                            this._excelReader.SetExcelStyle(style2);
                        }
                        catch (Exception exception2)
                        {
                            this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, exception2);
                        }
                    }
                }
            }
        }

        private void ReadCellStyleXfs(XElement node, short sheet)
        {
            this.ReadXFs(node, sheet, this._styleCellStyleXfs, true);
        }

        private void ReadCellXFs(XElement node, short sheet)
        {
            this.ReadXFs(node, sheet, this._styleCellXfs, false);
        }

        private static void ReadCfvoNode(IExcelConditionalFormatValueObject conditionalFormatValueObject, XElement cfvoNode)
        {
            string str = cfvoNode.GetAttributeValueOrDefaultOfStringType("type", null);
            if (str != null)
            {
                ExcelConditionalFormatValueObjectType min = ExcelConditionalFormatValueObjectType.Min;
                if (Enum.TryParse<ExcelConditionalFormatValueObjectType>(str, true, out min))
                {
                    conditionalFormatValueObject.Type = min;
                }
                if (cfvoNode.HasElements)
                {
                    foreach (XElement element in cfvoNode.Elements())
                    {
                        if (element.Name.LocalName == "f")
                        {
                            conditionalFormatValueObject.Value = element.Value;
                        }
                    }
                }
            }
        }

        private void ReadColors(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                XElement element = node.TryGetChildElement("indexedColors");
                if ((element != null) && element.HasElements)
                {
                    Dictionary<int, GcColor> palette = new Dictionary<int, GcColor>();
                    int num = 0;
                    using (IEnumerator<XElement> enumerator = element.Elements().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            uint num2 = enumerator.Current.GetAttributeValueOrDefaultOfUInt32Type("rgb", 0, true);
                            palette.Add(num, ColorExtension.FromArgb(num2 | 0xff000000));
                            num++;
                        }
                    }
                    this._excelReader.SetColorPalette(palette);
                }
            }
        }

        public static ColorScheme ReadColorScheme(XElement node)
        {
            string name = node.GetAttributeValueOrDefaultOfStringType("name", null);
            List<IExcelColor> schemeColors = new List<IExcelColor>();
            using (IEnumerator<XElement> enumerator = node.Elements().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ExcelColor color = TryReadThemeColor(enumerator.Current);
                    if (color != null)
                    {
                        schemeColors.Add(color);
                    }
                }
            }
            IExcelColor color2 = schemeColors[0];
            schemeColors[0] = schemeColors[1];
            schemeColors[1] = color2;
            color2 = schemeColors[2];
            schemeColors[2] = schemeColors[3];
            schemeColors[3] = color2;
            return new ColorScheme(name, schemeColors);
        }

        private void ReadColumnBreaks(XElement node, short sheet)
        {
            if (node != null)
            {
                int num1 = (int) node.GetAttributeValueOrDefault<int>("count", 0);
                List<int> list = new List<int>();
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "brk")
                    {
                        int num = element.GetAttributeValueOrDefaultOfInt32Type("id", 0);
                        if (num > 0)
                        {
                            list.Add(num);
                        }
                    }
                }
                this._excelPrintPageSetting.ColumnBreakLines = list;
            }
        }

        private void ReadColumnInfo(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                short columnFirst = 0;
                short columnLast = 0;
                short num3 = 0;
                double width = 0.0;
                byte outlineLevel = 0;
                bool hidden = false;
                bool collapsed = false;
                columnFirst =(short)( node.GetAttributeValueOrDefaultOfUInt16Type("min", 0) - 1);
                columnLast = (short)(node.GetAttributeValueOrDefaultOfUInt16Type("max", 0) - 1);
                num3 = (short) node.GetAttributeValueOrDefaultOfUInt16Type("style", 0);
                width = node.GetAttributeValueOrDefaultOfDoubleType("width", 8.0);
                hidden = node.GetAttributeValueOrDefaultOfBooleanType("hidden", false);
                collapsed = node.GetAttributeValueOrDefaultOfBooleanType("collapsed", false);
                outlineLevel = node.GetAttributeValueOrDefaultOfByteType("outlineLevel", 0);
                node.GetAttributeValueOrDefaultOfStringType("customWidth", null);
                if ((columnFirst >= 0) && (columnLast >= 0))
                {
                    this._excelReader.SetColumnInfo(sheet, columnFirst, columnLast, (short) (num3 + this._styleCellStyleXfs.Count), width, hidden, outlineLevel, collapsed);
                }
            }
        }

        private void ReadComment(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "comment")
                    {
                        StringBuilder builder = new StringBuilder();
                        string s = element.GetAttributeValueOrDefaultOfStringType("ref", null);
                        foreach (XElement element2 in element.Elements())
                        {
                            if (element2.Name.LocalName == "text")
                            {
                                foreach (XElement element3 in element2.Elements())
                                {
                                    if (element3.Name.LocalName == "r")
                                    {
                                        string childElementValue = element3.GetChildElementValue("t");
                                        if (!string.IsNullOrEmpty(childElementValue))
                                        {
                                            builder.Append(childElementValue);
                                        }
                                    }
                                }
                            }
                        }
                        int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(s);
                        int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(s);
                        this._excelReader.SetCellNote(sheet, rowIndexInNumber, columnIndexInNumber, false, builder.ToString());
                    }
                }
            }
        }

        private void ReadConditionalFormating(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                ExcelConditionalFormat format = new ExcelConditionalFormat {
                    IsOffice2007ConditionalFormat = true,
                    Ranges = this.GetRanges(node.GetAttributeValueOrDefaultOfStringType("sqref", null))
                };
                if ((format.Ranges != null) && (format.Ranges.Count != 0))
                {
                    try
                    {
                        foreach (XElement element in node.Elements())
                        {
                            ExcelConditionalFormatType type;
                            List<XElement> list;
                            ExcelColorScaleRule rule;
                            ExcelDataBarRule rule2;
                            XElement element2;
                            ExcelIconSetsRule rule4;
                            XElement element5;
                            string str4;
                            ExcelGeneralRule rule6;
                            if (!Enum.TryParse<ExcelConditionalFormatType>(element.GetAttributeValueOrDefaultOfStringType("type", null), true, out type))
                            {
                                continue;
                            }
                            int num = element.GetAttributeValueOrDefaultOfInt32Type("priority", 0);
                            switch (type)
                            {
                                case ExcelConditionalFormatType.CellIs:
                                {
                                    ExcelHighlightingRule rule5 = new ExcelHighlightingRule {
                                        Priority = num,
                                        StopIfTrue = element.GetAttributeValueOrDefaultOfBooleanType("stopIfTrue", false),
                                        DifferentialFormattingId = element.GetAttributeValueOrDefaultOfInt32Type("dxfId", -1)
                                    };
                                    ExcelConditionalFormattingOperator noComparison = ExcelConditionalFormattingOperator.NoComparison;
                                    Enum.TryParse<ExcelConditionalFormattingOperator>(element.GetAttributeValueOrDefaultOfStringType("operator", null), true, out noComparison);
                                    rule5.ComparisonOperator = noComparison;
                                    using (IEnumerator<XElement> enumerator3 = element.Elements().GetEnumerator())
                                    {
                                        while (enumerator3.MoveNext())
                                        {
                                            string formula = enumerator3.Current.Value;
                                            if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                                            {
                                                formula = this.ConvertA1FormulaToR1C1Formula(formula, 0, 0);
                                            }
                                            rule5.Formulas.Add(formula);
                                        }
                                    }
                                    format.IsOffice2007ConditionalFormat = false;
                                    format.ConditionalFormattingRules.Add(rule5);
                                    continue;
                                }
                                case ExcelConditionalFormatType.ColorScale:
                                    rule = new ExcelColorScaleRule {
                                        Priority = num
                                    };
                                    list = new List<XElement>(element.TryGetChildElement("colorScale").Elements());
                                    rule.HasMiddleNode = list.Count == 6;
                                    rule.Minimum.Type = this.GetConditionalFormatValueObjectType(list[0]);
                                    rule.Minimum.Value = list[0].GetAttributeValueOrDefaultOfStringType("val", null);
                                    if (!rule.HasMiddleNode)
                                    {
                                        break;
                                    }
                                    rule.Middle.Type = this.GetConditionalFormatValueObjectType(list[1]);
                                    rule.Middle.Value = list[1].GetAttributeValueOrDefaultOfStringType("val", null);
                                    rule.Maximum.Type = this.GetConditionalFormatValueObjectType(list[2]);
                                    rule.Maximum.Value = list[2].GetAttributeValueOrDefaultOfStringType("val", null);
                                    rule.MinimumColor = TryReadColor(list[3]);
                                    rule.MiddleColor = TryReadColor(list[4]);
                                    rule.MaximumColor = TryReadColor(list[5]);
                                    goto Label_0247;

                                case ExcelConditionalFormatType.DataBar:
                                {
                                    rule2 = new ExcelDataBarRule {
                                        Priority = num
                                    };
                                    element2 = element.TryGetChildElement("dataBar");
                                    string str2 = element2.GetAttributeValueOrDefaultOfStringType("showValue", null);
                                    if (string.IsNullOrWhiteSpace(str2) || (str2 != "0"))
                                    {
                                        goto Label_02A6;
                                    }
                                    rule2.ShowValue = false;
                                    goto Label_02AE;
                                }
                                case ExcelConditionalFormatType.IconSet:
                                {
                                    rule4 = new ExcelIconSetsRule {
                                        Priority = num
                                    };
                                    element5 = element.TryGetChildElement("iconSet");
                                    ExcelIconSetType result = ExcelIconSetType.Icon_3TrafficLights1;
                                    if (!Enum.TryParse<ExcelIconSetType>("Icon_" + element5.GetAttributeValueOrDefaultOfStringType("iconSet", "3TrafficLights1"), true, out result))
                                    {
                                        goto Label_04FB;
                                    }
                                    rule4.IconSet = result;
                                    goto Label_0503;
                                }
                                default:
                                    goto Label_06FC;
                            }
                            rule.Maximum.Type = this.GetConditionalFormatValueObjectType(list[1]);
                            rule.Maximum.Value = list[1].GetAttributeValueOrDefaultOfStringType("val", null);
                            rule.MinimumColor = TryReadColor(list[2]);
                            rule.MaximumColor = TryReadColor(list[3]);
                        Label_0247:
                            format.ConditionalFormattingRules.Add(rule);
                            continue;
                        Label_02A6:
                            rule2.ShowValue = true;
                        Label_02AE:
                            list = new List<XElement>(element2.Elements());
                            rule2.Minimum.Type = this.GetConditionalFormatValueObjectType(list[0]);
                            rule2.Minimum.Value = list[0].GetAttributeValueOrDefaultOfStringType("val", null);
                            rule2.MinimumDataBarLength = list[0].GetAttributeValueOrDefaultOfByteType("minLength", 10);
                            rule2.Maximum.Type = this.GetConditionalFormatValueObjectType(list[1]);
                            rule2.Maximum.Value = list[1].GetAttributeValueOrDefaultOfStringType("val", null);
                            rule2.MaximumDataBarLength = list[1].GetAttributeValueOrDefaultOfByteType("maxLength", 90);
                            rule2.Color = TryReadColor(list[2]);
                            if (element.TryGetChildElement("extLst") != null)
                            {
                                XElement element4 = element.TryGetChildElement("extLst").TryGetChildElement("ext");
                                if (element4 != null)
                                {
                                    string childElementValue = element4.GetChildElementValue("id");
                                    if (!string.IsNullOrEmpty(childElementValue))
                                    {
                                        Excel2010DataBarRule rule3 = new Excel2010DataBarRule {
                                            Priority = rule2.Priority,
                                            StopIfTrue = rule2.StopIfTrue,
                                            ShowValue = rule2.ShowValue
                                        };
                                        rule3.Minimum.Type = rule2.Minimum.Type;
                                        rule3.Minimum.Value = rule2.Minimum.Value;
                                        rule3.Maximum.Type = rule2.Maximum.Type;
                                        rule3.Maximum.Value = rule2.Maximum.Value;
                                        rule3.MinimumDataBarLength = rule2.MinimumDataBarLength;
                                        rule3.MaximumDataBarLength = rule2.MaximumDataBarLength;
                                        rule3.Color = rule2.Color;
                                        this._excel2010ExtensionDataBarRules.Add(childElementValue, new Tuple<IExcelConditionalFormat, IExcel2010DataBarRule>(format, rule3));
                                    }
                                }
                            }
                            else
                            {
                                format.ConditionalFormattingRules.Add(rule2);
                            }
                            continue;
                        Label_04FB:
                            rule4.IconSet = ExcelIconSetType.Icon_NIL;
                        Label_0503:
                            str4 = element5.GetAttributeValueOrDefaultOfStringType("showValue", null);
                            if (!string.IsNullOrWhiteSpace(str4) && (str4 == "0"))
                            {
                                rule4.IconOnly = true;
                            }
                            string str5 = element5.GetAttributeValueOrDefaultOfStringType("reverse", null);
                            if (!string.IsNullOrWhiteSpace(str5) && (str5 == "1"))
                            {
                                rule4.ReversedOrder = true;
                            }
                            list = new List<XElement>(element5.Elements());
                            foreach (XElement element6 in list)
                            {
                                ExcelConditionalFormatValueObject obj2 = new ExcelConditionalFormatValueObject {
                                    Type = this.GetConditionalFormatValueObjectType(element6),
                                    Value = element6.GetAttributeValueOrDefaultOfStringType("val", null)
                                };
                                string str6 = element6.GetAttributeValueOrDefaultOfStringType("gte", null);
                                if (!string.IsNullOrWhiteSpace(str6) || (str6 == "0"))
                                {
                                    rule4.NotPassTheThresholdsWhenEquals.Add(true);
                                }
                                else
                                {
                                    rule4.NotPassTheThresholdsWhenEquals.Add(false);
                                }
                                rule4.Thresholds.Add(obj2);
                            }
                            format.ConditionalFormattingRules.Add(rule4);
                            continue;
                        Label_06FC:
                            rule6 = new ExcelGeneralRule(type);
                            rule6.DifferentialFormattingId = element.GetAttributeValueOrDefaultOfInt32Type("dxfId", -1);
                            foreach (XAttribute attribute in element.Attributes())
                            {
                                if (attribute.Name.LocalName == "type")
                                {
                                    ExcelConditionalFormatType type3;
                                    if (Enum.TryParse<ExcelConditionalFormatType>(attribute.Value, true, out type3))
                                    {
                                        rule6.Type = type3;
                                    }
                                    rule6.AboveAverage = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("aboveAverage", true));
                                    if ((rule6.AboveAverage.HasValue && !rule6.AboveAverage.Value) && (rule6.Type == ExcelConditionalFormatType.AboveAverage))
                                    {
                                        rule6.Type = ExcelConditionalFormatType.BelowAverage;
                                    }
                                }
                                else if (attribute.Name.LocalName != "aboveAverage")
                                {
                                    if (attribute.Name.LocalName == "bottom")
                                    {
                                        rule6.Bottom = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("bottom", false));
                                    }
                                    else if (attribute.Name.LocalName == "equalAverage")
                                    {
                                        rule6.EqualAverage = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("equalAverage", false));
                                        if (rule6.EqualAverage.HasValue && rule6.EqualAverage.Value)
                                        {
                                            if (rule6.Type == ExcelConditionalFormatType.AboveAverage)
                                            {
                                                rule6.Type = ExcelConditionalFormatType.AboveOrEqualToAverage;
                                            }
                                            if (rule6.Type == ExcelConditionalFormatType.BelowAverage)
                                            {
                                                rule6.Type = ExcelConditionalFormatType.BelowOrEqualToAverage;
                                            }
                                        }
                                    }
                                    else if (attribute.Name.LocalName == "operator")
                                    {
                                        ExcelConditionalFormattingOperator operator2 = ExcelConditionalFormattingOperator.NoComparison;
                                        if (Enum.TryParse<ExcelConditionalFormattingOperator>(attribute.Value, true, out operator2))
                                        {
                                            rule6.Operator = new ExcelConditionalFormattingOperator?(operator2);
                                        }
                                    }
                                    else if (attribute.Name.LocalName == "percent")
                                    {
                                        rule6.Percent = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("percent", false));
                                    }
                                    else if (attribute.Name.LocalName == "priority")
                                    {
                                        rule6.Priority = element.GetAttributeValueOrDefaultOfInt32Type("priority", 0);
                                    }
                                    else if (attribute.Name.LocalName == "rank")
                                    {
                                        rule6.Rank = new int?(element.GetAttributeValueOrDefaultOfInt32Type("rank", 0));
                                    }
                                    else if (attribute.Name.LocalName == "stdDev")
                                    {
                                        rule6.StdDev = new int?(element.GetAttributeValueOrDefaultOfInt32Type("stdDev", 0));
                                    }
                                    else if (attribute.Name.LocalName == "stopIfTrue")
                                    {
                                        rule6.StopIfTrue = element.GetAttributeValueOrDefaultOfBooleanType("stopIfTrue", false);
                                    }
                                    else if (attribute.Name.LocalName == "text")
                                    {
                                        rule6.Text = element.GetAttributeValueOrDefaultOfStringType("text", null);
                                    }
                                    else if (attribute.Name.LocalName == "timePeriod")
                                    {
                                        ExcelConditionalFormatType today = ExcelConditionalFormatType.Today;
                                        if (Enum.TryParse<ExcelConditionalFormatType>(attribute.Value, true, out today))
                                        {
                                            rule6.Type = today;
                                        }
                                    }
                                }
                            }
                            foreach (XElement element8 in element.Elements())
                            {
                                string str8 = element8.Value;
                                if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                                {
                                    str8 = this.ConvertA1FormulaToR1C1Formula(str8, 0, 0);
                                }
                                rule6.Formulas.Add(element8.Value);
                            }
                            format.IsOffice2007ConditionalFormat = false;
                            format.ConditionalFormattingRules.Add(rule6);
                        }
                        format.Identifier = (short) this.identifier++;
                        this._excelReader.SetConditionalFormatting(sheet, format);
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readConditionalFormatError"), ExcelWarningCode.General, sheet, format.Ranges[0].Row, format.Ranges[0].Column, exception);
                    }
                }
            }
        }

        private void ReadContent(XElement node, short sheet)
        {
            this.ReadNode(node, sheet);
        }

        /// <summary>
        /// hdt 唐忠宝修改，修改最后的 case 2 中对range2 的初始化过程。
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sheet"></param>
        private void ReadDataValidation(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                List<IRange> list = null;
                try
                {
                    ExcelDataValidation dataValidation = new ExcelDataValidation();
                    ExcelDataValidationType none = ExcelDataValidationType.None;
                    ExcelDataValidationErrorStyle stop = ExcelDataValidationErrorStyle.Stop;
                    ExcelDataValidationOperator between = ExcelDataValidationOperator.Between;
                    Enum.TryParse<ExcelDataValidationType>((string) ((string) node.GetAttributeValueOrDefault<string>("type", "none")), true, out none);
                    Enum.TryParse<ExcelDataValidationErrorStyle>((string) ((string) node.GetAttributeValueOrDefault<string>("errorStyle", "stop")), true, out stop);
                    Enum.TryParse<ExcelDataValidationOperator>((string) ((string) node.GetAttributeValueOrDefault<string>("operator", "between")), true, out between);
                    dataValidation.Type = none;
                    dataValidation.ErrorStyle = stop;
                    dataValidation.CompareOperator = between;
                    dataValidation.AllowBlank = (bool) ((bool) node.GetAttributeValueOrDefault<bool>("allowBlank", false));
                    bool flag = (bool) ((bool) node.GetAttributeValueOrDefault<bool>("showDropDown", false));
                    dataValidation.ShowPromptBox = !flag;
                    dataValidation.ShowInputMessage = (bool) ((bool) node.GetAttributeValueOrDefault<bool>("showInputMessage", false));
                    dataValidation.ShowErrorBox = (bool) ((bool) node.GetAttributeValueOrDefault<bool>("showErrorMessage", false));
                    dataValidation.ErrorTitle = node.GetAttributeValueOrDefaultOfStringType("errorTitle", null);
                    dataValidation.Error = node.GetAttributeValueOrDefaultOfStringType("error", null);
                    if ((dataValidation.Error != null) && (_specicalPattern.Match(dataValidation.Error) != null))
                    {
                        dataValidation.Error = Regex.Replace(dataValidation.Error, "(_x005F)?_x00([0-1][0-9A-Fa-f])_", new MatchEvaluator(XlsxReader.EncodeEvaluator));
                    }
                    dataValidation.PromptTitle = node.GetAttributeValueOrDefaultOfStringType("promptTitle", null);
                    dataValidation.Prompt = node.GetAttributeValueOrDefaultOfStringType("prompt", null);
                    if ((dataValidation.Prompt != null) && (_specicalPattern.Match(dataValidation.Prompt) != null))
                    {
                        dataValidation.Prompt = Regex.Replace(dataValidation.Prompt, "(_x005F)?_x00([0-1][0-9A-Fa-f])_", new MatchEvaluator(XlsxReader.EncodeEvaluator));
                    }
                    if (this._calcProperty.RefMode == ExcelReferenceStyle.A1)
                    {
                        dataValidation.FirstFormula = node.GetChildElementValue("formula1");
                        dataValidation.SecondFormula = node.GetChildElementValue("formula2");
                    }
                    else
                    {
                        dataValidation.FirstFormula = this.ConvertA1FormulaToR1C1Formula(node.GetChildElementValue("formula1"), 0, 0);
                        dataValidation.SecondFormula = this.ConvertA1FormulaToR1C1Formula(node.GetChildElementValue("formula2"), 0, 0);
                    }
                    string str = node.GetAttributeValueOrDefaultOfStringType("sqref", null);
                    list = new List<IRange>();
                    foreach (string str2 in str.Split(new char[] { ' ' }))
                    {
                        string[] strArray2 = str2.Split(new char[] { ':' });
                        switch (strArray2.Length)
                        {
                            case 1:
                            {
                                int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray2[0]);
                                int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray2[0]);
                                ExcelCellRange range = new ExcelCellRange {
                                    Row = rowIndexInNumber,
                                    RowSpan = 1,
                                    Column = columnIndexInNumber,
                                    ColumnSpan = 1
                                };
                                list.Add(range);
                                break;
                            }
                            case 2:
                                ExcelCellRange range2;
                                range2 = new ExcelCellRange();                                
                                range2.Row = IndexHelper.GetRowIndexInNumber(strArray2[0]);
                                range2.RowSpan = (IndexHelper.GetRowIndexInNumber(strArray2[1]) - range2.Row) + 1;
                                range2.Column = IndexHelper.GetColumnIndexInNumber(strArray2[0]);
                                range2.ColumnSpan = (IndexHelper.GetColumnIndexInNumber(strArray2[1]) - range2.Column) + 1;  
                                list.Add(range2);
                                break;
                        }
                    }
                    dataValidation.Ranges = list;
                    this._excelReader.SetValidationData(sheet, dataValidation);
                }
                catch (Exception exception)
                {
                    if ((list != null) && (list.Count > 0))
                    {
                        this.LogError(ResourceHelper.GetResourceString("readDVError"), ExcelWarningCode.General, sheet, list[0].Row, list[0].Column, exception);
                    }
                    else
                    {
                        this.LogError(ResourceHelper.GetResourceString("readDVError"), ExcelWarningCode.General, sheet, -1, -1, exception);
                    }
                }
            }
        }

        private void ReadDataValidations(XElement node, short sheet)
        {
            if (node != null)
            {
                node.GetAttributeValueOrDefaultOfInt32Type("xWindow", 0);
                node.GetAttributeValueOrDefaultOfInt32Type("yWindow", 0);
                foreach (XElement element in node.Elements())
                {
                    this.ReadDataValidation(element, sheet);
                }
            }
        }

        private void ReadDefinedNames(XElement node, short sheet)
        {
            _definedNames.Clear();
            bool flag = false;
            if (this._calcProperty != null)
            {
                flag = this._calcProperty.RefMode == ExcelReferenceStyle.R1C1;
            }
            else
            {
                XElement element = node.Parent.TryGetChildElement("calcPr");
                if (element != null)
                {
                    flag = element.GetAttributeValueOrDefaultOfStringType("refMode", "") == "R1C1";
                }
            }
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element2 in node.Elements())
                {
                    string str = element2.GetAttributeValueOrDefaultOfStringType("localSheetId", null);
                    string name = element2.GetAttributeValueOrDefaultOfStringType("name", null);
                    short num = string.IsNullOrWhiteSpace(str) ? ((short) (-1)) : ((short) int.Parse(str));
                    bool flag2 = element2.GetAttributeValueOrDefaultOfBooleanType("hidden", false);
                    string str3 = element2.GetAttributeValueOrDefaultOfStringType("comment", null);
                    string formula = element2.Value;
                    if (flag)
                    {
                        formula = this.ConvertA1FormulaToR1C1Formula(formula, 0, 0);
                    }
                    switch (name)
                    {
                        case "_xlnm.Print_Area":
                            this._excelReader.SetPrintArea(num, formula);
                            break;

                        case "_xlnm.Print_Titles":
                            this._excelReader.SetPrintTitles(num, formula);
                            break;

                        default:
                        {
                            NamedCellRange range = new NamedCellRange(name, num) {
                                IsHidden = flag2,
                                Comment = str3
                            };
                            if (flag)
                            {
                                range.RefersToR1C1 = formula;
                            }
                            else
                            {
                                range.RefersTo = formula;
                            }
                            _definedNames.Add(range);
                            this._linkTable.AddDefinedNames(name, num, null);
                            break;
                        }
                    }
                }
            }
        }

        private void ReadDifferentialFormattingRecords(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                foreach (XElement element in node.Elements())
                {
                    DifferentialFormatting formatting = new DifferentialFormatting {
                        IsDFXExten = true
                    };
                    foreach (XElement element2 in element.Elements())
                    {
                        switch (element2.Name.LocalName)
                        {
                            case "font":
                                formatting.Font = ReadFont(element2);
                                break;

                            case "border":
                            {
                                ExcelTableBorder border = new ExcelTableBorder {
                                    Left = GetBorder(element2, "left"),
                                    Right = GetBorder(element2, "right"),
                                    Top = GetBorder(element2, "top"),
                                    Bottom = GetBorder(element2, "bottom"),
                                    Vertical = GetBorderIfExist(element2, "vertical"),
                                    Horizontal = GetBorderIfExist(element2, "horizontal")
                                };
                                formatting.Border = border;
                                break;
                            }
                            case "fill":
                            {
                                FillPatternType none = FillPatternType.None;
                                ExcelColor emptyColor = ExcelColor.EmptyColor;
                                ExcelColor color2 = ExcelColor.EmptyColor;
                                XElement element3 = element2.TryGetChildElement("patternFill");
                                if (element3 != null)
                                {
                                    string str2 = element3.GetAttributeValueOrDefaultOfStringType("patternType", null);
                                    if (!string.IsNullOrWhiteSpace(str2))
                                    {
                                        Enum.TryParse<FillPatternType>(str2, true, out none);
                                    }
                                    XElement element4 = element3.TryGetChildElement("fgColor");
                                    if (element4 != null)
                                    {
                                        emptyColor = TryReadColor(element4);
                                    }
                                    XElement element5 = element3.TryGetChildElement("bgColor");
                                    if (element5 != null)
                                    {
                                        color2 = TryReadColor(element5);
                                    }
                                    formatting.Fill = new Tuple<FillPatternType, IExcelColor, IExcelColor>(none, emptyColor, color2);
                                }
                                break;
                            }
                            case "numFmt":
                            {
                                string code = element2.GetAttributeValueOrDefaultOfStringType("formatCode", null);
                                int id = element2.GetAttributeValueOrDefaultOfInt32Type("numFmtId", 0);
                                formatting.NumberFormat = new ExcelNumberFormat(id, code);
                                break;
                            }
                            case "alignment":
                                ExcelHorizontalAlignment alignment;
                                ExcelVerticalAlignment alignment2;
                                Enum.TryParse<ExcelHorizontalAlignment>(element2.GetAttributeValueOrDefaultOfStringType("horizontal", null), true, out alignment);
                                Enum.TryParse<ExcelVerticalAlignment>(element2.GetAttributeValueOrDefaultOfStringType("vertical", null), true, out alignment2);
                                formatting.Alignment = new AlignmentBlock();
                                formatting.Alignment.HorizontalAlignment = alignment;
                                formatting.Alignment.VerticalAlignment = alignment2;
                                break;

                            case "protection":
                                formatting.IsHidden = element2.GetAttributeValueOrDefaultOfBooleanType("hidden", false);
                                formatting.IsLocked = element2.GetAttributeValueOrDefaultOfBooleanType("locked", false);
                                break;
                        }
                    }
                    if (formatting.NumberFormat == null)
                    {
                        formatting.FormatId = -1;
                    }
                    this._dxfRecords.Add(formatting);
                }
            }
        }

        private void ReadDimension(XElement node, short sheet)
        {
            string[] strArray = node.Attribute("ref").Value.Split(new char[] { ':' });
            if (strArray.Length == 2)
            {
                int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray[0]);
                int num2 = IndexHelper.GetRowIndexInNumber(strArray[1]);
                int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                int num4 = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                this._excelReader.SetDimensions(sheet, 0, Math.Max(rowIndexInNumber, num2) + 1, 0, (short) (Math.Max(columnIndexInNumber, num4) + 1));
            }
            else if (strArray.Length == 1)
            {
                int num5 = IndexHelper.GetRowIndexInNumber(strArray[0]);
                int num6 = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                this._excelReader.SetDimensions(sheet, 0, num5 + 1, 0, (short) (num6 + 1));
            }
        }

        private void ReadDrawingFile(XFile drawingFile, MemoryFolder mFolder, int sheetIndex)
        {
            if ((drawingFile != null) && !string.IsNullOrWhiteSpace(drawingFile.FileName))
            {
                this.ReadDrawings(mFolder, drawingFile, (short) sheetIndex);
            }
        }

        private void ReadDrawings(MemoryFolder mFolder, XFile drawingFile, short sheet)
        {
            Dictionary<string, XFile> chartFiles = new Dictionary<string, XFile>();
            Dictionary<string, XFile> imageFiles = new Dictionary<string, XFile>();
            Dictionary<string, Stream> dictionary3 = new Dictionary<string, Stream>();
            List<string> list = new List<string> { "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramColors", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramData", "http://schemas.microsoft.com/office/2007/relationships/diagramDrawing", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramLayout", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/diagramQuickStyle" };
            if (drawingFile.RelationFiles != null)
            {
                foreach (KeyValuePair<string, XFile> pair in drawingFile.RelationFiles)
                {
                    XFile fileByRelationID = drawingFile.GetFileByRelationID(pair.Key);
                    if (fileByRelationID.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart")
                    {
                        chartFiles[pair.Key] = fileByRelationID;
                    }
                    else if (fileByRelationID.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image")
                    {
                        imageFiles[pair.Key] = fileByRelationID;
                    }
                    else if ((pair.Value.Relationship != null) && (this._excelReader is IExcelLosslessReader))
                    {
                        UnsupportRecord unsupportRecord = new UnsupportRecord {
                            Category = RecordCategory.DrawingFileRelationShip,
                            FileType = ExcelFileType.XLSX,
                            Value = pair.Value.Relationship
                        };
                        (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, unsupportRecord);
                        if (list.Contains(fileByRelationID.FileType))
                        {
                            dictionary3[fileByRelationID.FileName] = mFolder.GetFile(fileByRelationID.FileName);
                            this.LogUnsupportedXmlContents(sheet, mFolder, fileByRelationID);
                        }
                    }
                }
            }
            if ((dictionary3.Count > 0) && (this._excelReader is IExcelLosslessReader))
            {
                UnsupportRecord record2 = new UnsupportRecord {
                    Category = RecordCategory.DrawingFileRelationFile,
                    FileType = ExcelFileType.XLSX,
                    Value = dictionary3
                };
                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(-1, record2);
            }
            Stream stream = mFolder.GetFile(drawingFile.FileName);
            stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            XElement element = XDocument.Load(stream).Root;
            if (((element != null) && element.HasElements) && (this._excelReader is IExcelChartReader))
            {
                foreach (XElement element2 in element.Elements())
                {
                    bool flag = false;
                    if ((element2.Name.LocalName == "oneCellAnchor") && element2.HasElements)
                    {
                        OneCellAnchor anchor = new OneCellAnchor();
                        IExcelChart chart = null;
                        IExcelImage excelImage = null;
                        foreach (XElement element3 in element2.Elements())
                        {
                            if (element3.Name.LocalName == "from")
                            {
                                anchor.FromRow = int.Parse(element3.GetChildElementValue("row"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor.FromColumn = int.Parse(element3.GetChildElementValue("col"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor.FromRowOffset = this.EMU2Pixles(int.Parse(element3.GetChildElementValue("rowOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                                anchor.FromColumnOffset = this.EMU2Pixles(int.Parse(element3.GetChildElementValue("colOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            else if (element3.Name.LocalName == "ext")
                            {
                                anchor.Width = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("cy", 0));
                                anchor.Height = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("cx", 0));
                            }
                            else if ((element3.Name.LocalName == "graphicFrame") && element3.HasElements)
                            {
                                AbsoluteAnchor anchor2;
                                chart = this.ReadGraphicFrame(mFolder, chartFiles, element3, out anchor2);
                            }
                            else if ((element3.Name.LocalName == "pic") && element3.HasElements)
                            {
                                AbsoluteAnchor anchor3;
                                excelImage = this.ReadPictureFrame(sheet, mFolder, imageFiles, element3, out anchor3);
                            }
                            else if (element3.Name.LocalName == "clientData")
                            {
                                bool flag2 = element3.GetAttributeValueOrDefaultOfBooleanType("fLocksWithSheet", true);
                                if (chart != null)
                                {
                                    chart.Locked = flag2;
                                }
                                if (excelImage != null)
                                {
                                    excelImage.Locked = flag2;
                                }
                            }
                        }
                        if ((anchor != null) && (chart != null))
                        {
                            chart.Anchor = anchor;
                            (this._excelReader as IExcelChartReader).SetExcelChart(sheet, chart);
                            this.LogChartErrors(chart.Name);
                            flag = true;
                        }
                        if ((anchor != null) && (excelImage != null))
                        {
                            excelImage.Anchor = anchor;
                            (this._excelReader as IExcelChartReader).SetExcelImage(sheet, excelImage);
                            flag = true;
                        }
                    }
                    if ((element2.Name.LocalName == "twoCellAnchor") && element2.HasElements)
                    {
                        TwoCellAnchor anchor4 = new TwoCellAnchor();
                        IExcelChart chart2 = null;
                        IExcelImage image2 = null;
                        foreach (XElement element4 in element2.Elements())
                        {
                            XElement element7;
                            if (element4.Name.LocalName == "from")
                            {
                                anchor4.FromRow = int.Parse(element4.GetChildElementValue("row"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor4.FromColumn = int.Parse(element4.GetChildElementValue("col"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor4.FromRowOffset = this.EMU2Pixles(int.Parse(element4.GetChildElementValue("rowOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                                anchor4.FromColumnOffset = this.EMU2Pixles(int.Parse(element4.GetChildElementValue("colOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                                continue;
                            }
                            if (element4.Name.LocalName == "to")
                            {
                                anchor4.ToRow = int.Parse(element4.GetChildElementValue("row"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor4.ToColumn = int.Parse(element4.GetChildElementValue("col"), (IFormatProvider) CultureInfo.InvariantCulture);
                                anchor4.ToRowOffset = this.EMU2Pixles(int.Parse(element4.GetChildElementValue("rowOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                                anchor4.ToColumnOffset = this.EMU2Pixles(int.Parse(element4.GetChildElementValue("colOff"), (IFormatProvider) CultureInfo.InvariantCulture));
                                continue;
                            }
                            if ((element4.Name.LocalName == "graphicFrame") && element4.HasElements)
                            {
                                AbsoluteAnchor anchor5;
                                chart2 = this.ReadGraphicFrame(mFolder, chartFiles, element4, out anchor5);
                                continue;
                            }
                            if ((element4.Name.LocalName == "pic") && element4.HasElements)
                            {
                                AbsoluteAnchor anchor6;
                                image2 = this.ReadPictureFrame(sheet, mFolder, imageFiles, element4, out anchor6);
                                continue;
                            }
                            if ((element4.Name.LocalName != "grpSp") || !element4.HasElements)
                            {
                                goto Label_0854;
                            }
                            foreach (XElement element5 in element4.Elements())
                            {
                                if (element5.Name.LocalName == "graphicFrame")
                                {
                                    AbsoluteAnchor anchor7;
                                    IExcelChart chart3 = this.ReadGraphicFrame(mFolder, chartFiles, element5, out anchor7);
                                    if (chart3 != null)
                                    {
                                        if (anchor7 != null)
                                        {
                                            chart3.Anchor = anchor7;
                                        }
                                        else
                                        {
                                            chart3.Anchor = anchor4;
                                        }
                                        (this._excelReader as IExcelChartReader).SetExcelChart(sheet, chart3);
                                        flag = true;
                                    }
                                }
                                else if (element5.Name.LocalName == "pic")
                                {
                                    AbsoluteAnchor anchor8;
                                    IExcelImage image3 = this.ReadPictureFrame(sheet, mFolder, imageFiles, element5, out anchor8);
                                    if ((image3 != null) && (anchor8 != null))
                                    {
                                        if (anchor8 != null)
                                        {
                                            image3.Anchor = anchor8;
                                        }
                                        else
                                        {
                                            image3.Anchor = anchor4;
                                        }
                                        (this._excelReader as IExcelChartReader).SetExcelImage(sheet, image3);
                                        flag = true;
                                    }
                                }
                            }
                            if (!(this._excelReader is IExcelLosslessReader))
                            {
                                continue;
                            }
                            while (true)
                            {
                                XElement element6 = element4.Element(XName.Get("graphicFrame", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"));
                                if (element6 == null)
                                {
                                    break;
                                }
                                element6.Remove();
                            }
                        Label_07C8:
                            element7 = element4.Element(XName.Get("pic", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing"));
                            if (element7 != null)
                            {
                                element7.Remove();
                                goto Label_07C8;
                            }
                            if (element4.Element(XName.Get("sp", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing")) != null)
                            {
                                string str = element2.ToString();
                                this.LogUnsupportedXmlContents(sheet, str.ToString());
                                UnsupportRecord record3 = new UnsupportRecord {
                                    Category = RecordCategory.Drawing,
                                    FileType = ExcelFileType.XLSX,
                                    Value = str
                                };
                                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, record3);
                            }
                            continue;
                        Label_0854:
                            if (element4.Name.LocalName == "clientData")
                            {
                                bool flag3 = element4.GetAttributeValueOrDefaultOfBooleanType("fLocksWithSheet", true);
                                if (chart2 != null)
                                {
                                    chart2.Locked = flag3;
                                }
                                if (image2 != null)
                                {
                                    image2.Locked = flag3;
                                }
                            }
                        }
                        if ((anchor4 != null) && (chart2 != null))
                        {
                            chart2.Anchor = anchor4;
                            (this._excelReader as IExcelChartReader).SetExcelChart(sheet, chart2);
                            this.LogChartErrors(chart2.Name);
                            flag = true;
                        }
                        if ((anchor4 != null) && (image2 != null))
                        {
                            image2.Anchor = anchor4;
                            (this._excelReader as IExcelChartReader).SetExcelImage(sheet, image2);
                            flag = true;
                        }
                    }
                    if ((element2.Name.LocalName == "absoluteAnchor") && element2.HasElements)
                    {
                        AbsoluteAnchor anchor9 = new AbsoluteAnchor();
                        IExcelChart chart4 = null;
                        IExcelImage image4 = null;
                        foreach (XElement element8 in element2.Elements())
                        {
                            if (element8.Name.LocalName == "pos")
                            {
                                anchor9.X = this.EMU2Pixles(element8.GetAttributeValueOrDefaultOfInt32Type("x", 0));
                                anchor9.Y = this.EMU2Pixles(element8.GetAttributeValueOrDefaultOfInt32Type("y", 0));
                            }
                            else if (element8.Name.LocalName == "ext")
                            {
                                anchor9.Width = this.EMU2Pixles(element8.GetAttributeValueOrDefaultOfInt32Type("cy", 0));
                                anchor9.Height = this.EMU2Pixles(element8.GetAttributeValueOrDefaultOfInt32Type("cx", 0));
                            }
                            else if ((element8.Name.LocalName == "graphicFrame") && element8.HasElements)
                            {
                                AbsoluteAnchor anchor10;
                                chart4 = this.ReadGraphicFrame(mFolder, chartFiles, element8, out anchor10);
                            }
                            else if ((element8.Name.LocalName == "pic") && element8.HasElements)
                            {
                                AbsoluteAnchor anchor11;
                                image4 = this.ReadPictureFrame(sheet, mFolder, imageFiles, element8, out anchor11);
                            }
                            else if (element8.Name.LocalName == "clientData")
                            {
                                bool flag4 = element8.GetAttributeValueOrDefaultOfBooleanType("fLocksWithSheet", true);
                                if (chart4 != null)
                                {
                                    chart4.Locked = flag4;
                                }
                                if (image4 != null)
                                {
                                    image4.Locked = flag4;
                                }
                            }
                        }
                        if ((anchor9 != null) && (chart4 != null))
                        {
                            chart4.Anchor = anchor9;
                            (this._excelReader as IExcelChartReader).SetExcelChart(sheet, chart4);
                            this.LogChartErrors(chart4.Name);
                            flag = true;
                        }
                        if ((anchor9 != null) && (image4 != null))
                        {
                            image4.Anchor = anchor9;
                            (this._excelReader as IExcelChartReader).SetExcelImage(sheet, image4);
                            flag = true;
                        }
                    }
                    if (!flag && (this._excelReader is IExcelLosslessReader))
                    {
                        string message = element2.ToString();
                        this.LogUnsupportedXmlContents(sheet, message);
                        if (!message.Contains("sle:slicer"))
                        {
                            UnsupportRecord record4 = new UnsupportRecord {
                                Category = RecordCategory.Drawing,
                                FileType = ExcelFileType.XLSX,
                                Value = element2.ToString()
                            };
                            (this._excelReader as IExcelLosslessReader).AddUnsupportItem(sheet, record4);
                        }
                    }
                }
            }
        }

        private void ReadEffectStyleList(XElement node, short sheet)
        {
        }

        private IExcelChart ReadExcelChart(XElement root, MemoryFolder mFolder, XFile chartFile)
        {
            if ((root == null) || !root.HasElements)
            {
                return null;
            }
            List<int> list = new List<int>();
            List<int> list2 = new List<int>();
            ExcelChart chart = null;
            bool flag = true;
            bool flag2 = true;
            int num = -1;
            foreach (XElement element in root.Elements())
            {
                if (element.Name.LocalName == "chart")
                {
                    chart = new ExcelChart();
                    chart.ReadXml(element, mFolder, chartFile);
                }
                else if (element.Name.LocalName == "style")
                {
                    num = element.GetAttributeValueOrDefaultOfInt32Type("val", -1);
                }
                else if (element.Name.LocalName == "roundedCorners")
                {
                    flag2 = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "date1904")
                {
                    flag = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "AlternateContent")
                {
                    foreach (XElement element2 in element.AsEnumerable())
                    {
                        if ((element2.Name.LocalName == "Choice") && (element2.GetNamespaceOfPrefix("c14").NamespaceName == "http://schemas.microsoft.com/office/drawing/2007/8/2/chart"))
                        {
                            list.Add((int) ((int) element2.GetChildElementAttributeValueOrDefault<int>("style", "val")));
                        }
                        else if (element2.Name.LocalName == "Fallback")
                        {
                            list2.Add((int) ((int) element2.GetChildElementAttributeValueOrDefault<int>("style", "val")));
                        }
                    }
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, chartFile);
                    if (chart != null)
                    {
                        chart.ChartFormat = format;
                    }
                }
                else if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format2 = new ExcelTextFormat();
                    format2.ReadXml(element, mFolder, chartFile);
                    if (chart != null)
                    {
                        chart.TextFormat = format2;
                    }
                }
            }
            if (chart == null)
            {
                return null;
            }
            chart.AlternateContentChoiceStyleList = list;
            chart.AlternateFallbackStyleList = list2;
            if (num != -1)
            {
                chart.DefaultStyleIndex = num;
            }
            chart.IsDate1904 = flag;
            chart.RoundedCorners = flag2;
            return chart;
        }

        private void ReadExtensionConditionalFormating(XElement node, short sheet)
        {
            IExcelConditionalFormat format = new ExcelConditionalFormat {
                IsOffice2007ConditionalFormat = true
            };
            IExcelConditionalFormatRule rule = null;
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "cfRule")
                {
                    string str = element.GetAttributeValueOrDefaultOfStringType("type", null);
                    if (str != null)
                    {
                        if (str.ToUpperInvariant() == "ICONSET")
                        {
                            rule = new ExcelIconSetsRule();
                        }
                        else if (str.ToUpperInvariant() == "DATABAR")
                        {
                            string str2 = element.GetAttributeValueOrDefaultOfStringType("id", null);
                            if (this._excel2010ExtensionDataBarRules.ContainsKey(str2))
                            {
                                Tuple<IExcelConditionalFormat, IExcel2010DataBarRule> tuple = this._excel2010ExtensionDataBarRules[str2];
                                if (tuple != null)
                                {
                                    format.Ranges = tuple.Item1.Ranges;
                                    format.IsOffice2007ConditionalFormat = tuple.Item1.IsOffice2007ConditionalFormat;
                                    format.Identifier = tuple.Item1.Identifier;
                                    rule = tuple.Item2;
                                }
                            }
                        }
                    }
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("priority", -1);
                    if ((rule != null) && (num >= 0))
                    {
                        rule.Priority = num;
                    }
                    if (rule is IExcelIconSetsRule)
                    {
                        this.ReadExtensionIconSetRule(rule, element);
                    }
                    if (rule is IExcel2010DataBarRule)
                    {
                        this.ReadExtensionDataBarRule(element, rule);
                    }
                }
                else if (element.Name.LocalName == "sqref")
                {
                    format.Ranges = this.GetRanges(element.Value);
                }
            }
            if (rule != null)
            {
                format.ConditionalFormattingRules.Add(rule);
                format.Identifier = (short) this.identifier++;
                this._excelReader.SetConditionalFormatting(sheet, format);
            }
        }

        private void ReadExtensionCondtionalFormating(short sheet, XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "conditionalFormattings")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "conditionalFormatting")
                        {
                            this.ReadExtensionConditionalFormating(element2, sheet);
                        }
                    }
                }
            }
        }

        private void ReadExtensionDataBarRule(XElement element, IExcelConditionalFormatRule rule)
        {
            Excel2010DataBarRule rule2 = rule as Excel2010DataBarRule;
            if (rule2 != null)
            {
                foreach (XElement element2 in element.Elements())
                {
                    if (element2.Name.LocalName == "dataBar")
                    {
                        rule2.MinimumDataBarLength = element2.GetAttributeValueOrDefaultOfByteType("minLength", rule2.MinimumDataBarLength);
                        rule2.MaximumDataBarLength = element2.GetAttributeValueOrDefaultOfByteType("maxLength", rule2.MaximumDataBarLength);
                        rule2.ShowValue = element2.GetAttributeValueOrDefaultOfBooleanType("showValue", rule2.ShowValue);
                        rule2.ShowBorder = element2.GetAttributeValueOrDefaultOfBooleanType("border", false);
                        rule2.IsGradientDatabar = element2.GetAttributeValueOrDefaultOfBooleanType("gradient", true);
                        DataBarDirection context = DataBarDirection.Context;
                        Enum.TryParse<DataBarDirection>(element2.GetAttributeValueOrDefaultOfStringType("direction", "context"), true, out context);
                        rule2.Direction = context;
                        rule2.NegativeBarColorAsPositive = element2.GetAttributeValueOrDefaultOfBooleanType("negativeBarColorSameAsPositive", false);
                        rule2.NegativeBorderColorSameAsPositive = element2.GetAttributeValueOrDefaultOfBooleanType("negativeBarBorderColorSameAsPositive", true);
                        DataBarAxisPosition automatic = DataBarAxisPosition.Automatic;
                        Enum.TryParse<DataBarAxisPosition>(element2.GetAttributeValueOrDefaultOfStringType("axisPosition", "automatic"), true, out automatic);
                        rule2.AxisPosition = automatic;
                        if (element2.HasElements)
                        {
                            List<XElement> list = new List<XElement>();
                            foreach (XElement element3 in element2.Elements())
                            {
                                if (element3.Name.LocalName == "cfvo")
                                {
                                    list.Add(element3);
                                }
                                else if (element3.Name.LocalName == "fillColor")
                                {
                                    rule2.Color = TryReadColor(element3);
                                }
                                else if (element3.Name.LocalName == "borderColor")
                                {
                                    rule2.BorderColor = TryReadColor(element3);
                                }
                                else if (element3.Name.LocalName == "negativeFillColor")
                                {
                                    rule2.NegativeFillColor = TryReadColor(element3);
                                }
                                else if (element3.Name.LocalName == "negativeBorderColor")
                                {
                                    rule2.NegativeBorderColor = TryReadColor(element3);
                                }
                                else if (element3.Name.LocalName == "axisColor")
                                {
                                    rule2.AxisColor = TryReadColor(element3);
                                }
                            }
                            if (list.Count == 2)
                            {
                                ReadCfvoNode(rule2.Minimum, list[0]);
                                ReadCfvoNode(rule2.Maximum, list[1]);
                            }
                        }
                    }
                }
            }
        }

        private void ReadExtensionIconSetRule(IExcelConditionalFormatRule rule, XElement item)
        {
            ExcelIconSetsRule rule2 = rule as ExcelIconSetsRule;
            foreach (XElement element in item.Elements())
            {
                if (element.Name.LocalName == "iconSet")
                {
                    ExcelIconSetType result = ExcelIconSetType.Icon_3TrafficLights1;
                    if (Enum.TryParse<ExcelIconSetType>("Icon_" + element.GetAttributeValueOrDefaultOfStringType("iconSet", "3TrafficLights1"), true, out result))
                    {
                        rule2.IconSet = result;
                    }
                    else
                    {
                        rule2.IconSet = ExcelIconSetType.Icon_3TrafficLights1;
                    }
                    rule2.IconOnly = !element.GetAttributeValueOrDefaultOfBooleanType("showValue", true);
                    rule2.ReversedOrder = element.GetAttributeValueOrDefaultOfBooleanType("reverse", false);
                    if (element.HasElements)
                    {
                        foreach (XElement element2 in element.Elements())
                        {
                            ExcelConditionalFormatValueObject obj2 = new ExcelConditionalFormatValueObject {
                                Type = this.GetConditionalFormatValueObjectType(element2)
                            };
                            if (element2.HasElements)
                            {
                                foreach (XElement element3 in element2.Elements())
                                {
                                    if (element3.Name.LocalName == "f")
                                    {
                                        obj2.Value = element3.Value;
                                    }
                                }
                            }
                            string str = element2.GetAttributeValueOrDefaultOfStringType("gte", null);
                            if (!string.IsNullOrWhiteSpace(str) || (str == "0"))
                            {
                                rule2.NotPassTheThresholdsWhenEquals.Add(true);
                            }
                            else
                            {
                                rule2.NotPassTheThresholdsWhenEquals.Add(false);
                            }
                            rule2.Thresholds.Add(obj2);
                        }
                    }
                }
            }
        }

        private void ReadExtensionList(XElement node, short sheet)
        {
            if (((node != null) && node.HasElements) && (sheet != -1))
            {
                foreach (XElement element in node.Elements())
                {
                    if (this.IsSparklineElement(element))
                    {
                        this.ReadSparklineSettings(sheet, element);
                    }
                    if (this.IsConditionalFormatElement(element))
                    {
                        this.ReadExtensionCondtionalFormating(sheet, element);
                    }
                }
            }
        }

        private void ReadExternalReferences(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                int num = 1;
                foreach (XElement element in node.Elements())
                {
                    string str;
                    if (this.TryReadAttribure(element, XNames.id, out str))
                    {
                        this._externalRefs.Add(num, new Tuple<string, string>(str, string.Empty));
                    }
                    num++;
                }
            }
        }

        private void ReadExternalRefernce(XFile workbookFile, MemoryFolder mFolder)
        {
            List<int> list = new List<int>((IEnumerable<int>) this._externalRefs.Keys);
            foreach (int num in list)
            {
                try
                {
                    Tuple<string, string> tuple = this._externalRefs[num];
                    XFile fileByRelationID = workbookFile.GetFileByRelationID(tuple.Item1);
                    string fileName = string.Empty;
                    using (Dictionary<string, XFile>.Enumerator enumerator2 = fileByRelationID.RelationFiles.GetEnumerator())
                    {
                        while (enumerator2.MoveNext())
                        {
                            fileName = Path.GetFileName(enumerator2.Current.Value.FileName);
                            this._externalRefs[num] = new Tuple<string, string>(tuple.Item1, fileName);
                        }
                    }
                    this._externalWorkbookSheetsNames.Clear();
                    this._externalFileName = fileName;
                    this._externalCellRanges.Add(fileName, new List<IName>());
                    this.ReadFile(fileByRelationID, mFolder, -1);
                    ExternalWorkbookInfo externWorkbookInfo = new ExternalWorkbookInfo {
                        Name = fileName,
                        SheetNames = new List<string>((IEnumerable<string>) this._externalWorkbookSheetsNames),
                        DefinedNames = new List<IName>(this._externalCellRanges[fileName])
                    };
                    this._excelReader.SetExternalReferencedWorkbookInfo(externWorkbookInfo);
                    this._externalWorkbookSheetsNames.Clear();
                    this._externalFileName = null;
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("externalReferenceError"), ExcelWarningCode.General, exception);
                }
            }
        }

        private void ReadExternalWorkbookInfo(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "sheetNames")
                    {
                        using (IEnumerator<XElement> enumerator2 = element.Elements().GetEnumerator())
                        {
                            while (enumerator2.MoveNext())
                            {
                                XAttribute attribute = enumerator2.Current.Attribute("val");
                                if (attribute != null)
                                {
                                    this._externalWorkbookSheetsNames.Add(attribute.Value);
                                }
                            }
                        }
                    }
                    if (element.Name.LocalName == "definedNames")
                    {
                        foreach (XElement element3 in element.Elements())
                        {
                            string name = element3.GetAttributeValueOrDefaultOfStringType("name", null);
                            string str2 = element3.GetAttributeValueOrDefaultOfStringType("refersTo", null);
                            short num = element3.GetAttributeValueOrDefaultOfInt16Type("sheetId", -1);
                            NamedCellRange range = new NamedCellRange(name, num) {
                                RefersTo = str2
                            };
                            this._externalCellRanges[this._externalFileName].Add(range);
                        }
                    }
                }
            }
        }

        private void ReadFile(XFile file, MemoryFolder mFolder, int sheetIndex)
        {
            if ((file != null) && !string.IsNullOrWhiteSpace(file.FileName))
            {
                if (file.RelationFiles != null)
                {
                    this._hyperLinksRelations.Clear();
                    foreach (KeyValuePair<string, XFile> pair in file.RelationFiles)
                    {
                        if (pair.Value.FileType.ToUpperInvariant().EndsWith("HYPERLINK"))
                        {
                            string fileName = pair.Value.FileName;
                            int index = fileName.ToUpperInvariant().IndexOf("HTTP");
                            if (index != -1)
                            {
                                this._hyperLinksRelations.Add(pair.Key, fileName.Substring(index));
                            }
                            else
                            {
                                this._hyperLinksRelations.Add(pair.Key, Path.GetFileName(fileName));
                            }
                        }
                    }
                }
                Stream stream = mFolder.GetFile(file.FileName);
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                this.ReadNode(XDocument.Load(stream).Root, (short) sheetIndex);
                if (file.RelationFiles != null)
                {
                    foreach (KeyValuePair<string, XFile> pair2 in file.RelationFiles)
                    {
                        if (pair2.Value.FileType.ToUpperInvariant().EndsWith("COMMENTS"))
                        {
                            Stream stream2 = mFolder.GetFile(pair2.Value.FileName);
                            stream2.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                            this.ReadNode(XDocument.Load(stream2).Root, (short) sheetIndex);
                        }
                    }
                }
            }
        }

        internal static Tuple<FillPatternType, ExcelColor, ExcelColor> ReadFillPattern(XElement element)
        {
            FillPatternType none;
            ExcelColor color = new ExcelColor(ExcelColorType.Indexed, 0x40, 0.0);
            ExcelColor color2 = new ExcelColor(ExcelColorType.Indexed, 0x41, 0.0);
            if (!Enum.TryParse<FillPatternType>(element.GetAttributeValueOrDefaultOfStringType("patternType", null), true, out none))
            {
                none = FillPatternType.None;
            }
            XElement node = element.TryGetChildElement("fgColor");
            if (node != null)
            {
                color = TryReadColor(node);
            }
            XElement element3 = element.TryGetChildElement("bgColor");
            if (element3 != null)
            {
                color2 = TryReadColor(element3);
            }
            return new Tuple<FillPatternType, ExcelColor, ExcelColor>(none, color, color2);
        }

        private void ReadFills(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.Element(XNames.patternFill) != null)
                    {
                        Tuple<FillPatternType, ExcelColor, ExcelColor> tuple = ReadFillPattern(element.Element(XNames.patternFill));
                        _styleFills.Add(tuple);
                    }
                    else
                    {
                        _styleFills.Add(new Tuple<FillPatternType, ExcelColor, ExcelColor>(FillPatternType.None, ExcelColor.EmptyColor, ExcelColor.EmptyColor));
                    }
                }
            }
        }

        private void ReadFillStyleList(XElement node, short sheet)
        {
            this.ReadSolidFill(node, sheet, this._themedFillStyles);
        }

        internal static ExcelFont ReadFont(XElement child)
        {
            ExcelFont font = new ExcelFont();
            if (child.TryGetChildElement("b") != null)
            {
                string str = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("b", "val"));
                if (string.IsNullOrWhiteSpace(str) || (str == "1"))
                {
                    font.IsBold = true;
                }
            }
            font.CharSetIndex = (byte) ((byte) child.GetChildElementAttributeValueOrDefault<byte>("charset", "val"));
            font.FontFamily = (ExcelFontFamily) ((byte) child.GetChildElementAttributeValueOrDefault<byte>("family", "val"));
            font.FontName = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("name", "val"));
            string str2 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("scheme", "val"));
            if (!string.IsNullOrWhiteSpace(str2))
            {
                FontSchemeCategory none = FontSchemeCategory.None;
                if (Enum.TryParse<FontSchemeCategory>(str2, true, out none))
                {
                    font.FontScheme = none;
                }
            }
            if (child.TryGetChildElement("u") != null)
            {
                UnderLineStyle single = UnderLineStyle.Single;
                string str3 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("u", "val"));
                if (!string.IsNullOrWhiteSpace(str3))
                {
                    Enum.TryParse<UnderLineStyle>(str3, true, out single);
                }
                font.UnderLineStyle = single;
            }
            if (child.TryGetChildElement("i") != null)
            {
                string str4 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("i", "val"));
                if (string.IsNullOrWhiteSpace(str4) || (str4 == "1"))
                {
                    font.IsItalic = true;
                }
            }
            if (child.TryGetChildElement("outline") != null)
            {
                string str5 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("outline", "val"));
                if (string.IsNullOrWhiteSpace(str5) || (str5 == "1"))
                {
                    font.IsOutlineStyle = true;
                }
            }
            if (child.TryGetChildElement("shadow") != null)
            {
                string str6 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("shadow", "val"));
                if (string.IsNullOrWhiteSpace(str6) || (str6 == "1"))
                {
                    font.IsShadowStyle = true;
                }
            }
            if (child.TryGetChildElement("strike") != null)
            {
                string str7 = (string) ((string) child.GetChildElementAttributeValueOrDefault<string>("strike", "val"));
                if (string.IsNullOrWhiteSpace(str7) || (str7 == "1"))
                {
                    font.IsStrikeOut = true;
                }
            }
            font.FontSize = (double) ((double) child.GetChildElementAttributeValueOrDefault<double>("sz", "val"));
            VerticalAlignRun baseLine = VerticalAlignRun.BaseLine;
            if (Enum.TryParse<VerticalAlignRun>((string) ((string) child.GetChildElementAttributeValueOrDefault<string>("vertAlign", "val")), true, out baseLine))
            {
                font.VerticalAlignRun = baseLine;
            }
            XElement element = child.TryGetChildElement("color");
            if (element != null)
            {
                element.GetAttributeValueOrDefaultOfBooleanType("auto", false);
                ExcelColor color = TryReadColor(element);
                if (color != ExcelColor.EmptyColor)
                {
                    font.FontColor = color;
                }
            }
            return font;
        }

        private void ReadFonts(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.HasElements)
                    {
                        ExcelFont font = ReadFont(element);
                        _styleFonts.Add(font);
                    }
                }
            }
        }

        public static FontScheme ReadFontScheme(XElement node)
        {
            string name = node.GetAttributeValueOrDefaultOfStringType("name", null);
            IThemeFonts majorFont = ReadThemeFonts(node.TryGetChildElement("majorFont"));
            return new FontScheme(name, majorFont, ReadThemeFonts(node.TryGetChildElement("minorFont")));
        }

        /// <summary>
        /// hdt 唐忠宝 修改ref关键字为 out
        /// </summary>
        /// <param name="mFolder"></param>
        /// <param name="chartFiles"></param>
        /// <param name="anchorChildItem"></param>
        /// <param name="absoluteAnchor"></param>
        /// <returns></returns>
        private IExcelChart ReadGraphicFrame(MemoryFolder mFolder, Dictionary<string, XFile> chartFiles, XElement anchorChildItem, out AbsoluteAnchor absoluteAnchor)
        {
            IExcelChart chart = null;
            string str = null;
            absoluteAnchor = null;
            bool flag = false;
            foreach (XElement element in anchorChildItem.Elements())
            {
                if (element.Name.LocalName == "nvGraphicFramePr")
                {
                    str = (string) (element.GetChildElementAttributeValueOrDefault<string>("cNvPr", "name") as string);
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "cNvPr")
                        {
                            flag = element2.GetAttributeValueOrDefaultOfBooleanType("hidden", false);
                        }
                    }
                }
                else if (element.Name.LocalName == "xfrm")
                {
                    absoluteAnchor = new AbsoluteAnchor();
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName == "off")
                        {
                            absoluteAnchor.X = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("x", 0));
                            absoluteAnchor.Y = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("y", 0));
                        }
                        else if (element3.Name.LocalName == "ext")
                        {
                            absoluteAnchor.Width = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("cx", 0));
                            absoluteAnchor.Height = this.EMU2Pixles(element3.GetAttributeValueOrDefaultOfInt32Type("cy", 0));
                        }
                    }
                }
                else if ((element.Name.LocalName == "graphic") && element.HasElements)
                {
                    foreach (XElement element4 in element.Elements())
                    {
                        if ((element4.Name.LocalName == "graphicData") && element4.HasElements)
                        {
                            foreach (XElement element5 in element4.Elements())
                            {
                                if (element5.Name.LocalName == "chart")
                                {
                                    string str2 = element5.GetAttributeValueOrDefaultOfStringType("id", null);
                                    XFile chartFile = null;
                                    if (chartFiles.TryGetValue(str2, out chartFile))
                                    {
                                        Stream file = mFolder.GetFile(chartFile.FileName);
                                        file.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                                        chart = this.ReadExcelChart(XDocument.Load(file).Root, mFolder, chartFile);
                                        chart.Name = str;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (chart != null)
            {
                chart.Name = str;
                chart.Hidden = flag;
            }
            return chart;
        }

        private void ReadHeaderFooter(XElement node, short sheet)
        {
            if (node != null)
            {
                this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterAlignWithPageMargin = node.GetAttributeValueOrDefaultOfBooleanType("alignWithMargins", true);
                this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentFirstPage = node.GetAttributeValueOrDefaultOfBooleanType("differentFirst", false);
                if (this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentFirstPage)
                {
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFirstPage = node.GetChildElementValue("firstHeader");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.FooterFirstPage = node.GetChildElementValue("firstFooter");
                }
                this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentOddEvenPages = node.GetAttributeValueOrDefaultOfBooleanType("differentOddEven", false);
                if (this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentOddEvenPages)
                {
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderEvenPage = node.GetChildElementValue("evenHeader");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.FooterEvenPage = node.GetChildElementValue("evenFooter");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderOddPage = node.GetChildElementValue("oddHeader");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.FooterOddPage = node.GetChildElementValue("oddFooter");
                }
                else
                {
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderEvenPage = node.GetChildElementValue("oddHeader");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.FooterEvenPage = node.GetChildElementValue("oddFooter");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderOddPage = node.GetChildElementValue("oddHeader");
                    this._excelPrintPageSetting.AdvancedHeadFooterSetting.FooterOddPage = node.GetChildElementValue("oddFooter");
                }
                this._excelPrintPageSetting.AdvancedHeadFooterSetting.HeaderFooterScalesWithDocument = node.GetAttributeValueOrDefaultOfBooleanType("scaleWithDoc", true);
            }
        }

        private void ReadHyperLink(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    int row = 0;
                    int rowIndexInNumber = 0;
                    int column = 0;
                    int columnIndexInNumber = 0;
                    try
                    {
                        string str4;
                        string str = element.GetAttributeValueOrDefaultOfStringType("ref", null);
                        string description = element.GetAttributeValueOrDefaultOfStringType("display", "");
                        if (!string.IsNullOrEmpty(str))
                        {
                            string[] strArray = str.Split(new char[] { ':' });
                            if (strArray.Length == 2)
                            {
                                row = IndexHelper.GetRowIndexInNumber(strArray[0]);
                                column = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                                rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray[1]);
                                columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                            }
                            else
                            {
                                row = rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray[0]);
                                column = columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                            }
                        }
                        string str3 = "";
                        foreach (XAttribute attribute in element.Attributes())
                        {
                            if (attribute.Name.LocalName == "id")
                            {
                                str3 = attribute.Value;
                                break;
                            }
                        }
                        if (this._hyperLinksRelations.TryGetValue(str3, out str4))//hdt ref 变 out
                        {
                            ExcelHyperLink hyperLink = new ExcelHyperLink(description, str4);
                            for (int i = row; i <= rowIndexInNumber; i++)
                            {
                                for (int j = column; j <= columnIndexInNumber; j++)
                                {
                                    this._excelReader.SetCellHyperLink(sheet, row, column, hyperLink);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readHyperLinkError"), ExcelWarningCode.General, sheet, row, column, exception);
                    }
                }
            }
        }

        private void ReadLineStyleList(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                int num = 0;
                foreach (XElement element in node.Elements())
                {
                    ExcelBorderSide side;
                    if (this.TryReadBorderSide(element, out side) && (side != null))
                    {
                        this._themeLineStyles[num] = side;
                    }
                    num++;
                }
            }
        }

        private void ReadMergeCells(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    int columnStart = 0;
                    int rowStart = 0;
                    try
                    {
                        string str;
                        if (element.TryGetAttributeValue("ref", out str) && (str != null))
                        {
                            string[] strArray = str.Split(new char[] { ':' });
                            columnStart = IndexHelper.GetColumnIndexInNumber(strArray[0]);
                            int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray[1]);
                            rowStart = IndexHelper.GetRowIndexInNumber(strArray[0]);
                            int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray[1]);
                            this._excelReader.SetMergeCells(sheet, rowStart, rowIndexInNumber, columnStart, columnIndexInNumber);
                        }
                    }
                    catch (Exception exception)
                    {
                        this.LogError(ResourceHelper.GetResourceString("readMergeCellError"), ExcelWarningCode.General, sheet, rowStart, columnStart, exception);
                    }
                }
            }
        }

        private void ReadNode(XElement node, short sheetIndex)
        {
            if (node != null)
            {
                foreach (XElement element in node.Elements())
                {
                    XlsxReadHandler handler;
                    if (this._xlsxHandlerMap.TryGetValue(element.Name.LocalName, out handler) && (handler != null))//hdt ref 变 out
                    {
                        try
                        {
                            handler(element, sheetIndex);
                        }
                        catch (Exception exception)
                        {
                            if (sheetIndex == -1)
                            {
                                this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { element.Name.LocalName }), ExcelWarningCode.General, exception);
                            }
                            else
                            {
                                this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { element.Name.LocalName }), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
                            }
                        }
                    }
                }
            }
        }

        private void ReadNumberFormats(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    int id = element.GetAttributeValueOrDefaultOfInt32Type("numFmtId", -2147483648);
                    if (id >= 0)
                    {
                        string code = element.GetAttributeValueOrDefaultOfStringType("formatCode", null);
                        _numberFormats.Add(id, new ExcelNumberFormat(id, code));
                    }
                }
            }
        }

        private void ReadPageMargins(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                double[] numArray = new double[6];
                string[] strArray = new string[] { "left", "right", "top", "bottom", "header", "footer" };
                for (int i = 0; i < 6; i++)
                {
                    string str;
                    if (this.TryReadAttribure(node, (XName) strArray[i], out str))
                    {
                        double.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out numArray[i]);//hdt ref 变 out
                    }
                }
                ExcelPrintPageMargin printMargin = new ExcelPrintPageMargin {
                    Left = numArray[0],
                    Right = numArray[1],
                    Top = numArray[2],
                    Bottom = numArray[3],
                    Header = numArray[4],
                    Footer = numArray[5]
                };
                this._excelReader.SetPrintPageMargin(sheet, printMargin);
            }
        }

        private void ReadPageSetup(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                this._excelPrintPageSetting.ShowColor = !node.GetAttributeValueOrDefaultOfBooleanType("blackAndWhite", false);
                this._excelPrintPageSetting.Copies = node.GetAttributeValueOrDefaultOfUInt16Type("copies", 1);
                this._excelPrintPageSetting.Draft = node.GetAttributeValueOrDefaultOfBooleanType("draft", false);
                this._excelPrintPageSetting.UseCustomStartingPage = node.GetAttributeValueOrDefaultOfBooleanType("useFirstPageNumber", false);
                this._excelPrintPageSetting.FirstPageNumber = node.GetAttributeValueOrDefaultOfInt16Type("firstPageNumber", 1);
                this._excelPrintPageSetting.SmartPrintPagesHeight = node.GetAttributeValueOrDefaultOfInt32Type("fitToHeight", 1);
                this._excelPrintPageSetting.SmartPrintPagesWidth = node.GetAttributeValueOrDefaultOfInt32Type("fitToWidth", 1);
                ExcelPrintPageOrder downThenOver = ExcelPrintPageOrder.DownThenOver;
                Enum.TryParse<ExcelPrintPageOrder>((string) ((string) node.GetAttributeValueOrDefault<string>("pageOrder", "auto")), true, out downThenOver);
                this._excelPrintPageSetting.PageOrder = downThenOver;
                int num = (int) ((int) node.GetAttributeValueOrDefault<int>("paperSize", 1));
                this._excelPrintPageSetting.PaperSizeIndex = num;
                node.GetAttributeValueOrDefaultOfStringType("paperHeight", null);
                node.GetAttributeValueOrDefaultOfStringType("paperWidth", null);
                this._excelPrintPageSetting.ZoomFactor = node.GetAttributeValueOrDefaultOfFloatType("scale", 100f) / 100f;
                ExcelPrintNotesStyle none = ExcelPrintNotesStyle.None;
                Enum.TryParse<ExcelPrintNotesStyle>(node.GetAttributeValueOrDefaultOfStringType("cellComments", "none"), true, out none);
                this._excelPrintPageSetting.CommentsStyle = none;
                ExcelCellErrorPrintStyle displayed = ExcelCellErrorPrintStyle.Displayed;
                Enum.TryParse<ExcelCellErrorPrintStyle>(node.GetAttributeValueOrDefaultOfStringType("errors", "displayed"), true, out displayed);
                this._excelPrintPageSetting.CellErrorPrintStyle = displayed;
                ExcelPrintOrientation auto = ExcelPrintOrientation.Auto;
                Enum.TryParse<ExcelPrintOrientation>(node.GetAttributeValueOrDefaultOfStringType("orientation", "auto"), true, out auto);
                this._excelPrintPageSetting.Orientation = auto;
            }
        }

        private void ReadPageSetupAdditionalProperty(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                this._excelPrintPageSetting.UseSmartPrint = node.GetAttributeValueOrDefaultOfBooleanType("fitToPage", false);
            }
        }

        private IExcelImage ReadPictureFrame(short sheet, MemoryFolder mFolder, Dictionary<string, XFile> imageFiles, XElement anchorChildItem, out AbsoluteAnchor absoluteAnchor)
        {
            ExcelImage image = null;
            string name = null;
            absoluteAnchor = null;
            bool flag = false;
            foreach (XElement element in anchorChildItem.Elements())
            {
                if (element.Name.LocalName == "nvPicPr")
                {
                    name = (string) (element.GetChildElementAttributeValueOrDefault<string>("cNvPr", "name") as string);
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "cNvPr")
                        {
                            flag = element2.GetAttributeValueOrDefaultOfBooleanType("hidden", false);
                        }
                    }
                }
                else if ((element.Name.LocalName == "blipFill") && element.HasElements)
                {
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName != "blip")
                        {
                            continue;
                        }
                        string str2 = element3.GetAttributeValueOrDefaultOfStringType("embed", null);
                        XFile file = null;
                        if (imageFiles.TryGetValue(str2, out file))//hdt ref 变 out
                        {
                            byte[] sourceArray = this.ReadStreamFully(mFolder.GetFile(file.FileName));
                            string str3 = Path.GetExtension(file.FileName).ToUpperInvariant();
                            ImageType bitmap = ImageType.Bitmap;
                            switch (str3)
                            {
                                case ".BMP":
                                    bitmap = ImageType.Bitmap;
                                    break;

                                case ".PNG":
                                    bitmap = ImageType.PNG;
                                    break;

                                case ".JPG":
                                case ".JPEG":
                                    bitmap = ImageType.JPG;
                                    break;

                                case ".GIF":
                                    bitmap = ImageType.Gif;
                                    break;

                                default:
                                    if (str3 == ".GIF")
                                    {
                                        bitmap = ImageType.Gif;
                                    }
                                    else
                                    {
                                        bitmap = ImageType.Unsupport;
                                    }
                                    break;
                            }
                            try
                            {
                                image = new ExcelImage(name, bitmap, sourceArray);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else if ((element.Name.LocalName == "spPr") && element.HasElements)
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, null);
                    if (image != null)
                    {
                        image.PictureFormat = format;
                    }
                    foreach (XElement element4 in element.Elements())
                    {
                        if (element4.Name.LocalName == "xfrm")
                        {
                            absoluteAnchor = new AbsoluteAnchor();
                            foreach (XElement element5 in element4.Elements())
                            {
                                if (element5.Name.LocalName == "off")
                                {
                                    absoluteAnchor.X = this.EMU2Pixles(element5.GetAttributeValueOrDefaultOfInt32Type("x", 0));
                                    absoluteAnchor.Y = this.EMU2Pixles(element5.GetAttributeValueOrDefaultOfInt32Type("y", 0));
                                }
                                else if (element5.Name.LocalName == "ext")
                                {
                                    absoluteAnchor.Width = this.EMU2Pixles(element5.GetAttributeValueOrDefaultOfInt32Type("cx", 0));
                                    absoluteAnchor.Height = this.EMU2Pixles(element5.GetAttributeValueOrDefaultOfInt32Type("cy", 0));
                                }
                            }
                        }
                    }
                }
            }
            if (image != null)
            {
                image.Name = name;
                image.Hidden = flag;
            }
            return image;
        }

        private void ReadPivotCached(XFile workbookFile, MemoryFolder mFolder, int excelSheetIndex)
        {
            if ((excelSheetIndex == -1) && (this._pivotCacheInfos != null))
            {
                foreach (PivotCacheInfo info in this._pivotCacheInfos)
                {
                    XFile fileByRelationID = workbookFile.GetFileByRelationID(info.rID);
                    this.LogUnsupportedXmlContents(-1, mFolder, fileByRelationID);
                    if (fileByRelationID.RelationFiles != null)
                    {
                        foreach (KeyValuePair<string, XFile> pair in fileByRelationID.RelationFiles)
                        {
                            fileByRelationID.GetFileByRelationID(pair.Key);
                            this.LogUnsupportedXmlContents(-1, mFolder, fileByRelationID);
                        }
                    }
                }
            }
        }

        private void ReadPivotChaches(XElement node, short sheet)
        {
            this._pivotCacheInfos.Clear();
            foreach (XElement element in node.Elements())
            {
                element.GetAttributeValueOrDefaultOfInt32Type("cachedId", 0);
                string str = element.GetAttributeValueOrDefaultOfStringType("id", null);
                PivotCacheInfo info = new PivotCacheInfo {
                    rID = str
                };
                this._pivotCacheInfos.Add(info);
            }
        }

        private void ReadPrintOptions(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                ExcelPrintOptions printOption = new ExcelPrintOptions();
                bool flag = node.GetAttributeValueOrDefaultOfBooleanType("gridLines", false);
                bool flag2 = node.GetAttributeValueOrDefaultOfBooleanType("gridLinesSet", true);
                if (flag && flag2)
                {
                    printOption.PrintGridLine = true;
                }
                else
                {
                    printOption.PrintGridLine = false;
                }
                printOption.HorizontalCentered = node.GetAttributeValueOrDefaultOfBooleanType("horizontalCentered", false);
                printOption.VerticalCentered = node.GetAttributeValueOrDefaultOfBooleanType("verticalCentered", false);
                printOption.PrintRowColumnsHeaders = node.GetAttributeValueOrDefaultOfBooleanType("headings", false);
                this._excelReader.SetPrintOption(sheet, printOption);
            }
        }

        private void ReadRow(XElement node, short sheet)
        {
            string str;
            byte num6;
            int row = node.GetAttributeValueOrDefaultOfInt32Type(XNameHelper.rName, 0) - 1;
            if (row >= 0)
            {
                this._rowCounter = new int?(row);
            }
            if (row < 0)
            {
                if (!this._rowCounter.HasValue)
                {
                    this._rowCounter = 0;
                    row = this._rowCounter.Value;
                }
                else
                {
                    this._rowCounter = new int?(this._rowCounter.Value + 1);
                    row = this._rowCounter.Value;
                }
            }
            int firstDefinedColumn = -1;
            int lastDefinedColumn = -1;
            bool collapsed = node.GetAttributeValueOrDefaultOfBooleanType(XNameHelper.collapsedName, false);
            bool zeroHeight = node.GetAttributeValueOrDefaultOfBooleanType(XNameHelper.hiddenName, false);
            double height = node.GetAttributeValueOrDefaultOfDoubleType(XNameHelper.htName, this._defaultRowHeight);
            short num5 = node.GetAttributeValueOrDefaultOfInt16Type(XNameHelper.sName, -32768);
            if (num5 == -32768)
            {
                num5 = -1;
            }
            num6 = num6 = node.GetAttributeValueOrDefaultOfByteType(XNameHelper.outlineLevelName, 0);
            if (this.TryReadAttribure(node, "spans", out str))
            {
                string[] strArray = str.Split(new char[] { ':' });
                if (strArray.Length == 2)
                {
                    int.TryParse(strArray[0], (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out firstDefinedColumn);//hdt ref 变 out
                    int.TryParse(strArray[1], (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out lastDefinedColumn);//hdt ref 变 out
                }
            }
            int num7 = (num5 == -1) ? -1 : (num5 + this._styleCellStyleXfs.Count);
            this._excelReader.SetRowInfo(sheet, row, firstDefinedColumn, lastDefinedColumn, (short) num7, height, num6, collapsed, zeroHeight, false, true);
            List<XElement> list = new List<XElement>(node.Elements());
            this._columnCounter = null;
            for (int i = 0; i < list.Count; i++)
            {
                this.ReadCell(list[i], row, sheet);
            }
        }

        private void ReadRow(XmlReader reader, short sheetIndex)
        {
            int row = -1;
            int firstDefinedColumn = -1;
            int lastDefinedColumn = -1;
            bool collapsed = false;
            bool zeroHeight = false;
            double height = this._defaultRowHeight;
            short num5 = -1;
            byte outlineLevel = 0;
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.LocalName == "r")
                    {
                        row = int.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture) - 1;
                    }
                    else if (reader.LocalName == "spans")
                    {
                        string[] strArray = reader.Value.Split(new char[] { ':' });
                        if (strArray.Length == 2)
                        {
                            int.TryParse(strArray[0], (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out firstDefinedColumn);//hdt ref 变 out
                            int.TryParse(strArray[1], (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out lastDefinedColumn);//hdt ref 变 out
                        }
                    }
                    else if (reader.LocalName == "s")
                    {
                        num5 = short.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
                    }
                    else if (reader.LocalName == "collapsed")
                    {
                        if (reader.Value == "1")
                        {
                            collapsed = true;
                        }
                    }
                    else if (reader.LocalName == "hidden")
                    {
                        if (reader.Value == "1")
                        {
                            zeroHeight = true;
                        }
                    }
                    else if (reader.LocalName == "ht")
                    {
                        height = double.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
                    }
                    else if (reader.LocalName == "outlineLevel")
                    {
                        outlineLevel = byte.Parse(reader.Value, (IFormatProvider) CultureInfo.InvariantCulture);
                    }
                }
                while (reader.MoveToNextAttribute());
                reader.MoveToElement();
            }
            if (row < 0)
            {
                if (!this._rowCounter.HasValue)
                {
                    row = 0;
                }
                else
                {
                    row = this._rowCounter.Value + 1;
                }
            }
            this._rowCounter = new int?(row);
            int num7 = (num5 == -1) ? -1 : (num5 + this._styleCellStyleXfs.Count);
            this._excelReader.SetRowInfo(sheetIndex, row, firstDefinedColumn, lastDefinedColumn, (short) num7, height, outlineLevel, collapsed, zeroHeight, false, true);
            this._columnCounter = null;
            if (!reader.IsEmptyElement)
            {
                int depth = reader.Depth;
                while (reader.Read() && (reader.Depth > depth))
                {
                    if (((reader.Depth == (depth + 1)) && (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))) && (reader.LocalName == "c"))
                    {
                        this.ReadCell(reader, sheetIndex, row);
                    }
                }
            }
        }

        private void ReadRowBreaks(XElement node, short sheet)
        {
            if (node != null)
            {
                node.GetAttributeValueOrDefaultOfInt32Type("count", 0);
                List<int> list = new List<int>();
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "brk")
                    {
                        int num = element.GetAttributeValueOrDefaultOfInt32Type("id", 0);
                        if (num > 0)
                        {
                            list.Add(num);
                        }
                    }
                }
                this._excelPrintPageSetting.RowBreakLines = list;
            }
        }

        private void ReadSharedStringItem(XElement node, short sheet)
        {
            string str = string.Empty;
            foreach (XElement element in node.Elements())
            {
                string str2;
                if (((str2 = element.Name.LocalName) != null) && (str2 != "phoneticPr"))
                {
                    if (str2 != "r")
                    {
                        if ((str2 != "rPh") && (str2 == "t"))
                        {
                            goto Label_00C3;
                        }
                    }
                    else
                    {
                        foreach (XElement element2 in element.Elements())
                        {
                            if (element2.Name.LocalName == "t")
                            {
                                str = str + element2.Value;
                            }
                        }
                    }
                }
                continue;
            Label_00C3:
                str = element.Value;
                if (_specicalPattern.Match(str) != null)
                {
                    str = Regex.Replace(str, "(_x005F)?_x00([0-1][0-9A-Fa-f])_", new MatchEvaluator(XlsxReader.EncodeEvaluator));
                }
            }
            this.SST.Add(str);
        }

        private void ReadSheetData(XmlReader reader, short sheetIndex)
        {
            if (!reader.IsEmptyElement)
            {
                int depth = reader.Depth;
                while (reader.Read() && (reader.Depth > depth))
                {
                    if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                    {
                        if ((reader.Depth == (depth + 1)) && (reader.LocalName == "row"))
                        {
                            this.ReadRow(reader, sheetIndex);
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                }
            }
        }

        private void ReadSheetFormatProperties(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                double d = node.GetAttributeValueOrDefaultOfDoubleType("defaultRowHeight", double.NaN);
                if (!double.IsNaN(d))
                {
                    this._excelReader.SetDefaultRowHeight(sheet, d);
                    this._defaultRowHeight = d;
                }
                double baseColumnWidth = node.GetAttributeValueOrDefaultOfDoubleType("baseColWidth", 8.0);
                double num3 = node.GetAttributeValueOrDefaultOfDoubleType("defaultColWidth", double.NaN);
                if (double.IsNaN(num3))
                {
                    this._excelReader.SetDefaultColumnWidth(sheet, baseColumnWidth, null);
                }
                else
                {
                    this._excelReader.SetDefaultColumnWidth(sheet, baseColumnWidth, new double?(num3));
                }
                int num4 = node.GetAttributeValueOrDefaultOfInt32Type("outlineLevelRow", 0) + 1;
                int num5 = node.GetAttributeValueOrDefaultOfInt32Type("outlineLevelCol", 0) + 1;
                this._excelReader.SetRowColumnGutters(sheet, 0, 0, (num4 > 0) ? ((short) num4) : ((short) 0), (num5 > 0) ? ((short) num5) : ((short) 0));
            }
        }

        private void ReadSheetPr(XElement node, short sheet)
        {
            if (node != null)
            {
                string str = node.GetAttributeValueOrDefaultOfStringType("codeName", null);
                if (!string.IsNullOrEmpty(str) && (this._excelReader is IExcelLosslessReader))
                {
                    (this._excelReader as IExcelLosslessReader).SetCodeName(sheet, str);
                }
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "outlinePr")
                    {
                        bool summaryRowsBelowDetail = element.GetAttributeValueOrDefaultOfBooleanType("summaryBelow", true);
                        bool summaryColumnsRightToDetail = element.GetAttributeValueOrDefaultOfBooleanType("summaryRight", true);
                        this._excelReader.SetOutlineDirection(sheet, summaryColumnsRightToDetail, summaryRowsBelowDetail);
                    }
                    else if (element.Name.LocalName == "pageSetUpPr")
                    {
                        this._excelPrintPageSetting.UseSmartPrint = element.GetAttributeValueOrDefaultOfBooleanType("fitToPage", false);
                    }
                    else if (element.Name.LocalName == "tabColor")
                    {
                        ExcelColor color = TryReadColor(element);
                        if (this._excelReader is IExcelReader2)
                        {
                            (this._excelReader as IExcelReader2).SetExcelSheetTabColor(sheet, color);
                        }
                    }
                }
            }
        }

        private void ReadSheetProtection(XElement node, short sheet)
        {
            if ((node != null) && node.HasAttributes)
            {
                bool isProtect = (bool) ((bool) node.GetAttributeValueOrDefault<bool>("sheet", false));
                if (isProtect)
                {
                    this._excelReader.SetProtect(sheet, isProtect);
                }
            }
        }

        private void ReadSheets(XElement node, short sheet)
        {
            this._sheetIDs.Clear();
            int num = 0;
            foreach (XElement element in node.Elements())
            {
                if ((sheet < 0) || (num == sheet))
                {
                    string str;
                    string str2;
                    string str3;
                    uint num3;
                    string str4;
                    this.TryReadAttribure(element, XNames.name, out str);
                    this.TryReadAttribure(element, XNames.state, out str2);
                    byte hiddenState = 0;
                    if (str2 != null)
                    {
                        if (str2 == "hidden")
                        {
                            hiddenState = 1;
                        }
                        else if (str2 == "veryHidden")
                        {
                            hiddenState = 2;
                        }
                    }
                    SheetInfo info = new SheetInfo {
                        name = str,
                        index = num
                    };
                    if ((this.TryReadAttribure(element, XNames.sheetId, out str3) && !string.IsNullOrEmpty(str3)) && uint.TryParse(str3, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num3))//hdt ref 变 out
                    {
                        info.sheetID = num3;
                    }
                    if (this.TryReadAttribure(element, XNames.id, out str4))
                    {
                        info.rID = str4;
                    }
                    if (!string.IsNullOrWhiteSpace(info.rID))
                    {
                        this._sheetIDs.Add(info);
                    }
                    ExcelSheetType worksheet = ExcelSheetType.Worksheet;
                    if (this._workbookFile != null)
                    {
                        XFile fileByRelationID = this._workbookFile.GetFileByRelationID(str4);
                        if ((fileByRelationID != null) && (fileByRelationID.FileType != null))
                        {
                            if (fileByRelationID.FileType.EndsWith("chartsheet"))
                            {
                                worksheet = ExcelSheetType.ChartSheet;
                            }
                            else if (fileByRelationID.FileType.EndsWith("dialogsheet"))
                            {
                                worksheet = ExcelSheetType.DialogSheet;
                            }
                        }
                    }
                    this._excelReader.AddSheet(str, hiddenState, worksheet);
                }
                num++;
            }
        }

        private void ReadSheetTable(XElement node, short sheet)
        {
            if (this._excelReader is IExcelTableReader)
            {
                ExcelTable table = new ExcelTable();
                if (((node != null) && node.HasAttributes) && node.HasElements)
                {
                    table.Id = node.GetAttributeValueOrDefaultOfInt32Type("id", 0);
                    string str = node.GetAttributeValueOrDefaultOfStringType("name", null);
                    table.Name = str;
                    string str2 = node.GetAttributeValueOrDefaultOfStringType("displayName", null);
                    table.DisplayName = str2;
                    string sqref = node.GetAttributeValueOrDefaultOfStringType("ref", null);
                    IRange range = this.GetRange(sqref);
                    table.Range = range;
                    if (node.GetAttributeValueOrDefaultOfInt16Type("headerRowCount", 1) >= 1)
                    {
                        table.ShowHeaderRow = true;
                    }
                    table.ShowTotalsRow = node.GetAttributeValueOrDefaultOfBooleanType("totalsRowShown", true);
                    int num2 = node.GetAttributeValueOrDefaultOfInt32Type("totalsRowCount", -1);
                    if (num2 < 1)
                    {
                        table.ShowTotalsRow = false;
                    }
                    else if (num2 >= 1)
                    {
                        table.ShowTotalsRow = true;
                    }
                    foreach (XElement element in node.Elements())
                    {
                        if (element.Name.LocalName == "autoFilter")
                        {
                            table.AutoFilter = this.ReadAutoFilter(element);
                        }
                        if (element.Name.LocalName == "tableColumns")
                        {
                            foreach (XElement element2 in element.Elements())
                            {
                                table.Columns.Add(this.ReadTableColumn(element2));
                            }
                        }
                        if (element.Name.LocalName == "tableStyleInfo")
                        {
                            table.TableStyleInfo = this.ReadTableStyleInfo(element);
                        }
                    }
                    (this._excelReader as IExcelTableReader).SetTable(sheet, table);
                }
            }
        }

        private void ReadSheetViews(XElement node, short sheet)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    bool showFormula = false;
                    bool showZeros = true;
                    bool showGridLine = true;
                    bool showRowColumnHeader = true;
                    bool rightToLeftColumns = false;
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("colorId", -2147483648);
                    if (num >= 0)
                    {
                        this._excelReader.SetGridlineColor(sheet, new ExcelColor(ExcelColorType.Indexed, (uint) num, 0.0));
                    }
                    showGridLine = element.GetAttributeValueOrDefaultOfBooleanType("showGridLines", true);
                    showRowColumnHeader = element.GetAttributeValueOrDefaultOfBooleanType("showRowColHeaders", true);
                    showFormula = element.GetAttributeValueOrDefaultOfBooleanType("showFormulas", false);
                    showZeros = element.GetAttributeValueOrDefaultOfBooleanType("showZeros", true);
                    rightToLeftColumns = element.GetAttributeValueOrDefaultOfBooleanType("rightToLeft", false);
                    string s = (string) ((string) element.GetAttributeValueOrDefault<string>("topLeftCell", "A1"));
                    this._excelReader.SetTopLeft(sheet, IndexHelper.GetRowIndexInNumber(s), IndexHelper.GetColumnIndexInNumber(s));
                    this._excelReader.SetDisplayElements(sheet, showFormula, showZeros, showGridLine, showRowColumnHeader, rightToLeftColumns);
                    double num2 = element.GetAttributeValueOrDefaultOfDoubleType("zoomScale", 100.0);
                    this._excelReader.SetZoom(sheet, ((float) num2) / 100f);
                    if (element.HasElements)
                    {
                        foreach (XElement element2 in element.Elements())
                        {
                            string localName = element2.Name.LocalName;
                            if (localName != null)
                            {
                                if (localName == "selection")
                                {
                                    string str;
                                    int rowActive = -1;
                                    int columnActive = -1;
                                    int selectionCount = 0;
                                    List<int> rowFirst = new List<int>();
                                    List<int> rowLast = new List<int>();
                                    List<int> colFirst = new List<int>();
                                    List<int> colLast = new List<int>();
                                    bool flag6 = false;
                                    if (this.TryReadAttribure(element2, "sqref", out str))
                                    {
                                        int num8;
                                        foreach (string str3 in str.Split(new char[] { ' ' }))
                                        {
                                            string[] strArray2 = str3.Split(new char[] { ':' });
                                            switch (strArray2.Length)
                                            {
                                                case 1:
                                                {
                                                    int rowIndexInNumber = IndexHelper.GetRowIndexInNumber(strArray2[0]);
                                                    int columnIndexInNumber = IndexHelper.GetColumnIndexInNumber(strArray2[0]);
                                                    rowFirst.Add(rowIndexInNumber);
                                                    rowLast.Add(rowIndexInNumber);
                                                    colFirst.Add(columnIndexInNumber);
                                                    colLast.Add(columnIndexInNumber);
                                                    selectionCount++;
                                                    break;
                                                }
                                                case 2:
                                                    rowFirst.Add(IndexHelper.GetRowIndexInNumber(strArray2[0]));
                                                    colFirst.Add(IndexHelper.GetColumnIndexInNumber(strArray2[0]));
                                                    rowLast.Add(IndexHelper.GetRowIndexInNumber(strArray2[1]));
                                                    colLast.Add(IndexHelper.GetColumnIndexInNumber(strArray2[1]));
                                                    selectionCount++;
                                                    break;
                                            }
                                        }
                                        if ((this.TryReadAttribure(element2, "activeCellId", out str) && int.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num8)) && (num8 < rowFirst.Count))//hdt ref 变 out
                                        {
                                            rowActive = rowFirst[num8];
                                            columnActive = colFirst[num8];
                                            flag6 = true;
                                        }
                                    }
                                    if (!flag6 && this.TryReadAttribure(element2, "activeCell", out str))
                                    {
                                        rowActive = IndexHelper.GetRowIndexInNumber(str);
                                        columnActive = IndexHelper.GetColumnIndexInNumber(str);
                                    }
                                    PaneType topLeft = PaneType.TopLeft;
                                    Enum.TryParse<PaneType>(element2.GetAttributeValueOrDefaultOfStringType("pane", "topLeft"), true, out topLeft);
                                    this._sheetActiveCells[sheet] = new Tuple<int, int>(rowActive, columnActive);
                                    this._excelReader.SetSelection(sheet, topLeft, rowActive, columnActive, selectionCount, rowFirst, rowLast, colFirst, colLast);
                                }
                                else if (localName == "pane")
                                {
                                    int horizontalPosition = (int) ((int) element2.GetAttributeValueOrDefault<int>("xSplit", 0));
                                    int verticalPosition = (int) ((int) element2.GetAttributeValueOrDefault<int>("ySplit", 0));
                                    string str4 = (string) ((string) element2.GetAttributeValueOrDefault<string>("topLeftCell", "A1"));
                                    string str5 = element2.GetAttributeValueOrDefaultOfStringType("state", null);
                                    bool isPanesFrozen = false;
                                    if ((str5 != null) && ((str5 == "frozen") || (str5 == "frozenSplit")))
                                    {
                                        isPanesFrozen = true;
                                    }
                                    PaneType result = PaneType.TopLeft;
                                    Enum.TryParse<PaneType>((string) ((string) element2.GetAttributeValueOrDefault<string>("activePane", "TopLeft")), true, out result);
                                    this._excelReader.SetPane(sheet, horizontalPosition, verticalPosition, IndexHelper.GetRowIndexInNumber(str4), IndexHelper.GetColumnIndexInNumber(str4), (int) result, isPanesFrozen);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Read a:fillStyleLst and a:bgFillStyleLst.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="sheetIndex"></param>
        /// <param name="results"></param>
        private void ReadSolidFill(XElement node, short sheetIndex, Dictionary<int, ExcelColor> results)
        {
            if (((node != null) && (results != null)) && node.HasElements)
            {
                int num = 0;
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "solidFill")
                    {
                        ExcelColor color = TryReadSoldFill(element);
                        if (color != null)
                        {
                            results[num] = color;
                        }
                    }
                    num++;
                }
            }
        }

        private ExcelSparkline ReadSparkline(XElement node)
        {
            ExcelSparkline sparkline = new ExcelSparkline();
            string text1 = node.TryGetChildElement("f").Value;
            ExternalCellRange externalCellRange = this.GetExternalCellRange(node.TryGetChildElement("f").Value);
            if (externalCellRange == null)
            {
                return null;
            }
            sparkline.DataRange = externalCellRange;
            sparkline.Location = this.GetRange(node.TryGetChildElement("sqref").Value);
            return sparkline;
        }

        private ExcelSparklineGroup ReadSparklineGroup(XElement node)
        {
            ExcelSparklineGroup group = new ExcelSparklineGroup {
                ManualMaxValue = node.GetAttributeValueOrDefaultOfDoubleType("manualMax", double.NaN),
                ManualMinValue = node.GetAttributeValueOrDefaultOfDoubleType("manualMin", double.NaN),
                LineWeight = node.GetAttributeValueOrDefaultOfDoubleType("lineWeight", 0.75)
            };
            ExcelSparklineType line = ExcelSparklineType.Line;
            Enum.TryParse<ExcelSparklineType>(node.GetAttributeValueOrDefaultOfStringType("type", "line"), true, out line);
            group.SparklineType = line;
            group.IsDateAxis = node.GetAttributeValueOrDefaultOfBooleanType("dateAxis", false);
            ExcelSparklineEmptyCellDisplayAs zero = ExcelSparklineEmptyCellDisplayAs.Zero;
            Enum.TryParse<ExcelSparklineEmptyCellDisplayAs>(node.GetAttributeValueOrDefaultOfStringType("displayEmptyCellsAs", "zero"), true, out zero);
            group.DisplayEmptyCellAs = zero;
            group.ShowMarkers = node.GetAttributeValueOrDefaultOfBooleanType("markers", false);
            group.ShowHighestDifferently = node.GetAttributeValueOrDefaultOfBooleanType("high", false);
            group.ShowLowestDifferently = node.GetAttributeValueOrDefaultOfBooleanType("low", false);
            group.ShowFirstDifferently = node.GetAttributeValueOrDefaultOfBooleanType("first", false);
            group.ShowLastDifferently = node.GetAttributeValueOrDefaultOfBooleanType("last", false);
            group.ShowNegativeDifferently = node.GetAttributeValueOrDefaultOfBooleanType("negative", false);
            group.ShowXAxis = node.GetAttributeValueOrDefaultOfBooleanType("displayXAxis", false);
            group.ShowHidden = node.GetAttributeValueOrDefaultOfBooleanType("displayHidden", false);
            group.RightToLeft = node.GetAttributeValueOrDefaultOfBooleanType("rightToLeft", false);
            ExcelSparklineAxisMinMax individual = ExcelSparklineAxisMinMax.Individual;
            Enum.TryParse<ExcelSparklineAxisMinMax>(node.GetAttributeValueOrDefaultOfStringType("minAxisType", "individual"), true, out individual);
            group.MinAxisType = individual;
            Enum.TryParse<ExcelSparklineAxisMinMax>(node.GetAttributeValueOrDefaultOfStringType("maxAxisType", "individual"), true, out individual);
            group.MaxAxisType = individual;
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "colorSeries")
                {
                    group.SeriesColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorNegative")
                {
                    group.NegativeColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorAxis")
                {
                    group.AxisColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorMarkers")
                {
                    group.MarkersColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorFirst")
                {
                    group.FirstColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorLast")
                {
                    group.LastColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorHigh")
                {
                    group.HighColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "colorLow")
                {
                    group.LowColor = TryReadColor(element);
                }
                else if (element.Name.LocalName == "f")
                {
                    if (this.GetExternalCellRange(element.Value) == null)
                    {
                        continue;
                    }
                    group.DateAxisRange = this.GetExternalCellRange(element.Value);
                }
                if (element.Name.LocalName == "sparklines")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        ExcelSparkline sparkline = this.ReadSparkline(element2);
                        if (sparkline != null)
                        {
                            group.Sparklines.Add(sparkline);
                        }
                    }
                }
            }
            return group;
        }

        private void ReadSparklineSettings(short sheet, XElement node)
        {
            if (this._excelReader is IExcelSparklineReader)
            {
                IExcelSparklineReader reader = this._excelReader as IExcelSparklineReader;
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "sparklineGroups")
                    {
                        List<IExcelSparklineGroup> excelSparklineGroups = new List<IExcelSparklineGroup>();
                        foreach (XElement element2 in element.Elements())
                        {
                            if (element2.Name.LocalName == "sparklineGroup")
                            {
                                excelSparklineGroups.Add(this.ReadSparklineGroup(element2));
                            }
                        }
                        reader.SetExcelSparklineGroups(sheet, excelSparklineGroups);
                    }
                }
            }
        }

        private byte[] ReadStreamFully(Stream input)
        {
            if (input != null)
            {
                input.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            }
            byte[] buffer = new byte[0x4000];
            using (MemoryStream stream = new MemoryStream())
            {
                int num;
                while ((num = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, num);
                }
                return stream.ToArray();
            }
        }

        private void ReadStringTables(XFile workbookFile, MemoryFolder mFolder)
        {
            try
            {
                XFile fileByType = workbookFile.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings");
                this.ReadFile(fileByType, mFolder, -1);
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("sharedStringError"), ExcelWarningCode.General, exception);
            }
        }

        private void ReadStyles(XFile workbookFile, MemoryFolder mFolder)
        {
            XFile fileByType = workbookFile.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
            this.ReadFile(fileByType, mFolder, -1);
            if ((this._styleCellXfs != null) && (this._styleCellXfs.Count > 0))
            {
                try
                {
                    this._excelReader.SetExcelDefaultCellFormat(this._styleCellXfs[0]);
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, exception);
                }
            }
            foreach (ExtendedFormat format in this._styleCellStyleXfs)
            {
                try
                {
                    this._excelReader.SetExcelCellFormat(format);
                }
                catch (Exception exception2)
                {
                    this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, exception2);
                }
            }
            for (int i = 0; i < this._styleCellXfs.Count; i++)
            {
                try
                {
                    this._excelReader.SetExcelCellFormat(this._styleCellXfs[i]);
                }
                catch (Exception exception3)
                {
                    this.LogError(ResourceHelper.GetResourceString("readStyleError"), ExcelWarningCode.General, exception3);
                }
            }
            if ((this._dxfRecords != null) && (this._dxfRecords.Count > 0))
            {
                try
                {
                    this._excelReader.SetDifferentialFormattingRecord(this._dxfRecords);
                }
                catch (Exception exception4)
                {
                    this.LogError(ResourceHelper.GetResourceString("biffReadDxfError"), ExcelWarningCode.General, exception4);
                }
            }
            if (this._excelReader is IExcelTableReader)
            {
                IExcelTableReader reader = this._excelReader as IExcelTableReader;
                if ((this._tableStyles != null) && (this._tableStyles.Count > 0))
                {
                    foreach (IExcelTableStyle style in this._tableStyles)
                    {
                        try
                        {
                            reader.SetTableStyle(style);
                        }
                        catch (Exception exception5)
                        {
                            this.LogError(ResourceHelper.GetResourceString("biffReadTableStyleError"), ExcelWarningCode.General, exception5);
                        }
                    }
                }
            }
        }

        private IExcelTableColumn ReadTableColumn(XElement node)
        {
            int num = node.GetAttributeValueOrDefaultOfInt32Type("id", 0);
            string str = node.GetAttributeValueOrDefaultOfStringType("name", null);
            string str2 = node.GetAttributeValueOrDefaultOfStringType("totalsRowLabel", null);
            ExcelTableColumn column = new ExcelTableColumn {
                Id = num,
                Name = str,
                TotalsRowLabel = str2
            };
            ExcelTableTotalsRowFunction none = ExcelTableTotalsRowFunction.None;
            Enum.TryParse<ExcelTableTotalsRowFunction>(node.GetAttributeValueOrDefaultOfStringType("totalsRowFunction", "none"), true, out none);
            column.TotalsRowFunction = none;
            if (column.TotalsRowFunction == ExcelTableTotalsRowFunction.Custom)
            {
                column.TotalsRowCustomFunction = node.GetChildElementValue("totalsRowFormula");
                if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
                {
                    column.TotalsRowCustomFunction = this.ConvertA1FormulaToR1C1Formula(column.TotalsRowCustomFunction, 0, 0);
                }
                column.TotalsRowFunctionIsArrayFormula = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("totalsRowFormula", "array"));
            }
            column.CalculatedColumnFormula = node.GetChildElementValue("calculatedColumnFormula");
            if (this._calcProperty.RefMode == ExcelReferenceStyle.R1C1)
            {
                column.CalculatedColumnFormula = this.ConvertA1FormulaToR1C1Formula(column.CalculatedColumnFormula, 0, 0);
            }
            if (column.CalculatedColumnFormula != null)
            {
                column.CalculatedColumnFormulaIsArrayFormula = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("calculatedColumnFormula", "array"));
            }
            return column;
        }

        private void ReadTableFile(XFile file, MemoryFolder mFolder, int sheetIndex)
        {
            if ((file != null) && !string.IsNullOrWhiteSpace(file.FileName))
            {
                Stream stream = mFolder.GetFile(file.FileName);
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                this.ReadSheetTable(XDocument.Load(stream).Root, (short) sheetIndex);
            }
        }

        private IExcelTableStyle ReadTableStyle(XElement child)
        {
            ExcelTableStyle style = new ExcelTableStyle {
                Name = child.GetAttributeValueOrDefaultOfStringType("name", null),
                IsPivotStyle = child.GetAttributeValueOrDefaultOfBooleanType("pivot", true),
                IsTableStyle = child.GetAttributeValueOrDefaultOfBooleanType("table", true)
            };
            if (child.GetAttributeValueOrDefaultOfInt32Type("count", 0) > 0)
            {
                foreach (XElement element in child.Elements())
                {
                    if (element.Name.LocalName == "tableStyleElement")
                    {
                        style.TableStyleElements.Add(this.ReadTableStyleElement(element));
                    }
                }
            }
            return style;
        }

        private IExcelTableStyleElement ReadTableStyleElement(XElement element)
        {
            TableStyleElement element2 = new TableStyleElement();
            ExcelTableElementType wholeTable = ExcelTableElementType.WholeTable;
            Enum.TryParse<ExcelTableElementType>(element.GetAttributeValueOrDefaultOfStringType("type", "wholeTable"), true, out wholeTable);
            element2.Type = wholeTable;
            element2.DifferentFormattingIndex = element.GetAttributeValueOrDefaultOfInt32Type("dxfId", 0);
            element2.Size = element.GetAttributeValueOrDefaultOfInt32Type("size", 1);
            return element2;
        }

        private IExcelTableStyleInfo ReadTableStyleInfo(XElement node)
        {
            return new ExcelTableStyleInfo { Name = node.GetAttributeValueOrDefaultOfStringType("name", null), ShowFirstColumn = node.GetAttributeValueOrDefaultOfBooleanType("showFirstColumn", false), ShowLastColumn = node.GetAttributeValueOrDefaultOfBooleanType("showLastColumn", false), ShowRowStripes = node.GetAttributeValueOrDefaultOfBooleanType("showRowStripes", false), ShowColumnStripes = node.GetAttributeValueOrDefaultOfBooleanType("showColumnStripes", false) };
        }

        private void ReadTableStyles(XElement node, short sheet)
        {
            if (this._excelReader is IExcelTableReader)
            {
                IExcelTableReader reader = this._excelReader as IExcelTableReader;
                if (node != null)
                {
                    string defaultTableStyleName = node.GetAttributeValueOrDefaultOfStringType("defaultTableStyle", null);
                    string defaultPivotTableStyleName = node.GetAttributeValueOrDefaultOfStringType("defaultPivotStyle", null);
                    reader.SetTableDefaultStyle(defaultTableStyleName, defaultPivotTableStyleName);
                    if (node.GetAttributeValueOrDefaultOfInt32Type("count", 0) > 0)
                    {
                        foreach (XElement element in node.Elements())
                        {
                            if (element.Name.LocalName == "tableStyle")
                            {
                                IExcelTableStyle style = this.ReadTableStyle(element);
                                this._tableStyles.Add(style);
                            }
                        }
                    }
                }
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
                    ColorScheme colorScheme = ReadColorScheme(element.TryGetChildElement("clrScheme"));
                    FontScheme fontScheme = ReadFontScheme(element.TryGetChildElement("fontScheme"));
                    ExcelTheme theme = new ExcelTheme(name, colorScheme, fontScheme);
                    this._excelReader.SetTheme(theme);
                }
            }
        }

        private void ReadTheme(XFile workbookFile, MemoryFolder mFolder)
        {
            try
            {
                XFile fileByType = workbookFile.GetFileByType("http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
                if ((fileByType != null) && !string.IsNullOrWhiteSpace(fileByType.FileName))
                {
                    Stream file = mFolder.GetFile(fileByType.FileName);
                    file.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    this.ReadTheme(XDocument.Load(file).Root);
                }
            }
            catch (Exception exception)
            {
                this.LogError(ResourceHelper.GetResourceString("readThemeError"), ExcelWarningCode.General, exception);
            }
        }

        private static IThemeFonts ReadThemeFonts(XElement node)
        {
            List<IRunFormatting> runFormats = new List<IRunFormatting>();
            List<IThemeFont> themesFonts = new List<IThemeFont>();
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "latin")
                {
                    string typeface = element.GetAttributeValueOrDefaultOfStringType("typeface", "");
                    runFormats.Add(new RunFormating(FontLanguage.LatinFont, typeface));
                }
                else if (element.Name.LocalName == "ea")
                {
                    string str2 = element.GetAttributeValueOrDefaultOfStringType("typeface", "");
                    runFormats.Add(new RunFormating(FontLanguage.EastAsianFont, str2));
                }
                else if (element.Name.LocalName == "cs")
                {
                    string str3 = element.GetAttributeValueOrDefaultOfStringType("typeface", "");
                    runFormats.Add(new RunFormating(FontLanguage.ComplexScriptFont, str3));
                }
                else if (element.Name.LocalName == "sym")
                {
                    string str4 = element.GetAttributeValueOrDefaultOfStringType("typeface", "");
                    runFormats.Add(new RunFormating(FontLanguage.SymbolFont, str4));
                }
                else if (element.Name.LocalName == "font")
                {
                    string script = element.GetAttributeValueOrDefaultOfStringType("script", null);
                    string str6 = element.GetAttributeValueOrDefaultOfStringType("typeface", null);
                    themesFonts.Add(new ThemeFont(script, str6));
                }
            }
            return new ThemeFonts(runFormats, themesFonts);
        }

        private void ReadVBA(int sheet, XFile workbookFile, MemoryFolder mFolder)
        {
            XFile fileByType = workbookFile.GetFileByType("http://schemas.microsoft.com/office/2006/relationships/vbaProject");
            if ((fileByType != null) && (this._excelReader is IExcelLosslessReader))
            {
                Stream file = mFolder.GetFile(fileByType.FileName);
                UnsupportRecord unsupportRecord = new UnsupportRecord {
                    Category = RecordCategory.VBA,
                    FileType = ExcelFileType.XLSX,
                    Value = file
                };
                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(-1, unsupportRecord);
                ExcelWarning excelWarning = new ExcelWarning(ResourceHelper.GetResourceString("UnsupportedRecords"), ExcelWarningCode.UnsupportedMacros, sheet, -1, -1);
                this._excelReader.OnExcelLoadError(excelWarning);
            }
        }

        private void ReadWorkbook(XFile workbookFile, MemoryFolder mFolder, int excelSheetIndex)
        {
            this.ReadFile(workbookFile, mFolder, excelSheetIndex);
        }

        private void ReadWorkbookProperties(XElement node, short sheet)
        {
            if (node != null)
            {
                ExcelWorkbookPropery workbookPropety = new ExcelWorkbookPropery {
                    SaveExternalLinks = node.GetAttributeValueOrDefaultOfBooleanType("saveExternalLinkValues", true)
                };
                if (node.GetAttributeValueOrDefaultOfBooleanType("dateCompatibility", true))
                {
                    workbookPropety.IsDate1904 = node.GetAttributeValueOrDefaultOfBooleanType("date1904", false);
                }
                this._excelReader.SetExcelWorkbookProperty(workbookPropety);
            }
        }

        private void ReadWorkSheetFile(XFile file, MemoryFolder mFolder, int sheetIndex)
        {
            if ((file != null) && !string.IsNullOrWhiteSpace(file.FileName))
            {
                if (file.RelationFiles != null)
                {
                    this._hyperLinksRelations.Clear();
                    foreach (KeyValuePair<string, XFile> pair in file.RelationFiles)
                    {
                        if (pair.Value.FileType.ToUpperInvariant().EndsWith("HYPERLINK"))
                        {
                            string fileName = pair.Value.FileName;
                            int index = fileName.ToUpperInvariant().IndexOf("HTTP");
                            if (index != -1)
                            {
                                this._hyperLinksRelations.Add(pair.Key, fileName.Substring(index));
                            }
                            else
                            {
                                this._hyperLinksRelations.Add(pair.Key, Path.GetFileName(fileName));
                            }
                        }
                    }
                }
                using (XmlReader reader = XmlReader.Create(mFolder.GetFile(file.FileName)))
                {
                    while (reader.Read())
                    {
                        if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && (reader.Depth == 1))
                        {
                            if (reader.LocalName == "sheetData")
                            {
                                this.ReadSheetData(reader, (short) sheetIndex);
                            }
                            else
                            {
                                XlsxReadHandler handler;
                                string localName = reader.LocalName;
                                if (this._xlsxHandlerMap.TryGetValue(localName, out handler) && (handler != null))//hdt ref 变 out
                                {
                                    try
                                    {
                                        handler(XElement.Load(reader.ReadSubtree()), (short) sheetIndex);
                                        continue;
                                    }
                                    catch (Exception exception)
                                    {
                                        if (sheetIndex == -1)
                                        {
                                            this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { localName }), ExcelWarningCode.General, exception);
                                        }
                                        else
                                        {
                                            this.LogError(string.Format(ResourceHelper.GetResourceString("generalRecordError"), (object[]) new object[] { localName }), ExcelWarningCode.General, sheetIndex, -1, -1, exception);
                                        }
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
                if (file.RelationFiles != null)
                {
                    foreach (KeyValuePair<string, XFile> pair2 in file.RelationFiles)
                    {
                        if (pair2.Value.FileType.ToUpperInvariant().EndsWith("COMMENTS"))
                        {
                            Stream stream2 = mFolder.GetFile(pair2.Value.FileName);
                            stream2.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                            this.ReadNode(XDocument.Load(stream2).Root, (short) sheetIndex);
                        }
                    }
                }
            }
        }

        private void ReadWorksheets(XFile workbookFile, MemoryFolder mFolder, int excelSheetIndex)
        {
            if (excelSheetIndex == -1)
            {
                foreach (SheetInfo info in this._sheetIDs)
                {
                    this._excel2010ExtensionDataBarRules.Clear();
                    if (this._rowCounter.HasValue)
                    {
                        this._rowCounter = null;
                    }
                    this._excelPrintPageSetting = new ExcelPrintPageSetting();
                    this.identifier = 0;
                    XFile fileByRelationID = workbookFile.GetFileByRelationID(info.rID);
                    this.ReadWorkSheetFile(fileByRelationID, mFolder, info.index);
                    this._excelReader.SetPrintPageSetting((short) info.index, this._excelPrintPageSetting);
                    this._excelPrintPageSetting = new ExcelPrintPageSetting();
                    this._sharedFormula.Clear();
                    if (((fileByRelationID != null) && (fileByRelationID.RelationFiles != null)) && (fileByRelationID.RelationFiles.Count > 0))
                    {
                        List<XFile> list = new List<XFile>();
                        foreach (KeyValuePair<string, XFile> pair in fileByRelationID.RelationFiles)
                        {
                            XFile file2 = fileByRelationID.GetFileByRelationID(pair.Key);
                            if (file2.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table")
                            {
                                list.Add(file2);
                            }
                        }
                        foreach (XFile file3 in list)
                        {
                            this.ReadTableFile(file3, mFolder, info.index);
                        }
                    }
                    if (((this._excelReader is IExcelChartReader) && (fileByRelationID != null)) && ((fileByRelationID.RelationFiles != null) && (fileByRelationID.RelationFiles.Count > 0)))
                    {
                        List<XFile> list2 = new List<XFile>();
                        foreach (KeyValuePair<string, XFile> pair2 in fileByRelationID.RelationFiles)
                        {
                            XFile file4 = fileByRelationID.GetFileByRelationID(pair2.Key);
                            if (file4.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing")
                            {
                                list2.Add(file4);
                            }
                        }
                        foreach (XFile file5 in list2)
                        {
                            this.ReadDrawingFile(file5, mFolder, info.index);
                        }
                    }
                    if (((this._excelReader is IExcelLosslessReader) && (fileByRelationID != null)) && ((fileByRelationID.RelationFiles != null) && (fileByRelationID.RelationFiles.Count > 0)))
                    {
                        Dictionary<string, Stream> dictionary = new Dictionary<string, Stream>();
                        foreach (KeyValuePair<string, XFile> pair3 in fileByRelationID.RelationFiles)
                        {
                            XFile file = fileByRelationID.GetFileByRelationID(pair3.Key);
                            if (file.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/ctrlProp")
                            {
                                dictionary[file.FileName] = mFolder.GetFile(file.FileName);
                                this.LogUnsupportedXmlContents(excelSheetIndex, mFolder, file);
                                UnsupportRecord unsupportRecord = new UnsupportRecord {
                                    Category = RecordCategory.SheetFileRelationShip,
                                    FileType = ExcelFileType.XLSX,
                                    Value = pair3.Value.Relationship
                                };
                                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(info.index, unsupportRecord);
                            }
                            if (file.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/pivotTable")
                            {
                                this.LogUnsupportedXmlContents(excelSheetIndex, mFolder, file);
                            }
                            if (file.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing")
                            {
                                Stream stream = mFolder.GetFile(file.FileName);
                                dictionary[file.FileName] = stream;
                                this.LogUnsupportedXmlContents(excelSheetIndex, mFolder, file);
                                UnsupportRecord record4 = new UnsupportRecord {
                                    Category = RecordCategory.SheetFileRelationShip,
                                    FileType = ExcelFileType.XLSX,
                                    Value = pair3.Value.Relationship
                                };
                                (this._excelReader as IExcelLosslessReader).AddUnsupportItem(info.index, record4);
                                if ((file.RelationFiles != null) && (file.RelationFiles.Count > 0))
                                {
                                    Dictionary<string, Stream> dictionary2 = new Dictionary<string, Stream>();
                                    foreach (KeyValuePair<string, XFile> pair4 in file.RelationFiles)
                                    {
                                        XFile file7 = file.GetFileByRelationID(pair4.Key);
                                        dictionary2[file7.FileName] = mFolder.GetFile(file7.FileName);
                                        this.LogUnsupportedXmlContents(excelSheetIndex, mFolder, file7);
                                        UnsupportRecord record2 = new UnsupportRecord {
                                            Category = RecordCategory.VmlMediaFileRelationShip,
                                            FileType = ExcelFileType.XLSX,
                                            Value = new Tuple<string, Relationship>(file.FileName, pair4.Value.Relationship)
                                        };
                                        (this._excelReader as IExcelLosslessReader).AddUnsupportItem(info.index, record2);
                                    }
                                    if (dictionary2.Count > 0)
                                    {
                                        UnsupportRecord record3 = new UnsupportRecord {
                                            Category = RecordCategory.VmlMediaFile,
                                            FileType = ExcelFileType.XLSX,
                                            Value = new Tuple<string, Dictionary<string, Stream>>(file.FileName, dictionary2)
                                        };
                                        (this._excelReader as IExcelLosslessReader).AddUnsupportItem(info.index, record3);
                                    }
                                }
                            }
                        }
                        if (dictionary.Count > 0)
                        {
                            UnsupportRecord record5 = new UnsupportRecord {
                                Category = RecordCategory.SheetFileRelationFile,
                                FileType = ExcelFileType.XLSX,
                                Value = dictionary
                            };
                            (this._excelReader as IExcelLosslessReader).AddUnsupportItem(info.index, record5);
                        }
                    }
                }
            }
            else if ((this._sheetIDs.Count == 1) && (excelSheetIndex >= 0))
            {
                this._excel2010ExtensionDataBarRules.Clear();
                excelSheetIndex = 0;
                if (this._rowCounter.HasValue)
                {
                    this._rowCounter = null;
                }
                this._excelPrintPageSetting = new ExcelPrintPageSetting();
                SheetInfo info2 = this._sheetIDs[excelSheetIndex];
                XFile file8 = workbookFile.GetFileByRelationID(info2.rID);
                this.ReadFile(file8, mFolder, (short) excelSheetIndex);
                this._excelReader.SetPrintPageSetting((short) excelSheetIndex, this._excelPrintPageSetting);
                this._excelPrintPageSetting = new ExcelPrintPageSetting();
            }
            else
            {
                this.LogError(ResourceHelper.GetResourceString("sheetIndexOutOfRange"), ExcelWarningCode.CannotOpen, null);
            }
        }

        internal static ExtendedFormat ReadXF(XElement node, List<ExcelFont> fonts, List<Tuple<FillPatternType, ExcelColor, ExcelColor>> fills, List<ExcelBorder> borders, bool isStyleXf = false)
        {
            ExtendedFormat format = null;
            ExcelVerticalAlignment alignment;
            ExcelHorizontalAlignment alignment2;
            if ((node == null) || !node.HasAttributes)
            {
                return null;
            }
            format = new ExtendedFormat {
                IsStyleFormat = isStyleXf
            };
            int num = node.GetAttributeValueOrDefaultOfUInt16Type("numFmtId", 0);
            ushort num2 = node.GetAttributeValueOrDefaultOfUInt16Type("fontId", 0);
            ushort num3 = node.GetAttributeValueOrDefaultOfUInt16Type("fillId", 0);
            ushort num4 = node.GetAttributeValueOrDefaultOfUInt16Type("borderId", 0);
            int num5 = node.GetAttributeValueOrDefaultOfInt32Type("applyAlignment", -1);
            switch (num5)
            {
                case 0:
                case 1:
                    format.ApplyAlignment = new bool?(num5 == 1);
                    break;
            }
            int num6 = node.GetAttributeValueOrDefaultOfInt32Type("applyBorder", -1);
            if ((num6 == 0) || (num6 == 1))
            {
                format.ApplyBorder = new bool?(num6 == 1);
            }
            int num7 = node.GetAttributeValueOrDefaultOfInt32Type("applyFill", -1);
            if ((num7 == 0) || (num7 == 1))
            {
                format.ApplyFill = new bool?(num7 == 1);
            }
            int num8 = node.GetAttributeValueOrDefaultOfInt32Type("applyFont", -1);
            if ((num8 == 0) || (num8 == 1))
            {
                format.ApplyFont = new bool?(num8 == 1);
            }
            int num9 = node.GetAttributeValueOrDefaultOfInt32Type("applyProtection", -1);
            if ((num9 == 0) || (num9 == 1))
            {
                format.ApplyProtection = new bool?(num9 == 1);
            }
            int num10 = node.GetAttributeValueOrDefaultOfInt32Type("applyNumberFormat", -1);
            if ((num10 == 0) || (num10 == 1))
            {
                format.ApplyNumberFormat = new bool?(num10 == 1);
            }
            int num11 = node.GetAttributeValueOrDefaultOfInt32Type("xfId", -1);
            if (num11 >= 0)
            {
                format.ParentFormatID = new int?(num11);
            }
            format.Font = fonts[num2];
            format.Border = borders[num4];
            if (_numberFormats.ContainsKey(num))
            {
                format.NumberFormat = _numberFormats[num];
            }
            else
            {
                format.NumberFormatIndex = num;
            }
            Tuple<FillPatternType, ExcelColor, ExcelColor> tuple = fills[num3];
            format.FillPattern = tuple.Item1;
            format.PatternColor = tuple.Item2;
            format.PatternBackgroundColor = tuple.Item3;
            format.IsLocked = true;
            format.IsHidden = false;
            format.HorizontalAlign = ExcelHorizontalAlignment.General;
            format.VerticalAlign = ExcelVerticalAlignment.Bottom;
            string str = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("alignment", "horizontal"));
            if (string.IsNullOrWhiteSpace(str))
            {
                str = "general";
            }
            string str2 = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("alignment", "vertical"));
            if (string.IsNullOrWhiteSpace(str2))
            {
                str2 = "bottom";
            }
            string str3 = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("alignment", "textRotation"));
            if (!string.IsNullOrWhiteSpace(str3))
            {
                format.Rotation = int.Parse(str3, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            string str4 = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("alignment", "readingOrder"));
            if (!string.IsNullOrWhiteSpace(str4))
            {
                format.ReadingOrder = (TextDirection)((byte) int.Parse(str4, (IFormatProvider) CultureInfo.InvariantCulture));
            }
            string str5 = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("alignment", "indent"));
            if (!string.IsNullOrWhiteSpace(str5))
            {
                format.Indent = byte.Parse(str5, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            format.IsJustfyLastLine = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("alignment", "justifyLastLine"));
            format.IsShrinkToFit = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("alignment", "shrinkToFit"));
            format.IsWordWrap = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("alignment", "wrapText"));
            Enum.TryParse<ExcelVerticalAlignment>(str2, true, out alignment);
            format.VerticalAlign = alignment;
            Enum.TryParse<ExcelHorizontalAlignment>(str, true, out alignment2);
            format.HorizontalAlign = alignment2;
            format.IsHidden = (bool) ((bool) node.GetChildElementAttributeValueOrDefault<bool>("protection", "hidden"));
            string str6 = (string) ((string) node.GetChildElementAttributeValueOrDefault<string>("protection", "locked"));
            if (!string.IsNullOrEmpty(str6) && ((str6 == "0") || (str6 == "false")))
            {
                format.IsLocked = false;
            }
            return format;
        }

        private void ReadXFs(XElement node, short sheetIndex, List<ExtendedFormat> result, bool isStyleXf = false)
        {
            if ((node != null) && node.HasElements)
            {
                using (IEnumerator<XElement> enumerator = node.Elements().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        ExtendedFormat format = ReadXF(enumerator.Current, _styleFonts, _styleFills, _styleBorders, isStyleXf);
                        result.Add(format);
                    }
                }
            }
        }

        private void SetLocalDefinedName()
        {
            foreach (NamedCellRange range in _definedNames)
            {
                try
                {
                    bool flag = false;
                    if (range.RefersToR1C1 != null)
                    {
                        flag = true;
                    }
                    string str = flag ? range.RefersToR1C1 : range.RefersTo;
                    if (!string.IsNullOrWhiteSpace(str) && (this._externalRefs.Count != 0))
                    {
                        int index = str.IndexOf('!');
                        if (index != -1)
                        {
                            int startIndex = str.IndexOf('[');
                            int num3 = str.IndexOf(']');
                            if ((startIndex < num3) && (num3 < index))
                            {
                                string str2 = str.Substring(startIndex + 1, (num3 - startIndex) - 1);
                                int num4 = 0;
                                if (int.TryParse(str2, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num4) && this._externalRefs.ContainsKey(num4))//hdt ref 变 out
                                {
                                    if ((num3 + 1) == index)
                                    {
                                        str = str.Replace(str.Substring(startIndex, (num3 - startIndex) + 1), "'" + this._externalRefs[num4].Item2 + "'");
                                    }
                                    else
                                    {
                                        str = str.Replace(str.Substring(startIndex, (num3 - startIndex) + 1), "[" + this._externalRefs[num4].Item2 + "]");
                                    }
                                }
                                index = str.IndexOf('!');
                                if (((index > 0) && (index != (str.Length - 1))) && (str[index - 1] != '\''))
                                {
                                    str = str.Insert(index, "'");
                                    int num5 = str.LastIndexOf('(', index);
                                    str = str.Insert((num5 >= 0) ? (num5 + 1) : 0, "'");
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        range.RefersToR1C1 = str;
                    }
                    else
                    {
                        range.RefersTo = str;
                    }
                    this._excelReader.SetNamedCellRange(range);
                }
                catch (Exception exception)
                {
                    this.LogError(string.Format(ResourceHelper.GetResourceString("definedNameGeneralError"), (object[]) new object[] { range.Name }), ExcelWarningCode.DefinedOrCustomNameError, exception);
                }
            }
        }

        private Tuple<string, int> SpitRowColumn(string text)
        {
            int startIndex = 0;
            while (char.IsLetter(text[startIndex]))
            {
                startIndex++;
            }
            string str = text.Substring(startIndex);
            int num2 = 0;
            if (!int.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num2))//hdt ref 变 out
            {
                num2 = 0x7fff;
            }
            return new Tuple<string, int>(text.Substring(0, startIndex), num2);
        }

        /// <summary>
        /// Read attribute which indicate by XName.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="attrName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool TryReadAttribure(XElement node, XName attrName, out string value)
        {
            value = null;
            if ((node == null) || (attrName == null))
            {
                return false;
            }
            XAttribute attribute = node.Attribute(attrName);
            if (attribute == null)
            {
                return false;
            }
            value = attribute.Value;
            return true;
        }

        /// <summary>
        /// Read a:ln
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="side">The side.</param>
        /// <returns></returns>
        private bool TryReadBorderSide(XElement node, out ExcelBorderSide side)
        {
            side = null;
            if (node == null)
            {
                return false;
            }
            node.GetAttributeValueOrDefaultOfStringType("algn", null);
            node.GetAttributeValueOrDefaultOfStringType("cap", null);
            node.GetAttributeValueOrDefaultOfStringType("cmpd", null);
            node.GetAttributeValueOrDefaultOfStringType("w", null);
            side = new ExcelBorderSide();
            foreach (XElement element in node.Elements())
            {
                string str;
                string localName = element.Name.LocalName;
                if (localName != null)
                {
                    if (localName == "solidFill")
                    {
                        ExcelColor color = TryReadSoldFill(element);
                        if (color != null)
                        {
                            side.Color = color;
                        }
                    }
                    else if (localName == "prstDash")
                    {
                        goto Label_00A9;
                    }
                }
                continue;
            Label_00A9:
                if (this.TryReadAttribure(element, "val", out str))
                {
                    switch (str)
                    {
                        case "solid":
                            side.LineStyle = ExcelBorderStyle.Thin;
                            break;

                        case "dot":
                            side.LineStyle = ExcelBorderStyle.Dotted;
                            break;

                        case "dash":
                            side.LineStyle = ExcelBorderStyle.Dashed;
                            break;

                        case "dashDot":
                            side.LineStyle = ExcelBorderStyle.DashDot;
                            break;
                    }
                }
            }
            return true;
        }

        private static ExcelColor TryReadColor(XElement node)
        {
            ExcelColor emptyColor = ExcelColor.EmptyColor;
            if ((node != null) && node.HasAttributes)
            {
                uint num3;
                if (node.GetAttributeValueOrDefaultOfBooleanType("auto", false))
                {
                    emptyColor.IsAutoColor = true;
                    return emptyColor;
                }
                int num = node.GetAttributeValueOrDefaultOfInt32Type("theme", -2147483648);
                double tint = node.GetAttributeValueOrDefaultOfDoubleType("tint", 0.0);
                if (num != -2147483648)
                {
                    return new ExcelColor(ExcelColorType.Theme, (uint) num, tint);
                }
                string str = node.GetAttributeValueOrDefaultOfStringType("rgb", null);
                if (!string.IsNullOrEmpty(str) && uint.TryParse(str, (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out num3))//hdt ref 变 out
                {
                    return new ExcelColor(ExcelColorType.RGB, num3, tint);
                }
                int num4 = node.GetAttributeValueOrDefaultOfInt32Type("indexed", -2147483648);
                if (num4 >= 0)
                {
                    return new ExcelColor((ExcelPaletteColor) num4);
                }
            }
            return emptyColor;
        }

        private static ExcelColor TryReadSchemeColor(XElement node)
        {
            if (node != null)
            {
                string str = node.GetAttributeValueOrDefaultOfStringType("val", null);
                if (!string.IsNullOrEmpty(str))
                {
                    return new ExcelColor(ExcelColorType.Theme, (uint) str.ToColorSchmeIndex(), 0.0);
                }
            }
            return null;
        }

        private static ExcelColor TryReadSoldFill(XElement node)
        {
            if ((node != null) && node.HasElements)
            {
                foreach (XElement element in node.Elements())
                {
                    if (element.Name.LocalName == "schemeClr")
                    {
                        return TryReadSchemeColor(element);
                    }
                    return TryReadThemeColor(element);
                }
            }
            return null;
        }

        private static ExcelColor TryReadThemeColor(XElement node)
        {
            ExcelColor color = null;
            XElement element = null;
            if ((node != null) && node.HasElements)
            {
                using (IEnumerator<XElement> enumerator = node.Elements().GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        element = enumerator.Current;
                        goto Label_0041;
                    }
                }
            }
        Label_0041:
            if (element == null)
            {
                return null;
            }
            if (!element.HasAttributes)
            {
                return null;
            }
            switch (element.Name.LocalName)
            {
                case "sysClr":
                {
                    GcColor color2 = new GcColor();
                    bool flag = ExcelSystemColor.TryGetSystemColor(element.GetAttributeValueOrDefaultOfStringType("val", null), out color2);
                    if (flag)
                    {
                        color = new ExcelColor(ExcelColorType.RGB, color2.ToArgb(), 0.0);
                    }
                    if (!flag)
                    {
                        string str = element.GetAttributeValueOrDefaultOfStringType("lastClr", null);
                        if (!string.IsNullOrEmpty(str))
                        {
                            uint num = 0;
                            if (uint.TryParse(str, (NumberStyles)NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture, out num))//hdt ref 变 out
                            {
                                num = 0xff000000 | num;
                                color = new ExcelColor(ExcelColorType.RGB, num, 0.0);
                            }
                        }
                    }
                    return color;
                }
                case "srgbClr":
                {
                    string str2 = element.GetAttributeValueOrDefaultOfStringType("val", null);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        uint num2 = 0;
                        if (uint.TryParse(str2, (NumberStyles)NumberStyles.HexNumber, (IFormatProvider)CultureInfo.InvariantCulture, out num2))//hdt ref 变 out
                        {
                            num2 = 0xff000000 | num2;
                            color = new ExcelColor(ExcelColorType.RGB, num2, 0.0);
                        }
                    }
                    return color;
                }
                case "scrgbClr":
                {
                    float val = element.GetAttributeValueOrDefaultOfFloatType("r", 0f);
                    float num4 = element.GetAttributeValueOrDefaultOfFloatType("g", 0f);
                    float num5 = element.GetAttributeValueOrDefaultOfFloatType("b", 0f);
                    return new ExcelColor(ExcelColorType.RGB, GcColor.FromArgb(0xff, ColorExtension.ScRgbTosRgb(val), ColorExtension.ScRgbTosRgb(num4), ColorExtension.ScRgbTosRgb(num5)).ToArgb(), 0.0);
                }
                case "hslClr":
                {
                    int num6 = element.GetAttributeValueOrDefaultOfInt32Type("hue", 0);
                    int num7 = element.GetAttributeValueOrDefaultOfInt32Type("sat", 0);
                    int num8 = element.GetAttributeValueOrDefaultOfInt32Type("lum", 0);
                    return new ExcelColor(ExcelColorType.RGB, ColorExtension.ConvertHLSToRGB((double) num6, (double) num8, (double) num7).ToArgb(), 0.0);
                }
                case "prstClr":
                {
                    string str3 = element.GetAttributeValueOrDefaultOfStringType("val", null);
                    if (!string.IsNullOrWhiteSpace(str3))
                    {
                        color = new ExcelColor(ExcelColorType.RGB, ColorExtension.FromPresetColorVal(str3).ToArgb(), 0.0);
                    }
                    return color;
                }
                case "schemeClr":
                {
                    string str4 = element.GetAttributeValueOrDefaultOfStringType("val", null);
                    if (!string.IsNullOrWhiteSpace(str4))
                    {
                        color = new ExcelColor(ExcelColorType.Theme, (uint) str4.ToColorSchmeIndex(), 0.0);
                    }
                    return color;
                }
            }
            return color;
        }

        private string UpdateFormulaIfContainsExternalReference(string formula)
        {
            int index = formula.IndexOf('!');
            if (index != -1)
            {
                int startIndex = formula.IndexOf('[');
                int num3 = formula.IndexOf(']');
                if ((startIndex >= num3) || (num3 >= index))
                {
                    return formula;
                }
                string str = formula.Substring(startIndex + 1, (num3 - startIndex) - 1);
                int num4 = 0;
                if (int.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture.NumberFormat, out num4) && this._externalRefs.ContainsKey(num4))//hdt ref 变 out
                {
                    if ((num3 + 1) == index)
                    {
                        formula = formula.Replace(formula.Substring(startIndex, (num3 - startIndex) + 1), "'" + this._externalRefs[num4].Item2 + "'");
                    }
                    else
                    {
                        formula = formula.Replace(formula.Substring(startIndex, (num3 - startIndex) + 1), "[" + this._externalRefs[num4].Item2 + "]");
                    }
                }
                index = formula.IndexOf('!');
                if ((index <= 0) || (formula[index - 1] == '\''))
                {
                    return formula;
                }
                formula = formula.Insert(index, "'");
                if (startIndex == -1)
                {
                    startIndex = formula.LastIndexOf('\'', index - 1);
                }
                if (startIndex == -1)
                {
                    startIndex = formula.LastIndexOf('(', index - 1);
                }
                if (startIndex == -1)
                {
                    startIndex = 0;
                }
                formula = formula.Insert((startIndex >= 0) ? startIndex : 0, "'");
            }
            return formula;
        }

        internal delegate void XlsxReadHandler(XElement node, short sheetIndex);

        private static class XNameHelper
        {
            internal static XName collapsedName = XName.Get("collapsed");
            internal static XName hiddenName = XName.Get("hidden");
            internal static XName htName = XName.Get("ht");
            internal static XName outlineLevelName = XName.Get("outlineLevel");
            internal static XName refName = XName.Get("ref");
            internal static XName rName = XName.Get("r");
            internal static XName siName = XName.Get("si");
            internal static XName sName = XName.Get("s");
            internal static XName tName = XName.Get("t");
        }

        internal class XNames
        {
            private static XName _auto;
            private static XName _color;
            private static XName _creator;
            private static XName _id;
            private static readonly object _initSyncRoot = new object();
            private static XName _majorFont;
            private static XName _minorFont;
            private static XName _name;
            private static XName _patternFill;
            private static XName _sheetId;
            private static XName _state;
            private static XName _style;
            private static XName _val;

            private static XName Ensure(ref XName name, string n, string ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main")
            {
                if (name == null)
                {
                    lock (_initSyncRoot)
                    {
                        if (name == null)
                        {
                            name = XName.Get(n, ns);
                        }
                    }
                }
                return name;
            }

            public static XName auto
            {
                get { return  Ensure(ref _auto, "auto", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"); }
            }

            public static XName color
            {
                get { return  Ensure(ref _color, "color", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"); }
            }

            public static XName creator
            {
                get { return  Ensure(ref _creator, "creator", "http://purl.org/dc/elements/1.1/"); }
            }

            public static XName id
            {
                get { return  Ensure(ref _id, "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships"); }
            }

            public static XName majorFont
            {
                get { return  Ensure(ref _majorFont, "majorFont", "http://schemas.openxmlformats.org/drawingml/2006/main"); }
            }

            public static XName minorFont
            {
                get { return  Ensure(ref _minorFont, "minorFont", "http://schemas.openxmlformats.org/drawingml/2006/main"); }
            }

            public static XName name
            {
                get { return  Ensure(ref _name, "name", string.Empty); }
            }

            public static XName patternFill
            {
                get { return  Ensure(ref _patternFill, "patternFill", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"); }
            }

            public static XName sheetId
            {
                get { return  Ensure(ref _sheetId, "sheetId", string.Empty); }
            }

            public static XName state
            {
                get { return  Ensure(ref _state, "state", string.Empty); }
            }

            public static XName style
            {
                get { return  Ensure(ref _style, "style", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"); }
            }

            public static XName val
            {
                get { return  Ensure(ref _val, "val", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"); }
            }
        }
    }
}

