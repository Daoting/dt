#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Text;
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class ExternalFontEventArgs : EventArgs
    {
        byte[] fontData;
        string fontFamilyName;
        Dt.Pdf.Text.SimpleTrueTypeFont simpleTrueTypeFont;

        public ExternalFontEventArgs(string fontFamilyName)
        {
            this.fontFamilyName = fontFamilyName;
        }

        public byte[] FontData
        {
            get { return  this.fontData; }
            set { this.fontData = value; }
        }

        public string FontFamilyName
        {
            get { return  this.fontFamilyName; }
        }

        public Dt.Pdf.Text.SimpleTrueTypeFont SimpleTrueTypeFont
        {
            get { return  this.simpleTrueTypeFont; }
            set { this.simpleTrueTypeFont = value; }
        }
    }
}

