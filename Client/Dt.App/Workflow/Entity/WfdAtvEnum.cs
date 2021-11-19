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
    public enum WfdAtvType
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

    /// <summary>
    /// 执行者限制
    /// </summary>
    public enum WfdAtvExecLimit
    {
        无限制,

        前一活动的执行者,

        前一活动的同部门执行者,

        已完成活动的执行者,

        已完成活动的同部门执行者
    }

    /// <summary>
    /// 执行者范围
    /// </summary>
    public enum WfdAtvExecScope
    {
        /// <summary>
        /// 一个角色的所有用户中有一个或多个用户处理活动
        /// </summary>
        一组用户,

        /// <summary>
        /// 一个角色的所有用户都必须处理活动
        /// </summary>
        所有用户,

        /// <summary>
        /// 一个角色的所有用户中需要指定一个用户处理活动
        /// </summary>
        单个用户,

        /// <summary>
        /// 角色的所有用户都可以接收活动，但只要有一个用户签收了活动，其他用户就不能处理活动
        /// </summary>
        任一用户
    }

    /// <summary>
    /// 同步活动的聚合方式
    /// </summary>
    public enum WfdAtvJoinKind
    {
        /// <summary>
        /// 同步活动前的活动实例必须全部完成，才能激活后续活动
        /// </summary>
        全部任务,

        /// <summary>
        /// 只要同步活动前存在一个已完成的活动实例，就激活后续活动
        /// </summary>
        任一任务,

        /// <summary>
        /// 等待在开始活动到同步活动之间所有已激活的活动完成
        /// </summary>
        即时同步
    }

    /// <summary>
    /// 活动的后续迁移方式
    /// </summary>
    public enum WfdAtvTransKind
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
