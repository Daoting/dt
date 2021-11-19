#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal static class MemoryStreamExtension
    {
        public static void Close(this MemoryStream This)
        {
            This.Dispose();
        }

        public static byte[] GetBuffer(this MemoryStream This)
        {
            return This.ToArray();
        }
    }
}

