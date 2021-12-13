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
using System.Reflection;
#endregion

namespace Dt.Xls.Biff
{
    internal class FileAllocationTable
    {
        private List<int> _innerList = new List<int>();

        /// <summary>
        /// Adds the specified sector number to file allocation table.
        /// </summary>
        /// <param name="sectorNumber">The sector number.</param>
        public void Add(int sectorNumber)
        {
            this._innerList.Add(sectorNumber);
        }

        /// <summary>
        /// Adds the range of file allocation table.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void AddRange(IEnumerable<int> collection)
        {
            this._innerList.AddRange(collection);
        }

        /// <summary>
        /// Allocates the specified number and type of sectors.
        /// </summary>
        /// <param name="sectorCount">The sector count.</param>
        /// <param name="type">The sector type.</param>
        /// <returns>Returns the first sector number.</returns>
        public int Allocate(int sectorCount, SectorType type)
        {
            if (sectorCount == 0)
            {
                return -2;
            }
            int num = this._innerList.Count;
            if (type == SectorType.Data)
            {
                for (int i = 0; i < (sectorCount - 1); i++)
                {
                    this._innerList.Add(this._innerList.Count + 1);
                }
                this._innerList.Add(-2);
                return num;
            }
            if (type == SectorType.FAT)
            {
                for (int j = 0; j < sectorCount; j++)
                {
                    this._innerList.Add(-3);
                }
                return num;
            }
            if (type == SectorType.DIFAT)
            {
                for (int k = 0; k < sectorCount; k++)
                {
                    this._innerList.Add(-4);
                }
            }
            return num;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned file allocation table.</returns>
        public FileAllocationTable Clone()
        {
            FileAllocationTable table = new FileAllocationTable();
            List<int> list = new List<int>();
            foreach (int num in this._innerList)
            {
                list.Add(num);
            }
            table._innerList = list;
            return table;
        }

        /// <summary>
        /// Gets the sector number list by its start sector number.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <returns>Returns the sector number list.</returns>
        public List<int> GetSectorList(int startIndex)
        {
            List<int> list = new List<int>();
            for (int i = startIndex; (i <= -6) || (i > -1); i = this._innerList[i])
            {
                list.Add(i);
            }
            return list;
        }

        /// <summary>
        /// Reads the specified count of file allocation table from the stream.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="count">The count.</param>
        public void Read(BinaryReader reader, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this._innerList.Add(reader.ReadInt32());
            }
        }

        /// <summary>
        /// Removes the range of file allocation table.
        /// </summary>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        public void RemoveRange(int startIndex, int count)
        {
            this._innerList.RemoveRange(startIndex, count);
        }

        /// <summary>
        /// Convert the file allocation table to array.
        /// </summary>
        /// <returns>Returns the array.</returns>
        public int[] ToArray()
        {
            return this._innerList.ToArray();
        }

        /// <summary>
        /// Writes the specified file allocation table to the stream.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        public void Write(BinaryWriter writer, int startIndex, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if ((startIndex >= 0) && (startIndex < this._innerList.Count))
                {
                    writer.Write(this._innerList[startIndex]);
                    startIndex++;
                }
                else
                {
                    for (int j = i; j < count; j++)
                    {
                        writer.Write(-1);
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Gets the count of the file allocation table.
        /// </summary>
        public int Count
        {
            get { return  this._innerList.Count; }
        }

        /// <summary>
        /// Gets the next sector number of the specified sector.
        /// </summary>
        /// <value>Returns the next sector number.</value>
        public int this[int index]
        {
            get { return  this._innerList[index]; }
        }
    }
}

