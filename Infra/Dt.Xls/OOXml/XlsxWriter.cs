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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
#endregion

namespace Dt.Xls.OOXml
{
    /// <summary>
    /// Represents the <see cref="T:Dt.Xls.OOXml.XlsxWriter" /> class.
    /// </summary>
    internal class XlsxWriter
    {
        private List<IExcelCell> _cellsHasHyperlink = new List<IExcelCell>();
        private IDocumentProperties _documentProperties;
        private Dictionary<int, string> _drawingMapRef = new Dictionary<int, string>();
        private static HashSet<string> _errorsSet = new HashSet<string>();
        private IExcelWriter _excelWriter;
        private List<Tuple<string, IExcelConditionalFormat, IExcelConditionalFormatRule>> _extensionDataBarCondtionalFormats = new List<Tuple<string, IExcelConditionalFormat, IExcelConditionalFormatRule>>();
        private List<Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule>> _extensionIconSetConditionalFormats = new List<Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule>>();
        private Dictionary<string, int> _externalRefMaps = new Dictionary<string, int>();
        private Dictionary<string, string> _externalRefs = new Dictionary<string, string>();
        private bool _isR1C1;
        private Dictionary<int, string> _legacyDrawingMapRef = new Dictionary<int, string>();
        private LinkTable _linkTable;
        private Dictionary<int, string> _numberFormats = new Dictionary<int, string>();
        private List<SheetInfo> _sheetIDs = new List<SheetInfo>();
        private List<string> _sheetNames;
        private Dictionary<string, int> _sstTable = new Dictionary<string, int>();
        private List<IExcelBorder> _styleBorders = new List<IExcelBorder>();
        private List<Tuple<FillPatternType, IExcelColor, IExcelColor>> _styleFills = new List<Tuple<FillPatternType, IExcelColor, IExcelColor>>();
        private List<IExcelFont> _styleFonts = new List<IExcelFont>();
        private int _styleOffset;
        private List<Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>> _styleXfs;
        private Dictionary<int, Dictionary<string, string>> _tablesMapRef = new Dictionary<int, Dictionary<string, string>>();
        private ExcelFileType _workbookType;
        private Dictionary<int, string> _xfMap = new Dictionary<int, string>();

        static XlsxWriter()
        {
            _errorsSet.Add("#REF!");
            _errorsSet.Add("#NULL!");
            _errorsSet.Add("#DIV/0!");
            _errorsSet.Add("#VALUE!");
            _errorsSet.Add("#NAME?");
            _errorsSet.Add("#NUM!");
            _errorsSet.Add("#N/A");
        }

        public XlsxWriter(IExcelWriter writer, IDocumentProperties docProp, ExcelFileType workbookType)
        {
            this._excelWriter = writer;
            this._documentProperties = docProp;
            this._workbookType = workbookType;
            this._isR1C1 = this._excelWriter.GetCalculationProperty().RefMode == ExcelReferenceStyle.R1C1;
            this._linkTable = new LinkTable();
            this.InitLinkTable();
            ParsingContext.LinkTable = this._linkTable;
            ParsingContext.ParsingErrors.Clear();
            if (this._isR1C1)
            {
                ParsingContext.ReferenceStyle = ExcelReferenceStyle.R1C1;
            }
            else
            {
                ParsingContext.ReferenceStyle = ExcelReferenceStyle.A1;
            }
        }

        private string ConvertExternalReferencedFormulaBack(string formula)
        {
            int index = formula.IndexOf('!');
            if (((index != -1) && (index != (formula.Length - 1))) && (index < formula.Length))
            {
                formula.Substring(index + 1);
                int num2 = formula.LastIndexOf(',', index);
                string str = formula.Substring(num2 + 1, (index - num2) - 1);
                int num3 = str.LastIndexOf('(');
                if (num3 != -1)
                {
                    str = str.Substring(num3 + 1);
                }
                if (((str.Length > 2) && (str[0] == '\'')) && (str[str.Length - 1] == '\''))
                {
                    str = str.Substring(1, str.Length - 2);
                }
                if (str.IndexOf(':') == -1)
                {
                    if (this.SheetNames.Contains(str))
                    {
                        return formula;
                    }
                    return formula;
                }
                string[] strArray = str.Split(new char[] { ':' });
                if (this.SheetNames.Contains(strArray[0]) && this.SheetNames.Contains(strArray[1]))
                {
                    return formula;
                }
                return formula;
            }
            foreach (string str2 in this._externalRefMaps.Keys)
            {
                if (formula.IndexOf(str2) != -1)
                {
                    formula = formula.Replace(str2, string.Format("[{0}]", (object[]) new object[] { ((int) this._externalRefMaps[str2]).ToString((IFormatProvider) CultureInfo.InvariantCulture) }));
                    return formula;
                }
            }
            return formula;
        }

        private string ConvertR1C1FormulaToA1Formula(string formula, int sheet, int row, int column)
        {
            try
            {
                return Parser.Unparse(Parser.Parse(formula, row, column, true, this._linkTable), row, column, false);
            }
            catch (Exception exception)
            {
                this.LogError(string.Format(ResourceHelper.GetResourceString("writeR1C1FormulaError"), (object[]) new object[] { formula }), ExcelWarningCode.FormulaError, sheet, row, column, exception);
                return formula;
            }
        }

        private string DecodeDefinedName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }
            if (name.ToUpperInvariant() == "_FILTERDATABASE")
            {
                name = "_xlnm._FilterDatabase";
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in name)
            {
                switch (ch)
                {
                    case ' ':
                    case '\'':
                    case '"':
                    case '!':
                    case ':':
                        builder.Append('_');
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            return builder.ToString();
        }

        private int GetNextKey(int baseKey, HashSet<int> keys)
        {
            while (true)
            {
                if (!keys.Contains(baseKey))
                {
                    keys.Add(baseKey);
                    return baseKey;
                }
                baseKey++;
            }
        }

        private string GetRangeRefString(IExternalRange range)
        {
            string str;
            StringBuilder builder = new StringBuilder();
            if ((range.WorkbookName != null) && (this._externalRefs.Count > 0))
            {
                int num = 1;
                using (Dictionary<string, string>.Enumerator enumerator = this._externalRefs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Key == range.WorkbookName)
                        {
                            builder.Append("[");
                            builder.Append(((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            builder.Append("]");
                            goto Label_0096;
                        }
                        num++;
                    }
                }
            }
        Label_0096:
            str = range.WorksheetName;
            bool flag = this.PreProcessWorksheetName(range.WorksheetName, out str);
            builder.Append(str);
            if (flag)
            {
                builder.Insert(0, "'");
                builder.Append('\'');
            }
            builder.Append("!");
            if (range.WorkbookName != null)
            {
                builder.Append("$");
            }
            builder.Append(IndexHelper.GetColumnIndexInA1Letter(range.Column));
            if (range.WorkbookName != null)
            {
                builder.Append("$");
            }
            int num2 = range.Row + 1;
            builder.Append(((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            builder.Append(":");
            if (range.WorkbookName != null)
            {
                builder.Append("$");
            }
            builder.Append(IndexHelper.GetColumnIndexInA1Letter((range.Column + range.ColumnSpan) - 1));
            if (range.WorkbookName != null)
            {
                builder.Append("$");
            }
            builder.Append((int) (range.Row + range.RowSpan));
            return builder.ToString();
        }

        private Tuple<int, int> GetRowColumn(IRange range)
        {
            return new Tuple<int, int>(range.Row, range.Column);
        }

        private string GetRunFormattingTypeface(ReadOnlyCollection<IRunFormatting> runFormats, FontLanguage targetLanguage)
        {
            if (runFormats != null)
            {
                foreach (IRunFormatting formatting in runFormats)
                {
                    if (formatting.FontLanguage == targetLanguage)
                    {
                        return formatting.Typeface;
                    }
                }
            }
            return string.Empty;
        }

        private string GetTargetFileName(string fileName)
        {
            if (((fileName.IndexOf('/') != 0) && !this.IsAbsolutePath(fileName)) && fileName.StartsWith(".."))
            {
                fileName = "xl" + fileName.Substring(2);
            }
            return XFile.FixFileName(fileName);
        }

        private int GetXfId(IExtendedFormat format, HashSet<int> usedId = null)
        {
            if (format != null)
            {
                for (int i = 0; i < this._styleXfs.Count; i++)
                {
                    if ((this._styleXfs[i].Item6 != null) && this._styleXfs[i].Item6.Equals(format))
                    {
                        if (usedId == null)
                        {
                            return i;
                        }
                        if (!usedId.Contains(i))
                        {
                            usedId.Add(i);
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        private void InitLinkTable()
        {
            this._linkTable.InternalSheetNames.AddRange((IEnumerable<string>) this.SheetNames);
            List<IName> internalDefinedNames = this._excelWriter.GetInternalDefinedNames();
            if (internalDefinedNames != null)
            {
                foreach (IName name in internalDefinedNames)
                {
                    this._linkTable.AddDefinedNames(name.Name, name.Index, null);
                }
            }
            List<IFunction> customOrFunctionNameList = this._excelWriter.GetCustomOrFunctionNameList();
            if (customOrFunctionNameList != null)
            {
                this._linkTable.CustomOrFuctionNames = Enumerable.ToList<string>(Enumerable.Select<IFunction, string>((IEnumerable<IFunction>) customOrFunctionNameList, delegate (IFunction item) {
                    return item.Name;
                }));
            }
            List<IExternalWorkbookInfo> externWorkbookInfo = this._excelWriter.GetExternWorkbookInfo();
            Dictionary<string, List<IName>> dictionary = new Dictionary<string, List<IName>>();
            if (externWorkbookInfo != null)
            {
                foreach (IExternalWorkbookInfo info in externWorkbookInfo)
                {
                    if (!string.IsNullOrWhiteSpace(info.Name) && (info.DefinedNames != null))
                    {
                        dictionary.Add(info.Name, info.DefinedNames);
                    }
                }
            }
            this._linkTable.ExternalNamedCellRanges = dictionary;
        }

        private bool IsAbsolutePath(string fileName)
        {
            return (fileName.IndexOf("file:///", (StringComparison) StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool IsDataBarExtensionConditionalFormats(IExcelDataBarRule rule)
        {
            if (!(rule is IExcel2010DataBarRule))
            {
                return false;
            }
            IExcel2010DataBarRule rule2 = rule as IExcel2010DataBarRule;
            return (((rule.Maximum.Type == ExcelConditionalFormatValueObjectType.AutoMax) || (rule.Minimum.Type == ExcelConditionalFormatValueObjectType.AutoMin)) || (!rule2.IsGradientDatabar || (rule2.ShowBorder || (((rule2.NegativeFillColor != null) || rule2.NegativeBarColorAsPositive) || (((rule2.NegativeBorderColor != null) || !rule2.NegativeBorderColorSameAsPositive) || ((rule2.AxisPosition != DataBarAxisPosition.Automatic) || ((rule2.AxisColor != null) || (rule2.Direction != DataBarDirection.Context))))))));
        }

        private bool IsDoubleEqual(double expected, double actual)
        {
            return (Math.Abs((double) (expected - actual)) < 1E-06);
        }

        private bool IsIconSetsExtensionConditionalFormats(IExcelConditionalFormat conditionalFormat)
        {
            if (conditionalFormat == null)
            {
                return false;
            }
            if (((conditionalFormat.ConditionalFormattingRules == null) || (conditionalFormat.ConditionalFormattingRules.Count != 1)) || !(conditionalFormat.ConditionalFormattingRules[0] is IExcelIconSetsRule))
            {
                return false;
            }
            IExcelIconSetsRule rule = conditionalFormat.ConditionalFormattingRules[0] as IExcelIconSetsRule;
            if (((rule.IconSet != ExcelIconSetType.Icon_5Boxes) && (rule.IconSet != ExcelIconSetType.Icon_3Stars)) && (rule.IconSet != ExcelIconSetType.Icon_3Triangles))
            {
                return false;
            }
            return true;
        }

        private void LogChartErrors(string chart)
        {
            foreach (string str in ParsingContext.ParsingErrors)
            {
                this.LogError(string.Format(ResourceHelper.GetResourceString("convertChartFormulaR1C1ToA1Error"), (object[]) new object[] { chart, str }), ExcelWarningCode.FormulaError, null);
            }
            ParsingContext.ParsingErrors.Clear();
        }

        private void LogError(string message, ExcelWarningCode warningCode, Exception ex)
        {
            this.LogError(message, warningCode, -1, -1, -1, ex);
        }

        private void LogError(string message, ExcelWarningCode warningCode, int sheet, int row, int column, Exception ex)
        {
            this._excelWriter.OnExcelSaveError(new ExcelWarning(message, warningCode, sheet, row, column, ex));
        }

        private bool NeedWriteExtension(XmlWriter write, short sheet)
        {
            if (this._extensionIconSetConditionalFormats.Count > 0)
            {
                return true;
            }
            if (this._extensionDataBarCondtionalFormats.Count > 0)
            {
                return true;
            }
            if (this._excelWriter is IExcelSparklineWriter)
            {
                IExcelSparklineWriter writer = this._excelWriter as IExcelSparklineWriter;
                if (writer != null)
                {
                    List<IExcelSparklineGroup> excelSparkLineGroups = writer.GetExcelSparkLineGroups(sheet);
                    if ((excelSparkLineGroups != null) && (excelSparkLineGroups.Count > 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool NeedWriteHeaderFooter(IExcelPrintPageSetting setting)
        {
            return (setting.AdvancedHeadFooterSetting.HeaderFooterDifferentOddEvenPages || (setting.AdvancedHeadFooterSetting.HeaderFooterDifferentFirstPage || (!setting.AdvancedHeadFooterSetting.HeaderFooterScalesWithDocument || (!setting.AdvancedHeadFooterSetting.HeaderFooterAlignWithPageMargin || (!string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.HeaderOddPage) || (!string.IsNullOrWhiteSpace(setting.Header) || (!string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.FooterOddPage) || (!string.IsNullOrWhiteSpace(setting.Footer) || (!string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.HeaderEvenPage) || (!string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.FooterEvenPage) || (!string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.HeaderFirstPage) || !string.IsNullOrWhiteSpace(setting.AdvancedHeadFooterSetting.FooterFirstPage))))))))))));
        }

        private int Pixel2EMU(double emu)
        {
            return (int) ((emu * 914400.0) / 96.0);
        }

        private bool PreProcessWorksheetName(string sheetName, out string newSheetName)
        {
            bool flag = false;
            StringBuilder builder = new StringBuilder();
            foreach (char ch in sheetName)
            {
                switch (ch)
                {
                    case '\'':
                        builder.Append("''");
                        flag = true;
                        break;

                    case ' ':
                    case '"':
                        builder.Append(ch);
                        flag = true;
                        break;

                    default:
                        builder.Append(ch);
                        break;
                }
            }
            newSheetName = builder.ToString();
            return flag;
        }

        internal void Save(Stream outStream)
        {
            if ((outStream != null) && outStream.CanWrite)
            {
                MemoryFolder mFolder = new MemoryFolder();
                try
                {
                    mFolder.Reset();
                    XFile file = new XFile("", "");
                    XFile file2 = new XFile("/xl/workbook.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument");
                    file.AddRelationFile(file2);
                    this.SaveStyles(file2, mFolder);
                    this.SaveVBA(file2, mFolder);
                    this.SaveExternalRefernces(file2, mFolder);
                    this.SaveWorksheets(file2, mFolder);
                    this.SaveStringTables(file2, mFolder);
                    this.SaveTheme(file2, mFolder);
                    this.SaveWorkbook(file2, mFolder);
                    file.SavePackageRelationFiles(mFolder);
                    PackageXml.SaveContentTypes(file, mFolder, this._workbookType);
                    ZipHelper.CompressFiles(mFolder, outStream);
                    this._excelWriter.Finish();
                    ParsingContext.LinkTable = null;
                    ParsingContext.ParsingErrors.Clear();
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("writeFileError"), ExcelWarningCode.General, exception);
                }
                finally
                {
                    if (mFolder != null)
                    {
                        mFolder.Dispose();
                    }
                }
            }
        }

        internal void Save(string fileName)
        {
            // hdt
            if (!string.IsNullOrEmpty(fileName))
            {
                using (Stream stream = File.Create(fileName))
                {
                    this.Save(stream);
                }
            }
        }

        private void SaveCalcProperties(XmlWriter writer)
        {
            if (writer != null)
            {
                using (writer.WriteElement("calcPr"))
                {
                    writer.WriteAttributeString("calcId", "0");
                    ICalculationProperty calculationProperty = this._excelWriter.GetCalculationProperty();
                    switch (calculationProperty.CalculationMode)
                    {
                        case ExcelCalculationMode.Manual:
                            writer.WriteAttributeString("calcMode", "manual");
                            break;

                        case ExcelCalculationMode.AutomaticExceptTables:
                            writer.WriteAttributeString("calcMode", "autoNoTable");
                            break;
                    }
                    if (calculationProperty.MaxIterationCount != 100)
                    {
                        writer.WriteAttributeString("iterateCount", ((int) calculationProperty.MaxIterationCount).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (calculationProperty.IsIterateCalculate)
                    {
                        writer.WriteAttributeString("iterate", "1");
                    }
                    if (calculationProperty.RefMode != ExcelReferenceStyle.A1)
                    {
                        writer.WriteAttributeString("refMode", calculationProperty.RefMode.ToString());
                    }
                    if (!calculationProperty.ReCalculationBeforeSave)
                    {
                        writer.WriteAttributeString("calcOnSave", "0");
                    }
                    if (!calculationProperty.IsFullPrecision)
                    {
                        writer.WriteAttributeString("fullPrecision", "0");
                    }
                    if (Math.Abs((double) (0.001 - calculationProperty.MaximunChange)) > 1E-05)
                    {
                        writer.WriteAttributeString("iterateDelta", ((double) calculationProperty.MaximunChange).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        private void SaveExternalRefernces(XmlWriter writer)
        {
            if (this._externalRefs.Count != 0)
            {
                using (writer.WriteElement("externalReferences"))
                {
                    int num = 1;
                    foreach (KeyValuePair<string, string> pair in this._externalRefs)
                    {
                        writer.WriteLeafElementWithPrefixedAttribute("externalReference", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", "r", "id", pair.Value);
                        this._externalRefMaps.Add(pair.Key, num);
                        num++;
                    }
                }
            }
        }

        private void SaveExternalRefernces(XFile workbookFile, MemoryFolder mFolder)
        {
            string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
            List<IExternalWorkbookInfo> externWorkbookInfo = this._excelWriter.GetExternWorkbookInfo();
            if (externWorkbookInfo != null)
            {
                List<IExternalWorkbookInfo> list2 = Enumerable.ToList<IExternalWorkbookInfo>((IEnumerable<IExternalWorkbookInfo>) (from item in (IEnumerable<IExternalWorkbookInfo>) externWorkbookInfo select item));
                if ((list2 != null) && (list2.Count > 0))
                {
                    int num = 1;
                    foreach (IExternalWorkbookInfo info in list2)
                    {
                        XFile file = new XFile(str + @"\externalLinks\externalLink" + ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLink");
                        XFile file2 = new XFile(info.Name, "http://schemas.openxmlformats.org/officeDocument/2006/relationships/externalLinkPath");
                        file.RelationFiles.Add("rId" + ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture), file2);
                        string str2 = workbookFile.AddRelationFile(file);
                        this._externalRefs.Add(info.Name, str2);
                        this._externalRefMaps.Add(string.Format("[{0}]", (object[]) new object[] { info.Name }), num);
                        MemoryStream stream = new MemoryStream();
                        this.WriteLinkFile((Stream) stream, "rId" + ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture), info.Name, info.SheetNames);
                        stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                        mFolder.CreateMemoryFile(file.FileName, (Stream) stream);
                        num++;
                    }
                }
            }
        }

        private void SaveInternalDefinedNames(XmlWriter writer)
        {
            List<IName> internalDefinedNames = this._excelWriter.GetInternalDefinedNames();
            if (internalDefinedNames == null)
            {
                internalDefinedNames = new List<IName>();
            }
            int sheetCount = this._excelWriter.GetSheetCount();
            ExcelReferenceStyle refMode = this._excelWriter.GetCalculationProperty().RefMode;
            for (int i = 0; i < sheetCount; i++)
            {
                string printArea = this._excelWriter.GetPrintArea(i);
                if (!string.IsNullOrWhiteSpace(printArea))
                {
                    NamedCellRange range = new NamedCellRange("_xlnm.Print_Area", i);
                    if (this._isR1C1)
                    {
                        range.RefersTo = this.ConvertR1C1FormulaToA1Formula(printArea, i, 0, 0);
                    }
                    else
                    {
                        range.RefersTo = printArea;
                    }
                    internalDefinedNames.Add(range);
                }
                string printTitle = this._excelWriter.GetPrintTitle(i);
                if (!string.IsNullOrWhiteSpace(printTitle))
                {
                    NamedCellRange range2 = new NamedCellRange("_xlnm.Print_Titles", i);
                    if (this._isR1C1)
                    {
                        range2.RefersTo = this.ConvertR1C1FormulaToA1Formula(printTitle, i, 0, 0);
                    }
                    else
                    {
                        range2.RefersTo = printTitle;
                    }
                    internalDefinedNames.Add(range2);
                }
            }
            if ((internalDefinedNames != null) && (internalDefinedNames.Count > 0))
            {
                using (writer.WriteElement("definedNames"))
                {
                    foreach (IName name in internalDefinedNames)
                    {
                        try
                        {
                            string refersTo = name.RefersTo;
                            if (string.IsNullOrWhiteSpace(refersTo) && this._isR1C1)
                            {
                                refersTo = this.ConvertR1C1FormulaToA1Formula(name.RefersToR1C1, name.Index, 0, 0);
                            }
                            if (this._externalRefs.Count > 0)
                            {
                                int index = refersTo.IndexOf('!');
                                if (index != -1)
                                {
                                    int num4 = refersTo.IndexOf('[');
                                    int num5 = refersTo.IndexOf(']');
                                    if ((num4 < num5) && (num5 < index))
                                    {
                                        string str4 = refersTo.Substring(num4 + 1, (num5 - num4) - 1);
                                        int num6 = 1;
                                        using (Dictionary<string, string>.Enumerator enumerator2 = this._externalRefs.GetEnumerator())
                                        {
                                            while (enumerator2.MoveNext())
                                            {
                                                if (enumerator2.Current.Key.EndsWith(str4))
                                                {
                                                    refersTo = refersTo.Replace(str4, ((int) num6).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                    int num7 = refersTo.LastIndexOf('\\');
                                                    if (num7 != -1)
                                                    {
                                                        int num8 = refersTo.LastIndexOf('\'', num7);
                                                        if (num8 == -1)
                                                        {
                                                            refersTo = refersTo.Substring(num7);
                                                        }
                                                        else
                                                        {
                                                            refersTo = refersTo.Remove(num8 + 1, num7 - num8);
                                                        }
                                                    }
                                                    break;
                                                }
                                                num6++;
                                            }
                                            goto Label_0333;
                                        }
                                    }
                                    int startIndex = refersTo.IndexOf('\\');
                                    startIndex = (startIndex > 0) ? startIndex : 0;
                                    string str5 = refersTo.Substring(startIndex, index - startIndex);
                                    if (((str5.Length > 2) && str5.StartsWith("'")) && str5.EndsWith("'"))
                                    {
                                        str5 = str5.Substring(1, str5.Length - 2);
                                    }
                                    if (str5 != "")
                                    {
                                        int num10 = 1;
                                        using (Dictionary<string, string>.Enumerator enumerator3 = this._externalRefs.GetEnumerator())
                                        {
                                            while (enumerator3.MoveNext())
                                            {
                                                if (enumerator3.Current.Key.EndsWith(str5))
                                                {
                                                    refersTo = refersTo.Replace(str5, "[" + ((int) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture) + "]");
                                                    goto Label_0333;
                                                }
                                                num10++;
                                            }
                                        }
                                    }
                                }
                            }
                        Label_0333:
                            if (!string.IsNullOrWhiteSpace(refersTo))
                            {
                                using (writer.WriteElement("definedName"))
                                {
                                    writer.WriteAttributeString("name", this.DecodeDefinedName(name.Name));
                                    if (name.Index != -1)
                                    {
                                        writer.WriteAttributeString("localSheetId", ((int) name.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                        List<GcRect> selectionList = new List<GcRect>();
                                        GcPoint activeCell = new GcPoint();
                                        PaneType topLeft = PaneType.TopLeft;
                                        this._excelWriter.GetSelectionList((short) name.Index, selectionList, ref activeCell, ref topLeft);
                                    }
                                    if (name.IsHidden)
                                    {
                                        writer.WriteAttributeString("hidden", "1");
                                    }
                                    if (refersTo[0] != '#')
                                    {
                                        if (refersTo.IndexOf("''!") != -1)
                                        {
                                            string str6 = refersTo.Replace("''!", "!");
                                            writer.WriteValue(str6);
                                        }
                                        else
                                        {
                                            writer.WriteValue(refersTo);
                                        }
                                        continue;
                                    }
                                    if (refersTo.StartsWith("#REF!"))
                                    {
                                        writer.WriteValue("#REF!");
                                    }
                                    else
                                    {
                                        writer.WriteValue(refersTo);
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            this.LogError(ResourceHelper.GetResourceString("writeNameError"), ExcelWarningCode.DefinedOrCustomNameError, name.Index, -1, -1, exception);
                        }
                    }
                }
            }
        }

        private void SaveStringTables(XFile workbookFile, MemoryFolder mFolder)
        {
            if (((workbookFile != null) && (mFolder != null)) && (this._sstTable.Count > 0))
            {
                string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
                XFile file = new XFile(str + @"\sharedStrings.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings");
                workbookFile.AddRelationFile(file);
                MemoryStream stream = new MemoryStream();
                new XmlWriterSettings().Encoding = Encoding.UTF8;
                XmlWriter @this = XmlWriter.Create((Stream) stream, new XmlWriterSettings());
                using (@this.WriteDocument(null))
                {
                    using (@this.WriteElement("sst", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                    {
                        @this.WriteAttributeString("count", ((int) this._sstTable.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        @this.WriteAttributeString("uniqueCount", ((int) this._sstTable.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        foreach (KeyValuePair<string, int> pair in this._sstTable)
                        {
                            using (@this.WriteElement("si"))
                            {
                                using (@this.WriteElement("t"))
                                {
                                    if (pair.Key.StartsWith(" "))
                                    {
                                        @this.WriteAttributeString("xml", "space", null, "preserve");
                                    }
                                    @this.WriteString(pair.Key.ToSpecialEncodeForXML());
                                }
                            }
                        }
                    }
                }
                @this.Flush();
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                mFolder.CreateMemoryFile(file.FileName, (Stream) stream);
            }
        }

        private void SaveStyles(XFile workbookFile, MemoryFolder mFolder)
        {
            string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
            XFile file = new XFile(str + @"\styles.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles");
            workbookFile.AddRelationFile(file);
            MemoryStream stream = new MemoryStream();
            this.WriteExcelStyleFormats((Stream) stream);
            stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            mFolder.CreateMemoryFile(file.FileName, (Stream) stream);
        }

        private void SaveTheme(XFile workbookFile, MemoryFolder mFolder)
        {
            string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
            XFile file = new XFile(str + @"\theme/theme1.xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/theme");
            workbookFile.AddRelationFile(file);
            MemoryStream stream = new MemoryStream();
            this.WriteTheme((Stream) stream);
            stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            mFolder.CreateMemoryFile(file.FileName, (Stream) stream);
        }

        private void SaveVBA(XFile workbookFile, MemoryFolder mFolder)
        {
            if ((this._workbookType == ExcelFileType.XLSM) && (this._excelWriter is IExcelLosslessWriter))
            {
                Stream stream = null;
                if (this._excelWriter is IExcelLosslessWriter)
                {
                    List<IUnsupportRecord> unsupportItems = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(-1);
                    if (unsupportItems != null)
                    {
                        foreach (IUnsupportRecord record in unsupportItems)
                        {
                            if (record.Category == RecordCategory.VBA)
                            {
                                stream = record.Value as Stream;
                                break;
                            }
                        }
                    }
                }
                if (stream != null)
                {
                    string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
                    XFile file = new XFile(str + @"\vbaProject.bin", "http://schemas.microsoft.com/office/2006/relationships/vbaProject");
                    workbookFile.AddRelationFile(file);
                    stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    mFolder.CreateMemoryFile(file.FileName, stream);
                }
            }
        }

        private void SaveWorkbook(XFile workbookFile, MemoryFolder mFolder)
        {
            if ((workbookFile != null) && (mFolder != null))
            {
                MemoryStream stream = new MemoryStream();
                new XmlWriterSettings().Encoding = Encoding.UTF8;
                XmlWriter @this = XmlWriter.Create((Stream) stream, new XmlWriterSettings());
                using (@this.WriteDocument(true))
                {
                    using (@this.WriteElement("workbook", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                    {
                        @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                        this.SaveWorkbookProperty(@this);
                        this.SaveWorkbookView(@this);
                        using (@this.WriteElement("sheets"))
                        {
                            foreach (SheetInfo info in this._sheetIDs)
                            {
                                using (@this.WriteElement("sheet"))
                                {
                                    @this.WriteAttributeString("name", info.name.ToSpecialEncodeForXML());
                                    @this.WriteAttributeString("sheetId", ((uint) info.sheetID).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    if (this._excelWriter.IsSheetHidden(info.index))
                                    {
                                        @this.WriteAttributeString("state", "hidden");
                                    }
                                    @this.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", info.rID);
                                }
                            }
                        }
                        this.SaveExternalRefernces(@this);
                        this.SaveInternalDefinedNames(@this);
                        this.SaveCalcProperties(@this);
                    }
                }
                @this.Flush();
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                mFolder.CreateMemoryFile(workbookFile.FileName, (Stream) stream);
            }
        }

        private void SaveWorkbookProperty(XmlWriter writer)
        {
            if (writer != null)
            {
                using (writer.WriteElement("workbookPr"))
                {
                    IExcelWorkbookPropery workbookProperty = this._excelWriter.GetWorkbookProperty();
                    if (workbookProperty.IsDate1904)
                    {
                        writer.WriteAttributeString("date1904", "1");
                    }
                    if (!workbookProperty.SaveExternalLinks)
                    {
                        writer.WriteAttributeString("saveExternalLinkValues", "0");
                    }
                }
            }
        }

        private void SaveWorkbookView(XmlWriter writer)
        {
            if (writer != null)
            {
                using (writer.WriteElement("bookViews"))
                {
                    using (writer.WriteElement("workbookView"))
                    {
                        bool hidden = false;
                        bool iconic = false;
                        bool horizontalScroll = true;
                        bool verticalScroll = true;
                        bool showTabs = true;
                        int selectedTabIndex = 0;
                        int firstDisplayedTabIndex = 0;
                        int selectedTabCount = 0;
                        int tabRatio = 600;
                        IExcelRect window = this._excelWriter.GetWindow(ref hidden, ref iconic);
                        this._excelWriter.GetTabs(ref showTabs, ref selectedTabIndex, ref firstDisplayedTabIndex, ref selectedTabCount, ref tabRatio);
                        this._excelWriter.GetScroll(ref horizontalScroll, ref verticalScroll);
                        if (hidden)
                        {
                            writer.WriteAttributeString("visibility", "hidden");
                        }
                        if (iconic)
                        {
                            writer.WriteAttributeString("minimized", "1");
                        }
                        if (!horizontalScroll)
                        {
                            writer.WriteAttributeString("showHorizontalScroll", "0");
                        }
                        if (!verticalScroll)
                        {
                            writer.WriteAttributeString("showVerticalScroll", "0");
                        }
                        if (!showTabs)
                        {
                            writer.WriteAttributeString("showSheetTabs", "0");
                        }
                        if (window != null)
                        {
                            if (window.Left > 0.0)
                            {
                                writer.WriteAttributeString("xWindow", ((double) window.Left).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (window.Top > 0.0)
                            {
                                writer.WriteAttributeString("yWindow", ((double) window.Top).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (window.Width > 0.0)
                            {
                                writer.WriteAttributeString("windowWidth", ((double) window.Width).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (window.Height > 0.0)
                            {
                                writer.WriteAttributeString("windowHeight", ((double) window.Height).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                        if (tabRatio != 600)
                        {
                            writer.WriteAttributeString("tabRatio", ((int) tabRatio).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (firstDisplayedTabIndex > 0)
                        {
                            writer.WriteAttributeString("firstSheet", ((int) firstDisplayedTabIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (selectedTabIndex > 0)
                        {
                            writer.WriteAttributeString("activeTab", ((int) selectedTabIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }

        private void SaveWorksheets(XFile workbookFile, MemoryFolder mFolder)
        {
            string str = string.IsNullOrEmpty(workbookFile.FileName) ? "" : Path.GetDirectoryName(workbookFile.FileName);
            int sheetCount = this._excelWriter.GetSheetCount();
            HashSet<string> set = new HashSet<string>();
            HashSet<string> set2 = new HashSet<string>();
            for (int i = 0; i < sheetCount; i++)
            {
                string sheetName = this._excelWriter.GetSheetName(i);
                if ((sheetName != null) && (sheetName.Length > 0))
                {
                    if ((this._excelWriter is IExcelLosslessWriter) && ((this._excelWriter as IExcelLosslessWriter).GetSheetType(i) == ExcelSheetType.ChartSheet))
                    {
                        set2.Add(sheetName);
                    }
                    else
                    {
                        set.Add(sheetName);
                    }
                }
            }
            Dictionary<string, Stream> dictionary = null;
            if (this._excelWriter is IExcelLosslessWriter)
            {
                List<IUnsupportRecord> unsupportItems = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(-1);
                if (unsupportItems != null)
                {
                    foreach (IUnsupportRecord record in unsupportItems)
                    {
                        if (record.Category == RecordCategory.DrawingFileRelationFile)
                        {
                            dictionary = record.Value as Dictionary<string, Stream>;
                            break;
                        }
                    }
                }
            }
            if (dictionary != null)
            {
                foreach (KeyValuePair<string, Stream> pair in dictionary)
                {
                    string fileName = pair.Key;
                    mFolder.CreateMemoryFile(fileName, pair.Value);
                }
            }
            int num3 = 0;
            mFolder.ImageCounter = 0;
            int num4 = 1;
            int num5 = 0;
            for (short j = 0; j < sheetCount; j++)
            {
                int num11;
                int num7 = 1;
                SheetInfo info = new SheetInfo {
                    name = this._excelWriter.GetSheetName(j)
                };
                if ((info.name != null) && (info.name.Length != 0))
                {
                    goto Label_0232;
                }
                int num8 = j;
            Label_01A4:
                num11 = num8 + 1;
                info.name = "Sheet" + ((int) num11).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                if (this._excelWriter is IExcelLosslessWriter)
                {
                    if ((this._excelWriter as IExcelLosslessWriter).GetSheetType(j) == ExcelSheetType.ChartSheet)
                    {
                        if (set2.Contains(info.name))
                        {
                            goto Label_0227;
                        }
                        set2.Add(info.name);
                        goto Label_0232;
                    }
                    if (!set.Contains(info.name))
                    {
                        set.Add(info.name);
                        goto Label_0232;
                    }
                }
            Label_0227:
                num8++;
                goto Label_01A4;
            Label_0232:
                this._excelWriter.IsSheetHidden(j);
                info.sheetID = (uint)(j + 1);
                info.index = j;
                XFile file = null;
                if ((this._excelWriter is IExcelLosslessWriter) && ((this._excelWriter as IExcelLosslessWriter).GetSheetType(j) == ExcelSheetType.ChartSheet))
                {
                    file = new XFile(str + @"\chartsheets\sheet" + ((uint) info.sheetID).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chartsheet");
                }
                else
                {
                    file = new XFile(str + @"\worksheets\sheet" + ((uint) info.sheetID).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet");
                }
                info.rID = workbookFile.AddRelationFile(file);
                this._sheetIDs.Add(info);
                if (this._excelWriter is IExcelLosslessWriter)
                {
                    List<IUnsupportRecord> list2 = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(j);
                    if (list2 != null)
                    {
                        foreach (IUnsupportRecord record2 in list2)
                        {
                            if (record2.FileType == ExcelFileType.XLSX)
                            {
                                if (record2.Category == RecordCategory.SheetFileRelationShip)
                                {
                                    Relationship relationship = record2.Value as Relationship;
                                    XFile file2 = new XFile(this.GetTargetFileName(relationship.Target), relationship.Type) {
                                        Target = relationship.Target,
                                        TargetMode = relationship.TargetMode
                                    };
                                    if (file2.FileType == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing")
                                    {
                                        foreach (IUnsupportRecord record3 in list2)
                                        {
                                            if (record3.Category == RecordCategory.VmlMediaFile)
                                            {
                                                Tuple<string, Dictionary<string, Stream>> tuple = record3.Value as Tuple<string, Dictionary<string, Stream>>;
                                                if (((tuple != null) && (tuple.Item2 != null)) && (Path.GetFileName(tuple.Item1) == Path.GetFileName(file2.FileName)))
                                                {
                                                    foreach (KeyValuePair<string, Stream> pair2 in tuple.Item2)
                                                    {
                                                        string introduced82 = pair2.Key;
                                                        mFolder.CreateMemoryFile(introduced82, pair2.Value);
                                                    }
                                                }
                                            }
                                            else if (record3.Category == RecordCategory.VmlMediaFileRelationShip)
                                            {
                                                Tuple<string, Relationship> tuple2 = record3.Value as Tuple<string, Relationship>;
                                                if (((tuple2 != null) && (Path.GetFileName(tuple2.Item1) == Path.GetFileName(file2.FileName))) && (tuple2.Item2 != null))
                                                {
                                                    XFile file3 = new XFile(this.GetTargetFileName(tuple2.Item2.Target), tuple2.Item2.Type) {
                                                        Target = tuple2.Item2.Target,
                                                        TargetMode = tuple2.Item2.TargetMode
                                                    };
                                                    file2.RelationFiles.Add(tuple2.Item2.Id, file3);
                                                }
                                            }
                                        }
                                    }
                                    file.RelationFiles.Add(relationship.Id, file2);
                                    if (relationship.Type == "http://schemas.openxmlformats.org/officeDocument/2006/relationships/vmlDrawing")
                                    {
                                        this._legacyDrawingMapRef[j] = relationship.Id;
                                    }
                                }
                                if (record2.Category == RecordCategory.SheetFileRelationFile)
                                {
                                    dictionary = record2.Value as Dictionary<string, Stream>;
                                }
                            }
                        }
                    }
                }
                if (dictionary != null)
                {
                    foreach (KeyValuePair<string, Stream> pair3 in dictionary)
                    {
                        Stream stream = pair3.Value;
                        stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                        mFolder.CreateMemoryFile(pair3.Key, stream);
                    }
                }
                if (this._excelWriter is IExcelChartWriter)
                {
                    IExcelChartWriter writer = this._excelWriter as IExcelChartWriter;
                    List<IExcelChart> worksheetCharts = writer.GetWorksheetCharts(j);
                    List<IExcelImage> workSheetImages = writer.GetWorkSheetImages(j);
                    List<string> list5 = new List<string>();
                    List<Relationship> list6 = new List<Relationship>();
                    new List<Relationship>();
                    if (writer is IExcelLosslessWriter)
                    {
                        List<IUnsupportRecord> list7 = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(j);
                        if (list7 != null)
                        {
                            foreach (IUnsupportRecord record4 in list7)
                            {
                                if (record4.FileType == ExcelFileType.XLSX)
                                {
                                    if (record4.Category == RecordCategory.Drawing)
                                    {
                                        list5.Add((string) (record4.Value as string));
                                    }
                                    else if (record4.Category == RecordCategory.DrawingFileRelationShip)
                                    {
                                        list6.Add(record4.Value as Relationship);
                                    }
                                }
                            }
                        }
                    }
                    if ((((worksheetCharts != null) && (worksheetCharts.Count > 0)) || ((workSheetImages != null) && (workSheetImages.Count > 0))) || (((list5 != null) && (list5.Count > 0)) || ((list6 != null) && (list6.Count > 0))))
                    {
                        num5++;
                        XFile file4 = new XFile(str + @"\drawings\drawing" + ((int) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing");
                        string str3 = string.Format("drawing{0}.xml", (object[]) new object[] { ((int) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture) });
                        file4.Target = "../drawings/" + str3;
                        string str4 = "rId" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                        num7++;
                        file.RelationFiles.Add(str4, file4);
                        this._drawingMapRef[j] = str4;
                        List<int> list8 = new List<int>();
                        if ((list6 != null) && (list6.Count > 0))
                        {
                            foreach (Relationship relationship2 in list6)
                            {
                                XFile file5 = new XFile(relationship2.Target, relationship2.Type) {
                                    Target = relationship2.Target,
                                    TargetMode = relationship2.TargetMode
                                };
                                file4.RelationFiles.Add(relationship2.Id, file5);
                                int num9 = int.Parse(relationship2.Id.Substring(3, relationship2.Id.Length - 3));
                                list8.Add(num9);
                            }
                        }
                        Dictionary<object, int> dictionary2 = new Dictionary<object, int>();
                        int num10 = 1;
                        if (worksheetCharts != null)
                        {
                            foreach (IExcelChart chart in worksheetCharts)
                            {
                                mFolder.ImageCounter++;
                                XFile chartFile = new XFile(str + @"\charts\chart" + ((int) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/chart");
                                string str6 = string.Format("chart{0}.xml", (object[]) new object[] { ((int) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture) });
                                chartFile.Target = "../charts/" + str6;
                                num4++;
                                while (list8.Contains(num10))
                                {
                                    num10++;
                                }
                                list8.Add(num10);
                                dictionary2[chart] = num10;
                                string str7 = "rId" + ((int) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                file4.RelationFiles.Add(str7, chartFile);
                                MemoryStream stream2 = new MemoryStream();
                                this.WriteExcelChart(chartFile, mFolder, (Stream) stream2, "rId" + ((int) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture), chart, j);
                                stream2.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                                mFolder.CreateMemoryFile(chartFile.FileName, (Stream) stream2);
                            }
                        }
                        if (workSheetImages != null)
                        {
                            foreach (IExcelImage image in workSheetImages)
                            {
                                mFolder.ImageCounter++;
                                string str8 = "jpg";
                                switch (image.ImageType)
                                {
                                    case ImageType.JPG:
                                        str8 = "jpg";
                                        break;

                                    case ImageType.PNG:
                                        str8 = "png";
                                        break;

                                    case ImageType.Bitmap:
                                        str8 = "bmp";
                                        break;

                                    case ImageType.Gif:
                                        str8 = "gif";
                                        break;
                                }
                                XFile file7 = new XFile(string.Concat((string[]) new string[] { str, @"\media\image", ((int) mFolder.ImageCounter).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat), ".", str8 }), "http://schemas.openxmlformats.org/officeDocument/2006/relationships/image");
                                string str9 = string.Format("image{0}.{1}", (object[]) new object[] { ((int) mFolder.ImageCounter).ToString((IFormatProvider) CultureInfo.InvariantCulture), str8 });
                                file7.Target = "../media/" + str9;
                                while (list8.Contains(num10))
                                {
                                    num10++;
                                }
                                list8.Add(num10);
                                dictionary2[image] = num10;
                                string str10 = "rId" + ((int) num10).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                file4.RelationFiles.Add(str10, file7);
                                MemoryStream stream3 = new MemoryStream(image.SourceArray);
                                stream3.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                                mFolder.CreateMemoryFile(file7.FileName, (Stream) stream3);
                            }
                        }
                        MemoryStream stream4 = new MemoryStream();
                        this.WriteDrawingFile(mFolder, (Stream) stream4, "rId" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture), worksheetCharts, workSheetImages, (IList<string>) Enumerable.ToList<string>(Enumerable.Cast<string>((IEnumerable) list5)), (IDictionary<object, int>) dictionary2);
                        stream4.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                        mFolder.CreateMemoryFile(file4.FileName, (Stream) stream4);
                    }
                }
                if (this._excelWriter is IExcelTableWriter)
                {
                    List<IExcelTable> sheetTables = (this._excelWriter as IExcelTableWriter).GetSheetTables(j);
                    if ((sheetTables != null) && (sheetTables.Count > 0))
                    {
                        foreach (IExcelTable table in sheetTables)
                        {
                            num3++;
                            XFile file8 = new XFile(str + @"\tables\table" + ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/table");
                            string str11 = string.Format("table{0}.xml", (object[]) new object[] { ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture) });
                            file8.Target = "../tables/" + str11;
                            string str12 = "rId" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                            num7++;
                            file.RelationFiles.Add(str12, file8);
                            if (!this._tablesMapRef.ContainsKey(j) || (this._tablesMapRef[j] == null))
                            {
                                this._tablesMapRef[j] = new Dictionary<string, string>();
                            }
                            this._tablesMapRef[j].Add(table.Name, str12);
                            MemoryStream stream5 = new MemoryStream();
                            this.WriteTableFile((Stream) stream5, "rId" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture), table);
                            stream5.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                            mFolder.CreateMemoryFile(file8.FileName, (Stream) stream5);
                        }
                    }
                }
                this._cellsHasHyperlink.Clear();
                MemoryStream stream6 = new MemoryStream();
                this.WriteSheet((Stream) stream6, j);
                if (this._cellsHasHyperlink.Count > 0)
                {
                    foreach (IExcelCell cell in this._cellsHasHyperlink)
                    {
                        XFile file9 = new XFile(str + @"\worksheets_rels\sheet" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture.NumberFormat) + ".xml", "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink") {
                            Target = UrlHelper.Encode(cell.Hyperlink.Address),
                            TargetMode = "External"
                        };
                        string str13 = "rId" + ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                        num7++;
                        file.RelationFiles.Add(str13, file9);
                    }
                }
                stream6.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                mFolder.CreateMemoryFile(file.FileName, (Stream) stream6);
            }
        }

        private string ToRange(IRange range)
        {
            if (range == null)
            {
                return "#REF!";
            }
            return string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
        }

        private CellType TryFigureOutCellType(object value)
        {
            if (value != null)
            {
                bool flag;
                double num;
                DateTime time;
                if (value is bool)
                {
                    return CellType.Boolean;
                }
                if (((value is int) || (value is float)) || (((value is double) || (value is short)) || (value is long)))
                {
                    return CellType.Numeric;
                }
                if ((value is DateTime) || (value is TimeSpan))
                {
                    return CellType.Datetime;
                }
                string str = Convert.ToString(value, (IFormatProvider) CultureInfo.InvariantCulture);
                if (bool.TryParse(str, out flag))
                {
                    return CellType.Boolean;
                }
                if (double.TryParse(str, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out num))
                {
                    return CellType.Numeric;
                }
                if (DateTime.TryParse(str, (IFormatProvider) CultureInfo.InvariantCulture, (DateTimeStyles) DateTimeStyles.None, out time))
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

        private string TryGetColumnIndexInA1LetterForm(int column)
        {
            column = Math.Min(column, 0x4000);
            return IndexHelper.GetColumnIndexInA1Letter(column);
        }

        private void WriteAutoFilter(XmlWriter writer, IExcelAutoFilter autoFilter)
        {
            using (writer.WriteElement("autoFilter"))
            {
                IRange range = autoFilter.Range;
                writer.WriteAttributeString("ref", this.ToRange(range));
                if (autoFilter.FilterColumns != null)
                {
                    foreach (IExcelFilterColumn column in autoFilter.FilterColumns)
                    {
                        if ((column != null) && ((((column.IconFilter != null) || (column.ColorFilter != null)) || ((column.Top10 != null) || (column.Filters != null))) || ((column.DynamicFilter != null) || (column.CustomFilters != null))))
                        {
                            using (writer.WriteElement("filterColumn"))
                            {
                                writer.WriteAttributeString("colId", ((uint) column.AutoFilterColumnId).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                if (column.IconFilter != null)
                                {
                                    this.WriteIconFilter(writer, column.IconFilter);
                                }
                                else if (column.ColorFilter != null)
                                {
                                    this.WriteColorFilter(writer, column.ColorFilter);
                                }
                                else if (column.Top10 != null)
                                {
                                    this.WriteTop10Filter(writer, column.Top10);
                                }
                                else if (column.Filters != null)
                                {
                                    this.WriteFilters(writer, column.Filters);
                                }
                                else if (column.DynamicFilter != null)
                                {
                                    this.WriteDynamicFilter(writer, column.DynamicFilter);
                                }
                                else if (column.CustomFilters != null)
                                {
                                    this.WriteCustomFilters(writer, column.CustomFilters);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void WriteAutoFilter(XmlWriter writer, short sheet)
        {
            IExcelAutoFilter autoFilter = this._excelWriter.GetAutoFilter(sheet);
            if ((autoFilter != null) && (autoFilter.Range != null))
            {
                try
                {
                    this.WriteAutoFilter(writer, autoFilter);
                }
                catch (Exception exception)
                {
                    this.LogError(ResourceHelper.GetResourceString("writeAutoFilterError"), ExcelWarningCode.General, sheet, autoFilter.Range.Row, autoFilter.Range.Column, exception);
                }
            }
        }

        private void WriteBorder(XmlWriter writer, IExcelBorder border)
        {
            using (writer.WriteElement("border"))
            {
                this.WriteBorder(writer, "left", border.Left);
                this.WriteBorder(writer, "right", border.Right);
                this.WriteBorder(writer, "top", border.Top);
                this.WriteBorder(writer, "bottom", border.Bottom);
                if (border is IExcelTableBorder)
                {
                    IExcelTableBorder border2 = border as IExcelTableBorder;
                    if (border2.Vertical != null)
                    {
                        this.WriteBorder(writer, "vertical", border2.Vertical);
                    }
                    if (border2.Horizontal != null)
                    {
                        this.WriteBorder(writer, "horizontal", border2.Horizontal);
                    }
                }
            }
        }

        private void WriteBorder(XmlWriter writer, string node, IExcelBorderSide side)
        {
            using (writer.WriteElement(node))
            {
                if (side != null)
                {
                    if (side.LineStyle != ExcelBorderStyle.None)
                    {
                        writer.WriteAttributeString("style", side.LineStyle.ToString().ToCamelCase());
                    }
                    if (side.Color != null)
                    {
                        this.WriteColor(writer, "color", side.Color);
                    }
                }
            }
        }

        private static void WriteChoice(XmlWriter writer, int style)
        {
            using (writer.WriteElement("Choice", null, "mc"))
            {
                writer.WriteAttributeString("xmlns", "c14", null, "http://schemas.microsoft.com/office/drawing/2007/8/2/chart");
                writer.WriteAttributeString("Requires", "c14");
                writer.WriteLeafElementWithAttribute("style", null, "c14", "val", ((int) style).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
        }

        private void WriteColomnInfo(XmlWriter writer, short sheet)
        {
            List<IExcelColumn> list = new List<IExcelColumn>();
            List<IExcelColumn> nonEmptyColumns = this._excelWriter.GetNonEmptyColumns(sheet);
            if (nonEmptyColumns != null)
            {
                foreach (IExcelColumn column in nonEmptyColumns)
                {
                    if (column.Index >= 0x4000)
                    {
                        break;
                    }
                    if (column != null)
                    {
                        list.Add(column);
                    }
                }
            }
            double defaultColumnWidth = this._excelWriter.GetDefaultColumnWidth(sheet);
            if (list.Count != 0)
            {
                using (writer.WriteElement("cols"))
                {
                    int num2 = 0;
                    while (num2 < list.Count)
                    {
                        IExcelColumn column2 = list[num2];
                        int index = column2.Index;
                        if (index >= 0x4000)
                        {
                            return;
                        }
                        int num4 = num2 + 1;
                        int num5 = index;
                        while (num4 < list.Count)
                        {
                            IExcelColumn column3 = list[num4];
                            if ((((column3.Index != (num5 + 1)) || (column3.Index >= 0x4000)) || ((column3.FormatId != column2.FormatId) || (column3.Visible != column2.Visible))) || (((column3.OutLineLevel != column2.OutLineLevel) || (column3.Collapsed != column2.Collapsed)) || (column3.Width != column2.Width)))
                            {
                                break;
                            }
                            num5 = column3.Index;
                            num2 = num4;
                            num4++;
                        }
                        num2++;
                        using (writer.WriteElement("col"))
                        {
                            int num6 = index + 1;
                            writer.WriteAttributeString("min", ((int) num6).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            int num7 = num5 + 1;
                            writer.WriteAttributeString("max", ((int) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            if (column2.FormatId >= 0)
                            {
                                string str;
                                if (this._xfMap.TryGetValue(column2.FormatId + this._styleOffset, out str))
                                {
                                    writer.WriteAttributeString("style", str);
                                }
                                else
                                {
                                    writer.WriteAttributeString("style", "0");
                                }
                            }
                            if (!double.IsNaN(column2.Width) && (column2.Width > 0.0))
                            {
                                writer.WriteAttributeString("width", ((double) column2.Width).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            else
                            {
                                writer.WriteAttributeString("width", ((double) defaultColumnWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!column2.Visible)
                            {
                                writer.WriteAttributeString("hidden", "1");
                            }
                            if (column2.Collapsed)
                            {
                                writer.WriteAttributeString("collapsed", "1");
                            }
                            if (!double.IsNaN(column2.Width) && (Math.Abs((double) (column2.Width - defaultColumnWidth)) > 0.0001))
                            {
                                writer.WriteAttributeString("customWidth", "1");
                            }
                            if (column2.OutLineLevel != 0)
                            {
                                writer.WriteAttributeString("outlineLevel", ((byte) column2.OutLineLevel).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            continue;
                        }
                    }
                }
            }
        }

        private void WriteColor(XmlWriter writer, string node, IExcelColor color)
        {
            if (color != null)
            {
                using (writer.WriteElement(node))
                {
                    if (color.IsAutoColor)
                    {
                        writer.WriteAttributeString("auto", "1");
                    }
                    else if (color.IsIndexedColor)
                    {
                        writer.WriteAttributeString("indexed", ((uint) color.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    else if (color.IsThemeColor)
                    {
                        writer.WriteAttributeString("theme", ((uint) color.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        if (color.Tint != 0.0)
                        {
                            writer.WriteAttributeString("tint", ((double) color.Tint).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    else if (color.IsRGBColor)
                    {
                        string str = ((uint) color.Value).ToString("X8");
                        writer.WriteAttributeString("rgb", str);
                        if (color.Tint != 0.0)
                        {
                            writer.WriteAttributeString("tint", ((double) color.Tint).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }

        private void WriteColor(XmlWriter writer, string node, string prefix, IExcelColor color)
        {
            if (color != null)
            {
                writer.WriteStartElement(prefix, node, null);
                if (color.IsAutoColor)
                {
                    writer.WriteAttributeString("auto", "1");
                }
                else
                {
                    if (color.IsIndexedColor)
                    {
                        writer.WriteAttributeString("indexed", ((uint) color.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    else if (color.IsThemeColor)
                    {
                        writer.WriteAttributeString("theme", ((uint) color.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        if (color.Tint != 0.0)
                        {
                            writer.WriteAttributeString("tint", ((double) color.Tint).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    else if (color.IsRGBColor)
                    {
                        string str = ((uint) color.Value).ToString("X8");
                        writer.WriteAttributeString("rgb", str);
                        if (color.Tint != 0.0)
                        {
                            writer.WriteAttributeString("tint", ((double) color.Tint).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    writer.WriteEndElement();
                }
            }
        }

        private void WriteColorFilter(XmlWriter writer, IExcelColorFilter excelColorFilter)
        {
            using (writer.WriteElement("colorFilter"))
            {
                if (!excelColorFilter.CellColor)
                {
                    writer.WriteAttributeString("cellColor", "0");
                }
                writer.WriteAttributeString("dxfId", ((uint) excelColorFilter.DxfId).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
        }

        private void WriteColorScheme(XmlWriter writer, string ns, string node, ColorSchemeIndex colorSchemeIndex, IColorScheme colorScheme)
        {
            if (colorScheme != null)
            {
                using (writer.WriteElement(node, ns, "a"))
                {
                    IExcelColor color = colorScheme[colorSchemeIndex];
                    if (node == "dk1")
                    {
                        using (writer.WriteElement("sysClr", ns, "a"))
                        {
                            writer.WriteAttributeString("val", "windowText");
                            writer.WriteAttributeString("lastClr", "000000");
                            return;
                        }
                    }
                    if (node == "lt1")
                    {
                        using (writer.WriteElement("sysClr", ns, "a"))
                        {
                            writer.WriteAttributeString("val", "window");
                            writer.WriteAttributeString("lastClr", "FFFFFF");
                            return;
                        }
                    }
                    if (color.ColorType == ExcelColorType.RGB)
                    {
                        using (writer.WriteElement("srgbClr", ns, "a"))
                        {
                            writer.WriteAttributeString("val", ((uint) color.Value).ToString("X8").Substring(2));
                            return;
                        }
                    }
                    if (color.ColorType == ExcelColorType.Theme)
                    {
                        using (writer.WriteElement("schemeClr", ns, "a"))
                        {
                            writer.WriteAttributeString("val", ((ColorSchemeIndex) color.Value).ToSchemeClrValue());
                            return;
                        }
                    }
                    if (color.ColorType == ExcelColorType.Indexed)
                    {
                        uint num = ColorExtension.GetPaletteColor((int) this._excelWriter.GetPaletteColor(color)).ToArgb();
                        using (writer.WriteElement("srgbClr", ns, "a"))
                        {
                            writer.WriteAttributeString("val", ((uint) num).ToString("X8".Substring(2)));
                        }
                    }
                }
            }
        }

        private void WriteConditionalFormating(XmlWriter writer, short sheet)
        {
            this._extensionIconSetConditionalFormats.Clear();
            this._extensionDataBarCondtionalFormats.Clear();
            List<IExcelConditionalFormat> conditionalFormat = this._excelWriter.GetConditionalFormat(sheet);
            if ((conditionalFormat != null) && (conditionalFormat.Count > 0))
            {
                foreach (IExcelConditionalFormat format in conditionalFormat)
                {
                    if (((format != null) && (format.Ranges != null)) && (format.Ranges.Count != 0))
                    {
                        if (this.IsIconSetsExtensionConditionalFormats(format))
                        {
                            this._extensionIconSetConditionalFormats.Add(new Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule>(format, format.ConditionalFormattingRules[0]));
                        }
                        else
                        {
                            try
                            {
                                using (writer.WriteElement("conditionalFormatting"))
                                {
                                    Tuple<int, int> rowColumn = this.GetRowColumn(format.Ranges[0]);
                                    List<string> list2 = new List<string>();
                                    foreach (IRange range in Enumerable.Distinct<IRange>((IEnumerable<IRange>) format.Ranges, new RangeComparer()))
                                    {
                                        this.GetRowColumn(range);
                                        string str = string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
                                        list2.Add(str);
                                    }
                                    writer.WriteAttributeString("sqref", string.Join(" ", (IEnumerable<string>) list2));
                                    foreach (IExcelConditionalFormatRule rule in format.ConditionalFormattingRules)
                                    {
                                        if (rule is IExcelDataBarRule)
                                        {
                                            IExcelDataBarRule rule2 = rule as IExcelDataBarRule;
                                            using (writer.WriteElement("cfRule"))
                                            {
                                                writer.WriteAttributeString("type", "dataBar");
                                                writer.WriteAttributeString("priority", ((int) rule2.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                using (writer.WriteElement("dataBar"))
                                                {
                                                    if (!rule2.ShowValue)
                                                    {
                                                        writer.WriteAttributeString("showValue", "0");
                                                    }
                                                    string str2 = rule2.Minimum.Type.ToString().ToCamelCase();
                                                    if (str2.ToUpperInvariant() == "AUTOMIN")
                                                    {
                                                        str2 = "min";
                                                    }
                                                    if (!string.IsNullOrEmpty(rule2.Minimum.Value))
                                                    {
                                                        KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("type", str2), new KeyValuePair<string, string>("val", rule2.Minimum.Value) };
                                                        writer.WriteLeafElementWithAttributes("cfvo", attributes);
                                                    }
                                                    else
                                                    {
                                                        KeyValuePair<string, string>[] pairArray2 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("type", str2) };
                                                        writer.WriteLeafElementWithAttributes("cfvo", pairArray2);
                                                    }
                                                    string str3 = rule2.Maximum.Type.ToString().ToCamelCase();
                                                    if (str3.ToUpperInvariant() == "AUTOMAX")
                                                    {
                                                        str3 = "max";
                                                    }
                                                    if (!string.IsNullOrEmpty(rule2.Maximum.Value))
                                                    {
                                                        KeyValuePair<string, string>[] pairArray3 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("type", str3), new KeyValuePair<string, string>("val", rule2.Maximum.Value) };
                                                        writer.WriteLeafElementWithAttributes("cfvo", pairArray3);
                                                    }
                                                    else
                                                    {
                                                        KeyValuePair<string, string>[] pairArray4 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("type", str3) };
                                                        writer.WriteLeafElementWithAttributes("cfvo", pairArray4);
                                                    }
                                                    writer.WriteLeafElementWithAttribute("color", "rgb", ((uint) rule2.Color.Value).ToString("X"));
                                                }
                                                if (this.IsDataBarExtensionConditionalFormats(rule2))
                                                {
                                                    using (writer.WriteElement("extLst"))
                                                    {
                                                        using (writer.WriteElement("ext"))
                                                        {
                                                            string str4 = "{" + Guid.NewGuid().ToString() + "}";
                                                            writer.WriteAttributeString("xmlns", "x14", null, "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
                                                            writer.WriteAttributeString("uri", "{B025F937-C7B1-47D3-B67F-A62EFF666E3E}");
                                                            writer.WriteStartElement("x14", "id", null);
                                                            writer.WriteValue(str4.ToString());
                                                            writer.WriteEndElement();
                                                            this._extensionDataBarCondtionalFormats.Add(new Tuple<string, IExcelConditionalFormat, IExcelConditionalFormatRule>(str4.ToString(), format, rule2));
                                                        }
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (rule is IExcelColorScaleRule)
                                        {
                                            IExcelColorScaleRule rule3 = rule as IExcelColorScaleRule;
                                            using (writer.WriteElement("cfRule"))
                                            {
                                                writer.WriteAttributeString("type", "colorScale");
                                                writer.WriteAttributeString("priority", ((int) rule3.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                using (writer.WriteElement("colorScale"))
                                                {
                                                    using (writer.WriteElement("cfvo"))
                                                    {
                                                        writer.WriteAttributeString("type", rule3.Minimum.Type.ToString().ToCamelCase());
                                                        if (!string.IsNullOrEmpty(rule3.Minimum.Value))
                                                        {
                                                            writer.WriteAttributeString("val", rule3.Minimum.Value);
                                                        }
                                                    }
                                                    if (rule3.HasMiddleNode)
                                                    {
                                                        using (writer.WriteElement("cfvo"))
                                                        {
                                                            writer.WriteAttributeString("type", rule3.Middle.Type.ToString().ToCamelCase());
                                                            if (!string.IsNullOrEmpty(rule3.Middle.Value))
                                                            {
                                                                writer.WriteAttributeString("val", rule3.Middle.Value);
                                                            }
                                                        }
                                                    }
                                                    using (writer.WriteElement("cfvo"))
                                                    {
                                                        writer.WriteAttributeString("type", rule3.Maximum.Type.ToString().ToCamelCase());
                                                        if (!string.IsNullOrEmpty(rule3.Maximum.Value))
                                                        {
                                                            writer.WriteAttributeString("val", rule3.Maximum.Value);
                                                        }
                                                    }
                                                    writer.WriteLeafElementWithAttribute("color", "rgb", ((uint) rule3.MinimumColor.Value).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                                                    if (rule3.HasMiddleNode)
                                                    {
                                                        writer.WriteLeafElementWithAttribute("color", "rgb", ((uint) rule3.MiddleColor.Value).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                                                    }
                                                    writer.WriteLeafElementWithAttribute("color", "rgb", ((uint) rule3.MaximumColor.Value).ToString("X", (IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                                continue;
                                            }
                                        }
                                        if (rule is IExcelIconSetsRule)
                                        {
                                            IExcelIconSetsRule rule4 = rule as IExcelIconSetsRule;
                                            if (((rule4.IconSet == ExcelIconSetType.Icon_3Stars) || (rule4.IconSet == ExcelIconSetType.Icon_3Triangles)) || (rule4.IconSet == ExcelIconSetType.Icon_5Boxes))
                                            {
                                                this._extensionIconSetConditionalFormats.Add(new Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule>(format, rule4));
                                                continue;
                                            }
                                            using (writer.WriteElement("cfRule"))
                                            {
                                                writer.WriteAttributeString("type", "iconSet");
                                                writer.WriteAttributeString("priority", ((int) rule4.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                using (writer.WriteElement("iconSet"))
                                                {
                                                    if (rule4.IconSet != ExcelIconSetType.Icon_NIL)
                                                    {
                                                        writer.WriteAttributeString("iconSet", rule4.IconSet.ToString().Substring(5));
                                                    }
                                                    if (rule4.ReversedOrder)
                                                    {
                                                        writer.WriteAttributeString("reverse", "1");
                                                    }
                                                    if (rule4.IconOnly)
                                                    {
                                                        writer.WriteAttributeString("showValue", "0");
                                                    }
                                                    for (int i = 0; i < rule4.Thresholds.Count; i++)
                                                    {
                                                        IExcelConditionalFormatValueObject obj2 = rule4.Thresholds[i];
                                                        using (writer.WriteElement("cfvo"))
                                                        {
                                                            writer.WriteAttributeString("type", obj2.Type.ToString().ToCamelCase());
                                                            writer.WriteAttributeString("val", obj2.Value);
                                                            if (rule4.NotPassTheThresholdsWhenEquals[i])
                                                            {
                                                                writer.WriteAttributeString("gte", "0");
                                                            }
                                                        }
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (rule is IExcelHighlightingRule)
                                        {
                                            IExcelHighlightingRule rule5 = rule as IExcelHighlightingRule;
                                            using (writer.WriteElement("cfRule"))
                                            {
                                                writer.WriteAttributeString("type", "cellIs");
                                                writer.WriteAttributeString("priority", ((int) rule5.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                if (rule5.DifferentialFormattingId >= 0)
                                                {
                                                    writer.WriteAttributeString("dxfId", ((int) rule5.DifferentialFormattingId).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                                if (rule5.StopIfTrue)
                                                {
                                                    writer.WriteAttributeString("stopIfTrue", "1");
                                                }
                                                if (rule5.ComparisonOperator != ExcelConditionalFormattingOperator.NoComparison)
                                                {
                                                    writer.WriteAttributeString("operator", rule5.ComparisonOperator.ToString().ToCamelCase());
                                                }
                                                foreach (string str5 in rule5.Formulas)
                                                {
                                                    if (this._isR1C1)
                                                    {
                                                        writer.WriteElementString("formula", this.ConvertR1C1FormulaToA1Formula(str5, sheet, rowColumn.Item2, rowColumn.Item1));
                                                    }
                                                    else
                                                    {
                                                        writer.WriteElementString("formula", str5);
                                                    }
                                                }
                                                continue;
                                            }
                                        }
                                        if (rule is IExcelGeneralRule)
                                        {
                                            IExcelGeneralRule rule6 = rule as IExcelGeneralRule;
                                            using (writer.WriteElement("cfRule"))
                                            {
                                                int type = (int) rule6.Type;
                                                if ((type >= 15) && (type <= 0x18))
                                                {
                                                    writer.WriteAttributeString("type", "timePeriod");
                                                }
                                                else if (((rule6.Type == ExcelConditionalFormatType.BelowAverage) || (rule6.Type == ExcelConditionalFormatType.BelowOrEqualToAverage)) || ((rule6.Type == ExcelConditionalFormatType.AboveAverage) || (rule6.Type == ExcelConditionalFormatType.AboveOrEqualToAverage)))
                                                {
                                                    writer.WriteAttributeString("type", "aboveAverage");
                                                    if ((rule6.Type == ExcelConditionalFormatType.AboveOrEqualToAverage) || (rule6.Type == ExcelConditionalFormatType.BelowOrEqualToAverage))
                                                    {
                                                        rule6.EqualAverage = true;
                                                    }
                                                    if ((rule6.Type == ExcelConditionalFormatType.BelowAverage) || (rule6.Type == ExcelConditionalFormatType.BelowOrEqualToAverage))
                                                    {
                                                        rule6.AboveAverage = false;
                                                    }
                                                }
                                                else if (((rule6.Type == ExcelConditionalFormatType.ContainsText) || (rule6.Type == ExcelConditionalFormatType.NotContainsText)) || ((rule6.Type == ExcelConditionalFormatType.BeginsWith) || (rule6.Type == ExcelConditionalFormatType.EndsWith)))
                                                {
                                                    if (((rule6.Text == null) && (rule6.Formulas != null)) && (rule6.Formulas.Count == 1))
                                                    {
                                                        string str6 = rule6.Formulas[0].ToUpperInvariant();
                                                        if (!string.IsNullOrWhiteSpace(str6))
                                                        {
                                                            int index = str6.IndexOf("SEARCH(");
                                                            if (index != -1)
                                                            {
                                                                int num4 = str6.IndexOf(',', index + 7);
                                                                rule6.Text = str6.Substring(index + 8, (num4 - index) - 9);
                                                            }
                                                            else
                                                            {
                                                                index = str6.IndexOf('=');
                                                                if (index != -1)
                                                                {
                                                                    rule6.Text = str6.Substring(index + 2, (str6.Length - index) - 3);
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (rule6.Operator.HasValue)
                                                    {
                                                        switch (rule6.Operator.Value)
                                                        {
                                                            case ExcelConditionalFormattingOperator.NotContains:
                                                                writer.WriteAttributeString("type", "notContainsText");
                                                                goto Label_0C7F;

                                                            case ExcelConditionalFormattingOperator.EndsWith:
                                                                writer.WriteAttributeString("type", "endsWith");
                                                                goto Label_0C7F;

                                                            case ExcelConditionalFormattingOperator.BeginsWith:
                                                                writer.WriteAttributeString("type", "beginsWith");
                                                                goto Label_0C7F;
                                                        }
                                                        writer.WriteAttributeString("type", "containsText");
                                                    }
                                                    else
                                                    {
                                                        writer.WriteAttributeString("type", "containsText");
                                                    }
                                                }
                                                else
                                                {
                                                    writer.WriteAttributeString("type", rule6.Type.ToString().ToCamelCase());
                                                }
                                            Label_0C7F:
                                                if (rule6.DifferentialFormattingId >= 0)
                                                {
                                                    writer.WriteAttributeString("dxfId", ((int) rule6.DifferentialFormattingId).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                                writer.WriteAttributeString("priority", ((int) rule6.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                if (rule6.StopIfTrue)
                                                {
                                                    writer.WriteAttributeString("stopIfTrue", "1");
                                                }
                                                if (rule6.AboveAverage.HasValue)
                                                {
                                                    if (rule6.AboveAverage.Value)
                                                    {
                                                        writer.WriteAttributeString("aboveAverage", "1");
                                                    }
                                                    else
                                                    {
                                                        writer.WriteAttributeString("aboveAverage", "0");
                                                    }
                                                }
                                                if (rule6.EqualAverage.HasValue)
                                                {
                                                    if (rule6.EqualAverage.Value)
                                                    {
                                                        writer.WriteAttributeString("equalAverage", "1");
                                                    }
                                                    else
                                                    {
                                                        writer.WriteAttributeString("equalAverage", "0");
                                                    }
                                                }
                                                if (rule6.Bottom.HasValue)
                                                {
                                                    if (rule6.Bottom.Value)
                                                    {
                                                        writer.WriteAttributeString("bottom", "1");
                                                    }
                                                    else
                                                    {
                                                        writer.WriteAttributeString("bottom", "0");
                                                    }
                                                }
                                                if (rule6.Percent.HasValue && rule6.Percent.Value)
                                                {
                                                    writer.WriteAttributeString("percent", "1");
                                                }
                                                if (rule6.Operator.HasValue && (((ExcelConditionalFormattingOperator) rule6.Operator.Value) != ExcelConditionalFormattingOperator.NoComparison))
                                                {
                                                    writer.WriteAttributeString("operator", rule6.Operator.Value.ToString().ToCamelCase());
                                                }
                                                if (rule6.Rank.HasValue)
                                                {
                                                    writer.WriteAttributeString("rank", ((int) rule6.Rank.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                                if (rule6.StdDev.HasValue)
                                                {
                                                    writer.WriteAttributeString("stdDev", ((int) rule6.StdDev.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                                if (rule6.Text != null)
                                                {
                                                    writer.WriteAttributeString("text", rule6.Text);
                                                }
                                                if ((type >= 15) && (type <= 0x18))
                                                {
                                                    writer.WriteAttributeString("timePeriod", rule6.Type.ToString().ToCamelCase());
                                                }
                                                if ((((rule6.Type == ExcelConditionalFormatType.Expression) || (rule6.Type == ExcelConditionalFormatType.ContainsText)) || ((rule6.Type == ExcelConditionalFormatType.NotContainsText) || (rule6.Type == ExcelConditionalFormatType.BeginsWith))) || ((((rule6.Type == ExcelConditionalFormatType.EndsWith) || (rule6.Type == ExcelConditionalFormatType.ContainsBlanks)) || ((rule6.Type == ExcelConditionalFormatType.NotContainsBlanks) || (rule6.Type == ExcelConditionalFormatType.ContainsErrors))) || ((rule6.Type == ExcelConditionalFormatType.NotContainsErrors) || ((type >= 15) && (type <= 0x18)))))
                                                {
                                                    foreach (string str7 in rule6.Formulas)
                                                    {
                                                        if (this._isR1C1)
                                                        {
                                                            writer.WriteElementString("formula", this.ConvertR1C1FormulaToA1Formula(str7, sheet, rowColumn.Item2, rowColumn.Item1));
                                                        }
                                                        else
                                                        {
                                                            writer.WriteElementString("formula", str7);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception exception)
                            {
                                this.LogError(ResourceHelper.GetResourceString("writeConditionalFormatError"), ExcelWarningCode.General, sheet, format.Ranges[0].Row, format.Ranges[0].Column, exception);
                            }
                        }
                    }
                }
            }
        }

        private void WriteCustomFilters(XmlWriter writer, IExcelCustomFilters excelCustomFilters)
        {
            if ((excelCustomFilters.Filter1 != null) || (excelCustomFilters.Filter2 != null))
            {
                using (writer.WriteElement("customFilters"))
                {
                    if (excelCustomFilters.And)
                    {
                        writer.WriteAttributeString("and", "1");
                    }
                    if (excelCustomFilters.Filter1 != null)
                    {
                        using (writer.WriteElement("customFilter"))
                        {
                            if (excelCustomFilters.Filter1.Operator != ExcelFilterOperator.None)
                            {
                                writer.WriteAttributeString("operator", excelCustomFilters.Filter1.Operator.ToString().ToCamelCase());
                            }
                            writer.WriteAttributeString("val", Convert.ToString(excelCustomFilters.Filter1.Value, (IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    if (excelCustomFilters.Filter2 != null)
                    {
                        using (writer.WriteElement("customFilter"))
                        {
                            if (excelCustomFilters.Filter2.Operator != ExcelFilterOperator.None)
                            {
                                writer.WriteAttributeString("operator", excelCustomFilters.Filter2.Operator.ToString().ToCamelCase());
                            }
                            writer.WriteAttributeString("val", Convert.ToString(excelCustomFilters.Filter2.Value, (IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }

        private void WriteDataBarExtensionRecord(XmlWriter writer, Tuple<string, IExcelConditionalFormat, IExcelConditionalFormatRule> item)
        {
            IExcelConditionalFormat format = item.Item2;
            writer.WriteStartElement("x14", "conditionalFormatting", null);
            writer.WriteAttributeString("xmlns", "xm", null, "http://schemas.microsoft.com/office/excel/2006/main");
            if (item.Item3 is IExcel2010DataBarRule)
            {
                IExcel2010DataBarRule rule = item.Item3 as IExcel2010DataBarRule;
                writer.WriteStartElement("x14", "cfRule", null);
                writer.WriteAttributeString("type", "dataBar");
                writer.WriteAttributeString("id", item.Item1);
                writer.WriteStartElement("x14", "dataBar", null);
                if (rule.MinimumDataBarLength != 10)
                {
                    writer.WriteAttributeString("minLength", ((byte) rule.MinimumDataBarLength).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (rule.MaximumDataBarLength != 90)
                {
                    writer.WriteAttributeString("maxLength", ((byte) rule.MaximumDataBarLength).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (!rule.ShowValue)
                {
                    writer.WriteAttributeString("showValue", "0");
                }
                if (rule.ShowBorder)
                {
                    writer.WriteAttributeString("border", "1");
                }
                if (!rule.IsGradientDatabar)
                {
                    writer.WriteAttributeString("gradient", "0");
                }
                if (rule.Direction != DataBarDirection.Context)
                {
                    writer.WriteAttributeString("direction", rule.Direction.ToString().ToCamelCase());
                }
                if (rule.NegativeBarColorAsPositive)
                {
                    writer.WriteAttributeString("negativeBarColorSameAsPositive", "1");
                }
                if (!rule.NegativeBorderColorSameAsPositive)
                {
                    writer.WriteAttributeString("negativeBarBorderColorSameAsPositive", "0");
                }
                if (rule.AxisPosition != DataBarAxisPosition.Automatic)
                {
                    writer.WriteAttributeString("axisPosition", rule.AxisPosition.ToString().ToCamelCase());
                }
                if (rule.Minimum.Type == ExcelConditionalFormatValueObjectType.AutoMin)
                {
                    writer.WriteStartElement("x14", "cfvo", null);
                    writer.WriteAttributeString("type", "autoMin");
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("x14", "cfvo", null);
                    writer.WriteAttributeString("type", rule.Minimum.Type.ToString().ToCamelCase());
                    if (!string.IsNullOrEmpty(rule.Minimum.Value))
                    {
                        writer.WriteStartElement("xm", "f", null);
                        writer.WriteValue(rule.Minimum.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (rule.Maximum.Type == ExcelConditionalFormatValueObjectType.AutoMax)
                {
                    writer.WriteStartElement("x14", "cfvo", null);
                    writer.WriteAttributeString("type", "autoMax");
                    writer.WriteEndElement();
                }
                else
                {
                    writer.WriteStartElement("x14", "cfvo", null);
                    writer.WriteAttributeString("type", rule.Maximum.Type.ToString().ToCamelCase());
                    if (!string.IsNullOrEmpty(rule.Maximum.Value))
                    {
                        writer.WriteStartElement("xm", "f", null);
                        writer.WriteValue(rule.Maximum.Value);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                if (rule.ShowBorder && (rule.BorderColor != null))
                {
                    this.WriteColor(writer, "borderColor", "x14", rule.BorderColor);
                }
                if (!rule.NegativeBarColorAsPositive)
                {
                    this.WriteColor(writer, "negativeFillColor", "x14", rule.NegativeFillColor);
                }
                if (!rule.NegativeBorderColorSameAsPositive && rule.ShowBorder)
                {
                    this.WriteColor(writer, "negativeBorderColor", "x14", rule.NegativeBorderColor);
                }
                if (rule.AxisPosition != DataBarAxisPosition.None)
                {
                    this.WriteColor(writer, "axisColor", "x14", rule.AxisColor);
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("xm", "sqref", null);
                this.GetRowColumn(format.Ranges[0]);
                List<string> list = new List<string>();
                foreach (IRange range in Enumerable.Distinct<IRange>((IEnumerable<IRange>) format.Ranges, new RangeComparer()))
                {
                    this.GetRowColumn(range);
                    string str = string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
                    list.Add(str);
                }
                writer.WriteValue(string.Join(" ", (IEnumerable<string>) list));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteDataValidations(XmlWriter writer, short sheet)
        {
            List<IExcelDataValidation> validationData = this._excelWriter.GetValidationData(sheet);
            if ((validationData != null) && (validationData.Count > 0))
            {
                using (writer.WriteElement("dataValidations"))
                {
                    writer.WriteAttributeString("count", ((int) validationData.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    foreach (IExcelDataValidation validation in validationData)
                    {
                        try
                        {
                            if (((validation != null) && (validation.Ranges != null)) && (validation.Ranges.Count != 0))
                            {
                                using (writer.WriteElement("dataValidation"))
                                {
                                    string str = string.Empty;
                                    foreach (IRange range in validation.Ranges)
                                    {
                                        str = str + string.Format("{0}{1}:{2}{3} ", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
                                    }
                                    if (validation.Type != ExcelDataValidationType.None)
                                    {
                                        writer.WriteAttributeString("type", validation.Type.ToString().ToCamelCase());
                                    }
                                    if (validation.ErrorStyle != ExcelDataValidationErrorStyle.Stop)
                                    {
                                        writer.WriteAttributeString("errorStyle", validation.ErrorStyle.ToString().ToCamelCase());
                                    }
                                    if (validation.CompareOperator != ExcelDataValidationOperator.Between)
                                    {
                                        writer.WriteAttributeString("operator", validation.CompareOperator.ToString().ToCamelCase());
                                    }
                                    if (validation.AllowBlank)
                                    {
                                        writer.WriteAttributeString("allowBlank", "1");
                                    }
                                    if (!validation.ShowPromptBox)
                                    {
                                        writer.WriteAttributeString("showDropDown", "1");
                                    }
                                    if (validation.ShowInputMessage)
                                    {
                                        writer.WriteAttributeString("showInputMessage", "1");
                                    }
                                    if (validation.ShowErrorBox)
                                    {
                                        writer.WriteAttributeString("showErrorMessage", "1");
                                    }
                                    if (!string.IsNullOrEmpty(validation.ErrorTitle))
                                    {
                                        writer.WriteAttributeString("errorTitle", validation.ErrorTitle);
                                    }
                                    if (!string.IsNullOrEmpty(validation.Error))
                                    {
                                        writer.WriteAttributeString("error", validation.Error.ToSpecialEncodeForXML());
                                    }
                                    if (!string.IsNullOrEmpty(validation.PromptTitle))
                                    {
                                        writer.WriteAttributeString("promptTitle", validation.PromptTitle);
                                    }
                                    if (!string.IsNullOrEmpty(validation.Prompt))
                                    {
                                        writer.WriteAttributeString("prompt", validation.Prompt.ToSpecialEncodeForXML());
                                    }
                                    writer.WriteAttributeString("sqref", str.TrimEnd(new char[] { ' ' }));
                                    if (this._isR1C1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(validation.FirstFormula))
                                        {
                                            writer.WriteElementString("formula1", this.ConvertR1C1FormulaToA1Formula(validation.FirstFormula.ToString(), sheet, 0, 0));
                                        }
                                        if (!string.IsNullOrWhiteSpace(validation.SecondFormula))
                                        {
                                            writer.WriteElementString("formula2", this.ConvertR1C1FormulaToA1Formula(validation.SecondFormula.ToString(), sheet, 0, 0));
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrWhiteSpace(validation.FirstFormula))
                                        {
                                            writer.WriteElementString("formula1", validation.FirstFormula.ToString());
                                        }
                                        if (!string.IsNullOrWhiteSpace(validation.SecondFormula))
                                        {
                                            writer.WriteElementString("formula2", validation.SecondFormula.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            this.LogError(ResourceHelper.GetResourceString("writeDvError"), ExcelWarningCode.General, sheet, validation.Ranges[0].Row, validation.Ranges[0].Column, exception);
                        }
                    }
                }
            }
        }

        private static void WriteDefaultMajorFonts(XmlWriter writer, string ns)
        {
            KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Jpan"), new KeyValuePair<string, string>("typeface", "ＭＳ Ｐゴシック") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", attributes);
            KeyValuePair<string, string>[] pairArray2 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hang"), new KeyValuePair<string, string>("typeface", "맑은 고딕") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray2);
            KeyValuePair<string, string>[] pairArray3 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hans"), new KeyValuePair<string, string>("typeface", "宋体") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray3);
            KeyValuePair<string, string>[] pairArray4 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hant"), new KeyValuePair<string, string>("typeface", "新細明體") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray4);
            KeyValuePair<string, string>[] pairArray5 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Arab"), new KeyValuePair<string, string>("typeface", "Times New Roman") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray5);
            KeyValuePair<string, string>[] pairArray6 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hebr"), new KeyValuePair<string, string>("typeface", "Times New Roman") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray6);
            KeyValuePair<string, string>[] pairArray7 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Thai"), new KeyValuePair<string, string>("typeface", "Tahoma") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray7);
            KeyValuePair<string, string>[] pairArray8 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Ethi"), new KeyValuePair<string, string>("typeface", "Nyala") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray8);
            KeyValuePair<string, string>[] pairArray9 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Beng"), new KeyValuePair<string, string>("typeface", "Vrinda") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray9);
            KeyValuePair<string, string>[] pairArray10 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Gujr"), new KeyValuePair<string, string>("typeface", "Shruti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray10);
            KeyValuePair<string, string>[] pairArray11 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Khmr"), new KeyValuePair<string, string>("typeface", "MoolBoran") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray11);
            KeyValuePair<string, string>[] pairArray12 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Knda"), new KeyValuePair<string, string>("typeface", "Tunga") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray12);
            KeyValuePair<string, string>[] pairArray13 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Guru"), new KeyValuePair<string, string>("typeface", "Raavi") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray13);
            KeyValuePair<string, string>[] pairArray14 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Cans"), new KeyValuePair<string, string>("typeface", "Euphemia") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray14);
            KeyValuePair<string, string>[] pairArray15 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Cher"), new KeyValuePair<string, string>("typeface", "Plantagenet Cherokee") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray15);
            KeyValuePair<string, string>[] pairArray16 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Yiii"), new KeyValuePair<string, string>("typeface", "Microsoft Yi Baiti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray16);
            KeyValuePair<string, string>[] pairArray17 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Tibt"), new KeyValuePair<string, string>("typeface", "Microsoft Himalaya") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray17);
            KeyValuePair<string, string>[] pairArray18 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Thaa"), new KeyValuePair<string, string>("typeface", "MV Boli") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray18);
            KeyValuePair<string, string>[] pairArray19 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Deva"), new KeyValuePair<string, string>("typeface", "Mangal") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray19);
            KeyValuePair<string, string>[] pairArray20 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Telu"), new KeyValuePair<string, string>("typeface", "Gautami") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray20);
            KeyValuePair<string, string>[] pairArray21 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Taml"), new KeyValuePair<string, string>("typeface", "Latha") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray21);
            KeyValuePair<string, string>[] pairArray22 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Syrc"), new KeyValuePair<string, string>("typeface", "Estrangelo Edessa") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray22);
            KeyValuePair<string, string>[] pairArray23 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Orya"), new KeyValuePair<string, string>("typeface", "Kalinga") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray23);
            KeyValuePair<string, string>[] pairArray24 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Mlym"), new KeyValuePair<string, string>("typeface", "Kartika") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray24);
            KeyValuePair<string, string>[] pairArray25 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Laoo"), new KeyValuePair<string, string>("typeface", "DokChampa") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray25);
            KeyValuePair<string, string>[] pairArray26 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Sinh"), new KeyValuePair<string, string>("typeface", "Iskoola Pota") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray26);
            KeyValuePair<string, string>[] pairArray27 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Mong"), new KeyValuePair<string, string>("typeface", "Mongolian Baiti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray27);
            KeyValuePair<string, string>[] pairArray28 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Viet"), new KeyValuePair<string, string>("typeface", "Times New Roman") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray28);
            KeyValuePair<string, string>[] pairArray29 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Uigh"), new KeyValuePair<string, string>("typeface", "Microsoft Uighur") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray29);
        }

        private static void WriteDefaultMinorFonts(XmlWriter writer, string ns)
        {
            KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Jpan"), new KeyValuePair<string, string>("typeface", "ＭＳ Ｐゴシック") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", attributes);
            KeyValuePair<string, string>[] pairArray2 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hang"), new KeyValuePair<string, string>("typeface", "맑은 고딕") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray2);
            KeyValuePair<string, string>[] pairArray3 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hans"), new KeyValuePair<string, string>("typeface", "宋体") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray3);
            KeyValuePair<string, string>[] pairArray4 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hant"), new KeyValuePair<string, string>("typeface", "新細明體") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray4);
            KeyValuePair<string, string>[] pairArray5 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Arab"), new KeyValuePair<string, string>("typeface", "Arial") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray5);
            KeyValuePair<string, string>[] pairArray6 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Hebr"), new KeyValuePair<string, string>("typeface", "Arial") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray6);
            KeyValuePair<string, string>[] pairArray7 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Thai"), new KeyValuePair<string, string>("typeface", "Tahoma") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray7);
            KeyValuePair<string, string>[] pairArray8 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Ethi"), new KeyValuePair<string, string>("typeface", "Nyala") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray8);
            KeyValuePair<string, string>[] pairArray9 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Beng"), new KeyValuePair<string, string>("typeface", "Vrinda") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray9);
            KeyValuePair<string, string>[] pairArray10 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Gujr"), new KeyValuePair<string, string>("typeface", "Shruti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray10);
            KeyValuePair<string, string>[] pairArray11 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Khmr"), new KeyValuePair<string, string>("typeface", "DaunPenh") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray11);
            KeyValuePair<string, string>[] pairArray12 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Knda"), new KeyValuePair<string, string>("typeface", "Tunga") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray12);
            KeyValuePair<string, string>[] pairArray13 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Guru"), new KeyValuePair<string, string>("typeface", "Raavi") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray13);
            KeyValuePair<string, string>[] pairArray14 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Cans"), new KeyValuePair<string, string>("typeface", "Euphemia") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray14);
            KeyValuePair<string, string>[] pairArray15 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Cher"), new KeyValuePair<string, string>("typeface", "Plantagenet Cherokee") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray15);
            KeyValuePair<string, string>[] pairArray16 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Yiii"), new KeyValuePair<string, string>("typeface", "Microsoft Yi Baiti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray16);
            KeyValuePair<string, string>[] pairArray17 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Tibt"), new KeyValuePair<string, string>("typeface", "Microsoft Himalaya") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray17);
            KeyValuePair<string, string>[] pairArray18 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Thaa"), new KeyValuePair<string, string>("typeface", "MV Boli") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray18);
            KeyValuePair<string, string>[] pairArray19 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Deva"), new KeyValuePair<string, string>("typeface", "Mangal") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray19);
            KeyValuePair<string, string>[] pairArray20 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Telu"), new KeyValuePair<string, string>("typeface", "Gautami") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray20);
            KeyValuePair<string, string>[] pairArray21 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Taml"), new KeyValuePair<string, string>("typeface", "Latha") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray21);
            KeyValuePair<string, string>[] pairArray22 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Syrc"), new KeyValuePair<string, string>("typeface", "Estrangelo Edessa") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray22);
            KeyValuePair<string, string>[] pairArray23 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Orya"), new KeyValuePair<string, string>("typeface", "Kalinga") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray23);
            KeyValuePair<string, string>[] pairArray24 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Mlym"), new KeyValuePair<string, string>("typeface", "Kartika") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray24);
            KeyValuePair<string, string>[] pairArray25 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Laoo"), new KeyValuePair<string, string>("typeface", "DokChampa") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray25);
            KeyValuePair<string, string>[] pairArray26 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Sinh"), new KeyValuePair<string, string>("typeface", "Iskoola Pota") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray26);
            KeyValuePair<string, string>[] pairArray27 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Mong"), new KeyValuePair<string, string>("typeface", "Mongolian Baiti") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray27);
            KeyValuePair<string, string>[] pairArray28 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Viet"), new KeyValuePair<string, string>("typeface", "Arial") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray28);
            KeyValuePair<string, string>[] pairArray29 = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("script", "Uigh"), new KeyValuePair<string, string>("typeface", "Microsoft Uighur") };
            writer.WriteLeafElementWithAttributes("font", ns, "a", pairArray29);
        }

        private static void WriteDefaultNormatStyle(XmlWriter writer)
        {
            using (writer.WriteElement("cellStyles"))
            {
                writer.WriteAttributeString("count", "1");
                using (writer.WriteElement("cellStyle"))
                {
                    writer.WriteAttributeString("name", "Normal");
                    writer.WriteAttributeString("builtinId", "0");
                    writer.WriteAttributeString("xfId", "0");
                }
            }
        }

        private void WriteDrawingFile(MemoryFolder mFolder, Stream stream, string rid, List<IExcelChart> charts, List<IExcelImage> images, IList<string> unsupportedItems, IDictionary<object, int> rids)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            using (@this.WriteDocument(true))
            {
                IDisposable disposable2;
                IDisposable disposable3;
                IDisposable disposable4;
                IDisposable disposable5;
                IDisposable disposable6;
                IDisposable disposable7;
                IDisposable disposable8;
                @this.WriteStartElement("xdr", "wsDr", "http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing");
                @this.WriteAttributeString("xmlns", "a", null, "http://schemas.openxmlformats.org/drawingml/2006/main");
                HashSet<int> keys = new HashSet<int>();
                Regex regex = new Regex("xdr:cNvPr\\s*id=\"(\\d+)\"");
                foreach (string str in unsupportedItems)
                {
                    Match match = regex.Match(str);
                    if ((match.Success && (match.Groups != null)) && (match.Groups.Count == 2))
                    {
                        try
                        {
                            int num = Convert.ToInt32(match.Groups[1].Value);
                            keys.Add(num);
                        }
                        catch
                        {
                        }
                    }
                    @this.WriteRaw(str);
                }
                int nextKey = this.GetNextKey(1, keys);
                if (charts != null)
                {
                    foreach (IExcelChart chart in charts)
                    {
                        ParsingContext.ParsingErrors.Clear();
                        IAnchor anchor = chart.Anchor;
                        if (anchor is AbsoluteAnchor)
                        {
                            AbsoluteAnchor anchor2 = anchor as AbsoluteAnchor;
                            using (disposable2 = @this.WriteElement("absoluteAnchor", null, "xdr"))
                            {
                                using (disposable3 = @this.WriteElement("pos", null, "xdr"))
                                {
                                    @this.WriteAttributeString("x", null, ((int) this.Pixel2EMU(anchor2.X)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteAttributeString("y", null, ((int) this.Pixel2EMU(anchor2.Y)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable4 = @this.WriteElement("ext", null, "xdr"))
                                {
                                    @this.WriteAttributeString("cx", null, ((int) this.Pixel2EMU(anchor2.Height)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteAttributeString("cy", null, ((int) this.Pixel2EMU(anchor2.Width)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable5 = @this.WriteElement("graphicFrame", null, "xdr"))
                                {
                                    @this.WriteAttributeString("macro", "");
                                    using (disposable6 = @this.WriteElement("nvGraphicFramePr", null, "xdr"))
                                    {
                                        using (disposable7 = @this.WriteElement("cNvPr", null, "xdr"))
                                        {
                                            @this.WriteAttributeString("id", ((int) nextKey).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            nextKey = this.GetNextKey(nextKey, keys);
                                            @this.WriteAttributeString("name", chart.Name);
                                            if (chart.Hidden)
                                            {
                                                @this.WriteAttributeString("hidden", "1");
                                            }
                                        }
                                        using (disposable8 = @this.WriteElement("cNvGraphicFramePr", null, "xdr"))
                                        {
                                            using (@this.WriteElement("graphicFrameLocks", null, "a"))
                                            {
                                            }
                                        }
                                    }
                                    using (@this.WriteElement("xfrm", null, "xdr"))
                                    {
                                        using (@this.WriteElement("off", null, "a"))
                                        {
                                            @this.WriteAttributeString("x", "0");
                                            @this.WriteAttributeString("y", "0");
                                        }
                                        using (@this.WriteElement("ext", null, "a"))
                                        {
                                            @this.WriteAttributeString("cx", "0");
                                            @this.WriteAttributeString("cy", "0");
                                        }
                                    }
                                    using (@this.WriteElement("graphic", null, "a"))
                                    {
                                        using (@this.WriteElement("graphicData", null, "a"))
                                        {
                                            @this.WriteAttributeString("uri", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteStartElement("c", "chart", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                                            string str2 = "rId" + ((int) rids[chart]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                            @this.WriteAttributeString("r", "id", null, str2);
                                            @this.WriteEndElement();
                                        }
                                    }
                                }
                                using (@this.WriteElement("clientData", null, "xdr"))
                                {
                                    if (!chart.Locked)
                                    {
                                        @this.WriteAttributeString("fLocksWithSheet", "0");
                                    }
                                }
                            }
                        }
                        if (anchor is OneCellAnchor)
                        {
                            OneCellAnchor anchor3 = anchor as OneCellAnchor;
                            using (@this.WriteElement("oneCellAnchor", null, "xdr"))
                            {
                                using (@this.WriteElement("from", null, "xdr"))
                                {
                                    @this.WriteElementString("xdr", "col", null, ((int) anchor3.FromColumn).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "colOff", null, ((int) this.Pixel2EMU(anchor3.FromColumnOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "row", null, ((int) anchor3.FromRow).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "rowOff", null, ((int) this.Pixel2EMU(anchor3.FromRowOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (@this.WriteElement("ext", null, "xdr"))
                                {
                                    @this.WriteAttributeString("cx", null, ((int) this.Pixel2EMU(anchor3.Height)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteAttributeString("cy", null, ((int) this.Pixel2EMU(anchor3.Width)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (@this.WriteElement("graphicFrame", null, "xdr"))
                                {
                                    @this.WriteAttributeString("macro", "");
                                    using (@this.WriteElement("nvGraphicFramePr", null, "xdr"))
                                    {
                                        using (@this.WriteElement("cNvPr", null, "xdr"))
                                        {
                                            @this.WriteAttributeString("id", ((int) nextKey).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            nextKey = this.GetNextKey(nextKey, keys);
                                            @this.WriteAttributeString("name", chart.Name);
                                            if (chart.Hidden)
                                            {
                                                @this.WriteAttributeString("hidden", "1");
                                            }
                                        }
                                        using (@this.WriteElement("cNvGraphicFramePr", null, "xdr"))
                                        {
                                            using (@this.WriteElement("graphicFrameLocks", null, "a"))
                                            {
                                            }
                                        }
                                    }
                                    using (@this.WriteElement("xfrm", null, "xdr"))
                                    {
                                        using (@this.WriteElement("off", null, "a"))
                                        {
                                            @this.WriteAttributeString("x", "0");
                                            @this.WriteAttributeString("y", "0");
                                        }
                                        using (@this.WriteElement("ext", null, "a"))
                                        {
                                            @this.WriteAttributeString("cx", "0");
                                            @this.WriteAttributeString("cy", "0");
                                        }
                                    }
                                    using (@this.WriteElement("graphic", null, "a"))
                                    {
                                        using (@this.WriteElement("graphicData", null, "a"))
                                        {
                                            @this.WriteAttributeString("uri", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteStartElement("c", "chart", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                                            string str3 = "rId" + ((int) rids[chart]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                            @this.WriteAttributeString("r", "id", null, str3);
                                            @this.WriteEndElement();
                                        }
                                    }
                                }
                                using (disposable2 = @this.WriteElement("clientData", null, "xdr"))
                                {
                                    if (!chart.Locked)
                                    {
                                        @this.WriteAttributeString("fLocksWithSheet", "0");
                                    }
                                }
                            }
                        }
                        if (anchor is TwoCellAnchor)
                        {
                            TwoCellAnchor anchor4 = anchor as TwoCellAnchor;
                            using (disposable2 = @this.WriteElement("twoCellAnchor", null, "xdr"))
                            {
                                using (disposable3 = @this.WriteElement("from", null, "xdr"))
                                {
                                    @this.WriteElementString("xdr", "col", null, ((int) anchor4.FromColumn).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "colOff", null, ((int) this.Pixel2EMU(anchor4.FromColumnOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "row", null, ((int) anchor4.FromRow).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "rowOff", null, ((int) this.Pixel2EMU(anchor4.FromRowOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable3 = @this.WriteElement("to", null, "xdr"))
                                {
                                    @this.WriteElementString("xdr", "col", null, ((int) anchor4.ToColumn).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "colOff", null, ((int) this.Pixel2EMU(anchor4.ToColumnOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "row", null, ((int) anchor4.ToRow).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "rowOff", null, ((int) this.Pixel2EMU(anchor4.ToRowOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable3 = @this.WriteElement("graphicFrame", null, "xdr"))
                                {
                                    @this.WriteAttributeString("macro", "");
                                    using (disposable4 = @this.WriteElement("nvGraphicFramePr", null, "xdr"))
                                    {
                                        using (disposable5 = @this.WriteElement("cNvPr", null, "xdr"))
                                        {
                                            @this.WriteAttributeString("id", ((int) nextKey).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            nextKey = this.GetNextKey(nextKey, keys);
                                            @this.WriteAttributeString("name", chart.Name);
                                            if (chart.Hidden)
                                            {
                                                @this.WriteAttributeString("hidden", "1");
                                            }
                                        }
                                        using (disposable5 = @this.WriteElement("cNvGraphicFramePr", null, "xdr"))
                                        {
                                            using (disposable6 = @this.WriteElement("graphicFrameLocks", null, "a"))
                                            {
                                            }
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("xfrm", null, "xdr"))
                                    {
                                        using (disposable5 = @this.WriteElement("off", null, "a"))
                                        {
                                            @this.WriteAttributeString("x", "0");
                                            @this.WriteAttributeString("y", "0");
                                        }
                                        using (disposable5 = @this.WriteElement("ext", null, "a"))
                                        {
                                            @this.WriteAttributeString("cx", "0");
                                            @this.WriteAttributeString("cy", "0");
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("graphic", null, "a"))
                                    {
                                        using (disposable5 = @this.WriteElement("graphicData", null, "a"))
                                        {
                                            @this.WriteAttributeString("uri", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteStartElement("c", "chart", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                                            @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                                            string str4 = "rId" + ((int) rids[chart]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                            @this.WriteAttributeString("r", "id", null, str4);
                                            @this.WriteEndElement();
                                        }
                                    }
                                }
                                using (disposable3 = @this.WriteElement("clientData", null, "xdr"))
                                {
                                    if (!chart.Locked)
                                    {
                                        @this.WriteAttributeString("fLocksWithSheet", "0");
                                    }
                                }
                            }
                        }
                        this.LogChartErrors(chart.Name);
                    }
                }
                if (images != null)
                {
                    foreach (IExcelImage image in images)
                    {
                        IAnchor anchor5 = image.Anchor;
                        if (anchor5 is TwoCellAnchor)
                        {
                            TwoCellAnchor anchor6 = anchor5 as TwoCellAnchor;
                            using (disposable2 = @this.WriteElement("twoCellAnchor", null, "xdr"))
                            {
                                using (disposable3 = @this.WriteElement("from", null, "xdr"))
                                {
                                    @this.WriteElementString("xdr", "col", null, ((int) anchor6.FromColumn).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "colOff", null, ((int) this.Pixel2EMU(anchor6.FromColumnOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "row", null, ((int) anchor6.FromRow).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "rowOff", null, ((int) this.Pixel2EMU(anchor6.FromRowOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable3 = @this.WriteElement("to", null, "xdr"))
                                {
                                    @this.WriteElementString("xdr", "col", null, ((int) anchor6.ToColumn).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "colOff", null, ((int) this.Pixel2EMU(anchor6.ToColumnOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "row", null, ((int) anchor6.ToRow).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteElementString("xdr", "rowOff", null, ((int) this.Pixel2EMU(anchor6.ToRowOffset)).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                using (disposable3 = @this.WriteElement("pic", null, "xdr"))
                                {
                                    using (disposable4 = @this.WriteElement("nvPicPr", null, "xdr"))
                                    {
                                        using (disposable5 = @this.WriteElement("cNvPr", null, "xdr"))
                                        {
                                            @this.WriteAttributeString("id", ((int) nextKey).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            nextKey = this.GetNextKey(nextKey, keys);
                                            @this.WriteAttributeString("name", image.Name);
                                            if (image.Hidden)
                                            {
                                                @this.WriteAttributeString("hidden", "1");
                                            }
                                        }
                                        using (disposable5 = @this.WriteElement("cNvPicPr", null, "xdr"))
                                        {
                                            using (disposable6 = @this.WriteElement("picLocks", null, "a"))
                                            {
                                                @this.WriteAttributeString("noChangeAspect", "1");
                                            }
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("blipFill", null, "xdr"))
                                    {
                                        using (disposable5 = @this.WriteElement("blip", null, "a"))
                                        {
                                            @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                                            string str5 = "rId" + ((int) rids[image]).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                            @this.WriteAttributeString("r", "embed", null, str5);
                                            @this.WriteAttributeString("cstate", "print");
                                            using (disposable6 = @this.WriteElement("extLst", null, "a"))
                                            {
                                                using (disposable7 = @this.WriteElement("ext", null, "a"))
                                                {
                                                    @this.WriteAttributeString("uri", "{28A0092B-C50C-407E-A947-70E740481C1C}");
                                                    using (disposable8 = @this.WriteElement("useLocalDpi", "http://schemas.microsoft.com/office/drawing/2010/main", "a14"))
                                                    {
                                                        @this.WriteAttributeString("val", "0");
                                                    }
                                                }
                                            }
                                        }
                                        using (disposable5 = @this.WriteElement("stretch", null, "a"))
                                        {
                                            using (disposable6 = @this.WriteElement("fillRect", null, "a"))
                                            {
                                            }
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("spPr", null, "xdr"))
                                    {
                                        using (disposable5 = @this.WriteElement("xfrm", null, "a"))
                                        {
                                            using (disposable6 = @this.WriteElement("off", null, "a"))
                                            {
                                                @this.WriteAttributeString("x", "0");
                                                @this.WriteAttributeString("y", "0");
                                            }
                                            using (disposable6 = @this.WriteElement("ext", null, "a"))
                                            {
                                                @this.WriteAttributeString("cx", "0");
                                                @this.WriteAttributeString("cy", "0");
                                            }
                                        }
                                        using (disposable5 = @this.WriteElement("prstGeom", null, "a"))
                                        {
                                            @this.WriteAttributeString("prst", "rect");
                                            using (disposable6 = @this.WriteElement("avLst", null, "a"))
                                            {
                                            }
                                        }
                                        if ((image.PictureFormat != null) && (image.PictureFormat.LineFormat != null))
                                        {
                                            (image.PictureFormat.LineFormat as LineFormat).WriteXml(@this, mFolder, null);
                                        }
                                    }
                                }
                                using (disposable3 = @this.WriteElement("clientData", null, "xdr"))
                                {
                                    if (!image.Locked)
                                    {
                                        @this.WriteAttributeString("fLocksWithSheet", "0");
                                    }
                                }
                            }
                        }
                    }
                }
                @this.WriteEndElement();
            }
            @this.Flush();
        }

        private void WriteDynamicFilter(XmlWriter writer, IExcelDynamicFilter dynamicFilter)
        {
            using (writer.WriteElement("dynamicFilter"))
            {
                if (dynamicFilter.Type != ExcelDynamicFilterType.Null)
                {
                    if (dynamicFilter.Type.ToString().StartsWith("Q") || dynamicFilter.Type.ToString().StartsWith("M"))
                    {
                        writer.WriteAttributeString("type", dynamicFilter.Type.ToString());
                    }
                    else
                    {
                        writer.WriteAttributeString("type", dynamicFilter.Type.ToString().ToCamelCase());
                    }
                }
                else
                {
                    writer.WriteAttributeString("type", "null");
                }
                if (((dynamicFilter.Value != null) && !dynamicFilter.Type.ToString().StartsWith("Q")) && !dynamicFilter.Type.ToString().StartsWith("M"))
                {
                    writer.WriteAttributeString("val", Convert.ToString(dynamicFilter.Value, (IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (((dynamicFilter.MaxValue != null) && !dynamicFilter.Type.ToString().StartsWith("Q")) && !dynamicFilter.Type.ToString().StartsWith("M"))
                {
                    writer.WriteAttributeString("maxVal", Convert.ToString(dynamicFilter.MaxValue, (IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        private void WriteExcelChart(XFile chartFile, MemoryFolder mFolder, Stream stream, string rid, IExcelChart chart, int sheet)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            using (@this.WriteDocument(true))
            {
                @this.WriteStartElement("c", "chartSpace", "http://schemas.openxmlformats.org/drawingml/2006/chart");
                @this.WriteAttributeString("xmlns", "a", null, "http://schemas.openxmlformats.org/drawingml/2006/main");
                @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                bool flag1 = this._excelWriter.GetWorkbookProperty().IsDate1904;
                @this.WriteLeafElementWithAttribute("date1904", null, "c", "val", "0");
                @this.WriteLeafElementWithAttribute("lang", null, "c", "val", "en-US");
                @this.WriteLeafElementWithAttribute("roundedCorners", null, "c", "val", chart.RoundedCorners ? "1" : "0");
                if (chart.DefaultStyleIndex != -1)
                {
                    @this.WriteLeafElementWithAttribute("style", null, "c", "val", ((int) chart.DefaultStyleIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                else
                {
                    @this.WriteStartElement("mc", "AlternateContent", "http://schemas.openxmlformats.org/markup-compatibility/2006");
                    if ((chart.AlternateContentChoiceStyleList != null) && (chart.AlternateContentChoiceStyleList.Count > 0))
                    {
                        foreach (int num in chart.AlternateContentChoiceStyleList)
                        {
                            WriteChoice(@this, num);
                        }
                    }
                    else
                    {
                        WriteChoice(@this, 0x66);
                    }
                    if ((chart.AlternateFallbackStyleList != null) || (chart.AlternateFallbackStyleList.Count > 0))
                    {
                        foreach (int num2 in chart.AlternateFallbackStyleList)
                        {
                            using (@this.WriteElement("Fallback", null, "mc"))
                            {
                                @this.WriteLeafElementWithAttribute("style", null, "c", "val", ((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                    }
                    else
                    {
                        using (@this.WriteElement("Fallback", null, "mc"))
                        {
                            @this.WriteLeafElementWithAttribute("style", null, "c", "val", "2");
                        }
                    }
                    @this.WriteEndElement();
                }
                (chart as ExcelChart).WriteXml(@this, mFolder, chartFile);
                if (chart.ChartFormat != null)
                {
                    (chart.ChartFormat as ExcelChartFormat).WriteXml(@this, mFolder, chartFile);
                }
                if (chart.TextFormat != null)
                {
                    (chart.TextFormat as ExcelTextFormat).WriteXml(@this, mFolder, chartFile);
                }
                using (@this.WriteElement("printSettings", null, "c"))
                {
                    using (@this.WriteElement("headerFooter", null, "c"))
                    {
                    }
                    IExcelPrintPageMargin printPageMargin = this._excelWriter.GetPrintPageMargin((short) sheet);
                    if (printPageMargin != null)
                    {
                        using (@this.WriteElement("pageMargins", null, "c"))
                        {
                            @this.WriteAttributeString("b", ((double) printPageMargin.Bottom).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            @this.WriteAttributeString("l", ((double) printPageMargin.Left).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            @this.WriteAttributeString("r", ((double) printPageMargin.Right).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            @this.WriteAttributeString("t", ((double) printPageMargin.Top).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            @this.WriteAttributeString("header", ((double) printPageMargin.Header).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            @this.WriteAttributeString("footer", ((double) printPageMargin.Footer).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    using (@this.WriteElement("pageSetup", null, "c"))
                    {
                    }
                }
                @this.WriteEndElement();
            }
            @this.Flush();
        }

        private void WriteExcelStyleFormats(Stream stream)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            List<IExtendedFormat> excelCellFormats = this._excelWriter.GetExcelCellFormats();
            IExtendedFormat excelDefaultCellFormat = this._excelWriter.GetExcelDefaultCellFormat();
            List<IExcelStyle> excelStyles = this._excelWriter.GetExcelStyles();
            if ((excelStyles == null) || (excelStyles.Count == 0))
            {
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
            string normal = "Normal";
            if (excelStyles != null)
            {
                foreach (IExcelStyle style2 in excelStyles)
                {
                    if (style2.IsBuiltInStyle)
                    {
                        ExcelStyle style3 = style2 as ExcelStyle;
                        if (style3.BuiltInStyle == BuiltInStyleIndex.Normal)
                        {
                            normal = style3.Name;
                        }
                    }
                }
            }
            IExcelStyle style4 = Enumerable.FirstOrDefault<IExcelStyle>((IEnumerable<IExcelStyle>) excelStyles, delegate (IExcelStyle item) {
                return item.Name == normal;
            });
            IExtendedFormat format2 = Enumerable.FirstOrDefault<IExtendedFormat>((IEnumerable<IExtendedFormat>) excelCellFormats, delegate (IExtendedFormat item) {
                return !item.IsStyleFormat;
            });
            IExtendedFormat format3 = Enumerable.FirstOrDefault<IExtendedFormat>((IEnumerable<IExtendedFormat>) excelCellFormats, delegate (IExtendedFormat item) {
                return item.IsStyleFormat;
            });
            if ((format2 == null) || !format2.Equals(excelDefaultCellFormat))
            {
                excelCellFormats.Insert(0, excelDefaultCellFormat);
                this._styleOffset++;
            }
            if ((format3 == null) || !format3.Equals(style4.Format))
            {
                excelCellFormats.Insert(0, style4.Format);
                this._styleOffset++;
            }
            int index = 0;
            int numberFormatId = 0;
            int num3 = 0;
            int num4 = 0;
            this._styleFonts.Add(style4.Format.Font);
            this._styleFills.Add(new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.None, ExcelColor.EmptyColor, ExcelColor.EmptyColor));
            this._styleFills.Add(new Tuple<FillPatternType, IExcelColor, IExcelColor>(FillPatternType.Gray125, ExcelColor.EmptyColor, ExcelColor.EmptyColor));
            this._styleBorders.Add(new ExcelBorder());
            List<Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>> list3 = new List<Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>>();
            this._styleXfs = new List<Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>>();
            int num5 = 0;
            List<IDifferentialFormatting> differentialFormattingRecords = this._excelWriter.GetDifferentialFormattingRecords();
            for (int i = 0; i < excelCellFormats.Count; i++)
            {
                IExtendedFormat format4 = excelCellFormats[i];
                if (format4.Font != null)
                {
                    index = this._styleFonts.IndexOf(format4.Font);
                    if (index == -1)
                    {
                        this._styleFonts.Add(format4.Font);
                        index = this._styleFonts.Count - 1;
                    }
                }
                if (format4.NumberFormat != null)
                {
                    numberFormatId = format4.NumberFormat.NumberFormatId;
                    this._numberFormats[format4.NumberFormat.NumberFormatId] = format4.NumberFormat.NumberFormatCode;
                }
                else if (format4.NumberFormatIndex >= 0)
                {
                    numberFormatId = format4.NumberFormatIndex;
                }
                if (format4.Border != null)
                {
                    num4 = this._styleBorders.IndexOf(format4.Border);
                    if (num4 == -1)
                    {
                        this._styleBorders.Add(format4.Border);
                        num4 = this._styleBorders.Count - 1;
                    }
                }
                if (format4.FillPattern == FillPatternType.None)
                {
                    num3 = 0;
                }
                else
                {
                    Tuple<FillPatternType, IExcelColor, IExcelColor> tuple = new Tuple<FillPatternType, IExcelColor, IExcelColor>(format4.FillPattern, (format4.PatternColor != null) ? format4.PatternColor : ExcelColor.EmptyColor, (format4.PatternBackgroundColor != null) ? format4.PatternBackgroundColor : ExcelColor.EmptyColor);
                    num3 = this._styleFills.IndexOf(tuple);
                    if (num3 == -1)
                    {
                        this._styleFills.Add(tuple);
                        num3 = this._styleFills.Count - 1;
                    }
                }
                if (format4.IsStyleFormat)
                {
                    this._xfMap[num5++] = ((int) this._styleXfs.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                    this._styleXfs.Add(new Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>(numberFormatId, index, num3, num4, 0, format4, new Tuple<bool, bool>(format4.IsHidden, format4.IsLocked)));
                }
                else
                {
                    this._xfMap[num5++] = ((int) list3.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                    list3.Add(new Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>(numberFormatId, index, num3, num4, 0, format4, new Tuple<bool, bool>(format4.IsHidden, format4.IsLocked)));
                }
            }
            using (@this.WriteDocument(true))
            {
                using (@this.WriteElement("styleSheet", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                {
                    IDisposable disposable3;
                    IDisposable disposable4;
                    if (this._numberFormats.Count > 0)
                    {
                        using (disposable3 = @this.WriteElement("numFmts"))
                        {
                            @this.WriteAttributeString("count", ((int) this._numberFormats.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            foreach (KeyValuePair<int, string> pair in this._numberFormats)
                            {
                                this.WriteNumFmt(@this, pair);
                            }
                        }
                    }
                    using (disposable4 = @this.WriteElement("fonts"))
                    {
                        @this.WriteAttributeString("count", ((int) this._styleFonts.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        foreach (IExcelFont font in this._styleFonts)
                        {
                            this.WriteFont(@this, font);
                        }
                    }
                    using (@this.WriteElement("fills"))
                    {
                        @this.WriteAttributeString("count", ((int) this._styleFills.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        using (@this.WriteElement("fill"))
                        {
                            @this.WriteLeafElementWithAttribute("patternFill", "patternType", "none");
                        }
                        using (@this.WriteElement("fill"))
                        {
                            @this.WriteLeafElementWithAttribute("patternFill", "patternType", "gray125");
                        }
                        for (int j = 2; j < this._styleFills.Count; j++)
                        {
                            this.WriteFill(@this, this._styleFills[j]);
                        }
                    }
                    using (@this.WriteElement("borders"))
                    {
                        @this.WriteAttributeString("count", ((int) this._styleBorders.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        foreach (IExcelBorder border in this._styleBorders)
                        {
                            this.WriteBorder(@this, border);
                        }
                    }
                    this.WriteXFs(@this, this._styleXfs, "cellStyleXfs", true);
                    this.WriteXFs(@this, list3, "cellXfs", false);
                    if (excelStyles.Count == 0)
                    {
                        WriteDefaultNormatStyle(@this);
                    }
                    else
                    {
                        List<Tuple<int, IExcelStyle>> list5 = new List<Tuple<int, IExcelStyle>>();
                        HashSet<int> usedId = new HashSet<int>();
                        foreach (IExcelStyle style5 in excelStyles)
                        {
                            int xfId = this.GetXfId(style5.Format, usedId);
                            if (xfId != -1)
                            {
                                list5.Add(new Tuple<int, IExcelStyle>(xfId, style5));
                            }
                        }
                        if (list5.Count == 0)
                        {
                            WriteDefaultNormatStyle(@this);
                        }
                        else
                        {
                            using (disposable3 = @this.WriteElement("cellStyles"))
                            {
                                @this.WriteAttributeString("count", ((int) list5.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                foreach (Tuple<int, IExcelStyle> tuple2 in list5)
                                {
                                    ExcelStyle style6 = tuple2.Item2 as ExcelStyle;
                                    if (style6 != null)
                                    {
                                        using (disposable4 = @this.WriteElement("cellStyle"))
                                        {
                                            @this.WriteAttributeString("name", style6.Name);
                                            @this.WriteAttributeString("xfId", ((int) tuple2.Item1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            if (style6.IsBuiltInStyle)
                                            {
                                                @this.WriteAttributeString("builtinId", ((int) style6.BuiltInStyle).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                if (style6.IsCustomBuiltIn)
                                                {
                                                    @this.WriteAttributeString("customBuiltin", "1");
                                                }
                                                if ((style6.BuiltInStyle == BuiltInStyleIndex.RowLevel) || (style6.BuiltInStyle == BuiltInStyleIndex.ColumnLevel))
                                                {
                                                    @this.WriteAttributeString("iLevel", ((byte) style6.OutLineLevel).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                                }
                                            }
                                            continue;
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("cellStyle"))
                                    {
                                        @this.WriteAttributeString("name", tuple2.Item2.Name);
                                        @this.WriteAttributeString("xfId", ((int) tuple2.Item1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                        }
                    }
                    if ((differentialFormattingRecords != null) && (differentialFormattingRecords.Count > 0))
                    {
                        using (disposable3 = @this.WriteElement("dxfs"))
                        {
                            @this.WriteAttributeString("count", ((int) differentialFormattingRecords.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            foreach (IDifferentialFormatting formatting in differentialFormattingRecords)
                            {
                                using (disposable4 = @this.WriteElement("dxf"))
                                {
                                    if (formatting.Font != null)
                                    {
                                        this.WriteFont(@this, formatting.Font);
                                    }
                                    if ((formatting.NumberFormat != null) || (formatting.FormatId >= 0))
                                    {
                                        int key = -1;
                                        string numberFormatCode = null;
                                        if (formatting.NumberFormat != null)
                                        {
                                            numberFormatCode = formatting.NumberFormat.NumberFormatCode;
                                            key = formatting.NumberFormat.NumberFormatId;
                                            if (key < 0)
                                            {
                                                for (int k = 0xa6; k < 0x3e8; k++)
                                                {
                                                    if (!this._numberFormats.ContainsKey(k))
                                                    {
                                                        this._numberFormats.Add(k, formatting.NumberFormat.NumberFormatCode);
                                                        key = k;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else if (formatting.FormatId >= 0)
                                        {
                                            key = formatting.FormatId;
                                            if (this._numberFormats.ContainsKey(key))
                                            {
                                                numberFormatCode = this._numberFormats[key];
                                            }
                                            else
                                            {
                                                numberFormatCode = ExtendedFormatHelper.GetFormatCode(key);
                                                if (numberFormatCode == null)
                                                {
                                                    numberFormatCode = "General";
                                                    key = 0;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            key = 0;
                                            numberFormatCode = "General";
                                        }
                                        this.WriteNumFmt(@this, new KeyValuePair<int, string>(key, numberFormatCode));
                                    }
                                    if (formatting.Fill != null)
                                    {
                                        this.WriteFill(@this, formatting.Fill);
                                    }
                                    if (formatting.Border != null)
                                    {
                                        this.WriteBorder(@this, formatting.Border);
                                    }
                                    if (formatting.IsHidden || formatting.IsLocked)
                                    {
                                        KeyValuePair<string, string>[] attributes = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("hidden", formatting.IsHidden ? "1" : "0"), new KeyValuePair<string, string>("locked", formatting.IsLocked ? "1" : "0") };
                                        @this.WriteLeafElementWithAttributes("protection", attributes);
                                    }
                                }
                            }
                            goto Label_0B82;
                        }
                    }
                    @this.WriteLeafElementWithAttribute("dxfs", "count", "0");
                Label_0B82:
                    if (this._excelWriter is IExcelTableWriter)
                    {
                        IExcelTableWriter writer2 = this._excelWriter as IExcelTableWriter;
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
                        using (disposable3 = @this.WriteElement("tableStyles"))
                        {
                            List<IExcelTableStyle> tableStyles = writer2.GetTableStyles();
                            if ((tableStyles != null) && (tableStyles.Count > 0))
                            {
                                @this.WriteAttributeString("count", ((int) tableStyles.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            @this.WriteAttributeString("defaultTableStyle", defaultTableStyleName);
                            @this.WriteAttributeString("defaultPivotStyle", defaultPivotTableStyleName);
                            if ((tableStyles != null) && (tableStyles.Count > 0))
                            {
                                foreach (IExcelTableStyle style7 in tableStyles)
                                {
                                    WriteTableStyle(@this, style7);
                                }
                            }
                        }
                    }
                    Dictionary<int, GcColor> colorPalette = this._excelWriter.GetColorPalette();
                    if ((colorPalette != null) && (colorPalette.Count == 0x40))
                    {
                        List<GcColor> list7 = Enumerable.ToList<GcColor>(Enumerable.Select<Tuple<int, GcColor>, GcColor>((IEnumerable<Tuple<int, GcColor>>) Enumerable.OrderBy<Tuple<int, GcColor>, int>(Enumerable.Select<KeyValuePair<int, GcColor>, Tuple<int, GcColor>>((IEnumerable<KeyValuePair<int, GcColor>>) colorPalette, delegate (KeyValuePair<int, GcColor> item) {
                            return new Tuple<int, GcColor>(item.Key, item.Value);
                        }), delegate (Tuple<int, GcColor> item) {
                            return item.Item1;
                        }), delegate (Tuple<int, GcColor> item) {
                            return item.Item2;
                        }));
                        using (disposable3 = @this.WriteElement("colors"))
                        {
                            using (disposable4 = @this.WriteElement("indexedColors"))
                            {
                                foreach (GcColor color in list7)
                                {
                                    @this.WriteLeafElementWithAttribute("rgbColor", "rgb", color.ToString());
                                }
                            }
                        }
                    }
                }
            }
            @this.Flush();
        }

        private void WriteExtensionCondtionalFormats(XmlWriter writer, short sheet)
        {
            if (((this._extensionIconSetConditionalFormats != null) && (this._extensionIconSetConditionalFormats.Count != 0)) || ((this._extensionDataBarCondtionalFormats != null) && (this._extensionDataBarCondtionalFormats.Count != 0)))
            {
                using (writer.WriteElement("ext"))
                {
                    writer.WriteAttributeString("xmlns", "x14", null, "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
                    writer.WriteAttributeString("uri", "{78C0D931-6437-407d-A8EE-F0AAD7539E65}");
                    writer.WriteStartElement("x14", "conditionalFormattings", null);
                    if (this._extensionIconSetConditionalFormats != null)
                    {
                        foreach (Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule> tuple in this._extensionIconSetConditionalFormats)
                        {
                            this.WriteIconSetExtensionRecord(writer, tuple);
                        }
                    }
                    if (this._extensionDataBarCondtionalFormats != null)
                    {
                        foreach (Tuple<string, IExcelConditionalFormat, IExcelConditionalFormatRule> tuple2 in this._extensionDataBarCondtionalFormats)
                        {
                            this.WriteDataBarExtensionRecord(writer, tuple2);
                        }
                    }
                    writer.WriteEndElement();
                }
            }
        }

        private void WriteExtensions(XmlWriter writer, short sheet)
        {
            if (this.NeedWriteExtension(writer, sheet))
            {
                using (writer.WriteElement("extLst"))
                {
                    this.WriteExtensionCondtionalFormats(writer, sheet);
                    this.WriteSparklineGroups(writer, sheet);
                }
            }
        }

        private void WriteFill(XmlWriter writer, Tuple<FillPatternType, IExcelColor, IExcelColor> fill)
        {
            using (writer.WriteElement("fill"))
            {
                this._excelWriter.GetColorPalette();
                using (writer.WriteElement("patternFill"))
                {
                    if (((FillPatternType) fill.Item1) != FillPatternType.None)
                    {
                        writer.WriteAttributeString("patternType", fill.Item1.ToString().ToCamelCase());
                    }
                    if ((fill.Item2 != null) && (fill.Item2 != ExcelColor.EmptyColor))
                    {
                        this.WriteColor(writer, "fgColor", fill.Item2);
                    }
                    if ((fill.Item3 != null) && (fill.Item3 != ExcelColor.EmptyColor))
                    {
                        this.WriteColor(writer, "bgColor", fill.Item3);
                    }
                }
            }
        }

        private void WriteFilters(XmlWriter writer, IExcelFilters excelFilters)
        {
            using (writer.WriteElement("filters"))
            {
                if (excelFilters.Blank)
                {
                    writer.WriteAttributeString("blank", "1");
                }
                if (excelFilters.CalendarType != ExcelCalendarType.None)
                {
                    writer.WriteAttributeString("calendarType", excelFilters.CalendarType.ToString().ToCamelCase());
                }
                if (excelFilters.Filter != null)
                {
                    foreach (string str in excelFilters.Filter)
                    {
                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            writer.WriteLeafElementWithAttribute("filter", "val", str);
                        }
                    }
                }
                if (excelFilters.DateGroupItem != null)
                {
                    foreach (IExcelDateGroupItem item in excelFilters.DateGroupItem)
                    {
                        using (writer.WriteElement("dateGroupItem"))
                        {
                            if (item.Year > 0)
                            {
                                writer.WriteAttributeString("year", ((ushort) item.Year).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if ((item.Month > 0) && (item.Month < 13))
                            {
                                writer.WriteAttributeString("month", ((ushort) item.Month).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if ((item.Day > 0) && (item.Day < 0x20))
                            {
                                writer.WriteAttributeString("day", ((ushort) item.Day).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (item.Hour < 0x18)
                            {
                                writer.WriteAttributeString("hour", ((ushort) item.Hour).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (item.Minute < 60)
                            {
                                writer.WriteAttributeString("minute", ((ushort) item.Minute).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (item.Second < 60)
                            {
                                writer.WriteAttributeString("second", ((ushort) item.Second).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            writer.WriteAttributeString("dateTimeGrouping", item.DateTimeGrouping.ToString().ToCamelCase());
                        }
                    }
                }
            }
        }

        private void WriteFont(XmlWriter writer, IExcelFont font)
        {
            if (font != null)
            {
                using (writer.WriteElement("font"))
                {
                    if (font.IsBold)
                    {
                        writer.WriteLeafElement("b");
                    }
                    if (font.IsItalic)
                    {
                        writer.WriteLeafElement("i");
                    }
                    if (font.IsStrikeOut)
                    {
                        writer.WriteLeafElement("strike");
                    }
                    if (font.UnderLineStyle != UnderLineStyle.None)
                    {
                        if (font.UnderLineStyle == UnderLineStyle.Single)
                        {
                            writer.WriteLeafElement("u");
                        }
                        else
                        {
                            writer.WriteLeafElementWithAttribute("u", "val", font.UnderLineStyle.ToString().ToCamelCase());
                        }
                    }
                    if (font.VerticalAlignRun != VerticalAlignRun.BaseLine)
                    {
                        writer.WriteLeafElementWithAttribute("vertAlign", "val", font.VerticalAlignRun.ToString().ToCamelCase());
                    }
                    if ((font.FontSize != 0.0) || (font.FontSize != -1.0))
                    {
                        double fontSize = font.FontSize;
                        if (fontSize > 0.0)
                        {
                            writer.WriteLeafElementWithAttribute("sz", "val", ((double) fontSize).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    if ((font.FontColor != null) && (font.FontColor != ExcelColor.EmptyColor))
                    {
                        this.WriteColor(writer, "color", font.FontColor);
                    }
                    if (!string.IsNullOrWhiteSpace(font.FontName))
                    {
                        writer.WriteLeafElementWithAttribute("name", "val", font.FontName);
                    }
                    if (font.FontFamily != ExcelFontFamily.Auto)
                    {
                        byte fontFamily = (byte) font.FontFamily;
                        writer.WriteLeafElementWithAttribute("family", "val", ((byte) fontFamily).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (font.CharSetIndex > 0)
                    {
                        writer.WriteLeafElementWithAttribute("charset", "val", ((byte) font.CharSetIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (font.IsShadowStyle)
                    {
                        writer.WriteLeafElementWithAttribute("shadow", "val", "1");
                    }
                    if (font.IsOutlineStyle)
                    {
                        writer.WriteLeafElementWithAttribute("outline", "val", "1");
                    }
                    if (font.FontScheme != FontSchemeCategory.None)
                    {
                        writer.WriteLeafElementWithAttribute("scheme", "val", font.FontScheme.ToString().ToCamelCase());
                    }
                }
            }
        }

        private void WriteHeadFooter(XmlWriter writer, short sheet)
        {
            IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheet);
            if ((printPageSetting != null) && this.NeedWriteHeaderFooter(printPageSetting))
            {
                using (writer.WriteElement("headerFooter"))
                {
                    if (printPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentOddEvenPages)
                    {
                        writer.WriteAttributeString("differentOddEven", "1");
                    }
                    if (printPageSetting.AdvancedHeadFooterSetting.HeaderFooterDifferentFirstPage)
                    {
                        writer.WriteAttributeString("differentFirst", "1");
                    }
                    if (!printPageSetting.AdvancedHeadFooterSetting.HeaderFooterScalesWithDocument)
                    {
                        writer.WriteAttributeString("scaleWithDoc", "0");
                    }
                    if (!printPageSetting.AdvancedHeadFooterSetting.HeaderFooterAlignWithPageMargin)
                    {
                        writer.WriteAttributeString("alignWithMargins", "0");
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.HeaderOddPage))
                    {
                        writer.WriteElementString("oddHeader", printPageSetting.AdvancedHeadFooterSetting.HeaderOddPage);
                    }
                    else if (!string.IsNullOrWhiteSpace(printPageSetting.Header))
                    {
                        writer.WriteElementString("oddHeader", printPageSetting.Header);
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.FooterOddPage))
                    {
                        writer.WriteElementString("oddFooter", printPageSetting.AdvancedHeadFooterSetting.FooterOddPage);
                    }
                    else if (!string.IsNullOrWhiteSpace(printPageSetting.Footer))
                    {
                        writer.WriteElementString("oddFooter", printPageSetting.Footer);
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.HeaderEvenPage))
                    {
                        writer.WriteElementString("evenHeader", printPageSetting.AdvancedHeadFooterSetting.HeaderEvenPage);
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.FooterEvenPage))
                    {
                        writer.WriteElementString("evenFooter", printPageSetting.AdvancedHeadFooterSetting.FooterEvenPage);
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.HeaderFirstPage))
                    {
                        writer.WriteElementString("firstHeader", printPageSetting.AdvancedHeadFooterSetting.HeaderFirstPage);
                    }
                    if (!string.IsNullOrWhiteSpace(printPageSetting.AdvancedHeadFooterSetting.FooterFirstPage))
                    {
                        writer.WriteElementString("firstFooter", printPageSetting.AdvancedHeadFooterSetting.FooterFirstPage);
                    }
                }
            }
        }

        private void WriteHyperLink(XmlWriter writer, short sheet)
        {
            if ((this._cellsHasHyperlink != null) && (this._cellsHasHyperlink.Count > 0))
            {
                string ns = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
                using (writer.WriteElement("hyperlinks"))
                {
                    int num = 0;
                    if (this._excelWriter is IExcelTableWriter)
                    {
                        List<IExcelTable> sheetTables = (this._excelWriter as IExcelTableWriter).GetSheetTables(sheet);
                        if (sheetTables != null)
                        {
                            num = sheetTables.Count;
                        }
                    }
                    int num2 = 1 + num;
                    foreach (IExcelCell cell in this._cellsHasHyperlink)
                    {
                        using (writer.WriteElement("hyperlink"))
                        {
                            object[] args = new object[2];
                            args[0] = this.TryGetColumnIndexInA1LetterForm(cell.Column);
                            int num3 = cell.Row + 1;
                            args[1] = ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                            writer.WriteAttributeString("ref", string.Format("{0}{1}", args));
                            writer.WriteAttributeString("r", "id", ns, "rId" + ((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            if (cell.Hyperlink.Description != cell.Hyperlink.Address)
                            {
                                writer.WriteAttributeString("display", cell.Hyperlink.Description);
                            }
                            num2++;
                        }
                    }
                }
            }
        }

        private void WriteIconFilter(XmlWriter writer, IExcelIconFilter excelIconFilter)
        {
            using (writer.WriteElement("iconFilter"))
            {
                writer.WriteAttributeString("iconSet", excelIconFilter.IconSet.ToString().Substring(5));
                if (!excelIconFilter.NoIcon)
                {
                    writer.WriteAttributeString("iconId", ((uint) excelIconFilter.IconId).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        private void WriteIconSetExtensionRecord(XmlWriter writer, Tuple<IExcelConditionalFormat, IExcelConditionalFormatRule> item)
        {
            IExcelConditionalFormat format = item.Item1;
            writer.WriteStartElement("x14", "conditionalFormatting", null);
            writer.WriteAttributeString("xmlns", "xm", null, "http://schemas.microsoft.com/office/excel/2006/main");
            if (item.Item2 is ExcelIconSetsRule)
            {
                ExcelIconSetsRule rule = item.Item2 as ExcelIconSetsRule;
                writer.WriteStartElement("x14", "cfRule", null);
                writer.WriteAttributeString("type", "iconSet");
                writer.WriteAttributeString("priority", ((int) rule.Priority).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteStartElement("x14", "iconSet", null);
                if (rule.IconSet != ExcelIconSetType.Icon_NIL)
                {
                    writer.WriteAttributeString("iconSet", rule.IconSet.ToString().Substring(5));
                }
                if (rule.IconOnly)
                {
                    writer.WriteAttributeString("showValue", "0");
                }
                if (rule.ReversedOrder)
                {
                    writer.WriteAttributeString("reverse", "1");
                }
                for (int i = 0; i < rule.Thresholds.Count; i++)
                {
                    IExcelConditionalFormatValueObject obj2 = rule.Thresholds[i];
                    writer.WriteStartElement("x14", "cfvo", null);
                    writer.WriteAttributeString("type", obj2.Type.ToString().ToCamelCase());
                    if (rule.NotPassTheThresholdsWhenEquals[i])
                    {
                        writer.WriteAttributeString("gte", "0");
                    }
                    writer.WriteStartElement("xm", "f", null);
                    writer.WriteValue(obj2.Value.ToString());
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteStartElement("xm", "sqref", null);
                this.GetRowColumn(format.Ranges[0]);
                List<string> list = new List<string>();
                foreach (IRange range in Enumerable.Distinct<IRange>((IEnumerable<IRange>) format.Ranges, new RangeComparer()))
                {
                    this.GetRowColumn(range);
                    string str = string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) });
                    list.Add(str);
                }
                writer.WriteValue(string.Join(" ", (IEnumerable<string>) list));
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        private void WriteLinkFile(Stream stream, string rid, string workbookName, List<string> sheetNames)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            using (@this.WriteDocument(true))
            {
                using (@this.WriteElement("externalLink", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                {
                    using (@this.WriteElement("externalBook"))
                    {
                        @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                        @this.WriteAttributeString("r", "id", "http://schemas.openxmlformats.org/officeDocument/2006/relationships", rid);
                        using (@this.WriteElement("sheetNames"))
                        {
                            foreach (string str in sheetNames)
                            {
                                @this.WriteLeafElementWithAttribute("sheetName", "val", str);
                            }
                        }
                    }
                }
            }
            @this.Flush();
        }

        private void WriteMergeCells(XmlWriter writer, short sheet)
        {
            List<IRange> mergedCells = this._excelWriter.GetMergedCells(sheet);
            if ((mergedCells != null) && (mergedCells.Count > 0))
            {
                using (writer.WriteElement("mergeCells"))
                {
                    writer.WriteAttributeString("count", ((int) mergedCells.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    foreach (IRange range in mergedCells)
                    {
                        using (writer.WriteElement("mergeCell"))
                        {
                            writer.WriteAttributeString("ref", string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(range.Column), ((int) (range.Row + 1)), this.TryGetColumnIndexInA1LetterForm((range.Column + range.ColumnSpan) - 1), ((int) (range.Row + range.RowSpan)) }));
                        }
                    }
                }
            }
        }

        private void WriteNumFmt(XmlWriter writer, KeyValuePair<int, string> format)
        {
            if (format.Key >= 0)
            {
                using (writer.WriteElement("numFmt"))
                {
                    writer.WriteAttributeString("numFmtId", ((int) format.Key).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("formatCode", format.Value);
                }
            }
        }

        private void WritePageMargins(XmlWriter writer, short sheetIndex)
        {
            IExcelPrintPageMargin printPageMargin = this._excelWriter.GetPrintPageMargin(sheetIndex);
            if (printPageMargin != null)
            {
                using (writer.WriteElement("pageMargins"))
                {
                    writer.WriteAttributeString("left", ((double) printPageMargin.Left).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("right", ((double) printPageMargin.Right).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("top", ((double) printPageMargin.Top).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("bottom", ((double) printPageMargin.Bottom).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("header", ((double) printPageMargin.Header).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    writer.WriteAttributeString("footer", ((double) printPageMargin.Footer).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        private void WritePageSetup(XmlWriter writer, short sheet)
        {
            IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheet);
            if (printPageSetting != null)
            {
                using (writer.WriteElement("pageSetup"))
                {
                    if (printPageSetting.PaperSizeIndex > 1)
                    {
                        writer.WriteAttributeString("paperSize", ((int) printPageSetting.PaperSizeIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (Math.Abs((double) (printPageSetting.ZoomFactor - 1.0)) > 0.01)
                    {
                        float num2 = printPageSetting.ZoomFactor * 100f;
                        writer.WriteAttributeString("scale", ((float) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (printPageSetting.FirstPageNumber != 1)
                    {
                        writer.WriteAttributeString("firstPageNumber", ((short) printPageSetting.FirstPageNumber).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if ((printPageSetting.SmartPrintPagesWidth > 0) && (printPageSetting.SmartPrintPagesWidth != 1))
                    {
                        writer.WriteAttributeString("fitToWidth", ((int) printPageSetting.SmartPrintPagesWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if ((printPageSetting.SmartPrintPagesHeight > 0) && (printPageSetting.SmartPrintPagesHeight != 1))
                    {
                        writer.WriteAttributeString("fitToHeight", ((int) printPageSetting.SmartPrintPagesHeight).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (printPageSetting.PageOrder != ExcelPrintPageOrder.Auto)
                    {
                        writer.WriteAttributeString("pageOrder", printPageSetting.PageOrder.ToString().ToCamelCase());
                    }
                    if (printPageSetting.Orientation != ExcelPrintOrientation.Auto)
                    {
                        writer.WriteAttributeString("orientation", printPageSetting.Orientation.ToString().ToCamelCase());
                    }
                    if (!printPageSetting.ShowColor)
                    {
                        writer.WriteAttributeString("blackAndWhite", "1");
                    }
                    if (printPageSetting.Draft)
                    {
                        writer.WriteAttributeString("draft", "1");
                    }
                    if (printPageSetting.CommentsStyle != ExcelPrintNotesStyle.None)
                    {
                        writer.WriteAttributeString("cellComments", printPageSetting.CommentsStyle.ToString().ToCamelCase());
                    }
                    if (printPageSetting.UseCustomStartingPage)
                    {
                        writer.WriteAttributeString("useFirstPageNumber", "1");
                    }
                    if (printPageSetting.CellErrorPrintStyle != ExcelCellErrorPrintStyle.Displayed)
                    {
                        if (printPageSetting.CellErrorPrintStyle == ExcelCellErrorPrintStyle.NA)
                        {
                            writer.WriteAttributeString("errors", "NA");
                        }
                        else
                        {
                            writer.WriteAttributeString("errors", printPageSetting.CellErrorPrintStyle.ToString().ToCamelCase());
                        }
                    }
                    if ((printPageSetting.Copies > 1) && (printPageSetting.Copies < 0x7fff))
                    {
                        writer.WriteAttributeString("copies", ((ushort) printPageSetting.Copies).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        private void WritePrintOptions(XmlWriter writer, short sheet)
        {
            IExcelPrintOptions printOptions = this._excelWriter.GetPrintOptions(sheet);
            if ((printOptions != null) && ((printOptions.PrintGridLine || printOptions.PrintRowColumnsHeaders) || (printOptions.HorizontalCentered || printOptions.VerticalCentered)))
            {
                using (writer.WriteElement("printOptions"))
                {
                    if (printOptions.PrintRowColumnsHeaders)
                    {
                        writer.WriteAttributeString("headings", "1");
                    }
                    if (printOptions.PrintGridLine)
                    {
                        writer.WriteAttributeString("gridLines", "1");
                    }
                    if (printOptions.HorizontalCentered)
                    {
                        writer.WriteAttributeString("horizontalCentered", "1");
                    }
                    if (printOptions.VerticalCentered)
                    {
                        writer.WriteAttributeString("verticalCentered", "1");
                    }
                }
            }
        }

        private void WriteRow(XmlWriter writer, short sheet, int rowIndex, List<IExcelCell> cells, IExcelRow row, bool isDefaultHeight)
        {
            string str2;
            writer.WriteStartElement("row");
            int num3 = rowIndex + 1;
            string str = ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture);
            writer.WriteAttributeString("r", str);
            if (((row != null) && (row.FormatId >= 0)) && this._xfMap.TryGetValue(row.FormatId + this._styleOffset, out str2))
            {
                writer.WriteAttributeString("s", str2);
                writer.WriteAttributeString("customFormat", "1");
            }
            if (row.CustomHeight)
            {
                writer.WriteAttributeString("customHeight", "1");
            }
            if (row.Height > 0.0)
            {
                writer.WriteAttributeString("ht", ((double) row.Height).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!row.Visible)
            {
                writer.WriteAttributeString("hidden", "1");
            }
            if (row.Collapsed)
            {
                writer.WriteAttributeString("collapsed", "1");
            }
            if (row.OutLineLevel != 0)
            {
                writer.WriteAttributeString("outlineLevel", ((byte) row.OutLineLevel).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            foreach (IExcelCell cell in cells)
            {
                if (cell.Column < 0x4000)
                {
                    try
                    {
                        string str9;
                        if (cell.Hyperlink != null)
                        {
                            this._cellsHasHyperlink.Add(cell);
                        }
                        if (cell == null)
                        {
                            continue;
                        }
                        object description = cell.Value;
                        int formatId = cell.FormatId;
                        if (((description == null) && (cell.Hyperlink != null)) && !string.IsNullOrWhiteSpace(cell.Hyperlink.Description))
                        {
                            description = cell.Hyperlink.Description;
                        }
                        string formula = cell.Formula;
                        if (this._isR1C1)
                        {
                            formula = cell.FormulaR1C1;
                        }
                        if (cell.IsArrayFormula)
                        {
                            formula = cell.FormulaArray;
                            if (this._isR1C1)
                            {
                                formula = cell.FormulaArrayR1C1;
                            }
                        }
                        CellType cellType = cell.CellType;
                        writer.WriteStartElement("c");
                        writer.WriteAttributeString("r", this.TryGetColumnIndexInA1LetterForm(cell.Column) + str);
                        if (cell.FormatId == -1)
                        {
                            writer.WriteAttributeString("s", "0");
                        }
                        else
                        {
                            string str4;
                            int num = cell.FormatId + this._styleOffset;
                            if (this._xfMap.TryGetValue(num, out str4))
                            {
                                writer.WriteAttributeString("s", str4);
                            }
                        }
                        if (cellType == CellType.Unknown)
                        {
                            if (!string.IsNullOrWhiteSpace(cell.Formula))
                            {
                                cellType = CellType.FormulaString;
                            }
                            else
                            {
                                cellType = this.TryFigureOutCellType(description);
                            }
                        }
                        switch (cellType)
                        {
                            case CellType.String:
                                writer.WriteAttributeString("t", "s");
                                goto Label_0360;

                            case CellType.FormulaString:
                            {
                                if (((cell.Value == null) || cell.IsArrayFormula) || !Convert.ToString(cell.Value, (IFormatProvider) CultureInfo.InvariantCulture).StartsWith("#"))
                                {
                                    goto Label_031A;
                                }
                                string str5 = Convert.ToString(cell.Value, (IFormatProvider) CultureInfo.InvariantCulture).ToUpperInvariant();
                                if (!_errorsSet.Contains(str5))
                                {
                                    break;
                                }
                                writer.WriteAttributeString("t", "e");
                                goto Label_0360;
                            }
                            case CellType.Boolean:
                                writer.WriteAttributeString("t", "b");
                                goto Label_0360;

                            case CellType.Error:
                                writer.WriteAttributeString("t", "e");
                                goto Label_0360;

                            case CellType.Array:
                                writer.WriteAttributeString("t", "str");
                                goto Label_0360;

                            default:
                                goto Label_0360;
                        }
                        writer.WriteAttributeString("t", "str");
                        goto Label_0360;
                    Label_031A:
                        writer.WriteAttributeString("t", "str");
                    Label_0360:
                        if (!string.IsNullOrEmpty(formula))
                        {
                            if (this._isR1C1)
                            {
                                if ((cell.CellFormula != null) && !string.IsNullOrWhiteSpace(cell.CellFormula.Formula))
                                {
                                    formula = cell.CellFormula.Formula;
                                }
                                else
                                {
                                    formula = this.ConvertR1C1FormulaToA1Formula(formula, sheet, cell.Row, cell.Column);
                                }
                            }
                            if ((cell.IsArrayFormula && (cell.CellFormula != null)) && (cell.CellFormula.ArrayFormulaRange != null))
                            {
                                if ((cell.Row != cell.CellFormula.ArrayFormulaRange.Row) || (cell.Column != cell.CellFormula.ArrayFormulaRange.Column))
                                {
                                    goto Label_0554;
                                }
                                using (writer.WriteElement("f"))
                                {
                                    writer.WriteAttributeString("t", "array");
                                    string str6 = string.Format("{0}{1}:{2}{3}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(cell.CellFormula.ArrayFormulaRange.Column), ((int) (cell.CellFormula.ArrayFormulaRange.Row + 1)), this.TryGetColumnIndexInA1LetterForm((cell.CellFormula.ArrayFormulaRange.Column + cell.CellFormula.ArrayFormulaRange.ColumnSpan) - 1), ((int) (cell.CellFormula.ArrayFormulaRange.Row + cell.CellFormula.ArrayFormulaRange.RowSpan)) });
                                    writer.WriteAttributeString("ref", str6);
                                    if (this._externalRefMaps.Count != 0)
                                    {
                                        formula = this.ConvertExternalReferencedFormulaBack(formula);
                                    }
                                    writer.WriteStringExt(formula, this._excelWriter);
                                    goto Label_0554;
                                }
                            }
                            using (writer.WriteElement("f"))
                            {
                                if (this._externalRefMaps.Count != 0)
                                {
                                    formula = this.ConvertExternalReferencedFormulaBack(formula);
                                }
                                writer.WriteStringExt(formula.ToSpecialEncodeForXML(), this._excelWriter);
                            }
                        }
                    Label_0554:
                        if (description == null)
                        {
                            goto Label_074A;
                        }
                        writer.WriteStartElement("v");
                        switch (cellType)
                        {
                            case CellType.String:
                            {
                                string str7 = (string) (description as string);
                                int num2 = -1;
                                if (!this._sstTable.TryGetValue(str7, out num2))
                                {
                                    num2 = this._sstTable.Count;
                                    this._sstTable.Add(str7, this._sstTable.Count);
                                }
                                writer.WriteValue(num2);
                                goto Label_0744;
                            }
                            case CellType.Error:
                                goto Label_0744;

                            default:
                                DateTime time;
                                if ((cell.Value is bool) || (cellType == CellType.Boolean))
                                {
                                    string str8 = "1";
                                    if ((cell.Value is bool) && !((bool) cell.Value))
                                    {
                                        str8 = "0";
                                    }
                                    else if (Convert.ToString(cell.Value, (IFormatProvider) CultureInfo.InvariantCulture).ToUpper() == "FALSE")
                                    {
                                        str8 = "0";
                                    }
                                    writer.WriteValue(str8);
                                    goto Label_0744;
                                }
                                if (cellType != CellType.Datetime)
                                {
                                    goto Label_0699;
                                }
                                if (DateTime.TryParse(Convert.ToString(description, (IFormatProvider) CultureInfo.InvariantCulture), (IFormatProvider) CultureInfo.InvariantCulture, (DateTimeStyles) DateTimeStyles.None, out time))
                                {
                                    try
                                    {
                                        str9 = ((double) time.ToOADate()).ToString((IFormatProvider) CultureInfo.InvariantCulture);
                                        break;
                                    }
                                    catch (OverflowException)
                                    {
                                        str9 = Convert.ToString(description, (IFormatProvider) CultureInfo.InvariantCulture);
                                        break;
                                    }
                                }
                                str9 = Convert.ToString(description, (IFormatProvider) CultureInfo.InvariantCulture);
                                break;
                        }
                        writer.WriteStringExt(str9, this._excelWriter);
                        goto Label_0744;
                    Label_0699:
                        if (description is string)
                        {
                            string str10 = (string) (description as string);
                            if (((str10 != null) && (str10.Length == 1)) && (str10[0] <= '\x001f'))
                            {
                                object[] args = new object[1];
                                int num7 = str10[0];
                                args[0] = ((int) num7).ToString("x");
                                writer.WriteStringExt(string.Format("_x00{0}_", args), this._excelWriter);
                            }
                            else if ((str10 != null) && (!_errorsSet.Contains(str10) || string.IsNullOrWhiteSpace(formula)))
                            {
                                writer.WriteStringExt(str10, this._excelWriter);
                            }
                        }
                        else
                        {
                            writer.WriteStringExt(Convert.ToString(description, (IFormatProvider) CultureInfo.InvariantCulture), this._excelWriter);
                        }
                    Label_0744:
                        writer.WriteEndElement();
                    Label_074A:
                        writer.WriteEndElement();
                    }
                    catch (ExcelException exception)
                    {
                        if (exception.Code == ExcelExceptionCode.ParseException)
                        {
                            this.LogError(string.Format(ResourceHelper.GetResourceString("writeFormulaParseError"), (object[]) new object[] { exception.Message }), ExcelWarningCode.FormulaError, sheet, cell.Row, cell.Column, exception);
                        }
                        if (exception.Code == ExcelExceptionCode.FormulaError)
                        {
                            this.LogError(ResourceHelper.GetResourceString("writeCellFormulaError"), ExcelWarningCode.FormulaError, sheet, cell.Row, cell.Column, exception);
                        }
                        else
                        {
                            this.LogError(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheet, cell.Row, cell.Column, exception);
                        }
                    }
                    catch (Exception exception2)
                    {
                        this.LogError(ResourceHelper.GetResourceString("writeCellError"), ExcelWarningCode.FormulaError, sheet, cell.Row, cell.Column, exception2);
                    }
                }
            }
            writer.WriteEndElement();
        }

        private void WriteRowColumnBreaks(XmlWriter writer, short sheet)
        {
            IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheet);
            if ((printPageSetting != null) && (printPageSetting.RowBreakLines != null))
            {
                this.WriteRowColumnBreaks(writer, printPageSetting.RowBreakLines, "rowBreaks");
            }
            if ((printPageSetting != null) && (printPageSetting.ColumnBreakLines != null))
            {
                this.WriteRowColumnBreaks(writer, printPageSetting.ColumnBreakLines, "colBreaks");
            }
        }

        private void WriteRowColumnBreaks(XmlWriter writer, List<int> breaks, string node)
        {
            using (writer.WriteElement(node))
            {
                writer.WriteAttributeString("count", ((int) breaks.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteAttributeString("manualBreakCount", ((int) breaks.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                foreach (int num in breaks)
                {
                    if (num > 0)
                    {
                        using (writer.WriteElement("brk"))
                        {
                            writer.WriteAttributeString("id", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            writer.WriteAttributeString("man", "1");
                        }
                    }
                }
            }
        }

        private void WriteSheet(Stream stream, short sheet)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            using (@this.WriteDocument(true))
            {
                string name = "worksheet";
                if ((this._excelWriter is IExcelLosslessWriter) && ((this._excelWriter as IExcelLosslessWriter).GetSheetType(sheet) == ExcelSheetType.ChartSheet))
                {
                    name = "chartsheet";
                }
                using (@this.WriteElement(name, "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                {
                    IDisposable disposable3;
                    @this.WriteAttributeString("xmlns", "r", null, "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
                    int column = 0;
                    int row = 0;
                    this._excelWriter.GetDimensions(sheet, ref row, ref column);
                    IExcelPrintPageSetting printPageSetting = this._excelWriter.GetPrintPageSetting(sheet);
                    bool summaryColumnsRightToDetail = true;
                    bool summaryRowsBelowDetail = true;
                    this._excelWriter.GetOutlineDirection(sheet, ref summaryColumnsRightToDetail, ref summaryRowsBelowDetail);
                    if (((printPageSetting != null) || !summaryColumnsRightToDetail) || !summaryRowsBelowDetail)
                    {
                        using (disposable3 = @this.WriteElement("sheetPr"))
                        {
                            if (this._excelWriter is IExcelLosslessWriter)
                            {
                                string codeName = (this._excelWriter as IExcelLosslessWriter).GetCodeName(sheet);
                                if (!string.IsNullOrEmpty(codeName))
                                {
                                    @this.WriteAttributeString("codeName", codeName);
                                }
                            }
                            if (this._excelWriter is IExcelWriter2)
                            {
                                IExcelColor sheetTabColor = (this._excelWriter as IExcelWriter2).GetSheetTabColor(sheet);
                                if (sheetTabColor != null)
                                {
                                    this.WriteColor(@this, "tabColor", sheetTabColor);
                                }
                            }
                            if (printPageSetting.UseSmartPrint)
                            {
                                using (@this.WriteElement("pageSetUpPr"))
                                {
                                    @this.WriteAttributeString("fitToPage", "1");
                                }
                            }
                            if (!summaryColumnsRightToDetail || !summaryRowsBelowDetail)
                            {
                                using (@this.WriteElement("outlinePr"))
                                {
                                    if (!summaryColumnsRightToDetail)
                                    {
                                        @this.WriteAttributeString("summaryRight", "0");
                                    }
                                    if (!summaryRowsBelowDetail)
                                    {
                                        @this.WriteAttributeString("summaryBelow", "0");
                                    }
                                }
                            }
                        }
                    }
                    if ((column > 0) && (row > 0))
                    {
                        @this.WriteLeafElementWithAttribute("dimension", "ref", "A1:" + this.TryGetColumnIndexInA1LetterForm(column - 1) + ((int) row));
                    }
                    else
                    {
                        @this.WriteLeafElementWithAttribute("dimension", "ref", "A1");
                    }
                    using (@this.WriteElement("sheetViews"))
                    {
                        using (@this.WriteElement("sheetView"))
                        {
                            List<GcRect> list;
                            bool showFormula = false;
                            bool showZeros = true;
                            bool showGridLine = true;
                            bool showRowColumnHeader = true;
                            bool rightToLeftColumns = false;
                            this._excelWriter.GetDisplayElements(sheet, ref showFormula, ref showZeros, ref showGridLine, ref showRowColumnHeader, ref rightToLeftColumns);
                            IExcelColor gridlineColor = this._excelWriter.GetGridlineColor(sheet);
                            if (((gridlineColor != null) && (gridlineColor.ColorType == ExcelColorType.Indexed)) && (gridlineColor.Value != 0x40))
                            {
                                @this.WriteAttributeString("defaultGridColor", "0");
                                @this.WriteAttributeString("colorId", ((uint) gridlineColor.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (showFormula)
                            {
                                @this.WriteAttributeString("showFormulas", "1");
                            }
                            if (!showZeros)
                            {
                                @this.WriteAttributeString("showZeros", "0");
                            }
                            if (!showGridLine)
                            {
                                @this.WriteAttributeString("showGridLines", "0");
                            }
                            if (!showRowColumnHeader)
                            {
                                @this.WriteAttributeString("showRowColHeaders", "0");
                            }
                            if (rightToLeftColumns)
                            {
                                @this.WriteAttributeString("rightToLeft", "1");
                            }
                            int topRow = 0;
                            int leftColumn = 0;
                            this._excelWriter.GetTopLeft(sheet, ref topRow, ref leftColumn);
                            int frozenColumnCount = 0;
                            int frozenRowCount = 0;
                            int frozenTrailingColumnCount = 0;
                            int frozenTrailingRowCount = 0;
                            int num9 = 0;
                            int topVisibleRow = 0;
                            int paneIndex = 3;
                            this._excelWriter.GetFrozen(sheet, ref frozenRowCount, ref frozenColumnCount, ref frozenTrailingRowCount, ref frozenTrailingColumnCount);
                            if ((topRow > 0) || (leftColumn > 0))
                            {
                                if (topRow > 0x100000)
                                {
                                    topRow = 0xfffff;
                                }
                                if (leftColumn > 0x4000)
                                {
                                    leftColumn = 0x4000;
                                }
                                if ((frozenColumnCount <= 0) && (frozenRowCount <= 0))
                                {
                                    @this.WriteAttributeString("topLeftCell", string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(leftColumn), ((int) (topRow + 1)) }));
                                }
                            }
                            float zoom = 1f;
                            this._excelWriter.GetZoom(sheet, ref zoom);
                            if (Math.Abs((float) (1f - zoom)) > 0.1f)
                            {
                                int num27 = (int)(zoom * 100f);
                                @this.WriteAttributeString("zoomScale", ((int) num27).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            @this.WriteAttributeString("workbookViewId", "0");
                            if ((frozenColumnCount > 0) || (frozenRowCount > 0))
                            {
                                if ((frozenColumnCount <= 0) && (frozenRowCount <= 0))
                                {
                                    goto Label_05F6;
                                }
                                if (frozenColumnCount > 0)
                                {
                                    num9 = frozenColumnCount;
                                }
                                else
                                {
                                    num9 = 0;
                                }
                                if (frozenRowCount > 0)
                                {
                                    topVisibleRow = frozenRowCount;
                                }
                                else
                                {
                                    topVisibleRow = 0;
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
                                using (disposable3 = @this.WriteElement("pane"))
                                {
                                    if (frozenColumnCount > 0)
                                    {
                                        @this.WriteAttributeString("xSplit", ((int) frozenColumnCount).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    if (frozenRowCount > 0)
                                    {
                                        @this.WriteAttributeString("ySplit", ((int) frozenRowCount).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    @this.WriteAttributeString("topLeftCell", string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(num9), ((int) (topVisibleRow + 1)) }));
                                    @this.WriteAttributeString("activePane", ((PaneType) paneIndex).ToString().ToCamelCase());
                                    @this.WriteAttributeString("state", "frozen");
                                    goto Label_05F6;
                                }
                            }
                            this._excelWriter.GetPane(sheet, ref frozenColumnCount, ref frozenRowCount, ref topVisibleRow, ref num9, ref paneIndex);
                            if (((frozenColumnCount != 0) || (frozenRowCount != 0)) || ((topVisibleRow != 0) || (num9 != 0)))
                            {
                                using (disposable3 = @this.WriteElement("pane"))
                                {
                                    @this.WriteAttributeString("activePane", ((PaneType) paneIndex).ToString().ToCamelCase());
                                    if (frozenColumnCount > 0)
                                    {
                                        @this.WriteAttributeString("xSplit", ((int) frozenColumnCount).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    if (frozenRowCount > 0)
                                    {
                                        @this.WriteAttributeString("ySplit", ((int) frozenRowCount).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    @this.WriteAttributeString("topLeftCell", string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(num9), ((int) (topVisibleRow + 1)) }));
                                }
                            }
                        Label_05F6:
                            list = new List<GcRect>();
                            GcPoint activeCell = new GcPoint();
                            short num13 = 0;
                            short num14 = 1;
                            short num15 = 0;
                            for (int i = 3; (i >= 0) && (i <= 3); i--)
                            {
                                list.Clear();
                                PaneType paneType = (PaneType) i;
                                if (this._excelWriter.GetSelectionList(sheet, list, ref activeCell, ref paneType))
                                {
                                    activeCell.X = Math.Min(16384.0, activeCell.X);
                                    activeCell.Y = Math.Min(1048576.0, activeCell.Y);
                                    num14 = (short) list.Count;
                                    num13 = 0;
                                    while (num13 < num14)
                                    {
                                        GcRect rect = list[num13];
                                        if (this.IsDoubleEqual(-1.0, rect.Left) || this.IsDoubleEqual(-1.0, rect.Width))
                                        {
                                            rect = new GcRect(0.0, rect.Top, 256.0, rect.Height);
                                        }
                                        if (this.IsDoubleEqual(-1.0, rect.Top) || this.IsDoubleEqual(-1.0, rect.Height))
                                        {
                                            rect = new GcRect(rect.Left, 0.0, rect.Width, 65536.0);
                                        }
                                        if (((activeCell.X >= rect.Left) && (activeCell.X <= ((rect.Left + rect.Width) - 1.0))) && ((activeCell.Y >= rect.Top) && (activeCell.Y <= ((rect.Top + rect.Height) - 1.0))))
                                        {
                                            num15 = num13;
                                        }
                                        num13++;
                                    }
                                    string str3 = string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm((int) activeCell.X), ((int) (activeCell.Y + 1.0)) });
                                    string str4 = "";
                                    for (num13 = 0; num13 < num14; num13++)
                                    {
                                        GcRect rect2 = list[num13];
                                        int num17 = Math.Min(0x100000, (int) (rect2.Top + 1.0));
                                        int num18 = Math.Min((int) rect2.Left, 0x4000);
                                        string str5 = string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(num18), ((int) num17) });
                                        int num19 = Math.Min((int) ((num18 + rect2.Width) - 1.0), 0x4000);
                                        int num20 = Math.Min((int) ((num17 + rect2.Height) - 1.0), 0x100000);
                                        string str6 = string.Format("{0}{1}", (object[]) new object[] { this.TryGetColumnIndexInA1LetterForm(num19), ((int) num20) });
                                        str4 = str4 + string.Format("{0}:{1} ", (object[]) new object[] { str5, str6 });
                                        if (num13 == num15)
                                        {
                                            str3 = str5;
                                        }
                                    }
                                    str4 = str4.TrimEnd(new char[0]);
                                    if (!string.IsNullOrWhiteSpace(str4))
                                    {
                                        using (disposable3 = @this.WriteElement("selection"))
                                        {
                                            if (paneType != PaneType.TopLeft)
                                            {
                                                @this.WriteAttributeString("pane", paneType.ToString().ToCamelCase());
                                            }
                                            @this.WriteAttributeString("activeCell", str3);
                                            if (num15 != 0)
                                            {
                                                @this.WriteAttributeString("activeCellId", ((short) num15).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                            }
                                            @this.WriteAttributeString("sqref", str4);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    bool customHeight = false;
                    double defaultRowHeight = this._excelWriter.GetDefaultRowHeight(sheet, ref customHeight);
                    double defaultColumnWidth = this._excelWriter.GetDefaultColumnWidth(sheet);
                    using (disposable3 = @this.WriteElement("sheetFormatPr"))
                    {
                        @this.WriteAttributeString("defaultColWidth", ((double) defaultColumnWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        if (customHeight)
                        {
                            @this.WriteAttributeString("customHeight", "1");
                        }
                        @this.WriteAttributeString("defaultRowHeight", ((double) defaultRowHeight).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        int rowMaxOutLineLevel = 0;
                        int columnMaxOutLineLevel = 0;
                        this._excelWriter.GetGutters(sheet, ref rowMaxOutLineLevel, ref columnMaxOutLineLevel);
                        rowMaxOutLineLevel--;
                        columnMaxOutLineLevel--;
                        if (rowMaxOutLineLevel > 0)
                        {
                            @this.WriteAttributeString("outlineLevelRow", ((int) rowMaxOutLineLevel).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (columnMaxOutLineLevel > 0)
                        {
                            @this.WriteAttributeString("outlineLevelCol", ((int) columnMaxOutLineLevel).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                    this.WriteColomnInfo(@this, sheet);
                    this.WriteSheetData(@this, sheet);
                    this.WriteSheetProtection(@this, sheet);
                    this.WriteAutoFilter(@this, sheet);
                    this.WriteMergeCells(@this, sheet);
                    this.WriteConditionalFormating(@this, sheet);
                    this.WriteDataValidations(@this, sheet);
                    this.WriteHyperLink(@this, sheet);
                    this.WritePrintOptions(@this, sheet);
                    this.WritePageMargins(@this, sheet);
                    this.WritePageSetup(@this, sheet);
                    this.WriteHeadFooter(@this, sheet);
                    this.WriteRowColumnBreaks(@this, sheet);
                    string str7 = "";
                    if (this._drawingMapRef.TryGetValue(sheet, out str7))
                    {
                        using (disposable3 = @this.WriteElement("drawing"))
                        {
                            @this.WriteAttributeString("r", "id", null, str7);
                        }
                    }
                    if (this._legacyDrawingMapRef.TryGetValue(sheet, out str7))
                    {
                        using (disposable3 = @this.WriteElement("legacyDrawing"))
                        {
                            @this.WriteAttributeString("r", "id", null, str7);
                        }
                    }
                    string data = null;
                    if (this._excelWriter is IExcelLosslessWriter)
                    {
                        List<IUnsupportRecord> unsupportItems = (this._excelWriter as IExcelLosslessWriter).GetUnsupportItems(sheet);
                        if (unsupportItems != null)
                        {
                            foreach (IUnsupportRecord record in unsupportItems)
                            {
                                if (record.Category == RecordCategory.AlternateContent)
                                {
                                    data = (string) (record.Value as string);
                                    break;
                                }
                            }
                        }
                    }
                    if (data != null)
                    {
                        int index = data.IndexOf("AlternateContent");
                        data = data.Insert(index + "AlternateContent".Length, " xmlns:x14=\"http://schemas.microsoft.com/office/spreadsheetml/2009/9/main\"");
                        @this.WriteRaw(data);
                    }
                    this.WriteSheetTablesPart(@this, sheet);
                    this.WriteExtensions(@this, sheet);
                }
            }
            @this.Flush();
        }

        /// <summary>
        /// Writes the sheet data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="sheet">The sheet.</param>
        private void WriteSheetData(XmlWriter writer, short sheet)
        {
            List<IExcelCell> cells = new List<IExcelCell>();
            Dictionary<int, Tuple<IExcelRow, List<IExcelCell>>> dictionary = new Dictionary<int, Tuple<IExcelRow, List<IExcelCell>>>();
            List<IExcelRow> nonEmptyRows = this._excelWriter.GetNonEmptyRows(sheet);
            if (nonEmptyRows != null)
            {
                foreach (IExcelRow row in nonEmptyRows)
                {
                    if ((row != null) && (row.Index < 0x100000))
                    {
                        dictionary.Add(row.Index, new Tuple<IExcelRow, List<IExcelCell>>(row, new List<IExcelCell>()));
                    }
                }
            }
            if (this._excelWriter.GetCells(sheet, cells))
            {
                foreach (IExcelCell cell in cells)
                {
                    if (cell != null)
                    {
                        if (dictionary.ContainsKey(cell.Row))
                        {
                            dictionary[cell.Row].Item2.Add(cell);
                        }
                        else
                        {
                            dictionary.Add(cell.Row, new Tuple<IExcelRow, List<IExcelCell>>(null, new List<IExcelCell>()));
                            dictionary[cell.Row].Item2.Add(cell);
                        }
                    }
                }
            }
            bool customHeight = false;
            double defaultRowHeight = this._excelWriter.GetDefaultRowHeight(sheet, ref customHeight);
            using (writer.WriteElement("sheetData"))
            {
                foreach (int num2 in dictionary.Keys)
                {
                    Tuple<IExcelRow, List<IExcelCell>> tuple = dictionary[num2];
                    this.WriteRow(writer, sheet, num2, tuple.Item2, tuple.Item1, (tuple.Item1 != null) ? (Math.Abs((double) (tuple.Item1.Height - defaultRowHeight)) < 0.0001) : true);
                }
            }
        }

        private void WriteSheetProtection(XmlWriter writer, short sheet)
        {
            bool isProtect = false;
            this._excelWriter.GetProtect(sheet, ref isProtect);
            if (isProtect)
            {
                using (writer.WriteElement("sheetProtection"))
                {
                    writer.WriteAttributeString("sheet", "1");
                    writer.WriteAttributeString("objects", "1");
                }
            }
        }

        private void WriteSheetTablesPart(XmlWriter writer, short sheet)
        {
            if (this._excelWriter is IExcelTableWriter)
            {
                List<IExcelTable> sheetTables = (this._excelWriter as IExcelTableWriter).GetSheetTables(sheet);
                if ((sheetTables != null) && (sheetTables.Count > 0))
                {
                    using (writer.WriteElement("tableParts"))
                    {
                        writer.WriteAttributeString("count", ((int) sheetTables.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        foreach (IExcelTable table in sheetTables)
                        {
                            writer.WriteStartElement("tablePart");
                            writer.WriteAttributeString("r", "id", null, this._tablesMapRef[sheet][table.Name]);
                            writer.WriteEndElement();
                        }
                    }
                }
            }
        }

        private void WriteSparklineGroups(XmlWriter writer, short sheet)
        {
            if (this._excelWriter is IExcelSparklineWriter)
            {
                IExcelSparklineWriter writer2 = this._excelWriter as IExcelSparklineWriter;
                if (writer2 != null)
                {
                    List<IExcelSparklineGroup> excelSparkLineGroups = writer2.GetExcelSparkLineGroups(sheet);
                    if ((excelSparkLineGroups != null) && (excelSparkLineGroups.Count > 0))
                    {
                        using (writer.WriteElement("ext"))
                        {
                            writer.WriteAttributeString("xmlns", "x14", null, "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
                            writer.WriteAttributeString("uri", "{05C60535-1F16-4fd2-B633-F4F36F0B64E0}");
                            writer.WriteStartElement("x14", "sparklineGroups", null);
                            writer.WriteAttributeString("xmlns", "xm", null, "http://schemas.microsoft.com/office/excel/2006/main");
                            foreach (IExcelSparklineGroup group in excelSparkLineGroups)
                            {
                                writer.WriteStartElement("x14", "sparklineGroup", null);
                                if (Math.Abs((double) (group.LineWeight - 0.75)) > 0.001)
                                {
                                    writer.WriteAttributeString("lineWeight", ((double) group.LineWeight).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                if (group.SparklineType != ExcelSparklineType.Line)
                                {
                                    writer.WriteAttributeString("type", group.SparklineType.ToString().ToCamelCase());
                                }
                                if (group.IsDateAxis)
                                {
                                    writer.WriteAttributeString("dateAxis", "1");
                                }
                                if (group.DisplayEmptyCellAs != ExcelSparklineEmptyCellDisplayAs.Zero)
                                {
                                    writer.WriteAttributeString("displayEmptyCellsAs", group.DisplayEmptyCellAs.ToString().ToCamelCase());
                                }
                                if (group.ShowMarkers)
                                {
                                    writer.WriteAttributeString("markers", "1");
                                }
                                if (group.ShowHighestDifferently)
                                {
                                    writer.WriteAttributeString("high", "1");
                                }
                                if (group.ShowLowestDifferently)
                                {
                                    writer.WriteAttributeString("low", "1");
                                }
                                if (group.ShowFirstDifferently)
                                {
                                    writer.WriteAttributeString("first", "1");
                                }
                                if (group.ShowLastDifferently)
                                {
                                    writer.WriteAttributeString("last", "1");
                                }
                                if (group.ShowNegativeDifferently)
                                {
                                    writer.WriteAttributeString("negative", "1");
                                }
                                if (group.ShowXAxis)
                                {
                                    writer.WriteAttributeString("displayXAxis", "1");
                                }
                                if (group.ShowHidden)
                                {
                                    writer.WriteAttributeString("displayHidden", "1");
                                }
                                if (group.RightToLeft)
                                {
                                    writer.WriteAttributeString("rightToLeft", "1");
                                }
                                if (group.MinAxisType != ExcelSparklineAxisMinMax.Individual)
                                {
                                    writer.WriteAttributeString("minAxisType", group.MinAxisType.ToString().ToCamelCase());
                                }
                                if (group.MaxAxisType != ExcelSparklineAxisMinMax.Individual)
                                {
                                    writer.WriteAttributeString("maxAxisType", group.MaxAxisType.ToString().ToCamelCase());
                                }
                                if ((group.MinAxisType == ExcelSparklineAxisMinMax.Custom) && !double.IsNaN(group.ManualMinValue))
                                {
                                    writer.WriteAttributeString("manualMin", ((double) group.ManualMinValue).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                if ((group.MaxAxisType == ExcelSparklineAxisMinMax.Custom) && !double.IsNaN(group.ManualMaxValue))
                                {
                                    writer.WriteAttributeString("manualMax", ((double) group.ManualMaxValue).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                }
                                if (group.SeriesColor != null)
                                {
                                    this.WriteColor(writer, "colorSeries", "x14", group.SeriesColor);
                                }
                                if (group.NegativeColor != null)
                                {
                                    this.WriteColor(writer, "colorNegative", "x14", group.NegativeColor);
                                }
                                if (group.AxisColor != null)
                                {
                                    this.WriteColor(writer, "colorAxis", "x14", group.AxisColor);
                                }
                                if (group.MarkersColor != null)
                                {
                                    this.WriteColor(writer, "colorMarkers", "x14", group.MarkersColor);
                                }
                                if (group.FirstColor != null)
                                {
                                    this.WriteColor(writer, "colorFirst", "x14", group.FirstColor);
                                }
                                if (group.LastColor != null)
                                {
                                    this.WriteColor(writer, "colorLast", "x14", group.LastColor);
                                }
                                if (group.HighColor != null)
                                {
                                    this.WriteColor(writer, "colorHigh", "x14", group.HighColor);
                                }
                                if (group.LowColor != null)
                                {
                                    this.WriteColor(writer, "colorLow", "x14", group.LowColor);
                                }
                                if (group.IsDateAxis && (group.DateAxisRange != null))
                                {
                                    writer.WriteStartElement("xm", "f", null);
                                    writer.WriteValue(this.GetRangeRefString(group.DateAxisRange));
                                    writer.WriteEndElement();
                                }
                                if (group.Sparklines.Count > 0)
                                {
                                    writer.WriteStartElement("x14", "sparklines", null);
                                    foreach (IExcelSparkline sparkline in group.Sparklines)
                                    {
                                        writer.WriteStartElement("x14", "sparkline", null);
                                        string rangeRefString = this.GetRangeRefString(sparkline.DataRange);
                                        writer.WriteStartElement("xm", "f", null);
                                        writer.WriteValue(rangeRefString);
                                        writer.WriteEndElement();
                                        writer.WriteStartElement("xm", "sqref", null);
                                        writer.WriteValue(string.Format("{0}{1}", (object[]) new object[] { IndexHelper.GetColumnIndexInA1Letter(sparkline.Location.Column), ((int) (sparkline.Location.Row + 1)) }));
                                        writer.WriteEndElement();
                                        writer.WriteEndElement();
                                    }
                                    writer.WriteEndElement();
                                }
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
            }
        }

        private void WriteTableFile(Stream stream, string rid, IExcelTable table)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            using (@this.WriteDocument(true))
            {
                using (@this.WriteElement("table", "http://schemas.openxmlformats.org/spreadsheetml/2006/main"))
                {
                    string name = table.Name;
                    string displayName = table.DisplayName;
                    if (string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(displayName))
                    {
                        name = displayName;
                    }
                    if (!string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = name;
                    }
                    @this.WriteAttributeString("id", ((int) table.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    @this.WriteAttributeString("name", name);
                    @this.WriteAttributeString("displayName", displayName);
                    @this.WriteAttributeString("ref", this.ToRange(table.Range));
                    if (!table.ShowHeaderRow)
                    {
                        @this.WriteAttributeString("headerRowCount", "0");
                    }
                    if (table.ShowTotalsRow)
                    {
                        @this.WriteAttributeString("totalsRowCount", "1");
                    }
                    if ((table.AutoFilter != null) && (table.AutoFilter.Range != null))
                    {
                        this.WriteAutoFilter(@this, table.AutoFilter);
                    }
                    if ((table.Columns != null) && (table.Columns.Count > 0))
                    {
                        using (@this.WriteElement("tableColumns"))
                        {
                            @this.WriteAttributeString("count", ((int) table.Columns.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            foreach (IExcelTableColumn column in table.Columns)
                            {
                                using (@this.WriteElement("tableColumn"))
                                {
                                    @this.WriteAttributeString("id", ((int) column.Id).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    @this.WriteAttributeString("name", column.Name);
                                    if (!string.IsNullOrWhiteSpace(column.TotalsRowLabel))
                                    {
                                        @this.WriteAttributeString("totalsRowLabel", column.TotalsRowLabel);
                                    }
                                    if (column.TotalsRowFunction != ExcelTableTotalsRowFunction.None)
                                    {
                                        @this.WriteAttributeString("totalsRowFunction", column.TotalsRowFunction.ToString().ToCamelCase());
                                        if (!string.IsNullOrWhiteSpace(column.CalculatedColumnFormula))
                                        {
                                            using (@this.WriteElement("calculatedColumnFormula"))
                                            {
                                                if (column.CalculatedColumnFormulaIsArrayFormula)
                                                {
                                                    @this.WriteAttributeString("array", "1");
                                                }
                                                if (this._isR1C1)
                                                {
                                                    @this.WriteValue(this.ConvertR1C1FormulaToA1Formula(column.CalculatedColumnFormula, -1, 0, 0));
                                                }
                                                else
                                                {
                                                    @this.WriteValue(column.CalculatedColumnFormula);
                                                }
                                            }
                                        }
                                        if (column.TotalsRowFunction == ExcelTableTotalsRowFunction.Custom)
                                        {
                                            using (@this.WriteElement("totalsRowFormula"))
                                            {
                                                if (column.TotalsRowFunctionIsArrayFormula)
                                                {
                                                    @this.WriteAttributeString("array", "1");
                                                }
                                                if (this._isR1C1)
                                                {
                                                    @this.WriteValue(this.ConvertR1C1FormulaToA1Formula(column.TotalsRowCustomFunction, -1, 0, 0));
                                                }
                                                else
                                                {
                                                    @this.WriteValue(column.TotalsRowCustomFunction);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (table.TableStyleInfo != null)
                    {
                        using (@this.WriteElement("tableStyleInfo"))
                        {
                            if (!string.IsNullOrWhiteSpace(table.TableStyleInfo.Name))
                            {
                                @this.WriteAttributeString("name", table.TableStyleInfo.Name);
                            }
                            if (table.TableStyleInfo.ShowFirstColumn)
                            {
                                @this.WriteAttributeString("showFirstColumn", "1");
                            }
                            else
                            {
                                @this.WriteAttributeString("showFirstColumn", "0");
                            }
                            if (table.TableStyleInfo.ShowLastColumn)
                            {
                                @this.WriteAttributeString("showLastColumn", "1");
                            }
                            else
                            {
                                @this.WriteAttributeString("showLastColumn", "0");
                            }
                            if (table.TableStyleInfo.ShowRowStripes)
                            {
                                @this.WriteAttributeString("showRowStripes", "1");
                            }
                            else
                            {
                                @this.WriteAttributeString("showRowStripes", "0");
                            }
                            if (table.TableStyleInfo.ShowColumnStripes)
                            {
                                @this.WriteAttributeString("showColumnStripes", "1");
                            }
                            else
                            {
                                @this.WriteAttributeString("showColumnStripes", "0");
                            }
                        }
                    }
                }
            }
            @this.Flush();
        }

        private static void WriteTableStyle(XmlWriter writer, IExcelTableStyle tableStyle)
        {
            using (writer.WriteElement("tableStyle"))
            {
                writer.WriteAttributeString("name", tableStyle.Name);
                if (!tableStyle.IsPivotStyle)
                {
                    writer.WriteAttributeString("pivot", "0");
                }
                if (!tableStyle.IsTableStyle)
                {
                    writer.WriteAttributeString("table", "0");
                }
                if ((tableStyle.TableStyleElements != null) && (tableStyle.TableStyleElements.Count > 0))
                {
                    writer.WriteAttributeString("count", ((int) tableStyle.TableStyleElements.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if ((tableStyle.TableStyleElements != null) && (tableStyle.TableStyleElements.Count > 0))
                {
                    foreach (IExcelTableStyleElement element in tableStyle.TableStyleElements)
                    {
                        WriteTableStyleElement(writer, element);
                    }
                }
            }
        }

        private static void WriteTableStyleElement(XmlWriter writer, IExcelTableStyleElement element)
        {
            if (element != null)
            {
                using (writer.WriteElement("tableStyleElement"))
                {
                    writer.WriteAttributeString("type", element.Type.ToString().ToCamelCase());
                    if (element.DifferentFormattingIndex >= 0)
                    {
                        writer.WriteAttributeString("dxfId", ((int) element.DifferentFormattingIndex).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (element.Size > 0)
                    {
                        writer.WriteAttributeString("size", ((int) element.Size).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        private void WriteTheme(Stream stream)
        {
            XmlWriterSettings settings2 = new XmlWriterSettings();
            settings2.Encoding = Encoding.UTF8;
            XmlWriterSettings settings = settings2;
            XmlWriter @this = XmlWriter.Create(stream, settings);
            IExcelTheme theme = this._excelWriter.GetTheme();
            using (@this.WriteDocument(true))
            {
                string ns = "http://schemas.openxmlformats.org/drawingml/2006/main";
                using (@this.WriteElement("theme", ns, "a"))
                {
                    if (theme != null)
                    {
                        @this.WriteAttributeString("xmlns", "a", null, ns);
                        @this.WriteAttributeString("name", theme.Name);
                    }
                    else
                    {
                        @this.WriteAttributeString("xmlns", "a", null, ns);
                        @this.WriteAttributeString("name", "Office Theme");
                    }
                    using (@this.WriteElement("themeElements", ns, "a"))
                    {
                        IDisposable disposable4;
                        IDisposable disposable5;
                        IDisposable disposable6;
                        IDisposable disposable7;
                        IDisposable disposable8;
                        IDisposable disposable9;
                        if ((theme != null) && (theme.ColorScheme != null))
                        {
                            IColorScheme colorScheme = theme.ColorScheme;
                            using (disposable4 = @this.WriteElement("clrScheme", ns, "a"))
                            {
                                @this.WriteAttributeString("name", colorScheme.Name);
                                this.WriteColorScheme(@this, ns, "dk1", ColorSchemeIndex.TextDark1, colorScheme);
                                this.WriteColorScheme(@this, ns, "lt1", ColorSchemeIndex.TextLight1, colorScheme);
                                this.WriteColorScheme(@this, ns, "dk2", ColorSchemeIndex.TextDark2, colorScheme);
                                this.WriteColorScheme(@this, ns, "lt2", ColorSchemeIndex.TextLight2, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent1", ColorSchemeIndex.Accent1, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent2", ColorSchemeIndex.Accent2, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent3", ColorSchemeIndex.Accent3, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent4", ColorSchemeIndex.Accent4, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent5", ColorSchemeIndex.Accent5, colorScheme);
                                this.WriteColorScheme(@this, ns, "accent6", ColorSchemeIndex.Accent6, colorScheme);
                                this.WriteColorScheme(@this, ns, "hlink", ColorSchemeIndex.Hyperlink, colorScheme);
                                this.WriteColorScheme(@this, ns, "folHlink", ColorSchemeIndex.FollowedHyperlink, colorScheme);
                                goto Label_050D;
                            }
                        }
                        using (disposable5 = @this.WriteElement("clrScheme", ns, "a"))
                        {
                            @this.WriteAttributeString("name", "Office");
                            using (disposable6 = @this.WriteElement("dk1", ns, "a"))
                            {
                                using (disposable7 = @this.WriteElement("sysClr", ns, "a"))
                                {
                                    @this.WriteAttributeString("val", "windowText");
                                    @this.WriteAttributeString("lastClr", "000000");
                                }
                            }
                            using (disposable8 = @this.WriteElement("lt1", ns, "a"))
                            {
                                using (disposable9 = @this.WriteElement("sysClr", ns, "a"))
                                {
                                    @this.WriteAttributeString("val", "window");
                                    @this.WriteAttributeString("lastClr", "FFFFFF");
                                }
                            }
                            using (@this.WriteElement("dk2", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "1F497D");
                            }
                            using (@this.WriteElement("lt2", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "EEECE1");
                            }
                            using (@this.WriteElement("accent1", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "4F81BD");
                            }
                            using (@this.WriteElement("accent2", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "C0504D");
                            }
                            using (@this.WriteElement("accent3", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "9BBB59");
                            }
                            using (@this.WriteElement("accent4", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "8064A2");
                            }
                            using (@this.WriteElement("accent5", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "4BACC6");
                            }
                            using (@this.WriteElement("accent6", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "F79646");
                            }
                            using (@this.WriteElement("hlink", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "0000FF");
                            }
                            using (@this.WriteElement("folHlink", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("srgbClr", ns, "a", "val", "800080");
                            }
                        }
                    Label_050D:
                        if ((theme != null) && (theme.FontScheme != null))
                        {
                            IFontScheme fontScheme = theme.FontScheme;
                            using (@this.WriteElement("fontScheme", ns, "a"))
                            {
                                @this.WriteAttributeString("name", fontScheme.Name);
                                using (@this.WriteElement("majorFont", ns, "a"))
                                {
                                    this.WriteThemeFonts(@this, ns, fontScheme.MajorFont, true);
                                }
                                using (@this.WriteElement("minorFont", ns, "a"))
                                {
                                    this.WriteThemeFonts(@this, ns, fontScheme.MinorFont, false);
                                }
                                goto Label_06E1;
                            }
                        }
                        using (@this.WriteElement("fontScheme", ns, "a"))
                        {
                            @this.WriteAttributeString("name", "Office");
                            using (@this.WriteElement("majorFont", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("latin", ns, "a", "typeface", "Cambria");
                                @this.WriteLeafElementWithAttribute("ea", ns, "a", "typeface", "");
                                @this.WriteLeafElementWithAttribute("cs", ns, "a", "typeface", "");
                                WriteDefaultMajorFonts(@this, ns);
                            }
                            using (@this.WriteElement("minorFont", ns, "a"))
                            {
                                @this.WriteLeafElementWithAttribute("latin", ns, "a", "typeface", "Calibri");
                                @this.WriteLeafElementWithAttribute("ea", ns, "a", "typeface", "");
                                @this.WriteLeafElementWithAttribute("cs", ns, "a", "typeface", "");
                                WriteDefaultMinorFonts(@this, ns);
                            }
                        }
                    Label_06E1:
                        using (@this.WriteElement("fmtScheme", ns, "a"))
                        {
                            @this.WriteAttributeString("name", "Office");
                            using (@this.WriteElement("fillStyleLst", ns, "a"))
                            {
                                using (@this.WriteElement("solidFill", ns, "a"))
                                {
                                    @this.WriteLeafElementWithAttribute("schemeClr", ns, "a", "val", "phClr");
                                }
                                using (@this.WriteElement("gradFill", ns, "a"))
                                {
                                    @this.WriteAttributeString("rotWithShape", "1");
                                    using (@this.WriteElement("gsLst", ns, "a"))
                                    {
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "0");
                                            using (@this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (@this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "50000");
                                                }
                                                using (@this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "300000");
                                                }
                                            }
                                        }
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "35000");
                                            using (@this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (@this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "37000");
                                                }
                                                using (@this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "300000");
                                                }
                                            }
                                        }
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "100000");
                                            using (@this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (@this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "15000");
                                                }
                                                using (@this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "350000");
                                                }
                                            }
                                        }
                                    }
                                    using (@this.WriteElement("lin", ns, "a"))
                                    {
                                        @this.WriteAttributeString("ang", "16200000");
                                        @this.WriteAttributeString("scaled", "1");
                                    }
                                }
                                using (@this.WriteElement("gradFill", ns, "a"))
                                {
                                    @this.WriteAttributeString("rotWithShape", "1");
                                    using (@this.WriteElement("gsLst", ns, "a"))
                                    {
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "0");
                                            using (@this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (@this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "51000");
                                                }
                                                using (@this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "130000");
                                                }
                                            }
                                        }
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "80000");
                                            using (@this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (@this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "93000");
                                                }
                                                using (@this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "130000");
                                                }
                                            }
                                        }
                                        using (@this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "100000");
                                            using (disposable4 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable5 = @this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "94000");
                                                }
                                                using (disposable5 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "135000");
                                                }
                                            }
                                        }
                                    }
                                    using (disposable4 = @this.WriteElement("lin", ns, "a"))
                                    {
                                        @this.WriteAttributeString("ang", "16200000");
                                        @this.WriteAttributeString("scaled", "0");
                                    }
                                }
                            }
                            using (disposable4 = @this.WriteElement("lnStyleLst", ns, "a"))
                            {
                                using (disposable5 = @this.WriteElement("ln", ns, "a"))
                                {
                                    @this.WriteAttributeString("w", "9525");
                                    @this.WriteAttributeString("cap", "flat");
                                    @this.WriteAttributeString("cmpd", "sng");
                                    @this.WriteAttributeString("algn", "ctr");
                                    using (disposable6 = @this.WriteElement("solidFill", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("schemeClr", ns, "a"))
                                        {
                                            @this.WriteAttributeString("val", "phClr");
                                            @this.WriteLeafElementWithAttribute("shade", ns, "a", "val", "95000");
                                            @this.WriteLeafElementWithAttribute("satMod", ns, "a", "val", "105000");
                                        }
                                    }
                                    @this.WriteLeafElementWithAttribute("prstDash", ns, "a", "val", "solid");
                                }
                                using (disposable5 = @this.WriteElement("ln", ns, "a"))
                                {
                                    @this.WriteAttributeString("w", "25400");
                                    @this.WriteAttributeString("cap", "flat");
                                    @this.WriteAttributeString("cmpd", "sng");
                                    @this.WriteAttributeString("algn", "ctr");
                                    using (disposable6 = @this.WriteElement("solidFill", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("schemeClr", ns, "a"))
                                        {
                                            @this.WriteAttributeString("val", "phClr");
                                        }
                                    }
                                    @this.WriteLeafElementWithAttribute("prstDash", ns, "a", "val", "solid");
                                }
                                using (disposable5 = @this.WriteElement("ln", ns, "a"))
                                {
                                    @this.WriteAttributeString("w", "38100");
                                    @this.WriteAttributeString("cap", "flat");
                                    @this.WriteAttributeString("cmpd", "sng");
                                    @this.WriteAttributeString("algn", "ctr");
                                    using (disposable6 = @this.WriteElement("solidFill", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("schemeClr", ns, "a"))
                                        {
                                            @this.WriteAttributeString("val", "phClr");
                                        }
                                    }
                                    @this.WriteLeafElementWithAttribute("prstDash", ns, "a", "val", "solid");
                                }
                            }
                            using (disposable4 = @this.WriteElement("effectStyleLst", ns, "a"))
                            {
                                using (disposable5 = @this.WriteElement("effectStyle", ns, "a"))
                                {
                                    using (disposable6 = @this.WriteElement("effectLst", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("outerShdw", ns, "a"))
                                        {
                                            @this.WriteAttributeString("blurRad", "40000");
                                            @this.WriteAttributeString("dist", "20000");
                                            @this.WriteAttributeString("dir", "5400000");
                                            @this.WriteAttributeString("rotWithShape", "0");
                                            using (disposable8 = @this.WriteElement("srgbClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "000000");
                                                @this.WriteLeafElementWithAttribute("alpha", ns, "a", "val", "38000");
                                            }
                                        }
                                    }
                                }
                                using (disposable5 = @this.WriteElement("effectStyle", ns, "a"))
                                {
                                    using (disposable6 = @this.WriteElement("effectLst", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("outerShdw", ns, "a"))
                                        {
                                            @this.WriteAttributeString("blurRad", "40000");
                                            @this.WriteAttributeString("dist", "23000");
                                            @this.WriteAttributeString("dir", "5400000");
                                            @this.WriteAttributeString("rotWithShape", "0");
                                            using (disposable8 = @this.WriteElement("srgbClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "000000");
                                                @this.WriteLeafElementWithAttribute("alpha", ns, "a", "val", "35000");
                                            }
                                        }
                                    }
                                }
                                using (disposable5 = @this.WriteElement("effectStyle", ns, "a"))
                                {
                                    using (disposable6 = @this.WriteElement("effectLst", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("outerShdw", ns, "a"))
                                        {
                                            @this.WriteAttributeString("blurRad", "40000");
                                            @this.WriteAttributeString("dist", "23000");
                                            @this.WriteAttributeString("dir", "5400000");
                                            @this.WriteAttributeString("rotWithShape", "0");
                                            using (disposable8 = @this.WriteElement("srgbClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "000000");
                                                @this.WriteLeafElementWithAttribute("alpha", ns, "a", "val", "35000");
                                            }
                                        }
                                    }
                                    using (disposable6 = @this.WriteElement("scene3d", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("camera", ns, "a"))
                                        {
                                            @this.WriteAttributeString("prst", "orthographicFront");
                                            using (disposable8 = @this.WriteElement("rot", ns, "a"))
                                            {
                                                @this.WriteAttributeString("lat", "0");
                                                @this.WriteAttributeString("lon", "0");
                                                @this.WriteAttributeString("rev", "0");
                                            }
                                        }
                                        using (disposable7 = @this.WriteElement("lightRig", ns, "a"))
                                        {
                                            @this.WriteAttributeString("rig", "threePt");
                                            @this.WriteAttributeString("dir", "t");
                                            using (disposable8 = @this.WriteElement("rot", ns, "a"))
                                            {
                                                @this.WriteAttributeString("lat", "0");
                                                @this.WriteAttributeString("lon", "0");
                                                @this.WriteAttributeString("rev", "1200000");
                                            }
                                        }
                                    }
                                    using (disposable6 = @this.WriteElement("sp3d", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("bevelT", ns, "a"))
                                        {
                                            @this.WriteAttributeString("w", "63500");
                                            @this.WriteAttributeString("h", "25400");
                                        }
                                    }
                                }
                            }
                            using (disposable4 = @this.WriteElement("bgFillStyleLst", ns, "a"))
                            {
                                using (disposable5 = @this.WriteElement("solidFill", ns, "a"))
                                {
                                    using (disposable6 = @this.WriteElement("schemeClr", ns, "a"))
                                    {
                                        @this.WriteAttributeString("val", "phClr");
                                    }
                                }
                                using (disposable5 = @this.WriteElement("gradFill", ns, "a"))
                                {
                                    @this.WriteAttributeString("rotWithShape", "1");
                                    using (disposable6 = @this.WriteElement("gsLst", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "0");
                                            using (disposable8 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable9 = @this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "40000");
                                                }
                                                using (disposable9 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "350000");
                                                }
                                            }
                                        }
                                        using (disposable7 = @this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "40000");
                                            using (disposable8 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable9 = @this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "45000");
                                                }
                                                using (disposable9 = @this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "99000");
                                                }
                                                using (disposable9 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "350000");
                                                }
                                            }
                                        }
                                        using (disposable7 = @this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "100000");
                                            using (disposable8 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable9 = @this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "20000");
                                                }
                                                using (disposable9 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "255000");
                                                }
                                            }
                                        }
                                    }
                                    using (disposable6 = @this.WriteElement("path", ns, "a"))
                                    {
                                        @this.WriteAttributeString("path", "circle");
                                        using (disposable7 = @this.WriteElement("fillToRect", ns, "a"))
                                        {
                                            @this.WriteAttributeString("l", "50000");
                                            @this.WriteAttributeString("t", "-80000");
                                            @this.WriteAttributeString("r", "50000");
                                            @this.WriteAttributeString("b", "180000");
                                        }
                                    }
                                }
                                using (disposable5 = @this.WriteElement("gradFill", ns, "a"))
                                {
                                    @this.WriteAttributeString("rotWithShape", "1");
                                    using (disposable6 = @this.WriteElement("gsLst", ns, "a"))
                                    {
                                        using (disposable7 = @this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "0");
                                            using (disposable8 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable9 = @this.WriteElement("tint", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "80000");
                                                }
                                                using (disposable9 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "300000");
                                                }
                                            }
                                        }
                                        using (disposable7 = @this.WriteElement("gs", ns, "a"))
                                        {
                                            @this.WriteAttributeString("pos", "100000");
                                            using (disposable8 = @this.WriteElement("schemeClr", ns, "a"))
                                            {
                                                @this.WriteAttributeString("val", "phClr");
                                                using (disposable9 = @this.WriteElement("shade", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "30000");
                                                }
                                                using (disposable9 = @this.WriteElement("satMod", ns, "a"))
                                                {
                                                    @this.WriteAttributeString("val", "200000");
                                                }
                                            }
                                        }
                                    }
                                    using (disposable6 = @this.WriteElement("path", ns, "a"))
                                    {
                                        @this.WriteAttributeString("path", "circle");
                                        using (disposable7 = @this.WriteElement("fillToRect", ns, "a"))
                                        {
                                            @this.WriteAttributeString("l", "50000");
                                            @this.WriteAttributeString("t", "50000");
                                            @this.WriteAttributeString("r", "50000");
                                            @this.WriteAttributeString("b", "50000");
                                        }
                                    }
                                }
                            }
                            goto Label_1AC1;
                        }
                    }
                Label_1AC1:
                    @this.WriteLeafElement("objectDefaults", ns, "a");
                    @this.WriteLeafElement("extraClrSchemeLst", ns, "a");
                }
            }
            @this.Flush();
        }

        private void WriteThemeFonts(XmlWriter writer, string ns, IThemeFonts themeFonts, bool isMajorFont = true)
        {
            if (themeFonts != null)
            {
                if (themeFonts.RunFormattings != null)
                {
                    writer.WriteLeafElementWithAttribute(FontLanguage.LatinFont.ToRunFormattingValue(), ns, "a", "typeface", this.GetRunFormattingTypeface(themeFonts.RunFormattings, FontLanguage.LatinFont));
                    writer.WriteLeafElementWithAttribute(FontLanguage.EastAsianFont.ToRunFormattingValue(), ns, "a", "typeface", this.GetRunFormattingTypeface(themeFonts.RunFormattings, FontLanguage.EastAsianFont));
                    writer.WriteLeafElementWithAttribute(FontLanguage.ComplexScriptFont.ToRunFormattingValue(), ns, "a", "typeface", this.GetRunFormattingTypeface(themeFonts.RunFormattings, FontLanguage.ComplexScriptFont));
                }
                if ((themeFonts.ThemesFonts != null) && (themeFonts.ThemesFonts.Count > 0))
                {
                    foreach (IThemeFont font in themeFonts.ThemesFonts)
                    {
                        using (writer.WriteElement("font", ns, "a"))
                        {
                            writer.WriteAttributeString("script", font.Script);
                            writer.WriteAttributeString("typeface", font.Typeface);
                        }
                    }
                }
                else if (isMajorFont)
                {
                    WriteDefaultMajorFonts(writer, ns);
                }
                else
                {
                    WriteDefaultMinorFonts(writer, ns);
                }
            }
        }

        private void WriteTop10Filter(XmlWriter writer, IExcelTop10Filter excelTop10)
        {
            using (writer.WriteElement("top10"))
            {
                if (!excelTop10.Top)
                {
                    writer.WriteAttributeString("top", "0");
                }
                if (excelTop10.Percent)
                {
                    writer.WriteAttributeString("percent", "1");
                }
                if (!double.IsNaN(excelTop10.Value))
                {
                    writer.WriteAttributeString("val", ((double) excelTop10.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (!double.IsNaN(excelTop10.FilterValue))
                {
                    writer.WriteAttributeString("filterVal", ((double) excelTop10.FilterValue).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        private void WriteXFs(XmlWriter writer, List<Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>>> _xfs, string root, bool isCellStyleXfs = false)
        {
            if (_xfs.Count == 0)
            {
                using (writer.WriteElement(root))
                {
                    writer.WriteAttributeString("count", "1");
                    using (writer.WriteElement("xf"))
                    {
                        writer.WriteAttributeString("numFmtId", "0");
                        writer.WriteAttributeString("fontId", "0");
                        writer.WriteAttributeString("fillId", "0");
                        writer.WriteAttributeString("borderId", "0");
                        if (!isCellStyleXfs)
                        {
                            writer.WriteAttributeString("xfId", "0");
                        }
                    }
                    return;
                }
            }
            using (writer.WriteElement(root))
            {
                writer.WriteAttributeString("count", ((int) _xfs.Count).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                foreach (Tuple<int, int, int, int, int, IExtendedFormat, Tuple<bool, bool>> tuple in _xfs)
                {
                    using (writer.WriteElement("xf"))
                    {
                        writer.WriteAttributeString("numFmtId", ((int) tuple.Item1).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("fontId", ((int) tuple.Item2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("fillId", ((int) tuple.Item3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("borderId", ((int) tuple.Item4).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        if (tuple.Item6 != null)
                        {
                            if (!isCellStyleXfs)
                            {
                                if (!tuple.Item6.IsStyleFormat && tuple.Item6.ParentFormatID.HasValue)
                                {
                                    string str = null;
                                    if (this._xfMap.TryGetValue(tuple.Item6.ParentFormatID.Value, out str))
                                    {
                                        writer.WriteAttributeString("xfId", str);
                                    }
                                    else
                                    {
                                        writer.WriteAttributeString("xfId", "0");
                                    }
                                }
                                else
                                {
                                    writer.WriteAttributeString("xfId", "0");
                                }
                            }
                            if (tuple.Item6.ApplyNumberFormat.HasValue)
                            {
                                writer.WriteAttributeString("applyNumberFormat", tuple.Item6.ApplyNumberFormat.Value.ToStringForXML());
                            }
                            if (tuple.Item6.ApplyFont.HasValue)
                            {
                                writer.WriteAttributeString("applyFont", tuple.Item6.ApplyFont.Value.ToStringForXML());
                            }
                            if (tuple.Item6.ApplyFill.HasValue)
                            {
                                writer.WriteAttributeString("applyFill", tuple.Item6.ApplyFill.Value.ToStringForXML());
                            }
                            if (tuple.Item6.ApplyBorder.HasValue)
                            {
                                writer.WriteAttributeString("applyBorder", tuple.Item6.ApplyBorder.Value.ToStringForXML());
                            }
                            if (tuple.Item6.ApplyAlignment.HasValue)
                            {
                                writer.WriteAttributeString("applyAlignment", tuple.Item6.ApplyAlignment.Value.ToStringForXML());
                            }
                            if (tuple.Item6.ApplyProtection.HasValue)
                            {
                                writer.WriteAttributeString("applyProtection", tuple.Item6.ApplyProtection.Value.ToStringForXML());
                            }
                            if ((((tuple.Item6.HorizontalAlign != ExcelHorizontalAlignment.General) || (tuple.Item6.VerticalAlign != ExcelVerticalAlignment.Bottom)) || ((tuple.Item6.Rotation != 0) || (tuple.Item6.ReadingOrder != TextDirection.AccordingToContext))) || ((tuple.Item6.IsWordWrap || tuple.Item6.IsShrinkToFit) || (tuple.Item6.IsJustfyLastLine || (tuple.Item6.Indent > 0))))
                            {
                                using (writer.WriteElement("alignment"))
                                {
                                    if (tuple.Item6.HorizontalAlign != ExcelHorizontalAlignment.General)
                                    {
                                        writer.WriteAttributeString("horizontal", tuple.Item6.HorizontalAlign.ToString().ToCamelCase());
                                    }
                                    if (tuple.Item6.VerticalAlign != ExcelVerticalAlignment.Bottom)
                                    {
                                        writer.WriteAttributeString("vertical", tuple.Item6.VerticalAlign.ToString().ToCamelCase());
                                    }
                                    if (tuple.Item6.Rotation != 0)
                                    {
                                        writer.WriteAttributeString("textRotation", ((int) tuple.Item6.Rotation).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    if (tuple.Item6.ReadingOrder != TextDirection.AccordingToContext)
                                    {
                                        writer.WriteAttributeString("readingOrder", ((int) tuple.Item6.ReadingOrder).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                    if (tuple.Item6.IsWordWrap)
                                    {
                                        writer.WriteAttributeString("wrapText", "1");
                                    }
                                    if (tuple.Item6.IsShrinkToFit)
                                    {
                                        writer.WriteAttributeString("shrinkToFit", "1");
                                    }
                                    if (tuple.Item6.IsJustfyLastLine)
                                    {
                                        writer.WriteAttributeString("justifyLastLine", "1");
                                    }
                                    if (tuple.Item6.Indent > 0)
                                    {
                                        writer.WriteAttributeString("indent", ((byte) tuple.Item6.Indent).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                                    }
                                }
                            }
                        }
                        if ((tuple.Item7 != null) && (tuple.Item7.Item1 || !tuple.Item7.Item2))
                        {
                            using (writer.WriteElement("protection"))
                            {
                                if (!tuple.Item7.Item2)
                                {
                                    writer.WriteAttributeString("locked", "0");
                                }
                                if (tuple.Item7.Item1)
                                {
                                    writer.WriteAttributeString("hidden", "1");
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<string> SheetNames
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
    }
}

