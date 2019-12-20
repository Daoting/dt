#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    internal class ExtendedFormatHelper
    {
        private static Dictionary<int, string> _builtInNumberFomrat = new Dictionary<int, string>();

        static ExtendedFormatHelper()
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
            _builtInNumberFomrat.Add(0x2f, "mmss.0");
            _builtInNumberFomrat.Add(0x30, "##0.0E+0");
            _builtInNumberFomrat.Add(0x31, "@");
            if (LanguageHelper.GetCurrentRuntimeLanguage() == Language.Ja_jp)
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
            if (LanguageHelper.GetCurrentRuntimeLanguage() == Language.Zh_cn)
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
            else
            {
                _builtInNumberFomrat[0x1b] = "m/d/yyyy";
                _builtInNumberFomrat[0x1c] = "m/d/yyyy";
                _builtInNumberFomrat[0x1d] = "m/d/yyyy";
                _builtInNumberFomrat[30] = "m/d/yyyy";
                _builtInNumberFomrat[0x1f] = "m/d/yyyy";
                _builtInNumberFomrat[0x20] = "h:mm:ss";
                _builtInNumberFomrat[0x21] = "h:mm:ss";
                _builtInNumberFomrat[0x22] = "m/d/yyyy";
                _builtInNumberFomrat[0x23] = "m/d/yyyy";
                _builtInNumberFomrat[0x24] = "m/d/yyyy";
                _builtInNumberFomrat[50] = "m/d/yyyy";
                _builtInNumberFomrat[0x33] = "m/d/yyyy";
                _builtInNumberFomrat[0x34] = "m/d/yyyy";
                _builtInNumberFomrat[0x35] = "m/d/yyyy";
                _builtInNumberFomrat[0x36] = "m/d/yyyy";
                _builtInNumberFomrat[0x37] = "m/d/yyyy";
                _builtInNumberFomrat[0x38] = "m/d/yyyy";
                _builtInNumberFomrat[0x39] = "m/d/yyyy";
                _builtInNumberFomrat[0x3a] = "m/d/yyyy";
            }
        }

        public static string GetFormatCode(IExtendedFormat format)
        {
            if (format.NumberFormat != null)
            {
                return format.NumberFormat.NumberFormatCode;
            }
            if (_builtInNumberFomrat.ContainsKey(format.NumberFormatIndex))
            {
                return _builtInNumberFomrat[format.NumberFormatIndex];
            }
            return null;
        }

        public static string GetFormatCode(int key)
        {
            string str = null;
            _builtInNumberFomrat.TryGetValue(key, out str);
            return str;
        }
    }
}

