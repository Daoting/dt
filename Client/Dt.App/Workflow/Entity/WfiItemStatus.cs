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
    /// 工作项状态
    /// </summary>
    public enum WfiItemStatus
    {
        活动,

        结束,

        终止,

        同步
    }

    /// <summary>
    /// 指派方式
    /// </summary>
    public enum WfiItemAssignKind
    {
        普通指派,

        起始指派,

        回退,

        跳转,

        追回,

        回退指派
    }
}
