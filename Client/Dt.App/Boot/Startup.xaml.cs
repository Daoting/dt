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

namespace Dt.App
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
            string error = await AtApp.OpenModelDb("cm");
            if (!string.IsNullOrEmpty(error))
                _tb.Text = error;
            else
                await Login();
        }

        async Task Login()
        {
            string phone = AtLocal.GetCookie("LoginPhone");
            string pwd = AtLocal.GetCookie("LoginPwd");

            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                Dict dt = await AtCm.LoginByPwd(phone, pwd);
                if (dt.Bool("valid"))
                {
                    MenuKit.Roles = dt.Str("roles").Split(',');
                    AtApp.LoginSuccess(dt.Str("userid"), phone, dt.Str("name"));
                    return;
                }
            }

            // 未登录或登录失败
            AtApp.Login();
        }
    }
}
