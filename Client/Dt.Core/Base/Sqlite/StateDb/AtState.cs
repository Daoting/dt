#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-04-16 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 本地sqlite状态库，At = Access To
    /// </summary>
    public class AtState : DataAccess<AtState.Info>
    {
        public class Info : AccessInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "state";
        }
    }
}
