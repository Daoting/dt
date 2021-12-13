#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// Cmap table
    /// more info: http://www.microsoft.com/typography/default.mspx
    /// </summary>
    internal class CMapTable : OpenTypeFontTable
    {
        private Dictionary<int, int> macMap;
        private Dictionary<int, int> symbolMap;
        private Dictionary<int, int> ucs4Map;
        private Dictionary<int, int> unicodeMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CMapTable" /> class.
        /// </summary>
        public CMapTable()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:CMapTable" /> class.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="checkSum">The check sum.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="length">The length.</param>
        /// <param name="reader">The reader.</param>
        public CMapTable(string tag, long checkSum, long offset, long length, OpenTypeFontReader reader) : base(tag, checkSum, offset, length, reader)
        {
        }

        /// <summary>
        /// Loads the data.
        /// </summary>
        public override void LoadData()
        {
            if (base.reader.ReadUSHORT() != 0)
            {
                throw new PdfReadFontException("cmap table version must be 0");
            }
            int num = base.reader.ReadUSHORT();
            for (int i = 0; i < num; i++)
            {
                long pos = 0L;
                int num4 = base.reader.ReadUSHORT();
                int num5 = base.reader.ReadUSHORT();
                long num6 = base.reader.ReadULONG();
                if ((num4 == 1) && (num5 == 0))
                {
                    pos = base.reader.Position;
                    this.macMap = this.ReadMap(num6);
                }
                else if (num4 == 3)
                {
                    switch (num5)
                    {
                        case 0:
                            pos = base.reader.Position;
                            this.symbolMap = this.ReadMap(num6);
                            break;

                        case 1:
                            pos = base.reader.Position;
                            this.unicodeMap = this.ReadMap(num6);
                            break;

                        case 10:
                            goto Label_00CF;
                    }
                }
                goto Label_00E9;
            Label_00CF:
                pos = base.reader.Position;
                this.ucs4Map = this.ReadMap(num6);
            Label_00E9:
                if (pos > 0L)
                {
                    base.reader.Seek(pos);
                }
            }
            base.LoadData();
        }

        /// <summary>
        /// Reads the format0.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> ReadFormat0()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            base.reader.Skip(4L);
            for (int i = 0; i < 0x100; i++)
            {
                dictionary.Add(i, base.reader.ReadBYTE());
            }
            return dictionary;
        }

        /// <summary>
        /// Reads the format12.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> ReadFormat12()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            base.reader.Skip(6L);
            int num = base.reader.ReadLONG();
            for (int i = 0; i < num; i++)
            {
                int num3 = base.reader.ReadLONG();
                int num4 = base.reader.ReadLONG();
                int num5 = base.reader.ReadLONG();
                int num6 = num3;
                while (num6 < num4)
                {
                    dictionary[i] = num5;
                    num6++;
                    num5++;
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Reads the format4.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> ReadFormat4()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            int num = base.reader.ReadUSHORT();
            base.reader.Skip(2L);
            int num2 = base.reader.ReadUSHORT() / 2;
            base.reader.Skip(6L);
            int[] numArray = new int[num2];
            for (int i = 0; i < num2; i++)
            {
                numArray[i] = base.reader.ReadUSHORT();
            }
            base.reader.Skip(2L);
            int[] numArray2 = new int[num2];
            for (int j = 0; j < num2; j++)
            {
                numArray2[j] = base.reader.ReadUSHORT();
            }
            int[] numArray3 = new int[num2];
            for (int k = 0; k < num2; k++)
            {
                numArray3[k] = base.reader.ReadUSHORT();
            }
            int[] numArray4 = new int[num2];
            for (int m = 0; m < num2; m++)
            {
                numArray4[m] = base.reader.ReadUSHORT();
            }
            int[] numArray5 = new int[((num / 2) - 8) - (num2 * 4)];
            for (int n = 0; n < numArray5.Length; n++)
            {
                numArray5[n] = base.reader.ReadUSHORT();
            }
            for (int num8 = 0; num8 < num2; num8++)
            {
                for (int num9 = numArray2[num8]; (num9 <= numArray[num8]) && (num9 < 0xffff); num9++)
                {
                    int num10;
                    if (numArray4[num8] == 0)
                    {
                        num10 = (num9 + numArray3[num8]) & 0xffff;
                    }
                    else
                    {
                        int index = (((num8 + (numArray4[num8] / 2)) - num2) + num9) - numArray2[num8];
                        if (index >= numArray5.Length)
                        {
                            continue;
                        }
                        num10 = (numArray5[index] + numArray3[num8]) & 0xffff;
                    }
                    if (this.symbolMap != null)
                    {
                        dictionary[((num9 & 0xff00) == 0xf000) ? (num9 & 0xff) : num9] = num10;
                    }
                    else
                    {
                        dictionary[num9] = num10;
                    }
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Reads the format6.
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, int> ReadFormat6()
        {
            Dictionary<int, int> dictionary = new Dictionary<int, int>();
            base.reader.Skip(4L);
            int num = base.reader.ReadUSHORT();
            int num2 = base.reader.ReadUSHORT();
            for (int i = 0; i < num2; i++)
            {
                dictionary[i + num] = base.reader.ReadUSHORT();
            }
            return dictionary;
        }

        /// <summary>
        /// Reads the map.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <returns></returns>
        private Dictionary<int, int> ReadMap(long pos)
        {
            base.reader.Seek(pos);
            switch (base.reader.ReadUSHORT())
            {
                case 4:
                    return this.ReadFormat4();

                case 6:
                    return this.ReadFormat6();

                case 12:
                    return this.ReadFormat12();

                case 0:
                    return this.ReadFormat0();
            }
            throw new PdfReadFontException("doesn't support camp table format. support 0, 4, 6 and 12.");
        }

        /// <summary>
        /// Gets the current map.
        /// </summary>
        /// <value>The current map.</value>
        public Dictionary<int, int> CurrentMap
        {
            get
            {
                base.CheckLoad();
                if (this.UCS4Map != null)
                {
                    return this.UCS4Map;
                }
                bool flag = this.SymbolMap != null;
                if (!flag && (this.UnicodeMap != null))
                {
                    return this.UnicodeMap;
                }
                if (flag && (this.SymbolMap != null))
                {
                    return this.SymbolMap;
                }
                if (this.UnicodeMap != null)
                {
                    return this.UnicodeMap;
                }
                if (this.MacMap != null)
                {
                    return this.MacMap;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:System.Int32" /> at the specified index.
        /// </summary>
        /// <value></value>
        public int this[int index]
        {
            get
            {
                base.CheckLoad();
                if ((this.UCS4Map != null) && this.UCS4Map.ContainsKey(index))
                {
                    return this.UCS4Map[index];
                }
                bool flag = this.SymbolMap != null;
                if ((!flag && (this.UnicodeMap != null)) && this.UnicodeMap.ContainsKey(index))
                {
                    return this.UnicodeMap[index];
                }
                if (flag && (this.SymbolMap != null))
                {
                    if ((index & 0xf000) != 0)
                    {
                        return -1;
                    }
                    int key = index;
                    key |= 0xf000;
                    if (this.SymbolMap.ContainsKey(key))
                    {
                        return this.SymbolMap[key];
                    }
                }
                if ((this.UnicodeMap != null) && this.UnicodeMap.ContainsKey(index))
                {
                    return this.UnicodeMap[index];
                }
                if ((this.MacMap != null) && this.MacMap.ContainsKey(index))
                {
                    return this.MacMap[index];
                }
                return -1;
            }
        }

        /// <summary>
        /// Gets the mac map.
        /// </summary>
        /// <value>The mac map.</value>
        public Dictionary<int, int> MacMap
        {
            get
            {
                base.CheckLoad();
                return this.macMap;
            }
        }

        /// <summary>
        /// Gets the symbol map.
        /// </summary>
        /// <value>The symbol map.</value>
        public Dictionary<int, int> SymbolMap
        {
            get
            {
                base.CheckLoad();
                return this.symbolMap;
            }
        }

        /// <summary>
        /// Gets the UC s4 map.
        /// </summary>
        /// <value>The UC s4 map.</value>
        public Dictionary<int, int> UCS4Map
        {
            get
            {
                base.CheckLoad();
                return this.ucs4Map;
            }
        }

        /// <summary>
        /// Gets the unicode map.
        /// </summary>
        /// <value>The unicode map.</value>
        public Dictionary<int, int> UnicodeMap
        {
            get
            {
                base.CheckLoad();
                return this.unicodeMap;
            }
        }
    }
}

