#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class MeasureInfo
    {
        public MeasureInfo()
        {
        }

        public MeasureInfo(Cell cell, Size maxSize)
        {
            FontFamily = cell.FontFamily;
            cell.CacheStyleObject(true);
            FontSize = cell.FontSize;
            FontStretch = cell.FontStretch;
            FontStyle = cell.FontStyle;
            FontWeight = cell.FontWeight;
            WordWrap = cell.WordWrap;
            cell.CacheStyleObject(false);
            MaximumSize = maxSize;
        }

        public override bool Equals(object obj)
        {
            MeasureInfo info = obj as MeasureInfo;
            if (info == null)
            {
                return false;
            }
            return (((((FontFamily == info.FontFamily) && (FontSize == info.FontSize)) && ((FontStretch == info.FontStretch) && (FontStyle == info.FontStyle))) && ((FontWeight.Weight == info.FontWeight.Weight) && (MaximumSize == info.MaximumSize))) && (WordWrap == info.WordWrap));
        }

        public override int GetHashCode()
        {
            StringBuilder builder = new StringBuilder();
            if (FontFamily != null)
            {
                builder.Append(FontFamily.ToString());
            }
            builder.Append(FontStyle.ToString());
            builder.Append(FontStretch.ToString());
            builder.Append(((ushort) FontWeight.Weight).ToString());
            builder.Append(WordWrap.ToString());
            builder.Append(MaximumSize.ToString());
            return builder.ToString().GetHashCode();
        }

        public Windows.UI.Xaml.Media.FontFamily FontFamily { get; set; }

        public double FontSize { get; set; }

        public Windows.UI.Text.FontStretch FontStretch { get; set; }

        public Windows.UI.Text.FontStyle FontStyle { get; set; }

        public Windows.UI.Text.FontWeight FontWeight { get; set; }

        public Size MaximumSize { get; set; }

        public bool WordWrap { get; set; }
    }
}

