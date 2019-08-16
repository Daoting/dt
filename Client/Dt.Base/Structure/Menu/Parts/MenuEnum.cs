#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2011-03-04 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    /// <summary>
    /// phone模式一级菜单项显示状态
    /// </summary>
    public enum VisibleInPhone
    {
        /// <summary>
        /// 只显示文字
        /// </summary>
        ID,

        /// <summary>
        /// 只显示图标
        /// </summary>
        Icon,

        /// <summary>
        /// 显示文字和图标
        /// </summary>
        IconAndID,
    }

    /// <summary>
    /// 鼠标模式触发上下文菜单的事件种类
    /// </summary>
    public enum MouseTriggerEvent
    {
        /// <summary>
        /// 鼠标右键
        /// </summary>
        RightTapped,

        /// <summary>
        /// 鼠标左键
        /// </summary>
        LeftTapped,

        /// <summary>
        /// 自定义触发方式
        /// </summary>
        Custom
    }

    /// <summary>
    /// 触摸模式触发上下文菜单的事件种类
    /// </summary>
    public enum TouchTriggerEvent
    {
        /// <summary>
        /// 长按
        /// </summary>
        Holding,

        /// <summary>
        /// 触摸点击
        /// </summary>
        Tapped,

        /// <summary>
        /// 自定义触发方式
        /// </summary>
        Custom
    }

    /// <summary>
    /// 上下文菜单的显示位置
    /// </summary>
    public enum MenuPosition
    {
        /// <summary>
        /// 在指定位置显示
        /// </summary>
        Default,

        /// <summary>
        /// 左上对齐（对话框的左上角与目标元素的左上角重叠）
        /// </summary>
        TopLeft,

        /// <summary>
        /// 右上对齐（对话框的左上角与目标元素的右上角重叠）
        /// </summary>
        TopRight,

        /// <summary>
        /// 中心对齐（对话框的中心与目标元素的中心重叠）
        /// </summary>
        Center,

        /// <summary>
        /// 左下对齐（默认，对话框的左上角与目标元素的左下角重叠）
        /// </summary>
        BottomLeft,

        /// <summary>
        /// 右下对齐（对话框的左上角与目标元素的右下角重叠）
        /// </summary>
        BottomRight,

        /// <summary>
        /// 对话框的右上角与目标元素的左上角重叠
        /// </summary>
        OuterLeftTop,

        /// <summary>
        /// 对话框的左下角与目标元素的左上角重叠
        /// </summary>
        OuterTop
    }

    /// <summary>
    /// 菜单项使用范围
    /// </summary>
    public enum MiScope
    {
        /// <summary>
        /// 始终显示
        /// </summary>
        Both,

        /// <summary>
        /// 只在Phone模式显示
        /// </summary>
        Phone,

        /// <summary>
        /// 只在Win模式显示
        /// </summary>
        Windows
    }

    /// <summary>
    /// 菜单项状态
    /// </summary>
    internal enum MenuItemState
    {
        /// <summary>
        /// 普遍状态
        /// </summary>
        Normal,

        /// <summary>
        /// 移入状态
        /// </summary>
        PointerOver,

        /// <summary>
        /// 点击状态
        /// </summary>
        Pressed
    }
}
