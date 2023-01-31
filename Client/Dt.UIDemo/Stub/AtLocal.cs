#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022/8/4 13:04:15 创建
******************************************************************************/
#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// 本地sqlite库，文件名 local.db
    /// </summary>
    public class AtLocal : EntityAccess<AtLocal.LocalInfo>
    {
        public class LocalInfo : AccessInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "local";
        }
    }
}