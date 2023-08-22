using Dt.Core;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
                    // 只使用第一级的模板名作为命名空间
                    ns = folder.Name;

                    // 父文件夹添加前面
                    //ns = $"{folder.Name}.{ns}";
                }
                else
                {
                    break;
                }
            }
            return root + "." + ns;
        }

        public static string GetRootNamespace()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var projects = (UIHierarchyItem[])dte2?.ToolWindows.SolutionExplorer.SelectedItems;
            var folder = projects[0].Object as ProjectItem;
            return folder.ContainingProject.Properties.Item("RootNamespace").Value.ToString();
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

        /// <summary>
        /// 是否为客户端项目
        /// </summary>
        /// <returns></returns>
        public static bool IsClientPrj()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var folder = ((UIHierarchyItem[])dte2?.ToolWindows.SolutionExplorer.SelectedItems)[0].Object as ProjectItem;
            var tgts = folder.ContainingProject.Properties.Item("TargetFrameworks").Value.ToString();
            return tgts.Contains("-android") || tgts.Contains("-ios") || tgts.Contains("-windows");
        }

        /// <summary>
        /// 是否为服务项目
        /// </summary>
        /// <returns></returns>
        public static bool IsSvcPrj()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var folder = ((UIHierarchyItem[])dte2?.ToolWindows.SolutionExplorer.SelectedItems)[0].Object as ProjectItem;
            var tgts = folder.ContainingProject.Properties.Item("TargetFrameworks").Value.ToString();
            return string.IsNullOrEmpty(tgts);
        }

        /// <summary>
        /// 在输出窗口显示信息
        /// </summary>
        /// <param name="p_msg"></param>
        public static void Output(string p_msg)
        {
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            var win = dte2?.ToolWindows.OutputWindow;
            OutputWindowPane pane;
            try
            {
                pane = win.OutputWindowPanes.Item("搬运工");
            }
            catch
            {
                pane = win.OutputWindowPanes.Add("搬运工");
            }

            pane.OutputString(p_msg + "\r\n");
            pane.Activate();
        }

        public static void WritePrjFile(string p_filePath, string p_templateName, Dictionary<string, string> p_replace, bool p_checkExists = true)
        {
            if (p_checkExists && File.Exists(p_filePath))
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

        public static string GetText(TextBox p_tb, string p_msg)
        {
            var txt = p_tb.Text.Trim();
            if (txt == "")
            {
                p_tb.Focus();
                MessageBox.Show(p_msg);
                throw new Exception();
            }
            return txt;
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

        /// <summary>
        /// 表名不包括前缀
        /// </summary>
        public static bool TblNameNoPrefix = false;

        public static string GetClsName(string p_tblName)
        {
            string clsName;
            string[] arr = p_tblName.ToLower().Split('_');
            if (arr.Length > 1)
            {
                clsName = SetFirstToUpper(arr[1]);
                if (arr.Length > 2)
                {
                    for (int i = 2; i < arr.Length; i++)
                    {
                        clsName += SetFirstToUpper(arr[i]);
                    }
                }
            }
            else
            {
                clsName = SetFirstToUpper(p_tblName);
            }
            return clsName;
        }

        public static void OpenFile(string p_filePath)
        {
            var dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
            if (File.Exists(p_filePath))
                dte2.ItemOperations.OpenFile(p_filePath);
        }

        static string SetFirstToUpper(string p_str)
        {
            char[] a = p_str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        #region Tooltip
        public const string DataProviderTip =
@"该类在生成的代码中用到，请确认该类存在
默认提供 AtSvc 类做为远程服务的数据操作";

        public const string RootNameTip =
@"生成的窗口、列表、表单、查询等类的词根，命名规范如：
查询：词根 + Query
窗口：词根 + Win
列表：词根 + List
表单：词根 + Form";

        public const string SvcUrlTip =
@"请确保当前服务正在运行，通过服务：
1. 获取所有表目录
2. 根据表结构动态生成列表、表单等xaml内容和框架代码";

        public const string SvcNameTip =
@"单体模式时用于区分不同服务；
不同服务可能连接不同数据库；
非单体模式只有一个服务";

        public const string AllTblsTip =
@"通过服务获取的所有表目录
服务不可用时目录为空";

        public const string AutoSqlTip =
@"当选择的表名有效时，可生成以下键名的sql：
1. XXX-全部
2. XXX-模糊查询，通用搜索面板时生成
3. XXX-编辑
XXX为实体中文标题
当lob_sql中存在某键名时，不覆盖";
        #endregion
    }
}
