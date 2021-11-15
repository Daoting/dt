#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.Biff
{
    internal class TXORuns
    {
        private readonly int _size;

        public TXORuns(int size)
        {
            this._size = size;
        }

        internal void Read(BinaryReader reader)
        {
            this.rgTXORuns = new Run[(this._size / 8) - 1];
            for (int i = 0; i < this.rgTXORuns.Length; i++)
            {
                this.rgTXORuns[i] = new Run();
                this.rgTXORuns[i].Read(reader);
            }
            this.lastRun = new TXOLastRun();
            this.lastRun.Read(reader);
        }

        internal void Write(BinaryWriter writer)
        {
            for (int i = 0; i < this.rgTXORuns.Length; i++)
            {
                this.rgTXORuns[i].Write(writer);
            }
            this.lastRun.Write(writer);
        }

        internal List<BiffRecord> ContinueRecords { get; set; }

        internal TXOLastRun lastRun { get; set; }

        internal Run[] rgTXORuns { get; set; }

        internal int Size
        {
            get { return  this._size; }
        }
    }
}

