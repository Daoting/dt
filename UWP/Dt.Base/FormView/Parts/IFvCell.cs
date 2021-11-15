#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-09-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格接口
    /// </summary>
    public interface IFvCell
    {
        Visibility Visibility { get; set; }

        Size DesiredSize { get; }

        /// <summary>
        /// 是否水平填充
        /// </summary>
        bool IsHorStretch { get; set; }

        /// <summary>
        /// 占用的行数
        /// </summary>
        int RowSpan { get; set; }

        /// <summary>
        /// 在面板上的布局区域
        /// </summary>
        Rect Bounds { get; set; }
        
        void Measure(Size availableSize);

        void Arrange(Rect finalRect);
    }
}
