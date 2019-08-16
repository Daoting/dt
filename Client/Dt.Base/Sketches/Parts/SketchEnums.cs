#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    /// <summary>
    /// 背景图形
    /// </summary>
    public enum ShapesType
    {
        任务,
        开始,
        判断,
        预定义,
        存储数据,
        文档,
        数据,
        内部存储,
        输出纸带,
        序列化数据,
        直接数据,
        手工输入,
        卡片,
        延时,
        多文档,
        多任务,
        信息提示,
        人员,
        存储类型,
        显示,
        循环,
        数据传送,
        外部引用,
        结束,
        圆角矩形,
        三维盒子,
        加号,
        标题样式,
        五边形,
        七边形,
        八边形,
        三角形,
        五角星,
        六角星,
        七角星,
        自定义
    }

    /// <summary>
    /// 图元连接点位置
    /// </summary>
    public enum LinkPortPosition
    {
        /// <summary>
        /// 左上
        /// </summary>
        LeftTop = 7,

        /// <summary>
        /// 左中
        /// </summary>
        LeftCenter = 6,

        /// <summary>
        /// 左下
        /// </summary>
        LeftBottom = 5,

        /// <summary>
        /// 右上
        /// </summary>
        RightTop = 1,

        /// <summary>
        /// 右中
        /// </summary>
        RightCenter = 2,

        /// <summary>
        /// 右下
        /// </summary>
        RightBottom = 3,

        /// <summary>
        /// 上中
        /// </summary>
        TopCenter = 0,

        /// <summary>
        /// 下中
        /// </summary>
        BottomCenter = 4
    }

    /// <summary>
    /// 连接线端点图形
    /// </summary>
    public enum LinkPortShapes
    {
        /// <summary>
        /// 无
        /// </summary>
        None,

        /// <summary>
        /// 箭头
        /// </summary>
        Arrow,

        /// <summary>
        /// 菱形
        /// </summary>
        Diamond,

        /// <summary>
        /// 圆
        /// </summary>
        Circle
    }
}
