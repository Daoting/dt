#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 尺寸调节器拖动过程中的标准线
    /// </summary>
    public partial class PreviewControl : Control
    {
        Point _splitterOrigin = new Point();

        /// <summary>
        /// 
        /// </summary>
        public PreviewControl()
        {
            DefaultStyleKey = typeof(PreviewControl);
        }

        /// <summary>
        /// 应用GridResizer属性
        /// </summary>
        /// <param name="p_resizer"></param>
        public void Bind(GridResizer p_resizer)
        {
            UIElement parent = p_resizer.Parent as UIElement;
            if (parent != null)
            {
                Matrix matrix = ((MatrixTransform)p_resizer.TransformToVisual(parent)).Matrix;
                _splitterOrigin.X = matrix.OffsetX;
                _splitterOrigin.Y = matrix.OffsetY;

                ItemPlacement direction = p_resizer.Placement ?? ItemPlacement.Left;
                if (p_resizer.Placement == ItemPlacement.Left)
                {
                    Height = parent.RenderSize.Height;
                    Width = GridResizer.ResizerSize;
                    SetValue(Canvas.LeftProperty, _splitterOrigin.X);
                }
                else if (p_resizer.Placement == ItemPlacement.Right)
                {
                    Height = parent.RenderSize.Height;
                    Width = GridResizer.ResizerSize;
                    SetValue(Canvas.LeftProperty, _splitterOrigin.X);
                }
                else if (p_resizer.Placement == ItemPlacement.Top)
                {
                    Width = parent.RenderSize.Width;
                    Height = GridResizer.ResizerSize;
                    SetValue(Canvas.TopProperty, _splitterOrigin.Y);
                }
                else
                {
                    Width = parent.RenderSize.Width;
                    Height = GridResizer.ResizerSize;
                    SetValue(Canvas.TopProperty, _splitterOrigin.Y);
                }
            }
        }

        public double OffsetX
        {
            get { return (((double)GetValue(Canvas.LeftProperty)) - _splitterOrigin.X); }
            set { SetValue(Canvas.LeftProperty, _splitterOrigin.X + value); }
        }

        public double OffsetY
        {
            get { return (((double)GetValue(Canvas.TopProperty)) - _splitterOrigin.Y); }
            set { SetValue(Canvas.TopProperty, _splitterOrigin.Y + value); }
        }
    }
}
