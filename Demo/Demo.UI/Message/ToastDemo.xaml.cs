#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Demo.UI
{
    public sealed partial class ToastDemo : Win
    {
        public ToastDemo()
        {
            InitializeComponent();
        }
        
        void OnCommonToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("普通通知", "无启动参数\r\n" + DateTime.Now.ToString());
        }

        void OnParamsToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("带自启动参数的通知", "点击打开LvHome\r\n" + DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });
        }
    }
}
