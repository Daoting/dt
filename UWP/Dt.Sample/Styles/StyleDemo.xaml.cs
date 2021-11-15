#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class StyleDemo : Win
    {
        public StyleDemo()
        {
            InitializeComponent();

            _sp.Children.Add(new TextBox { Text = "代码创建" });
        }

        void OnTest(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var a = _tb.ActualHeight;
            var b = new TextBox();
            var c = b.FontSize;
        }
    }
}
