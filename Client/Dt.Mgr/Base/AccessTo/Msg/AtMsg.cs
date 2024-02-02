namespace Dt.Mgr
{
    /// <summary>
    /// msg服务的数据访问
    /// </summary>
    public partial class AtMsg : AccessAgent<AtMsg.Info>
    {
        public class Info : AgentInfo
        {
            public override string Name => "msg";
        }
    }
}