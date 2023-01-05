using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class LvXaml : UserControl, ICmdForm
    {
        const string _tipItemHeight = "大于0时为固定行高，0时以第一行高度为准，NaN时自动调整高度(性能差)，默认0";

        public LvXaml()
        {
            InitializeComponent();

            _viewMode.SelectedIndex = 0;
            _selectionMode.SelectedIndex = 1;
            AtSvc.BindSvcUrl(_svcUrl);
            AddTooltip();
        }

        async Task<string> ICmdForm.GetText()
        {
            StringBuilder sb = new StringBuilder();
            var txt = _name.Text.Trim() == "" ? "_lv" : _name.Text.Trim();
            sb.Append($"<a:Lv x:Name=\"{txt}\"");

            if (_viewMode.SelectedIndex > 0)
                sb.Append($" ViewMode=\"{_viewMode.SelectedItem}\"");

            if (_selectionMode.SelectedIndex != 1)
                sb.Append($" SelectionMode=\"{_selectionMode.SelectedItem}\"");

            txt = _itemHeight.Text.Trim() == "" ? "0" : _itemHeight.Text.Trim();
            if (txt != "0")
                sb.Append($" ItemHeight=\"{txt}\"");

            txt = _groupName.Text.Trim();
            if (txt != "")
                sb.Append($" GroupName=\"{txt}\"");

            txt = _minItemWidth.Text.Trim();
            if (txt != "" && txt != "160")
                sb.Append($" MinItemWidth=\"{txt}\"");

            if (!_showGroupHeader.Checked)
                sb.Append(" ShowGroupHeader=\"False\"");

            if (!_showItemBorder.Checked)
                sb.Append(" ShowItemBorder=\"False\"");

            if (_autoScrollBottom.Checked)
                sb.Append(" AutoScrollBottom=\"True\"");

            sb.AppendLine(">");

            if (_showToolbar.Checked)
            {
                sb.AppendLine("<a:Lv.Toolbar>");
                sb.AppendLine("<a:Menu>");
                sb.AppendLine("<a:Mi ID=\"排序\" CmdParam=\"列名\" />");
                sb.AppendLine("</a:Menu>");
                sb.AppendLine("</a:Lv.Toolbar>");
            }

            if (_showFilter.Checked)
            {
                sb.AppendLine("<a:Lv.FilterCfg>");
                sb.AppendLine("<a:FilterCfg IsRealtime=\"True\" />");
                sb.AppendLine("</a:Lv.FilterCfg>");
            }

            string tbl = null;
            if (_cbTbls.SelectedItem != null)
                tbl = _cbTbls.SelectedItem.ToString();
            switch (_viewMode.SelectedIndex)
            {
                case 0:
                case 2:
                    if (string.IsNullOrEmpty(tbl))
                    {
                        sb.AppendLine("<DataTemplate>\r\n<StackPanel Padding=\"10\">\r\n<a:Dot ID=\"x1\" />\r\n<a:Dot ID=\"x2\" Foreground=\"{StaticResource 深灰边框}\" />\r\n</StackPanel>\r\n</DataTemplate>");
                    }
                    else
                    {
                        var xaml = await AtSvc.GetLvItemTemplate(tbl);
                        sb.AppendLine("<DataTemplate>");
                        sb.AppendLine(xaml);
                        sb.AppendLine("</DataTemplate>");
                    }
                    break;
                case 1:
                    if (string.IsNullOrEmpty(tbl))
                    {
                        sb.AppendLine("<a:Cols>\r\n<a:Col ID=\"xm\" Title=\"名称\" Width=\"120\" />\r\n</a:Cols>");
                    }
                    else
                    {
                        var xaml = await AtSvc.GetLvTableCols(tbl);
                        sb.AppendLine(xaml);
                    }
                    break;
            }
            sb.Append("</a:Lv>");
            return sb.ToString();
        }

        async void _cbTbls_DropDown(object sender, EventArgs e)
        {
            var ls = await AtSvc.GetAllTables();
            if (_cbTbls.DataSource != ls)
                _cbTbls.DataSource = ls;
        }

        void AddTooltip()
        {
            ToolTip tip = new ToolTip();
            tip.SetToolTip(_itemHeight, _tipItemHeight);
            tip.SetToolTip(linkLabel1, _tipItemHeight);
            tip.SetToolTip(_minItemWidth, "只磁贴视图有效！");
            tip.SetToolTip(linkLabel3, Kit.SvcUrlTip);
            tip.SetToolTip(linkLabel4, Kit.AllTblsTip);
        }

        // 编辑器中的光标位置
        //ThreadHelper.ThrowIfNotOnUIThread();
        //DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;
        //if (dte != null)
        //{
        //    var obj = dte.ActiveWindow.Selection as TextSelection;
        //    var col = obj.CurrentColumn;
        //    var l = obj.CurrentLine;
        //}
    }
}
