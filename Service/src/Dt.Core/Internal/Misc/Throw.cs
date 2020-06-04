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
                ThrowMsg(p_msg);
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
                ThrowMsg(p_msg);
        }

        /// <summary>
        /// 字符串null或空时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_value">待判断串</param>
        /// <param name="p_msg">异常消息</param>
        public static void IfNullOrEmpty(string p_value, string p_msg = null)
        {
            if (string.IsNullOrEmpty(p_value))
                ThrowMsg(p_msg);
        }

        /// <summary>
        /// 直接抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_msg">异常消息</param>
        public static void Msg(string p_msg)
        {
            ThrowMsg(p_msg);
        }

        /// <summary>
        /// uno中在UI主线程调用时，如在Button.Click事件方法中调用，若方法是同步，不catch也没能抛出未处理异常！诡异
        /// </summary>
        /// <param name="p_msg"></param>
        static void ThrowMsg(string p_msg)
        {
            if (!string.IsNullOrEmpty(p_msg))
                throw new KnownException(p_msg);

            // 获取调用堆栈信息
            var st = new StackTrace();
            if (st.FrameCount > 2)
            {
                var method = st.GetFrame(2).GetMethod();
                p_msg = $"异常位置：{method.DeclaringType.Name}.{method.Name} -> Throw.{st.GetFrame(1).GetMethod().Name}";
            }
            else
            {
                p_msg = "异常位置未知";
            }
            throw new KnownException(p_msg);
        }
    }
}
