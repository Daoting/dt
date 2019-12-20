#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Indicates a preset type of pattern fill.
    /// </summary>
    /// <remarks>
    /// It correspond to members of the HatchStyle enumeration in the .NET Framework.
    /// </remarks>
    public enum ChartFormatPatternFillStyle
    {
        /// <summary>
        /// A pattern of lines on a diagonal from upper right to lower left.
        /// </summary>
        BackwardDiagonal = 3,
        /// <summary>
        /// Specifies horizontal and vertical lines that cross.
        /// </summary>
        Cross = 4,
        /// <summary>
        /// Specifies diagonal lines that slant to the right from top points to bottom
        /// points, are spaced 50 percent closer together than, and are twice the width
        /// of Dt.Xls.PatternFillPattern.ForwardDiagonal. This hatch pattern
        /// is not antialiased.
        /// </summary>
        DarkDownwardDiagonal = 20,
        /// <summary>
        /// Specifies horizontal lines that are spaced 50 percent closer together than
        /// Dt.Xls.PatternFillPattern.Horizontal and are twice the width of
        /// Dt.Xls.PatternFillPattern.Horizontal.
        /// </summary>
        DarkHorizontal = 0x1d,
        /// <summary>
        /// Specifies diagonal lines that slant to the left from top points to bottom
        /// points, are spaced 50 percent closer together than Dt.Xls.PatternFillPattern.BackwardDiagonal,
        /// and are twice its width, but the lines are not antialiased.
        /// </summary>
        DarkUpwardDiagonal = 0x15,
        /// <summary>
        /// Specifies vertical lines that are spaced 50 percent closer together than
        /// Dt.Xls.PatternFillPattern.Vertical and are twice its width.
        /// </summary>
        DarkVertical = 0x1c,
        /// <summary>
        /// Specifies dashed diagonal lines, that slant to the right from top points
        /// to bottom points.
        /// </summary>
        DashedDownwardDiagonal = 30,
        /// <summary>
        /// Specifies dashed horizontal lines.
        /// </summary>
        DashedHorizontal = 0x20,
        /// <summary>
        /// Specifies dashed diagonal lines, that slant to the left from top points to
        /// bottom points.
        /// </summary>
        DashedUpwardDiagonal = 0x1f,
        /// <summary>
        /// Specifies dashed vertical lines.
        /// </summary>
        DashedVertical = 0x21,
        /// <summary>
        /// Specifies a hatch that has the appearance of layered bricks that slant to
        /// the left from top points to bottom points.
        /// </summary>
        DiagonalBrick = 0x26,
        /// <summary>
        /// A pattern of crisscross diagonal lines.
        /// </summary>
        DiagonalCross = 5,
        /// <summary>
        /// Specifies a hatch that has the appearance of divots.
        /// </summary>
        Divot = 0x2a,
        /// <summary>
        /// Specifies forward diagonal and backward diagonal lines, each of which is
        /// composed of dots, that cross.
        /// </summary>
        DottedDiamond = 0x2c,
        /// <summary>
        /// Specifies horizontal and vertical lines, each of which is composed of dots,
        /// that cross.
        /// </summary>
        DottedGrid = 0x2b,
        /// <summary>
        /// A pattern of lines on a diagonal from upper left to lower right.
        /// </summary>
        ForwardDiagonal = 2,
        /// <summary>
        /// A pattern of horizontal lines.
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Specifies a hatch that has the appearance of horizontally layered bricks.
        /// </summary>
        HorizontalBrick = 0x27,
        /// <summary>
        /// Specifies a hatch that has the appearance of a checkerboard with squares
        /// that are twice the size of Dt.Xls.PatternFillPattern.SmallCheckerBoard.
        /// </summary>
        LargeCheckerBoard = 50,
        /// <summary>
        /// Specifies a hatch that has the appearance of confetti, and is composed of
        /// larger pieces than Dt.Xls.PatternFillPattern.SmallConfetti.
        /// </summary>
        LargeConfetti = 0x23,
        /// <summary>
        /// Specifies the hatch style Dt.Xls.PatternFillPattern.Cross.
        /// </summary>
        LargeGrid = 0x36,
        /// <summary>
        /// Specifies diagonal lines that slant to the right from top points to bottom
        /// points and are spaced 50 percent closer together than Dt.Xls.PatternFillPattern.ForwardDiagonal,
        /// but are not antialiased.
        /// </summary>
        LightDownwardDiagonal = 0x12,
        /// <summary>
        /// Specifies horizontal lines that are spaced 50 percent closer together than
        /// Dt.Xls.PatternFillPattern.Horizontal.
        /// </summary>
        LightHorizontal = 0x19,
        /// <summary>
        /// Specifies diagonal lines that slant to the left from top points to bottom
        /// points and are spaced 50 percent closer together than Dt.Xls.PatternFillPattern.BackwardDiagonal,
        /// but they are not antialiased.
        /// </summary>
        LightUpwardDiagonal = 0x13,
        /// <summary>
        /// Specifies vertical lines that are spaced 50 percent closer together than
        /// Dt.Xls.PatternFillPattern.Vertical.
        /// </summary>
        LightVertical = 0x18,
        /// <summary>
        /// Specifies horizontal lines that are spaced 75 percent closer together than
        /// hatch style Dt.Xls.PatternFillPattern.Horizontal (or 25 percent
        /// closer together than Dt.Xls.PatternFillPattern.LightHorizontal).
        /// </summary>
        NarrowHorizontal = 0x1b,
        /// <summary>
        /// Specifies vertical lines that are spaced 75 percent closer together than
        /// hatch style Dt.Xls.PatternFillPattern.Vertical (or 25 percent closer
        /// together than Dt.Xls.PatternFillPattern.LightVertical).
        /// </summary>
        NarrowVertical = 0x1a,
        /// <summary>
        /// Specifies forward diagonal and backward diagonal lines that cross but are
        /// not antialiased.
        /// </summary>
        OutlinedDiamond = 0x33,
        /// <summary>
        /// Specifies a 5-percent hatch. The ratio of foreground color to background
        /// color is 5:95.
        /// </summary>
        Percent05 = 6,
        /// <summary>
        /// Specifies a 10-percent hatch. The ratio of foreground color to background
        /// color is 10:90.
        /// </summary>
        Percent10 = 7,
        /// <summary>
        /// Specifies a 20-percent hatch. The ratio of foreground color to background
        /// color is 20:80.
        /// </summary>
        Percent20 = 8,
        /// <summary>
        /// Specifies a 25-percent hatch. The ratio of foreground color to background
        /// color is 25:75.
        /// </summary>
        Percent25 = 9,
        /// <summary>
        /// Specifies a 30-percent hatch. The ratio of foreground color to background
        /// color is 30:70.
        /// </summary>
        Percent30 = 10,
        /// <summary>
        /// Specifies a 40-percent hatch. The ratio of foreground color to background
        /// color is 40:60.
        /// </summary>
        Percent40 = 11,
        /// <summary>
        /// Specifies a 50-percent hatch. The ratio of foreground color to background
        /// color is 50:50.
        /// </summary>
        Percent50 = 12,
        /// <summary>
        /// Specifies a 60-percent hatch. The ratio of foreground color to background
        /// color is 60:40.
        /// </summary>
        Percent60 = 13,
        /// <summary>
        /// Specifies a 70-percent hatch. The ratio of foreground color to background
        /// color is 70:30.
        /// </summary>
        Percent70 = 14,
        /// <summary>
        /// Specifies a 75-percent hatch. The ratio of foreground color to background
        /// color is 75:25.
        /// </summary>
        Percent75 = 15,
        /// <summary>
        /// Specifies a 80-percent hatch. The ratio of foreground color to background
        /// color is 80:100.
        /// </summary>
        Percent80 = 0x10,
        /// <summary>
        /// Specifies a 90-percent hatch. The ratio of foreground color to background
        /// color is 90:10.
        /// </summary>
        Percent90 = 0x11,
        /// <summary>
        /// Specifies a hatch that has the appearance of a plaid material.
        /// </summary>
        Plaid = 0x29,
        /// <summary>
        /// Specifies a hatch that has the appearance of diagonally layered shingles
        /// that slant to the right from top points to bottom points.
        /// </summary>
        Shingle = 0x2d,
        /// <summary>
        /// Specifies a hatch that has the appearance of a checkerboard.
        /// </summary>
        SmallCheckerBoard = 0x31,
        /// <summary>
        /// Specifies a hatch that has the appearance of confetti.
        /// </summary>
        SmallConfetti = 0x22,
        /// <summary>
        /// Specifies horizontal and vertical lines that cross and are spaced 50 percent
        /// closer together than hatch style Dt.Xls.PatternFillPattern.Cross.
        /// </summary>
        SmallGrid = 0x30,
        /// <summary>
        /// Specifies a hatch that has the appearance of a checkerboard placed diagonally.
        /// </summary>
        SolidDiamond = 0x34,
        /// <summary>
        /// Specifies a hatch that has the appearance of spheres laid adjacent to one
        /// another.
        /// </summary>
        Sphere = 0x2f,
        /// <summary>
        /// Specifies a hatch that has the appearance of a trellis.
        /// </summary>
        Trellis = 0x2e,
        /// <summary>
        /// A pattern of vertical lines.
        /// </summary>
        Vertical = 1,
        /// <summary>
        /// Specifies horizontal lines that are composed of tildes.
        /// </summary>
        Wave = 0x25,
        /// <summary>
        /// Specifies a hatch that has the appearance of a woven material.
        /// </summary>
        Weave = 40,
        /// <summary>
        /// Specifies diagonal lines that slant to the right from top points to bottom
        /// points, have the same spacing as hatch style Dt.Xls.PatternFillPattern.ForwardDiagonal,
        /// and are triple its width, but are not antialiased.
        /// </summary>
        WideDownwardDiagonal = 0x16,
        /// <summary>
        /// Specifies diagonal lines that slant to the left from top points to bottom
        /// points, have the same spacing as hatch style Dt.Xls.PatternFillPattern.BackwardDiagonal,
        /// and are triple its width, but are not antialiased.
        /// </summary>
        WideUpwardDiagonal = 0x17,
        /// <summary>
        /// Specifies horizontal lines that are composed of zigzags.
        /// </summary>
        ZigZag = 0x24
    }
}

