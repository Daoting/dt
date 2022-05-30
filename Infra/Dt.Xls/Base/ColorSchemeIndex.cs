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

namespace Dt.Xls
{
    /// <summary>
    /// Defines a set of colors for the theme. The set of colors consists of twelve color slots 
    /// that can each hold a color of choice
    /// </summary>
    public enum ColorSchemeIndex
    {
        /// <summary>
        /// Semantic additional background color
        /// </summary>
        _BackgroudnColor2 = 0xf4,
        /// <summary>
        /// Semantic background color
        /// </summary>
        _BackgroundColor1 = 0xf3,
        /// <summary>
        /// A color used in the theme definitions which means "use the color of the style"
        /// </summary>
        /// <remarks>There is no correspond color definition in theme file, but excel use it in schemeClr</remarks>
        _PlaceHolderColor = 240,
        /// <summary>
        /// Semantic text color
        /// </summary>
        _TextColor1 = 0xf1,
        /// <summary>
        /// Semantic additional text color
        /// </summary>
        _TextColor2 = 0xf2,
        /// <summary>
        /// Represents "Accent1" color in the theme
        /// </summary>
        Accent1 = 4,
        /// <summary>
        /// Represents "Accent2" color in the theme
        /// </summary>
        Accent2 = 5,
        /// <summary>
        /// Represents "Accent3" color in the theme
        /// </summary>
        Accent3 = 6,
        /// <summary>
        /// Represents "Accent4" color in the theme
        /// </summary>
        Accent4 = 7,
        /// <summary>
        /// Represents "Accent5" color in the theme
        /// </summary>
        Accent5 = 8,
        /// <summary>
        /// Represents "Accent6" color in the theme
        /// </summary>
        Accent6 = 9,
        /// <summary>
        /// Represents the color of a followed hyperlink
        /// </summary>
        FollowedHyperlink = 11,
        /// <summary>
        /// Represents the color the hyperlinks
        /// </summary>
        Hyperlink = 10,
        /// <summary>
        /// A flag indicate the ColorSchemIndex has not been set yet.
        /// </summary>
        None = 0xff,
        /// <summary>
        /// Represents a dark color, usually defined as a system text color
        /// </summary>
        TextDark1 = 1,
        /// <summary>
        /// Represents a second dark color for use
        /// </summary>
        TextDark2 = 3,
        /// <summary>
        /// Represents a light color, usually defined as the system window color
        /// </summary>
        TextLight1 = 0,
        /// <summary>
        /// Represents a second light color for use
        /// </summary>
        TextLight2 = 2
    }
}

