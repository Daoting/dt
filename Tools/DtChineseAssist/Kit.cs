using System.Text;
using System.Threading;

namespace DtChineseAssist
{
    public class Kit
    {
        public static readonly AsyncLocal<bool> RecursionLock = new AsyncLocal<bool>();

        static Encoding _gbk = Encoding.GetEncoding("GBK");

        public static string GetPingYin(string p_str)
        {
            if (string.IsNullOrWhiteSpace(p_str))
                return null;

            bool exists = false;
            foreach (var ch in p_str)
            {
                if (0x4e00 <= ch && ch <= 0x9fa5)
                {
                    exists = true;
                    break;
                }
            }
            if (!exists)
                return null;

            var unis = Encoding.Unicode.GetBytes(p_str);
            var gbks = Encoding.Convert(Encoding.Unicode, _gbk, unis);
            StringBuilder sb = new StringBuilder();
            int i = 0;
            ushort c = 0;

            while (i < gbks.Length)
            {
                var b = gbks[i];
                if (b <= 127)
                {
                    sb.Append((char)b);
                    i++;
                }
                else
                {
                    c = (ushort)(gbks[i] * 256 + gbks[i + 1]);
                    if (c >= 0xB0A1 && c <= 0xB0C4)
                    {
                        sb.Append("A");
                    }
                    else if (c >= 0xB0C5 && c <= 0xB2C0)
                    {
                        sb.Append("B");
                    }
                    else if (c >= 0xB2C1 && c <= 0xB4ED)
                    {
                        sb.Append("C");
                    }
                    else if (c >= 0xB4EE && c <= 0xB6E9)
                    {
                        sb.Append("D");
                    }
                    else if (c >= 0xB6EA && c <= 0xB7A1)
                    {
                        sb.Append("E");
                    }
                    else if (c >= 0xB7A2 && c <= 0xB8C0)
                    {
                        sb.Append("F");
                    }
                    else if (c >= 0xB8C1 && c <= 0xB9FD)
                    {
                        sb.Append("G");
                    }
                    else if (c >= 0xB9FE && c <= 0xBBF6)
                    {
                        sb.Append("H");
                    }
                    else if (c >= 0xBBF7 && c <= 0xBFA5)
                    {
                        sb.Append("J");
                    }
                    else if (c >= 0xBFA6 && c <= 0xC0AB)
                    {
                        sb.Append("K");
                    }
                    else if (c >= 0xC0AC && c <= 0xC2E7)
                    {
                        sb.Append("L");
                    }
                    else if (c >= 0xC2E8 && c <= 0xC4C2)
                    {
                        sb.Append("M");
                    }
                    else if (c >= 0xC4C3 && c <= 0xC5B5)
                    {
                        sb.Append("N");
                    }
                    else if (c >= 0xC5B6 && c <= 0xC5BD)
                    {
                        sb.Append("O");
                    }
                    else if (c >= 0xC5BE && c <= 0xC6D9)
                    {
                        sb.Append("P");
                    }
                    else if (c >= 0xC6DA && c <= 0xC8BA)
                    {
                        sb.Append("Q");
                    }
                    else if (c >= 0xC8BB && c <= 0xC8F5)
                    {
                        sb.Append("R");
                    }
                    else if (c >= 0xC8F6 && c <= 0xCBF9)
                    {
                        sb.Append("S");
                    }
                    else if (c >= 0xCBFA && c <= 0xCDD9)
                    {
                        sb.Append("T");
                    }
                    else if (c >= 0xCDDA && c <= 0xCEF3)
                    {
                        sb.Append("W");
                    }
                    else if (c >= 0xCEF4 && c <= 0xD188)
                    {
                        sb.Append("X");
                    }
                    else if (c >= 0xD1B9 && c <= 0xD4D0)
                    {
                        sb.Append("Y");
                    }
                    else if (c >= 0xD4D1 && c <= 0xD7F9)
                    {
                        sb.Append("Z");
                    }
                    else
                    {
                        sb.Append("?");
                    }
                    i = i + 2;
                }
            }
            return sb.ToString();
        }
    }
}
