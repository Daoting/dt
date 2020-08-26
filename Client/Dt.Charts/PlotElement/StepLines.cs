#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public partial class StepLines : Lines
    {
        Rect _labelRect;

        internal override object Clone()
        {
            StepLines clone = new StepLines();
            base.CloneAttributes(clone);
            clone.Smoothed = base.Smoothed;
            return clone;
        }

        protected override bool Render(RenderContext rc)
        {
            Point[] points = rc.Points;
            if ((points == null) || (points.Length < 2))
            {
                return false;
            }

            BaseRenderer renderer = rc.Renderer as BaseRenderer;
            bool inverted = (renderer != null) && renderer.Inverted;
            points = Lines.CreateSteps(points, inverted);
            Rect rect = rc.Bounds2D;
            bool isCustomClipping = rc.IsCustomClipping;

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            if (rc.hasNan)
            {
                List<Point[]> list = base.SplitPointsWithHoles(points);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        PathFigure[] figureArray = base.RenderSegment(list[i], double.NaN, isCustomClipping ? rect : Extensions.EmptyRect);
                        int length = figureArray.Length;
                        for (int j = 0; j < length; j++)
                        {
                            geometry.Figures.Add(figureArray[j]);
                        }
                    }
                }
            }
            else
            {
                PathFigure[] figureArray2 = base.RenderSegment(points, double.NaN, isCustomClipping ? rect : Extensions.EmptyRect);
                int num4 = figureArray2.Length;
                for (int k = 0; k < num4; k++)
                {
                    geometry.Figures.Add(figureArray2[k]);
                }
            }
            Data = geometry;
            _labelRect = rect;

            if (!isCustomClipping)
            {
                RectangleGeometry geometry2 = new RectangleGeometry();
                geometry2.Rect = rect;
                base.Clip = geometry2;
            }
            return true;
        }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }
    }
}

