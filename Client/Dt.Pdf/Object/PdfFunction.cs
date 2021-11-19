#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Exceptions;
using Dt.Pdf.Object.Filter;
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// Function of Pdf
    /// </summary>
    public static class PdfFunction
    {
        /// <summary>
        /// Type0s the specified domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="size">The size.</param>
        /// <param name="bitsPerSample">The bits per sample.</param>
        /// <param name="order">The order.</param>
        /// <param name="encode">The encode.</param>
        /// <param name="decode">The decode.</param>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static PdfStream Type0(float[] domain, float[] range, int[] size, int bitsPerSample, int order, float[] encode, float[] decode, byte[] stream)
        {
            if (domain == null)
            {
                throw new PdfArgumentNullException("domain");
            }
            if (size == null)
            {
                throw new PdfArgumentNullException("size");
            }
            PdfVersionDependStream stream2 = new PdfVersionDependStream(PdfFilter.FlateFilter);
            stream2.Properties.Add(PdfName.FunctionType, PdfNumber.Zero);
            stream2.Properties.Add(PdfName.Domain, PdfArray.Convert(domain));
            stream2.Properties.Add(PdfName.Size, PdfArray.Convert(size));
            stream2.Properties.Add(PdfName.BitsPerSample, new PdfNumber((double) bitsPerSample));
            if (range != null)
            {
                stream2.Properties.Add(PdfName.Range, PdfArray.Convert(range));
            }
            if (order != -1)
            {
                stream2.Properties.Add(PdfName.Order, new PdfNumber((double) order));
            }
            if (encode != null)
            {
                stream2.Properties.Add(PdfName.Encode, PdfArray.Convert(encode));
            }
            if (decode != null)
            {
                stream2.Properties.Add(PdfName.Decode, PdfArray.Convert(decode));
            }
            if (stream != null)
            {
                stream2.Psw.WriteBytes(stream);
            }
            stream2.PdfVersion = PdfVersion.PDF1_2;
            return stream2;
        }

        /// <summary>
        /// Type2s the specified domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="n">The n.</param>
        /// <param name="c0">The c0.</param>
        /// <param name="c1">The c1.</param>
        /// <returns></returns>
        public static PdfDictionary Type2(float[] domain, float[] range, float n, float[] c0, float[] c1)
        {
            if (domain == null)
            {
                throw new PdfArgumentNullException("domain");
            }
            PdfVersionDependDictionary dictionary2 = new PdfVersionDependDictionary();
            dictionary2.Add(PdfName.FunctionType, PdfNumber.Two);
            dictionary2.Add(PdfName.Domain, PdfArray.Convert(domain));
            dictionary2.Add(PdfName.N, new PdfNumber((double) n));
            PdfVersionDependDictionary dictionary = dictionary2;
            if (range != null)
            {
                dictionary.Add(PdfName.Range, PdfArray.Convert(range));
            }
            if (c0 != null)
            {
                dictionary.Add(PdfName.C0, PdfArray.Convert(c0));
            }
            if (c1 != null)
            {
                dictionary.Add(PdfName.C1, PdfArray.Convert(c1));
            }
            dictionary.PdfVersion = PdfVersion.PDF1_3;
            return dictionary;
        }

        /// <summary>
        /// Type3s the specified domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="functions">The functions.</param>
        /// <param name="bounds">The bounds.</param>
        /// <param name="encode">The encode.</param>
        /// <returns></returns>
        public static PdfDictionary Type3(float[] domain, float[] range, PdfObjectBase[] functions, float[] bounds, float[] encode)
        {
            if (domain == null)
            {
                throw new PdfArgumentNullException("domain");
            }
            if (functions == null)
            {
                throw new PdfArgumentNullException("functions");
            }
            if (bounds == null)
            {
                throw new PdfArgumentNullException("bounds");
            }
            if (encode == null)
            {
                throw new PdfArgumentNullException("encode");
            }
            PdfVersionDependDictionary dictionary2 = new PdfVersionDependDictionary();
            dictionary2.Add(PdfName.FunctionType, PdfNumber.Three);
            dictionary2.Add(PdfName.Domain, PdfArray.Convert(domain));
            dictionary2.Add(PdfName.Functions, new PdfArray(functions));
            dictionary2.Add(PdfName.Bounds, PdfArray.Convert(bounds));
            dictionary2.Add(PdfName.Encode, PdfArray.Convert(encode));
            PdfVersionDependDictionary dictionary = dictionary2;
            if (range != null)
            {
                dictionary.Add(PdfName.Range, PdfArray.Convert(range));
            }
            dictionary.PdfVersion = PdfVersion.PDF1_3;
            return dictionary;
        }

        /// <summary>
        /// Type4s the specified domain.
        /// </summary>
        /// <param name="domain">The domain.</param>
        /// <param name="range">The range.</param>
        /// <param name="postscript">The postscript.</param>
        /// <returns></returns>
        public static PdfStream Type4(float[] domain, float[] range, string postscript)
        {
            if (domain == null)
            {
                throw new PdfArgumentNullException("domain");
            }
            if (postscript == null)
            {
                throw new PdfArgumentNullException("postscript");
            }
            PdfVersionDependStream stream = new PdfVersionDependStream(PdfFilter.FlateFilter);
            stream.Properties.Add(PdfName.FunctionType, PdfNumber.Four);
            stream.Properties.Add(PdfName.Domain, PdfArray.Convert(domain));
            if (range != null)
            {
                stream.Properties.Add(PdfName.Range, PdfArray.Convert(range));
            }
            stream.Psw.WriteString(postscript);
            stream.PdfVersion = PdfVersion.PDF1_3;
            return stream;
        }
    }
}

