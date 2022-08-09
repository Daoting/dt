using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        }

        private void _btnOK_Click(object sender, EventArgs e)
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

            _path = Kit.GetFolderPath();
            WriteEntityObj();

            //Kit.WritePrjFile(Path.Combine(path, $"{entity}Form.xaml"), "Dt.SingleTbl.EntityForm.xaml", dt);
            //Kit.WritePrjFile(Path.Combine(path, $"{entity}Form.xaml.cs"), "Dt.SingleTbl.EntityForm.xaml.cs", dt);

            //if (_cbWin.SelectedIndex == 0)
            //{
            //    // 三栏
            //    string listSearchCs;
            //    if (_cbSearch.SelectedIndex == 0)
            //    {
            //        // 通用搜索面板
            //        dt["$winsearchxaml$"] = $"                <a:SearchMv x:Name=\"_search\" Placeholder=\"{title}\" Search=\"OnSearch\">\r\n                    <x:String>全部</x:String>\r\n                </a:SearchMv>";
            //        dt["$winsearchcs$"] = "        public SearchMv Search => _search;\r\n\r\n        void OnSearch(object sender, string e)\r\n        {\r\n            _list.OnSearch(e);\r\n        }";
            //        listSearchCs = "EntityDefSearch";
            //    }
            //    else
            //    {
            //        // 自定义搜索面板
            //        dt["$winsearchxaml$"] = $"                <l:{entity}Search x:Name=\"_search\" Search=\"OnSearch\" />";
            //        dt["$winsearchcs$"] = $"        public {entity}Search Search => _search;\r\n\r\n        void OnSearch(object sender, Row e)\r\n        {{\r\n            _list.OnSearch(e);\r\n        }}";
            //        listSearchCs = "EntityCusSearch";

            //        Kit.WritePrjFile(Path.Combine(path, $"{entity}Search.xaml"), "Dt.SingleTbl.EntitySearch.xaml", dt);
            //        Kit.WritePrjFile(Path.Combine(path, $"{entity}Search.xaml.cs"), "Dt.SingleTbl.EntitySearch.xaml.cs", dt);
            //    }
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml"), "Dt.SingleTbl.EntityWin.xaml", dt);
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml.cs"), "Dt.SingleTbl.EntityWin.xaml.cs", dt);

            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml"), "Dt.SingleTbl.EntityList.xaml", dt);

            //    using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.SingleTbl.{listSearchCs}.cs")))
            //    {
            //        dt["$listsearchcs$"] = sr.ReadToEnd().Replace("$entitytitle$", title).Replace("$entityname$", entity).Replace("$agent$", agent);
            //    }
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml.cs"), "Dt.SingleTbl.EntityList.xaml.cs", dt);
            //}
            //else
            //{
            //    // 两栏
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml"), "Dt.SingleTbl.TwoPanWin.xaml", dt);
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}Win.xaml.cs"), "Dt.SingleTbl.TwoPanWin.xaml.cs", dt);

            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml"), "Dt.SingleTbl.TwoPanList.xaml", dt);

            //    string cs;
            //    if (_cbSearch.SelectedIndex == 0)
            //    {
            //        cs = "TwoDefSearch";
            //    }
            //    else
            //    {
            //        cs = "TwoCusSearch";
            //        Kit.WritePrjFile(Path.Combine(path, $"{entity}Search.xaml"), "Dt.SingleTbl.EntitySearch.xaml", dt);
            //        Kit.WritePrjFile(Path.Combine(path, $"{entity}Search.xaml.cs"), "Dt.SingleTbl.EntitySearch.xaml.cs", dt);
            //    }

            //    using (var sr = new StreamReader(Assembly.GetAssembly(typeof(Kit)).GetManifestResourceStream($"Dt.SingleTbl.{cs}.cs")))
            //    {
            //        dt["$listsearchcs$"] = sr.ReadToEnd().Replace("$entitytitle$", title).Replace("$entityname$", entity);
            //    }
            //    Kit.WritePrjFile(Path.Combine(path, $"{entity}List.xaml.cs"), "Dt.SingleTbl.TwoPanList.xaml.cs", dt);
            //}

            Close();
        }

        async void WriteEntityObj()
        {
            if (_isSelectTbl)
            {
                var entity = await AtSvc.CreateAgent(_cbTbls.SelectedItem.ToString());
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

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            if (_svcUrl.Text == AtSvc.SvcUrl && _cbTbls.DataSource != null)
                return;

            AtSvc.SvcUrl = _svcUrl.Text;
            var ls = await AtSvc.GetAllTables();
            if (ls != null && ls.Count > 0)
                ls.Insert(0, "");
            _cbTbls.DataSource = ls;
        }

        private void _cbTbls_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var tbl = _cbTbls.SelectedItem.ToString();
            _isSelectTbl = !string.IsNullOrEmpty(tbl);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ((LinkLabel)sender).ShowDataProviderTip();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ((LinkLabel)sender).ShowEntityTip();
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ((LinkLabel)sender).ShowSvcUrlTip();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ((LinkLabel)sender).ShowAllTblsTip();
        }

    }
}
