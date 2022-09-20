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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class ExceptionDemo : Win
    {
        public ExceptionDemo()
        {
            InitializeComponent();
        }

        /// <para>.net6.0 maui抛异常规律：</para>
        /// <para>1. UI主线程同步方法中抛异常被.net内部拦截，不触发未处理异常事件</para>
        /// <para>2. UI主线程异步方法中抛异常，触发未处理异常事件</para>
        /// <para>3. Task内部异常，不管同步或异步都不触发未处理异常事件</para>
        /// <para>因为触发未处理异常事件的不确定性，无法统一处理，警告提示信息只能在抛出异常前显示</para>
        /// <para>.net6.0 maui中非KnownException类型的异常，在UI同步方法或后台抛出时都无法捕获！</para>

        #region 同步异常
        void ThrowIfSync(object sender, RoutedEventArgs e)
        {
            Throw.If(true, "业务条件true时的异常警告");
            int b = 12;
            b++;
        }

        void ThrowIfNullSync(object sender, RoutedEventArgs e)
        {
            TextBlock tb = null;
            Throw.IfNull(tb, "对象null时的异常警告");
        }
        
        void ThrowIfEmptySync(object sender, RoutedEventArgs e)
        {
            Throw.IfEmpty(null, "字符串空或null的异常警告");
        }

        void ThrowMsgSync(object sender, RoutedEventArgs e)
        {
            Throw.Msg("业务异常消息");
        }
        
        void ThrowUnhandleSync(object sender, RoutedEventArgs e)
        {
            throw new Exception("未处理异常信息");
        }
        #endregion

        #region 异步异常
        async void ThrowAsync(object sender, RoutedEventArgs e)
        {
            Throw.If(true, "异步业务异常警告");
            int b = 12;
            string str = "avb";
            b += str.Length;
            Kit.Msg(b.ToString());
            await Task.CompletedTask;
        }

        void ThrowTaskSync(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Throw.Msg("Task内同步业务异常"));
        }

        void ThrowTaskAsync(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                Throw.Msg("Task内异步业务异常");
                await Task.CompletedTask;
            });
        }

        async void ThrowUnhandleAsync(object sender, RoutedEventArgs e)
        {
            await Task.CompletedTask;
            throw new Exception("主线程异步未处理异常");
        }

        void ThrowUnhandleTaskSync(object sender, RoutedEventArgs e)
        {
            Task.Run(() => throw new Exception("Task内同步未处理异常"));
        }

        void ThrowUnhandleTaskAsync(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.CompletedTask;
                throw new Exception("Task内异步未处理异常");
            });
        }
        #endregion

        #region 服务端异常
        async void TestException(object sender, RoutedEventArgs e)
        {
            await AtTestCm.ThrowException();
        }

        async void TestRpcException(object sender, RoutedEventArgs e)
        {
            await AtTestCm.ThrowBusinessException();
        }

        async void TestPosException(object sender, RoutedEventArgs e)
        {
            await AtTestCm.ThrowPostionException();
        }
        #endregion
    }
}