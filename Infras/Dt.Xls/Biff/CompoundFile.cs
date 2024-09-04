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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Xls.Biff
{
    internal class CompoundFile
    {
        /// <summary>
        /// Represents the double indirect file allocation table of this compound file.
        /// </summary>
        private FileAllocationTable _difatList = new FileAllocationTable();
        /// <summary>
        /// Represents the directory entries of this compound file.
        /// </summary>
        private DirectoryEntryCollection _directoryEntries = new DirectoryEntryCollection();
        /// <summary>
        /// Represents the file allocation table of this compound file.
        /// </summary>
        private FileAllocationTable _fatList = new FileAllocationTable();
        /// <summary>
        /// Represents the header of this compound file.
        /// </summary>
        private CompoundFileHeader _header = new CompoundFileHeader();
        /// <summary>
        /// Represents the mini file allocation table of this compound file.
        /// </summary>
        private FileAllocationTable _miniFatList = new FileAllocationTable();
        /// <summary>
        /// Represents the removing directory entries of the compound file.
        /// </summary>
        private List<int> _removingDirectoryEntries = new List<int>();
        /// <summary>
        /// Specifies a DIFAT sector in the FAT
        /// </summary>
        public const int DifSect = -4;
        /// <summary>
        /// Represents the compound file directory entry size.
        /// </summary>
        public const int DirectoryEntrySize = 0x80;
        /// <summary>
        /// End of linked chain of sectors
        /// </summary>
        public const int EndOfChain = -2;
        /// <summary>
        /// Specifies a FAT sector in the FAT
        /// </summary>
        public const int FatSect = -3;
        /// <summary>
        /// Specifies unallocated sector in the FAT, Mini FAT, or DIFAT
        /// </summary>
        public const int FreeSect = -1;
        /// <summary>
        /// Represents the compound file header signature.
        /// </summary>
        public const ulong HeaderSignature = 16220472316735377360L;
        /// <summary>
        /// Represents the maximum regular sector number.
        /// </summary>
        public const int MaxRegSect = -6;
        /// <summary>
        /// Represents the compound file mini sector size.
        /// </summary>
        public static int MiniSectorSize = 0x40;
        /// <summary>
        /// Specifies the cutoff size of the mini stream.  
        /// </summary>
        public const int MiniStreamCufoffSize = 0x1000;
        /// <summary>
        /// Specifies the root directory entry name.
        /// </summary>
        public const string RootName = "Root Entry";
        /// <summary>
        /// Represents the compound file sector size.  (V3 only)
        /// </summary>
        public static int SectorSize = 0x200;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Biff.CompoundFile" /> class.
        /// </summary>
        public CompoundFile(bool isReadOnly = false)
        {
            DirectoryEntry directoryEntry = new DirectoryEntry {
                Name = "Root Entry",
                Type = CompoundFileObjectType.Stream,
                NodeColor = NodeColor.Black,
                ChildID = DirectoryEntry.NoStream
            };
            if (!isReadOnly)
            {
                directoryEntry.CreationTime = DateTime.Now;
                directoryEntry.ModifiedTime = DateTime.Now;
            }
            directoryEntry.LeftSiblingID = DirectoryEntry.NoStream;
            directoryEntry.RightSiblingID = DirectoryEntry.NoStream;
            directoryEntry.ChildID = DirectoryEntry.NoStream;
            directoryEntry.Type = CompoundFileObjectType.RootStorage;
            directoryEntry.StartSector = -2;
            directoryEntry.StreamSize = 0;
            directoryEntry.Bytes = null;
            this._directoryEntries.Add(directoryEntry);
        }

        /// <summary>
        /// Adds the stream to the root directory.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="bytes">The bytes that will be written into the compound file.</param>
        public bool AddStream(string name, byte[] bytes)
        {
            return this.AddStream(name, bytes, "Root Entry");
        }

        /// <summary>
        /// Adds the stream to the specified parent storage.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="bytes">The bytes that will be written into the compound file.</param>
        /// <param name="parentName">The name of the parent storage.</param>
        public bool AddStream(string name, byte[] bytes, string parentName)
        {
            int num = -1;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if ((entry.Name == parentName) && ((entry.Type == CompoundFileObjectType.Storage) || (entry.Type == CompoundFileObjectType.RootStorage)))
                {
                    num = i;
                    break;
                }
            }
            if (num == -1)
            {
                return false;
            }
            if (this.HasStream(name, parentName))
            {
                this.GetDirectoryEntry(name).Bytes = bytes;
                return true;
            }
            DirectoryEntry directoryEntry = new DirectoryEntry {
                Name = name,
                Type = CompoundFileObjectType.Stream,
                NodeColor = NodeColor.Black,
                ChildID = DirectoryEntry.NoStream,
                CreationTime = DateTime.Now,
                ModifiedTime = DateTime.Now,
                StartSector = 0,
                StreamSize = 0,
                Bytes = bytes
            };
            this._directoryEntries.Add(directoryEntry);
            int num3 = this._directoryEntries.Count - 1;
            if (this._directoryEntries[num].ChildID == DirectoryEntry.NoStream)
            {
                directoryEntry.LeftSiblingID = DirectoryEntry.NoStream;
                directoryEntry.RightSiblingID = DirectoryEntry.NoStream;
                this._directoryEntries[num].ChildID = num3;
            }
            else
            {
                List<int> ids = new List<int> {
                    this._directoryEntries[num].ChildID,
                    num3
                };
                this.GetSiblingDirectoryEntryIds(this._directoryEntries[num].ChildID, ids);
                this._directoryEntries[num].ChildID = ids[(ids.Count - 1) / 2];
                this.ResetAllSiblingDirectoryEntryIds(ids);
                this.BalanceSiblingDirectoryEntryIds((ids.Count - 1) / 2, 0, ids.Count - 1, ids);
            }
            return true;
        }

        /// <summary>
        /// Populates all user data sectors and mini sectors.
        /// </summary>
        private void AllocateUserDataSectors()
        {
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if ((entry.Name != "Root Entry") && !string.IsNullOrEmpty(entry.Name))
                {
                    if ((entry.Type == CompoundFileObjectType.Stream) && entry.IsMiniSteam())
                    {
                        int miniSectorCount = entry.GetMiniSectorCount();
                        entry.StartSector = this._miniFatList.Allocate(miniSectorCount, SectorType.Data);
                        entry.StreamSize = entry.Bytes.Length;
                    }
                    else if (entry.Type == CompoundFileObjectType.Stream)
                    {
                        int sectorCount = entry.GetSectorCount();
                        entry.StartSector = this._fatList.Allocate(sectorCount, SectorType.Data);
                        entry.StreamSize = entry.Bytes.Length;
                    }
                }
            }
        }

        /// <summary>
        /// Balances the sibling directory entry ids.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="from">From index.</param>
        /// <param name="to">To index.</param>
        /// <param name="ids">The ids.</param>
        private void BalanceSiblingDirectoryEntryIds(int index, int from, int to, List<int> ids)
        {
            if (from < to)
            {
                int num = ids[index];
                if (from != index)
                {
                    int num2 = from + ((index - from) / 2);
                    this._directoryEntries[num].LeftSiblingID = ids[num2];
                    this.BalanceSiblingDirectoryEntryIds(num2, from, index - 1, ids);
                }
                if (to != index)
                {
                    int num3 = (index + 1) + ((to - index) / 2);
                    this._directoryEntries[num].RightSiblingID = ids[num3];
                    this.BalanceSiblingDirectoryEntryIds(num3, index + 1, to, ids);
                }
            }
        }

        /// <summary>
        /// Ends to remove the directory entries from the compound file.
        /// </summary>
        private void EndRemoveDirectoryEntries()
        {
            for (int i = 0; i < this._removingDirectoryEntries.Count; i++)
            {
                int index = this._removingDirectoryEntries[i];
                this._directoryEntries.RemoveAt(index);
                for (int j = 0; j < this._directoryEntries.Count; j++)
                {
                    DirectoryEntry entry = this._directoryEntries[j];
                    if (entry.LeftSiblingID > index)
                    {
                        entry.LeftSiblingID--;
                    }
                    if (entry.RightSiblingID > index)
                    {
                        entry.RightSiblingID--;
                    }
                    if (entry.ChildID > index)
                    {
                        entry.ChildID--;
                    }
                }
                for (int k = i + 1; k < this._removingDirectoryEntries.Count; k++)
                {
                    if (this._removingDirectoryEntries[k] > index)
                    {
                        List<int> list;
                        int num5;
                        (list = this._removingDirectoryEntries)[num5 = k] = list[num5] - 1;
                    }
                }
            }
            this._removingDirectoryEntries.Clear();
        }

        /// <summary>
        /// Fills all empty directory entries in the sector.
        /// </summary>
        private void FillEmptyDirectoryEntries()
        {
            int num = SectorSize / 0x80;
            if ((this._directoryEntries.Count % num) != 0)
            {
                int num2 = num - (this._directoryEntries.Count % num);
                for (int i = 0; i < num2; i++)
                {
                    DirectoryEntry entry = this._directoryEntries.Add("");
                    entry.LeftSiblingID = DirectoryEntry.NoStream;
                    entry.RightSiblingID = DirectoryEntry.NoStream;
                    entry.ChildID = DirectoryEntry.NoStream;
                    entry.Type = CompoundFileObjectType.Unknown;
                }
            }
        }

        /// <summary>
        /// Gets the directory entry.
        /// </summary>
        /// <param name="directoryEntryName">Name of the directory entry.</param>
        /// <returns>Returns the directory entry object.</returns>
        public DirectoryEntry GetDirectoryEntry(string directoryEntryName)
        {
            return this._directoryEntries[directoryEntryName];
        }

        /// <summary>
        /// Gets the directory contents from input stream.
        /// </summary>
        /// <param name="directoryEntry">The directoryEntry.</param>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the byte array of the specified directory entry.</returns>
        private byte[] GetDirectoryEntryBytes(DirectoryEntry directoryEntry, BinaryReader reader)
        {
            if (directoryEntry == null)
            {
                return null;
            }
            if (directoryEntry.StreamSize < this._header.MiniStreamCutoffSize)
            {
                return this.GetMiniSectorBytes(directoryEntry.StartSector, directoryEntry.StreamSize, reader);
            }
            return this.GetSectorBytes(directoryEntry.StartSector, directoryEntry.StreamSize, reader);
        }

        /// <summary>
        /// Gets the mini sector contents from the input stream.
        /// </summary>
        /// <param name="startSector">The start sector.</param>
        /// <param name="streamSize"></param>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the byte array of the specified mini sector number.</returns>
        private byte[] GetMiniSectorBytes(int startSector, int streamSize, BinaryReader reader)
        {
            MemoryStream @this = null;
            byte[] buffer2;
            try
            {
                List<int> sectorList = this._miniFatList.GetSectorList(startSector);
                @this = new MemoryStream();
                for (int i = 0; i < sectorList.Count; i++)
                {
                    this.MoveToMiniSector(sectorList[i], reader);
                    int miniSectorSize = MiniSectorSize;
                    if ((i == (sectorList.Count - 1)) && ((streamSize % MiniSectorSize) != 0))
                    {
                        miniSectorSize = streamSize % MiniSectorSize;
                    }
                    @this.Write(reader.ReadBytes(miniSectorSize), 0, miniSectorSize);
                }
                @this.Flush();
                buffer2 = @this.ToArray();
            }
            catch
            {
                buffer2 = null;
            }
            finally
            {
                if (@this != null)
                {
                    @this.Close();
                }
            }
            return buffer2;
        }

        /// <summary>
        /// Gets the sector contents from the input stream.
        /// </summary>
        /// <param name="startSector">The start sector.</param>
        /// <param name="streamSize"></param>
        /// <param name="reader">The reader.</param>
        /// <returns>Returns the byte array of the specified sector number.</returns>
        private byte[] GetSectorBytes(int startSector, int streamSize, BinaryReader reader)
        {
            byte[] buffer2;
            List<int> sectorList = this._fatList.GetSectorList(startSector);
            List<ContinuousSectors> list2 = this.MakeSectors(sectorList);
            MemoryStream @this = null;
            try
            {
                @this = new MemoryStream();
                for (int i = 0; i < list2.Count; i++)
                {
                    this.MoveToSector(list2[i].StartSectorIndex, reader);
                    int num2 = SectorSize * list2[i].SectorCount;
                    if ((i == (list2.Count - 1)) && ((streamSize % SectorSize) != 0))
                    {
                        num2 -= SectorSize - (streamSize % SectorSize);
                    }
                    num2 = Math.Min(num2, (int) (reader.BaseStream.Length - reader.BaseStream.Position));
                    @this.Write(reader.ReadBytes(num2), 0, num2);
                }
                @this.Flush();
                buffer2 = @this.ToArray();
            }
            catch
            {
                buffer2 = null;
            }
            finally
            {
                if (@this != null)
                {
                    @this.Close();
                }
            }
            return buffer2;
        }

        /// <summary>
        /// Gets the sibling directory entry ids of the specified entry.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="ids">The id list.</param>
        private void GetSiblingDirectoryEntryIds(int id, List<int> ids)
        {
            if (this._directoryEntries[id].LeftSiblingID != DirectoryEntry.NoStream)
            {
                ids.Add(this._directoryEntries[id].LeftSiblingID);
                this.GetSiblingDirectoryEntryIds(this._directoryEntries[id].LeftSiblingID, ids);
            }
            if (this._directoryEntries[id].RightSiblingID != DirectoryEntry.NoStream)
            {
                ids.Add(this._directoryEntries[id].RightSiblingID);
                this.GetSiblingDirectoryEntryIds(this._directoryEntries[id].RightSiblingID, ids);
            }
        }

        /// <summary>
        /// Gets the specified stream from the compound file root storage.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <returns>Returns the bytes of the specified stream.</returns>
        public byte[] GetStream(string name)
        {
            return this.GetStream(name, "Root Entry");
        }

        /// <summary>
        /// Gets the specified stream from the compound file.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="parentName">The parent storage name of the stream.</param>
        /// <returns>Returns the bytes of the specified stream.</returns>
        public byte[] GetStream(string name, string parentName)
        {
            int num = -1;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if ((entry.Name == parentName) && ((entry.Type == CompoundFileObjectType.Storage) || (entry.Type == CompoundFileObjectType.RootStorage)))
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                if (this._directoryEntries[num].ChildID == DirectoryEntry.NoStream)
                {
                    return null;
                }
                List<int> ids = new List<int> {
                    this._directoryEntries[num].ChildID
                };
                this.GetSiblingDirectoryEntryIds(this._directoryEntries[num].ChildID, ids);
                for (int j = 0; j < ids.Count; j++)
                {
                    DirectoryEntry entry2 = this._directoryEntries[ids[j]];
                    if ((entry2.Name == name) && (entry2.Type == CompoundFileObjectType.Stream))
                    {
                        return entry2.Bytes;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the total user data mini sector count.
        /// </summary>
        /// <returns>Returns the mini sector count.</returns>
        private int GetTotalUserDataMiniSectorCount()
        {
            int num = 0;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if (entry.Name != "Root Entry")
                {
                    num += entry.GetMiniSectorCount();
                }
            }
            return num;
        }

        /// <summary>
        /// Gets the total user data sector count.
        /// </summary>
        /// <returns>Returns the sector count.</returns>
        private int GetTotalUserDataSectorCount()
        {
            int num = 0;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if (entry.Name != "Root Entry")
                {
                    num += entry.GetSectorCount();
                }
            }
            return num;
        }

        /// <summary>
        /// Determines whether the specified name of stream exists.
        /// </summary>
        /// <param name="name">The name of the stream.</param>
        /// <param name="parentName">The name of the parent storage.</param>
        /// <returns>
        /// <see langword="true" /> if the specified name of stream exists; otherwise, <c>false</c>.
        /// </returns>
        public bool HasStream(string name, string parentName)
        {
            int num = -1;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if ((entry.Name == parentName) && ((entry.Type == CompoundFileObjectType.Storage) || (entry.Type == CompoundFileObjectType.RootStorage)))
                {
                    num = i;
                    break;
                }
            }
            if (num != -1)
            {
                if (this._directoryEntries[num].ChildID == DirectoryEntry.NoStream)
                {
                    return false;
                }
                List<int> ids = new List<int> {
                    this._directoryEntries[num].ChildID
                };
                this.GetSiblingDirectoryEntryIds(this._directoryEntries[num].ChildID, ids);
                for (int j = 0; j < ids.Count; j++)
                {
                    DirectoryEntry entry2 = this._directoryEntries[ids[j]];
                    if ((entry2.Name == name) && (entry2.Type == CompoundFileObjectType.Stream))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the input stream is a legal compound file stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>
        /// <see langword="true" /> if it's a compound file stream; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLegal(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            ulong num = reader.ReadUInt64();
            bool flag = false;
            if (num == 16220472316735377360L)
            {
                flag = true;
            }
            reader.BaseStream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            return flag;
        }

        /// <summary>
        /// Determines whether the input stream is a valid compound file.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>
        /// <see langword="true" /> if file is valid; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidCompoundFile(BinaryReader reader)
        {
            reader.BaseStream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            return (reader.ReadUInt64() == 16220472316735377360L);
        }

        /// <summary>
        /// Makes the continuous sectors.
        /// </summary>
        /// <param name="input">The input sector number list.</param>
        /// <returns>Returns the merged sector number list.</returns>
        private List<ContinuousSectors> MakeSectors(List<int> input)
        {
            ContinuousSectors sectors = new ContinuousSectors(0, 0);
            List<ContinuousSectors> list = new List<ContinuousSectors>();
            for (int i = 0; i < input.Count; i++)
            {
                if (sectors.SectorCount == 0)
                {
                    sectors.StartSectorIndex = input[i];
                    sectors.SectorCount = 1;
                }
                else if (input[i] == (sectors.SectorCount + sectors.StartSectorIndex))
                {
                    sectors.SectorCount++;
                }
                else
                {
                    list.Add(sectors);
                    sectors.SectorCount = 0;
                    i--;
                }
            }
            list.Add(sectors);
            return list;
        }

        /// <summary>
        /// Moves current stream position to the specified mini sector number.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="reader">The reader.</param>
        private void MoveToMiniSector(int index, BinaryReader reader)
        {
            int num = SectorSize / MiniSectorSize;
            int num2 = (int) Math.Floor((double) (((float) index) / ((float) num)));
            List<int> sectorList = this._fatList.GetSectorList(this._directoryEntries[0].StartSector);
            this.MoveToSector(sectorList[num2], reader);
            long num3 = (index % num) * MiniSectorSize;
            reader.BaseStream.Seek(num3, (SeekOrigin) SeekOrigin.Current);
        }

        /// <summary>
        /// Moves current stream position to the specified mini sector number.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="writer">The writer.</param>
        private void MoveToMiniSector(int index, BinaryWriter writer)
        {
            int num = SectorSize / MiniSectorSize;
            int num2 = (int) Math.Floor((double) (((float) index) / ((float) num)));
            List<int> sectorList = this._fatList.GetSectorList(this._directoryEntries[0].StartSector);
            this.MoveToSector(sectorList[num2], writer);
            long num3 = (index % num) * MiniSectorSize;
            writer.BaseStream.Seek(num3, (SeekOrigin) SeekOrigin.Current);
        }

        /// <summary>
        /// Moves current stream position to the specified sector number.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="reader">The reader.</param>
        private void MoveToSector(int index, BinaryReader reader)
        {
            long num = CompoundFileHeader.Size + (SectorSize * index);
            reader.BaseStream.Seek(num, (SeekOrigin) SeekOrigin.Begin);
        }

        /// <summary>
        /// Moves current stream position to the specified sector number.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="writer">The writer.</param>
        private void MoveToSector(int index, BinaryWriter writer)
        {
            long num = CompoundFileHeader.Size + (SectorSize * index);
            writer.BaseStream.Seek(num, (SeekOrigin) SeekOrigin.Begin);
        }

        /// <summary>
        /// Search all removing the specified directory entry and all its child entries.
        /// </summary>
        /// <param name="id">The id.</param>
        private void PendingRemoveDirectoryEntries(int id)
        {
            DirectoryEntry entry = this._directoryEntries[id];
            if (((entry.Type == CompoundFileObjectType.RootStorage) || (entry.Type == CompoundFileObjectType.Storage)) && (entry.ChildID != DirectoryEntry.NoStream))
            {
                List<int> ids = new List<int> {
                    entry.ChildID
                };
                this.GetSiblingDirectoryEntryIds(entry.ChildID, ids);
                for (int i = 0; i < ids.Count; i++)
                {
                    this.PendingRemoveDirectoryEntries(ids[i]);
                }
            }
            this._removingDirectoryEntries.Add(id);
        }

        /// <summary>
        /// Populate all stream fields for the compound file.
        /// </summary>
        private void PopulateStreamFields()
        {
            DirectoryEntry entry = this._directoryEntries["Root Entry"];
            if (this._directoryEntries.IndexOf("Root Entry") != 0)
            {
                throw new Exception("The root storage entry MUST exist and proceed all entries.");
            }
            if (((entry.Type != CompoundFileObjectType.RootStorage) || (entry.LeftSiblingID != DirectoryEntry.NoStream)) || (entry.RightSiblingID != DirectoryEntry.NoStream))
            {
                throw new ArgumentException("Properties of root entry are not valid for compound file.");
            }
            if ((this._directoryEntries.Count > 1) && (entry.ChildID == DirectoryEntry.NoStream))
            {
                throw new Exception("The child id of root directory entry was not set correctly.");
            }
            this.FillEmptyDirectoryEntries();
            this._difatList = new FileAllocationTable();
            this._fatList = new FileAllocationTable();
            this._miniFatList = new FileAllocationTable();
            int sectorCount = (int) Math.Ceiling((double) ((this._directoryEntries.Count * 128.0) / ((double) SectorSize)));
            int num2 = this.GetTotalUserDataSectorCount() + sectorCount;
            int num3 = (int) Math.Ceiling((double) (((double) this.GetTotalUserDataMiniSectorCount()) / ((double) (SectorSize / MiniSectorSize))));
            int num4 = num2 + num3;
            int num5 = SectorSize / 4;
            int num6 = SectorSize / 4;
            int num7 = (SectorSize - 4) / 4;
            int num8 = (int) Math.Ceiling((double) (((double) num4) / ((double) num5)));
            int num9 = (int) Math.Ceiling((double) (((double) Math.Max(0, num8 - CompoundFileHeader.MaxDifatArrayLength)) / ((double) num7)));
            int num10 = (int) Math.Ceiling((double) (((double) this.GetTotalUserDataMiniSectorCount()) / ((double) num6)));
            for (int i = (((num2 + num3) + num8) + num9) + num10; i != num4; i = (((num2 + num3) + num8) + num9) + num10)
            {
                num4 = i;
                num8 = (int) Math.Ceiling((double) (((double) num4) / ((double) num5)));
                num9 = (int) Math.Ceiling((double) (((double) Math.Max(0, num8 - CompoundFileHeader.MaxDifatArrayLength)) / ((double) num7)));
            }
            this.AllocateUserDataSectors();
            int num12 = -2;
            if (num3 > 0)
            {
                int num13 = this._fatList.Allocate(num3, SectorType.Data);
                entry.StartSector = num13;
                entry.StreamSize = this.GetTotalUserDataMiniSectorCount() * MiniSectorSize;
                num12 = this._fatList.Allocate(num10, SectorType.Data);
            }
            int num14 = this._fatList.Allocate(sectorCount, SectorType.Data);
            int num15 = this._fatList.Allocate(num8, SectorType.FAT);
            int num16 = this._fatList.Allocate(num9, SectorType.DIFAT);
            for (int j = num15; j < (num15 + num8); j++)
            {
                this._difatList.Add(j);
            }
            this._header = new CompoundFileHeader();
            this._header.FirstMiniFatSector = num12;
            this._header.MiniFatSectorCount = num10;
            this._header.FirstDirectorySector = num14;
            this._header.FatSectorCount = this._difatList.Count;
            this._header.DifatSectorCount = num9;
            this._header.FirstDifatSector = (num9 > 0) ? num16 : -2;
            if (num9 > 0)
            {
                this._header.DifatArray = new int[CompoundFileHeader.MaxDifatArrayLength];
                for (int k = 0; k < this._header.DifatArray.Length; k++)
                {
                    this._header.DifatArray[k] = this._difatList[k];
                }
            }
            else
            {
                this._header.DifatArray = this._difatList.ToArray();
            }
        }

        /// <summary>
        /// Reads the compound file from the specified stream.
        /// </summary>
        public bool Read(Stream stream)
        {
            BinaryReader reader = null;
            try
            {
                reader = new BinaryReader(stream, Encoding.Unicode);
                if (!this.IsValidCompoundFile(reader))
                {
                    return false;
                }
                this.ReadHeader(reader);
                this.ReadFatList(reader);
                this.ReadDirectoryEntries(reader);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads the directory entries from the input stream.
        /// </summary>
        private void ReadDirectoryEntries(BinaryReader reader)
        {
            this._directoryEntries.Clear();
            int firstDirectorySector = this._header.FirstDirectorySector;
            List<int> sectorList = this._fatList.GetSectorList(firstDirectorySector);
            for (int i = 0; i < sectorList.Count; i++)
            {
                this.MoveToSector(sectorList[i], reader);
                this._directoryEntries.Read(reader, SectorSize / 0x80);
            }
            DirectoryEntry entry = this._directoryEntries[0];
            if (entry.Name == "Root Entry".Replace(" ", ""))
            {
                entry.Name = "Root Entry";
            }
            if (entry.Name != "Root Entry")
            {
                throw new Exception("The input stream is not a valid compound file stream.");
            }
            for (int j = 1; j < this._directoryEntries.Count; j++)
            {
                DirectoryEntry directoryEntry = this._directoryEntries[j];
                if (directoryEntry.Type == CompoundFileObjectType.Stream)
                {
                    directoryEntry.Bytes = this.GetDirectoryEntryBytes(directoryEntry, reader);
                }
            }
        }

        /// <summary>
        /// Reads the file allocation table from the input stream.
        /// </summary>
        private void ReadFatList(BinaryReader reader)
        {
            this._difatList.AddRange(this._header.DifatArray);
            int num = this._header.FatSectorCount - this._header.DifatArray.Length;
            int firstDifatSector = this._header.FirstDifatSector;
            int num3 = (SectorSize - 4) / 4;
            while (num > 0)
            {
                this.MoveToSector(firstDifatSector, reader);
                this._difatList.Read(reader, Math.Min(num3, num));
                num -= Math.Min(num3, num);
                if (num > 0)
                {
                    firstDifatSector = reader.ReadInt32();
                }
            }
            for (int i = 0; i < this._difatList.Count; i++)
            {
                this.MoveToSector(this._difatList[i], reader);
                this._fatList.Read(reader, SectorSize / 4);
            }
            List<int> sectorList = this._fatList.GetSectorList(this._header.FirstMiniFatSector);
            for (int j = 0; j < sectorList.Count; j++)
            {
                this.MoveToSector(sectorList[j], reader);
                this._miniFatList.Read(reader, SectorSize / 4);
            }
        }

        /// <summary>
        /// Reads the header of the compound file. 
        /// </summary>
        private void ReadHeader(BinaryReader reader)
        {
            reader.BaseStream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
            this._header.Read(reader);
            SectorSize = this._header.SectorSize;
            MiniSectorSize = this._header.MiniSectorSize;
        }

        /// <summary>
        /// Removes the storage.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="parentName">Name of the parent.</param>
        /// <returns><see langword="true" /> if the storage is removed successfully; otherwise <c>false</c>.</returns>
        public bool RemoveStorage(string name, string parentName)
        {
            int num = -1;
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                DirectoryEntry entry = this._directoryEntries[i];
                if ((entry.Name == parentName) && ((entry.Type == CompoundFileObjectType.Storage) || (entry.Type == CompoundFileObjectType.RootStorage)))
                {
                    num = i;
                    break;
                }
            }
            if (num == -1)
            {
                return false;
            }
            if (this._directoryEntries[num].ChildID == DirectoryEntry.NoStream)
            {
                return false;
            }
            int id = -1;
            List<int> ids = new List<int> {
                this._directoryEntries[num].ChildID
            };
            this.GetSiblingDirectoryEntryIds(this._directoryEntries[num].ChildID, ids);
            for (int j = 0; j < ids.Count; j++)
            {
                DirectoryEntry entry2 = this._directoryEntries[ids[j]];
                if ((entry2.Name == name) && (entry2.Type == CompoundFileObjectType.Storage))
                {
                    id = ids[j];
                    ids.RemoveAt(j);
                    break;
                }
            }
            if (id == -1)
            {
                return false;
            }
            if (ids.Count == 0)
            {
                this._directoryEntries[num].ChildID = DirectoryEntry.NoStream;
            }
            else
            {
                this._directoryEntries[num].ChildID = ids[(ids.Count - 1) / 2];
                this.ResetAllSiblingDirectoryEntryIds(ids);
                this.BalanceSiblingDirectoryEntryIds((ids.Count - 1) / 2, 0, ids.Count - 1, ids);
            }
            this.StartRemoveDirectoryEntries();
            this.PendingRemoveDirectoryEntries(id);
            this.EndRemoveDirectoryEntries();
            return true;
        }

        /// <summary>
        /// Resets all sibling directory entry ids.
        /// </summary>
        /// <param name="ids">The ids.</param>
        private void ResetAllSiblingDirectoryEntryIds(List<int> ids)
        {
            for (int i = 0; i < ids.Count; i++)
            {
                this._directoryEntries[ids[i]].LeftSiblingID = DirectoryEntry.NoStream;
                this._directoryEntries[ids[i]].RightSiblingID = DirectoryEntry.NoStream;
            }
        }

        /// <summary>
        /// Starts to remove the directory entries from the compound file.
        /// </summary>
        private void StartRemoveDirectoryEntries()
        {
            this._removingDirectoryEntries.Clear();
        }

        /// <summary>
        /// Writes the compound file to the specified stream.
        /// </summary>
        public bool Write(Stream stream)
        {
            BinaryWriter writer = null;
            try
            {
                writer = new BinaryWriter(stream, Encoding.Unicode);
                this.PopulateStreamFields();
                this.WriteInternal(writer);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Write this compound file to the output stream.
        /// </summary>
        private void WriteInternal(BinaryWriter writer)
        {
            this._header.Write(writer);
            for (int i = 0; i < this._directoryEntries.Count; i++)
            {
                if ((this._directoryEntries[i].Type == CompoundFileObjectType.Stream) && (this._directoryEntries[i].StartSector != -2))
                {
                    if (this._directoryEntries[i].IsMiniSteam())
                    {
                        this.MoveToMiniSector(this._directoryEntries[i].StartSector, writer);
                    }
                    else
                    {
                        this.MoveToSector(this._directoryEntries[i].StartSector, writer);
                    }
                    writer.Write(this._directoryEntries[i].Bytes);
                }
            }
            this.MoveToSector(this._header.FirstDirectorySector, writer);
            for (int j = 0; j < this._directoryEntries.Count; j++)
            {
                this._directoryEntries[j].Write(writer);
            }
            if (this._header.FirstMiniFatSector != -2)
            {
                this.MoveToSector(this._header.FirstMiniFatSector, writer);
                this._miniFatList.Write(writer, 0, this._miniFatList.Count);
            }
            int startIndex = 0;
            for (int k = 0; k < this._difatList.Count; k++)
            {
                this.MoveToSector(this._difatList[k], writer);
                int count = SectorSize / 4;
                this._fatList.Write(writer, startIndex, count);
                startIndex += count;
            }
            startIndex = CompoundFileHeader.MaxDifatArrayLength;
            int firstDifatSector = this._header.FirstDifatSector;
            while (startIndex < this._header.FatSectorCount)
            {
                this.MoveToSector(firstDifatSector, writer);
                int num7 = (SectorSize - 4) / 4;
                this._difatList.Write(writer, startIndex, num7);
                startIndex += num7;
                if (startIndex < this._header.FatSectorCount)
                {
                    firstDifatSector++;
                    writer.Write(firstDifatSector);
                }
                else
                {
                    writer.Write(-1);
                }
            }
            writer.Flush();
        }

        /// <summary>
        /// Represents some continuous sectors.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct ContinuousSectors
        {
            /// <summary>
            /// An <b>int</b> value indicates the start block index.
            /// </summary>
            public int StartSectorIndex;
            /// <summary>
            /// An <b>int</b> value indicates the count of continuous sectors.
            /// </summary>
            public int SectorCount;
            /// <summary>
            /// Initialize a <see cref="T:Dt.Xls.Biff.CompoundFile.ContinuousSectors" /> struct with its start index
            /// and block count.
            /// </summary>
            public ContinuousSectors(int start, int count)
            {
                this.StartSectorIndex = start;
                this.SectorCount = count;
            }
        }
    }
}

