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
    /// Represents built in styles used in Excel
    /// </summary>
    public enum BuiltInStyleIndex
    {
        /// <summary>
        /// The Accent1 style
        /// </summary>
        Accent1 = 0x1d,
        /// <summary>
        /// The 20% Accent1 style
        /// </summary>
        Accent1_20 = 30,
        /// <summary>
        /// The 40% Accent1 style
        /// </summary>
        Accent1_40 = 0x1f,
        /// <summary>
        /// The 60% Accent1 style
        /// </summary>
        Accent1_60 = 0x20,
        /// <summary>
        /// The Accent2 style
        /// </summary>
        Accent2 = 0x21,
        /// <summary>
        /// The 20% Accent2 style
        /// </summary>
        Accent2_20 = 0x22,
        /// <summary>
        /// The 40% Accent2 style
        /// </summary>
        Accent2_40 = 0x23,
        /// <summary>
        /// The 60% Accent2 style
        /// </summary>
        Accent2_60 = 0x24,
        /// <summary>
        /// The Accent3 style
        /// </summary>
        Accent3 = 0x25,
        /// <summary>
        /// The 20% Accent3 style
        /// </summary>
        Accent3_20 = 0x26,
        /// <summary>
        /// The 40% Accent3 style
        /// </summary>
        Accent3_40 = 0x27,
        /// <summary>
        /// The 60% Accent3 style
        /// </summary>
        Accent3_60 = 40,
        /// <summary>
        /// The Accent4 style
        /// </summary>
        Accent4 = 0x29,
        /// <summary>
        /// The 20% Accent4 style
        /// </summary>
        Accent4_20 = 0x2a,
        /// <summary>
        /// The 40% Accent4 style
        /// </summary>
        Accent4_40 = 0x2b,
        /// <summary>
        /// The 60% Accent4 style
        /// </summary>
        Accent4_60 = 0x2c,
        /// <summary>
        /// The Accent5 style
        /// </summary>
        Accent5 = 0x2d,
        /// <summary>
        /// The 20% Accent5 style
        /// </summary>
        Accent5_20 = 0x2e,
        /// <summary>
        /// The 40% Accent5 style
        /// </summary>
        Accent5_40 = 0x2f,
        /// <summary>
        /// The 60% Accent5 style
        /// </summary>
        Accent5_60 = 0x30,
        /// <summary>
        /// The Accent6 style
        /// </summary>
        Accent6 = 0x31,
        /// <summary>
        /// The 20% Accent6 style
        /// </summary>
        Accent6_20 = 50,
        /// <summary>
        /// The 40% Accent6 style
        /// </summary>
        Accent6_40 = 0x33,
        /// <summary>
        /// The 60% Accent6 style
        /// </summary>
        Accent6_60 = 0x34,
        /// <summary>
        /// The Bad style
        /// </summary>
        Bad = 0x1b,
        /// <summary>
        /// The Calculation style
        /// </summary>
        Calculation = 0x16,
        /// <summary>
        /// The CheckCell style
        /// </summary>
        CheckCell = 0x17,
        /// <summary>
        /// the Column level style
        /// </summary>
        /// /// <remarks>
        /// It used with the outline to represents the style of the specified column outline level, for example, ColLevel_1, ColLevel_2, etc.
        /// </remarks>
        ColumnLevel = 2,
        /// <summary>
        /// The Comma style
        /// </summary>
        Comma = 3,
        /// <summary>
        /// The Comma [0] style
        /// </summary>
        Comma0 = 6,
        /// <summary>
        /// The Currency style
        /// </summary>
        Currency = 4,
        /// <summary>
        /// The Currency [0] style
        /// </summary>
        Currency0 = 7,
        /// <summary>
        /// The ExplanatoryText style
        /// </summary>
        ExplanatoryText = 0x35,
        /// <summary>
        /// The FollowedHyperLink style
        /// </summary>
        FollowedHyperLink = 9,
        /// <summary>
        /// The Good style
        /// </summary>
        Good = 0x1a,
        /// <summary>
        /// The Heading 1 style
        /// </summary>
        Heading1 = 0x10,
        /// <summary>
        /// The Heading 2 style
        /// </summary>
        Heading2 = 0x11,
        /// <summary>
        /// The Heading 3 style
        /// </summary>
        Heading3 = 0x12,
        /// <summary>
        /// The Heading 4 style
        /// </summary>
        Heading4 = 0x13,
        /// <summary>
        /// The HyperLink style
        /// </summary>
        HyperLink = 8,
        /// <summary>
        /// The Input style
        /// </summary>
        Input = 20,
        /// <summary>
        /// The LinkedCell style
        /// </summary>
        LinkedCell = 0x18,
        /// <summary>
        /// The Neutral style
        /// </summary>
        Neutral = 0x1c,
        /// <summary>
        /// The Normal style
        /// </summary>
        Normal = 0,
        /// <summary>
        /// The Note style
        /// </summary>
        Note = 10,
        /// <summary>
        /// The Output style
        /// </summary>
        Output = 0x15,
        /// <summary>
        /// The Percent style
        /// </summary>
        Percent = 5,
        /// <summary>
        /// The Row level style
        /// </summary>
        /// <remarks>
        /// It used with the outline to represents the style of the specified row outline level, for example, RowLevel_1, RowLevel_2, etc.
        /// </remarks>
        RowLevel = 1,
        /// <summary>
        /// The Title style
        /// </summary>
        Title = 15,
        /// <summary>
        /// The Total style
        /// </summary>
        Total = 0x19,
        /// <summary>
        /// The WariningText style
        /// </summary>
        WarningText = 11
    }
}

