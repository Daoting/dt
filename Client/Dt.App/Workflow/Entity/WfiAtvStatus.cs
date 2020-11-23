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
    /// 活动实例状态
    /// </summary>
    public enum WfiAtvStatus
    {
        /// <summary>
        /// 活动
        /// </summary>
        Active,

        /// <summary>
        /// 结束
        /// </summary>
        Finish,

        /// <summary>
        /// 终止
        /// </summary>
        Terminate,

        /// <summary>
        /// 同步活动
        /// </summary>
        Sync
    }
}
