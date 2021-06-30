#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
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
                    throw new Exception(string.Format("无法将【{0}】转换到【{1}】类型！", val, typeof(T)));
                }
                return (T)val;
            }
            return default(T);
        }
    }
}
