#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.DataTransfer;
#endregion

namespace Demo.UI
{
    public sealed partial class BrushDemo : Win
    {
        public BrushDemo()
        {
            InitializeComponent();
        }

        void OnCopy(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var txt = ((TextBlock)(btn.Parent as Grid).Children[0]).Text;

            DataPackage data = new DataPackage();
            data.SetText(txt);
            Clipboard.SetContent(data);
            Kit.Msg(string.Format("已复制：{0}", txt));
        }
    }
}
