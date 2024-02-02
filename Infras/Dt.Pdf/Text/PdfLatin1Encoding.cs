#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The latin1 encoding
    /// </summary>
    internal class PdfLatin1Encoding : PdfEncodingBase
    {
        internal static Dictionary<char, byte> arrayByteBestFit = new Dictionary<char, byte>();
        internal static readonly char[] arrayCharBestFit = new char[] { 
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\x000e', '\x000f', 
            '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', '\x001c', '\x001d', '\x001e', '\x001f', 
            ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/', 
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?', 
            '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_', 
            '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '\x007f', 
            '€', '�', '‚', 'ƒ', '„', '…', '†', '‡', 'ˆ', '‰', 'Š', '‹', 'Œ', '�', 'Ž', '�', 
            '�', '‘', '’', '“', '”', '•', '–', '—', '˜', '™', 'š', '›', 'œ', '�', 'ž', 'Ÿ', 
            '\x00a0', '\x00a1', '\x00a2', '\x00a3', '\x00a4', '\x00a5', '\x00a6', '\x00a7', '\x00a8', '\x00a9', '\x00aa', '\x00ab', '\x00ac', '\x00ad', '\x00ae', '\x00af', 
            '\x00b0', '\x00b1', '\x00b2', '\x00b3', '\x00b4', '\x00b5', '\x00b6', '\x00b7', '\x00b8', '\x00b9', '\x00ba', '\x00bb', '\x00bc', '\x00bd', '\x00be', '\x00bf', 
            '\x00c0', '\x00c1', '\x00c2', '\x00c3', '\x00c4', '\x00c5', '\x00c6', '\x00c7', '\x00c8', '\x00c9', '\x00ca', '\x00cb', '\x00cc', '\x00cd', '\x00ce', '\x00cf', 
            '\x00d0', '\x00d1', '\x00d2', '\x00d3', '\x00d4', '\x00d5', '\x00d6', '\x00d7', '\x00d8', '\x00d9', '\x00da', '\x00db', '\x00dc', '\x00dd', '\x00de', '\x00df', 
            '\x00e0', '\x00e1', '\x00e2', '\x00e3', '\x00e4', '\x00e5', '\x00e6', '\x00e7', '\x00e8', '\x00e9', '\x00ea', '\x00eb', '\x00ec', '\x00ed', '\x00ee', '\x00ef', 
            '\x00f0', '\x00f1', '\x00f2', '\x00f3', '\x00f4', '\x00f5', '\x00f6', '\x00f7', '\x00f8', '\x00f9', '\x00fa', '\x00fb', '\x00fc', '\x00fd', '\x00fe', '\x00ff'
         };
        internal static readonly PdfASCIIEncoding Instance = new PdfASCIIEncoding();

        /// <summary>
        /// Initializes the <see cref="T:PdfLatin1Encoding" /> class.
        /// </summary>
        static PdfLatin1Encoding()
        {
            for (byte i = 0x80; i < 0xa1; i++)
            {
                char ch = arrayCharBestFit[i];
                if (ch != 0xfffd)
                {
                    arrayByteBestFit[ch] = i;
                }
            }
        }

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
                char ch = chars[i];
                if ((ch < '\x0080') || ((ch > '\x00a0') && (ch <= '\x00ff')))
                {
                    sourceArray[num++] = (byte) ch;
                }
                else
                {
                    sourceArray[num++] = arrayByteBestFit[ch];
                }
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
                sourceArray[num++] = arrayCharBestFit[bytes[i] & 0xff];
            }
            Array.Copy(sourceArray, 0, chars, charIndex, sourceArray.Length);
            return byteCount;
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

