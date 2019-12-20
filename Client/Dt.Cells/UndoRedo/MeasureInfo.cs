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

        public MeasureInfo(Cell cell, Windows.Foundation.Size maxSize)
        {
            this.FontFamily = cell.FontFamily;
            cell.CacheStyleObject(true);
            this.FontSize = cell.FontSize;
            this.FontStretch = cell.FontStretch;
            this.FontStyle = cell.FontStyle;
            this.FontWeight = cell.FontWeight;
            this.WordWrap = cell.WordWrap;
            cell.CacheStyleObject(false);
            this.MaximumSize = maxSize;
        }

        public override bool Equals(object obj)
        {
            MeasureInfo info = obj as MeasureInfo;
            if (info == null)
            {
                return false;
            }
            return (((((this.FontFamily == info.FontFamily) && (this.FontSize == info.FontSize)) && ((this.FontStretch == info.FontStretch) && (this.FontStyle == info.FontStyle))) && ((this.FontWeight.Weight == info.FontWeight.Weight) && (this.MaximumSize == info.MaximumSize))) && (this.WordWrap == info.WordWrap));
        }

        public override int GetHashCode()
        {
            StringBuilder builder = new StringBuilder();
            if (this.FontFamily != null)
            {
                builder.Append(this.FontFamily.ToString());
            }
            builder.Append(this.FontStyle.ToString());
            builder.Append(this.FontStretch.ToString());
            builder.Append(((ushort) this.FontWeight.Weight).ToString());
            builder.Append(this.WordWrap.ToString());
            builder.Append(this.MaximumSize.ToString());
            return builder.ToString().GetHashCode();
        }

        public Windows.UI.Xaml.Media.FontFamily FontFamily { get; set; }

        public double FontSize { get; set; }

        public Windows.UI.Text.FontStretch FontStretch { get; set; }

        public Windows.UI.Text.FontStyle FontStyle { get; set; }

        public Windows.UI.Text.FontWeight FontWeight { get; set; }

        public Windows.Foundation.Size MaximumSize { get; set; }

        public bool WordWrap { get; set; }
    }
}

