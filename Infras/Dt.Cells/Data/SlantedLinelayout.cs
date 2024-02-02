#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class SlantedLinelayout
    {
        const int HORIZONTAL = 0;
        const int VERTICAL = 1;

        public static void Layout(ComboLine comboLine, LineItem lineItem, double hOffset, double vOffset)
        {
            if (lineItem.Direction == 0)
            {
                LayoutHorizontal(comboLine, lineItem, hOffset, vOffset);
            }
            else
            {
                LayoutVertical(comboLine, lineItem, hOffset, vOffset);
            }
        }

        public static void LayoutHorizontal(ComboLine comboLine, LineItem lineItem, double hOffset, double vOffset)
        {
            double num;
            double num2;
            double num3;
            double num4;
            BorderLineLayoutEngine.CalcNormalLayout(lineItem, hOffset, vOffset, 0, out num, out num2, out num3, out num4);
            comboLine._line1.X1 = num;
            comboLine._line1.X2 = num2;
            comboLine._line1.Y1 = num3 - 1.0;
            comboLine._line1.Y2 = num4 - 1.0;
            comboLine._line2.X1 = num;
            comboLine._line2.X2 = num2;
            comboLine._line2.Y1 = num3;
            comboLine._line2.Y2 = num4;
            comboLine._line1.StrokeDashOffset = ((lineItem.Line.StyleData.StrokeDashOffset + comboLine._line1.StrokeThickness) == 0.0) ? 0.0 : (((num + hOffset) / comboLine._line1.StrokeThickness) - 1.0);
            comboLine._line2.StrokeDashOffset = ((lineItem.Line.StyleData.StrokeDashOffset + comboLine._line2.StrokeThickness) == 0.0) ? 0.0 : ((num + hOffset) / comboLine._line2.StrokeThickness);
        }

        public static void LayoutVertical(ComboLine comboLine, LineItem lineItem, double hOffset, double vOffset)
        {
            double num;
            double num2;
            double num3;
            double num4;
            BorderLineLayoutEngine.CalcNormalLayout(lineItem, hOffset, vOffset, 1, out num, out num2, out num3, out num4);
            comboLine._line1.X1 = num - 1.0;
            comboLine._line1.X2 = num2 - 1.0;
            comboLine._line1.Y1 = num3;
            comboLine._line1.Y2 = num4;
            comboLine._line2.X1 = num;
            comboLine._line2.X2 = num2;
            comboLine._line2.Y1 = num3;
            comboLine._line2.Y2 = num4;
            comboLine._line1.StrokeDashOffset = ((lineItem.Line.StyleData.StrokeDashOffset + comboLine._line1.StrokeThickness) == 0.0) ? 0.0 : (((num3 + vOffset) / comboLine._line1.StrokeThickness) - 1.0);
            comboLine._line2.StrokeDashOffset = ((lineItem.Line.StyleData.StrokeDashOffset + comboLine._line2.StrokeThickness) == 0.0) ? 0.0 : ((num3 + vOffset) / comboLine._line2.StrokeThickness);
        }
    }
}

