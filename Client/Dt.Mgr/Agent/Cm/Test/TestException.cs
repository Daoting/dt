namespace Dt.Agent
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    public partial class AtTestCm
    {
        public static Task<string> ThrowException()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestException.ThrowException"
            );
        }

        public static Task<string> ThrowBusinessException()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestException.ThrowBusinessException"
            );
        }

        public static Task<string> ThrowPostionException()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestException.ThrowPostionException"
            );
        }

        public static Task<Dict> ThrowSerializeException()
        {
            return Kit.Rpc<Dict>(
                "cm",
                "TestException.ThrowSerializeException"
            );
        }
    }
}
