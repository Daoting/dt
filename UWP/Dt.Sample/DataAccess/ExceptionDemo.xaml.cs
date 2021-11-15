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
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class ExceptionDemo : Win
    {
        public ExceptionDemo()
        {
            InitializeComponent();
        }

        // 注意uno中存在诡异现象，在UI主线程调用Throw抛出异常时，
        // 如在Button.Click事件方法中调用，若方法是同步，不catch也没能抛出未处理异常，方法加async就能正常抛出！

        async void ThrowIf(object sender, RoutedEventArgs e)
        {
            Throw.If(true, "业务条件true时的异常警告");
            int b = 12;
            string str = "avb";
            b += str.Length;
            Kit.Msg(b.ToString());
            await Task.CompletedTask;
        }

        async void ThrowIfNull(object sender, RoutedEventArgs e)
        {
            TextBlock tb = null;
            Throw.IfNull(tb, "对象null时的异常警告");
            await Task.CompletedTask;
        }

        async void ThrowIfNullOrEmpty(object sender, RoutedEventArgs e)
        {
            Throw.IfNullOrEmpty(null, "字符串空或null的异常警告");
            await Task.CompletedTask;
        }

        async void ThrowMsg(object sender, RoutedEventArgs e)
        {
            Throw.Msg("直接业务异常消息");
            await Task.CompletedTask;
        }

        async void ThrowUnhandle(object sender, RoutedEventArgs e)
        {
            //Throw.If(true);
            Throw.IfNullOrEmpty(null);
            await Task.CompletedTask;
        }

        #region 服务端异常
        async void TestException(object sender, RoutedEventArgs e)
        {
            await AtTest.ThrowException();
        }

        async void TestSerializeException(object sender, RoutedEventArgs e)
        {
            await AtTest.ThrowSerializeException();
        }

        async void TestRpcException(object sender, RoutedEventArgs e)
        {
            await AtTest.ThrowBusinessException();
        }

        async void TestPosException(object sender, RoutedEventArgs e)
        {
            await AtTest.ThrowPostionException();
        }
        #endregion
    }
}