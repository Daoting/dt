namespace Dt.MgrDemo
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    public partial class AtTestCm
    {
        public static Task<string> NoTrans()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestIntercept.NoTrans"
            );
        }

        /// <summary>
        /// 不拦截内嵌方法
        /// </summary>
        /// <returns></returns>
        public static Task<string> CallInline()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestIntercept.CallInline"
            );
        }

        /// <summary>
        /// 外层不拦截，拦截内嵌方法
        /// </summary>
        /// <returns></returns>
        public static Task<string> NotIntercept()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestIntercept.NotIntercept"
            );
        }

        /// <summary>
        /// 调用过程异常
        /// </summary>
        /// <returns></returns>
        public static Task CallInlineException()
        {
            return Kit.Rpc<object>(
                "cm",
                "TestIntercept.ThrowException"
            );
        }

        public static Task<string> GetSql()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestIntercept.GetSql"
            );
        }
    }
}
