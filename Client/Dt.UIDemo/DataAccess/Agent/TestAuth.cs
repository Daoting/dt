namespace Dt.UIDemo
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    public partial class AtTestCm
    {
        public static Task<string> NoAuth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.NoAuth"
            );
        }

        public static Task<string> Auth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.Auth"
            );
        }

        public static Task<string> CustomAuth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.CustomAuth"
            );
        }
    }
}
