#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

namespace $safeprojectname$
{
    /// <summary>
    /// lob服务的数据访问
    /// </summary>
    public partial class AtSvc : DataAccess<AtSvc.Info>
    {
        public class Info : AccessInfo
        {
            public override string Name => "lob";
        }
    }
}