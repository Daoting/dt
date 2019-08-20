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
    public partial class RpcDemo : PageWin
    {
        public RpcDemo()
        {
            InitializeComponent();
        }

        async void OnGetString(object sender, RoutedEventArgs e)
        {
            _tbInfo.Text = "返回：" + await AtTestRpc.GetString();
        }

        async void OnSetString(object sender, RoutedEventArgs e)
        {
            if (await AtTestRpc.SetString("abc"))
                _tbInfo.Text = "OnSetString成功";
        }

        async void OnServerStream(object sender, RoutedEventArgs e)
        {
            _tbInfo.Text = "ServerStream模式：";
            var reader = await AtTestRpc.OnServerStream("hello");
            while (await reader.MoveNext())
            {
                _tbInfo.Text += $"{Environment.NewLine}收到：{reader.Val<string>()}";
            }
        }

        async void OnClientStream(object sender, RoutedEventArgs e)
        {
            _tbInfo.Text = "ClientStream模式：";
            var writer = await AtTestRpc.OnClientStream("hello");
            for (int i = 0; i < 50; i++)
            {
                var msg = $"hello {i}";
                await writer.Write(msg);
                _tbInfo.Text += $"{Environment.NewLine}写入：{msg}";
                await Task.Delay(1000);
            }
        }

        void OnDuplexStream(object sender, RoutedEventArgs e)
        {

        }
    }

    internal static class AtTestRpc
    {
        /// <summary>
        /// 返回字符串
        /// </summary>
        /// <returns></returns>
        public static Task<string> GetString()
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.GetString"
            ).Call<string>();
        }

        /// <summary>
        /// 字符串参数
        /// </summary>
        /// <param name="p_str"></param>
        public static Task<bool> SetString(string p_str)
        {
            return new UnaryRpc(
                "cm",
                "TestSerialize.SetString",
                p_str
            ).Call<bool>();
        }

        public static Task<ResponseReader> OnServerStream(string p_title)
        {
            return new ServerStreamRpc(
                "cm",
                "TestStreamRpc.OnServerStream",
                p_title
            ).Call();
        }

        public static Task<RequestWriter> OnClientStream(string p_title)
        {
            return new ClientStreamRpc(
                "cm",
                "TestStreamRpc.OnClientStream",
                p_title
            ).Call();
        }

        public static Task<DuplexStream> OnDuplexStream(string p_title)
        {
            return new DuplexStreamRpc(
                "cm",
                "TestStreamRpc.OnDuplexStream",
                p_title
            ).Call();
        }
    }
}