#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Globalization;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class WorkbookParserContext : CalcParserContext
    {
        Workbook _context;

        public WorkbookParserContext(Workbook context, bool useR1C1 = false, int baseRowIndex = 0, int baseColumnIndex = 0, CultureInfo culture = null) : base(useR1C1, baseRowIndex, baseColumnIndex, culture)
        {
            this._context = context;
        }

        public override ICalcSource GetExternalSource(string workbookName, string worksheetName)
        {
            if (this._context != null)
            {
                return this._context.Sheets[worksheetName];
            }
            return base.GetExternalSource(workbookName, worksheetName);
        }

        public override string GetExternalSourceToken(ICalcSource source)
        {
            Worksheet worksheet = source as Worksheet;
            if (worksheet != null)
            {
                return worksheet.Name;
            }
            return base.GetExternalSourceToken(source);
        }
    }
}

