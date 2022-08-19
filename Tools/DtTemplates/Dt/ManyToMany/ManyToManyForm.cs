using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.ManyToMany
{
    public partial class ManyToManyForm : Form
    {
        FileParams _params;
        string _path;

        public ManyToManyForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            _cbSearch.SelectedIndex = 0;
            _svcUrl.Text = AtSvc.SvcUrl;
            AddTooltip();
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _params = new FileParams
                {
                    NameSpace = Kit.GetText(_ns),
                    Agent = Kit.GetText(_agentName),
                    MainTbl = _cbTbls.SelectedItem.ToString(),
                    MainEntity = Kit.GetText(_clsa),
                    MainTitle = Kit.GetText(_clsaTitle),
                };

                var bClss = Kit.GetText(_clsb);
                _params.RelatedEntities = bClss.Replace(" ", "").Split(',');
                var bTitles = Kit.GetText(_clsbTitle);
                _params.RelatedTitles = bTitles.Replace(" ", "").Split(',');
            }
            catch
            {
                _info.Text = "当前内容不可为空！";
                return;
            }

            if (_params.RelatedEntities.Length != _params.RelatedTitles.Length)
            {
                MessageBox.Show("关联的实体类个数和标题个数不同！");
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("正在生成多对多框架...")
                .AppendLine(_params.IsSelectedMainTbl ? $"选择主表：{_cbTbls.SelectedItem}" : "未选择主表")
                .AppendLine($"主实体名称：{_params.MainEntity}")
                .AppendLine($"主实体标题：{_params.MainTitle}")
                .AppendLine($"关联实体名称：{_clsb.Text}")
                .AppendLine($"关联实体标题：{_clsbTitle.Text}")
                .AppendLine(_cbSearch.SelectedIndex == 0 ? "通用搜索面板" : "自定义搜索面板");

            _path = Kit.GetFolderPath();
            await WriteEntityObj(_params.MainTbl, _params.MainEntity);
            var code = WriteReleation();
            WriteMainWin(code);
            await WriteMainForm(code);
            WriteMainSearch();
            await WriteMainList(code);

            await CreateSql(sb);
            Kit.Output(sb.ToString());
            Close();
        }

        GenaralCode WriteReleation()
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
            for (int i = 0; i < _params.RelatedEntities.Length; i++)
            {
                var cls = _params.RelatedEntities[i];
                var rtitle = _params.RelatedTitles[i];
                dt["$relatedcls$"] = cls;
                dt["$relatedtitle$"] = rtitle;

                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}List.xaml.cs"), "Dt.ManyToMany.Res.MainRelatedList.xaml.cs", dt);
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}{cls}List.xaml"), "Dt.ManyToMany.Res.MainRelatedList.xaml", dt);

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

            dt["$relatedlist$"] = p_code.WinXaml;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Win.xaml"), $"Dt.ManyToMany.Res.MainWin.xaml", dt);

            dt["$time$"] = _params.Time;
            dt["$username$"] = _params.UserName;
            dt["$relatedlist$"] = p_code.WinCode;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Win.xaml.cs"), $"Dt.ManyToMany.Res.MainWin.xaml.cs", dt);
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
                var body = await AtSvc.GetFvCells(_params.MainTbl);
                // 可能包含命名空间
                dt["$fvbody$"] = body.Replace("$namespace$", _params.NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());
            }
            else
            {
                dt["$fvbody$"] = "        <a:CText ID=\"name\" Title=\"名称\" />\r\n        <a:CText ID=\"note\" Title=\"描述\" />";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Form.xaml"), "Dt.ManyToMany.Res.MainForm.xaml", dt);
            dt.Remove("$fvbody$");

            dt["$agent$"] = _params.Agent;
            dt["$time$"] = _params.Time;
            dt["$username$"] = _params.UserName;
            dt["$relatedupdate$"] = p_code.Update;
            dt["$relatedclear$"] = p_code.Clear;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}Form.xaml.cs"), "Dt.ManyToMany.Res.MainForm.xaml.cs", dt);
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

            if (_params.IsSelectedMainTbl)
            {
                var body = await AtSvc.GetLvItemTemplate(_params.MainTbl);
                dt["$lvbody$"] = body;
            }
            else
            {
                dt["$lvbody$"] = "            <StackPanel Padding=\"10\">\r\n                <a:Dot ID=\"name\" />\r\n                <a:Dot ID=\"note\" Font=\"小灰\" />\r\n            </StackPanel>";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}List.xaml"), "Dt.ManyToMany.Res.MainList.xaml", dt);
            dt.Remove("$lvbody$");

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
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.MainEntity}List.xaml.cs"), "Dt.ManyToMany.Res.MainList.xaml.cs", dt);
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

        async Task CreateSql(StringBuilder p_sb)
        {
            if (_cbSql.Checked && _params.IsSelectedMainTbl)
            {
                string msg = await AtSvc.GetManyToManySql(
                        _params.MainTbl,
                        _params.MainTitle,
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

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel1, Kit.DataProviderTip);
            tip.SetToolTip(linkLabel2, Kit.EntityTip);
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);

            tip.SetToolTip(linkLabel5,
@"和主实体相同，一般为不包含前后缀的表名
请确保所有关联实体类在其他位置已生成
本次不生成任何关联实体类");

            tip.SetToolTip(linkLabel7,
@"当选择的表名有效时，可生成以下键名的sql：
1. 主实体标题-全部
2. 主实体标题-模糊查询，通用搜索面板时生成
3. 主实体标题-编辑
当lob_sql中存在某键名时，不覆盖");
        }
    }
}
