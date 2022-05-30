#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Text;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct DrawStringInfo
    {
        public string str;
        public BaseFont font;
    }
}

