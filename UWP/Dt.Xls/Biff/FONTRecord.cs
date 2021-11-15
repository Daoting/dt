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
    internal class FONTRecord : BiffRecord
    {
        private bool _isItalic;
        private bool _isOutline;
        private bool _isShadow;
        private bool _isStrikeOut;
        public bool IsBold;

        private ushort GetFontAttributeRecord()
        {
            ushort num = 0;
            if (this.IsItalic)
            {
                num |= 2;
            }
            if (this.IsStrikeOut)
            {
                num |= 8;
            }
            if (this.IsOutline)
            {
                num |= 0x10;
            }
            if (this.IsShadow)
            {
                num |= 0x20;
            }
            return num;
        }

        private void InitFontAttribute(ushort grbit)
        {
            this._isItalic = (grbit & 2) == 2;
            this._isOutline = (grbit & 0x10) == 0x10;
            this._isShadow = (grbit & 0x20) == 0x20;
            this._isStrikeOut = (grbit & 8) == 8;
        }

        public void Read(SimpleBinaryReader reader)
        {
            string str;
            this.FontHeight = ((double) reader.ReadUInt16()) / 20.0;
            this.InitFontAttribute(reader.ReadUInt16());
            this.ColorIndex = reader.ReadUInt16();
            this.IsBold = reader.ReadUInt16() == 700;
            this.SuperScript = reader.ReadUInt16();
            this.UnderlineStyle = reader.ReadByte();
            this.FontFamily = reader.ReadByte();
            this.CharacterSet = reader.ReadByte();
            reader.ReadByte();
            this.FontNameLength = reader.ReadByte();
            reader.ReadUncompressedString(this.FontNameLength, out str);
            this.FontName = str;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.RecordNumber);
            ushort num = (ushort)(0x10 + (this.FontName.Length * 2));
            writer.Write(num);
            writer.Write((ushort) ((ushort) this.FontHeight));
            writer.Write(this.GetFontAttributeRecord());
            writer.Write(this.ColorIndex);
            if (this.IsBold)
            {
                writer.Write((short) 700);
            }
            else
            {
                writer.Write((short) 400);
            }
            writer.Write(this.SuperScript);
            writer.Write(this.UnderlineStyle);
            writer.Write(this.FontFamily);
            writer.Write(this.CharacterSet);
            writer.Write((byte) 0);
            WriteHelper.WriteBiffStr(writer, this.FontName, true, true, false, false, 1);
        }

        public byte CharacterSet { get; set; }

        public ushort ColorIndex { get; set; }

        public ushort FontAttributes { get; set; }

        public byte FontFamily { get; set; }

        public double FontHeight { get; set; }

        public string FontName { get; set; }

        public byte FontNameLength { get; set; }

        public bool IsItalic
        {
            get { return  this._isItalic; }
            set { this._isItalic = value; }
        }

        public bool IsOutline
        {
            get { return  this._isOutline; }
            set { this._isOutline = value; }
        }

        public bool IsShadow
        {
            get { return  this._isShadow; }
            set { this._isShadow = value; }
        }

        public bool IsStrikeOut
        {
            get { return  this._isStrikeOut; }
            set { this._isStrikeOut = value; }
        }

        public ushort RecordLength { get; set; }

        public ushort RecordNumber
        {
            get { return  0x31; }
        }

        public byte Reserved
        {
            get { return  0; }
        }

        public ushort SuperScript { get; set; }

        public byte UnderlineStyle { get; set; }
    }
}

