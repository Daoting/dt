#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2020-08-17 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Cells.UI;
using Dt.Cells.Data;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells;
using Microsoft.UI.Input;
using System;
using System.Text.RegularExpressions;
using System.Linq;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        public bool IsLoadOnly = false;
        public string DocType = "";
        public string XlsTemplate = "";
        public string export = @"C:\fifoEX\";
        public string tmprgf = "", tmpexl = "";
        public string exlfile = "*";
        public string subfolder = "";
        public string filekey = "";
        public bool IsAutoFromat = false;
        public string Template = "";
        public string rgffile = "";
        public string File = "";
        public string Folder 
        {
            get
            {
                if (File=="")
                    return "";
                string strFolderName = File.Substring(0, File.LastIndexOf("\\"));
                return strFolderName;
            }
        }
        public string ShortName
        {
            get
            {
                if (File=="")
                    return "";
                string filename = File.Substring(1+File.LastIndexOf("\\")).Replace(".xlsx","");
                return filename;
            }
        }
        public bool XlsGrid = false;
        public bool YHeaderShow = false;
        public bool XHeaderShow = false;
        public Visibility XlsTab = Visibility.Collapsed;
        public bool XlsNewTab = false;

        public int ColNameToColIndex(string colName)
        {
            if (!Regex.IsMatch(colName.ToUpper(), @"[A-Z]+"))
            {
                ExcelKit.Warn("��Ч����");
                return -1;
            }

            int index = 0;
            char[] chars = colName.ToUpper().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                index += ((int)chars[i] - (int)'A' + 1) * (int)Math.Pow(26, chars.Length - i - 1);
            }
            return index - 1;
        }
    }
}