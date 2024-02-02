#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Dt.Xls.OOXml
{
    internal static class ParsingContext
    {
        internal static Dt.Xls.LinkTable LinkTable = null;
        internal static List<string> ParsingErrors = new List<string>();
        internal static ExcelReferenceStyle ReferenceStyle;

        internal static string ConvertA1FormulaToR1C1Formula(string formula, int row, int column)
        {
            if (LinkTable == null)
            {
                Debugger.Break();
            }
            try
            {
                return Parser.Unparse(Parser.Parse(formula, row, column, false, LinkTable), row, column, true);
            }
            catch
            {
                ParsingErrors.Add(formula);
            }
            return formula;
        }

        internal static string ConvertR1C1FormulaToA1Formula(string formula, int row, int column)
        {
            try
            {
                return Parser.Unparse(Parser.Parse(formula, row, column, true, LinkTable), row, column, false);
            }
            catch
            {
                ParsingErrors.Add(formula);
                return formula;
            }
        }
    }
}

