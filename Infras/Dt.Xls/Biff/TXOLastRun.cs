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

namespace Dt.Xls.Biff
{
    internal class TXOLastRun
    {
        internal void Read(BinaryReader reader)
        {
            this.cchText = reader.ReadUInt16();
            reader.ReadUInt16();
            reader.ReadUInt32();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write(this.cchText);
            writer.Write((ushort) 0);
            writer.Write((uint) 0);
        }

        internal ushort cchText { get; set; }
    }
}

