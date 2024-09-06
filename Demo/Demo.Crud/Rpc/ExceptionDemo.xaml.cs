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

namespace Demo.Crud
{
    public partial class ExceptionDemo : Win
    {
        const string _msg = "异常未被中断！";

        public ExceptionDemo()
        {
            InitializeComponent();
        }

        // .net7.0 maui抛异常规律：
        // 1. UI主线程同步方法中抛异常被.net内部拦截处理，不触发未处理异常事件
        // 2. UI主线程异步方法中抛异常，触发未处理异常事件
        // 3. Task内部异常，不管同步或异步都不触发未处理异常事件
        // 因为触发未处理异常事件的不确定性，要想统一提供警告提示信息，只能在抛出KnownException异常前显示
        // 
        // WinAppSdk V1.2 都能触发未处理异常事件，已完美解决崩溃问题
        // 
        // 总结：所有平台都不会因为异常而崩溃，对于maui上的非KnownException类型异常，在UI同步方法或后台抛出时无法给出警告提示！

        #region 同步异常
        void ThrowIfSync(object sender, RoutedEventArgs e)
        {
            Throw.If(true, "业务条件true时的异常警告");
            Kit.Msg(_msg);
        }

        void ThrowIfNullSync(object sender, RoutedEventArgs e)
        {
            TextBlock tb = null;
            Throw.IfNull(tb, "对象null时的异常警告");
            Kit.Msg(_msg);
        }

        void ThrowIfEmptySync(object sender, RoutedEventArgs e)
        {
            Throw.IfEmpty(null, "字符串空或null的异常警告");
            Kit.Msg(_msg);
        }

        void ThrowMsgSync(object sender, RoutedEventArgs e)
        {
            Throw.Msg("业务异常消息");
            Kit.Msg(_msg);
        }

        void ThrowMethod(object sender, RoutedEventArgs e)
        {
            Throw.IfEmpty(null);
            Kit.Msg(_msg);
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
            Kit.Msg(_msg);
            await Task.CompletedTask;
        }

        async void ThrowMethodAsync(object sender, RoutedEventArgs e)
        {
            Throw.If(true);
            Kit.Msg(_msg);
            await Task.CompletedTask;
        }

        async void ThrowTaskSync(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Throw.Msg("Task内同步业务异常");
                Kit.Msg(_msg);
            }).Wait();

            Kit.Msg(_msg);
            await Task.CompletedTask;
        }

        void ThrowTaskAsync(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                Throw.Msg("Task内异步业务异常");
                Kit.Msg(_msg);
                await Task.CompletedTask;
            }).Wait();

            Kit.Msg(_msg);
        }

        void ThrowTaskMethod(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                Throw.If(true);
                Kit.Msg(_msg);
                await Task.CompletedTask;
            }).Wait();

            Kit.Msg(_msg);
        }

        async void ThrowUnhandleAsync(object sender, RoutedEventArgs e)
        {
            await Task.CompletedTask;
            throw new Exception("主线程异步未处理异常");
        }

        void ThrowUnhandleTaskSync(object sender, RoutedEventArgs e)
        {
            Task.Run(() => throw new Exception("Task内同步未处理异常")).Wait();
            Kit.Msg(_msg);
        }

        void ThrowUnhandleTaskAsync(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Task.CompletedTask;
                throw new Exception("Task内异步未处理异常");
            }).Wait();

            Kit.Msg(_msg);
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

        void OnCommonToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("普通通知", "无启动参数\r\n" + DateTime.Now.ToString());
        }

        void OnParamsToast(object sender, RoutedEventArgs e)
        {
            Kit.Toast("带自启动参数的通知", "点击打开[实体]\r\n" + DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(AccessDemo).AssemblyQualifiedName, Title = "实体" });
        }
    }
}