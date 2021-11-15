#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Sample
{
    public sealed partial class TestDemo2 : Win
    {
        public TestDemo2()
        {
            InitializeComponent();
        }

        void OnTest(object sender, RoutedEventArgs e)
        {
            var btn = (BtnItem)XamlReader.Load("<a:BtnItem xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:a=\"using:Dt.Base\" Icon=\"个人信息\" Title=\"个人选项\" Desc=\"包括自定义参数设置，查看修改个人信息及密码等功能。\" />");
            Grid.SetRow(btn, 1);
            _grid.Children.Add(btn);
        }
    }
}
