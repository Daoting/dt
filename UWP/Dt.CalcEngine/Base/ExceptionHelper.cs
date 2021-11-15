#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine
{
    internal class ExceptionHelper
    {
        public static void ThrowChangePartOfArrayFormulaException()
        {
            throw new InvalidOperationException("Exceptions.ChangeArrayFromula");
        }
    }
}

