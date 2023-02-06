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
    /// cm服务的数据访问
    /// </summary>
    public partial class AtCm : DataAccess<AtCm.Info>
    {
        public class Info : AccessInfo
        {
            public override string Name => "cm";
        }
    }
}