using Dt.Core;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.LocalTbl
{
    public partial class LocalTblForm : Form
    {
        FileParams _params;
        string _path;
        Table _schema;

        public LocalTblForm()
        {
            InitializeComponent();
            _nameSpace.Text = Kit.GetNamespace();
            _cbSearch.SelectedIndex = 0;
            _cbWin.SelectedIndex = 0;
            AddTooltip();
        }

        async void _btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                _params = new FileParams
                {
                    NameSpace = Kit.GetText(_nameSpace),
                    Table = _cbTbls.SelectedItem.ToString(),
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
            sb.AppendLine("正在生成sqlite单表框架...")
                .AppendLine($"选择表：{_cbTbls.SelectedItem}")
                .AppendLine($"实体名称：{_params.Entity}")
                .AppendLine($"实体标题：{_params.Title}")
                .Append("窗口布局：")
                .AppendLine(_cbWin.SelectedIndex == 0 ? "三栏" : "两栏")
                .AppendLine(_cbSearch.SelectedIndex == 0 ? "通用搜索面板" : "自定义搜索面板");

            _path = Kit.GetFolderPath();
            _schema = AtLocal.Query($"PRAGMA table_info({_params.Table})");
            WriteEntityForm();

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

            Kit.Output(sb.ToString());
            Close();
        }

        void WriteEntityForm()
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$table$", _params.Table },
                {"$entityname$", _params.Entity },
                {"$entitytitle$", _params.Title },
                {"$time$", _params.Time },
                {"$username$", _params.UserName },
            };
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Form.xaml.cs"), "Dt.LocalTbl.Res.EntityForm.xaml.cs", dt);

            dt["$fvbody$"] = GetFvCells();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Form.xaml"), "Dt.LocalTbl.Res.EntityForm.xaml", dt);
        }

        async Task WriteTwoPane()
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$entityname$", _params.Entity },
                {"$entitytitle$", _params.Title },
                {"$time$", _params.Time },
                {"$username$", _params.UserName },
            };
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml"), "Dt.SingleTbl.Res.TwoPanWin.xaml", dt);
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Win.xaml.cs"), "Dt.SingleTbl.Res.TwoPanWin.xaml.cs", dt);

            if (_cbSearch.SelectedIndex == 1)
            {
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml"), "Dt.SingleTbl.Res.EntitySearch.xaml", dt);
                Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}Search.xaml.cs"), "Dt.SingleTbl.Res.EntitySearch.xaml.cs", dt);
            }

            // 搜索方式
            string cs = _cbSearch.SelectedIndex == 0 ? "TwoDefSearch" : "TwoCusSearch";
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.LocalTbl.Res.{cs}.cs")))
            {
                var txt = await sr.ReadToEndAsync();
                dt["$listsearchcs$"] = txt
                    .Replace("$entitytitle$", _params.Title)
                    .Replace("$entityname$", _params.Entity)
                    .Replace("$table$", _params.Table);

                if (_cbSearch.SelectedIndex == 0)
                {
                    dt["$listsearchcs$"] = dt["$listsearchcs$"].Replace("$blursql$", GetBlurSql());
                }
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml.cs"), "Dt.SingleTbl.Res.TwoPanList.xaml.cs", dt);
            dt.Remove("$listsearchcs$");

            dt["$lvbody$"] = GetLvItemTemplate();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml"), "Dt.SingleTbl.Res.TwoPanList.xaml", dt);
        }

        async Task WriteThreePane()
        {
            var dt = new Dictionary<string, string>
            {
                {"$rootnamespace$", _params.NameSpace },
                {"$entityname$", _params.Entity },
                {"$entitytitle$", _params.Title },
                {"$time$", _params.Time },
                {"$username$", _params.UserName },
            };
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
            using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.LocalTbl.Res.{listSearchCs}.cs")))
            {
                var txt = await sr.ReadToEndAsync();
                dt["$listsearchcs$"] = txt
                    .Replace("$entitytitle$", _params.Title)
                    .Replace("$entityname$", _params.Entity)
                    .Replace("$table$", _params.Table);

                if (_cbSearch.SelectedIndex == 0)
                {
                    dt["$listsearchcs$"] = dt["$listsearchcs$"].Replace("$blursql$", GetBlurSql());
                }
            }
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml.cs"), "Dt.SingleTbl.Res.EntityList.xaml.cs", dt);
            dt.Remove("$listsearchcs$");

            dt["$lvbody$"] = GetLvItemTemplate();
            Kit.WritePrjFile(Path.Combine(_path, $"{_params.Entity}List.xaml"), "Dt.SingleTbl.Res.EntityList.xaml", dt);
        }

        string GetFvCells()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var row in _schema)
            {
                if (row.Str("pk") == "1")
                    continue;

                if (sb.Length > 0)
                    sb.AppendLine();
                AppendTabSpace(sb, 2);

                string name = row.Str("name");

                // 只有text integer两种类型，无法区分
                sb.Append($"<a:CText ID=\"{name}\" Title=\"{name}\" />");
            }
            return sb.ToString();
        }

        string GetLvItemTemplate()
        {
            var sb = new StringBuilder();
            AppendTabSpace(sb, 3);
            sb.Append("<StackPanel Padding=\"10\">");
            foreach (var row in _schema)
            {
                if (row.Str("pk") == "1")
                    continue;

                sb.AppendLine();
                AppendTabSpace(sb, 4);
                sb.Append($"<a:Dot ID=\"{row.Str("name")}\" />");
            }
            sb.AppendLine();
            AppendTabSpace(sb, 3);
            sb.Append("</StackPanel>");
            return sb.ToString();
        }

        string GetBlurSql()
        {
            var sb = new StringBuilder($"select * from '{_params.Table}' where false");
            foreach (var row in _schema)
            {
                if (row.Str("pk") == "1")
                    continue;

                sb.Append(" or ");
                sb.Append(row.Str("name"));
                sb.Append(" like @input");
            }
            return sb.ToString();
        }

        static void AppendTabSpace(StringBuilder p_sb, int p_num)
        {
            for (int i = 0; i < p_num; i++)
            {
                p_sb.Append("    ");
            }
        }

        private void _cbTbls_SelectedIndexChanged(object sender, EventArgs e)
        {
            var tbl = _cbTbls.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(tbl))
            {
                _entityName.Text = tbl;
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Sqlite库文件(*.db)|*.db",
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _dbPath.Text = dlg.FileName;
                if (AtLocal.IsOpened)
                {
                    AtLocal.CloseDb();
                    _cbTbls.DataSource = null;
                }

                if (AtLocal.OpenDb(dlg.FileName))
                {
                    //select * from sqlite_master where type="table"
                    //PRAGMA table_info(BIOG_MAIN)

                    _cbTbls.DataSource = AtLocal.FirstCol<string>("select name from sqlite_master where type='table' and name!='sqlite_sequence'");
                }
            }
        }

        private void AddTooltip()
        {
            var tip = new ToolTip();
            tip.SetToolTip(linkLabel1, "选择sqlite库文件路径，用于查询所有表目录及表结构");
            tip.SetToolTip(linkLabel2, "表映射的实体类名，默认同名\r\n本次不生成实体类，请确保已存在");
            tip.SetToolTip(linkLabel4, "从所有表目录中选择表名\r\n根据表结构生成列表和表单xaml内容");
        }
    }
}
