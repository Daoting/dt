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
    /// 活动类别
    /// </summary>
    public enum AtvType
    {
        /// <summary>
        /// 普通活动
        /// </summary>
        Normal,

        /// <summary>
        /// 开始活动
        /// </summary>
        Start,

        /// <summary>
        /// 同步活动
        /// </summary>
        Sync,

        /// <summary>
        /// 结束活动
        /// </summary>
        Finish
    }
}
