#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    internal static class XmlReaderExtension
    {
        public static void Close(this XmlReader This)
        {
            This.Dispose();
        }
    }
}

