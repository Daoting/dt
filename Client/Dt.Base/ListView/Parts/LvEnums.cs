#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 定义选择模式的常量
    /// </summary>
    public enum SelectionMode
    {
        /// <summary>
        /// 不能选择项
        /// </summary>
        None = 0,

        /// <summary>
        /// 只能选择单个项
        /// </summary>
        Single = 1,

        /// <summary>
        /// 可以选择多个项
        /// </summary>
        Multiple = 2,
    }

    /// <summary>
    /// 视图类型
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// 列表视图
        /// </summary>
        List,

        /// <summary>
        /// 表格视图
        /// </summary>
        Table,

        /// <summary>
        /// 磁贴视图
        /// </summary>
        Tile,
    }
}
