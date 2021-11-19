#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal static class BuiltInStylesExtension
    {
        public static string Name(this BuiltInStyleIndex builtInStyle)
        {
            switch (builtInStyle)
            {
                case BuiltInStyleIndex.Normal:
                    return "Normal";

                case BuiltInStyleIndex.RowLevel:
                    return "RowLevel";

                case BuiltInStyleIndex.ColumnLevel:
                    return "ColLevel";

                case BuiltInStyleIndex.Comma:
                    return "Comma";

                case BuiltInStyleIndex.Currency:
                    return "Currency";

                case BuiltInStyleIndex.Percent:
                    return "Percent";

                case BuiltInStyleIndex.Comma0:
                    return "Comma [0]";

                case BuiltInStyleIndex.Currency0:
                    return "Currency [0]";

                case BuiltInStyleIndex.HyperLink:
                    return "Hyperlink";

                case BuiltInStyleIndex.FollowedHyperLink:
                    return "Followed Hyperlink";

                case BuiltInStyleIndex.Note:
                    return "Note";

                case BuiltInStyleIndex.WarningText:
                    return "Warning Text";

                case BuiltInStyleIndex.Title:
                    return "Title";

                case BuiltInStyleIndex.Heading1:
                    return "Heading 1";

                case BuiltInStyleIndex.Heading2:
                    return "Heading 2";

                case BuiltInStyleIndex.Heading3:
                    return "Heading 3";

                case BuiltInStyleIndex.Heading4:
                    return "Heading 4";

                case BuiltInStyleIndex.Input:
                    return "Input";

                case BuiltInStyleIndex.Output:
                    return "Output";

                case BuiltInStyleIndex.Calculation:
                    return "Calculation";

                case BuiltInStyleIndex.CheckCell:
                    return "Check Cell";

                case BuiltInStyleIndex.LinkedCell:
                    return "Linked Cell";

                case BuiltInStyleIndex.Total:
                    return "Total";

                case BuiltInStyleIndex.Good:
                    return "Good";

                case BuiltInStyleIndex.Bad:
                    return "Bad";

                case BuiltInStyleIndex.Neutral:
                    return "Neutral";

                case BuiltInStyleIndex.Accent1:
                    return "Accent1";

                case BuiltInStyleIndex.Accent1_20:
                    return "20% - Accent1";

                case BuiltInStyleIndex.Accent1_40:
                    return "40% - Accent1";

                case BuiltInStyleIndex.Accent1_60:
                    return "60% - Accent1";

                case BuiltInStyleIndex.Accent2:
                    return "Accent2";

                case BuiltInStyleIndex.Accent2_20:
                    return "20% - Accent2";

                case BuiltInStyleIndex.Accent2_40:
                    return "40% - Accent2";

                case BuiltInStyleIndex.Accent2_60:
                    return "60% - Accent2";

                case BuiltInStyleIndex.Accent3:
                    return "Accent3";

                case BuiltInStyleIndex.Accent3_20:
                    return "20% - Accent3";

                case BuiltInStyleIndex.Accent3_40:
                    return "40% - Accent3";

                case BuiltInStyleIndex.Accent3_60:
                    return "60% - Accent3";

                case BuiltInStyleIndex.Accent4:
                    return "Accent4";

                case BuiltInStyleIndex.Accent4_20:
                    return "20% - Accent4";

                case BuiltInStyleIndex.Accent4_40:
                    return "40% - Accent4";

                case BuiltInStyleIndex.Accent4_60:
                    return "60% - Accent4";

                case BuiltInStyleIndex.Accent5:
                    return "Accent5";

                case BuiltInStyleIndex.Accent5_20:
                    return "20% - Accent5";

                case BuiltInStyleIndex.Accent5_40:
                    return "40% - Accent5";

                case BuiltInStyleIndex.Accent5_60:
                    return "60% - Accent5";

                case BuiltInStyleIndex.Accent6:
                    return "Accent6";

                case BuiltInStyleIndex.Accent6_20:
                    return "20% - Accent6";

                case BuiltInStyleIndex.Accent6_40:
                    return "40% - Accent6";

                case BuiltInStyleIndex.Accent6_60:
                    return "60% - Accent6";

                case BuiltInStyleIndex.ExplanatoryText:
                    return "Explanatory Text";
            }
            return string.Empty;
        }
    }
}

