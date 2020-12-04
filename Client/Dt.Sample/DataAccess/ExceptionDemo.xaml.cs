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


        async void ThrowIf(object sender, RoutedEventArgs e)
        {
            Throw.If(true, "业务条件true时的异常警告");
            int b = 12;
            string str = "avb";
            b += str.Length;
            AtKit.Msg(b.ToString());
            await Task.CompletedTask;
        }

        void ThrowIfNull(object sender, RoutedEventArgs e)
        {
            TextBlock tb = null;
            Throw.IfNull(tb, "对象null时的异常警告");
        }

        void ThrowIfNullOrEmpty(object sender, RoutedEventArgs e)
        {
            Throw.IfNullOrEmpty(null, "字符串空或null的异常警告");
        }

        void ThrowMsg(object sender, RoutedEventArgs e)
        {
            Throw.Msg("直接业务异常消息");
        }

        void ThrowUnhandle(object sender, RoutedEventArgs e)
        {
            //Throw.If(true);
            Throw.IfNullOrEmpty(null);
        }

        class AtCm : DataProvider<cm>
        { }

        class cm { }

        #region 服务端异常
        async void TestException(object sender, RoutedEventArgs e)
        {
            //await AtTest.ThrowException();
            var ls = await AtCm.Query("select name,id,phone from cm_user");
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