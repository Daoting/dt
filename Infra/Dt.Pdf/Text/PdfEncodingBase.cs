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
using System.Text;
#endregion

namespace Dt.Pdf.Text
{
    /// <summary>
    /// The encoding object base
    /// </summary>
    internal abstract class PdfEncodingBase : Encoding
    {
        public static Dictionary<string, int> CodePages = new Dictionary<string, int>();
        public static Encoding UnicodeBigUnmarked = new UnicodeEncoding(true, false);

        /// <summary>
        /// Initializes the <see cref="T:PdfEncodingBase" /> class.
        /// </summary>
        static PdfEncodingBase()
        {
            CodePages.Add("Windows-1252", 0x4e4);
            CodePages.Add("us-ascii", 0x4e9f);
        }

        protected PdfEncodingBase()
        {
        }

        /// <summary>
        /// When overridden in a derived class, calculates the number of bytes produced by encoding a set of characters from the specified character array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="index">The index of the first character to encode.</param>
        /// <param name="count">The number of characters to encode.</param>
        /// <returns>
        /// The number of bytes produced by encoding the specified characters.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="chars" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero.
        /// -or-
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in <paramref name="chars" />.
        /// </exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public override int GetByteCount(char[] chars, int index, int count)
        {
            if (chars == null)
            {
                throw new ArgumentNullException("chars", "ArgumentNull_Array");
            }
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((chars.Length - index) < count)
            {
                throw new ArgumentOutOfRangeException("chars", "ArgumentOutOfRange_IndexCountBuffer");
            }
            if (chars.Length == 0)
            {
                return 0;
            }
            return this.GetByteCountInternal(chars, index, count);
        }

        /// <summary>
        /// Gets the byte count internal.
        /// </summary>
        /// <param name="chars">The chars.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        protected abstract int GetByteCountInternal(char[] chars, int index, int count);
        /// <summary>
        /// When overridden in a derived class, encodes a set of characters from the specified character array into the specified byte array.
        /// </summary>
        /// <param name="chars">The character array containing the set of characters to encode.</param>
        /// <param name="charIndex">The index of the first character to encode.</param>
        /// <param name="charCount">The number of characters to encode.</param>
        /// <param name="bytes">The byte array to contain the resulting sequence of bytes.</param>
        /// <param name="byteIndex">The index at which to start writing the resulting sequence of bytes.</param>
        /// <returns>
        /// The actual number of bytes written into <paramref name="bytes" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="chars" /> is null.
        /// -or-
        /// <paramref name="bytes" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="charIndex" /> or <paramref name="charCount" /> or <paramref name="byteIndex" /> is less than zero.
        /// -or-
        /// <paramref name="charIndex" /> and <paramref name="charCount" /> do not denote a valid range in <paramref name="chars" />.
        /// -or-
        /// <paramref name="byteIndex" /> is not a valid index in <paramref name="bytes" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="bytes" /> does not have enough capacity from <paramref name="byteIndex" /> to the end of the array to accommodate the resulting bytes.
        /// </exception>
        /// <exception cref="T:System.Text.EncoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.EncoderFallback" /> is set to <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            if ((chars == null) || (bytes == null))
            {
                throw new ArgumentNullException((chars == null) ? "chars" : "bytes", "ArgumentNull_Array");
            }
            if ((charIndex < 0) || (charCount < 0))
            {
                throw new ArgumentOutOfRangeException((charIndex < 0) ? "charIndex" : "charCount", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((chars.Length - charIndex) < charCount)
            {
                throw new ArgumentOutOfRangeException("chars", "ArgumentOutOfRange_IndexCountBuffer");
            }
            if ((byteIndex < 0) || (byteIndex > bytes.Length))
            {
                throw new ArgumentOutOfRangeException("byteIndex", "ArgumentOutOfRange_Index");
            }
            if (chars.Length == 0)
            {
                return 0;
            }
            return this.GetBytesInternal(chars, charIndex, charCount, bytes, byteIndex);
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
        protected abstract int GetBytesInternal(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex);
        /// <summary>
        /// When overridden in a derived class, calculates the number of characters produced by decoding a sequence of bytes from the specified byte array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="index">The index of the first byte to decode.</param>
        /// <param name="count">The number of bytes to decode.</param>
        /// <returns>
        /// The number of characters produced by decoding the specified sequence of bytes.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="bytes" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="index" /> or <paramref name="count" /> is less than zero.
        /// -or-
        /// <paramref name="index" /> and <paramref name="count" /> do not denote a valid range in <paramref name="bytes" />.
        /// </exception>
        /// <exception cref="T:System.Text.DecoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.DecoderFallback" /> is set to <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes", "ArgumentNull_Array");
            }
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((bytes.Length - index) < count)
            {
                throw new ArgumentOutOfRangeException("bytes", "ArgumentOutOfRange_IndexCountBuffer");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            return this.GetCharCountInternal(bytes, index, count);
        }

        /// <summary>
        /// Gets the char count internal.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        protected abstract int GetCharCountInternal(byte[] bytes, int index, int count);
        /// <summary>
        /// When overridden in a derived class, decodes a sequence of bytes from the specified byte array into the specified character array.
        /// </summary>
        /// <param name="bytes">The byte array containing the sequence of bytes to decode.</param>
        /// <param name="byteIndex">The index of the first byte to decode.</param>
        /// <param name="byteCount">The number of bytes to decode.</param>
        /// <param name="chars">The character array to contain the resulting set of characters.</param>
        /// <param name="charIndex">The index at which to start writing the resulting set of characters.</param>
        /// <returns>
        /// The actual number of characters written into <paramref name="chars" />.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="bytes" /> is null.
        /// -or-
        /// <paramref name="chars" /> is null.
        /// </exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="byteIndex" /> or <paramref name="byteCount" /> or <paramref name="charIndex" /> is less than zero.
        /// -or-
        /// <paramref name="charIndex" /> is not a valid index in <paramref name="chars" />.
        /// </exception>
        /// <exception cref="T:System.ArgumentException">
        /// <paramref name="chars" /> does not have enough capacity from <paramref name="charIndex" /> to the end of the array to accommodate the resulting characters.
        /// </exception>
        /// <exception cref="T:System.Text.DecoderFallbackException">
        /// A fallback occurred (see Understanding Encodings for complete explanation)
        /// -and-
        /// <see cref="P:System.Text.Encoding.DecoderFallback" /> is set to <see cref="T:System.Text.DecoderExceptionFallback" />.
        /// </exception>
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            if ((bytes == null) || (chars == null))
            {
                throw new ArgumentNullException((bytes == null) ? "bytes" : "chars", "ArgumentNull_Array");
            }
            if ((byteIndex < 0) || (byteCount < 0))
            {
                throw new ArgumentOutOfRangeException((byteIndex < 0) ? "byteIndex" : "byteCount", "ArgumentOutOfRange_NeedNonNegNum");
            }
            if ((bytes.Length - byteIndex) < byteCount)
            {
                throw new ArgumentOutOfRangeException("bytes", "ArgumentOutOfRange_IndexCountBuffer");
            }
            if ((charIndex < 0) || (charIndex > chars.Length))
            {
                throw new ArgumentOutOfRangeException("charIndex", "ArgumentOutOfRange_Index");
            }
            if (bytes.Length == 0)
            {
                return 0;
            }
            return this.GetCharsInternal(bytes, byteIndex, byteCount, chars, charIndex);
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
        protected abstract int GetCharsInternal(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex);
        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="codePage">The code page.</param>
        /// <returns></returns>
        new public static Encoding GetEncoding(int codePage)
        {
            switch (codePage)
            {
                case 0x4e4:
                    return PdfLatin1Encoding.Instance;

                case 0x4e9f:
                    return PdfASCIIEncoding.Instance;
            }
            return null;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="encoding">The encoding.</param>
        /// <returns></returns>
        new public static Encoding GetEncoding(string encoding)
        {
            if (encoding == null)
            {
                throw new PdfArgumentNullException("encoding");
            }
            if (CodePages.ContainsKey(encoding))
            {
                return GetEncoding(CodePages[encoding]);
            }
            return Encoding.GetEncoding(encoding);
        }
    }
}

