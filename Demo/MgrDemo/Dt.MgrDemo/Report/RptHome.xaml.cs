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

namespace Dt.MgrDemo
{
    public partial class RptHome : Tab
    {
        public RptHome()
        {
            InitializeComponent();
            AttachEvent();
        }

        void AttachEvent()
        {
            foreach (var item in _fv.Items)
            {
                if (item is Panel pnl)
                {
                    foreach (Button btn in pnl.Children.OfType<Button>())
                    {
                        btn.Click += OnBtnClick;
                    }
                }
            }
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            string name = ((Button)sender).Content.ToString();
            var uri = $"embedded://Dt.MgrDemo/Dt.MgrDemo.Files.Embed.模板.{name}.rpt";
            if ((bool)_cb.IsChecked)
            {
                _ = Rpt.ShowDesign(new RptDesignInfo { Uri = uri, ShowNewFile = true, ShowOpenFile = true, ShowSave = true });
            }
            else
            {
                Rpt.Show(new RptInfo { Uri = uri });
            }
        }

        void OnTest(object sender, RoutedEventArgs e)
        {
            var txt = ResKit.GetText("模板.内部变量.rpt");
            Kit.Msg(txt);
        }
    }
}