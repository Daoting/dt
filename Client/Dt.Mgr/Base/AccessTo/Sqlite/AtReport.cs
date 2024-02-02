#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 本地库
    /// </summary>
    public class AtReport : AccessAgent<AtMenu.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "report";
        }
    }
}