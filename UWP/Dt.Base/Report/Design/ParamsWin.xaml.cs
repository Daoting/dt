#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class ParamsWin : Win
    {
        RptDesignInfo _info;

        public ParamsWin(RptDesignInfo p_info)
        {
            InitializeComponent();
            _info = p_info;
            _info.TemplateChanged += (s, e) => LoadTbl();
            _info.Saved += OnSaved;
            LoadTbl();
            ((CList)_fv["val"]).Data = ValueExpression.Data;
        }

        void OnSaved(object sender, EventArgs e)
        {
            _info.Root.Params.Data.AcceptChanges();
        }

        void LoadTbl()
        {
            _lv.Data = _info.Root.Params.Data;
            _fv.Data = null;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            _fv.Data = e.Row;
            SelectTab("编辑");
        }

        protected override void OnInitPhoneTabs(PhoneTabs p_tabs)
        {
            if (_info.Root.Params.Data.Count == 0)
                p_tabs.Select("编辑");
        }

        void OnAdd(object sender, Mi e)
        {
            _fv.Data = _info.Root.Params.Data.AddRow(new { name = "新参数", type = "string" });
        }

        void OnDel(object sender, Mi e)
        {
            _lv.Table.Remove(_fv.Row);
            _fv.Data = null;
        }

        void OnCreatePreview(object sender, Mi e)
        {
            Fv fv = new Fv();
            _info.Root.Params.LoadFvCells(fv);
            _tab.Content = fv;
        }

        #region 单元格模板
        void OnCellChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1)
                return;

            switch (((ComboBoxItem)e.AddedItems[0]).Content.ToString())
            {
                case "CText":
                    _tbCell.Text = _ctext + _common;
                    break;
                case "CBool":
                    _tbCell.Text = _cbool + _common;
                    break;
                case "CNum":
                    _tbCell.Text = _cnum + _common;
                    break;
                case "CDate":
                    _tbCell.Text = _cdate + _common;
                    break;
                case "CList":
                    _tbCell.Text = _clist + _common;
                    break;
            }
        }

        const string _ctext =
            "<a:CText Title=\"标题\" />\n\n" +
            "属性：\n" +
            "AcceptsReturn=\"True\"\n" +
            "MaxLength=\"10\"\n";

        const string _cbool =
            "<a:CBool Title=\"标题\" />\n\n" +
            "属性：\n" +
            "IsSwitch=\"True\"\n" +
            "TrueVal=\"1\"\n" +
            "FalseVal=\"0\"\n";

        const string _cnum =
            "<a:CNum Title=\"标题\" />\n\n" +
            "属性：\n" +
            "IsInteger=\"True\"\n" +
            "Decimals=\"4\"\n" +
            "Maximum=\"100\"\n" +
            "Minimum=\"-100\"\n" +
            "NullValue=\"为空时的串\"\n";

        const string _cdate =
            "<a:CDate Title=\"标题\" />\n\n" +
            "属性：\n" +
            "Format=\"yyyy-MM-dd HH:mm:ss\"\n";

        const string _clist =
            "<a:CList Title=\"标题\" />\n\n" +
            "定义选项列表：\n" +
            "<a:CList Title=\"标题\">\n" +
            "  <a:CList.Items>\n" +
            "    <x:String>选项一</x:String>\n" +
            "    <x:String>选项二</x:String>\n" +
            "  </a:CList.Items>\n" +
            "</a:CList>\n\n" +
            "<a:CList Title=\"标题\" SrcID=\"id\" TgtID=\"sex\">\n" +
            "  <a:CList.Items>\n" +
            "    <a:IDStr ID=\"1\" Str=\"男\" />\n" +
            "    <a:IDStr ID=\"0\" Str=\"女\" />\n" +
            "  </a:CList.Items>\n" +
            "</a:CList>\n\n" +
            "属性：\n" +
            "IsEditable=\"True\"\n" +
            "Option=\"基础选项\"\n" +
            "Sql=\"Cm:select * from dt_log\"\n" +
            "Enum=\"Dt.Base.CtType,Dt.Base\"\n" +
            "RefreshData=\"True\"\n" +
            "SrcID=\"源属性列表，用#隔开\"\n" +
            "TgtID=\"目标属性列表，用#隔开\"\n";

        const string _common =
            "\n公共属性：\n" +
            "Title=\"标题\"\n" +
            "RowSpan=\"3\"\n" +
            "ShowTitle=\"False\"\n" +
            "TitleWidth=\"20\"\n" +
            "Placeholder=\"占位符文本\"\n" +
            "IsVerticalTitle=\"True\"\n" +
            "IsHorStretch=\"True\"\n" +
            "IsReadOnly=\"True\"";
        #endregion
    }
}
