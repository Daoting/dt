#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class FontX
    {
        internal void Read(BinaryReader reader)
        {
            this.ifnt = reader.ReadUInt16();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.ifnt);
        }

        internal ExcelFont Font { get; set; }

        internal ushort ifnt { get; set; }
    }
}

