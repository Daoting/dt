#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-01 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 增加前选项
    /// </summary>
    public enum FvBeforeAdd
    {
        /// <summary>
        /// 自动保存已修改的数据
        /// </summary>
        AutoSave,

        /// <summary>
        /// 提示是否放弃已修改的数据
        /// </summary>
        Confirm,

        /// <summary>
        /// 不检查，直接增加
        /// </summary>
        None
    }

    /// <summary>
    /// 刷新列表的时机
    /// </summary>
    public enum RefreshListOption
    {
        /// <summary>
        /// 保存或删除后实时刷新列表
        /// </summary>
        Realtime,

        /// <summary>
        /// 关闭对话框后刷新列表
        /// </summary>
        DlgClosed,

        /// <summary>
        /// 不刷新列表
        /// </summary>
        None
    }

    enum FvRefreshList
    {
        /// <summary>
        /// 成功保存后
        /// </summary>
        Saved,

        /// <summary>
        /// 成功删除后
        /// </summary>
        Deleted,

        /// <summary>
        /// 对话框关闭后
        /// </summary>
        DlgClosed
    }
}