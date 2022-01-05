#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    public partial class AuthAccess : Win
    {
        public AuthAccess()
        {
            InitializeComponent();
        }

        async void OnNoAuth(object sender, RoutedEventArgs e)
        {
            Kit.Msg(await NoAuth());
        }

        async void OnAuth(object sender, RoutedEventArgs e)
        {
            Kit.Msg(await Auth());
        }

        async void OnCustomAuth(object sender, RoutedEventArgs e)
        {
            Kit.Msg(await CustomAuth());
        }

        #region TestAuth
        public static Task<string> NoAuth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.NoAuth"
            );
        }

        public static Task<string> Auth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.Auth"
            );
        }

        public static Task<string> CustomAuth()
        {
            return Kit.Rpc<string>(
                "cm",
                "TestAuth.CustomAuth"
            );
        }
        #endregion
    }
}