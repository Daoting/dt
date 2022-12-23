using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dt.Editor
{
    public partial class CList : UserControl, ICellControl
    {
        public CList()
        {
            InitializeComponent();

            ToolTip tip = new ToolTip();
            tip.SetToolTip(_ex, "设置扩展CList功能的类名和参数，用于控制下拉对话框和数据源，类名和参数之间用#隔开");
        }

        string ICellControl.GetText()
        {
            StringBuilder sb = new StringBuilder("<a:CList");
            _header.GetText(sb);

            if (_selectionMode.SelectedIndex != 1)
                sb.Append($" SelectionMode=\"{_selectionMode.SelectedItem}\"");

            var txt = _ex.Text.Trim();
            if (txt != "")
                sb.Append($" Ex=\"{txt}\"");

            txt = _srcID.Text.Trim();
            if (txt != "")
                sb.Append($" SrcID=\"{txt}\"");

            txt = _tgtID.Text.Trim();
            if (txt != "")
                sb.Append($" TgtID=\"{txt}\"");

            if (_viewMode.SelectedIndex != 0)
                sb.Append($" ViewMode=\"{_viewMode.SelectedItem}\"");

            if (_refreshData.Checked)
                sb.Append(" RefreshData=\"True\"");

            if (_isEditable.Checked)
                sb.Append(" IsEditable=\"True\"");

            _footer.GetText(sb);

            if (_staticData.SelectedIndex != 0 || _customView.Checked)
            {
                sb.AppendLine(">");
                if (_customView.Checked)
                {
                    sb.AppendLine("<DataTemplate>\r\n<StackPanel Padding=\"10\">\r\n<a:Dot ID=\"x1\" />\r\n<a:Dot ID=\"x2\" Foreground=\"{StaticResource 深灰边框}\" />\r\n</StackPanel>\r\n</DataTemplate>");
                }

                if (_staticData.SelectedIndex == 1)
                {
                    sb.AppendLine("<a:CList.Items>\r\n<x:String>选项1</x:String>\r\n<x:String>选项2</x:String>\r\n</a:CList.Items>");
                }
                else if (_staticData.SelectedIndex == 2)
                {
                    sb.AppendLine("<a:CList.Items>\r\n<a:IDStr ID=\"1\" Str=\"男\" />\r\n<a:IDStr ID=\"0\" Str=\"女\" />\r\n</a:CList.Items>");
                }
                else if (_staticData.SelectedIndex == 3)
                {
                    sb.AppendLine("<a:CList.Items>\r\n<x:Int32>1</x:Int32>\r\n<x:Int32>2</x:Int32>\r\n</a:CList.Items>");
                }
                sb.AppendLine("</a:CList>");
            }
            else
            {
                sb.AppendLine(" />");
            }
            return sb.ToString();
        }

        void ICellControl.Reset()
        {
            _header.Reset();
            _footer.Reset();
            _selectionMode.SelectedIndex = 1;
            _ex.Text = "";
            _staticData.SelectedIndex = 0;
            _srcID.Text = "";
            _tgtID.Text = "";
            _refreshData.Checked = false;
            _isEditable.Checked = false;
            _viewMode.SelectedIndex = 0;
            _customView.Checked = false;
        }
    }
}
