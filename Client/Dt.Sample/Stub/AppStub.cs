#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Mgr;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 存根
    /// </summary>
    public class AppStub : DefaultStub
    {
        public AppStub()
        {
            Title = "搬运工";
            //SvcUrl = "http://10.10.1.16/dt-cm";
            LogSetting.FileEnabled = true;
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
            p_svcs.AddTransient<IPushApi, PushApi>();
            p_svcs.AddTransient<IReceiveShare, ReceiveShare>();
            p_svcs.AddTransient<IFixedMenus, FixedMenus>();
        }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected override async Task OnStartup()
        {
            AtLocal.OpenDb();

            // 初次运行，显示用户协议、隐私政策、向导
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PrivacyDlg("lob/DtAgreement.html", "lob/DtPrivacy.html").ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            // 以默认登录页、主页方式启动
            //await StartRun();

            // 以自定义方式启动
            await StartRun(typeof(Sample.SamplesMain), false);
        }

        #region 自动生成
        // 本地库结构变化或视图类型变化后，需通过《 win版app -> 系统日志 -> 存根 》重新生成！

        /// <summary>
        /// 视图名称与窗口类型的映射字典，菜单项用，同名时覆盖内置的视图类型
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, Type> GetViewTypes()
        {
            return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "样例", typeof(Dt.Sample.SamplesMain) },
                { "ShoppingWin", typeof(Dt.Sample.ModuleView.OneToMany1.ShoppingWin) },
            };
        }

        /// <summary>
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        /// <returns></returns>
        protected override Dictionary<string, SqliteTblsInfo> GetSqliteDbs()
        {
            return new Dictionary<string, SqliteTblsInfo>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    "local",
                    new SqliteTblsInfo
                    {
                        Version = "0a68f7fe86b78452e885c5e7394762ca",
                        Tables = new List<Type>
                        {
                            typeof(Dt.Sample.LocalDict),
                        }
                    }
                },
            };
        }
        #endregion
    }
}