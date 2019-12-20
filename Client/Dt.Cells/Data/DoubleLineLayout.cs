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
    internal class DoubleLineLayout
    {
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
            double num5;
            double num6;
            double num7;
            double num8;
            double num9;
            double num10;
            double num11;
            double num12;
            BorderLineLayoutEngine.CalcDoubleLayout(lineItem, hOffset, vOffset, 0, out num, out num2, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12);
            comboLine._line1.X1 = num5;
            comboLine._line1.X2 = num7;
            comboLine._line3.X1 = num6;
            comboLine._line3.X2 = num8;
            comboLine._line1.Y1 = num3 - 1.0;
            comboLine._line1.Y2 = num4 - 1.0;
            comboLine._line3.Y1 = num3 + 1.0;
            comboLine._line3.Y2 = num4 + 1.0;
        }

        public static void LayoutVertical(ComboLine comboLine, LineItem lineItem, double hOffset, double vOffset)
        {
            double num;
            double num2;
            double num3;
            double num4;
            double num5;
            double num6;
            double num7;
            double num8;
            double num9;
            double num10;
            double num11;
            double num12;
            BorderLineLayoutEngine.CalcDoubleLayout(lineItem, hOffset, vOffset, 1, out num, out num2, out num3, out num4, out num5, out num6, out num7, out num8, out num9, out num10, out num11, out num12);
            comboLine._line1.Y1 = num9;
            comboLine._line1.Y2 = num11;
            comboLine._line3.Y1 = num10;
            comboLine._line3.Y2 = num12;
            comboLine._line1.X1 = num - 1.0;
            comboLine._line1.X2 = num2 - 1.0;
            comboLine._line3.X1 = num + 1.0;
            comboLine._line3.X2 = num2 + 1.0;
        }
    }
}

