#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The Ascii encoding
    /// </summary>
    internal class PdfASCIIEncoding : PdfEncodingBase
    {
        internal static readonly PdfASCIIEncoding Instance = new PdfASCIIEncoding();

        /// <summary>
        /// Gets the byte count internal.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        protected override int GetByteCountInternal(char[] chars, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// Gets the bytes internal.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="charIndex">Index of the char.</param>
        /// <param name="charCount">The char count.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="byteIndex">Index of the byte.</param>
        /// <returns></returns>
        protected override int GetBytesInternal(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            byte[] sourceArray = new byte[charCount];
            int num = 0;
            for (int i = charIndex; i < (charIndex + charCount); i++)
            {
                sourceArray[num++] = (byte) chars[i];
            }
            Array.Copy(sourceArray, 0, bytes, byteIndex, sourceArray.Length);
            return charCount;
        }

        /// <summary>
        /// Gets the char count internal.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        protected override int GetCharCountInternal(byte[] bytes, int index, int count)
        {
            return count;
        }

        /// <summary>
        /// Gets the chars internal.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="byteIndex">Index of the byte.</param>
        /// <param name="byteCount">The byte count.</param>
        /// <param name="chars">The chars.</param>
        /// <param name="charIndex">Index of the char.</param>
        /// <returns></returns>
        protected override int GetCharsInternal(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            char[] sourceArray = new char[byteCount];
            int num = 0;
            for (int i = byteIndex; i < (byteIndex + byteCount); i++)
            {
                sourceArray[num++] = (char)(bytes[i] & 0xff);
            }
            Array.Copy(sourceArray, 0, chars, charIndex, sourceArray.Length);
            return sourceArray.Length;
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of bytes produced by encoding the specified number of characters.
        /// </summary>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <returns>
        /// The maximum number of bytes produced by encoding the specified number of characters.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="charCount" /> is less than zero.
        /// </exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public override int GetMaxByteCount(int charCount)
        {
            return Encoding.Unicode.GetMaxByteCount(charCount);
        }

        /// <summary>
        /// When overridden in a derived class, calculates the maximum number of characters produced by decoding the specified number of bytes.
        /// </summary>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <returns>
        /// The maximum number of characters produced by decoding the specified number of bytes.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="byteCount" /> is less than zero.
        /// </exception>
        /// <exception cref="T:System.Text.DecoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.DecoderFallback" /> is set to <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public override int GetMaxCharCount(int byteCount)
        {
            return Encoding.Unicode.GetMaxCharCount(byteCount);
        }
    }
}

