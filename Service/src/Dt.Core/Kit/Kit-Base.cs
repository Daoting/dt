#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 工具方法
    /// </summary>
    public partial class Kit
    {
        static readonly SnowflakeId _snowflake = new SnowflakeId();

        /// <summary>
        /// 产生新ID，采用Snowflake算法
        /// </summary>
        /// <returns></returns>
        public static long NewID => _snowflake.NextId();

        /// <summary>
        /// 获取新Guid，小写无连字符'-'
        /// </summary>
        public static string NewGuid => Guid.NewGuid().ToString("N");

        /// <summary>
        /// 转换对象的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_val"></param>
        /// <returns></returns>
        public static T ConvertType<T>(object p_val)
        {
            if (p_val != null)
            {
                if (typeof(T) == p_val.GetType())
                {
                    // 结果对象与给定对象类型相同时
                    return (T)p_val;
                }

                object val = null;
                try
                {
                    val = Convert.ChangeType(p_val, typeof(T));
                }
                catch
                {
                    throw new Exception(string.Format("无法将【{0}】转换到【{1}】类型！", p_val, typeof(T)));
                }
                return (T)val;
            }
            return default(T);
        }

        /// <summary>
        /// 将字节长度转成描述信息
        /// </summary>
        /// <param name="p_size"></param>
        /// <returns></returns>
        public static string GetFileSizeDesc(ulong p_size)
        {
            if (p_size < KB)
                return string.Format("{0}B", p_size);
            if (p_size < MB)
                return string.Format("{0}KB", Math.Round(p_size / (float)KB, 2));
            if (p_size < GB)
                return string.Format("{0}MB", Math.Round(p_size / (float)MB, 2));
            return string.Format("{0}GB", Math.Round(p_size / (float)GB, 2));
        }

        /// <summary>
        /// 获取字符串按utf8编码的字节长度
        /// </summary>
        /// <param name="p_content">要计算的字符串 </param>
        /// <returns></returns>
        public static int GetUtf8Length(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;
            return Encoding.UTF8.GetBytes(p_content).Length;
        }

        static Encoding _gbk;
        /// <summary>
        /// 获取字符串按GBK编码的字节长度
        /// </summary>
        /// <param name="p_content"></param>
        /// <returns></returns>
        public static int GetGbkLength(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;

            if (_gbk == null)
            {
                _gbk = Encoding.GetEncoding("GBK");
            }
            return _gbk.GetBytes(p_content).Length;
        }

        /// <summary>
        /// 获取字符串按Unicode编码的字节长度
        /// </summary>
        /// <param name="p_content"></param>
        /// <returns></returns>
        public static int GetUnicodeLength(string p_content)
        {
            if (string.IsNullOrEmpty(p_content))
                return 0;
            return Encoding.Unicode.GetBytes(p_content).Length;
        }

        #region 常量
        /// <summary>
        /// 1GB
        /// </summary>
        public const int GB = 1024 * 1024 * 1024;

        /// <summary>
        /// 1MB
        /// </summary>
        public const int MB = 1024 * 1024;

        /// <summary>
        /// 1KB
        /// </summary>
        public const int KB = 1024;
        #endregion
    }
}
