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

        protected override Size MeasureOverride(Size constraint)
        {
            UpdateGeometry(null, Size);
            return base.MeasureOverride(constraint);
        }

        protected override bool Render(RenderContext rc)
        {
            _symCenter.X = rc.Current.X;
            _symCenter.Y = rc.Current.Y;
            return true;
        }

        protected virtual void UpdateGeometry(PathGeometry pg, Size sz)
        {
        }

        internal override Rect LabelRect
        {
            get
            {
                UpdateGeometry(null, Size);
                return base.LabelRect;
            }
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
                    UpdateGeometry(null, Size);
                }
            }
        }
    }
}

