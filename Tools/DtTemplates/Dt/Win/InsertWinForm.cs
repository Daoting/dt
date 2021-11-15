using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Dt
{
    public partial class InsertWinForm : Form
    {
        public InsertWinForm()
        {
            InitializeComponent();
            _ns.Text = Kit.GetNamespace();
            _cb.SelectedIndex = 4;
        }

        private void _btnOK_Click(object sender, EventArgs e)
        {
            string ns, cls;
            try
            {
                ns = Kit.GetText(_ns);
                cls = Kit.GetText(_cls);
            }
            catch
            {
                _lbl.Text = "当前内容不可为空！";
                return;
            }

            var dt = new Dictionary<string, string>
                {
                    {"$rootnamespace$", ns },
                    {"$safeitemname$", cls },
                    {"$time$", DateTime.Now.ToString("yyyy-MM-dd") },
                    {"$username$", Environment.UserName },
                };

            string res;
            switch (_cb.SelectedIndex)
            {
                case 0:
                    res = "Dt.Win.BlankWin.xaml";
                    break;
                case 1:
                    res = "Dt.Win.SingleWin.xaml";
                    break;
                case 2:
                    res = "Dt.Win.ToggleMainWin.xaml";
                    break;
                case 3:
                    res = "Dt.Win.TwoPanelWin.xaml";
                    break;
                default:
                    res = "Dt.Win.ThreePanelWin.xaml";
                    break;
            }

            var path = Kit.GetFolderPath();

            Kit.WritePrjFile(Path.Combine(path, $"{cls}.xaml"), res, dt);
            Kit.WritePrjFile(Path.Combine(path, $"{cls}.xaml.cs"), res + ".cs", dt);

            Close();
        }

        void _cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (_cb.SelectedIndex)
            {
                case 0:
                    _lbl.Text = "整个窗口内容为空，完全自定义，相当于空白页面";
                    break;
                case 1:
                    _lbl.Text = "只包括主区，主区有标题栏，等同于有标题栏的空白窗口";
                    break;
                case 2:
                    _lbl.Text = "包括左区和主区，主区内容支持UserControl、窗口及所有可视元素，一般通过左区操作联动来切换主区内容";
                    break;
                case 3:
                    _lbl.Text = "包括左区和主区，每个区都支持多Tab页，各Tab页之间在Windows模式可联动、Phone模式时可导航";
                    break;
                case 4:
                    _lbl.Text = "包括左区、主区、右区，每个区都支持多Tab页，各Tab页之间在Windows模式可联动、Phone模式时可导航";
                    break;
            }
        }
    }
}
