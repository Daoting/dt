#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cells.Data
{
    internal class ExtendedNumberFormatHelper
    {
        static Dictionary<int, string> _builtInNumberFomrat = new Dictionary<int, string>();
        static Dictionary<string, int> _builtInNumberFormat2 = new Dictionary<string, int>();
        static Dictionary<string, int> _customNumberFormat = new Dictionary<string, int>();
        static Dictionary<string, string> _formatCodeTable = new Dictionary<string, string>();
        static Dictionary<int, string> _languageIndepedentNumberFormat = new Dictionary<int, string>();
        static int _startKey = 170;

        internal static string GetExcelFormatCode(string spreadSheetFormatCode)
        {
            string str;
            if (_formatCodeTable.TryGetValue(spreadSheetFormatCode, out str))
            {
                return str;
            }
            return spreadSheetFormatCode;
        }

        internal static string GetFormatCode(IExtendedFormat format)
        {
            if (format.NumberFormat != null)
            {
                if (BuiltInNumberFomrat.ContainsKey(format.NumberFormat.NumberFormatId) && (BuiltInNumberFomrat[format.NumberFormat.NumberFormatId] != format.NumberFormat.NumberFormatCode))
                {
                    BuiltInNumberFomrat[format.NumberFormat.NumberFormatId] = format.NumberFormat.NumberFormatCode;
                    return format.NumberFormat.NumberFormatCode;
                }
                if (BuiltInNumberFomrat2.ContainsKey(format.NumberFormat.NumberFormatCode) && (BuiltInNumberFomrat2[format.NumberFormat.NumberFormatCode] != format.NumberFormat.NumberFormatId))
                {
                    BuiltInNumberFomrat2[format.NumberFormat.NumberFormatCode] = format.NumberFormat.NumberFormatId;
                    return format.NumberFormat.NumberFormatCode;
                }
                if (!_customNumberFormat.ContainsKey(format.NumberFormat.NumberFormatCode))
                {
                    _customNumberFormat.Add(format.NumberFormat.NumberFormatCode, format.NumberFormat.NumberFormatId);
                }
                return format.NumberFormat.NumberFormatCode;
            }
            if (BuiltInNumberFomrat.ContainsKey(format.NumberFormatIndex))
            {
                return BuiltInNumberFomrat[format.NumberFormatIndex];
            }
            if (LanguageIndepedentNumberFormat.ContainsKey(format.NumberFormatIndex))
            {
                return LanguageIndepedentNumberFormat[format.NumberFormatIndex];
            }
            return "General";
        }

        internal static int GetFormatId(string excelFormatCode, ref bool isBuiltIn)
        {
            if (BuiltInNumberFomrat.ContainsValue(excelFormatCode))
            {
                foreach (KeyValuePair<int, string> pair in _builtInNumberFomrat)
                {
                    if (pair.Value == excelFormatCode)
                    {
                        isBuiltIn = true;
                        return pair.Key;
                    }
                }
            }
            if (BuiltInNumberFomrat2.ContainsKey(excelFormatCode))
            {
                isBuiltIn = true;
                return _builtInNumberFormat2[excelFormatCode];
            }
            if (_customNumberFormat.ContainsKey(excelFormatCode))
            {
                isBuiltIn = false;
                return _customNumberFormat[excelFormatCode];
            }
            int nextAvirableKey = GetNextAvirableKey();
            _customNumberFormat.Add(excelFormatCode, nextAvirableKey);
            isBuiltIn = false;
            return nextAvirableKey;
        }

        static int GetNextAvirableKey()
        {
            int num = 0;
            if (_customNumberFormat.Count > 0)
            {
                num = Enumerable.Max((IEnumerable<int>) _customNumberFormat.Values);
            }
            return (Math.Max(_startKey++, num) + 1);
        }

        static void InitBuintNumberFormat()
        {
            _builtInNumberFomrat.Add(0, "General");
            _builtInNumberFomrat.Add(1, "0");
            _builtInNumberFomrat.Add(2, "0.00");
            _builtInNumberFomrat.Add(3, "#,##0");
            _builtInNumberFomrat.Add(4, "#,##0.00");
            _builtInNumberFomrat.Add(9, "0%");
            _builtInNumberFomrat.Add(10, "0.00%");
            _builtInNumberFomrat.Add(11, "0.00E+00");
            _builtInNumberFomrat.Add(12, "# ?/?");
            _builtInNumberFomrat.Add(13, "# ??/??");
            _builtInNumberFomrat.Add(14, "m/d/yyyy");
            _builtInNumberFomrat.Add(15, "d-mmm-yy");
            _builtInNumberFomrat.Add(0x10, "d-mmm");
            _builtInNumberFomrat.Add(0x11, "mmm-yy");
            _builtInNumberFomrat.Add(0x12, "h:mm AM/PM");
            _builtInNumberFomrat.Add(0x13, "h:mm:ss AM/PM");
            _builtInNumberFomrat.Add(20, "h:mm");
            _builtInNumberFomrat.Add(0x15, "h:mm:ss");
            _builtInNumberFomrat.Add(0x16, "m/d/yyyy h:mm");
            _builtInNumberFomrat.Add(0x25, "#,##0 ;(#,##0)");
            _builtInNumberFomrat.Add(0x26, "#,##0 ;[Red](#,##0)");
            _builtInNumberFomrat.Add(0x27, "#,##0.00;(#,##0.00)");
            _builtInNumberFomrat.Add(40, "#,##0.00;[Red](#,##0.00)");
            _builtInNumberFomrat.Add(0x2d, "mm:ss");
            _builtInNumberFomrat.Add(0x2e, "[h]:mm:ss");
            _builtInNumberFomrat.Add(0x2f, "mm:ss.0");
            _builtInNumberFomrat.Add(0x30, "##0.0E+0");
            _builtInNumberFomrat.Add(0x31, "@");
            if (Dt.Cells.Data.LanguageHelper.GetCurrentRuntimeLanguage() == Dt.Cells.Data.Language.Ja_jp)
            {
                _builtInNumberFomrat[0x1b] = "[$-411]ge.m.d";
                _builtInNumberFomrat[0x1c] = "[$-411]ggge\"年\"m\"月\"d\"";
                _builtInNumberFomrat[0x1d] = "[$-411]ggge\"年\"m\"月\"d\"";
                _builtInNumberFomrat[30] = "m/d/yy";
                _builtInNumberFomrat[0x1f] = "yyyy\"年\"m\"月\"d\"日\"";
                _builtInNumberFomrat[0x20] = "h\"時\"mm\"分\"";
                _builtInNumberFomrat[0x21] = "h\"時\"mm\"分\"ss\"秒\"";
                _builtInNumberFomrat[0x22] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x23] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x24] = "[$-411]ge.m.d";
                _builtInNumberFomrat[50] = "[$-411]ge.m.d";
                _builtInNumberFomrat[0x33] = "[$-411]ggge\"年\"m\"月\"d\"日\"";
                _builtInNumberFomrat[0x34] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x35] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x36] = "[$-411]ggge\"年\"m\"月\"d\"日\"";
                _builtInNumberFomrat[0x37] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x38] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x39] = "[$-411]ge.m.d";
                _builtInNumberFomrat[0x3a] = "[$-411]ggge\"年\"m\"月\"d\"日\"";
            }
            if (Dt.Cells.Data.LanguageHelper.GetCurrentRuntimeLanguage() == Dt.Cells.Data.Language.Zh_cn)
            {
                _builtInNumberFomrat[0x1b] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x1c] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x1d] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[30] = "m-d-yy";
                _builtInNumberFomrat[0x1f] = "yyyy\"年\"m\"月\"d\"日\"";
                _builtInNumberFomrat[0x20] = "h\"时\"mm\"分\"";
                _builtInNumberFomrat[0x21] = "h\"时\"mm\"分\"ss\"秒\"";
                _builtInNumberFomrat[0x22] = "上午/下午h\"时\"mm\"分\"";
                _builtInNumberFomrat[0x23] = "上午/下午h\"时\"mm\"分\"ss\"秒\"";
                _builtInNumberFomrat[0x24] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[50] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x33] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x34] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x35] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x36] = "m\"月\"d\"日\"";
                _builtInNumberFomrat[0x37] = "上午/下午h\"时\"mm\"分\"";
                _builtInNumberFomrat[0x38] = "上午/下午h\"时\"mm\"分\"ss\"秒\"";
                _builtInNumberFomrat[0x39] = "yyyy\"年\"m\"月\"";
                _builtInNumberFomrat[0x3a] = "m\"月\"d\"日\"";
            }
        }

        static void InitBuintNumberFormat2()
        {
            if (Dt.Cells.Data.LanguageHelper.GetCurrentRuntimeLanguage() == Dt.Cells.Data.Language.Ja_jp)
            {
                _builtInNumberFormat2["[$-411]ge.m.d"] = 0x1b;
                _builtInNumberFormat2["[$-411]ggge\"年\"m\"月\"d\""] = 0x1c;
                _builtInNumberFormat2["[$-411]ggge\"年\"m\"月\"d\""] = 0x1d;
                _builtInNumberFormat2["m/d/yy"] = 30;
                _builtInNumberFormat2["yyyy\"年\"m\"月\"d\"日\""] = 0x1f;
                _builtInNumberFormat2["h\"時\"mm\"分\""] = 0x20;
                _builtInNumberFormat2["h\"時\"mm\"分\"ss\"秒\""] = 0x21;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x22;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x23;
                _builtInNumberFormat2["[$-411]ge.m.d"] = 0x24;
                _builtInNumberFormat2["[$-411]ge.m.d"] = 50;
                _builtInNumberFormat2["[$-411]ggge\"年\"m\"月\"d\"日\""] = 0x33;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x34;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x35;
                _builtInNumberFormat2["[$-411]ggge\"年\"m\"月\"d\"日\""] = 0x36;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x37;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x38;
                _builtInNumberFormat2["[$-411]ge.m.d"] = 0x39;
                _builtInNumberFormat2["[$-411]ggge\"年\"m\"月\"d\"日\""] = 0x3a;
            }
            if (Dt.Cells.Data.LanguageHelper.GetCurrentRuntimeLanguage() == Dt.Cells.Data.Language.Zh_cn)
            {
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x1b;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x1c;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x1d;
                _builtInNumberFormat2["m-d-yy"] = 30;
                _builtInNumberFormat2["yyyy\"年\"m\"月\"d\"日\""] = 0x1f;
                _builtInNumberFormat2["h\"时\"mm\"分\""] = 0x20;
                _builtInNumberFormat2["h\"时\"mm\"分\"ss\"秒\""] = 0x21;
                _builtInNumberFormat2["上午/下午h\"时\"mm\"分\""] = 0x22;
                _builtInNumberFormat2["上午/下午h\"时\"mm\"分\"ss\"秒\""] = 0x23;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x24;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 50;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x33;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x34;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x35;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x36;
                _builtInNumberFormat2["上午/下午h\"时\"mm\"分\""] = 0x37;
                _builtInNumberFormat2["上午/下午h\"时\"mm\"分\"ss\"秒\""] = 0x38;
                _builtInNumberFormat2["yyyy\"年\"m\"月\""] = 0x39;
                _builtInNumberFormat2["m\"月\"d\"日\""] = 0x3a;
            }
        }

        static void InitLanguageIndepedentNumberFormat()
        {
            _languageIndepedentNumberFormat[0x1b] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x1c] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x1d] = "m/d/yyyy";
            _languageIndepedentNumberFormat[30] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x1f] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x20] = "h:mm:ss";
            _languageIndepedentNumberFormat[0x21] = "h:mm:ss";
            _languageIndepedentNumberFormat[0x22] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x23] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x24] = "m/d/yyyy";
            _languageIndepedentNumberFormat[50] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x33] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x34] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x35] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x36] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x37] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x38] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x39] = "m/d/yyyy";
            _languageIndepedentNumberFormat[0x3a] = "m/d/yyyy";
        }

        internal static void Reset()
        {
            _customNumberFormat.Clear();
            _startKey = 170;
        }

        internal static void UpdateFormatCodeTable(string formatCodeInSpreadSheet, string formatCode)
        {
            _formatCodeTable[formatCodeInSpreadSheet] = formatCode;
        }

        internal static Dictionary<int, string> BuiltInNumberFomrat
        {
            get
            {
                if ((_builtInNumberFomrat == null) || (_builtInNumberFomrat.Count == 0))
                {
                    InitBuintNumberFormat();
                }
                return _builtInNumberFomrat;
            }
        }

        internal static Dictionary<string, int> BuiltInNumberFomrat2
        {
            get
            {
                if ((_builtInNumberFormat2 == null) || (_builtInNumberFormat2.Count == 0))
                {
                    InitBuintNumberFormat2();
                }
                return _builtInNumberFormat2;
            }
        }

        internal static Dictionary<int, string> LanguageIndepedentNumberFormat
        {
            get
            {
                if ((_languageIndepedentNumberFormat == null) || (_languageIndepedentNumberFormat.Count == 0))
                {
                    InitLanguageIndepedentNumberFormat();
                }
                return _languageIndepedentNumberFormat;
            }
        }
    }
}

