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
    internal class XFRecrod : BiffRecord
    {
        public XFRecrod()
        {
            this.RecordLength = 20;
        }

        private byte GetAlignmentAndTextBreakRecord()
        {
            byte num = 0;
            num |= this.HAlignment;
            if (this.IsWordWrap)
            {
                num |= 8;
            }
            num |= (byte)(this.VAlignment << 4);
            if (this.JustifyLastCharacter)
            {
                num |= 0x80;
            }
            return num;
        }

        private uint GetBorderAndFillPatternRecord()
        {
            uint num = 0;
            num |= this.TopBorderColorIndex;
            num |= (uint)(this.BottomBorderColorIndex << 7);
            return (num | (uint)(this.FillPatternIndex << 0x1a));
        }

        private uint GetBorderRecord()
        {
            uint num = 0;
            num |= (uint)this.LeftBorderLine;
            num |= (uint)this.RightBorderLine << 4;
            num |= (uint)this.TopBorderLine << 8;
            num |= (uint)this.BottomBorderLine << 12;
            num |= (uint)this.LeftBorderColorIndex << 0x10;
            return (num | (uint)(this.RightBorderColorIndex << 0x17));
        }

        private ushort GetCellProtectionAndParentRecord()
        {
            ushort num = 0;
            if (this.IsLocked)
            {
                num |= 1;
            }
            if (this.IsHidden)
            {
                num |= 2;
            }
            if (this.IsStyleXF)
            {
                num |= 4;
                return (ushort) (num | 0xfff0);
            }
            num &= 0xfffb;
            num |= (ushort)(this.ParentXFIndex << 4);
            if (this.IsF123Prefix)
            {
                num |= 8;
            }
            return num;
        }

        private ushort GetFillPatternColorRecord()
        {
            ushort num = 0;
            num |= this.FillPatternColor;
            return (ushort) (num | (this.FillPatternBackgroundColor << 7));
        }

        private byte GetFlags()
        {
            int num = 0;
            if (this.IsStyleXF)
            {
                if (!this.ApplyNumberFormat.HasValue || (this.ApplyNumberFormat.HasValue && !this.ApplyNumberFormat.Value))
                {
                    num |= 4;
                }
                if (this.ApplyFont.HasValue && !this.ApplyFont.Value)
                {
                    num |= 8;
                }
                if (this.ApplyAlignment.HasValue && !this.ApplyAlignment.Value)
                {
                    num |= 0x10;
                }
                if (this.ApplyBorder.HasValue && !this.ApplyBorder.Value)
                {
                    num |= 0x20;
                }
                if (this.ApplyFill.HasValue && !this.ApplyFill.Value)
                {
                    num |= 0x40;
                }
                if (this.ApplyProtection.HasValue && !this.ApplyProtection.Value)
                {
                    num |= 0x80;
                }
            }
            else
            {
                if (this.ApplyNumberFormat.HasValue && this.ApplyNumberFormat.Value)
                {
                    num |= 4;
                }
                if (this.ApplyFont.HasValue && this.ApplyFont.Value)
                {
                    num |= 8;
                }
                if (this.ApplyAlignment.HasValue && this.ApplyAlignment.Value)
                {
                    num |= 0x10;
                }
                if (this.ApplyBorder.HasValue && this.ApplyBorder.Value)
                {
                    num |= 0x20;
                }
                if (this.ApplyFill.HasValue && this.ApplyFill.Value)
                {
                    num |= 0x40;
                }
                if (this.ApplyProtection.HasValue && this.ApplyProtection.Value)
                {
                    num |= 0x80;
                }
            }
            return (byte) num;
        }

        private byte GetIndentShrinkAndTextDirectionRecord()
        {
            byte num = 0;
            if (this.IndentLevel > 15)
            {
                this.IndentLevel = 15;
            }
            num |= this.IndentLevel;
            if (this.IsShrinkContent)
            {
                num |= 0x10;
            }
            return (byte) (num | (this.Direction << 6));
        }

        public void Read(SimpleBinaryReader reader)
        {
            this.FontIndex = reader.ReadUInt16();
            this.FormatIndex = reader.ReadUInt16();
            this.ReadXFTyleCellProtectionAndParentXF(reader.ReadUInt16());
            this.ReadAlignmentAndTextBreak(reader.ReadByte());
            this.TextRotation = reader.ReadByte();
            this.ReadIndentShrinkAndTextDirection(reader.ReadByte());
            this.ReadFlags(reader.ReadByte());
            this.ReadBorder(reader.ReadUInt32());
            this.ReadBorderAndFillPattern(reader.ReadUInt32());
            this.ReadFillPatternColor(reader.ReadUInt16());
        }

        private void ReadAlignmentAndTextBreak(byte data)
        {
            this.HAlignment = (byte)(data & 7);
            this.IsWordWrap = (data & 8) == 8;
            this.VAlignment = (byte)((data & 0x70) >> 4);
            this.JustifyLastCharacter = (data & 0x80) == 0x80;
        }

        private void ReadBorder(uint data)
        {
            this.LeftBorderLine = (byte) (data & 15);
            this.RightBorderLine = (byte)((data & 240) >> 4);
            this.TopBorderLine = (byte)((data & 0xf00) >> 8);
            this.BottomBorderLine = (byte)((data & 0xf000) >> 12);
            this.LeftBorderColorIndex = (byte)((data & 0x7f0000) >> 0x10);
            this.RightBorderColorIndex = (byte)((data & 0x3f800000) >> 0x17);
        }

        private void ReadBorderAndFillPattern(uint data)
        {
            this.TopBorderColorIndex = (byte) (data & 0x7f);
            this.BottomBorderColorIndex = (byte)((data & 0x3f80) >> 7);
            this.FillPatternIndex = (byte)((data & -67108864) >> 0x1a);
        }

        private void ReadFillPatternColor(ushort data)
        {
            this.FillPatternColor = (byte)(data & 0x7f);
            this.FillPatternBackgroundColor = (byte)((data & 0x3f80) >> 7);
        }

        private void ReadFlags(byte data)
        {
            if (this.IsStyleXF)
            {
                data = (byte)(data >> 2);
                this.ApplyNumberFormat = new bool?((data & 1) == 0);
                this.ApplyFont = new bool?((data & 2) == 0);
                this.ApplyAlignment = new bool?((data & 4) == 0);
                this.ApplyBorder = new bool?((data & 8) == 0);
                this.ApplyFill = new bool?((data & 0x10) == 0);
                this.ApplyProtection = new bool?((data & 0x20) == 0);
            }
            else
            {
                data = (byte)(data >> 2);
                this.ApplyNumberFormat = new bool?((data & 1) == 1);
                this.ApplyFont = new bool?((data & 2) == 2);
                this.ApplyAlignment = new bool?((data & 4) == 4);
                this.ApplyBorder = new bool?((data & 8) == 8);
                this.ApplyFill = new bool?((data & 0x10) == 0x10);
                this.ApplyProtection = new bool?((data & 0x20) == 0x20);
            }
        }

        private void ReadIndentShrinkAndTextDirection(byte data)
        {
            this.IndentLevel = (byte)(data & 15);
            this.IsShrinkContent = (data & 0x10) == 0x10;
            this.Direction = (byte)((data & 0xc0) >> 6);
        }

        private void ReadXFTyleCellProtectionAndParentXF(ushort data)
        {
            if ((data & 1) == 1)
            {
                this.IsLocked = true;
            }
            if ((data & 2) == 2)
            {
                this.IsHidden = true;
            }
            if ((data & 4) == 4)
            {
                this.IsStyleXF = true;
                this.ParentXFIndex = 0xfff;
            }
            if ((data & 8) == 8)
            {
                this.IsF123Prefix = true;
            }
            if (!this.IsStyleXF)
            {
                this.ParentXFIndex = (ushort)((data & 0xfff0) >> 4);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(this.RecordNumber);
            writer.Write(this.RecordLength);
            writer.Write(this.FontIndex);
            writer.Write(this.FormatIndex);
            writer.Write(this.GetCellProtectionAndParentRecord());
            writer.Write(this.GetAlignmentAndTextBreakRecord());
            writer.Write(this.TextRotation);
            writer.Write(this.GetIndentShrinkAndTextDirectionRecord());
            writer.Write(this.GetFlags());
            writer.Write(this.GetBorderRecord());
            writer.Write(this.GetBorderAndFillPatternRecord());
            writer.Write(this.GetFillPatternColorRecord());
        }

        public void WriteToBuffer(byte[] buffer)
        {
            BinaryWriter writer = new BinaryWriter((Stream) new MemoryStream(buffer));
            writer.Write(this.FontIndex);
            writer.Write(this.FormatIndex);
            writer.Write(this.GetCellProtectionAndParentRecord());
            writer.Write(this.GetAlignmentAndTextBreakRecord());
            writer.Write(this.TextRotation);
            writer.Write(this.GetIndentShrinkAndTextDirectionRecord());
            writer.Write(this.GetFlags());
            writer.Write(this.GetBorderRecord());
            writer.Write(this.GetBorderAndFillPatternRecord());
            writer.Write(this.GetFillPatternColorRecord());
        }

        public bool? ApplyAlignment { get; set; }

        public bool? ApplyBorder { get; set; }

        public bool? ApplyFill { get; set; }

        public bool? ApplyFont { get; set; }

        public bool? ApplyNumberFormat { get; set; }

        public bool? ApplyProtection { get; set; }

        public byte BottomBorderColorIndex { get; set; }

        public byte BottomBorderLine { get; set; }

        public byte Direction { get; set; }

        public byte FillPatternBackgroundColor { get; set; }

        public byte FillPatternColor { get; set; }

        public byte FillPatternIndex { get; set; }

        public ushort FontIndex { get; set; }

        public ushort FormatIndex { get; set; }

        public byte HAlignment { get; set; }

        public byte IndentLevel { get; set; }

        public bool IsF123Prefix { get; set; }

        public bool IsHidden { get; set; }

        public bool IsLocked { get; set; }

        public bool IsShrinkContent { get; set; }

        public bool IsStyleXF { get; set; }

        public bool IsWordWrap { get; set; }

        public bool JustifyLastCharacter { get; set; }

        public byte LeftBorderColorIndex { get; set; }

        public byte LeftBorderLine { get; set; }

        public ushort ParentXFIndex { get; set; }

        public ushort RecordLength { get; private set; }

        public ushort RecordNumber
        {
            get { return  0xe0; }
        }

        public byte RightBorderColorIndex { get; set; }

        public byte RightBorderLine { get; set; }

        public byte TextRotation { get; set; }

        public byte TopBorderColorIndex { get; set; }

        public byte TopBorderLine { get; set; }

        public byte VAlignment { get; set; }
    }
}

