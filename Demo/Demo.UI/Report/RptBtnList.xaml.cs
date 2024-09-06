#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class RptBtnList : Tab
    {
        RptTab _tab;

        public RptBtnList()
        {
            InitializeComponent();
        }

        public void Init(int p_type, string p_title, RptTab p_tab = null)
        {
            Title = p_title;
            _tab = p_tab;

            if (p_type == 2)
                _fv.Items.RemoveAt(0);
            
            foreach (var item in _fv.Items)
            {
                if (item is Panel pnl)
                {
                    foreach (Button btn in pnl.Children.OfType<Button>())
                    {
                        if (p_type == 0)
                        {
                            btn.Click += OnShowInTab;
                        }
                        else if (p_type == 1)
                        {
                            btn.Click += OnShowInWin;
                        }
                        else
                        {
                            btn.Click += OnShowEditor;
                        }
                    }
                }
            }
        }

        void OnShowInTab(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            var uri = $"embedded://Demo.UI/Demo.UI.Files.Embed.模板.{name}.rpt";
            _tab.IsPdf = (bool)_cbPdf.IsChecked;
            _tab.LoadReport(new RptInfo { Uri = uri });
            NaviTo(_tab);
        }
        
        void OnShowInWin(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            var uri = $"embedded://Demo.UI/Demo.UI.Files.Embed.模板.{name}.rpt";
            Rpt.Show(new RptInfo { Uri = uri }, (bool)_cbPdf.IsChecked);
        }
        
        void OnShowEditor(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            var uri = $"embedded://Demo.UI/Demo.UI.Files.Embed.模板.{name}.rpt";
            _ = Rpt.ShowDesign(new RptDesignInfo { Uri = uri, ShowNewFile = true, ShowOpenFile = true, ShowSave = true });
        }
    }
}