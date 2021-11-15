#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 字符串加解密
    /// </summary>
    public partial class Kit
    {
        #region 成员变量
        const string _pad = "=";
        static Dictionary<int, char> _cvtChar;
        static Dictionary<char, int> _cvtBckChar;
        #endregion

        /// <summary>
        ///  获取给定字符串的MD5码
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        public static string GetMD5(string p_str)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(p_str);
                MD5 md = MD5.Create();
                byte[] data = md.ComputeHash(bytes);

                string[] hexDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    int num = data[i];
                    if (num < 0)
                    {
                        num = 0x100 + num;
                    }
                    int index = num / 0x10;
                    int num3 = num % 0x10;

                    builder.Append(hexDigits[index] + hexDigits[num3]);
                }
                return builder.ToString();
            }
            catch (Exception)
            {
                throw new Exception("不支持MD5算法");
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Encrypt(string source)
        {
            if (_cvtChar == null)
                InitDict();

            if (source == null || source == "")
                return "AAQ=";

            StringBuilder sb = new StringBuilder();
            byte[] tmp = Encoding.UTF8.GetBytes(source);
            int remain = tmp.Length % 3;
            int patch = 3 - remain;
            if (remain != 0)
            {
                Array.Resize(ref tmp, tmp.Length + patch);
            }
            int cnt = (int)Math.Ceiling(tmp.Length * 1.0 / 3);
            for (int i = 0; i < cnt; i++)
            {
                sb.Append(EncodeUnit(tmp[i * 3], tmp[i * 3 + 1], tmp[i * 3 + 2]));
            }
            if (remain != 0)
            {
                sb.Remove(sb.Length - patch, patch);
                for (int i = 0; i < patch; i++)
                {
                    sb.Append(_pad);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取给定字符串的MD5码字节数组
        /// </summary>
        /// <param name="p_str"></param>
        /// <returns></returns>
        public static byte[] GetMD5Bytes(string p_str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(p_str);
            MD5 md = MD5.Create();
            return md.ComputeHash(bytes);
        }

        static string EncodeUnit(params byte[] unit)
        {
            int[] obj = new int[4];
            obj[0] = (unit[0] & 0xfc) >> 2;
            obj[1] = ((unit[0] & 0x03) << 4) + ((unit[1] & 0xf0) >> 4);
            obj[2] = ((unit[1] & 0x0f) << 2) + ((unit[2] & 0xc0) >> 6);
            obj[3] = unit[2] & 0x3f;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < obj.Length; i++)
            {
                sb.Append(_cvtChar[(int)obj[i]]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Decrypt(string source)
        {
            if (_cvtChar == null)
                InitDict();

            if (source == null || source == "" || source == "AAQ=")
                return "";

            List<byte> list = new List<byte>();
            char[] tmp = source.ToCharArray();
            int remain = tmp.Length % 4;
            if (remain != 0)
            {
                Array.Resize(ref tmp, tmp.Length - remain);
            }
            int patch = source.IndexOf(_pad);
            if (patch != -1)
            {
                patch = source.Length - patch;
            }
            int cnt = tmp.Length / 4;
            for (int i = 0; i < cnt; i++)
            {
                DecodeUnit(list, tmp[i * 4], tmp[i * 4 + 1], tmp[i * 4 + 2], tmp[i * 4 + 3]);
            }
            for (int i = 0; i < patch; i++)
            {
                list.RemoveAt(list.Count - 1);
            }
            return Encoding.UTF8.GetString(list.ToArray(), 0, list.Count);
        }

        static void DecodeUnit(List<byte> byteArr, params char[] chArray)
        {
            int[] res = new int[3];
            byte[] unit = new byte[chArray.Length];
            for (int i = 0; i < chArray.Length; i++)
            {
                unit[i] = (byte)_cvtBckChar[chArray[i]];
            }
            res[0] = (unit[0] << 2) + ((unit[1] & 0x30) >> 4);
            res[1] = ((unit[1] & 0xf) << 4) + ((unit[2] & 0x3c) >> 2);
            res[2] = ((unit[2] & 0x3) << 6) + unit[3];
            for (int i = 0; i < res.Length; i++)
            {
                byteArr.Add((byte)res[i]);
            }
        }

        static void InitDict()
        {
            _cvtChar = new Dictionary<int, char>();
            _cvtBckChar = new Dictionary<char, int>();
            _cvtBckChar.Add(_pad[0], -1);

            string codeTable = @"ABCDEFGHIJKLMNOPQRSTUVWXYZbacdefghijklmnopqrstuvwxyz0123456789*-";
            for (int i = 0; i < codeTable.Length; i++)
            {
                _cvtChar.Add(i, codeTable[i]);
                _cvtBckChar.Add(codeTable[i], i);
            }
        }
    }
}