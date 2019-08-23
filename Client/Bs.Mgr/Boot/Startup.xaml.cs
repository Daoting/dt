#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 启动页面：
    /// 1. 显示动画，
    /// 2. 更新打开模型库
    /// 3. 切换到主页或登录页
    /// </summary>
    public partial class Startup : Page
    {
        public Startup()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        async void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            string error = await AtApp.OpenModelDb("Mgr");
            if (!string.IsNullOrEmpty(error))
                _tb.Text = error;
            else
                await Login();
        }

        async Task Login()
        {
            string phone = AtLocal.GetCookie("LoginPhone");
            string pwd = AtLocal.GetCookie("LoginPwd");
            
            // 未登录
            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(pwd))
            {
                AtApp.Login();
                return;
            }

            Dict dt = await AtAuth.LoginByPwd(phone, pwd);

            // 登录失败
            if (!dt.Bool("valid"))
            {
                AtApp.Login();
                return;
            }

            // 切换到主页
            AtUser.Init(dt.Str("userid"), phone, dt.Str("name"));
            AtApp.LoadRootContent();
        }
    }
}
