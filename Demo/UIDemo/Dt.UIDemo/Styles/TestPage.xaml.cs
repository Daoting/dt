#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public sealed partial class TestPage : Page
    {
        static Stub _stub;
        static LaunchActivatedEventArgs _args;

        public TestPage()
        {
            InitializeComponent();
        }

        void OnStart(object sender, RoutedEventArgs e)
        {
            _stub?.OnLaunched(_args);
        }

        /// <summary>
        /// 显示测试页面，在release版启动就崩溃又无法调试时用
        /// </summary>
        /// <param name="p_stub"></param>
        /// <param name="p_args"></param>
        public static void OnLaunched(Stub p_stub, LaunchActivatedEventArgs p_args)
        {
            _stub = p_stub;
            _args = p_args;

            var win = Microsoft.UI.Xaml.Window.Current;
            var frame = new Frame();
            win.Content = frame;
            frame.Navigate(typeof(TestPage));
            win.Activate();
        }
    }
}
