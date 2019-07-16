﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Security.Cryptography;
using System.Text;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Kit
    {
        static string[] _hexDigits = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string str)
        {
            string str2;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(str);
                MD5 md = new MD5CryptoServiceProvider();
                str2 = ByteArrayToHexString(md.ComputeHash(bytes));
            }
            catch (Exception)
            {
                throw new Exception("不支持MD5算法");
            }
            return str2;
        }

        public static byte[] GetMD5Bytes(string str)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            MD5 md = new MD5CryptoServiceProvider();
            return md.ComputeHash(bytes);
        }

        public static string ByteArrayToHexString(byte[] b)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
            {
                builder.Append(ByteToHexString(b[i]));
            }
            return builder.ToString();
        }

        static string ByteToHexString(byte b)
        {
            int num = b;
            if (num < 0)
            {
                num = 0x100 + num;
            }
            int index = num / 0x10;
            int num3 = num % 0x10;
            return (_hexDigits[index] + _hexDigits[num3]);
        }
    }
}
