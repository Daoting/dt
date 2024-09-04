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
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class DirectoryEntry
    {
        private byte[] _bytes;
        private int _childID = NoStream;
        private int _CLSID1;
        private int _CLSID2;
        private int _CLSID3;
        private int _CLSID4;
        private long _creationTime;
        private int _leftSiblingID = NoStream;
        private long _modifiedTime;
        private char[] _name = new char[0x20];
        private short _nameLength;
        private Dt.Xls.Biff.NodeColor _nodeColor = Dt.Xls.Biff.NodeColor.Black;
        private int _rightSiblingID = NoStream;
        private int _startSector = -1;
        private int _stateBits;
        private int _streamSize;
        /// <summary>
        /// For a version 3 compound file with 512-byte sector size, the high DWORD MAY be uninitialized 
        /// or non-zero. Implementations MUST ignore the high DWORD when reading a version 3 compound file, 
        /// and MUST set the high DWORD to zero when writing a version 3 compound file.
        /// </summary>
        private int _streamSize2;
        private CompoundFileObjectType _type = CompoundFileObjectType.RootStorage;
        /// <summary>
        /// Represents the directory entry size in bytes.
        /// </summary>
        public static int DirectoryEntrySize = 0x80;
        /// <summary>
        /// An <b>int</b> value indicates the there is no stream.
        /// </summary>
        public static int NoStream = -1;

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned directory entry object.</returns>
        public DirectoryEntry Clone()
        {
            return (base.MemberwiseClone() as DirectoryEntry);
        }

        /// <summary>
        /// Gets the mini sector count of this directory entry.
        /// </summary>
        /// <returns>Returns the mini sector count.</returns>
        public int GetMiniSectorCount()
        {
            if (this.Name != "Root Entry")
            {
                if (this._bytes == null)
                {
                    return 0;
                }
                if (this._bytes.Length < 0x1000)
                {
                    return (int) Math.Ceiling((double) (((double) this._bytes.Length) / ((double) CompoundFile.MiniSectorSize)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the normal sector count of this directory entry.
        /// </summary>
        /// <returns>Returns the normal sector count.</returns>
        public int GetSectorCount()
        {
            if (this.Name != "Root Entry")
            {
                if (this._bytes == null)
                {
                    return 0;
                }
                if (this._bytes.Length >= 0x1000)
                {
                    return (int) Math.Ceiling((double) (((double) this._bytes.Length) / ((double) CompoundFile.SectorSize)));
                }
            }
            return 0;
        }

        /// <summary>
        /// Determines whether the storing stream is mini steam.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if it is mini steam; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMiniSteam()
        {
            if (this.Name == "Root Entry")
            {
                return false;
            }
            return ((this._bytes != null) && (this._bytes.Length < 0x1000));
        }

        /// <summary>
        /// Reads the directory entry from the input stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Read(BinaryReader reader)
        {
            this._name = reader.ReadChars(this._name.Length);
            this._nameLength = reader.ReadInt16();
            string name = this.Name;
            if (name != null)
            {
                if (name == "R")
                {
                    this.Name = "Root Entry";
                }
                else if (name == "Book")
                {
                    this.Name = XlsDirectoryEntryNames.WorkbookStream;
                }
            }
            this._type = (CompoundFileObjectType) reader.ReadByte();
            this._nodeColor = (Dt.Xls.Biff.NodeColor) reader.ReadByte();
            this._leftSiblingID = reader.ReadInt32();
            this._rightSiblingID = reader.ReadInt32();
            this._childID = reader.ReadInt32();
            this._CLSID1 = reader.ReadInt32();
            this._CLSID2 = reader.ReadInt32();
            this._CLSID3 = reader.ReadInt32();
            this._CLSID4 = reader.ReadInt32();
            this._stateBits = reader.ReadInt32();
            this._creationTime = reader.ReadInt64();
            this._modifiedTime = reader.ReadInt64();
            this._startSector = reader.ReadInt32();
            this._streamSize = reader.ReadInt32();
            this._streamSize2 = reader.ReadInt32();
        }

        /// <summary>
        /// Writes the directory entry to the out stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(this._name);
            writer.Write(this._nameLength);
            writer.Write((byte) this._type);
            writer.Write((byte) this._nodeColor);
            writer.Write(this._leftSiblingID);
            writer.Write(this._rightSiblingID);
            writer.Write(this._childID);
            writer.Write(this._CLSID1);
            writer.Write(this._CLSID2);
            writer.Write(this._CLSID3);
            writer.Write(this._CLSID4);
            writer.Write(this._stateBits);
            writer.Write(this._creationTime);
            writer.Write(this._modifiedTime);
            writer.Write(this._startSector);
            writer.Write(this._streamSize);
            writer.Write(this._streamSize2);
        }

        /// <summary>
        /// Gets or sets the content of current directory entry.
        /// </summary>
        public byte[] Bytes
        {
            get { return  this._bytes; }
            set { this._bytes = value; }
        }

        /// <summary>
        /// This field contains the Stream ID of a child object. If there is no child object, then the field 
        /// MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public int ChildID
        {
            get { return  this._childID; }
            set { this._childID = value; }
        }

        /// <summary>
        /// This field contains the creation time for a storage object. The Windows FILETIME structure 
        /// is used to represent this field in UTC. If there is no creation time set on the object, this 
        /// field MUST be all zeroes. For a root storage object, this field MUST be all zeroes, and the 
        /// creation time is retrieved or set on the compound file itself.
        /// </summary>
        public DateTime CreationTime
        {
            get { return  DateTime.FromFileTime(this._creationTime); }
            set { this._creationTime = value.ToFileTime(); }
        }

        /// <summary>
        /// This field contains the Stream ID of the left sibling. If there is no left sibling, the field 
        /// MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public int LeftSiblingID
        {
            get { return  this._leftSiblingID; }
            set { this._leftSiblingID = value; }
        }

        /// <summary>
        /// This field contains the modification time for a storage object. The Windows FILETIME structure 
        /// is used to represent this field in UTC. If there is no modified time set on the object, this 
        /// field MUST be all zeroes. For a root storage object, this field MUST be all zeroes, and the 
        /// modified time is retrieved or set on the compound file itself.
        /// </summary>
        public DateTime ModifiedTime
        {
            get { return  DateTime.FromFileTime(this._modifiedTime); }
            set { this._modifiedTime = value.ToFileTime(); }
        }

        /// <summary>
        /// This field MUST contain a Unicode string for the storage or stream name encoded in UTF-16. 
        /// The name MUST be terminated with a UTF-16 NUL character. Thus storage and stream names are 
        /// limited to 32 UTF-16 code points, including the NUL terminator character. When locating an 
        /// object in the compound file except for the root storage, the directory entry name is compared 
        /// using a special case-insensitive upper-case mapping, described in Red-Black Tree. The following 
        /// characters are illegal and MUST NOT be part of the name: '/', '\', ':', '!'.
        /// </summary>
        public string Name
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this._name);
                while (builder.Length > 0)
                {
                    if (builder[builder.Length - 1] != '\0')
                    {
                        break;
                    }
                    builder.Remove(builder.Length - 1, 1);
                }
                string str = builder.ToString();
                int index = str.IndexOf('\0');
                if (index != -1)
                {
                    return str.Substring(0, index);
                }
                return str;
            }
            set
            {
                for (int i = 0; (i < this._name.Length) && (i < value.Length); i++)
                {
                    this._name[i] = value[i];
                }
                this._nameLength = (short) Math.Min((int) (this._name.Length * 2), (int) ((value.Length + 1) * 2));
            }
        }

        /// <summary>
        /// This field MUST be 0x00 (red) or 0x01 (black).
        /// </summary>
        public Dt.Xls.Biff.NodeColor NodeColor
        {
            get { return  this._nodeColor; }
            set { this._nodeColor = value; }
        }

        /// <summary>
        /// This field contains the Stream ID of the right sibling. If there is no right sibling, the field 
        /// MUST be set to NOSTREAM (0xFFFFFFFF).
        /// </summary>
        public int RightSiblingID
        {
            get { return  this._rightSiblingID; }
            set { this._rightSiblingID = value; }
        }

        /// <summary>
        /// This field contains the first sector location if this is a stream object. For a root storage 
        /// object, this field MUST contain the first sector of the mini stream, if the mini stream exists.
        /// </summary>
        public int StartSector
        {
            get { return  this._startSector; }
            set { this._startSector = value; }
        }

        /// <summary>
        /// This 64-bit integer field contains the size of the user-defined data, if this is a stream object. 
        /// For a root storage object, this field contains the size of the mini stream.
        /// </summary>
        public int StreamSize
        {
            get { return  this._streamSize; }
            set { this._streamSize = value; }
        }

        /// <summary>
        /// This field MUST be 0x00, 0x01, 0x02, or 0x05, depending on the actual type of object.
        /// </summary>
        public CompoundFileObjectType Type
        {
            get { return  this._type; }
            set { this._type = value; }
        }
    }
}

