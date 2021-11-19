#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Specifies the values for ExcelException that can be retrieved
    /// by accessing the Code property of an ExcelException object. 
    /// </summary>
    internal enum ExcelExceptionCode
    {
        BiffStreamError,
        CannotCreateBiffRecordError,
        IncorrectErrorCode,
        ParseException,
        FormulaError,
        IncorrectFile
    }
}

