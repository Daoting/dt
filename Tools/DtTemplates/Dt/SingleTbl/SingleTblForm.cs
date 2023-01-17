using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.SingleTbl
{
    public partial class SingleTblForm : Form
    {
        bool _isSelectTbl;
        FileParams _params;
        string _path;

        public SingleTblForm()
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
                    Entity = Kit.GetText(_entityName),
                    Title = Kit.GetText(_entityTitle),
                };
            }
            catch
            {
                _info.Text = "当前内容不可为空！";
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("正在生成单表框架...")
                .AppendLine(_isSelectTbl ? $"选择表：{_cbTbls.SelectedItem}" : "未选择表")
                .AppendLine($"实体名称：{_params.Entity}")
                .AppendLine($"实体标题：{_params.Title}")
                .Append("窗口布局：")
                .AppendLine(_cbWin.SelectedIndex == 0 ? "三栏" : "两栏")
                .AppendLine(_cbSearch.SelectedIndex == 0 ? "通用搜索面板" : "自定义搜索面板");

            _path = Kit.GetFolderPath();
            await WriteEntityObj();
            await WriteEntityForm();

            if (_cbWin.SelectedIndex == 0)
            {
                // 三栏
                await WriteThreePane();
            }
            else
            {
                // 两栏
                await WriteTwoPane();
            }

            if (_isSelectTbl && _cbSql.Checked)
            {
                string msg = await AtSvc.GetSingleTblSql(_cbTbls.SelectedItem.ToString(), _params.Title, _cbSearch.SelectedIndex == 0);
                sb.AppendLine(msg);
            }
            else
            {
                sb.AppendLine("不生成sql");
            }

            Kit.Output(sb.ToString());
            Close();
        }

        async Task WriteEntityObj()
        {
            if (_isSelectTbl)
            {
                var entity = await AtSvc.GetEntityClass(_cbTbls.SelectedItem.ToString(), _params.Entity + "Obj");
                if (string.IsNullOrEmpty(entity))
                {
                    _isSelectTbl = false;
                }
                else
                {
                    var dt = _params.Params;
                    dt["$entitybody$"] = entity;
                    Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Obj.cs"), "Dt.SingleTbl.Res.EntityObj-tbl.cs", dt);
                }
            }

            if (!_isSelectTbl)
            {
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Obj.cs"), "Dt.SingleTbl.Res.EntityObj.cs", _params.Params);
            }
        }

        async Task WriteEntityForm()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Form.xaml.cs"), "Dt.SingleTbl.Res.EntityForm.xaml.cs", dt);

            if (_isSelectTbl)
            {
                var body = await AtSvc.GetFvCells(new List<string> { _cbTbls.SelectedItem.ToString() });
                // 可能包含命名空间
                dt["$fvbody$"] = body.Replace("$namespace$", _params.NameSpace).Replace("$rootnamespace$", Kit.GetRootNamespace());
            }
            else
            {
                dt["$fvbody$"] = "        <a:CText ID=\"name\" Title=\"名称\" />\r\n        <a:CText ID=\"note\" Title=\"描述\" />";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Form.xaml"), "Dt.SingleTbl.Res.EntityForm.xaml", dt);
        }

        async Task WriteTwoPane()
        {
            var dt = _params.Params;
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml"), "Dt.SingleTbl.Res.TwoPanWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml.cs"), "Dt.SingleTbl.Res.TwoPanWin.xaml.cs", dt);

            if (_cbSearch.SelectedIndex == 1)
            {
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml"), "Dt.SingleTbl.Res.EntitySearch.xaml", dt);
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml.cs"), "Dt.SingleTbl.Res.EntitySearch.xaml.cs", dt);
            }

            // 搜索方式
            string cs = _cbSearch.SelectedIndex == 0 ? "TwoDefSearch" : "TwoCusSearch";
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.SingleTbl.Res.{cs}.cs")))
            {
                var txt = await sr.ReadToEndAsync();
                dt["$listsearchcs$"] = txt
                    .Replace("$entitytitle$", _params.Title)
                    .Replace("$entityname$", _params.Entity)
                    .Replace("$agent$", _params.Agent);
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml.cs"), "Dt.SingleTbl.Res.TwoPanList.xaml.cs", dt);
            dt.Remove("$listsearchcs$");

            if (_isSelectTbl)
            {
                var body = await AtSvc.GetLvItemTemplate(new List<string> { _cbTbls.SelectedItem.ToString() });
                dt["$lvbody$"] = body;
            }
            else
            {
                dt["$lvbody$"] = "            <StackPanel Padding=\"10\">\r\n                <a:Dot ID=\"name\" />\r\n                <a:Dot ID=\"note\" Font=\"小灰\" />\r\n            </StackPanel>";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml"), "Dt.SingleTbl.Res.TwoPanList.xaml", dt);
        }

        async Task WriteThreePane()
        {
            var dt = _params.Params;
            if (_cbSearch.SelectedIndex == 1)
            {
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml"), "Dt.SingleTbl.Res.EntitySearch.xaml", dt);
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml.cs"), "Dt.SingleTbl.Res.EntitySearch.xaml.cs", dt);
            }

            dt["$winsearchxaml$"] = _cbSearch.SelectedIndex == 0 ?
                $"                <a:SearchMv x:Name=\"_search\" Placeholder=\"{_params.Title}\" Search=\"OnSearch\">\r\n                    <x:String>全部</x:String>\r\n                </a:SearchMv>"
                : $"                <l:{_params.Entity}Search x:Name=\"_search\" Search=\"OnSearch\" />";
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml"), "Dt.SingleTbl.Res.EntityWin.xaml", dt);
            dt.Remove("$winsearchxaml$");

            dt["$winsearchcs$"] = _cbSearch.SelectedIndex == 0 ?
                "        public SearchMv Search => _search;\r\n\r\n        void OnSearch(object sender, string e)\r\n        {\r\n            _list.OnSearch(e);\r\n        }"
                : $"        public {_params.Entity}Search Search => _search;\r\n\r\n        void OnSearch(object sender, Row e)\r\n        {{\r\n            _list.OnSearch(e);\r\n        }}";
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml.cs"), "Dt.SingleTbl.Res.EntityWin.xaml.cs", dt);
            dt.Remove("$winsearchcs$");

            string listSearchCs = _cbSearch.SelectedIndex == 0 ? "EntityDefSearch" : "EntityCusSearch";
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.SingleTbl.Res.{listSearchCs}.cs")))
            {
                var txt = await sr.ReadToEndAsync();
                dt["$listsearchcs$"] = txt
                    .Replace("$entitytitle$", _params.Title)
                    .Replace("$entityname$", _params.Entity)
                    .Replace("$agent$", _params.Agent);
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml.cs"), "Dt.SingleTbl.Res.EntityList.xaml.cs", dt);
            dt.Remove("$listsearchcs$");

            if (_isSelectTbl)
            {
                var body = await AtSvc.GetLvItemTemplate(new List<string> { _cbTbls.SelectedItem.ToString() });
                dt["$lvbody$"] = body;
            }
            else
            {
                dt["$lvbody$"] = "            <StackPanel Padding=\"10\">\r\n                <a:Dot ID=\"name\" />\r\n                <a:Dot ID=\"note\" Font=\"小灰\" />\r\n            </StackPanel>";
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml"), "Dt.SingleTbl.Res.EntityList.xaml", dt);
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
            _isSelectTbl = !string.IsNullOrEmpty(tbl);
            if (_isSelectTbl)
            {
                _entityName.Text = Kit.GetClsName(tbl);
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel1, Kit.DataProviderTip);
            tip.SetToolTip(linkLabel2, Kit.RootNameTip);
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
            tip.SetToolTip(linkLabel5, Kit.AutoSqlTip);
        }
    }
}
