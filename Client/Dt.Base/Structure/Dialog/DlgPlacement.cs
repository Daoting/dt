#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-09 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 对话框的显示位置
    /// </summary>
    public enum DlgPlacement
    {
        /// <summary>
        /// 居中显示，windows模式默认
        /// </summary>
        CenterScreen,

        /// <summary>
        /// 最大化显示，phone模式默认
        /// </summary>
        Maximized,

        /// <summary>
        /// 从上边缘向下弹出
        /// </summary>
        FromTop,

        /// <summary>
        /// 从左上角向下弹出
        /// </summary>
        FromTopLeft,

        /// <summary>
        /// 从右上角向下弹出
        /// </summary>
        FromTopRight,

        /// <summary>
        /// 从左侧向右弹出
        /// </summary>
        FromLeft,

        /// <summary>
        /// 从左上角向右弹出
        /// </summary>
        FromLeftTop,

        /// <summary>
        /// 从左下角向右弹出
        /// </summary>
        FromLeftBottom,

        /// <summary>
        /// 从右侧向左弹出
        /// </summary>
        FromRight,

        /// <summary>
        /// 从右上角向左弹出
        /// </summary>
        FromRightTop,

        /// <summary>
        /// 从右下角向左弹出
        /// </summary>
        FromRightBottom,

        /// <summary>
        /// 从下边缘向上弹出
        /// </summary>
        FromBottom,

        /// <summary>
        /// 从左下角向上弹出
        /// </summary>
        FromBottomLeft,

        /// <summary>
        /// 从右下角向上弹出
        /// </summary>
        FromBottomRight,

        /// <summary>
        /// 左上对齐（对话框的左上角与目标元素的左上角重叠）
        /// </summary>
        TargetTopLeft,

        /// <summary>
        /// 右上对齐（对话框的左上角与目标元素的右上角重叠）
        /// </summary>
        TargetTopRight,

        /// <summary>
        /// 中心对齐（对话框的中心与目标元素的中心重叠）
        /// </summary>
        TargetCenter,

        /// <summary>
        /// 左下对齐（默认，对话框的左上角与目标元素的左下角重叠）
        /// </summary>
        TargetBottomLeft,

        /// <summary>
        /// 右下对齐（对话框的左上角与目标元素的右下角重叠）
        /// </summary>
        TargetBottomRight,

        /// <summary>
        /// 对话框的右上角与目标元素的左上角重叠
        /// </summary>
        TargetOuterLeftTop,

        /// <summary>
        /// 对话框的左下角与目标元素的左上角重叠
        /// </summary>
        TargetOuterTop,

        /// <summary>
        /// 对话框与目标元素完全重叠，大小及位置都相同
        /// </summary>
        TargetOverlap,
    }

    /// <summary>
    /// 对话框关闭前事件参数
    /// </summary>
    public class DlgClosingEventArgs : AsyncCancelEventArgs
    {
        /// <summary>
        /// 对话框关闭时的返回值
        /// </summary>
        public bool Result { get; internal set; }
    }
}
