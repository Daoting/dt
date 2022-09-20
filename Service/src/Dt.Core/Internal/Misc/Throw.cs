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
    //[DebuggerStepThrough]
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
                Msg(p_msg);
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
                Msg(p_msg);
        }

        /// <summary>
        /// 字符串为 null、空、只空格 时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_value">待判断串</param>
        /// <param name="p_msg">异常消息</param>
        public static void IfEmpty(string p_value, string p_msg = null)
        {
            if (string.IsNullOrWhiteSpace(p_value))
                Msg(p_msg);
        }

        /// <summary>
        /// 直接抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// <para>.net6.0 maui抛异常规律：</para>
        /// <para>1. UI主线程同步方法中抛异常被.net内部拦截，不触发未处理异常事件</para>
        /// <para>2. UI主线程异步方法中抛异常，触发未处理异常事件</para>
        /// <para>3. Task内部异常，不管同步或异步都不触发未处理异常事件</para>
        /// <para>因为触发未处理异常事件的不确定性，无法统一处理，警告提示信息只能在抛出异常前显示</para>
        /// <para>.net6.0 maui中非KnownException类型的异常，在UI同步方法或后台抛出时都无法捕获！</para>
        /// </summary>
        /// <param name="p_msg">异常消息</param>
        public static void Msg(string p_msg)
        {
            if (!string.IsNullOrEmpty(p_msg))
            {
#if !SERVER
                Kit.Warn(p_msg);
#endif
                throw new KnownException(p_msg);
            }

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

#if !SERVER
            Kit.Warn(p_msg);
#endif
            throw new KnownException(p_msg);
        }
    }
}
