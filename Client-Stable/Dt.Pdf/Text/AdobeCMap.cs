#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Exceptions;
using Dt.Pdf.Utility;
using System;
using System.IO;
using System.Text;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// Adobe Character map object
    /// </summary>
    public class AdobeCMap
    {
        private static readonly object s_AdobeCmapSync = new object();
        private static AdobeCmapBase s_CNS1;
        private static AdobeCmapBase s_GB1;
        private static AdobeCmapBase s_Japan1;
        private static AdobeCmapBase s_Korea1;

        /// <summary>
        /// Gets the glyph index.
        /// </summary>
        /// <param name="c">The char.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        internal static int GetCode(ushort c, string encoding)
        {
            switch (encoding)
            {
                case "UniGB-UCS2-H":
                    return GB1.GetCode(c);

                case "UniCNS-UCS2-H":
                    return CNS1.GetCode(c);

                case "UniJIS-UCS2-H":
                    return Japan1.GetCode(c);

                case "UniKS-UCS2-H":
                    return Korea1.GetCode(c);
            }
            return c;
        }

        /// <summary>
        /// Gets the CNS1 cmap.
        /// </summary>
        /// <value>The CNS1 cmap.</value>
        public static AdobeCmapBase CNS1
        {
            get
            {
                lock (s_AdobeCmapSync)
                {
                    if (s_CNS1 == null)
                    {
                        s_CNS1 = new AdobeCmapCNS1();
                    }
                }
                return s_CNS1;
            }
        }

        /// <summary>
        /// Gets the GB1 cmap.
        /// </summary>
        /// <value>The GB1 cmap.</value>
        public static AdobeCmapBase GB1
        {
            get
            {
                lock (s_AdobeCmapSync)
                {
                    if (s_GB1 == null)
                    {
                        s_GB1 = new AdobeCmapGB1();
                    }
                }
                return s_GB1;
            }
        }

        /// <summary>
        /// Gets the Japan1 cmap.
        /// </summary>
        /// <value>The japan1 cmap.</value>
        public static AdobeCmapBase Japan1
        {
            get
            {
                lock (s_AdobeCmapSync)
                {
                    if (s_Japan1 == null)
                    {
                        s_Japan1 = new AdobeCmapJapan1();
                    }
                }
                return s_Japan1;
            }
        }

        /// <summary>
        /// Gets the Korea1 cmap.
        /// </summary>
        /// <value>The korea1 cmap.</value>
        public static AdobeCmapBase Korea1
        {
            get
            {
                lock (s_AdobeCmapSync)
                {
                    if (s_Korea1 == null)
                    {
                        s_Korea1 = new AdobeCmapKorea1();
                    }
                }
                return s_Korea1;
            }
        }

        /// <summary>
        /// Adobe cmap base object
        /// </summary>
        public class AdobeCmapBase
        {
            internal ushort[,] m_Address;
            protected string m_MapData;

            internal AdobeCmapBase(string name)
            {
                using (Stream stream = FlateUtility.GetResource(name))
                {
                    if (stream == null)
                    {
                        throw new PdfArgumentNullException("Embedded resource " + name + " cannot be found.");
                    }
                    byte[] buffer = new byte[(int) stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    buffer = FlateUtility.FlateDecode(buffer, true);
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i += 2)
                    {
                        builder.Append((char) ((buffer[i] << 8) + buffer[i + 1]));
                    }
                    this.InitAddress(builder.ToString());
                }
            }

            private int CheckSS(string s1, string s2)
            {
                for (int i = 0; i < s1.Length; i++)
                {
                    if (s1[i] != s2[i])
                    {
                        return i;
                    }
                }
                return -1;
            }

            internal int GetCode(ushort c)
            {
                int num = 0;
                int num2 = c >> 8;
                int num3 = this.m_Address[num2, 0];
                int num4 = this.m_Address[num2, 1];
                for (int i = num4; i < this.m_MapData.Length; i += 2)
                {
                    int num6 = this.m_MapData[i] >> 8;
                    int num7 = this.m_MapData[i] & '\x00ff';
                    int num8 = this.m_MapData[i + 1];
                    num3 += num6;
                    if (c < num3)
                    {
                        return num;
                    }
                    if (((num8 != 0) && (c >= num3)) && (c <= (num3 + num7)))
                    {
                        return (num8 + (c - num3));
                    }
                    num3 += num7 + 1;
                }
                return num;
            }

            internal void InitAddress(string cmap)
            {
                this.m_MapData = cmap;
                this.m_Address = new ushort[0x100, 2];
                int num = 0;
                int num2 = 0xffff;
                for (int i = 0; i < this.m_MapData.Length; i += 2)
                {
                    int num4 = num;
                    int num5 = this.m_MapData[i] >> 8;
                    int num6 = this.m_MapData[i] & '\x00ff';
                    num += num5;
                    int num7 = num >> 8;
                    if (num7 != (num2 >> 8))
                    {
                        this.m_Address[num7, 0] = (ushort) num4;
                        this.m_Address[num7, 1] = (ushort) i;
                        num2 = num;
                    }
                    num += num6 + 1;
                }
            }
        }

        /// <summary>
        /// CNS1 cmap
        /// </summary>
        public class AdobeCmapCNS1 : AdobeCMap.AdobeCmapBase
        {
            internal AdobeCmapCNS1() : base("CNS1.cmap")
            {
            }
        }

        /// <summary>
        /// GB1 cmap
        /// </summary>
        public class AdobeCmapGB1 : AdobeCMap.AdobeCmapBase
        {
            internal AdobeCmapGB1() : base("GB1.cmap")
            {
            }
        }

        /// <summary>
        /// Japan1 cmap
        /// </summary>
        public class AdobeCmapJapan1 : AdobeCMap.AdobeCmapBase
        {
            internal AdobeCmapJapan1() : base("Japan1.cmap")
            {
            }
        }

        /// <summary>
        /// Korea1 cmap
        /// </summary>
        public class AdobeCmapKorea1 : AdobeCMap.AdobeCmapBase
        {
            internal AdobeCmapKorea1() : base("Korea1.cmap")
            {
            }
        }
    }
}

