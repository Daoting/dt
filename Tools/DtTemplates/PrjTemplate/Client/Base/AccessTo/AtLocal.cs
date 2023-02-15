#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

namespace $ext_safeprojectname$
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