namespace Dt.Mgr
{
    /// <summary>
    /// msg服务的数据访问
    /// </summary>
    public partial class AtMsg : EntityAccess<AtMsg.Info>
    {
        public class Info : AccessInfo
        {
            public override string Name => "msg";
        }
    }
}