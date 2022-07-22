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
#endregion

namespace Dt.Xls.Biff
{
    internal class CompoundFileHeader
    {
        private ushort _byteOrder = 0xfffe;
        private int _CLSID1;
        private int _CLSID2;
        private int _CLSID3;
        private int _CLSID4;
        public int[] _difatArray = new int[0];
        public int _difatSectorCount;
        private short _directorySectorCount;
        private int _fatSectorCount;
        private int _firstDifatSector = -2;
        private int _firstDirectorySector;
        private int _firstMiniFatSector = -2;
        private ulong _headerSignature = 16220472316735377360L;
        private int _miniFatSectorCount;
        private int _miniSectorShift = 6;
        private int _miniStreamCufoffSize = 0x1000;
        private int _reserved1;
        private short _reserved2;
        private ushort _revisionNo = 0x3e;
        private short _sectorShift = 9;
        private int _transactionSignature;
        private ushort _versionNo = 3;
        /// <summary>
        /// An <b>int</b> value indicates the max length of DIFAT in header.
        /// </summary>
        public static int MaxDifatArrayLength = 0x6d;
        /// <summary>
        /// An <b>int</b> value indicates the size of compound file header.
        /// </summary>
        public static int Size = 0x200;

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned compound file header.</returns>
        public CompoundFileHeader Clone()
        {
            return (base.MemberwiseClone() as CompoundFileHeader);
        }

        /// <summary>
        /// Reads the compound file header from the stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Read(BinaryReader reader)
        {
            this._headerSignature = reader.ReadUInt64();
            this._CLSID1 = reader.ReadInt32();
            this._CLSID2 = reader.ReadInt32();
            this._CLSID3 = reader.ReadInt32();
            this._CLSID4 = reader.ReadInt32();
            this._revisionNo = reader.ReadUInt16();
            this._versionNo = reader.ReadUInt16();
            if (this._versionNo > 3)
            {
                throw new Exception(string.Format("Compound file version {0} is not supported.", (object[]) new object[] { ((ushort) this._versionNo) }));
            }
            this._byteOrder = reader.ReadUInt16();
            this._sectorShift = reader.ReadInt16();
            this._miniSectorShift = reader.ReadInt32();
            this._reserved1 = reader.ReadInt32();
            this._reserved2 = reader.ReadInt16();
            this._directorySectorCount = reader.ReadInt16();
            this._fatSectorCount = reader.ReadInt32();
            this._firstDirectorySector = reader.ReadInt32();
            this._transactionSignature = reader.ReadInt32();
            this._miniStreamCufoffSize = reader.ReadInt32();
            this._firstMiniFatSector = reader.ReadInt32();
            this._miniFatSectorCount = reader.ReadInt32();
            this._firstDifatSector = reader.ReadInt32();
            this._difatSectorCount = reader.ReadInt32();
            int num = Math.Min(this._fatSectorCount, MaxDifatArrayLength);
            this._difatArray = new int[num];
            for (int i = 0; i < num; i++)
            {
                this._difatArray[i] = reader.ReadInt32();
            }
        }

        /// <summary>
        /// Writes the compound file header to the stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(this._headerSignature);
            writer.Write(this._CLSID1);
            writer.Write(this._CLSID2);
            writer.Write(this._CLSID3);
            writer.Write(this._CLSID4);
            writer.Write(this._revisionNo);
            writer.Write(this._versionNo);
            writer.Write(this._byteOrder);
            writer.Write(this._sectorShift);
            writer.Write(this._miniSectorShift);
            writer.Write(this._reserved1);
            writer.Write(this._reserved2);
            writer.Write(this._directorySectorCount);
            writer.Write(this._fatSectorCount);
            writer.Write(this._firstDirectorySector);
            writer.Write(this._transactionSignature);
            writer.Write(this._miniStreamCufoffSize);
            writer.Write(this._firstMiniFatSector);
            writer.Write(this._miniFatSectorCount);
            writer.Write(this._firstDifatSector);
            writer.Write(this._difatSectorCount);
            for (int i = 0; i < MaxDifatArrayLength; i++)
            {
                if (i < this._difatArray.Length)
                {
                    writer.Write(this._difatArray[i]);
                }
                else
                {
                    writer.Write(-1);
                }
            }
        }

        /// <summary>
        /// This array of 32-bit integer fields contains the first 109 FAT sector locations of the compound file.
        /// </summary>
        public int[] DifatArray
        {
            get { return  this._difatArray; }
            set { this._difatArray = value; }
        }

        /// <summary>
        /// This integer field contains the count of the number of DIFAT sectors in the compound file.
        /// </summary>
        public int DifatSectorCount
        {
            set { this._difatSectorCount = value; }
        }

        /// <summary>
        /// Gets or sets the main BAT(block allocation table) count, every main BAT block
        /// contains some block indexes which used to find the sectors that contain block allocation
        /// table.
        /// </summary>
        public int FatSectorCount
        {
            get { return  this._fatSectorCount; }
            set { this._fatSectorCount = value; }
        }

        /// <summary>
        /// This integer field contains the starting sector number for the DIFAT.
        /// </summary>
        public int FirstDifatSector
        {
            get { return  this._firstDifatSector; }
            set { this._firstDifatSector = value; }
        }

        /// <summary>
        /// This integer field contains the starting sector number for the Storage Stream.
        /// </summary>
        public int FirstDirectorySector
        {
            get { return  this._firstDirectorySector; }
            set { this._firstDirectorySector = value; }
        }

        /// <summary>
        /// This integer field contains the starting sector number for the mini FAT.
        /// </summary>
        public int FirstMiniFatSector
        {
            get { return  this._firstMiniFatSector; }
            set { this._firstMiniFatSector = value; }
        }

        /// <summary>
        /// This integer field contains the mini sector count for the mini FAT.
        /// </summary>
        public int MiniFatSectorCount
        {
            get { return  this._miniFatSectorCount; }
            set { this._miniFatSectorCount = value; }
        }

        /// <summary>
        /// This field MUST be set to 0x0006. This field specifies the sector size of the Mini Stream as 
        /// a power of 2. The sector size of the Mini Stream MUST be 64 bytes.
        /// </summary>
        public int MiniSectorSize
        {
            get { return  Convert.ToInt32(Math.Pow(2.0, (double) this._miniSectorShift)); }
        }

        /// <summary>
        /// This integer field MUST be set to 0x00001000. This field specifies the maximum size of a 
        /// user-defined data stream allocated from the mini FAT and mini stream, and that cutoff is 
        /// 4096 bytes. Any user-defined data stream larger than or equal to this cutoff size must 
        /// be allocated as normal sectors from the FAT.
        /// </summary>
        public int MiniStreamCutoffSize
        {
            get { return  this._miniStreamCufoffSize; }
        }

        /// <summary>
        /// This field MUST be set to 0x0009, or 0x000c, depending on the Major Version field. This field 
        /// specifies the sector size of the compound file as a power of 2.
        /// </summary>
        /// <remarks>
        /// If Major Version is 3, then the Sector Shift MUST be 0x0009, specifying a sector size of 512 bytes
        /// If Major Version is 4, then the Sector Shift MUST be 0x000C, specifying a sector size of 4096 bytes.
        /// </remarks>
        public int SectorSize
        {
            get { return  Convert.ToInt32(Math.Pow(2.0, (double) this._sectorShift)); }
        }
    }
}

