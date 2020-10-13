#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-13 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Fv静态方法
    /// </summary>
    public partial class Fv : DtControl
    {
        /// <summary>
        /// 根据类型生成格
        /// </summary>
        /// <param name="p_type"></param>
        /// <param name="p_id"></param>
        /// <returns></returns>
        public static FvCell CreateCell(Type p_type, string p_id)
        {
            if (p_type == typeof(string))
                return new CText { ID = p_id };

            if (p_type == typeof(int) || p_type == typeof(long) || p_type == typeof(short))
                return new CNum { ID = p_id, IsInteger = true };

            if (p_type == typeof(double) || p_type == typeof(float))
                return new CNum { ID = p_id };

            if (p_type == typeof(bool))
                return new CBool { ID = p_id };

            if (p_type == typeof(DateTime))
                return new CDate { ID = p_id };

            if (p_type == typeof(Icons))
                return new CIcon { ID = p_id };

            if (p_type.IsEnum)
                return new CList { ID = p_id };

            if (p_type == typeof(SolidColorBrush) || p_type == typeof(Color))
                return new CColor { ID = p_id };

            return new CText { ID = p_id };
        }

        /// <summary>
        /// 根据数据生成格
        /// </summary>
        /// <param name="p_row">包含列：id,title,type</param>
        /// <returns></returns>
        public static FvCell CreateCell(Row p_row)
        {
            // 数据类型
            Type tp;
            switch (p_row.Str("type").ToLower())
            {
                case "bool":
                    tp = typeof(bool);
                    break;
                case "double":
                    tp = typeof(double);
                    break;
                case "int":
                    tp = typeof(int);
                    break;
                case "datetime":
                    tp = typeof(DateTime);
                    break;
                case "date":
                    tp = typeof(DateTime);
                    break;
                default:
                    tp = typeof(string);
                    break;
            }

            FvCell cell = CreateCell(tp, p_row.Str("id"));
            if (!p_row.Bool("showtitle"))
            {
                cell.ShowTitle = false;
            }
            else
            {
                if (p_row.Double("titlewidth") > 0)
                    cell.TitleWidth = p_row.Double("titlewidth");
                string title = p_row.Str("title");
                if (string.IsNullOrEmpty(title))
                    title = p_row.Str("id");
                cell.Title = title;
            }

            if (p_row.Bool("showstar"))
                cell.ShowStar = true;
            if (p_row.Bool("isverticaltitle"))
                cell.IsVerticalTitle = true;
            if (p_row.Bool("ishorstretch"))
                cell.IsHorStretch = true;

            if (p_row.Int("rowspan") > 1)
                cell.RowSpan = p_row.Int("RowSpan");
            if (p_row.Str("placeholder") != string.Empty)
                cell.Placeholder = p_row.Str("Placeholder");
            if (p_row.Bool("isreadonly"))
                cell.IsReadOnly = true;
            if (p_row.Bool("hide"))
                cell.Visibility = Visibility.Collapsed;
            return cell;
        }
    }
}
