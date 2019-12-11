#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 新手向导
    /// </summary>
    public partial class GuidePage : Page
    {
        bool _clicked;

        public GuidePage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            VisualStateManager.GoToState(this, "Animated", true);
        }

        void OnNextClick(object sender, RoutedEventArgs e)
        {
            if (!_clicked)
            {
                VisualStateManager.GoToState(this, "Guide2", true);
                _clicked = true;
            }
            else
            {
                AtSys.Login(false);
            }
        }
    }
}
