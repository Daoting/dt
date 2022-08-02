using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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

        /// <summary>
        /// 获取默认命名空间
        /// </summary>
        /// <returns></returns>
        public static string GetNamespace()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var projects = (UIHierarchyItem[])dte2?.ToolWindows.SolutionExplorer.SelectedItems;
            var folder = projects[0].Object as ProjectItem;
            var root = folder.ContainingProject.Properties.Item("RootNamespace").Value.ToString();

            // 类型为文件夹
            string ns = folder.Name;
            while (true)
            {
                folder = folder.Collection.Parent as ProjectItem;
                if (folder != null && folder.Kind == "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}")
                {
                    // 父文件夹添加前面
                    ns = $"{folder.Name}.{ns}";
                }
                else
                {
                    break;
                }
            }
            return root + "." + ns;
        }

        /// <summary>
        /// 获取选择的文件夹的完整路径
        /// </summary>
        /// <returns></returns>
        public static string GetFolderPath()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var projects = (UIHierarchyItem[])dte2?.ToolWindows.SolutionExplorer.SelectedItems;
            var folder = projects[0].Object as ProjectItem;
            return folder.Properties.Item("FullPath").Value.ToString();
        }

        public static void WritePrjFile(string p_filePath, string p_templateName, Dictionary<string, string> p_replace)
        {
            if (File.Exists(p_filePath))
            {
                if (MessageBox.Show($"{Path.GetFileName(p_filePath)}已存在，覆盖后不可恢复，确认要覆盖此文件吗？", "覆盖文件", MessageBoxButtons.OKCancel)
                    != DialogResult.OK)
                {
                    return;
                }
            }

            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream(p_templateName)))
            {
                string txt = sr.ReadToEnd();
                foreach (var item in p_replace)
                {
                    txt = txt.Replace(item.Key, item.Value);
                }

                using (var writer = new StreamWriter(File.Open(p_filePath, FileMode.Create, FileAccess.Write), Encoding.UTF8))
                {
                    writer.Write(txt);
                }
            }
        }

        public static string GetText(TextBox p_tb)
        {
            var txt = p_tb.Text.Trim();
            if (txt == "")
            {
                p_tb.Focus();
                throw new Exception();
            }
            return txt;
        }

        #region Tooltip
        public static void ShowDataProviderTip(this LinkLabel p_label)
        {
            p_label.ShowTooltip("该类在生成的代码中用到，但本次并不生成该类\r\n继承自DataProvider<TSvc>或SqliteProvider<Sqlite_name>\r\n支持本地sqlite库或远程服务的数据操作\r\n通过静态方法实现CRUD");
        }

        public static void ShowEntityTip(this LinkLabel p_label)
        {
            p_label.ShowTooltip("一般为不包含前后缀的表名，是所有生成类的根命名\r\n生成的实体类、窗口、列表、表单等的命名规范：\r\n实体类：实体 + Obj\r\n窗口：实体 + Win\r\n列表：实体 + List\r\n表单：实体 + Form");
        }

        public static void ShowTooltip(this LinkLabel p_label, string p_msg)
        {
            if (_tip == null)
                _tip = new ToolTip();

            var pt = p_label.PointToClient(Control.MousePosition);
            _tip.Show(p_msg, p_label, pt);
            _tip.Active = true;
            p_label.LostFocus += OnLabelLostFocus;
        }

        private static void OnLabelLostFocus(object sender, EventArgs e)
        {
            var lbl = (LinkLabel)sender;
            lbl.LostFocus -= OnLabelLostFocus;
            _tip.Hide(lbl);
        }

        static ToolTip _tip;
        #endregion
    }
}
