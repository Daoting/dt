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
    internal class Run
    {
        internal void Read(BinaryReader reader)
        {
            this.ich = reader.ReadUInt16();
            this.ifnt = reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt16();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.ich);
            writer.Write(this.ifnt);
            writer.Write((ushort) 0);
            writer.Write((ushort) 0);
        }

        internal ExcelFont Font { get; set; }

        internal ushort ich { get; set; }

        internal ushort ifnt { get; set; }
    }
}

