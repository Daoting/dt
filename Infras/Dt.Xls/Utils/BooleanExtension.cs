#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Utils
{
    internal static class BooleanExtension
    {
        internal static bool ToBoolean(this string value)
        {
            return (value == "1");
        }

        internal static string ToStringForXML(this bool value)
        {
            if (value)
            {
                return "1";
            }
            return "0";
        }
    }
}

