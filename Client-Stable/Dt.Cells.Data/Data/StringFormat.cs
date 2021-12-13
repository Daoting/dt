#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Cells.Data
{
    internal class StringFormat
    {
        StringAlignment alignment;
        StringAlignment lineAlignment;
        Microsoft.UI.Xaml.TextWrapping textWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap;

        public StringAlignment Alignment
        {
            get { return  this.alignment; }
            set { this.alignment = value; }
        }

        public StringAlignment LineAlignment
        {
            get { return  this.lineAlignment; }
            set { this.lineAlignment = value; }
        }

        public Microsoft.UI.Xaml.TextWrapping TextWrapping
        {
            get { return  this.textWrapping; }
            set { this.textWrapping = value; }
        }
    }
}

