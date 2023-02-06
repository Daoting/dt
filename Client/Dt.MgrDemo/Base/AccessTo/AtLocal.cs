#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.MgrDemo
{
    /// <summary>
    /// 本地sqlite库，文件名 local.db
    /// </summary>
    public class AtLocal : DataAccess<AtLocal.Info>
    {
        public class Info : AccessInfo
        {
            public override AccessType Type => AccessType.Local;

            public override string Name => "local";
        }
    }
}