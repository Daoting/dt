#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Sample
{
    public sealed partial class FlyoutDemo : Win
    {
        public FlyoutDemo()
        {
            InitializeComponent();
        }

        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_tb.Text.Contains("4"))
                _tb.Warn("文本框内容不可以含有4！");
        }

        void OnMsgClick(object sender, RoutedEventArgs e)
        {
            _btn.Msg("在目标元素上部显示提示信息，提示信息内容过长时换行");
        }
    }
}
