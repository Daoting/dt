#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    internal class FunctionCompare
    {
        public static int Compare(FormulaFunction f1, FormulaFunction f2)
        {
            if (f1 == null)
            {
                if (f2 == null)
                {
                    return 0;
                }
                return -1;
            }
            if (f2 == null)
            {
                return 1;
            }
            return string.Compare(f1.Name, f2.Name);
        }
    }
}

