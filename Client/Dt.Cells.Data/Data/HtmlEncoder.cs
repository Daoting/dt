#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// HtmlEncoder
    /// </summary>
    internal static class HtmlEncoder
    {
        static string[] htmlCode;

        static HtmlEncoder()
        {
            int num;
            htmlCode = new string[0x100];
            for (num = 0; num < 10; num++)
            {
                htmlCode[num] = "&#00" + ((int) num) + ";";
            }
            for (num = 10; num < 0x20; num++)
            {
                htmlCode[num] = "&#0" + ((int) num) + ";";
            }
            for (num = 0x20; num < 0x80; num++)
            {
                char ch = (char) num;
                htmlCode[num] = ((char) ch).ToString();
            }
            htmlCode[9] = "\t";
            htmlCode[10] = "<br />\n";
            htmlCode[0x22] = "&quot;";
            htmlCode[0x26] = "&amp;";
            htmlCode[60] = "&lt;";
            htmlCode[0x3e] = "&gt;";
            for (num = 0x80; num < 0x100; num++)
            {
                htmlCode[num] = "&#" + ((int) num) + ";";
            }
        }

        public static string Encode(string str)
        {
            int length = str.Length;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                char index = str[i];
                if (index < 'Ā')
                {
                    builder.Append(htmlCode[index]);
                }
                else
                {
                    builder.Append("&#").Append((int) index).Append(';');
                }
            }
            return builder.ToString();
        }
    }
}

