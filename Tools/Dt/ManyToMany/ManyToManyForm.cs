using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Dt
{
    public partial class ManyToManyForm : Form
    {
        public ManyToManyForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            _cbSearch.SelectedIndex = 0;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string ns, mainCls, mainTitle, bClss, bTitles;
            try
            {
                ns = Kit.GetText(_ns);
                mainCls = Kit.GetText(_clsa);
                mainTitle = Kit.GetText(_clsaTitle);
                bClss = Kit.GetText(_clsb);
                bTitles = Kit.GetText(_clsbTitle);
            }
            catch
            {
                _info.Text = "当前内容不可为空！";
                return;
            }

            var relatedClss = bClss.Split(',');
            var relatedTitles = bTitles.Split(',');
            if (relatedClss.Length != relatedTitles.Length)
            {
                MessageBox.Show("关联的实体类个数和标题个数不同！");
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$maincls$", mainCls },
                    {"$maintitle$", mainTitle },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Kit.GetFolderPath();
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Obj.cs"), "Dt.ManyToMany.MainObj.cs", dt);

            // $relatedlistxaml$
            // $relatedlistcs$
            // $navitolist$
            // $relatedupdate$
            // $relatedclear$
            // $relatedcls$
            // $relatedtitle$
            string winXaml, winCode, naviTo, update, clear;
            winXaml = winCode = naviTo = update = clear = "";
            for (int i = 0; i < relatedClss.Length; i++)
            {
                var cls = relatedClss[i].Trim();
                var rtitle = relatedTitles[i].Trim();
                dt["$relatedcls$"] = cls;
                dt["$relatedtitle$"] = rtitle;

                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}List.xaml"), "Dt.ManyToMany.MainRelatedList.xaml", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}List.xaml.cs"), "Dt.ManyToMany.MainRelatedList.xaml.cs", dt);

                if (i > 0)
                {
                    winXaml += "\r\n";
                    winCode += "\r\n";
                    update += "\r\n";
                    clear += "\r\n";
                }

                winXaml += $"            <a:Tab>\r\n                <l:{mainCls}{cls}List x:Name=\"_{char.ToLower(cls[0])}{cls.Substring(1)}List\" />\r\n            </a:Tab>";
                winCode += $"        public {mainCls}{cls}List {cls}List => _{char.ToLower(cls[0])}{cls.Substring(1)}List;\r\n";
                naviTo += $" _win.{cls}List,";
                update += $"            _win.{cls}List.Update(p_id);";
                clear += $"            _win.{cls}List.Clear();";
            }

            // MainWin
            dt["$relatedlistxaml$"] = winXaml;
            dt["$relatedlistcs$"] = winCode;
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Win.xaml"), "Dt.ManyToMany.MainWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Win.xaml.cs"), "Dt.ManyToMany.MainWin.xaml.cs", dt);

            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Form.xaml"), "Dt.ManyToMany.MainForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}List.xaml"), "Dt.ManyToMany.MainList.xaml", dt);

            // MainForm
            dt["$relatedupdate$"] = update;
            dt["$relatedclear$"] = clear;
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Form.xaml.cs"), "Dt.ManyToMany.MainForm.xaml.cs", dt);

            // MainList
            dt["$navitolist$"] = naviTo;
            string cs;
            if (_cbSearch.SelectedIndex == 0)
            {
                cs = "DefaultSearch";
            }
            else
            {
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Search.xaml"), "Dt.ManyToMany.MainSearch.xaml", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Search.xaml.cs"), "Dt.ManyToMany.MainSearch.xaml.cs", dt);
                cs = "CustomSearch";
            }

            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.ManyToMany.{cs}.cs")))
            {
                dt["$listsearchcs$"] = sr.ReadToEnd().Replace("$maincls$", mainCls).Replace("$maintitle$", mainTitle);
            }
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}List.xaml.cs"), "Dt.ManyToMany.MainList.xaml.cs", dt);

            Close();
        }


    }
}
