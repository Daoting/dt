using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Dt
{
    public partial class OnToManyForm : Form
    {
        public OnToManyForm()
        {
            InitializeComponent();
            _nameSpace.Text = Kit.GetNamespace();
            _cbSearch.SelectedIndex = 0;
            _cbWin.SelectedIndex = 0;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string ns, agent, mainCls, mainTitle, bClss, bTitles;
            try
            {
                ns = Kit.GetText(_nameSpace);
                agent = Kit.GetText(_agentName);
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

            var childClss = bClss.Split(',');
            var childTitles = bTitles.Split(',');
            if (childClss.Length != childTitles.Length)
            {
                MessageBox.Show("子类个数和子类标题个数不同！");
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$agent$", agent },
                    {"$maincls$", mainCls },
                    {"$maintitle$", mainTitle },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };
            var path = Kit.GetFolderPath();
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Obj.cs"), "Dt.OnToMany.ParentObj.cs", dt);

            string winXaml, winCode, naviTo, update, clear;
            winXaml = winCode = naviTo = update = clear = "";
            for (int i = 0; i < childClss.Length; i++)
            {
                var cls = childClss[i].Trim();
                var rtitle = childTitles[i].Trim();
                dt["$childcls$"] = cls;
                dt["$childtitle$"] = rtitle;

                Kit.WritePrjFile(Path.Combine(path, $"{cls}Obj.cs"), "Dt.OnToMany.ChildObj.cs", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}List.xaml"), "Dt.OnToMany.ChildList.xaml", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}List.xaml.cs"), "Dt.OnToMany.ChildList.xaml.cs", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}Form.xaml"), "Dt.OnToMany.ChildForm.xaml", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}{cls}Form.xaml.cs"), "Dt.OnToMany.ChildForm.xaml.cs", dt);

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
                update += $"            _win?.{cls}List.Update(p_id);";
                clear += $"            _win?.{cls}List.Clear();";
            }

            // MainWin
            dt["$relatedlistxaml$"] = winXaml;
            dt["$relatedlistcs$"] = winCode;
            var resName = _cbWin.SelectedIndex == 0 ? "TreeWin" : "TwoWin";
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Win.xaml"), $"Dt.OnToMany.{resName}.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Win.xaml.cs"), $"Dt.OnToMany.{resName}.xaml.cs", dt);

            // MainForm
            dt["$relatedupdate$"] = update;
            dt["$relatedclear$"] = clear;
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Form.xaml"), "Dt.OnToMany.ParentForm.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Form.xaml.cs"), "Dt.OnToMany.ParentForm.xaml.cs", dt);

            string cs;
            if (_cbSearch.SelectedIndex == 0)
            {
                cs = "DefaultSearch";
            }
            else
            {
                // 复用ManyToMany中的文件
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Search.xaml"), "Dt.ManyToMany.MainSearch.xaml", dt);
                Kit.WritePrjFile(Path.Combine(path, $"{mainCls}Search.xaml.cs"), "Dt.ManyToMany.MainSearch.xaml.cs", dt);
                cs = "CustomSearch";
            }

            // MainList
            // 复用ManyToMany中的文件
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.ManyToMany.{cs}.cs")))
            {
                dt["$listsearchcs$"] = sr.ReadToEnd().Replace("$maincls$", mainCls).Replace("$maintitle$", mainTitle).Replace("$agent$", agent);
            }
            dt["$navitolist$"] = naviTo;
            resName = _cbWin.SelectedIndex == 0 ? "ThreeList" : "TwoList";
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}List.xaml"), $"Dt.OnToMany.{resName}.xaml", dt);
            Kit.WritePrjFile(Path.Combine(path, $"{mainCls}List.xaml.cs"), $"Dt.OnToMany.{resName}.xaml.cs", dt);

            Close();
        }


    }
}
