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

    /// <summary>
    /// 单元格UI类型
    /// </summary>
    public enum CellUIType
    {
        /// <summary>
        /// 默认
        /// </summary>
        Default,

        /// <summary>
        /// 显示为图标字符
        /// </summary>
        Icon,

        /// <summary>
        /// 显示为CheckBox字符
        /// </summary>
        CheckBox,

        /// <summary>
        /// 显示为图片
        /// </summary>
        Image,

        /// <summary>
        /// 显示为文件列表链接
        /// </summary>
        File,

        /// <summary>
        /// 显示为枚举类型的名称
        /// </summary>
        Enum,

        /// <summary>
        /// 自适应时间转换器，如 昨天，09:13, 2015-04-09
        /// </summary>
        AutoDate,

        /// <summary>
        /// 红底白字的警告样式
        /// </summary>
        Warning,
    }

    /// <summary>
    /// Dot 常用文字样式种类
    /// </summary>
    public enum CellFontStyle
    {
        默认,

        小灰,

        黑白,

        蓝白,

        红白,
    }
}
