namespace Dt.Agent
{
    /// <summary>
    /// 公共测试的Api
    /// </summary>
    public partial class AtTestMsg : SvcTestAgent<AtMsg.MSG>
    {
        public static Task<int> CloseAllOnline()
        {
            return Kit.Rpc<int>(
                "msg",
                "TestMsg.CloseAllOnline"
            );
        }
    }
}
