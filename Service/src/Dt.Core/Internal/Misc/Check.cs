#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Dt.Core
{
    [DebuggerStepThrough]
    public static class Check
    {
        /// <summary>
        /// 为null时抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static T NotNull<T>(T p_value)
        {
            // 省去参数名称，异常输出中已包含文件、行号、方法名！
            if (p_value == null)
                throw new ArgumentNullException();
            return p_value;
        }

        /// <summary>
        /// 字符串为null或空时抛出异常
        /// </summary>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static string NotNullOrEmpty(string p_value)
        {
            if (string.IsNullOrEmpty(p_value))
                throw new ArgumentException("字符串参数不可为null或空！");
            return p_value;
        }

        /// <summary>
        /// 字符串为null或只空格时抛出异常
        /// </summary>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static string NotNullOrWhiteSpace(string p_value)
        {
            if (string.IsNullOrWhiteSpace(p_value))
                throw new ArgumentException("字符串参数不可为null、空或只空格！");
            return p_value;
        }

        /// <summary>
        /// 集合为null或空时抛出异常
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static ICollection<T> NotNullOrEmpty<T>(ICollection<T> p_value)
        {
            if (p_value == null || p_value.Count <= 0)
                throw new ArgumentException("集合参数不可为null或空！");
            return p_value;
        }
    }
}
