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
    public enum BeforeAddOption
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
    /// 触发UpdateList的源事件类别
    /// </summary>
    public enum UpdateListEvent
    {
        /// <summary>
        /// 成功保存后实时触发
        /// </summary>
        Saved,

        /// <summary>
        /// 成功删除后实时触发
        /// </summary>
        Deleted,

        /// <summary>
        /// 保存或删除过数据后，对话框关闭
        /// </summary>
        DlgClosed
    }

    /// <summary>
    /// 触发UpdateRelated的源事件类别
    /// </summary>
    public enum UpdateRelatedEvent
    {
        /// <summary>
        /// 点击增加后
        /// </summary>
        Add,
        
        /// <summary>
        /// 调用 OnGet 加载数据后
        /// </summary>
        Loaded,
        
        /// <summary>
        /// 成功保存后
        /// </summary>
        Saved,

        /// <summary>
        /// 成功删除后
        /// </summary>
        Deleted,

        /// <summary>
        /// 清空后
        /// </summary>
        Clear,
        
        /// <summary>
        /// 对话框不可见
        /// </summary>
        DlgClosed,
    }

    /// <summary>
    /// 控制Form行为
    /// </summary>
    public enum FormAction
    {
        /// <summary>
        /// 显示对话框
        /// </summary>
        Open,

        /// <summary>
        /// 关闭对话框
        /// </summary>
        Close,

        /// <summary>
        /// 只清空数据，对话框无操作
        /// </summary>
        Clear,

        /// <summary>
        /// 无任何操作
        /// </summary>
        None,
    }

    /// <summary>
    /// List中事件类别
    /// </summary>
    public enum LvEventType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown,

        /// <summary>
        /// 增加
        /// </summary>
        Add,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit,

        /// <summary>
        /// 切换选择行
        /// </summary>
        SelectionChanged,

        /// <summary>
        /// 双击行
        /// </summary>
        DbClick,

        /// <summary>
        /// List不可见时刷新数据
        /// </summary>
        NotSelectedTab,
    }

    /// <summary>
    /// Tree中事件类别
    /// </summary>
    public enum TvEventType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown,

        /// <summary>
        /// 增加
        /// </summary>
        Add,

        /// <summary>
        /// 编辑
        /// </summary>
        Edit,

        /// <summary>
        /// 切换选择行
        /// </summary>
        SelectionChanged,
    }
}