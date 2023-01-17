using Dt.Core;
using Dt.Editor;
using Microsoft.VisualStudio.RpcContracts.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.OnToMany
{
    public partial class OnToManyForm : Form
    {
        FileParams _params;
        string _path;

        public OnToManyForm()
        {
            InitializeComponent();
            _nameSpace.Text = Kit.GetNamespace();
            _cbSearch.SelectedIndex = 0;
            _cbWin.SelectedIndex = 0;
            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _params = new FileParams
                {
                    NameSpace = Kit.GetText(_nameSpace),
                    Agent = Kit.GetText(_agentName),
                    MainTbl = _cbTbls.SelectedItem.ToString(),
                    MainEntity = Kit.GetText(_clsa),
                    MainTitle = Kit.GetText(_clsaTitle),
                };

                var ctbls = _childTbls.Text;
                if (!string.IsNullOrEmpty(ctbls))
                    _params.ChildTbls = ctbls.Replace(" ", "").Split(',');

                var bClss = Kit.GetText(_clsb);
                _params.ChildEntities = bClss.Replace(" ", "").Split(',');
                var bTitles = Kit.GetText(_clsbTitle);
                _params.ChildTitles = bTitles.Replace(" ", "").Split(',');
            }
            catch
            {
                _info.Text = "当前内容不可为空！";
                return;
            }

            if (_params.ChildEntities.Length != _params.ChildTitles.Length)
            {
                MessageBox.Show("子实体个数和子实体标题个数不同！");
                return;
            }

            if (_params.IsSelectedChildTbls && _params.ChildTbls.Length != _params.ChildEntities.Length)
            {
                MessageBox.Show("子表个数和子实体个数不同！");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("正在生成一对多框架...")
                .AppendLine(_params.IsSelectedMainTbl ? $"选择父表：{_cbTbls.SelectedItem}" : "未选择父表")
                .AppendLine($"父实体名称：{_params.MainEntity}")
                .AppendLine($"父实体标题：{_params.MainTitle}")
                .AppendLine(_params.IsSelectedChildTbls ? $"选择子表：{_childTbls.Text}" : "未选择子表")
                .AppendLine($"子实体名称：{_clsb.Text}")
                .AppendLine($"子实体标题：{_clsbTitle.Text}")
                .Append("窗口布局：")
                .AppendLine(_cbWin.SelectedIndex == 0 ? "三栏" : "两栏")
                .AppendLine(_cbSearch.SelectedIndex == 0 ? "通用搜索面板" : "自定义搜索面板");

            _path = Kit.GetFolderPath();
            await WriteEntityObj(_params.MainTbl, _params.MainEntity);

            for (int i = 0; i < _params.ChildEntities.Length; i++)
            {
                await WriteEntityObj(_params.ChildTbls != null ? _params.ChildTbls[i] : null, _params.ChildEntities[i]);
            }

            var code = await WriteChildren();

            WriteMainWin(code);
            await WriteMainForm(code);
            WriteMainSearch();
            await WriteMainList(code);

            await CreateSql(sb);
            Kit.Output(sb.ToString());
            Close();
        }

        async Task WriteEntityObj(string p_tbl, string p_entity)
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$agent$", _params.Agent },
                {"$entityname$", p_entity },
                {"$time$", _params.Time },
                {"$username$", _params.UserName },
            };

            if (!string.IsNullOrEmpty(p_tbl))
            {
                var entity = await AtSvc.GetEntityClass(p_tbl, p_entity + "Obj");
                dt["$entitybody$"] = entity;
                Kit.WritePrjFile(Path.Combine(_path, $"{p_entity}Obj.cs"), "Dt.SingleTbl.Res.EntityObj-tbl.cs", dt);
            }
            else
            {
                Kit.WritePrjFile(Path.Combine(_path, $"{p_entity}Obj.cs"), "Dt.SingleTbl.Res.EntityObj.cs", dt);
            }
        }

        async Task<GenaralCode> WriteChildren()
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$agent$", _params.Agent },
                {"$maincls$", _params.MainEntity },
                {"$maintitle$", _params.MainTitle },
                {"$time$", _params.Time },
                {"$username$", _params.UserName },
            };

            GenaralCode code = new GenaralCode();
            for (int i = 0; i < _params.ChildEntities.Length; i++)
            {
                var cls = _params.ChildEntities[i];
                var rtitle = _params.ChildTitles[i];
                dt["$childcls$"] = cls;
                dt["$childtitle$"] = rtitle;

                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}Form.xaml.cs"), "Dt.OnToMany.Res.ChildForm.xaml.cs", dt);
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}List.xaml.cs"), "Dt.OnToMany.Res.ChildList.xaml.cs", dt);

                if (_params.IsSelectedChildTbls)
                {
                    var body = await AtSvc.GetFvCells(new List<string> { _params.ChildTbls[i] });
                    // 可能包含命名空间
                    dt["$fvbody$"] = body.Replace("$namespace$", _params.NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());
                }
                else
                {
                    dt["$fvbody$"] = "        <a:CText ID=\"name\" Title=\"名称\" />\r\n        <a:CText ID=\"note\" Title=\"描述\" />";
                }
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}Form.xaml"), "Dt.OnToMany.Res.ChildForm.xaml", dt);
                dt.Remove("$fvbody$");

                if (_params.IsSelectedChildTbls)
                {
                    var body = await AtSvc.GetLvItemTemplate(new List<string> { _params.ChildTbls[i] });
                    dt["$lvbody$"] = body;
                }
                else
                {
                    dt["$lvbody$"] = "            <StackPanel Padding=\"10\">\r\n                <a:Dot ID=\"name\" />\r\n                <a:Dot ID=\"note\" Font=\"小灰\" />\r\n            </StackPanel>";
                }
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}List.xaml"), "Dt.OnToMany.Res.ChildList.xaml", dt);
                dt.Remove("$lvbody$");

                if (i > 0)
                {
                    code.WinXaml += "\r\n";
                    code.WinCode += "\r\n";
                    code.Update += "\r\n";
                    code.Clear += "\r\n";
                }

                code.WinXaml += $"            <a:Tab>\r\n                <l:{_params.MainEntity}{cls}List x:Name=\"_{char.ToLower(cls[0])}{cls.Substring(1)}List\" />\r\n            </a:Tab>";
                code.WinCode += $"        public {_params.MainEntity}{cls}List {cls}List => _{char.ToLower(cls[0])}{cls.Substring(1)}List;\r\n";
                code.NaviTo += $" _win.{cls}List,";
                code.Update += $"            _win?.{cls}List.Update(p_id);";
                code.Clear += $"            _win?.{cls}List.Clear();";
            }
            return code;
        }

        void WriteMainWin(GenaralCode p_code)
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$maincls$", _params.MainEntity },
            };

            var resName = _cbWin.SelectedIndex == 0 ? "ThreeWin" : "TwoWin";
            dt["$relatedlist$"] = p_code.WinXaml;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Win.xaml"), $"Dt.OnToMany.Res.{resName}.xaml", dt);

            dt["$time$"] = _params.Time;
            dt["$username$"] = _params.UserName;
            dt["$relatedlist$"] = p_code.WinCode;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Win.xaml.cs"), $"Dt.OnToMany.Res.{resName}.xaml.cs", dt);
        }

        async Task WriteMainForm(GenaralCode p_code)
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$maincls$", _params.MainEntity },
                {"$maintitle$", _params.MainTitle },
            };

            if (_params.IsSelectedMainTbl)
            {
                var body = await AtSvc.GetFvCells(new List<string> { _params.MainTbl });
                // 可能包含命名空间
                dt["$fvbody$"] = body.Replace("$namespace$", _params.NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());
            }
            else
            {
                dt["$fvbody$"] = "        <a:CText ID=\"name\" Title=\"名称\" />\r\n        <a:CText ID=\"note\" Title=\"描述\" />";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Form.xaml"), "Dt.OnToMany.Res.ParentForm.xaml", dt);
            dt.Remove("$fvbody$");

            dt["$agent$"] = _params.Agent;
            dt["$time$"] = _params.Time;
            dt["$username$"] = _params.UserName;
            dt["$relatedupdate$"] = p_code.Update;
            dt["$relatedclear$"] = p_code.Clear;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Form.xaml.cs"), "Dt.OnToMany.Res.ParentForm.xaml.cs", dt);
        }

        private void WriteMainSearch()
        {
            if (_cbSearch.SelectedIndex == 1)
            {
                var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", _params.NameSpace },
                    {"$entityname$", _params.MainEntity },
                };

                // 复用SingleTbl中的文件
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Search.xaml"), "Dt.SingleTbl.Res.EntitySearch.xaml", dt);

                dt["$time$"] = _params.Time;
                dt["$username$"] = _params.UserName;
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Search.xaml.cs"), "Dt.SingleTbl.Res.EntitySearch.xaml.cs", dt);
            }
        }

        async Task WriteMainList(GenaralCode p_code)
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$maincls$", _params.MainEntity },
                {"$maintitle$", _params.MainTitle },
            };

            var resName = _cbWin.SelectedIndex == 0 ? "ThreeList" : "TwoList";
            if (_params.IsSelectedMainTbl)
            {
                var body = await AtSvc.GetLvItemTemplate(new List<string> { _params.MainTbl });
                dt["$lvbody$"] = body;
            }
            else
            {
                dt["$lvbody$"] = "            <StackPanel Padding=\"10\">\r\n                <a:Dot ID=\"name\" />\r\n                <a:Dot ID=\"note\" Font=\"小灰\" />\r\n            </StackPanel>";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}List.xaml"), $"Dt.OnToMany.Res.{resName}.xaml", dt);
            dt.Remove("$lvbody$");

            // 复用ManyToMany中的文件
            string cs = _cbSearch.SelectedIndex == 0 ? "DefaultSearch" : "CustomSearch";
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.ManyToMany.Res.{cs}.cs")))
            {
                var content = await sr.ReadToEndAsync();
                dt["$listsearchcs$"] = content
                    .Replace("$maincls$", _params.MainEntity)
                    .Replace("$maintitle$", _params.MainTitle)
                    .Replace("$agent$", _params.Agent);
            }

            dt["$agent$"] = _params.Agent;
            dt["$time$"] = _params.Time;
            dt["$username$"] = _params.UserName;
            dt["$navitolist$"] = p_code.NaviTo;
            dt["$relatedupdate$"] = p_code.Update;
            dt["$relatedclear$"] = p_code.Clear;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}List.xaml.cs"), $"Dt.OnToMany.Res.{resName}.xaml.cs", dt);
        }

        async Task CreateSql(StringBuilder p_sb)
        {
            if (_cbSql.Checked && _params.IsSelectedMainTbl)
            {
                string msg = await AtSvc.GetOneToManySql(
                        _params.MainTbl,
                        _params.MainTitle,
                        _params.ChildTbls?.ToList(),
                        _params.ChildTitles?.ToList(),
                        _cbSearch.SelectedIndex == 0);
                p_sb.AppendLine(msg);
            }
            else
            {
                p_sb.AppendLine("不生成sql");
            }
        }

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            var ls = await AtSvc.GetAllTables();
            if (_cbTbls.DataSource != ls)
                _cbTbls.DataSource = ls;
        }

        private void _cbTbls_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var tbl = _cbTbls.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(tbl))
            {
                _clsa.Text = Kit.GetClsName(tbl);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dlg = new SelectChildTbls(_childTbls.Text);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string tbls = "";
                string titles = "";
                foreach (var item in dlg.GetSelection())
                {
                    if (tbls.Length > 0)
                        tbls += ",";
                    tbls += item;

                    if (titles.Length > 0)
                        titles += ",";
                    titles += Kit.GetClsName(item);
                }
                _childTbls.Text = tbls;
                if (!string.IsNullOrEmpty(titles))
                    _clsb.Text = titles;
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel1, Kit.DataProviderTip);
            tip.SetToolTip(linkLabel2, Kit.RootNameTip);
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
            tip.SetToolTip(linkLabel5, "服务运行时可选择多个子表\r\n子表需包含 [ParentID] 字段");

            tip.SetToolTip(linkLabel6,
@"和父实体相同，一般为不包含前后缀的表名，是所有生成类的根命名
生成的实体类、窗口、列表、表单等的命名规范：
实体类：实体 + Obj
窗口：实体 + Win
列表：实体 + List
表单：实体 + Form");

            tip.SetToolTip(linkLabel7,
@"当选择的表名有效时，可生成以下键名的sql：
1. 父标题-全部
2. 父标题-模糊查询，通用搜索面板时生成
3. 父标题-编辑
4. 父标题-关联子标题
5. 子标题-编辑
当lob_sql中存在某键名时，不覆盖");
        }
    }
}
