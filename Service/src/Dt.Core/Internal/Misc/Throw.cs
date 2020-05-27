#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-27 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Diagnostics;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 抛出异常
    /// </summary>
    [DebuggerStepThrough]
    public static class Throw
    {
        /// <summary>
        /// 条件true时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_assert">true时抛出异常</param>
        /// <param name="p_msg">异常消息</param>
        public static void If(bool p_assert, string p_msg = null)
        {
            if (p_assert)
            {
                if (p_msg == null)
                    p_msg = GetStackTrace();
                throw new KnownException(p_msg);
            }
        }

        /// <summary>
        /// 参数为null时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_value">待判断对象</param>
        /// <param name="p_msg">异常消息</param>
        public static void IfNull<T>(T p_value, string p_msg = null)
            where T : class
        {
            if (p_value == null)
            {
                if (p_msg == null)
                    p_msg = GetStackTrace();
                throw new KnownException(p_msg);
            }
        }

        /// <summary>
        /// 字符串null或空时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_value">待判断串</param>
        /// <param name="p_msg">异常消息</param>
        public static void IfNullOrEmpty(string p_value, string p_msg = null)
        {
            if (string.IsNullOrEmpty(p_value))
            {
                if (p_msg == null)
                    p_msg = GetStackTrace();
                throw new KnownException(p_msg);
            }
        }

        /// <summary>
        /// 直接抛出异常
        /// </summary>
        /// <param name="p_msg">异常消息</param>
        public static void Msg(string p_msg)
        {
            if (p_msg == null)
                p_msg = GetStackTrace();
            throw new KnownException(p_msg);
        }

        /// <summary>
        /// 获取调用堆栈信息
        /// </summary>
        /// <returns></returns>
        static string GetStackTrace()
        {
            var st = new StackTrace();
            if (st.FrameCount > 2)
            {
                var method = st.GetFrame(2).GetMethod();
                return $"出错位置：{method.DeclaringType.Name}.{method.Name}";
            }
            return "出错位置未知";
        }
    }
}
