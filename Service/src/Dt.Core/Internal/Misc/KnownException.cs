#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 程序中已做判断的异常
    /// </summary>
    public class KnownException : Exception
    {
        public KnownException(string p_message)
            : base(p_message)
        {
        }
    }
}

