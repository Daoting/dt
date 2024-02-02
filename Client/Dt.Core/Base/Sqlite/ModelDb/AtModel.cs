#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-20 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 本地sqlite模型库，At = Access To，只读
    /// </summary>
    public class AtModel : AccessAgent<AtModel.Info>
    {
        public class Info : AgentInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "model";
        }
    }
}