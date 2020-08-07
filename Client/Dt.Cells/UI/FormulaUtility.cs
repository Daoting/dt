#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using Dt.Cells.Data;
using System;
using System.Globalization;
#endregion

namespace Dt.Cells.UI
{
    internal class FormulaUtility
    {
        public static bool IsFormula(object val)
        {
            return (((val is string) && val.ToString().StartsWith("=")) && (val.ToString().Length > 1));
        }

        public static string StringInvariantToVariant(Worksheet worksheet, string invariantString)
        {
            if (worksheet == null)
            {
                return invariantString;
            }
            bool flag = worksheet.ReferenceStyle == ReferenceStyle.A1;
            string str = invariantString;
            bool flag2 = (invariantString != null) && invariantString.StartsWith("=");
            try
            {
                CalcParser parser = new CalcParser();
                CalcExpression expr = parser.Parse(invariantString, new SpreadCalcParserContext(worksheet, !flag, 0, 0, CultureInfo.InvariantCulture));
                str = parser.Unparse(expr, new SpreadCalcParserContext(worksheet, !flag, 0, 0, CultureInfo.CurrentCulture));
                if (flag2)
                {
                    str = "=" + str;
                }
            }
            catch
            {
            }
            return str;
        }

        public static string StringVariantToInvariant(Worksheet worksheet, string variantString)
        {
            if (worksheet == null)
            {
                return variantString;
            }
            bool flag = worksheet.ReferenceStyle == ReferenceStyle.A1;
            string str = variantString;
            bool flag2 = (str != null) && str.StartsWith("=");
            try
            {
                CalcParser parser = new CalcParser();
                CalcExpression expr = parser.Parse(variantString, new SpreadCalcParserContext(worksheet, !flag, 0, 0, CultureInfo.CurrentCulture));
                str = parser.Unparse(expr, new SpreadCalcParserContext(worksheet, !flag, 0, 0, CultureInfo.InvariantCulture));
                if (flag2)
                {
                    variantString = "=" + variantString;
                }
            }
            catch
            {
            }
            return str;
        }
    }
}

