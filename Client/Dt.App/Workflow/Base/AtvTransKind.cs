#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-24 创建
**************************************************************************/
#endregion

#region 命名空间

#endregion

namespace Dt.App.Workflow
{
    /// <summary>
    /// 活动的后续迁移方式
    /// </summary>
    public enum AtvTransKind
    {
        /// <summary>
        /// 自由选择后续迁移
        /// </summary>
        自由选择,

        /// <summary>
        /// 活动的所有后续迁移都执行
        /// </summary>
        全部,

        /// <summary>
        /// 只允许选择一条后续迁移
        /// </summary>
        独占式选择
    }
}
