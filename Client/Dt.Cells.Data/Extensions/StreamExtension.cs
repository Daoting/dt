#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class StreamExtension
    {
        public static void Close(this Stream This)
        {
            This.Dispose();
        }
    }
}

