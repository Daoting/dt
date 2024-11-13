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
        /// <param name="p_cell">通过单元格触发警告信息事件</param>
        public static void If(bool p_assert, string p_msg = null, ICell p_cell = null)
        {
            if (p_assert)
                ThrowMsg(p_msg, p_cell);
        }

        /// <summary>
        /// 参数为null时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_value">待判断对象</param>
        /// <param name="p_msg">异常消息</param>
        /// <param name="p_cell">通过单元格触发警告信息事件</param>
        public static void IfNull<T>(T p_value, string p_msg = null, ICell p_cell = null)
            where T : class
        {
            if (p_value == null)
                ThrowMsg(p_msg, p_cell);
        }

        /// <summary>
        /// 字符串为 null、空、只空格 时抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_value">待判断串</param>
        /// <param name="p_msg">异常消息</param>
        /// <param name="p_cell">通过单元格触发警告信息事件</param>
        public static void IfEmpty(string p_value, string p_msg = null, ICell p_cell = null)
        {
            if (string.IsNullOrWhiteSpace(p_value))
                ThrowMsg(p_msg, p_cell);
        }

        /// <summary>
        /// 直接抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// </summary>
        /// <param name="p_msg">异常消息</param>
        /// <param name="p_cell">通过单元格触发警告信息事件</param>
        public static void Msg(string p_msg, ICell p_cell = null)
        {
            ThrowMsg(p_msg, p_cell);
        }

        /// <summary>
        /// 直接抛出异常，业务处理异常请指定异常消息，未指定异常消息时只抛出异常位置辅助判断
        /// <para>.net7.0 winui抛异常规律：</para>
        /// <para>1. UI主线程同步方法中抛异常被.net内部拦截处理，不触发未处理异常事件</para>
        /// <para>2. UI主线程异步方法中抛异常，触发未处理异常事件</para>
        /// <para>3. Task内部异常，不管同步或异步都不触发未处理异常事件</para>
        /// <para>因为触发未处理异常事件的不确定性，要想统一提供警告提示信息，只能在抛出KnownException异常前显示</para>
        /// <para></para>
        /// <para>WinAppSdk V1.2 都能触发未处理异常事件，已完美解决崩溃问题</para>
        /// <para></para>
        /// <para>总结：所有平台都不会因为异常而崩溃，对于winui上的非KnownException类型异常，在UI同步方法或后台抛出时无法给出警告提示！</para>
        /// </summary>
        /// <param name="p_msg">异常消息</param>
        /// <param name="p_cell">通过单元格触发警告信息事件</param>
        static void ThrowMsg(string p_msg, ICell p_cell)
        {
            if (string.IsNullOrEmpty(p_msg))
            {
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

                // 未指定异常消息时按错误输出到日志
                Log.Error(p_msg);
            }

#if !SERVER
            // 首先触发Cell警告信息事件 -> FvCell显示警告框
            if (p_cell == null || !p_cell.Warn(p_msg))
            {
                // 外部无订阅时，显示普通警告
                Kit.Warn(p_msg);
            }
#endif
            throw new KnownException(p_msg);
        }
    }
}
