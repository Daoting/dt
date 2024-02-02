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
}