#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Core.Mask
{
    internal class RegExpPokeableReader
    {
        #region 成员变量
        List<TextReader> readers;
        #endregion

        #region 构造方法
        public RegExpPokeableReader()
        {
            this.readers = new List<TextReader>();
        }

        public RegExpPokeableReader(TextReader firstReader)
            : this()
        {
            this.PokeReader(firstReader);
        }
        #endregion

        #region 外部方法
        public int Peek()
        {
            while (this.readers.Count > 0)
            {
                int num = this.readers[0].Peek();
                if (num >= 0)
                {
                    return num;
                }
                this.readers.RemoveAt(0);
            }
            return -1;
        }

        public void Poke(string nextInput)
        {
            using (StringReader reader = new StringReader(nextInput))
            {
                this.PokeReader(reader);
            }
        }

        public void PokeReader(TextReader reader)
        {
            this.readers.Insert(0, reader);
        }

        public int Read()
        {
            while (this.readers.Count > 0)
            {
                int num = this.readers[0].Read();
                if (num >= 0)
                {
                    return num;
                }
                this.readers.RemoveAt(0);
            }
            return -1;
        }
        #endregion
    }
}

