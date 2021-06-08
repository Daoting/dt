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
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            return new UnaryRpc(
                "cm",
                "TestAuth.NoAuth"
            ).Call<string>();
        }

        public static Task<string> Auth()
        {
            return new UnaryRpc(
                "cm",
                "TestAuth.Auth"
            ).Call<string>();
        }

        public static Task<string> CustomAuth()
        {
            return new UnaryRpc(
                "cm",
                "TestAuth.CustomAuth"
            ).Call<string>();
        }
        #endregion
    }
}