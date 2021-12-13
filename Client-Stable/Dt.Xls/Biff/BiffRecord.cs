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
    internal class BiffRecord
    {
        public byte[] DataBuffer;
        public BiffRecordNumber RecordType;

        public void FromByteArray(byte[] buffer)
        {
            this.RecordType = (BiffRecordNumber) BitConverter.ToInt16(buffer, 0);
            this.DataLength = BitConverter.ToInt16(buffer, 2);
        }

        /// <summary>
        /// Reads the BIFF record header from the base stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public virtual void Read(BinaryReader reader)
        {
            this.RecordType = (BiffRecordNumber) reader.ReadInt16();
            this.DataLength = reader.ReadInt16();
            this.DataBuffer = reader.ReadBytes(this.DataLength);
        }

        /// <summary>
        /// Writes the BIFF record header to the base stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write((short) ((short) this.RecordType));
            writer.Write(this.DataLength);
            if ((this.DataBuffer != null) && (this.DataLength > 0))
            {
                writer.Write(this.DataBuffer, 0, this.DataLength);
            }
        }

        public virtual short DataLength { get; set; }

        public static int Length
        {
            get { return  4; }
        }
    }
}

