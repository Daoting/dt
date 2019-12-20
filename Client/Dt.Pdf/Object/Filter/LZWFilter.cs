#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Utility;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object.Filter
{
    /// <summary>
    /// LZW filter for Pdf file
    /// </summary>
    public class LZWFilter : PdfFilter
    {
        private const int Clear_Table = 0x100;
        private const int EOD = 0x101;
        private const int StartCode = 0x102;

        /// <summary>
        /// Decodes the inner.
        /// </summary>
        /// <param name="compressedData">The compressed data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected override void DecodeInner(Stream compressedData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encodes the inner.
        /// </summary>
        /// <param name="rawData">The raw data.</param>
        /// <param name="result">The result.</param>
        /// <param name="options">The options.</param>
        protected override void EncodeInner(Stream rawData, Stream result, Dictionary<PdfName, PdfObjectBase> options)
        {
            StreamNBitWriter writer = new StreamNBitWriter(result, 9);
            writer.Write(0x100);
            if (rawData.Length > 0L)
            {
                int num3;
                int num = 0x102;
                PathTree<byte, int> tree = new PathTree<byte, int>();
                for (int i = 0; i < 0x100; i++)
                {
                    tree.Root[(byte) i] = new PathTreeNode<byte, int>(i);
                }
                PathTreeNode<byte, int> node = tree.Root[(byte) rawData.ReadByte()];
                while ((num3 = rawData.ReadByte()) != -1)
                {
                    byte path = (byte) num3;
                    if (node.ContainsPath(path))
                    {
                        node = node[path];
                    }
                    else
                    {
                        writer.Write(node.Data);
                        node[path] = new PathTreeNode<byte, int>(num++);
                        node = tree.Root[path];
                    }
                    if (num == 0x1000)
                    {
                        writer.Write(0x100);
                        tree = new PathTree<byte, int>();
                        for (int j = 0; j < 0x100; j++)
                        {
                            tree.Root[(byte) j] = new PathTreeNode<byte, int>(j);
                        }
                        num = 0x102;
                        writer.N = 9;
                    }
                    else if (num > 0x7ff)
                    {
                        writer.N = 12;
                    }
                    else
                    {
                        if (num > 0x3ff)
                        {
                            writer.N = 11;
                            continue;
                        }
                        if (num > 0x1ff)
                        {
                            writer.N = 10;
                        }
                    }
                }
                writer.Write(tree.GetPath(node));
            }
            writer.Write(0x101);
            writer.Flush();
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override PdfName GetName()
        {
            return PdfName.LZWDecode;
        }
    }
}

