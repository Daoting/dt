using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Windows.Forms;

namespace Dt
{
    public static class Kit
    {
        /// <summary>
        /// 将文本粘贴到编辑器
        /// </summary>
        /// <param name="p_txt"></param>
        public static void Paste(string p_txt)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IDataObject dataObject = Clipboard.GetDataObject();
            try
            {
                Clipboard.SetText(p_txt);
                DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
                if (dte != null)
                {
                    dte.ExecuteCommand("Edit.Paste", "");
                }
                else
                {
                    SendKeys.Send("^V");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "无效的内容", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            Clipboard.SetDataObject(dataObject);
        }
    }
}
