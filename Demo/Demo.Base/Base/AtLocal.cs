#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-01-23 15:19:25 创建
******************************************************************************/
#endregion

namespace Demo.Base
{
    /// <summary>
    /// 本地sqlite库，文件名 local.db
    /// </summary>
    public class AtLocal : AccessAgent<AtLocal.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "local";
        }
    }
}