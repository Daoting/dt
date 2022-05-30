#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.BaseObject;
using Dt.Pdf.Text;
using System;
using System.Collections.Generic;
using System.IO;
#endregion

namespace Dt.Pdf.Object.Filter
{
    /// <summary>
    /// ASCII Hex filter for Pdf stream
    /// </summary>
    public class ASCIIHexFilter : PdfFilter
    {
        private static readonly byte[][] TABLE = new byte[][] { 
            GetBytes("00"), GetBytes("01"), GetBytes("02"), GetBytes("03"), GetBytes("04"), GetBytes("05"), GetBytes("06"), GetBytes("07"), GetBytes("08"), GetBytes("09"), GetBytes("0A"), GetBytes("0B"), GetBytes("0C"), GetBytes("0D"), GetBytes("0E"), GetBytes("0F"), 
            GetBytes("10"), GetBytes("11"), GetBytes("12"), GetBytes("13"), GetBytes("14"), GetBytes("15"), GetBytes("16"), GetBytes("17"), GetBytes("18"), GetBytes("19"), GetBytes("1A"), GetBytes("1B"), GetBytes("1C"), GetBytes("1D"), GetBytes("1E"), GetBytes("1F"), 
            GetBytes("20"), GetBytes("21"), GetBytes("22"), GetBytes("23"), GetBytes("24"), GetBytes("25"), GetBytes("26"), GetBytes("27"), GetBytes("28"), GetBytes("29"), GetBytes("2A"), GetBytes("2B"), GetBytes("2C"), GetBytes("2D"), GetBytes("2E"), GetBytes("2F"), 
            GetBytes("30"), GetBytes("31"), GetBytes("32"), GetBytes("33"), GetBytes("34"), GetBytes("35"), GetBytes("36"), GetBytes("37"), GetBytes("38"), GetBytes("39"), GetBytes("3A"), GetBytes("3B"), GetBytes("3C"), GetBytes("3D"), GetBytes("3E"), GetBytes("3F"), 
            GetBytes("40"), GetBytes("41"), GetBytes("42"), GetBytes("43"), GetBytes("44"), GetBytes("45"), GetBytes("46"), GetBytes("47"), GetBytes("48"), GetBytes("49"), GetBytes("4A"), GetBytes("4B"), GetBytes("4C"), GetBytes("4D"), GetBytes("4E"), GetBytes("4F"), 
            GetBytes("50"), GetBytes("51"), GetBytes("52"), GetBytes("53"), GetBytes("54"), GetBytes("55"), GetBytes("56"), GetBytes("57"), GetBytes("58"), GetBytes("59"), GetBytes("5A"), GetBytes("5B"), GetBytes("5C"), GetBytes("5D"), GetBytes("5E"), GetBytes("5F"), 
            GetBytes("60"), GetBytes("61"), GetBytes("62"), GetBytes("63"), GetBytes("64"), GetBytes("65"), GetBytes("66"), GetBytes("67"), GetBytes("68"), GetBytes("69"), GetBytes("6A"), GetBytes("6B"), GetBytes("6C"), GetBytes("6D"), GetBytes("6E"), GetBytes("6F"), 
            GetBytes("70"), GetBytes("71"), GetBytes("72"), GetBytes("73"), GetBytes("74"), GetBytes("75"), GetBytes("76"), GetBytes("77"), GetBytes("78"), GetBytes("79"), GetBytes("7A"), GetBytes("7B"), GetBytes("7C"), GetBytes("7D"), GetBytes("7E"), GetBytes("7F"), 
            GetBytes("80"), GetBytes("81"), GetBytes("82"), GetBytes("83"), GetBytes("84"), GetBytes("85"), GetBytes("86"), GetBytes("87"), GetBytes("88"), GetBytes("89"), GetBytes("8A"), GetBytes("8B"), GetBytes("8C"), GetBytes("8D"), GetBytes("8E"), GetBytes("8F"), 
            GetBytes("90"), GetBytes("91"), GetBytes("92"), GetBytes("93"), GetBytes("94"), GetBytes("95"), GetBytes("96"), GetBytes("97"), GetBytes("98"), GetBytes("99"), GetBytes("9A"), GetBytes("9B"), GetBytes("9C"), GetBytes("9D"), GetBytes("9E"), GetBytes("9F"), 
            GetBytes("A0"), GetBytes("A1"), GetBytes("A2"), GetBytes("A3"), GetBytes("A4"), GetBytes("A5"), GetBytes("A6"), GetBytes("A7"), GetBytes("A8"), GetBytes("A9"), GetBytes("AA"), GetBytes("AB"), GetBytes("AC"), GetBytes("AD"), GetBytes("AE"), GetBytes("AF"), 
            GetBytes("B0"), GetBytes("B1"), GetBytes("B2"), GetBytes("B3"), GetBytes("B4"), GetBytes("B5"), GetBytes("B6"), GetBytes("B7"), GetBytes("B8"), GetBytes("B9"), GetBytes("BA"), GetBytes("BB"), GetBytes("BC"), GetBytes("BD"), GetBytes("BE"), GetBytes("BF"), 
            GetBytes("C0"), GetBytes("C1"), GetBytes("C2"), GetBytes("C3"), GetBytes("C4"), GetBytes("C5"), GetBytes("C6"), GetBytes("C7"), GetBytes("C8"), GetBytes("C9"), GetBytes("CA"), GetBytes("CB"), GetBytes("CC"), GetBytes("CD"), GetBytes("CE"), GetBytes("CF"), 
            GetBytes("D0"), GetBytes("D1"), GetBytes("D2"), GetBytes("D3"), GetBytes("D4"), GetBytes("D5"), GetBytes("D6"), GetBytes("D7"), GetBytes("D8"), GetBytes("D9"), GetBytes("DA"), GetBytes("DB"), GetBytes("DC"), GetBytes("DD"), GetBytes("DE"), GetBytes("DF"), 
            GetBytes("E0"), GetBytes("E1"), GetBytes("E2"), GetBytes("E3"), GetBytes("E4"), GetBytes("E5"), GetBytes("E6"), GetBytes("E7"), GetBytes("E8"), GetBytes("E9"), GetBytes("EA"), GetBytes("EB"), GetBytes("EC"), GetBytes("ED"), GetBytes("EE"), GetBytes("EF"), 
            GetBytes("F0"), GetBytes("F1"), GetBytes("F2"), GetBytes("F3"), GetBytes("F4"), GetBytes("F5"), GetBytes("F6"), GetBytes("F7"), GetBytes("F8"), GetBytes("F9"), GetBytes("FA"), GetBytes("FB"), GetBytes("FC"), GetBytes("FD"), GetBytes("FE"), GetBytes("FF")
         };

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
            int num;
            while ((num = rawData.ReadByte()) != -1)
            {
                result.Write(TABLE[num], 0, 2);
            }
            result.WriteByte(0x3e);
        }

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        private static byte[] GetBytes(string str)
        {
            return PdfASCIIEncoding.Instance.GetBytes(str);
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <returns></returns>
        public override PdfName GetName()
        {
            return PdfName.ASCIIHexDecode;
        }
    }
}

