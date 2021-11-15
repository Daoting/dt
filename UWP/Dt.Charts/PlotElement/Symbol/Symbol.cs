#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class Symbol : PlotElement
    {
        protected Point _symCenter = new Point();

        protected override bool IsCompatible(IRenderer rend)
        {
            if (!base.IsCompatible(rend))
            {
                return (rend is RadarRenderer);
            }
            return true;
        }

        /*********************************************************************************************************/
        // MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // UWP和uno的调用顺序不同！
        // UWP：MeasureOverride > _owner.SizeChanged > SizeChanged > Loaded
        // uno：Loaded > MeasureOverride > SizeChanged > _owner.SizeChanged
        /*********************************************************************************************************/

        protected override bool Render(RenderContext rc)
        {
            _symCenter.X = rc.Current.X;
            _symCenter.Y = rc.Current.Y;

            // 放在MeasureOverride中造成 iOS 上死循环！！！
            UpdateGeometry(Size);
            return true;
        }

        protected virtual void UpdateGeometry(Size sz)
        {
        }

        internal override Rect LabelRect
        {
            get { return new Rect(new Point(), Size); }
        }

        protected override Shape LegendShape
        {
            get
            {
                return (PlotElement)Clone();
            }
        }

        public override Size Size
        {
            get { return  base.Size; }
            set
            {
                if (base.Size != value)
                {
                    base.Size = value;
                    UpdateGeometry(Size);
                }
            }
        }
    }
}

