#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a BorderLineLayoutEngine object.
    /// </summary>
    public class BorderLineLayoutEngine
    {
        /// <summary>
        /// Represents that the line is horizontal.
        /// </summary>
        public const int HORIZONTAL = 0;
        internal const int NEXT = 1;
        internal const int PREVIOUS = -1;
        /// <summary>
        /// Represents that the line is vertical.
        /// </summary>
        public const int VERTICAL = 1;

        /// <summary>
        /// Calculates the layout of a double LineItem object.
        /// </summary>
        /// <param name="lineItem">The specified LineItem object.</param>
        /// <param name="hOffset">The horizontal offset of the LineItem object.</param>
        /// <param name="vOffset">The vertical offset of the LineItem object.</param>
        /// <param name="direction">The direction of the LineItem object.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="y2">The y2.</param>
        /// <param name="x1_1">The X1_1.</param>
        /// <param name="x1_3">The X1_3.</param>
        /// <param name="x2_1">The X2_1.</param>
        /// <param name="x2_3">The X2_3.</param>
        /// <param name="y1_1">The y1_1.</param>
        /// <param name="y1_3">The y1_3.</param>
        /// <param name="y2_1">The y2_1.</param>
        /// <param name="y2_3">The y2_3.</param>
        public static void CalcDoubleLayout(LineItem lineItem, double hOffset, double vOffset, int direction, out double x1, out double x2, out double y1, out double y2, out double x1_1, out double x1_3, out double x2_1, out double x2_3, out double y1_1, out double y1_3, out double y2_1, out double y2_3)
        {
            if (direction == 0)
            {
                CalcLayoutHorizontal(lineItem, hOffset, vOffset, out x1, out x2, out y1, out y2);
            }
            else
            {
                if (direction != 1)
                {
                    throw new NotSupportedException(ResourceStrings.BorderLineLayoutNotSupportDirectionError);
                }
                CalcLayoutVertical(lineItem, hOffset, vOffset, out x1, out x2, out y1, out y2);
            }
            x1_1 = x1;
            x1_3 = x1;
            x2_1 = x2;
            x2_3 = x2;
            y1_1 = y1;
            y1_3 = y1;
            y2_1 = y2;
            y2_3 = y2;
            bool flag = IsDoubleLine(lineItem.PreviousBreaker1);
            bool flag2 = IsDoubleLine(lineItem.PreviousBreaker2);
            bool flag3 = IsDoubleLine(lineItem.PreviousLine);
            if ((flag && flag2) && flag3)
            {
                if (direction == 0)
                {
                    Layout4CrossRoad(lineItem, -1, ref x1_1, ref x1_3);
                }
                else if (direction == 1)
                {
                    Layout4CrossRoad(lineItem, -1, ref y1_1, ref y1_3);
                }
            }
            else if (flag && !flag2)
            {
                if (direction == 0)
                {
                    Layout4TurnRoad(lineItem, -1, 1, ref x1_3, ref x1_1);
                }
                else if (direction == 1)
                {
                    Layout4TurnRoad(lineItem, -1, 1, ref y1_3, ref y1_1);
                }
            }
            else if (!flag && flag2)
            {
                if (direction == 0)
                {
                    Layout4TurnRoad(lineItem, -1, 2, ref x1_1, ref x1_3);
                }
                else if (direction == 1)
                {
                    Layout4TurnRoad(lineItem, -1, 2, ref y1_1, ref y1_3);
                }
            }
            else if ((flag && flag2) && !flag3)
            {
                if (direction == 0)
                {
                    Layout4TRoad(lineItem, -1, ref x1_1, ref x1_3);
                }
                else if (direction == 1)
                {
                    Layout4TRoad(lineItem, -1, ref y1_1, ref y1_3);
                }
            }
            else if (direction == 0)
            {
                Layout4Connected(lineItem, -1, ref x1_1, ref x1_3);
            }
            else if (direction == 1)
            {
                Layout4Connected(lineItem, -1, ref y1_1, ref y1_3);
            }
            flag = IsDoubleLine(lineItem.NextBreaker1);
            flag2 = IsDoubleLine(lineItem.NextBreaker2);
            bool flag4 = IsDoubleLine(lineItem.NextLine);
            if ((flag && flag2) && flag4)
            {
                if (direction == 0)
                {
                    Layout4CrossRoad(lineItem, 1, ref x2_1, ref x2_3);
                }
                else if (direction == 1)
                {
                    Layout4CrossRoad(lineItem, 1, ref y2_1, ref y2_3);
                }
            }
            else if (flag && !flag2)
            {
                if (direction == 0)
                {
                    Layout4TurnRoad(lineItem, 1, 1, ref x2_3, ref x2_1);
                }
                else if (direction == 1)
                {
                    Layout4TurnRoad(lineItem, 1, 1, ref y2_3, ref y2_1);
                }
            }
            else if (!flag && flag2)
            {
                if (direction == 0)
                {
                    Layout4TurnRoad(lineItem, 1, 2, ref x2_1, ref x2_3);
                }
                else if (direction == 1)
                {
                    Layout4TurnRoad(lineItem, 1, 2, ref y2_1, ref y2_3);
                }
            }
            else if ((flag && flag2) && !flag4)
            {
                if (direction == 0)
                {
                    Layout4TRoad(lineItem, 1, ref x2_1, ref x2_3);
                }
                else if (direction == 1)
                {
                    Layout4TRoad(lineItem, 1, ref y2_1, ref y2_3);
                }
            }
            else if (direction == 0)
            {
                Layout4Connected(lineItem, 1, ref x2_1, ref x2_3);
            }
            else if (direction == 1)
            {
                Layout4Connected(lineItem, 1, ref y2_1, ref y2_3);
            }
        }

        static void CalcLayoutHorizontal(LineItem lineItem, double hOffset, double vOffset, out double x1, out double x2, out double y1, out double y2)
        {
            if (lineItem.Line.StyleData.DrawingThickness == 2)
            {
                y1 = lineItem.Bounds[0].Bottom - 1.0;
                y2 = lineItem.Bounds[0].Bottom - 1.0;
            }
            else
            {
                y1 = lineItem.Bounds[0].Bottom - 0.5;
                y2 = lineItem.Bounds[0].Bottom - 0.5;
            }
            x1 = lineItem.Bounds[0].Left;
            x2 = lineItem.Bounds[lineItem.Bounds.Count - 1].Right;
            BorderLine objA = BorderLine.Max(lineItem.PreviousBreaker1, lineItem.PreviousBreaker2);
            if (!IsAllowExtend(lineItem, -1))
            {
                x1++;
            }
            else if (IsDoubleLine(lineItem.Line) && ((IsDoubleLine(lineItem.PreviousBreaker1) || IsDoubleLine(lineItem.PreviousBreaker2)) || IsDoubleLine(lineItem.PreviousLine)))
            {
                x1 -= 2.0;
            }
            else if (objA > lineItem.Line)
            {
                if (objA != null)
                {
                    if (objA.StyleData.Thickness == 3)
                    {
                        x1++;
                    }
                    else if (objA.StyleData.Thickness == 2)
                    {
                        x1 += 0.0;
                    }
                    else if (objA.StyleData.Thickness == 1)
                    {
                        x1 += 0.0;
                    }
                }
            }
            else if ((objA != null) && (lineItem.Line > objA))
            {
                if (lineItem.Line > lineItem.PreviousLine)
                {
                    if (objA != null)
                    {
                        if (objA.StyleData.Thickness == 3)
                        {
                            x1 -= 2.0;
                        }
                        else if (objA.StyleData.Thickness == 2)
                        {
                            x1 -= 2.0;
                        }
                        else if (objA.StyleData.Thickness == 1)
                        {
                            x1--;
                        }
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.PreviousLine))
                {
                    if (lineItem.PreviousLine > objA)
                    {
                        if (objA != null)
                        {
                            if (objA.StyleData.Thickness == 3)
                            {
                                x1++;
                            }
                            else if (objA.StyleData.Thickness == 2)
                            {
                                x1 += 0.0;
                            }
                            else if (objA.StyleData.Thickness == 1)
                            {
                                x1 += 0.0;
                            }
                        }
                    }
                    else
                    {
                        x1 -= 0.0;
                    }
                }
            }
            else if (object.Equals(objA, lineItem.Line) && (lineItem.Line > lineItem.PreviousLine))
            {
                if (objA.StyleData.Thickness == 3)
                {
                    x1 -= 2.0;
                }
                else if (objA.StyleData.Thickness == 2)
                {
                    x1 -= 2.0;
                }
                else if (objA.StyleData.Thickness == 1)
                {
                    x1--;
                }
            }
            BorderLine line2 = BorderLine.Max(lineItem.NextBreaker1, lineItem.NextBreaker2);
            if (!IsAllowExtend(lineItem, 1))
            {
                x2 -= 2.0;
            }
            else if (IsDoubleLine(lineItem.Line) && ((IsDoubleLine(lineItem.NextBreaker1) || IsDoubleLine(lineItem.NextBreaker2)) || IsDoubleLine(lineItem.NextLine)))
            {
                x2++;
            }
            else if (line2 > lineItem.Line)
            {
                if (line2 != null)
                {
                    if (line2.StyleData.Thickness == 3)
                    {
                        x2 -= 2.0;
                    }
                    else if (line2.StyleData.Thickness == 2)
                    {
                        x2--;
                    }
                    else if (line2.StyleData.Thickness == 1)
                    {
                        x2--;
                    }
                }
            }
            else if (lineItem.Line > line2)
            {
                if (lineItem.Line > lineItem.NextLine)
                {
                    if (line2 != null)
                    {
                        if (line2.StyleData.Thickness == 3)
                        {
                            x2++;
                        }
                        if (line2.StyleData.Thickness == 2)
                        {
                            x2 += 0.0;
                        }
                        if (line2.StyleData.Thickness == 1)
                        {
                            x2 += 0.0;
                        }
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.NextLine))
                {
                    x2--;
                }
            }
            else if (object.Equals(line2, lineItem.Line))
            {
                if (lineItem.Line > lineItem.NextLine)
                {
                    if (line2.StyleData.Thickness == 3)
                    {
                        x2++;
                    }
                    if (line2.StyleData.Thickness == 2)
                    {
                        x2 += 0.0;
                    }
                    if (line2.StyleData.Thickness == 1)
                    {
                        x2 += 0.0;
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.NextLine))
                {
                    x2--;
                }
            }
            x1 -= hOffset;
            x2 -= hOffset;
            y1 -= vOffset;
            y2 -= vOffset;
        }

        static void CalcLayoutVertical(LineItem lineItem, double hOffset, double vOffset, out double x1, out double x2, out double y1, out double y2)
        {
            y1 = lineItem.Bounds[0].Top;
            y2 = lineItem.Bounds[lineItem.Bounds.Count - 1].Bottom;
            if (lineItem.Line.StyleData.DrawingThickness == 2)
            {
                x1 = lineItem.Bounds[0].Right - 1.0;
                x2 = lineItem.Bounds[0].Right - 1.0;
            }
            else
            {
                x1 = lineItem.Bounds[0].Right - 0.5;
                x2 = lineItem.Bounds[0].Right - 0.5;
            }
            BorderLine objA = BorderLine.Max(lineItem.PreviousBreaker1, lineItem.PreviousBreaker2);
            if (!IsAllowExtend(lineItem, -1))
            {
                y1++;
            }
            else if (IsDoubleLine(lineItem.Line) && ((IsDoubleLine(lineItem.PreviousBreaker1) || IsDoubleLine(lineItem.PreviousBreaker2)) || IsDoubleLine(lineItem.PreviousLine)))
            {
                y1 -= 2.0;
            }
            else if (objA > lineItem.Line)
            {
                if (objA != null)
                {
                    if (objA.StyleData.Thickness == 3)
                    {
                        y1++;
                    }
                    else if (objA.StyleData.Thickness == 2)
                    {
                        y1 += 0.0;
                    }
                    else if (objA.StyleData.Thickness == 1)
                    {
                        y1 += 0.0;
                    }
                }
            }
            else if (lineItem.Line > objA)
            {
                if (lineItem.Line > lineItem.PreviousLine)
                {
                    if (objA != null)
                    {
                        if (objA.StyleData.Thickness == 3)
                        {
                            y1 -= 2.0;
                        }
                        else if (objA.StyleData.Thickness == 2)
                        {
                            y1 -= 2.0;
                        }
                        else if (objA.StyleData.Thickness == 1)
                        {
                            y1--;
                        }
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.PreviousLine))
                {
                    if (lineItem.PreviousLine > objA)
                    {
                        if (objA != null)
                        {
                            if (objA.StyleData.Thickness == 3)
                            {
                                y1++;
                            }
                            else if (objA.StyleData.Thickness == 2)
                            {
                                y1 += 0.0;
                            }
                            else if (objA.StyleData.Thickness == 1)
                            {
                                y1 += 0.0;
                            }
                        }
                    }
                    else
                    {
                        y1 -= 0.0;
                    }
                }
            }
            else if (object.Equals(objA, lineItem.Line) && (lineItem.Line > lineItem.PreviousLine))
            {
                if (objA.StyleData.Thickness == 3)
                {
                    y1 -= 2.0;
                }
                else if (objA.StyleData.Thickness == 2)
                {
                    y1 -= 2.0;
                }
                else if (objA.StyleData.Thickness == 1)
                {
                    y1--;
                }
            }
            BorderLine line2 = BorderLine.Max(lineItem.NextBreaker1, lineItem.NextBreaker2);
            if (!IsAllowExtend(lineItem, 1))
            {
                y2 -= 2.0;
            }
            else if (IsDoubleLine(lineItem.Line) && ((IsDoubleLine(lineItem.NextBreaker1) || IsDoubleLine(lineItem.NextBreaker2)) || IsDoubleLine(lineItem.NextLine)))
            {
                y2++;
            }
            else if (line2 > lineItem.Line)
            {
                if (line2 != null)
                {
                    if (line2.StyleData.Thickness == 3)
                    {
                        y2 -= 2.0;
                    }
                    else if (line2.StyleData.Thickness == 2)
                    {
                        y2 -= 2.0;
                    }
                    else if (line2.StyleData.Thickness == 1)
                    {
                        y2--;
                    }
                }
            }
            else if ((line2 != null) && (lineItem.Line > line2))
            {
                if (lineItem.Line > lineItem.NextLine)
                {
                    if (line2 != null)
                    {
                        if (line2.StyleData.Thickness == 3)
                        {
                            y2++;
                        }
                        if (line2.StyleData.Thickness == 2)
                        {
                            y2 += 0.0;
                        }
                        if (line2.StyleData.Thickness == 1)
                        {
                            y2 += 0.0;
                        }
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.NextLine))
                {
                    y2--;
                }
            }
            else if (object.Equals(line2, lineItem.Line))
            {
                if (lineItem.Line > lineItem.NextLine)
                {
                    if (line2.StyleData.Thickness == 3)
                    {
                        y2++;
                    }
                    if (line2.StyleData.Thickness == 2)
                    {
                        y2 += 0.0;
                    }
                    if (line2.StyleData.Thickness == 1)
                    {
                        y2 += 0.0;
                    }
                }
                else if (!object.Equals(lineItem.Line, lineItem.NextLine))
                {
                    y2--;
                }
            }
            x1 -= hOffset;
            x2 -= hOffset;
            y1 -= vOffset;
            y2 -= vOffset;
        }

        /// <summary>
        /// Calculates the layout of a normal LineItem object.
        /// </summary>
        /// <param name="lineItem">The specified LineItem object.</param>
        /// <param name="hOffset">The horizontal offset of the LineItem object.</param>
        /// <param name="vOffset">The vertical offset of the LineItem object.</param>
        /// <param name="direction">The direction of the LineItem object.</param>
        /// <param name="x1">The x1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="y2">The y2.</param>
        public static void CalcNormalLayout(LineItem lineItem, double hOffset, double vOffset, int direction, out double x1, out double x2, out double y1, out double y2)
        {
            if (direction == 0)
            {
                CalcLayoutHorizontal(lineItem, hOffset, vOffset, out x1, out x2, out y1, out y2);
            }
            else
            {
                if (direction != 1)
                {
                    throw new NotSupportedException(ResourceStrings.BorderLineLayoutNotSupportDirectionError);
                }
                CalcLayoutVertical(lineItem, hOffset, vOffset, out x1, out x2, out y1, out y2);
            }
        }

        static BorderLine GetBreaker(LineItem lineItem, int type, int breaker)
        {
            if (breaker == 1)
            {
                return GetBreaker1(lineItem, type);
            }
            if (breaker == 2)
            {
                return GetBreaker2(lineItem, type);
            }
            return null;
        }

        static BorderLine GetBreaker1(LineItem lineItem, int type)
        {
            if (type == -1)
            {
                return lineItem.PreviousBreaker1;
            }
            if (type == 1)
            {
                return lineItem.NextBreaker1;
            }
            return null;
        }

        static BorderLine GetBreaker2(LineItem lineItem, int type)
        {
            if (type == -1)
            {
                return lineItem.PreviousBreaker2;
            }
            if (type == 1)
            {
                return lineItem.NextBreaker2;
            }
            return null;
        }

        static BorderLine GetNeighbor(LineItem lineItem, int type)
        {
            if (type == -1)
            {
                return lineItem.PreviousLine;
            }
            if (type == 1)
            {
                return lineItem.NextLine;
            }
            return null;
        }

        static bool IsAllowExtend(LineItem lineItem, int type)
        {
            if (IsDoubleLine(lineItem.Line))
            {
                return true;
            }
            int num = 0;
            if (type == -1)
            {
                num += IsDoubleLine(lineItem.PreviousLine) ? 1 : 0;
                num += IsDoubleLine(lineItem.PreviousBreaker1) ? 1 : 0;
                num += IsDoubleLine(lineItem.PreviousBreaker2) ? 1 : 0;
            }
            else if (type == 1)
            {
                num += IsDoubleLine(lineItem.NextLine) ? 1 : 0;
                num += IsDoubleLine(lineItem.NextBreaker1) ? 1 : 0;
                num += IsDoubleLine(lineItem.NextBreaker2) ? 1 : 0;
            }
            return (num < 2);
        }

        /// <summary>
        /// Determines whether the specified line is a double line.
        /// </summary>
        /// <param name="line">The specified line.</param>
        /// <returns><c>true</c> if the specified line is a double line; otherwise, <c>false</c>.</returns>
        public static bool IsDoubleLine(BorderLine line)
        {
            return ((line != null) && (line.Style == BorderLineStyle.Double));
        }

        static void Layout4Connected(LineItem lineItem, int type, ref double o1, ref double o3)
        {
            BorderLine neighbor = GetNeighbor(lineItem, type);
            if (IsDoubleLine(neighbor))
            {
                double thickness = 1.0;
                BorderLine line2 = BorderLine.Max(GetBreaker1(lineItem, type), GetBreaker2(lineItem, type));
                if (line2 != null)
                {
                    thickness = line2.StyleData.Thickness;
                }
                if ((lineItem.Line > neighbor) || object.Equals(neighbor, lineItem.Line))
                {
                    if (type == 1)
                    {
                        o1 -= type;
                        o3 -= type;
                    }
                    else if (thickness == 1.0)
                    {
                        o1 -= type;
                        o3 -= type;
                    }
                }
                else if (type == 1)
                {
                    if (thickness == 1.0)
                    {
                        o1 -= type * 2;
                        o3 -= type * 2;
                    }
                    else
                    {
                        o1 -= type * 3;
                        o3 -= type * 3;
                    }
                }
                else
                {
                    o1 -= type * 2;
                    o3 -= type * 2;
                }
            }
        }

        static void Layout4CrossRoad(LineItem lineItem, int type, ref double o1, ref double o3)
        {
            BorderLine objB = GetBreaker1(lineItem, type);
            BorderLine line2 = GetBreaker2(lineItem, type);
            if ((lineItem.Line > objB) || object.Equals(lineItem.Line, objB))
            {
                o1 -= type * 2;
            }
            else
            {
                o1 -= type * 3;
            }
            if ((lineItem.Line > line2) || object.Equals(lineItem.Line, line2))
            {
                o3 -= type * 2;
            }
            else
            {
                o3 -= type * 3;
            }
        }

        static void Layout4TRoad(LineItem lineItem, int type, ref double o1, ref double o3)
        {
            BorderLine objB = GetBreaker1(lineItem, type);
            BorderLine line2 = GetBreaker2(lineItem, type);
            if ((lineItem.Line > objB) || object.Equals(lineItem.Line, objB))
            {
                o1 -= type * 2;
            }
            else
            {
                o1 -= type * 3;
            }
            if ((lineItem.Line > line2) || object.Equals(lineItem.Line, line2))
            {
                o3 -= type * 2;
            }
            else
            {
                o3 -= type * 3;
            }
        }

        static void Layout4TurnRoad(LineItem lineItem, int type, int breaker, ref double o1, ref double o3)
        {
            BorderLine objB = GetBreaker(lineItem, type, breaker);
            BorderLine neighbor = GetNeighbor(lineItem, type);
            bool flag = IsDoubleLine(GetNeighbor(lineItem, type));
            if ((lineItem.Line > objB) || object.Equals(lineItem.Line, objB))
            {
                o3 -= type * 2;
            }
            else
            {
                o3 -= type * 3;
            }
            if (flag)
            {
                if (!object.Equals(lineItem.Line, neighbor) && (lineItem.Line <= neighbor))
                {
                    o1 -= type * 3;
                }
            }
            else if ((lineItem.Line <= objB) && !object.Equals(lineItem.Line, objB))
            {
                o1 -= type;
            }
        }
    }
}

